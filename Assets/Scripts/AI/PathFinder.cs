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
    /// Локальный маршрутизатор - ищет маршруты от локальной точки какого-либо региона до указанного региона
    /// </summary>
    public class LocalPathFinder
    {
        /// <summary>
        /// Построение маршрута от заданной точки до ближайшей точки целевого региона
        /// </summary>
        /// <param name="start">Начальная точка для поиска</param>
        /// <param name="destination">Целевой регион</param>
        /// <param name="movementProperties">Параметры движения</param>
        /// <returns>Список точек маршрута</returns>
        public List<PathNode> FindPath(PathNode start, BaseRegion destination, MovementProperties movementProperties)
        {
            //  Реализовать что-то наподобие A* тут
            //  Можно попробовать и по-другому, например, с помощью NaviMesh. Только оно с динамическим регионом не сработает
            return new List<PathNode>();
        }
    }

    /// <summary>
    /// Глобальный маршрутизатор - сделать этого гада через делегаты и работу в отдельном потоке!!!
    /// </summary>
    public class PathFinder : MonoBehaviour
    {
        /// <summary>
        /// Пока что просто так вешаем отладочный объект, из которого можно вытащить путь
        /// </summary>
        public DebugHelper dbgHelper;

        /// <summary>
        /// Объект сцены, на котором размещены коллайдеры
        /// </summary>
        [SerializeField] private GameObject CollidersCollection;

        /// <summary>
        /// Картограф - класс, хранящий информацию о геометрии уровня, регионах и прочем
        /// </summary>
        [SerializeField] private Cartographer сartographer;
        
        public PathFinder()
        {
            
        }

        /// <summary>
        /// Собственно метод построения пути, который запускается в отдельном потоке
        /// </summary>
        private void PathfindingTask(PathNode start, PathNode finish, MovementProperties movementProperties, UpdatePathListDelegate updater)
        {
            //  Тут реализовать основную работу, пока что сделано заглушкой
            //  Моделируем паузу в построении пути - посмотрим, как себя ботик поведёт в этом случае
            System.Threading.Thread.Sleep(2000);
            Debug.Log("Пауза пройдена после построения маршрута");

            //  Вызываем обновление пути. Теоретически мы обращаемся к списку из другого потока, надо бы синхронизировать как-то
            updater(dbgHelper.GetRoute());
            Debug.Log("Маршрут обновлён");
        }

        /// <summary>
        /// Основной метод поиска пути, запускает работу в отдельном потоке. Аккуратно с асинхронностью - мало ли, вроде бы 
        /// потокобезопасен, т.к. не изменяет данные о регионах сценах и прочем - работает как ReadOnly
        /// </summary>
        /// <returns></returns>
        public bool FindPath(PathNode start, PathNode finish, MovementProperties movementProperties, UpdatePathListDelegate updater)
        {
            //  Тут какие-то базовые проверки при необходимости, и запуск задачи построения пути в отдельном потоке
            Task taskA = new Task(() => PathfindingTask(start, finish, movementProperties, updater));
            taskA.Start();
            //  Из функции выходим, когда путь будет построен - запустится делегат и обновит список точек
            return true;
        }




        //// Start is called before the first frame update
        void Start()
        {
            //  Инициализируем картографа, ну и всё вроде бы
            сartographer = new Cartographer(CollidersCollection);
        }

        //// Update is called once per frame
        //void Update()
        //{

        //}
    }
}