using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RnadomScream : MonoBehaviour
{
    [Header("Fuentes de Audio (Asigna las 3 en el Inspector)")]
    [Tooltip("Cada AudioSource debe tener ya su propio clip asignado en el editor")]
    [SerializeField] private AudioSource[] fuentesAmbiente;

    [Header("Configuración de Tiempo")]
    [Tooltip("Tiempo en segundos entre cada sonido")]
    [SerializeField] private float tiempoEspera = 30f;

    void Start()
    {
        // Validación por si te olvidas de arrastrarlas en el Inspector
        if (fuentesAmbiente == null || fuentesAmbiente.Length == 0)
        {
            Debug.LogWarning("¡No has asignado ningún AudioSource en el array del script!");
            return;
        }

        // Iniciamos el temporizador en bucle
        StartCoroutine(BucleSonidosAleatorios());
    }

    IEnumerator BucleSonidosAleatorios()
    {
        while (true)
        {
            // Espera exactamente los 30 segundos
            yield return new WaitForSeconds(tiempoEspera);

            if (fuentesAmbiente.Length > 0)
            {
                // Elegimos una de las 3 fuentes al azar
                int indiceAleatorio = Random.Range(0, fuentesAmbiente.Length);

                // Verificamos que el slot no esté vacío y reproducimos
                if (fuentesAmbiente[indiceAleatorio] != null)
                {
                    fuentesAmbiente[indiceAleatorio].Play();
                }
            }
        }
    }
}
