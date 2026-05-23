using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrujaController : MonoBehaviour
{
    [SerializeField] private GameManager gameScript;
    public bool onMouse = false;

    // --- NUEVAS VARIABLES ---
    [Header("Configuración de la Mirada")]
    [SerializeField] private Transform playerTransform; // Arrastra al jugador aquí desde el inspector
    [SerializeField] private float rotationSpeed = 5.0f; // Velocidad de rotación

    void Start()
    {
        // Si no asignaste al jugador en el inspector, lo buscamos por su Tag
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            MirarAlJugador3D();
        }
    }

    private void MirarAlJugador3D()
    {
        // Calculamos la dirección hacia el jugador
        Vector3 direction = playerTransform.position - transform.position;

        // Forzamos a que no mire hacia arriba o abajo (mantiene la bruja recta)
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            // Creamos la rotación hacia esa dirección
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Rotamos suavemente hacia el objetivo
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnMouseDown()
    {
        if (gameScript.index == 0 && gameScript.dialouguesText.text == string.Empty)
        {
            onMouse = true;
            gameScript.WitchDialogues();
        }
    }
}

