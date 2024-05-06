using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            Debug.Log("’……");
        }
        else
        {
            Debug.Log("…‚ÌŠO‚Å‚·");
        }
    }
}
