using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CubesManager : MonoBehaviour, IPunObservable
{
    public struct CubeState {
        public float posX;
        public float posY;
        public float posZ;

        public float rotX;
        public float rotY;
        public float rotZ;
        public float rotW;

        public CubeState(Transform cube)
        {
            this.posX = cube.position.x;
            this.posY = cube.position.y;
            this.posZ = cube.position.z;

            this.rotX = cube.rotation.x;
            this.rotY = cube.rotation.y;
            this.rotZ = cube.rotation.z;
            this.rotW = cube.rotation.w;
        }
    }

    [SerializeField] private Transform cube;
    [SerializeField] private List<Transform> interactables;
    [SerializeField] private float force = 2000;
    [SerializeField] private float rotateSpeed = 200;
    public List<CubeState> cubes = new List<CubeState>();
    public List<CubeState> networtkCubes = new List<CubeState>();

    private Vector3 cubeNetworkPosition;
    private Quaternion cubeNetworkRotation;
    private Vector3 cubeAroundNetworkPosition;
    private Quaternion cubeAroundNetworkRotation;


    void Start()
    {
        //if (PhotonNetwork.IsMasterClient) {
            interactables.Add(cube);
            //for (int i = 0; i < interactables.Count; i++)
            //{
            //    cubes.Add(new CubeState(interactables[i]));
            //}
        //}
    }


    internal void initializeClient(List<CubeState> masterCubes)
    {
        cubes = masterCubes;
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            cube.GetComponent<Rigidbody>().AddForce(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * force);
        }
        else 
        {   for (int i = 0; i < networtkCubes.Count; i++) {
                Vector3 networkPosition = new Vector3(networtkCubes[i].posX, networtkCubes[i].posY, networtkCubes[i].posZ);
                Quaternion networkRotation = new Quaternion(networtkCubes[i].rotX, networtkCubes[i].rotY, networtkCubes[i].rotZ, networtkCubes[i].rotW);
                interactables[i].position = networkPosition;
                interactables[i].rotation = networkRotation;

            }

            //cube.position = cube.position + 0.9f *(cubeNetworkPosition - cube.position);
            //cube.rotation = Quaternion.Slerp(cube.rotation, cubeNetworkRotation, 0.9f);
            
            //cubeAround.position = cubeAround.position + 0.9f * (cubeAroundNetworkPosition - cubeAround.position);
            //cubeAround.rotation = Quaternion.Slerp(cubeAround.rotation, cubeAroundNetworkRotation, 0.9f);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        cubes = new List<CubeState>();
        for (int i = 0; i < interactables.Count; i++)
        {
            cubes.Add(new CubeState(interactables[i]));
        }
        if (stream.IsWriting)
        {
            stream.SendNext(cubes);
           // stream.SendNext(cubeAround.position);
           // stream.SendNext(cubeAround.rotation);
        }
        else
        {
            cubes = (List<CubeState>)stream.ReceiveNext();
          //  cubeAroundNetworkPosition = (Vector3)stream.ReceiveNext();
          //  cubeAroundNetworkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
