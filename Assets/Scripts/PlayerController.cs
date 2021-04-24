using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator animator;
    Rigidbody rigidbody; 
    [ReadOnly]
    public float movementSpeed = 2.0f;
    [ReadOnly]
    public float rotationSpeed = 1000.0f;
   

   

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        handlePlayerInput();
    }

    void handlePlayerInput()
    {

        //Mouse
        float mouseX = Input.GetAxis("Mouse X")  * rotationSpeed*Time.deltaTime;
        this.transform.Rotate(Vector3.up * mouseX);

        //Keyboard
        float movementX = Input.GetAxis("Horizontal") * Time.deltaTime * movementSpeed;
        float movementY = Input.GetAxis("Vertical") * Time.deltaTime * movementSpeed;

        Vector3 movementVertical = this.transform.forward.normalized * movementY;
        Vector3 movementHorizontal = this.transform.right.normalized * movementX;
        Vector3 movementVelocity = movementHorizontal + movementVertical;
       
        if (movementY != 0)
        {
            animator.SetBool("isMovingVertical", true);
            animator.SetFloat("speedVertical", movementY);
        }
        else
        {
            animator.SetBool("isMovingVertical", false);
            animator.SetFloat("speedVertical", 0);
        }

        if (movementX != 0)
        {
            animator.SetBool("isMovingHorizontal", true);
            animator.SetFloat("speedHorizontal", movementX);
        }
        else
        {
            animator.SetBool("isMovingHorizontal", false);
            animator.SetFloat("speedHorizontal", 0);
        }
        //moving
        rigidbody.velocity = movementVelocity*100f;

    }
}
