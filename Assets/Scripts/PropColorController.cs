using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropColorController : MonoBehaviour
{
    public bool interacting = false;
    private Coroutine coroutine=null;
    private Coroutine colorCoroutine = null;
    private Renderer renderer;
    private Rigidbody rb;
    private Vector3 oldPos=Vector3.zero;
    private Quaternion oldRot = Quaternion.identity;
    private Vector3 oldVel = Vector3.zero;
    void Start()
    {
        renderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {   
        
        if (oldPos != transform.position || oldRot != transform.rotation || oldVel != rb.angularVelocity)
        {
            oldPos = transform.position;
            oldRot = transform.rotation;
            oldVel = rb.angularVelocity;
            interacting = true;
        }
        else {
            interacting = false;
        }
        interact();
    }
    IEnumerator resetInteracting() 
    {
        yield return new WaitForSeconds(1);
        interacting = false;
        if (colorCoroutine != null)
        {
            StopCoroutine(colorCoroutine);
        }
        colorCoroutine = StartCoroutine("FadeG2R");
    }

    IEnumerator FadeR2G()
    {
        for (float alpha = 1f; alpha >= 0; alpha -= 0.1f)
        {
            renderer.material.color = Color.Lerp(Color.red, Color.gray, alpha);
            yield return new WaitForSeconds(.1f);
        }
    }

    IEnumerator FadeG2R()
    {
        for (float alpha = 1f; alpha >= 0; alpha -= 0.1f)
        {
            renderer.material.color = Color.Lerp(Color.gray, Color.red, alpha);
            yield return new WaitForSeconds(.1f);
        }
    }

    public void interact() {
        
        if (!interacting) 
        {
            if (colorCoroutine != null)
            {
                StopCoroutine(colorCoroutine);
            }
            colorCoroutine = StartCoroutine("FadeR2G");
        }
        
        interacting = true;
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine("resetInteracting");
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Player")) {
            interacting = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            interacting = false;
        }
    }
}
