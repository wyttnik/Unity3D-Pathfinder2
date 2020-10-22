using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetUp : MonoBehaviour
{
    [SerializeField] float force = 5.0f;
    [SerializeField] float max_angle = 20.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //  AddForceAtPosition

        var angle = Vector3.Angle(Vector3.up, transform.up);
        if (angle > max_angle)
        {
            GetComponent<Rigidbody>().AddForceAtPosition(force*Vector3.up, transform.position + transform.up, ForceMode.Force);
            Debug.Log("Angle : " + angle.ToString());
            //GetComponent<Rigidbody>().Sleep();
            //Quaternion target = Quaternion.Euler(Vector3.up);
            //transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
        };
    }
}
