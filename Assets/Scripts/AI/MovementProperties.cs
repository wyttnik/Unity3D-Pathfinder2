using System;
using UnityEngine;

namespace BaseAI
{
    /// <summary>
    /// Параметры движения агента - как может поворачивать, какие шаги делать
    /// </summary>
    [Serializable]
    public class MovementProperties
    {
        /// <summary>
        /// Максимальная скорость движения агента
        /// </summary>
        public float maxSpeed;
        /// <summary>
        /// Максимальный угол поворота агента
        /// </summary>
        public float rotationAngle;
        /// <summary>
        /// Количество дискретных углов поворота
        /// </summary>
        public float angleSteps;
        /// <summary>
        /// Длина прыжка (фиксированная)
        /// </summary>
        public float jumpLength;
        /// <summary>
        /// эпсилон-окрестность точки, в пределах которой точка считается достигнутой
        /// </summary>
        public float epsilon = 0.1f;

    }



}
