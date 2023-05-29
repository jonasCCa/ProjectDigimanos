using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Vector3 offset;
    Vector3 lookDirection;

    void FixedUpdate()
    {
        //transform.LookAt(Camera.main.transform);

        lookDirection = Camera.main.transform.position;
        lookDirection.x = this.transform.position.x;
        transform.LookAt(lookDirection);

        transform.Rotate(offset);
    }
}