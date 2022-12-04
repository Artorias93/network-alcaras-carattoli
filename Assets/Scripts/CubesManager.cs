using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CubesManager : MonoBehaviour, IPunObservable
{   
    [SerializeField] private Transform cube;
    private Vector3 cubeNetworkPosition;
    private Quaternion cubeNetworkRotation;

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {

            cube.GetComponent<Rigidbody>().AddForce(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * 2000);

        }
        else {
            cube.position = cube.position + 0.9f *(cubeNetworkPosition- cube.position);
            cube.rotation = Quaternion.Slerp(cube.rotation, cubeNetworkRotation, 0.9f);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(cube.position);
            stream.SendNext(cube.rotation);
        }
        else
        {
            cubeNetworkPosition = (Vector3)stream.ReceiveNext();
            cubeNetworkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
