using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class WITCHFINAL : MonoBehaviour
{
    private enum EstadoBruja { Patrullando, PersecucionCercana, PersecucionLejana }
    [Header("Estado Actual")]
    [SerializeField] private EstadoBruja estadoActual = EstadoBruja.Patrullando;

    [Header("Componentes")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform playerTR;
    [SerializeField] private Animator witchAnim;

    [Header("Configuración de Audio (Fuentes y Clips)")]
    [SerializeField] private AudioSource audioDeteccion;
    [SerializeField] private AudioClip clipDeteccion;
    [Space(10)]
    [SerializeField] private AudioSource musicaPersecucion;
    [SerializeField] private AudioClip clipMusicaPersecucion;

    [Header("Puntos de Ruta")]
    [SerializeField] private Transform SpawnPoint;
    [SerializeField] private Transform[] Patrulla;
    private int indexPos;

    [Header("Configuración de los 3 Rangos")]
    [SerializeField] private float rangoDeteccionCercana = 5f;
    [SerializeField] private float rangoPatrullaNormal = 15f;
    [SerializeField] private float rangoPersecucionLejana = 25f;

    [Header("Velocidades")]
    [SerializeField] private float witchSpeedP;
    [SerializeField] private float witchSpeedD;

    [Header("Efectos y Escenario")]
    [SerializeField] private GameObject volumeOff;
    [SerializeField] private GameObject horribleSphere;
    [SerializeField] private GameObject screamer;
    [SerializeField] private GameObject[] ToDestroy;
    [SerializeField] private GameManager ManagerScript;
    [SerializeField] private bool witchMap;

    void Start()
    {
        // Configuración y asignación de audio por código para evitar errores
        AsignarClipsDeAudio();

        if (witchMap)
        {
            indexPos = 0;

            if (agent != null)
                agent.Warp(SpawnPoint.position);
            else
                transform.position = SpawnPoint.position;

            estadoActual = EstadoBruja.Patrullando;
            agent.speed = witchSpeedP;

            ActualizarDestinoPatrulla();
        }
    }

    void Update()
    {
        if (witchAnim != null && agent != null)
        {
            witchAnim.SetFloat("Speed", agent.velocity.magnitude);
        }

        if (!witchMap)
        {
            agent.destination = playerTR.position;
            return;
        }

        float playDis = Vector3.Distance(transform.position, playerTR.position);

        // =================================================================
        // MAQUINA DE ESTADOS: EVALUACIÓN DE DISTANCIAS
        // =================================================================

        // RANGO 3: Persecución Lejana
        if (playDis >= rangoPersecucionLejana)
        {
            if (estadoActual != EstadoBruja.PersecucionLejana)
            {
                if (estadoActual == EstadoBruja.Patrullando) ActivarAudiosPersecucion();

                estadoActual = EstadoBruja.PersecucionLejana;
                agent.speed = witchSpeedD;

                if (volumeOff != null) volumeOff.SetActive(false);
                if (horribleSphere != null) horribleSphere.SetActive(false);
            }
        }
        // RANGO 1: Persecución Cercana
        else if (playDis <= rangoDeteccionCercana)
        {
            if (estadoActual != EstadoBruja.PersecucionCercana)
            {
                if (estadoActual == EstadoBruja.Patrullando) ActivarAudiosPersecucion();

                estadoActual = EstadoBruja.PersecucionCercana;
                agent.speed = witchSpeedD;

                if (volumeOff != null) volumeOff.SetActive(false);
                if (horribleSphere != null) horribleSphere.SetActive(false);
            }
        }
        // RANGO 2: Zona media (Patrulla)
        else
        {
            if (estadoActual != EstadoBruja.Patrullando)
            {
                DesactivarAudiosPersecucion();

                estadoActual = EstadoBruja.Patrullando;
                agent.speed = witchSpeedP;

                if (volumeOff != null) volumeOff.SetActive(true);
                if (horribleSphere != null) horribleSphere.SetActive(true);

                indexPos = ObtenerPuntoPatrullaMasCercano();
                ActualizarDestinoPatrulla();
            }
        }

        // =================================================================
        // EJECUCIÓN DE COMPORTAMIENTOS
        // =================================================================
        switch (estadoActual)
        {
            case EstadoBruja.Patrullando:
                float takeDis = Vector3.Distance(transform.position, Patrulla[indexPos].position);

                if (takeDis <= 1.5f)
                {
                    indexPos = (indexPos + 1) % Patrulla.Length;
                    ActualizarDestinoPatrulla();
                }
                break;

            case EstadoBruja.PersecucionCercana:
            case EstadoBruja.PersecucionLejana:
                agent.destination = playerTR.position;
                break;
        }
    }

    // =================================================================
    // GESTIÓN DE AUDIO
    // =================================================================
    private void AsignarClipsDeAudio()
    {
        // Asignamos el clip de susto al primer AudioSource
        if (audioDeteccion != null && clipDeteccion != null)
        {
            audioDeteccion.clip = clipDeteccion;
            audioDeteccion.loop = false; // El susto NO va en bucle
            audioDeteccion.playOnAwake = false;
        }

        // Asignamos el clip de música al segundo AudioSource y lo ponemos en bucle
        if (musicaPersecucion != null && clipMusicaPersecucion != null)
        {
            musicaPersecucion.clip = clipMusicaPersecucion;
            musicaPersecucion.loop = true; // La persecución SÍ va en bucle
            musicaPersecucion.playOnAwake = false;
            musicaPersecucion.Stop(); // Nos aseguramos de que empiece apagada
        }
    }

    private void ActivarAudiosPersecucion()
    {
        if (audioDeteccion != null && !audioDeteccion.isPlaying)
        {
            audioDeteccion.Play();
        }

        if (musicaPersecucion != null && !musicaPersecucion.isPlaying)
        {
            musicaPersecucion.Play();
        }
    }

    private void DesactivarAudiosPersecucion()
    {
        if (musicaPersecucion != null && musicaPersecucion.isPlaying)
        {
            musicaPersecucion.Stop();
        }

        if (audioDeteccion != null && audioDeteccion.isPlaying)
        {
            audioDeteccion.Stop();
        }
    }

    private int ObtenerPuntoPatrullaMasCercano()
    {
        int indiceMasCercano = 0;
        float distanciaMinima = Mathf.Infinity;

        for (int i = 0; i < Patrulla.Length; i++)
        {
            if (Patrulla[i] != null)
            {
                float dist = Vector3.Distance(transform.position, Patrulla[i].position);
                if (dist < distanciaMinima)
                {
                    distanciaMinima = dist;
                    indiceMasCercano = i;
                }
            }
        }
        return indiceMasCercano;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLAYER"))
        {
            // Apagamos todo el ruido de persecución antes del screamer
            DesactivarAudiosPersecucion();

            if (screamer != null) screamer.SetActive(true);

            for (int i = 0; i < ToDestroy.Length; i++)
            {
                if (ToDestroy[i] != null) Destroy(ToDestroy[i]);
            }
        }
    }

    private void ActualizarDestinoPatrulla()
    {
        if (horribleSphere != null && (volumeOff == null || volumeOff.activeSelf))
        {
            horribleSphere.SetActive(true);
        }

        if (Patrulla.Length > 0 && Patrulla[indexPos] != null)
        {
            agent.destination = Patrulla[indexPos].position;
        }
    }
}

