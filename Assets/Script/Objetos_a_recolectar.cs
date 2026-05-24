using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class Objetos_a_recolectar : MonoBehaviour
{
    [SerializeField] private GameManager gameScript;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        gameScript.objetosRecolectar += 1;
        Destroy(gameObject);
    }

    private void OnMouseEnter()
    {
        gameScript.pointerColor(0);
    }

    private void OnMouseExit()
    {
        gameScript.pointerColor(1);
    }

}
