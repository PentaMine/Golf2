using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingIconContreoller : MonoBehaviour
{
    void Start()
    {
        InvokeRepeating("Rotate", 0, .075f);
    }

    void Rotate()
    {
        // rotate the icon 30 deg
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, transform.rotation.eulerAngles.z - 30));
    }
}
