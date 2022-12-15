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


    private Vector3 moveVector;
    private bool floating;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            moveVector.x = Input.GetAxis("Horizontal");
            moveVector.z = Input.GetAxis("Vertical");
            rb.AddForce(moveForce * Time.fixedDeltaTime * moveVector);

            if (Input.GetButton("Jump"))
            {
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                if (transform.position.y < flaotingPosition)
                {
                    rb.AddForce(Vector3.up * floatStrenght * Time.fixedDeltaTime);
                }

                Collider[] colliders = Physics.OverlapSphere(transform.position, repulsiveRadius);
                foreach (Collider hit in colliders)
                {
                    if (hit.CompareTag("Prop"))
                    {
                        //if (hit.GetComponent<Rigidbody>() == null)
                        //{
                        //    hit.gameObject.AddComponent<Rigidbody>();
                        //    hit.transform.parent = null;
                        //}
                        Vector3 dir = transform.position - hit.transform.position;
                        hit.GetComponent<Rigidbody>().AddForce(-repulsiveForce * dir.normalized, ForceMode.Force);


                    }

                }
            }
            else {

                Collider[] colliders = Physics.OverlapSphere(transform.position, attractiveRadius);
                foreach (Collider hit in colliders)
                {
                    if (hit.CompareTag("Prop"))
                    {
                        //if (hit.GetComponent<Rigidbody>() == null)
                        //{
                        //    hit.gameObject.AddComponent<Rigidbody>();
                        //    hit.transform.parent = null;
                        //}
                        Vector3 dir = transform.position - hit.transform.position;
                        hit.GetComponent<Rigidbody>().AddForce(attractiveForce*dir.normalized*Time.fixedDeltaTime, ForceMode.Force);

                        // hit.transform.position = Vector3.MoveTowards(hit.transform.position, transform.position, attractiveForce*Time.fixedDeltaTime);
                    }

                }
            }
            if (Input.GetButtonUp("Jump"))
            {
                rb.constraints = RigidbodyConstraints.None;
            }

        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.CompareTag("Prop"))
        //{
        //    for (int i = 0; i < collision.contactCount; i++) {
        //        if (collision.GetContact(i).thisCollider.transform == transform)
        //        {
        //            Collider contactCollider = collision.GetContact(i).otherCollider;
        //            contactCollider.transform.parent = transform;
        //            Destroy(contactCollider.gameObject.GetComponent<Rigidbody>());
        //        }
        //    }
        //}
    }
}
