using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropColorController : MonoBehaviour
{
    public bool interacting = false;
    private Coroutine coroutine=null;
    private Renderer render;

    void Start()
    {
        render = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (interacting)
        {
            render.material.color = Color.red;
        }
        else {
            render.material.color = Color.gray;
        }
        
    }

    IEnumerator resetInteracting() {
        yield return new WaitForSeconds(1);
        interacting = false;
    }

    public void interact() {
        interacting = true;
        if (coroutine != null)
        {
            StopCoroutine("resetInteracting");
        }
        coroutine = StartCoroutine("resetInteracting");
    }
}
