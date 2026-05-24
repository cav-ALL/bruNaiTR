using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private CharacterController charPlayer;

    [Header("Configuración de Velocidad")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 9f;
    [SerializeField] private float crouchSpeed = 2.5f;
    private float currentSpeed;

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

    [Header("Sistema de Audio y Pasos")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidosDePasos;

    [Tooltip("Tiempo mínimo en segundos entre cada pisada según el estado")]
    [SerializeField] private float cadenciaCaminar = 0.5f;
    [SerializeField] private float cadenciaCorrer = 0.3f;
    [SerializeField] private float cadenciaAgachado = 0.7f;

    [Tooltip("Volumen del sonido de 0.0 (mudo) a 1.0 (máximo)")]
    [Range(0f, 1f)][SerializeField] private float volumenCaminar = 0.6f;
    [Range(0f, 1f)][SerializeField] private float volumenCorrer = 1.0f;
    [Range(0f, 1f)][SerializeField] private float volumenAgachado = 0.25f;

    private float timerPasos = 0f;

    [Header("Audio de Fatiga (Respiración)")]
    [SerializeField] private AudioSource audioSourceRespiracion;
    [SerializeField] private AudioClip audioRespiracionFuerte;
    [Range(0f, 1f)][SerializeField] private float volumenRespiracion = 1.0f;

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
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (audioSourceRespiracion == null)
        {
            audioSourceRespiracion = gameObject.AddComponent<AudioSource>();
            audioSourceRespiracion.playOnAwake = false;
        }
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

        Vector3 move = (transform.right * moveX) + (transform.forward * moveZ);

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
        ManejarPasosModulados();
    }

    private void ActivarAgotamiento()
    {
        estaAgotado = true;
        audioSource.Stop();

        if (audioRespiracionFuerte != null && !audioSourceRespiracion.isPlaying)
        {
            audioSourceRespiracion.clip = audioRespiracionFuerte;
            audioSourceRespiracion.volume = volumenRespiracion;
            audioSourceRespiracion.loop = true;
            audioSourceRespiracion.Play();
        }
    }

    private void DesactivarAgotamiento()
    {
        estaAgotado = false;
        if (audioSourceRespiracion.isPlaying)
        {
            audioSourceRespiracion.Stop();
        }
    }

    private void ManejarPasosModulados()
    {
        if (estaAgotado)
        {
            timerPasos = cadenciaCaminar;
            return;
        }

        // Modificación: Solo suena pasos si está tocando físicamente el suelo
        if (charPlayer.isGrounded && charPlayer.velocity.magnitude > 0.5f)
        {
            timerPasos += Time.deltaTime;

            float intervaloActual = cadenciaCaminar;
            float volumenActual = volumenCaminar;

            if (isCrouching)
            {
                intervaloActual = cadenciaAgachado;
                volumenActual = volumenAgachado;
            }
            else if (isRunning)
            {
                intervaloActual = cadenciaCorrer;
                volumenActual = volumenCorrer;
            }

            if (timerPasos >= intervaloActual)
            {
                ReproducirPaso(volumenActual);
                timerPasos = 0f;
            }
        }
        else
        {
            timerPasos = cadenciaCaminar;
        }
    }

    private void ReproducirPaso(float volumen)
    {
        if (sonidosDePasos != null && !estaAgotado)
        {
            audioSource.pitch = Random.Range(0.88f, 1.12f);
            audioSource.PlayOneShot(sonidosDePasos, volumen);
        }
    }
}

