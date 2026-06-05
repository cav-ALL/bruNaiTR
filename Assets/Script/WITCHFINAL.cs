using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class WITCHFINAL : MonoBehaviour
{
    // Definimos los 3 estados lógicos de la bruja
    private enum EstadoBruja { Patrullando, PersecucionCercana, PersecucionLejana }
    [Header("Estado Actual")]
    [SerializeField] private EstadoBruja estadoActual = EstadoBruja.Patrullando;

    [Header("Componentes")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform playerTR;
    [SerializeField] private Animator witchAnim;

    [Header("Puntos de Ruta")]
    [SerializeField] private Transform SpawnPoint;
    [SerializeField] private Transform[] Patrulla;
    private int indexPos;

    [Header("Configuración de los 3 Rangos")]
    [Tooltip("Si el jugador está a esta distancia o menos, la bruja lo detecta y lo persigue.")]
    [SerializeField] private float rangoDeteccionCercana = 5f;

    [Tooltip("Distancia intermedia donde la bruja ignorará al jugador y se mantendrá patrullando.")]
    [SerializeField] private float rangoPatrullaNormal = 15f;

    [Tooltip("Si el jugador se aleja más que esta distancia, la bruja dejará la patrulla para perseguirlo de lejos.")]
    [SerializeField] private float rangoPersecucionLejana = 25f;

    [Header("Velocidades")]
    [SerializeField] private float witchSpeedP; // Velocidad de patrulla
    [SerializeField] private float witchSpeedD; // Velocidad de persecución

    [Header("Efectos y Escenario")]
    [SerializeField] private GameObject volumeOff;
    [SerializeField] private GameObject horribleSphere;
    [SerializeField] private GameObject screamer;
    [SerializeField] private GameObject[] ToDestroy;
    [SerializeField] private GameManager ManagerScript;
    [SerializeField] private bool witchMap;

    void Start()
    {
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
        // Actualizar la animación con la velocidad física del NavMesh
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

        // RANGO 3: El jugador está DEMASIADO lejos, modo persecución lejana para mantener la tensión
        if (playDis >= rangoPersecucionLejana)
        {
            if (estadoActual != EstadoBruja.PersecucionLejana)
            {
                estadoActual = EstadoBruja.PersecucionLejana;
                agent.speed = witchSpeedD;

                // Apagamos esferas/volúmenes de patrulla mientras corre hacia ti
                if (volumeOff != null) volumeOff.SetActive(false);
                if (horribleSphere != null) horribleSphere.SetActive(false);
            }
        }
        // RANGO 1: El jugador está DEMASIADO cerca, detección y persecución mortal
        else if (playDis <= rangoDeteccionCercana)
        {
            if (estadoActual != EstadoBruja.PersecucionCercana)
            {
                estadoActual = EstadoBruja.PersecucionCercana;
                agent.speed = witchSpeedD;

                if (volumeOff != null) volumeOff.SetActive(false);
                if (horribleSphere != null) horribleSphere.SetActive(false);
            }
        }
        // RANGO 2: El jugador está en la zona media segura, la bruja patrulla de forma normal
        else
        {
            // Si la bruja cambia desde una persecución hacia el estado de patrulla de vuelta
            if (estadoActual != EstadoBruja.Patrullando)
            {
                estadoActual = EstadoBruja.Patrullando;
                agent.speed = witchSpeedP;

                if (volumeOff != null) volumeOff.SetActive(true);
                if (horribleSphere != null) horribleSphere.SetActive(true);

                // IMPORTANTE: Busca el punto de ruta más cercano a su posición actual para retomar la ruta de forma fluida
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

                // Si llega a un nodo de patrulla, avanza al siguiente
                if (takeDis <= 1.5f)
                {
                    indexPos = (indexPos + 1) % Patrulla.Length;
                    ActualizarDestinoPatrulla();
                }
                break;

            case EstadoBruja.PersecucionCercana:
            case EstadoBruja.PersecucionLejana:
                // En ambos estados de persecución, su destino directo es el jugador
                agent.destination = playerTR.position;
                break;
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

