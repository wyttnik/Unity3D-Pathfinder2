﻿using BaseAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform1Movement : MonoBehaviour, BaseAI.IBaseRegion
{
    private Vector3 initialPosisition;
    [SerializeField] private bool moving;
    private Vector3 rotationCenter;
    private Vector3 rotationStartPos;
    [SerializeField] private float rotationSpeed = 1.0f;

    /// <summary>
    /// Тело региона - коллайдер
    /// </summary>
    public SphereCollider body;

    public IBaseRegion parent { get; set; } = null;

    /// <summary>
    /// Индекс региона в списке регионов
    /// </summary>
    public int index { get; set; } = -1;
    public float distance { get; set; } = float.PositiveInfinity;
    bool IBaseRegion.Dynamic { get; } = true;

    public IList<BaseAI.IBaseRegion> Neighbors { get; set; } = new List<BaseAI.IBaseRegion>();

    void Start()
    {
        rotationCenter = transform.position + 10 * Vector3.back;
        rotationStartPos = transform.position;
    }

    void Update()
    {
        if (!moving) return;

        transform.RotateAround(rotationCenter, Vector3.up, Time.deltaTime*rotationSpeed);
    }

    void IBaseRegion.TransformPoint(PathNode parent, PathNode node) {
        
        float timeDelta = node.TimeMoment - parent.TimeMoment;

        Vector3 dir = node.Position - rotationCenter;
        node.Position = rotationCenter + Quaternion.AngleAxis(-rotationSpeed * timeDelta, Vector3.up) * dir;
        node.Direction = Quaternion.AngleAxis(-rotationSpeed * timeDelta, Vector3.up) * node.Direction;
        return;
    }

    void IBaseRegion.TransformGlobalToLocal(PathNode node) 
    {
        //  Вот тут всё плохо - определяем момент времени, через который нам нужна точка
        float timeDelta = node.TimeMoment - Time.time;
        //  Откручиваем точку обратно в направлении, противоположном движению региона

        Vector3 dir = node.Position - rotationCenter;
        node.Position = rotationCenter + Quaternion.AngleAxis(-rotationSpeed * timeDelta, Vector3.up) * dir;
        node.Direction = Quaternion.AngleAxis(-rotationSpeed * timeDelta, Vector3.up) * node.Direction;
        //  Преобразуем в локальные координаты
        node.Position = transform.InverseTransformPoint(node.Position);
        node.Direction = transform.InverseTransformDirection(node.Direction);
        //  Всё вроде бы
    }

    bool IBaseRegion.Contains(PathNode node)
    {
        var coll = GetComponent<Collider>();
        if (coll != null && coll.bounds.Contains(node.Position) && !node.JumpNode) return true;
        //  Самая жуткая функция - тут думать надо
        //  Вывести точку через 2 секунды - положение платформы через 2 секунды в будущем
        float deltaTime = node.TimeMoment - Time.time;
        if (deltaTime < 0) return false;

        Vector3 dir = node.Position - rotationCenter;
        Vector3 newPoint = rotationCenter + Quaternion.AngleAxis(-rotationSpeed * deltaTime, Vector3.up) * dir;
        //  Осторожно! Тут два коллайдера у объекта, проверить какой именно вытащили.
        
        return coll != null && coll.bounds.Contains(newPoint);
    }

    Vector3 IBaseRegion.GetCenter()
    {
        return rotationCenter;
    }

    float IBaseRegion.SqrDistanceTo(PathNode node)
    {
        //  Вот тоже должно быть странно - как-то надо узнать, эта точка вообще попадает в коллайдер, 
        //  и если попадает, то когда? Может, тупо до центра области сделать? Сойдёт же!
        throw new System.NotImplementedException();
    }

    float IBaseRegion.TransferTime(IBaseRegion source, float transitStart, IBaseRegion dest)
    {
        //  Время перехода через регион - вроде бы несложно, можно даже захардкодить
        throw new System.NotImplementedException();
    }

    void IBaseRegion.AddTransferTime(IBaseRegion source, IBaseRegion dest)
    {
        throw new System.NotImplementedException();
    }
}
