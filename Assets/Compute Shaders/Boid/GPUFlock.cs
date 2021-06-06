using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUFlock : MonoBehaviour {

    public ComputeShader cshader;

    public GameObject boidPrefab;
    public int boidsCount;
    public float spawnRadius;
    public GameObject[] boidsGo;
    public GPUBoid[] boidsData;
    public float flockSpeed;
    public float nearbyDis;
    public Transform[] checkPoints;

    private Vector3 targetPos = Vector3.zero;
    private int kernelHandle;
    

    void Start()
    {
        boidsGo = new GameObject[boidsCount];
        boidsData = new GPUBoid[boidsCount];
        kernelHandle = cshader.FindKernel("CSMain");
        checkPoints = new Transform[4];


        for (int i = 0; i < this.boidsCount; i++)
        {
            boidsData[i] = this.CreateBoidData();
            boidsGo[i] = Instantiate(boidPrefab, this.boidsData[i].pos, Quaternion.Euler(this.boidsData[i].rot)) as GameObject;
            boidsData[i].rot = this.boidsGo[i].transform.forward;
        }
    }

    GPUBoid CreateBoidData()
    {
        GPUBoid boidData = new GPUBoid();
        Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
        Quaternion rot = Quaternion.Slerp(transform.rotation, Random.rotation, 0.3f);
        boidData.pos = pos;
        boidData.flockPos = transform.position;
        boidData.boidsCount = this.boidsCount;
        boidData.nearbyDis = this.nearbyDis;
        boidData.speed = this.flockSpeed + Random.Range(-0.5f, 0.5f);

        return boidData;
    }

    void Update()
    {
        //if(targetPos == checkPoints[0].transform.position)
        //{
        //    Mathf.Lerp(targetPos.x, checkPoints[1].transform.position.x, 5.0f);
        //    Mathf.Lerp(targetPos.x, checkPoints[1].transform.position.x, 5.0f);

        //}
        /*
        targetPos += new Vector3(2f, 5f, 3f);
        transform.localPosition += new Vector3(
            (Mathf.Sin(Time.deltaTime * this.targetPos.x) * -0.2f),
            (Mathf.Sin(Time.deltaTime * this.targetPos.y) * 0.2f),
            (Mathf.Sin(Time.deltaTime * this.targetPos.z) * 0.2f)
        );
        */

        ComputeBuffer buffer = new ComputeBuffer(boidsCount, 48);

        for (int i = 0; i < this.boidsData.Length; i++)
        {
            this.boidsData[i].flockPos = this.transform.position;
        }

        buffer.SetData(this.boidsData);

        cshader.SetBuffer(this.kernelHandle, "boidBuffer", buffer);
        cshader.SetFloat("deltaTime", Time.deltaTime);

        cshader.Dispatch(this.kernelHandle, this.boidsCount, 1, 1);

        buffer.GetData(this.boidsData);

        buffer.Release();

        for (int i = 0; i < this.boidsData.Length; i++)
        {

            this.boidsGo[i].transform.localPosition = this.boidsData[i].pos;

            if(!this.boidsData[i].rot.Equals(Vector3.zero))
            {
                this.boidsGo[i].transform.rotation = Quaternion.LookRotation(this.boidsData[i].rot);
            }

        }
    }
}
