using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalOfTheGame : MonoBehaviour
{
    [SerializeField] private int scene;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLAYER"))
            SceneManager.LoadScene(scene);
    }
}
