using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed;
    public GameObject cam;
    private Vector2 prevPos;
    private Vector3 referenceEuler;
    private Vector3 newPos;
    private List<int> distances = new() { 20 };
    private int distanceIndex;


    void Start()
    {
        speed = .2f;
        prevPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        cam = Camera.main.gameObject;
        for (int i = 1; i < 30; i++)
        {
            distances.Add((int)Mathf.Pow(distances[i - 1], 1.02f));
            Debug.Log(i + " " + (int)Mathf.Pow(distances[i - 1], 1.02f));
        }
        
        distances.Reverse();
        distanceIndex = 18;

        transform.rotation = Quaternion.Euler(new Vector3(
            35,
            MathUtil.radiansToDegrees(Mathf.Atan2(transform.position.x, transform.position.z)) + 180,
            0
        ));
    }

    // Update is called once per frame
    void Update()
    {
        // this code rotates the camera
        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 mouseDelta = mousePos - prevPos;

        if (Input.GetMouseButtonDown(1))
        {
            referenceEuler = transform.rotation.eulerAngles;
        }

        if (Input.GetMouseButton(1) && mouseDelta != Vector2.zero)
        {
            Vector3 newRotation = new Vector3(-mouseDelta.y, mouseDelta.x, 0) * speed + referenceEuler;
            newRotation = getValidCamRotation(newRotation);
            transform.rotation = Quaternion.Euler(newRotation);

            referenceEuler = VectorUtil.getNormalisedEuler(transform.rotation.eulerAngles);
            prevPos = mousePos;
        }
        else
        {
            prevPos = mousePos;
        }
        
        // this moves the camera
        if (Input.mouseScrollDelta != Vector2.zero)
        {
            int newIndex = distanceIndex + (int)Input.mouseScrollDelta.y;
            distanceIndex = isValidDistanceIndex(newIndex) ? newIndex : distanceIndex;
        }
    }

    private void FixedUpdate()
    {
        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(0, 0, -distances[distanceIndex]), 0.04f);
        //Debug.Log(-distances[distanceIndex]);
    }

    bool isValidDistanceIndex(int distanceInd)
    {
        //Debug.Log(distanceInd);
        return distanceInd > 0 && distanceInd < distances.Count;
    }

    Vector3 getValidCamRotation(Vector3 euler)
    {   // return a version of the euler angle clamped between 90 and -90 deg
        euler = VectorUtil.getNormalisedEuler(euler);
        //Debug.Log(euler);
        euler.z = 0;
        if (euler.x is > 0 and < 90)
        {
            return euler;
        }
        else if (euler.x is >= 90 and < 180)
        {
            euler.x = 90;
            return euler;
        }
        else
        {
            euler.x = 0;
            return euler;
        }
    }
}