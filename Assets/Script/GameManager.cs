using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string[] dialoguesWitch;
    public TextMeshProUGUI dialouguesText;
    public int index;
    [SerializeField] private float speedText;
    [SerializeField] private BrujaController witchStatickScript;
    [SerializeField] private RawImage pointer;
    [SerializeField] private Color[] colors = { Color.red, Color.white };
    void Start()
    {
        dialouguesText.text = string.Empty;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && witchStatickScript.onMouse)
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
