using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public float xSensitivity;
    public float ySensitivity;

    public Transform orientation;//smjer u kojem gledam
    public Transform cameraPosition;

    public Transform player;

    float xRotation;
    float yRotation;

    private void Start()
    {
        transform.position = cameraPosition.position;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);//ogranicava rotaciju po x osi

        transform.position = cameraPosition.position;
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);//rotira kameru za vrijednost mouseX i mouseY

        player.rotation = Quaternion.Euler(0, yRotation, 0);//rotira Player objekt samo po y osi

    }
}
