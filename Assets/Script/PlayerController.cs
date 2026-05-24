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

    [Header("Sistema de Estamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrain = 25f;
    [SerializeField] private float staminaRegen = 15f;
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

        // Validar el estado de agotamiento estricto
        if (currentStamina <= 0 && !estaAgotado)
        {
            ActivarAgotamiento();
        }

        if (estaAgotado && currentStamina >= maxStamina)
        {
            DesactivarAgotamiento();
        }

        // Asignación limpia de velocidad según estado
        if (isCrouching)
        {
            isRunning = false;
            currentSpeed = crouchSpeed;
        }
        else if (wantsToRun && !estaAgotado) // Mientras corras normal, NO entra aquí el estado agotado
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

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        // 3. APLICAR LA ALTURA AL CHARACTER CONTROLLER
        float targetHeight = isCrouching ? crouchingHeight : standingHeight;
        charPlayer.height = Mathf.MoveTowards(charPlayer.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
        charPlayer.center = new Vector3(0, charPlayer.height / 2f, 0);

        // 4. MOVIMIENTO
        float moveX = Input.GetAxis("Horizontal") * currentSpeed;
        float moveZ = Input.GetAxis("Vertical") * currentSpeed;

        Vector3 move = (transform.right * moveX) + (transform.forward * moveZ);

        if (charPlayer.isGrounded)
        {
            verticalVelocity = -0.5f;
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        move.y = verticalVelocity;
        charPlayer.Move(move * Time.deltaTime);

        // 5. EJECUCIÓN DE LOS PASOS DINÁMICOS
        ManejarPasosModulados();
    }

    private void ActivarAgotamiento()
    {
        estaAgotado = true;

        // Apagamos los pasos de inmediato al caer exhausto
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
        // Si está agotado (sin estamina), bloqueamos por completo los pasos
        if (estaAgotado)
        {
            timerPasos = cadenciaCaminar;
            return;
        }

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

