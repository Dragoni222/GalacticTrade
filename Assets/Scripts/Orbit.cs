using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{

    Rigidbody rb;
    Rigidbody parentRb;
    void Start()
    {
        int[] direction = new int[] { -1, 1 };
        int positiveX = direction[UnityEngine.Random.Range(0, 2)];
        int positiveZ = direction[UnityEngine.Random.Range(0, 2)];
        transform.localPosition = new Vector3(UnityEngine.Random.Range(15f, 25f) * positiveX, 0, UnityEngine.Random.Range(15f, 25f) * positiveZ);
        rb = GetComponent<Rigidbody>();
        parentRb = transform.parent.GetComponent<Rigidbody>();
        Vector3 towardsParent = transform.parent.position - transform.position;
        Vector3 rotated = Quaternion.AngleAxis(90, Vector3.up) * towardsParent;
        rb.AddForce(rotated * UnityEngine.Random.Range(5f, 10f));




    }

    void FixedUpdate()
    {
        Vector3 towardsParent = transform.parent.position - transform.position;
        towardsParent = (float)(1 / Math.Pow(towardsParent.magnitude, 2)) * towardsParent.normalized * parentRb.mass * rb.mass;
        rb.AddForce(towardsParent * 0.001f);
    }
}
