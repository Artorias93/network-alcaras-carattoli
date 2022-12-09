using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    private bool b_isGrounded = true;
    private bool b_canJump = false;

    #region Getter
    public bool CanJump()
    {
        return b_canJump;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        print(b_isGrounded + " 1");

        if (b_isGrounded)
        {
            if(Input.GetButton("Jump"))
            {
                b_isGrounded = false;
                b_canJump = true;
            }
        }
        else b_canJump = false;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            b_isGrounded = true;
        }
        
        print(b_isGrounded + "EHI");

    }
}
