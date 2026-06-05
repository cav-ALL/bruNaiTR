using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class WITCHFINAL : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform playerTR;
    [SerializeField] private Transform[] Patrulla;
    [SerializeField] private GameObject[] ToDestroy;
    [SerializeField] private GameObject screamer;

    [SerializeField] private float[] distance;
    [SerializeField] private bool patroll;
    [SerializeField] private bool detected;
    [SerializeField] private int indexPos;
    [SerializeField] Transform SpawnPoint;

    [SerializeField] private GameObject volumeOff;
    [SerializeField] private GameObject horribleSphere;

    [SerializeField] private GameManager ManagerScript;
    [SerializeField] private bool witchMap;

    void Start()
    {
        if (witchMap)
        {
            indexPos = 0;

            if (agent != null)
            {
                agent.Warp(SpawnPoint.position);
            }
            else
            {
                transform.position = SpawnPoint.position;
            }

            patroll = true;
            detected = false;

            ActualizarDestinoPatrulla();
        }
    }

    void Update()
    {
        float playDis = Vector3.Distance(transform.position, playerTR.position);

        if (witchMap)
        {
            if (patroll)
            {
                float takeDis = Vector3.Distance(transform.position, Patrulla[indexPos].position);

                if (takeDis <= 1.5f)
                {
                    indexPos++;
                    if (indexPos >= Patrulla.Length)
                    {
                        indexPos = 0;
                    }
                    ActualizarDestinoPatrulla();
                }

                if (playDis <= distance[0])
                {
                    patroll = false;
                    detected = true;
                    horribleSphere.SetActive(false);
                }
            }
            else if (detected)
            {
                agent.destination = playerTR.position;

                if (volumeOff.activeSelf)
                {
                    volumeOff.SetActive(false);
                    horribleSphere.SetActive(false);
                }

                if (playDis >= distance[1])
                {
                    patroll = true;
                    detected = false;
                    volumeOff.SetActive(true);
                    horribleSphere.SetActive(true);
                    ActualizarDestinoPatrulla();
                }
            }
        }
        else
        {
            agent.destination = playerTR.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLAYER"))
        {
            screamer.SetActive(true);
            for (int i = 0; i < ToDestroy.Length; i++)
            {
                if (ToDestroy[i] != null)
                {
                    Destroy(ToDestroy[i]);
                }
            }
        }
    }

    private void ActualizarDestinoPatrulla()
    {
        if (volumeOff.activeSelf == false)
        {
            horribleSphere.SetActive(true);
        }

        agent.destination = Patrulla[indexPos].position;
    }
}
