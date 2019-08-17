using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float camSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float camAngle = transform.eulerAngles.x;
        transform.Translate(Input.GetAxis("Horizontal") * camSpeed, 0f, Input.GetAxis("Vertical") * camSpeed, Space.World);
        if (Input.GetKey(KeyCode.KeypadPlus) && camAngle > 1f)
        {
            transform.Translate(0f, 0f, camSpeed * 2);
            transform.Rotate(-0.8f, 0f, 0f);
            camSpeed = camSpeed * 0.995f;
        }
        if (Input.GetKey(KeyCode.KeypadMinus) && (camAngle < 89f))
        {
            camSpeed = camSpeed / 0.995f;
            transform.Rotate(0.8f, 0f, 0f);
            transform.Translate(0f, 0f, -camSpeed * 2);
        }
    }
}
