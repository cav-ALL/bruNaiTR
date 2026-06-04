using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public CharacterController charPlayer;


    [Header("Configuración de Velocidad")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 9f;
    [SerializeField] private float crouchSpeed = 2.5f;
    private float currentSpeed;
    public Vector3 move;

    [Header("Mecánica de Salto")]
    [SerializeField] private float jumpHeight = 2.5f; // Altura máxima del salto en unidades de Unity
    [SerializeField] private float saltoStaminaCost = 15f; // Estamina que cuesta saltar

    [Header("Sistema de Estamina")]
    [SerializeField] private float maxStamina = 200f;
    [SerializeField] private float staminaDrain = 10f;
    [SerializeField] private float staminaRegen = 25f;
    [SerializeField] private float staminaCooldown = 1.5f;

    private float currentStamina;
    private float cooldownTimer;
    private bool isRunning;
    private bool estaAgotado = false;

    [Header("Configuración de Agacharse")]
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float crouchingHeight = 1.0f;
    [SerializeField] private float crouchTransitionSpeed = 10f;
    public bool isCrouching = false;

    private float verticalVelocity;

    // Propiedades públicas
    public bool IsRunning => isRunning;
    public bool IsCrouching => isCrouching;
    public float StaminaPercentage => currentStamina / maxStamina;

    void Start()
    {
        currentStamina = maxStamina;
        currentSpeed = walkSpeed;

        if (charPlayer == null) charPlayer = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 1. INPUT DE AGACHARSE
        isCrouching = Input.GetKey(KeyCode.LeftControl);

        // 2. CONTROL DE ESTAMINA, SPRINT Y VELOCIDAD
        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        bool wantsToRun = Input.GetKey(KeyCode.LeftShift) && isMoving;

        if (currentStamina <= 0 && !estaAgotado)
        {
            ActivarAgotamiento();
        }

        if (estaAgotado && currentStamina >= maxStamina)
        {
            DesactivarAgotamiento();
        }

        if (isCrouching)
        {
            isRunning = false;
            currentSpeed = crouchSpeed;
        }
        else if (wantsToRun && !estaAgotado)
        {
            isRunning = true;
            currentSpeed = runSpeed;
            currentStamina -= staminaDrain * Time.deltaTime;
            cooldownTimer = staminaCooldown;
        }
        else
        {
            isRunning = false;
            currentSpeed = walkSpeed;

            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
            }
            else if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegen * Time.deltaTime;
            }
        }

        // 3. MECÁNICA DE SALTO (Solo si está en el suelo, no está agachado y no está agotado)
        if (Input.GetButtonDown("Jump") && charPlayer.isGrounded && !isCrouching && !estaAgotado)
        {
            // Ecuación física estándar para saltar basándose en la altura deseada: v = sqrt(h * -2 * g)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);

            // Consumo de estamina por el esfuerzo físico del salto
            currentStamina -= saltoStaminaCost;
            cooldownTimer = staminaCooldown;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        // 4. APLICAR LA ALTURA AL CHARACTER CONTROLLER
        float targetHeight = isCrouching ? crouchingHeight : standingHeight;
        charPlayer.height = Mathf.MoveTowards(charPlayer.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
        charPlayer.center = new Vector3(0, charPlayer.height / 2f, 0);

        // 5. MOVIMIENTO Y GRAVEDAD
        float moveX = Input.GetAxis("Horizontal") * currentSpeed;
        float moveZ = Input.GetAxis("Vertical") * currentSpeed;

        move = (transform.right * moveX) + (transform.forward * moveZ);

        if (charPlayer.isGrounded)
        {
            // Si la velocidad vertical es menor a cero (cayendo o estático), la mantenemos baja para no perder el suelo
            if (verticalVelocity < 0)
            {
                verticalVelocity = -0.5f;
            }
        }
        else
        {
            // Aplicar gravedad en el aire de forma continua
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        move.y = verticalVelocity;
        charPlayer.Move(move * Time.deltaTime);

        // 6. EJECUCIÓN DE LOS PASOS DINÁMICOS
    }

    private void ActivarAgotamiento()
    {
        estaAgotado = true;
    }

    private void DesactivarAgotamiento()
    {
        estaAgotado = false;
    }

}

