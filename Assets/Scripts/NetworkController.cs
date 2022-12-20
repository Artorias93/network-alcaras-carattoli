using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkController : MonoBehaviourPunCallbacks
{
    public bool isMaster =false;
    [SerializeField] private CubesManager cubesManager;
    
    public void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.SerializationRate=10;
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom("Test", roomOptions, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        isMaster = true;
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (!isMaster) {
            GameObject cubesContainer = cubesManager.cubesContainer;
            for (int i = 0; i < cubesContainer.transform.childCount; i++)
            {
                Transform item = cubesContainer.transform.GetChild(i);
                //Destroy(item.GetComponent<Rigidbody>());
                item.GetComponent<Rigidbody>().isKinematic = true;
            }
            //Destroy(cubesManager.cube.GetComponent<Rigidbody>());
            cubesManager.cube.GetComponent<Rigidbody>().isKinematic = true;

        }

    }
}
