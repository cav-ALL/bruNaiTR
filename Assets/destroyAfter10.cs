using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyAfter10 : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if(Time.time > 7f)
            Destroy(gameObject);
    }
}
