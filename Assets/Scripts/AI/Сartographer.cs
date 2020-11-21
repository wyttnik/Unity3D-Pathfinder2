using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// По-хорошему это октодерево должно быть, но неохота.
/// Класс, владеющий полной информацией о сцене - какие области где расположены, 
/// как связаны между собой, и прочая информация.
/// Должен по координатам точки определять номер области.
/// </summary>

namespace BaseAI
{
    /// <summary>
    /// Базовый класс для реализации региона - квадратной или круглой области
    /// </summary>
    abstract public class BaseRegion
    {
        /// <summary>
        /// Индекс региона - соответствует индексу элемента в списке регионов
        /// </summary>
        public int index = -1;

        /// <summary>
        /// Список соседних регионов (в которые можно перейти из этого)
        /// </summary>
        public List<BaseRegion> Neighbors { get; set; } = new List<BaseRegion>();
        
        /// <summary>
        /// Принадлежит ли точка региону (с учётом времени)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        abstract public bool Contains(PathNode node);
        
        /// <summary>
        /// Квадрат расстояния до ближайшей точки региона (без учёта времени)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        abstract public float SqrDistanceTo(PathNode node);

        /// <summary>
        /// Время перехода через область насквозь, от одного до другого 
        /// </summary>
        /// <param name="source">Регион, с границы которого стартуем</param>
        /// <param name="transitStart">Глобальное время начала перехода</param>
        /// <param name="dest">Регион назначения - ближайшая точка</param>
        /// <returns>Глобальное время появления в целевом регионе</returns>
        abstract public float TransferTime(BaseRegion source, float transitStart, BaseRegion dest);
    }

    /// <summary>
    /// Сферический регион на основе SphereCollider
    /// </summary>
    public class SphereRegion : BaseRegion
    {
        /// <summary>
        /// Тело региона - коллайдер
        /// </summary>
        public SphereCollider body;
        
        public SphereRegion(int RegionIndex, Vector3 Position, float Radius)
        {
            body = new SphereCollider
            {
                center = Position,
                radius = Radius
            };
            index = RegionIndex;
        }
        
        /// <summary>
        /// Квадрат расстояния до региона (минимально расстояние до границ коллайдера в квадрате)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        override public float SqrDistanceTo(PathNode node) { return body.bounds.SqrDistance(node.Position); }
        /// <summary>
        /// Проверка принадлежности точки региону
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        override public bool Contains(PathNode node) { return body.bounds.Contains(node.Position); }

        /// <summary>
        /// Время перехода через область насквозь, от одного до другого 
        /// </summary>
        /// <param name="source">Регион, с границы которого стартуем</param>
        /// <param name="transitStart">Глобальное время начала перехода</param>
        /// <param name="dest">Регион назначения - ближайшая точка</param>
        /// <returns>Глобальное время появления в целевом регионе</returns>
        override public float TransferTime(BaseRegion source, float transitStart, BaseRegion dest) {
            throw new System.NotImplementedException();
        }
    }
    
    /// <summary>
    /// Сферический регион на основе BoxCollider
    /// </summary>
    public class BoxRegion : BaseRegion
    {
        /// <summary>
        /// Тело коллайдера для представления региона
        /// </summary>
        public BoxCollider body;
        
        /// <summary>
        /// Создание региона с кубическим коллайдером в качестве основы
        /// </summary>
        /// <param name="RegionIndex"></param>
        /// <param name="position"></param>
        /// <param name="size"></param>
        public BoxRegion(int RegionIndex, Vector3 Position, Vector3 Size)
        {
            body = new BoxCollider
            {
                center = Position,
                size = Size
            };
            index = RegionIndex;
        }
        
        /// <summary>
        /// Квадрат расстояния до региона (минимально расстояние до границ коллайдера в квадрате)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        override public float SqrDistanceTo(PathNode node) { return body.bounds.SqrDistance(node.Position); }
        
        /// <summary>
        /// Проверка принадлежности точки региону
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        override public bool Contains(PathNode node) { return body.bounds.Contains(node.Position); }
        
        /// <summary>
        /// Время перехода через область насквозь, от одного до другого 
        /// </summary>
        /// <param name="source">Регион, с границы которого стартуем</param>
        /// <param name="transitStart">Глобальное время начала перехода</param>
        /// <param name="dest">Регион назначения - ближайшая точка</param>
        /// <returns>Глобальное время появления в целевом регионе</returns>
        override public float TransferTime(BaseRegion source, float transitStart, BaseRegion dest)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Cartographer
    {
        //  Список регионов
        public List<BaseRegion> regions = new List<BaseRegion>();

        // Start is called before the first frame update
        void Start()
        {
            //  Создаём региончики
            regions.Add(new BoxRegion(0, new Vector3(21f, 11f, 25f), new Vector3(40f, 2f, 49f)));
            regions.Add(new BoxRegion(1, new Vector3(50f, 11f, 10f), new Vector3(14f, 2f, 20f)));
            regions.Add(new BoxRegion(2, new Vector3(78f, 11f, 23f), new Vector3(42f, 2f, 46f)));
            regions.Add(new SphereRegion(3, new Vector3(8.79f, 10f, 50f), 3.5f));
            regions.Add(new SphereRegion(4, new Vector3(99.2f, 10f, 20.9f), 3.5f));

            //  Настраиваем связи между регионами - не самая лучшая идея, но для крупных регионов сойдёт
            regions[0].Neighbors.Add(regions[1]);
            regions[0].Neighbors.Add(regions[3]);

            regions[1].Neighbors.Add(regions[0]);
            regions[1].Neighbors.Add(regions[2]);

            regions[2].Neighbors.Add(regions[1]);
            regions[2].Neighbors.Add(regions[4]);

            regions[3].Neighbors.Add(regions[0]);

            regions[4].Neighbors.Add(regions[2]);


            //  Платформы потом
        }

        /// <summary>
        /// Регион, которому принадлежит точка. Сделать абы как
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Индекс региона, -1 если не принадлежит (не проходима)</returns>
        BaseRegion GetRegion(PathNode node)
        {
            for (var i = 0; i < regions.Count; ++i)
                //  Метод полиморфный и для всяких платформ должен быть корректно в них реализован
                if (regions[i].Contains(node))
                    return regions[i];
            return null;
        }
    }
}