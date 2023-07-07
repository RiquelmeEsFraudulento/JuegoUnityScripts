using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    private GameObject playerObject;
    private Transform playerTransform;

    public Transform cameraPosition; // Reference to the camera position

    float xRotation;
    float yRotation;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.GetComponent<Transform>();

            cameraPosition = playerTransform.Find("CameraPos");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerObject != null && playerTransform != null && cameraPosition != null)
        {
            // Set the camera position to the player's camera position
            transform.position = playerTransform.position + playerTransform.rotation * cameraPosition.position;
            transform.rotation = playerTransform.rotation * cameraPosition.rotation;

            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

            yRotation += mouseX;
            xRotation -= mouseY;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
            playerTransform.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }

}
