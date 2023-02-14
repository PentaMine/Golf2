using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostpadController : MonoBehaviour
{
    // Start is called before the first frame update
    public float force;
    private Transform directionRef;
    private Vector3 baseForce;

    void Start()
    {
        force = 1f;
        // find the child object placed 10 units downrange to use it for calculating the force vector
        directionRef = transform.Find("direction");
        baseForce = new Vector3(directionRef.position.x - transform.position.x,
            0,
            directionRef.position.z - transform.position.z);
    }

    void OnTriggerEnter(Collider collisionInfo)
    {
        // launch the object if it enters the trigger area
        Rigidbody rb = collisionInfo.gameObject.GetComponent<Rigidbody>();
        rb.AddForce(baseForce * 600);
    }
}
