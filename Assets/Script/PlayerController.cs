using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController charPlayer;

    [Header("Configuración de Velocidad")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 9f;
    private float currentSpeed;

    [Header("Sistema de Estamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrain = 25f;    // Cuánta estamina gasta por segundo al correr
    [SerializeField] private float staminaRegen = 15f;    // Cuánta estamina recupera por segundo
    [SerializeField] private float staminaCooldown = 1.5f; // Tiempo de espera para volver a regenerar tras cansarse

    private float currentStamina;
    private float cooldownTimer;
    private bool isRunning;

    // Propiedades públicas para que la cámara pueda leerlas
    public bool IsRunning => isRunning;
    public float StaminaPercentage => currentStamina / maxStamina;

    void Start()
    {
        currentStamina = maxStamina;
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        // 1. CONTROL DE ESTAMINA Y SPRINT
        // Detectamos si presiona el Shift Izquierdo y si se está moviendo hacia adelante o los lados
        bool wantsToRun = Input.GetKey(KeyCode.LeftShift) && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0);

        if (wantsToRun && currentStamina > 0)
        {
            isRunning = true;
            currentSpeed = runSpeed;
            currentStamina -= staminaDrain * Time.deltaTime;
            cooldownTimer = staminaCooldown; // Reseteamos el tiempo de espera para regenerar
        }
        else
        {
            isRunning = false;
            currentSpeed = walkSpeed;

            // Manejo de la regeneración con cooldown
            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
            }
            else if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegen * Time.deltaTime;
            }
        }

        // Aseguramos que la estamina no se salga de los límites (0 a maxStamina)
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        // 2. MOVIMIENTO (Tu código original adaptado a la velocidad actual)
        float moveX = Input.GetAxis("Horizontal") * currentSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * currentSpeed * Time.deltaTime;

        Vector3 move = moveX * transform.right + moveZ * transform.forward;

        charPlayer.Move(move);
    }
}
