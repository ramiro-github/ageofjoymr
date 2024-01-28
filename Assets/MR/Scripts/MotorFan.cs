using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorFan : MonoBehaviour
{
    
    public float rotationSpeed = 0;
    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

    }
}
