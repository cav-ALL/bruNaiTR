using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    [Header("Configuración del Screamer")]
    [Tooltip("El número de la escena de Game Over en el Build Settings")]
    public int gameOverSceneIndex = 3;

    [Tooltip("Tiempo que el screamer estará en pantalla antes de cambiar de escena")]
    public float waitTime = 2f;

    // OnEnable se ejecuta automáticamente cuando el GameObject cambia a SetActive(true)
    private void OnEnable()
    {
        // Iniciamos la cuenta regresiva en cuanto el objeto aparece
        StartCoroutine(ScreamerSequence());
    }

    private IEnumerator ScreamerSequence()
    {
        // 1. El screamer ya está activo en pantalla en este punto.
        // Aquí podrías, por ejemplo, reproducir un sonido.

        // 2. Esperamos los 10 segundos exactos.
        yield return new WaitForSeconds(waitTime);

        // 3. Cambiamos a la escena de Game Over.
        SceneManager.LoadScene(gameOverSceneIndex);
    }
}
