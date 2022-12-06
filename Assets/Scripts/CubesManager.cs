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

    [SerializeField] private Transform cube;
    [SerializeField] private List<Transform> cubesList;
    [SerializeField] private float force = 2000;
    [SerializeField] private float rotateSpeed = 200;
    public Dictionary<int,CubeState> cubesDict = new Dictionary<int, CubeState>();
    public Dictionary<int, CubeState> networkCubesDict = new Dictionary<int, CubeState>();

    private readonly Dictionary<int, Transform> interactables= new Dictionary<int, Transform>();

    void Start()
    {
        for (int i = 0; i < cubesList.Count; i++) {
            interactables.Add(i, cubesList[i]);
        }
        interactables.Add(interactables.Count, cube);
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            cube.GetComponent<Rigidbody>().AddForce(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * force);
        }
        else 
        {
            foreach (var item in networkCubesDict)
            {
                Vector3 networkPosition = item.Value.position;
                Quaternion networkRotation = item.Value.rotation;
                interactables[item.Key].position = networkPosition;
                interactables[item.Key].rotation = networkRotation;
            }
            //cube.position = cube.position + 0.9f *(cubeNetworkPosition - cube.position);
            //cube.rotation = Quaternion.Slerp(cube.rotation, cubeNetworkRotation, 0.9f);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        int[] keys = new int[interactables.Count];
        Vector3[] positions = new Vector3[interactables.Count];
        Quaternion[] rotations = new Quaternion[interactables.Count];

        if (stream.IsWriting)
        {
            foreach (var item in interactables)
            {
                keys[item.Key] = item.Key;
                positions[item.Key] = item.Value.position;
                rotations[item.Key] = item.Value.rotation;
            }

            stream.SendNext(keys);
            stream.SendNext(positions);
            stream.SendNext(rotations);
        }
        else
        {

            keys = (int[]) stream.ReceiveNext();
            positions = (Vector3[])stream.ReceiveNext();
            rotations = (Quaternion[])stream.ReceiveNext();
            for (int i = 0; i < keys.Length; i++) {
                if (networkCubesDict.ContainsKey(i))
                {
                    networkCubesDict[i] = new CubeState(positions[i], rotations[i]);
                }
                else
                {
                    networkCubesDict.Add(i, new CubeState(positions[i], rotations[i]));
                }
            }
        }
    }
}
