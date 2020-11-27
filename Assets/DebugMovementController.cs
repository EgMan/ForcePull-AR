using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMovementController : MonoBehaviour
{
    private void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.DownArrow))
        {
            transform.Rotate(new Vector3(.1f, 0, 0));
        }
        if (Input.GetKey(KeyCode.K) || Input.GetKey(KeyCode.UpArrow))
        {
            transform.Rotate(new Vector3(-.1f, 0, 0));
        }
        if (Input.GetKey(KeyCode.H) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(new Vector3(0, -.1f, 0));
        }
        if (Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(new Vector3(0, .1f, 0));
        }
    }
}
