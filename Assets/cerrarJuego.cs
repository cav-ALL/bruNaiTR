using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class cerrarJuego : MonoBehaviour
{
    private PlayableDirector director;

    void Awake()
    {
        director = GetComponent<PlayableDirector>();
    }

    void OnEnable()
    {
        // Nos suscribimos al evento que se activa cuando la timeline se detiene
        director.stopped += OnTimelineStopped;
    }

    void OnDisable()
    {
        // Buena práctica: desuscribirse para evitar fugas de memoria
        director.stopped -= OnTimelineStopped;
    }

    void OnTimelineStopped(PlayableDirector obj)
    {
        Debug.Log("La Timeline ha terminado. Cerrando el juego...");

        // Cierra la aplicación (funciona en el build ejecutable)
        Application.Quit();

        // Esto es solo para que puedas ver que funciona mientras pruebas dentro de Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
