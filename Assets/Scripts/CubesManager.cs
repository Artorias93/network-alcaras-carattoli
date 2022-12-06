using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CubesManager : MonoBehaviour, IPunObservable
{   
    [SerializeField] private Transform cube;
    [SerializeField] private Transform cubeAround;
    [SerializeField] private float force = 2000;
    [SerializeField] private float rotateSpeed = 200;

    private Vector3 cubeNetworkPosition;
    private Quaternion cubeNetworkRotation;
    private Vector3 cubeAroundNetworkPosition;
    private Quaternion cubeAroundNetworkRotation;

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            cube.GetComponent<Rigidbody>().AddForce(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * force);
            cubeAround.RotateAround(cube.position, Vector3.up, rotateSpeed * Time.deltaTime);
        }
        else 
        {
            cube.position = cube.position + 0.9f *(cubeNetworkPosition - cube.position);
            cube.rotation = Quaternion.Slerp(cube.rotation, cubeNetworkRotation, 0.9f);
            cubeAround.position = cubeAround.position + 0.9f * (cubeAroundNetworkPosition - cubeAround.position);
            cubeAround.rotation = Quaternion.Slerp(cubeAround.rotation, cubeAroundNetworkRotation, 0.9f);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(cube.position);
            stream.SendNext(cube.rotation);
            stream.SendNext(cubeAround.position);
            stream.SendNext(cubeAround.rotation);
        }
        else
        {
            cubeNetworkPosition = (Vector3)stream.ReceiveNext();
            cubeNetworkRotation = (Quaternion)stream.ReceiveNext();
            cubeAroundNetworkPosition = (Vector3)stream.ReceiveNext();
            cubeAroundNetworkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
