using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string[] dialoguesWitch;
    public TextMeshProUGUI dialouguesText;
    public int index;
    [SerializeField] private float speedText;
    [SerializeField] private BrujaController witchStatickScript;
    [SerializeField] private RawImage pointer;
    [SerializeField] private Color[] colors = { Color.red, Color.white };
    public int objetosRecolectar = 0;
    [SerializeField] private GameObject finalwitch;
    [SerializeField] private GameObject witch;


    [SerializeField] private Transform witchTrans;
    [SerializeField] private Transform newTransWitch;
    void Start()
    {
        dialouguesText.text = string.Empty;
    }

    // Update is called once per frame
    void Update()
    {
        if (objetosRecolectar == 1)
        {
            finalwitch.SetActive(true);
            Destroy(witch);
        }

        if (Input.GetMouseButtonDown(1) && witchStatickScript.onMouse)
        {
            if (dialouguesText.text == dialoguesWitch[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                dialouguesText.text = dialoguesWitch[index];
            }

        }
    }

    public void WitchDialogues()
    {
        index = 0;
        dialouguesText.gameObject.SetActive(true);
        StartCoroutine(WriteLine());
    }

    IEnumerator WriteLine()
    {
        foreach (char letter in dialoguesWitch[index].ToCharArray())
        {
            dialouguesText.text += letter;

            yield return new WaitForSeconds(speedText);
        }
    }

    public void NextLine()
    {
        if (index < dialoguesWitch.Length - 1)
        {
            index++;
            dialouguesText.text = string.Empty;
            StartCoroutine(WriteLine());
        }
        else
        {
            dialouguesText.text = string.Empty;
            dialouguesText.gameObject.SetActive(false);
            witchStatickScript.onMouse = false;
            index = 0;
        }
    }

    public void pointerColor(int index)
    {
        pointer.color = colors[index];
    }

}
