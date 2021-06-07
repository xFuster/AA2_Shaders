using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUFlock : MonoBehaviour {

    public struct BoidStruct
    {
        public Vector3 pos, rot, flockPos;
        public float speed, nearbyDis, boidsCount;
    }

    public ComputeShader computeShader;

    public GameObject seagul;
    public int boidsCount;
    public float spawnRadius;
    public GameObject[] boidsGo;
    public BoidStruct[] boidsData;
    public float flockSpeed;
    public float nearbyDis;
    private int kernelHandle;
    

    void Start()
    {
        boidsGo = new GameObject[boidsCount];
        boidsData = new BoidStruct[boidsCount];
        kernelHandle = computeShader.FindKernel("CSMain");


        for (int i = 0; i < boidsCount; i++)
        {
            boidsData[i] = CreateBoidData();
            boidsGo[i] = Instantiate(seagul, boidsData[i].pos, Quaternion.Euler(boidsData[i].rot)) as GameObject;
            boidsData[i].rot = boidsGo[i].transform.forward;
        }
    }

    BoidStruct CreateBoidData()
    {
        BoidStruct boidData = new BoidStruct();
        Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
        Quaternion rot = Quaternion.Slerp(transform.rotation, Random.rotation, 0.3f);
        boidData.pos = pos;
        boidData.flockPos = transform.position;
        boidData.boidsCount = boidsCount;
        boidData.nearbyDis = nearbyDis;
        boidData.speed = flockSpeed + Random.Range(-0.5f, 0.5f);

        return boidData;
    }

    void Update()
    {
        ComputeBuffer buffer = new ComputeBuffer(boidsCount, 48);

        for (int i = 0; i < boidsData.Length; i++)
        {
            boidsData[i].flockPos = transform.position;
        }

        buffer.SetData(boidsData);

        computeShader.SetBuffer(kernelHandle, "boidBuffer", buffer);
        computeShader.SetFloat("deltaTime", Time.deltaTime);

        computeShader.Dispatch(kernelHandle, boidsCount, 1, 1);

        buffer.GetData(boidsData);

        buffer.Release();

        for (int i = 0; i < boidsData.Length; i++)
        {

            boidsGo[i].transform.localPosition = boidsData[i].pos;

            if(!boidsData[i].rot.Equals(Vector3.zero))
            {
                boidsGo[i].transform.rotation = Quaternion.LookRotation(boidsData[i].rot);
            }

        }
    }
}
