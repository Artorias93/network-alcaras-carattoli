using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CubeController : MonoBehaviour
{
    [SerializeField] private float repulsiveRadius = 6.0f;
    [SerializeField] private float repulsiveForce = 2.0f;
    [SerializeField] private float moveForce = 700;
    [SerializeField] private float flaotingPosition=4.0f;
    [SerializeField] private float floatStrenght=700.0f;
    [SerializeField] private float attractiveForce=4f;
    [SerializeField] private float attractiveRadius = 4.0f;

    private bool floatButtonPressed;
    private Vector3 moveVector;
    private Rigidbody rb;

    private Collider[] colliders;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.z = Input.GetAxis("Vertical");

        if (Input.GetButton("Jump"))
        {
            floatButtonPressed = true;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            colliders = Physics.OverlapSphere(transform.position, repulsiveRadius);
        }
        else
        { 
            colliders = Physics.OverlapSphere(transform.position, attractiveRadius); 
        }

        if (Input.GetButtonUp("Jump"))
        {
            floatButtonPressed = false;
            rb.constraints = RigidbodyConstraints.None;
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            rb.AddForce(moveForce * Time.fixedDeltaTime * moveVector);

            if (floatButtonPressed)
            {
                if (transform.position.y < flaotingPosition)
                {
                    rb.AddForce(Vector3.up * floatStrenght * Time.fixedDeltaTime);
                }

                foreach (Collider hit in colliders)
                {
                    if (hit.CompareTag("Prop"))
                    {
                        Vector3 dir = transform.position - hit.transform.position;
                        hit.GetComponent<PropColorController>().interact();
                        hit.GetComponent<Rigidbody>().AddForce(-repulsiveForce * dir.normalized, ForceMode.Force);
                    }

                }
            }
            else {

                foreach (Collider hit in colliders)
                {
                    if (hit.CompareTag("Prop"))
                    {
                        Vector3 dir = transform.position - hit.transform.position;
                        hit.GetComponent<PropColorController>().interact();
                        hit.GetComponent<Rigidbody>().AddForce(attractiveForce*dir.normalized*Time.fixedDeltaTime, ForceMode.Force);
                    }

                }
            }

        }

    }
}

