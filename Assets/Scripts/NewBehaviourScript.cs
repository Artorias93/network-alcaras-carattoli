using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour, IPunObservable
{
    [SerializeField] private Transform cube;
    private Vector3 cubeNetworkPosition;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(cube.position);
        }
        else
        {
            cube.position= (Vector3)stream.ReceiveNext();
        }
    }
}
