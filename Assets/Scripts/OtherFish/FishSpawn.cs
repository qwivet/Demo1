using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Collections;

public class FishSpawn : MonoBehaviour
{
    
    [SerializeField]
    private int numberOfFishes;
    [SerializeField]
    private GameObject fish;
    [SerializeField]
    private Vector3 minPosition;
    [SerializeField]
    private Vector3 maxPosition;
    private GameObject[] fishObjects;
    private Transform[] fishTransform;
    private NativeArray<Vector3> fishPositions;
    private TransformAccessArray transformArray;

    void Awake()
    {
        fishObjects = new GameObject[numberOfFishes];
        fishPositions = new NativeArray<Vector3> (numberOfFishes, Allocator.Persistent);
        fishTransform =  new Transform [numberOfFishes];
        for (int i = 0; i < numberOfFishes; i++)
        {
            fishObjects[i] = GameObject.Instantiate(fish, new Vector3 (Random.Range (minPosition.x, maxPosition.x),Random.Range (minPosition.x, maxPosition.x),Random.Range (minPosition.x, maxPosition.x)), Quaternion.identity);
            fishTransform[i] = fishObjects[i].transform;
        }
        transformArray = new TransformAccessArray (fishTransform);
    }
    private void Update() 
    {
        for (int i = 0; i < numberOfFishes; i++)
        {
            fishPositions[i] = transformArray[i].position;
        }
        var fishMove = new FishMove 
        {
            Positions = fishPositions,
            DeltaTime = Time.deltaTime,
            Distances = new NativeArray<int> (4, Allocator.Persistent)
        };
        var handle = fishMove.Schedule(transformArray);
        handle.Complete();
    }
}
public struct FishMove : IJobParallelForTransform
{
    [NativeDisableParallelForRestriction]
    public NativeArray<Vector3> Positions;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> Distances;
    public float DeltaTime;
    private Vector3 finalPosition;
    public void Execute (int index, TransformAccess transform)
    {
        for (int i = 0; i < Positions.Length; i++)
        {
            for (int j = 0; j < Distances.Length; j++)
            {
                if (Vector3.Distance (transform.position, Positions[Distances[j]]) < Vector3.Distance (transform.position, Positions[i]))
                {
                    Distances[j] = i;
                    break;
                }
            }
        }
        for (int i = 0; i < Distances.Length; i++)
        {
            finalPosition += Positions[Distances[i]];
        }
        finalPosition /= Distances.Length;
        transform.position += (finalPosition - transform.position).normalized * DeltaTime;
    }
}
