using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    public float camSpeed;
    public GameObject cam;
    private Vector2 prevPos;
    private Vector3 referenceEuler;
    private Vector3 newPos;
    private List<int> distances = new() { 20 };
    private int distanceIndex;
    

    void Start()
    {
        speed = .2f;
        camSpeed = 1f;
        prevPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        cam = Camera.main.gameObject;
        for (int i = 1; i < 22; i++)
        {
            distances.Add((int) Mathf.Pow(distances[i - 1], 1.02f));
            Debug.Log((int) Mathf.Pow(distances[i - 1], 1.02f));
        }
        distances.Reverse();
        distanceIndex = 18;
    }

    // Update is called once per frame
    void Update()
    {
        // this code moves the camera
        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 mouseDelta = mousePos - prevPos;
        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(0, 0, -distances[distanceIndex]), 0.02f);

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

        if (Input.mouseScrollDelta != Vector2.zero)
        {
            int newIndex = distanceIndex + (int)Input.mouseScrollDelta.y;
            distanceIndex = isValidDistanceIndex(newIndex) ? newIndex : distanceIndex;
            /*
            newPos = new Vector3(0, 0, Input.mouseScrollDelta.y) * camSpeed + cam.transform.localPosition;
            newPos.z = MathUtil.RoundToMultiple(newPos.z, camSpeed);
            Debug.Log(newPos);*/

            //cam.transform.localPosition = newPos;
        }
    }

    bool isValidDistanceIndex(int distanceInd)
    {
        return distanceInd is > 0 and < 21;
    }

    Vector3 getValidCamRotation(Vector3 euler)
    {
        // return a version of the euler angle clamped between 90 and -90 deg
        euler = VectorUtil.getNormalisedEuler(euler);
        euler.z = 0;
        if (euler.x is > 270 or < 90)
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
            euler.x = 270;
            return euler;
        }
    }
}