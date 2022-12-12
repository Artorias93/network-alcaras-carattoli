using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CubeController : MonoBehaviour
{
    [SerializeField] private float repulsiveRadius = 5.0f;
    [SerializeField] private float repulsiveForce = 10.0f;
    [SerializeField] private float upwardsModifier = 3.0f;
    [SerializeField] private float moveForce = 2000;
    [SerializeField] private float jumpForce = 20;

    private bool b_isGrounded = true;
    private bool b_canExplode = false;
    private Vector3 moveVector;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    { 
        if (PhotonNetwork.IsMasterClient) {
            moveVector.x = Input.GetAxis("Horizontal");
            moveVector.z = Input.GetAxis("Vertical");
            rb.AddForce(moveForce * Time.deltaTime * moveVector);
            if (b_isGrounded)
            {
                if (Input.GetButton("Jump"))
                {
                    b_isGrounded = false;
                   
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                    Vector3 explosionPos = transform.position;
                    Collider[] colliders = Physics.OverlapSphere(explosionPos, repulsiveRadius);
                    foreach (Collider hit in colliders)
                    {
                        if (hit.CompareTag("Player") || hit.CompareTag("Floor")) b_canExplode = false;
                        else b_canExplode = true;

                       // Rigidbody rb = hit.GetComponent<Rigidbody>();

                        if (b_canExplode)
                        {
                            foreach (Transform child in transform)
                            {
                                child.gameObject.AddComponent<Rigidbody>();
                                child.transform.parent = null;
                            }
                            rb.AddExplosionForce(repulsiveForce, explosionPos, repulsiveRadius, upwardsModifier, ForceMode.Impulse);
                        }
                    }
                }
            }
        }
       
    }

    private void OnCollisionEnter(Collision collision)
    {
        b_isGrounded = true;
        if (!collision.gameObject.CompareTag("Floor")) {
            collision.transform.parent = transform;
            Destroy(collision.gameObject.GetComponent<Rigidbody>());
        }
    }
}
