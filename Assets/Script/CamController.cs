using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    [SerializeField] private Transform playerTrans;
    [SerializeField] private float sensibility;

    [SerializeField] private float height;
    [SerializeField] private float heightCrouching;

    [SerializeField] private PlayerController playCont; // Tu referencia al jugador
    [SerializeField] private float xRotate;

    [Header("Configuración de Transición Suave")]
    [SerializeField] private float smoothSpeed = 8f; // Qué tan suave baja y sube la cámara

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Guardamos las posiciones locales iniciales para que la cámara se mueva con el jugador
        height = transform.localPosition.y;
        heightCrouching = height * 0.6f;
    }

    void Update()
    {
        // 1. ROTACIÓN DE LA CÁMARA (Tu código original)
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensibility;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensibility;

        xRotate -= mouseY;
        xRotate = Mathf.Clamp(xRotate, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotate, 0, 0);
        playerTrans.transform.Rotate(Vector3.up * mouseX);


        // 2. TRANSICIÓN SUAVE (Agacharse / Pararse)
        // Definimos la altura objetivo local según el estado del jugador
        float targetY = playCont.isCrouching ? heightCrouching : height;

        // Calculamos la nueva posición Y de forma fluida usando Lerp
        float newY = Mathf.Lerp(transform.localPosition.y, targetY, Time.deltaTime * smoothSpeed);

        // Aplicamos la nueva posición manteniendo intactos los ejes X y Z locales
        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
    }
}


