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

    [SerializeField] private LayerMask player;
    [SerializeField] private float[] distance;
    [SerializeField] private bool patroll;
    [SerializeField] private bool detected;
    void Start()
    {
        Patroll();
    }

    // Update is called once per frame
    void Update()
    {
        float playDis = Vector3.Distance(transform.position,playerTR.transform.position);
        if (patroll)
        {
            Patroll();
        }

        if (playDis <= distance[0])
        {
            patroll = false;
            detected = true;
        }

        if (detected)
        {
            Detected();
        }

        if (playDis >= distance[1]&&detected)
        {
            patroll = true;
            detected = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLAYER"))
        {
            screamer.SetActive(true);
            for (int i = 0; i < ToDestroy.Length; i++)
            {
                Destroy(ToDestroy[i]);
            }
        }
    }

    private void Patroll()
    {
        for(int i = 0; i < Patrulla.Length; i++)
        {
            float takeDis = Vector3.Distance(transform.position, Patrulla[i].position);
            if (takeDis > 0.5f)
            {
                agent.destination = Patrulla[i].position;
            }

            if (i <= Patrulla.Length - 1)
            {
                i = 0;
            }
        }
    }

    public void Detected()
    {
        agent.destination = playerTR.transform.position;
    }
}
