using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ChangeSceneGameOver : MonoBehaviour
{
    [SerializeField] private int scene;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
            SceneManager.LoadScene(scene);
    }
}
