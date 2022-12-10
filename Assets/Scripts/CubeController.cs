using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    [SerializeField] private float repulsiveRadius = 5.0f;
    [SerializeField] private float repulsiveForce = 10.0f;
    [SerializeField] private float upwardsModifier = 3.0f;

    private bool b_isGrounded = true;
    private bool b_canJump = false;
    private bool b_canExplode = false;

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
        if (b_isGrounded)
        {
            if(Input.GetButton("Jump"))
            {
                b_isGrounded = false;
                b_canJump = true;

                Vector3 explosionPos = transform.position;
                Collider[] colliders = Physics.OverlapSphere(explosionPos, repulsiveRadius);
                foreach (Collider hit in colliders)
                {
                    if (hit.CompareTag("Player") || hit.CompareTag("Floor")) b_canExplode = false;
                    else b_canExplode = true;

                    Rigidbody rb = hit.GetComponent<Rigidbody>();

                    if (rb && b_canExplode)
                        rb.AddExplosionForce(repulsiveForce, explosionPos, repulsiveRadius, upwardsModifier,ForceMode.Impulse);
                }
            }
        }
        else b_canJump = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        b_isGrounded = true;
    }
}
