using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerController controllScript;

    [SerializeField] private float waitForAudio;

    [SerializeField] private AudioClip foot;
    void Start()
    {
        StartCoroutine(PlayFootStep());
    }

    IEnumerator PlayFootStep()
    {
        while (true)
        {
            if(controllScript.move.magnitude > 0.5f && controllScript.charPlayer.isGrounded)
            {
                AudioManager.instance.PlaySFX(foot);
            }

                yield return new WaitForSeconds(waitForAudio);
        }
    }
    
}
