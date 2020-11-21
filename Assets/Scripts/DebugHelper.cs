using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseAI
{

    /// <summary>
    /// Класс, созданный с одной целью - генерация отладочного списка точек для движения бота, и не более.
    /// В нормальном проекте его надо будет вырезать
    /// </summary>
    public class DebugHelper : MonoBehaviour
    {
        private List<PathNode> pathNodes;

        /// <summary>
        /// Список объектов, через которые бот будет двигаться - выставляются в Unity для отладки
        /// </summary>
        [SerializeField] private List<GameObject> wayPoints;


        /// <summary>
        /// Список объектов, через которые бот будет двигаться - выставляются в Unity для отладки
        /// </summary>
        [SerializeField] private List<Collider> regions;

        // На старте однократно определить список точек маршрута
        void Start()
        {
            Debug.Log("Debug helper created!");
            pathNodes = new List<PathNode>();
            //  Для каждой точки списка формируем объект класса PathNode

            //  movementSript.plannedPath.Add(new BaseAI.PathNode(point.transform.position));
            for (int i = 0; i < wayPoints.Count; ++i)
            {
                pathNodes.Add(new BaseAI.PathNode(wayPoints[i].transform.position, Vector3.zero));
                pathNodes[pathNodes.Count - 1].TimeMoment = 5.3f * i;
            }
        }
        
        /// <summary>
        /// Получить маршрут для движения
        /// </summary>
        /// <returns></returns>
        public List<PathNode> GetRoute()
        {
            return pathNodes;
        }

    }
}