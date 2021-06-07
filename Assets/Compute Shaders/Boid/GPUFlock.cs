using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUFlock : MonoBehaviour {

    public struct FishStruct
    {
        public Vector3 flockingPosicion;
        public Vector3 posicion;
        public Vector3 rotacion;
        public float distanceBetweeenFish;
        public float fishSpeed;
        public float fishNumber;
    }

    public ComputeShader computeShader;
    public GameObject fish;
    public GameObject[] fishBanc;
    public FishStruct[] fishData;

    public int numFish;
    public float spawnRadius;
    public float flockSpeed;
    public float nearbyDis;
    private int kernelHandle;
    

    void Start()
    {
        fishBanc = new GameObject[numFish];
        fishData = new FishStruct[numFish];
        kernelHandle = computeShader.FindKernel("CSMain");


        for (int i = 0; i < numFish; i++)
        {
            fishData[i] = CreateBoidData();
            fishBanc[i] = Instantiate(fish, fishData[i].posicion, Quaternion.Euler(fishData[i].rotacion)) as GameObject;
            fishData[i].rotacion = fishBanc[i].transform.forward;
        }
    }

    void Update()
    {
        ComputeBuffer buffer = new ComputeBuffer(numFish, 48);

        for (int i = 0; i < fishData.Length; i++)
        {
            fishData[i].flockingPosicion = transform.position;
        }

        buffer.SetData(fishData);

        computeShader.SetBuffer(kernelHandle, "boidBuffer", buffer);
        computeShader.SetFloat("deltaTime", Time.deltaTime);

        computeShader.Dispatch(kernelHandle, numFish, 1, 1);

        buffer.GetData(fishData);

        buffer.Release();

        for (int i = 0; i < fishData.Length; i++)
        {

            fishBanc[i].transform.localPosition = fishData[i].posicion;

            if(!fishData[i].rotacion.Equals(Vector3.zero))
            {
                fishBanc[i].transform.rotation = Quaternion.LookRotation(fishData[i].rotacion);
            }

        }
    }

    private FishStruct CreateBoidData()
    {
        Quaternion rot = Quaternion.Slerp(transform.rotation, Random.rotation, 0.1f);
        Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;

        FishStruct boidData = new FishStruct();
        boidData.fishSpeed = flockSpeed + Random.Range(-0.1f, 0.1f);
        boidData.fishNumber = numFish;
        boidData.posicion = pos;
        boidData.flockingPosicion = transform.position;
        boidData.distanceBetweeenFish = nearbyDis;
       
        return boidData;
    }
}
