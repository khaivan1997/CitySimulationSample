using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float rotationSpeed;
    float localRotation;
    public float minAngle = -10f;
    public float maxAngle = 60f;
    // Start is called before the first frame update
    void Start()
    {
        localRotation = this.transform.localRotation.eulerAngles.x;
        rotationSpeed = this.transform.parent.GetComponent<PlayerController>().rotationSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        handlePlayerInput();
    }
    void handlePlayerInput()
    {
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed*Time.deltaTime;
        localRotation-= mouseY;
        localRotation = Mathf.Clamp(localRotation, minAngle, maxAngle);
        transform.localRotation = Quaternion.Euler(localRotation, 0f, 0f);
    }
}
