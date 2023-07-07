using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private GameObject playerObject;
    private Transform cameraTransform;

    private void Start()
    {
        playerObject = GameObject.FindWithTag("Player");

        cameraTransform = GetComponent<Transform>();
    }

    private void Update()
    {
        if (playerObject != null)
        {
            cameraTransform.position = playerObject.transform.position;
        }
    }
}
