using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullDownController : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(0, -150, 0));
    }
}
