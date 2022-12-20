using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropColorController : MonoBehaviour
{
    public bool interacting = false;
    private Coroutine coroutine=null;
    private Coroutine colorCoroutine = null;
    private Renderer renderer;
    private Vector3 oldPos=Vector3.zero;
    private Quaternion oldRot = Quaternion.identity;
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }


    private void Update()
    {
        if (oldPos != transform.position || oldRot != transform.rotation) {
            oldPos = transform.position;
            oldRot = transform.rotation;
            interact();
        }
    }
    IEnumerator resetInteracting() 
    {
        yield return new WaitForSeconds(1);
        
        if (colorCoroutine != null)
        {
            StopCoroutine(colorCoroutine);
        }
        colorCoroutine = StartCoroutine("FadeG2R");
        interacting = false;
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
}
