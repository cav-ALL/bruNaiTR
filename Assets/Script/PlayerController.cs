using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController charPlayer;
    [SerializeField] private float speed;

    void Start()
    {

    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        Vector3 move = moveX * transform.right + moveZ * transform.forward;

        charPlayer.Move(move);
    }
}
