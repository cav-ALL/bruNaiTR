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
    void Start()
    {
        indexPos = 0;
        Patroll();
    }

    // Update is called once per frame
    void Update()
    {
        float playDis = Vector3.Distance(transform.position,playerTR.transform.position);
        if (patroll)
        {
            Patroll();
            if (playDis <= distance[0])
            {
                patroll = false;
                detected = true;
            }
        }
      
        else if (detected)
        {
            Detected();
            if (playDis >= distance[1])
            {
                patroll = true;
                detected = false;
            }
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

        agent.destination = Patrulla[indexPos].transform.position;

        float takeDis = Vector3.Distance(transform.position, Patrulla[indexPos].position);

        if (takeDis <= 0.5f)
        {
            indexPos++;
            if (indexPos >= Patrulla.Length)
            {
                indexPos = 0;
            }
        }
    }

    public void Detected()
    {
        agent.destination = playerTR.transform.position;
    }
}
