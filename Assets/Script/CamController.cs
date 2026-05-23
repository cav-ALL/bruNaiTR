using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    [SerializeField] private Transform playerTrans;
    [SerializeField] private float sensibility;

    [SerializeField] private float xRotate;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensibility;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensibility;

        xRotate -= mouseY;
        xRotate = Mathf.Clamp(xRotate, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotate, 0, 0);
        playerTrans.transform.Rotate(Vector3.up * mouseX);
    }
}
