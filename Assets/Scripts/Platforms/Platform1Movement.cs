using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform1Movement : MonoBehaviour
{
    private Vector3 initialPosisition;
    [SerializeField] private bool moving;
    private Vector3 rotationCenter;
    [SerializeField] private float rotationSpeed = 1.0f;


    void Start()
    {
        rotationCenter = transform.position + 10 * Vector3.left;
    }

    void Update()
    {
        //  Если нет компонента Rigidbody - то и движение не реализуется
        if (!moving) return;

        transform.RotateAround(rotationCenter, Vector3.up, rotationSpeed);

    }
}
