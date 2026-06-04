using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstHorrorEvent : MonoBehaviour
{
    [SerializeField] private float timeScene;
    [SerializeField] private float timer;
    [SerializeField] private GameObject[] cave;
    [SerializeField] private AudioSource main;
    [SerializeField] private int i = 0;
    [SerializeField] private Transform playerTr;

    private bool musicaDetenida = false;

    void Start()
    {
        timeScene = timer;
    }

    void Update()
    {
        if (cave[1].activeSelf)
        {
            if (!musicaDetenida)
            {
                main.Stop();
                musicaDetenida = true;
            }

            timer -= 1.0f * Time.deltaTime;

            if (timer <= 0f)
            {
                i = 1;
            }
        }

        if (i == 1)
        {
            main.Play();

            cave[0].SetActive(true);
            cave[2].SetActive(true);
            cave[1].SetActive(false);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLAYER"))
        {
            cave[0].SetActive(false);
            cave[1].SetActive(true);
            cave[2].SetActive(false);
        }
    }
}
