using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotMovement : MonoBehaviour
{
    public float speed = 3.0f;
    public float obstacleRange = 5.0f;
    public int steps = 0;
    private float leftLegAngle = 3f;
    [SerializeField] private bool walking = false;


    //  Сила, тянущая "вверх" упавшего врага и заставляющая его вставать
    [SerializeField] float force = 5.0f;
    //  Угол отклонения, при котором начинает действовать "поднимающая" сила
    [SerializeField] float max_angle = 20.0f;

    [SerializeField] private GameObject leftLeg = null;
    [SerializeField] private GameObject rightLeg = null;
    [SerializeField] private GameObject leftLegJoint = null;
    [SerializeField] private GameObject rightLegJoint = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    /// <summary>
    /// Движение ног - болтаем туда-сюда
    /// </summary>
    void MoveLegs()
    {
        //  Движение ножек сделать
        if (steps >= 20)
        {
            leftLegAngle = -leftLegAngle;
            steps = -20;
        }
        steps++;

        leftLeg.transform.RotateAround(leftLegJoint.transform.position, transform.right, leftLegAngle);
        rightLeg.transform.RotateAround(rightLegJoint.transform.position, transform.right, -leftLegAngle);
    }

    // Update is called once per frame
    void Update()
    {
        //  Фрагмент кода, отвечающий за вставание
        var vertical_angle = Vector3.Angle(Vector3.up, transform.up);
        if (vertical_angle > max_angle)
        {
            GetComponent<Rigidbody>().AddForceAtPosition(force * Vector3.up, transform.position + 3.0f * transform.up, ForceMode.Force);
        };

        if (!walking) return;

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.SphereCast(ray, 0.75f, out hit))
            if (hit.distance < obstacleRange)
            {
                float angle = Random.Range(-110, 110);
                transform.Rotate(0, angle, 0);
                return;
            }

        transform.Translate(0, 0, speed * Time.deltaTime);
        MoveLegs();
    }
}
