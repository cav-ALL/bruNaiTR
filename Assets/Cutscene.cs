using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Cutscene : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private VideoPlayer myVideoPlayer;

    [Header("Configuración de Escena")]
    [Tooltip("Index de la escena a la que quieres ir (revisa tu Build Settings)")]
    [SerializeField] private int nextSceneIndex;

    void OnEnable()
    {
        // Si no asignaste el VideoPlayer en el inspector, intenta buscarlo en el mismo objeto
        if (myVideoPlayer == null)
        {
            myVideoPlayer = GetComponent<VideoPlayer>();
        }

        if (myVideoPlayer != null)
        {
            // Nos suscribimos al evento de Unity que detecta el final del video
            myVideoPlayer.loopPointReached += CambiarEscenaAlTerminar;
        }
        else
        {
            Debug.LogError("¡Falta el componente VideoPlayer en este GameObject!");
        }
    }

    void OnDisable()
    {
        // Buena práctica: nos desuscribimos del evento al apagar el objeto para evitar fugas de memoria
        if (myVideoPlayer != null)
        {
            myVideoPlayer.loopPointReached -= CambiarEscenaAlTerminar;
        }
    }

    // Esta función se ejecuta automáticamente en el segundo exacto donde el video llega a su fin
    private void CambiarEscenaAlTerminar(VideoPlayer source)
    {
        SceneManager.LoadScene(nextSceneIndex);
    }
}

