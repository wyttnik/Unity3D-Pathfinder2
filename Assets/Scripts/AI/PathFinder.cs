using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace BaseAI
{
    /// <summary>
    /// Делегат для обновления пути - вызывается по завершению построения пути
    /// </summary>
    /// <param name="pathNodes"></param>
    /// /// <returns>Успешно ли построен путь до цели</returns>
    public delegate void UpdatePathListDelegate(List<PathNode> pathNodes);

    /// <summary>
    /// Глобальный маршрутизатор - сделать этого гада через делегаты и работу в отдельном потоке!!!
    /// </summary>
    public class PathFinder : MonoBehaviour
    {
        /// <summary>
        /// Объект сцены, на котором размещены коллайдеры
        /// </summary>
<<<<<<< HEAD
        [SerializeField] private GameObject CollidersCollection;
=======
        /// <param name="start">Начальная точка для поиска</param>
        /// <param name="destination">Целевой регион</param>
        /// <param name="movementProperties">Параметры движения</param>
        /// <returns>Список точек маршрута</returns>
        public List<PathNode> FindPath(PathNode start, IBaseRegion destination, MovementProperties movementProperties)
        {
            //  Реализовать что-то наподобие A* тут
            //  Можно попробовать и по-другому, например, с помощью NaviMesh. Только оно с динамическим регионом не сработает
            return new List<PathNode>();
        }
    }
>>>>>>> origin/master

        /// <summary>
<<<<<<< HEAD
        /// Картограф - класс, хранящий информацию о геометрии уровня, регионах и прочем
        /// </summary>
        [SerializeField] private Cartographer cartographer;

        /// <summary>
        /// Маска слоя с препятствиями (для проверки столкновений)
=======
        /// Объект сцены, на котором размещены коллайдеры
>>>>>>> origin/master
        /// </summary>
        private int obstaclesLayerMask;

        /// <summary>
        /// 
        /// </summary>
<<<<<<< HEAD
=======
        [SerializeField] private Cartographer сartographer;

        /// <summary>
        /// Маска слоя с препятствиями (для проверки столкновений)
        /// </summary>
        private int obstaclesLayerMask;

        /// <summary>
        /// 
        /// </summary>
>>>>>>> origin/master
        private float rayRadius;

        public PathFinder()
        {

<<<<<<< HEAD
        }

        /// <summary>
        /// Проверка того, что точка проходима. Необходимо обратиться к коллайдеру, ну ещё и проверить высоту над поверхностью
        /// </summary>
        /// <param name="node">Точка</param>
        /// <returns></returns>
        private bool CheckWalkable(ref PathNode node)
        {
            //  Сначала проверяем, принадлежит ли точка какому-то региону
            int regionInd = -1;
            //  Первая проверка - того региона, который в точке указан, это будет быстрее
            if(node.RegionIndex >= 0 && node.RegionIndex < cartographer.regions.Count)
            {
                if (cartographer.regions[node.RegionIndex].Contains(node))
                    regionInd = node.RegionIndex;
            };
            if(regionInd == -1)
            {
                var region = cartographer.GetRegion(node);
                if (region != null) regionInd = region.index;
            }
            if (regionInd == -1) return false;
            node.RegionIndex = regionInd;

            //  Следующая проверка - на то, что над поверхностью расстояние не слишком большое
            //  Технически, тут можно как-то корректировать высоту - с небольшим шагом, позволить объекту спускаться или подниматься
            //  Но на это сейчас сил уже нет. Кстати, эту штуку можно через коллайдеры попробовать сделать

            float distToFloor = node.Position.y - cartographer.SceneTerrain.SampleHeight(node.Position);
            if (distToFloor > 2.0f || distToFloor < 0.0f)
            {
                //Debug.Log("Incorrect node height");
                return false;
            }

            //  Ну и осталось проверить препятствия - для движущихся не сработает такая штука, потому что проверка выполняется для
            //  момента времени в будущем.
            //  Но из этой штуки теоретически можно сделать и для перемещающихся препятствий работу - надо будет перемещающиеся
            //  заворачивать в отдельный 

            //if (node.Parent != null && Physics.CheckSphere(node.Position, 2.0f, obstaclesLayerMask))
            //if (node.Parent != null && Physics.Linecast(node.Parent.Position, node.Position, obstaclesLayerMask))
            if (node.Parent != null && Physics.CheckSphere(node.Position, 1.0f, obstaclesLayerMask))
                return false;
            
            return true;
        }

        private static float Heur(PathNode node, PathNode target, MovementProperties properties)
        {
            //  Эвристику переделать - сейчас учитываются уже затраченное время, оставшееся до цели время и угол поворота
            float angle = Mathf.Abs(Vector3.Angle(node.Direction, target.Position - node.Position)) / properties.rotationAngle;
            return node.TimeMoment + 2 * node.Distance(target) / properties.maxSpeed + angle * properties.deltaTime;
        }

        /// <summary>
        /// Получение списка соседей для некоторой точки
        /// </summary>
        /// <param name="node"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public List<PathNode> GetNeighbours(PathNode node, MovementProperties properties)
        {
            //  Вот тут хардкодить не надо, это должно быть в properties
            //  У нас есть текущая точка, и свойства движения (там скорость, всякое такое)
            //float step = 1f;
            float step = properties.deltaTime * properties.maxSpeed;

            List<PathNode> result = new List<PathNode>();

            //  Внешний цикл отвечает за длину шага - либо 0 (остаёмся в точке), либо 1 - шагаем вперёд
            for (int mult = 0; mult <= 1; ++mult)
                //  Внутренний цикл перебирает углы поворота
                for (int angleStep = -properties.angleSteps; angleStep <= properties.angleSteps; ++angleStep)
                {
                    PathNode next = node.SpawnChildren(step * mult, angleStep * properties.rotationAngle, properties.deltaTime);

                    //  Точка передаётся по ссылке, т.к. возможно обновление региона, которому она принадлежит
                    if (CheckWalkable(ref next))
                    {
                        result.Add(next);
                        Debug.DrawLine(node.Position, next.Position, Color.blue, 10f);
                        if (next.RegionIndex == 1)
                            Debug.Log("First region visited");
                    }
                }
            //  Если регион node динамический, то трансформировать те точки, которые принадлежат тому же региону
            IBaseRegion region = cartographer.GetRegion(node);

            if (region != null && region.Dynamic)
            {
                //  Надо заставить регион преобразовать все точки в списке дочерних

            }

            return result;
        }

        /// <summary>
        /// Собственно метод построения пути
        /// Если ничего не построил, то возвращает null в качестве списка вершин
        /// </summary>
        private void FindPath(PathNode start, PathNode target, MovementProperties movementProperties, UpdatePathListDelegate updater, System.Func<PathNode, PathNode, bool> finishPredicate)
        {
            Debug.Log("Начато построение пути");

            //  Вот тут вместо equals надо использовать == (как минимум), а лучше измерять расстояние между точками
            //  сравнивая с некоторым epsilon. Хотя может это для каких-то специальных случаев?
            //  if (position.Position.Equals(target.Position)) return new List<PathNode>();
            if (Vector3.Distance(start.Position, target.Position) < movementProperties.epsilon)
            {
                updater(null);  //  Это функция, которая обновляет путь в агенте
                return;
            }

            Priority_Queue.SimplePriorityQueue<PathNode> opened = new Priority_Queue.SimplePriorityQueue<PathNode>();

            //  Тут тоже вопрос - а почему с 0 добавляем? Хотя она же сразу извлекается, не важно
            opened.Enqueue(start, 0);
            int steps = 0;

            //  Посещенные узлы (с некоторым шагом, аналог сетки)
            HashSet<(int, int, int, int)> closed = new HashSet<(int, int, int, int)>();
            closed.Add(start.ToGrid4DPoint(movementProperties.deltaDist, movementProperties.deltaTime));

            PathNode current = opened.First;

            while (opened.Count != 0 && steps < 4000)
            {
                steps++;
                current = opened.Dequeue();

                //  Тут сложная проверка - если извлечённая вершина находится в одном шаге от цели
                if(finishPredicate(current, target))
                //if (Vector3.Distance(current.Position, target.Position) <= movementProperties.epsilon)
                {
                    //  Последнюю точку заменяем целевой - чтобы в точности в неё прийти
                    //  current.Position = target.Position;
                    Debug.Log("Braked by closest point. Steps : " + steps.ToString());
                    break;
                }

                //  Получаем список соседей
                var neighbours = GetNeighbours(current, movementProperties);
                foreach (var nextNode in neighbours)
                {
                    var discreteNode = nextNode.ToGrid4DPoint(movementProperties.deltaDist, movementProperties.deltaTime);
                    if (!closed.Contains(discreteNode))
                    {
                        nextNode.H = Heur(nextNode, target, movementProperties);
                        opened.Enqueue(nextNode, nextNode.H);
                        closed.Add(discreteNode);
                    }
                }
            }

            if (finishPredicate(current, target) == false)
            {
                Debug.Log("Failed to build a way. Steps : " + steps.ToString());
                updater(null);
                return;
            }

            List<PathNode> result = new List<PathNode>();

            //  Восстанавливаем путь от целевой к стартовой
            //  Может, заменить последнюю на целевую, с той же отметкой по времени? Но тогда с поворотом сложновато получается
            while (current != null)
            {
                if(current.Parent != null) 
                    Debug.DrawLine(current.Position, current.Parent.Position, Color.red, 20f);
                result.Add(current);
                current = current.Parent;
            }

            result.Reverse();
            //result.RemoveAt(0);
            updater(result);

            Debug.Log("Финальная точка маршрута : " + result[result.Count-1].Position.ToString() + "; target : " + target.Position.ToString());
            return;

            //  Вызываем обновление пути. Теоретически мы обращаемся к списку из другого потока, надо бы синхронизировать как-то
=======
        }

        /// <summary>
        /// Проверка того, что точка проходима. Необходимо обратиться к коллайдеру, ну ещё и проверить высоту над поверхностью
        /// </summary>
        /// <param name="node">Точка</param>
        /// <returns></returns>
        private bool CheckWalkable(ref PathNode node)
        {
            //  Сначала проверяем, принадлежит ли точка какому-то региону
            int regionInd = -1;
            //  Первая проверка - того региона, который в точке указан, это будет быстрее
            if (node.RegionIndex >= 0 && node.RegionIndex < сartographer.regions.Count)
            {
                if (сartographer.regions[node.RegionIndex].Contains(node))
                    regionInd = node.RegionIndex;
            }
            else
            {
                var region = сartographer.GetRegion(node);
                if (region != null) regionInd = region.index;
            }
            if (regionInd == -1) return false;
            node.RegionIndex = regionInd;

            //  Следующая проверка - на то, что над поверхностью расстояние не слишком большое
            //  Технически, тут можно как-то корректировать высоту - с небольшим шагом, позволить объекту спускаться или подниматься
            //  Но на это сейчас сил уже нет. Кстати, эту штуку можно через коллайдеры попробовать сделать

            float distToFloor = node.Position.y - сartographer.SceneTerrain.SampleHeight(node.Position);
            if (distToFloor > 2.0f || distToFloor < 0.0f)
            {
                //Debug.Log("Incorrect node height");
                return false;
            }

            //  Ну и осталось проверить препятствия - для движущихся не сработает такая штука, потому что проверка выполняется для
            //  момента времени в будущем.
            //  Но из этой штуки теоретически можно сделать и для перемещающихся препятствий работу - надо будет перемещающиеся
            //  заворачивать в отдельный 

            //if (node.Parent != null && Physics.CheckSphere(node.Position, 2.0f, obstaclesLayerMask))
            //if (node.Parent != null && Physics.Linecast(node.Parent.Position, node.Position, obstaclesLayerMask))
            if (node.Parent != null && Physics.CheckSphere(node.Position, 1.0f, obstaclesLayerMask))
                return false;

            return true;
        }

        private static float Heur(PathNode node, PathNode target, MovementProperties properties)
        {
            //  Эвристику подобрать!!!! - сейчас учитываются уже затраченное время, оставшееся до цели время и угол поворота

            float angle = Mathf.Abs(Vector3.Angle(node.Direction, target.Position - node.Position)) / properties.rotationAngle;
            return node.TimeMoment + 2 * node.Distance(target) / properties.maxSpeed + angle * properties.deltaTime;
        }

        /// <summary>
        /// Получение списка соседей для некоторой точки
        /// </summary>
        /// <param name="node"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public List<PathNode> GetNeighbours(PathNode node, MovementProperties properties)
        {
            //  Вот тут хардкодить не надо, это должно быть в properties
            //  У нас есть текущая точка, и свойства движения (там скорость, всякое такое)
            //float step = 1f;
            float step = properties.deltaTime * properties.maxSpeed;

            List<PathNode> result = new List<PathNode>();

            //  Внешний цикл отвечает за длину шага - либо 0 (остаёмся в точке), либо 1 - шагаем вперёд
            for (int mult = 0; mult <= 1; ++mult)
                //  Внутренний цикл перебирает углы поворота
                for (int angleStep = -properties.angleSteps; angleStep <= properties.angleSteps; ++angleStep)
                {
                    PathNode next = node.SpawnChildren(step * mult, angleStep * properties.rotationAngle, properties.deltaTime);
                    //  Точка передаётся по ссылке, т.к. возможно обновление региона, которому она принадлежит
                    if (CheckWalkable(ref next))
                    {
                        result.Add(next);
                        Debug.DrawLine(node.Position, next.Position, Color.blue, 10f);
                    }
                }
            return result;
        }

        /// <summary>
        /// Собственно метод построения пути
        /// Если ничего не построил, то возвращает null в качестве списка вершин
        /// </summary>
        private bool FindPath(PathNode start, PathNode target, MovementProperties movementProperties, UpdatePathListDelegate updater)
        {
            /*
            List<PathNode> result = new List<PathNode>();

            updater(result);

            Debug.Log("Маршрут обновлён");
            Debug.Log("Финальная точка маршрута : " + result[result.Count-1].Position.ToString() + "; target : " + target.Position.ToString());
            return;
            */
            //  Вызываем обновление пути. Теоретически мы обращаемся к списку из другого потока, надо бы синхронизировать как-то
            return false;
>>>>>>> origin/master
        }

        /// <summary>
        /// Основной метод поиска пути, запускает работу в отдельном потоке. Аккуратно с асинхронностью - мало ли, вроде бы 
        /// потокобезопасен, т.к. не изменяет данные о регионах сценах и прочем - работает как ReadOnly
        /// </summary>
        /// <returns></returns>
        public bool BuildRoute(PathNode start, PathNode finish, MovementProperties movementProperties, UpdatePathListDelegate updater)
        {
<<<<<<< HEAD
            /*  Эта функция выполняет построение глобального пути. Её задача - определить, находятся ли 
             *  начальная и целевая точка в одном регионе. Если да, то просто запустить локальный
             *  маршрутизатор и построить маршрут в этом регионе.
             *  Иначе, если регионы разные - найти кратчайший маршрут между регионами (это задача
             *  глобального планировщика, и он должен вернуть регион, соседний с текущим – это регион, 
             *  в который должны шагнуть. После этого необходимо использовать другой вариант функции поиска пути - с другой эвристикой, в качестве которой можно использовать расстояние до центральной точки целевого региона, и другой функций проверки целевого состояния, вместо близости к некоторой точке надо проверять, достигли ли мы целевого региона. А кто умеет лямбды в C#?
             *  В целом тут можно банально использовать алгоритм Дейкстры. Можно немного усложнить, проверяя
             *  расстояние до границ текущего региона, и как-то до цели, но это уже улучшения. Вообще до 
             *  bounds можно это самое расстояние считать как-то. В базовой версии никаких особых извращений не нужно. Можно, конечно, и не Дейкстру, ну или модифицировать его немного.
            */

            //  Получить регион текущей позиции
            //  Если целевая в другом регионе - построить глобальный путь, получить следующий регион
            //  Построить маршрут до следующего региона

            IBaseRegion startRegion = cartographer.GetRegion(start);
            IBaseRegion finishRegion = cartographer.GetRegion(finish);
            if(startRegion != finishRegion)
            {
                //  Тут работает глобальный планировщик
                if (startRegion.index == 0) finishRegion = cartographer.regions[1];

                PathNode targetPoint = new PathNode(finishRegion.GetCenter(), Vector3.zero);
                Debug.Log("Going to center of closest region");
                FindPath(start, targetPoint, movementProperties, updater, (curPathNode, finPathNode) => cartographer.GetRegion(curPathNode) == finishRegion);
                return true;
            }

            //                 //if (Vector3.Distance(current.Position, target.Position) <= movementProperties.epsilon)

            FindPath(start, finish, movementProperties, updater, (curPathNode, finPathNode) => Vector3.Distance(curPathNode.Position, finPathNode.Position) <= movementProperties.epsilon);
            return true;
        }

=======
            //  Тут какие-то базовые проверки при необходимости, и запуск задачи построения пути в отдельном потоке
            //Task taskA = new Task(() => FindPath(start, finish, movementProperties, updater));
            //taskA.Start();
            //  Из функции выходим, когда путь будет построен - запустится делегат и обновит список точек
            FindPath(start, finish, movementProperties, updater);
            return true;
        }

>>>>>>> origin/master
        //// Start is called before the first frame update
        void Start()
        {
            //  Инициализируем картографа, ну и всё вроде бы
<<<<<<< HEAD
            cartographer = new Cartographer(CollidersCollection);
=======
            сartographer = new Cartographer(CollidersCollection);
>>>>>>> origin/master
            obstaclesLayerMask = 1 << LayerMask.NameToLayer("Obstacles");
            var rend = GetComponent<Renderer>();
            if (rend != null)
                rayRadius = rend.bounds.size.y / 2.5f;
        }

        //// Update is called once per frame
        //void Update()
        //{

        //}
    }
}
