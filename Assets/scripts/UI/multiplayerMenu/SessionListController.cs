using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionListController : MonoBehaviour
{
    public static SessionListController controller;

    private void Start()
    {
        controller = this;
    }

    public void AddSession(Golf2ApiWrapper.Session session)
    {
        
    }
}
