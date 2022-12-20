using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CubesManager : MonoBehaviour, IPunObservable
{
    public struct CubeState {
        public Vector3 position;
        public Quaternion rotation;

        public CubeState(Transform cube)
        {
            this.position = cube.position;
            this.rotation = cube.rotation;
        }
        public CubeState(Vector3 position,Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }

    [SerializeField] public GameObject cube;
    [SerializeField] public GameObject cubesContainer;
    [SerializeField] private List<Transform> cubesList;
    [SerializeField] private NetworkController network;

    
    public Dictionary<int, CubeState> networkCubesDict = new Dictionary<int, CubeState>();


    private readonly Dictionary<int, Transform> interactables= new Dictionary<int, Transform>();

    void Start()
    {

        for (int i = 0; i < cubesContainer.transform.childCount; i++)
        {
            Transform item = cubesContainer.transform.GetChild(i);
            cubesList.Add(item);
            interactables.Add(i, item);
        }

        interactables.Add(interactables.Count, cube.transform);

    }
    
    private void Update()
    {
        if (!network.isMaster)
        {
            foreach (var item in networkCubesDict)
            {
                Vector3 networkPosition = item.Value.position;
                Quaternion networkRotation = item.Value.rotation;
                float distance = Vector3.Distance(interactables[item.Key].position, networkPosition);
                float angle = Quaternion.Angle(interactables[item.Key].rotation, networkRotation);

                interactables[item.Key].position = Vector3.MoveTowards(interactables[item.Key].position, networkPosition, distance * Time.deltaTime * PhotonNetwork.SerializationRate);
                interactables[item.Key].rotation = Quaternion.RotateTowards(interactables[item.Key].rotation, networkRotation, angle * Time.deltaTime * PhotonNetwork.SerializationRate);
                PropColorController controller = interactables[item.Key].GetComponent<PropColorController>();
                if (controller != null) {
                    controller.interact();
                }
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {


        if (stream.IsWriting)
        {
            List<int> keys = new List<int>();
            List<Vector3> positions = new List<Vector3>();
            List<Quaternion> rotations = new List<Quaternion>();
            foreach (var item in interactables)
            {
                PropColorController controller =  item.Value.GetComponent<PropColorController>();
                if (controller == null || controller.interacting)
                {
                    keys.Add(item.Key);
                    positions.Add(item.Value.position);
                    rotations.Add(item.Value.rotation);
                }
            }

            stream.SendNext(keys.ToArray());
            stream.SendNext(positions.ToArray());
            stream.SendNext(rotations.ToArray());
        }
        else
        {   
            networkCubesDict = new Dictionary<int, CubeState>();
            int[] keys = (int[])stream.ReceiveNext();
            Vector3[] positions = (Vector3[])stream.ReceiveNext();
            Quaternion[] rotations = (Quaternion[])stream.ReceiveNext();
            for (int i = 0; i < keys.Length; i++) {
                networkCubesDict.Add(keys[i], new CubeState(positions[i], rotations[i]));
            }
        }
    }
}
