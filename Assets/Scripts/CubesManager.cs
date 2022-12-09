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

    [SerializeField] private GameObject cube;
    [SerializeField] private List<Transform> cubesList;
    [SerializeField] private float moveForce = 2000;
    [SerializeField] private float jumpForce = 200;
    [SerializeField] private float rotateSpeed = 200;
    
    public Dictionary<int,CubeState> cubesDict = new Dictionary<int, CubeState>();
    public Dictionary<int, CubeState> networkCubesDict = new Dictionary<int, CubeState>();

    private CubeController cubeController;
    private Vector3 moveVector;
    private Rigidbody rb;

    private readonly Dictionary<int, Transform> interactables= new Dictionary<int, Transform>();

    void Start()
    {
        cubeController = cube.GetComponent<CubeController>();
        rb = cube.GetComponent<Rigidbody>();

        for (int i = 0; i < cubesList.Count; i++)
        {
            interactables.Add(i, cubesList[i]);
        }

        interactables.Add(interactables.Count, cube.transform);
    }
    
    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            moveVector.x = Input.GetAxis("Horizontal");
            moveVector.z = Input.GetAxis("Vertical");
            rb.AddForce(moveVector * Time.deltaTime * moveForce);
            print(cubeController.CanJump() + " 2");

            if (cubeController.CanJump())
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
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
