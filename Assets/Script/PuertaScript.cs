using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuertaScript : MonoBehaviour
{
    [SerializeField] private int Scene;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        changeScene(Scene);
    }

    public void changeScene(int sce)
    {
        SceneManager.LoadScene(sce);
    }
}
