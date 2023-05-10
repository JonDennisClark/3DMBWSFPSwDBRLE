using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSens = 100f;
    public Transform playerBody;
    float xRotation = 0;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; 
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;
        //Moving the camera along the x axis should also rotate the player around the y axis
        playerBody.Rotate(Vector3.up * mouseX);

        //Allows for vertical mouse camera control.
        xRotation -= mouseY;
        //Prevents the camera from "overlapping" itself.
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        //The use of localRotation ensures the tranform is based on where the player is currently looking.
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
    }
}
