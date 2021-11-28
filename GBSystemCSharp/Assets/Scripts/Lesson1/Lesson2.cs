using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;


public class Lesson2 : MonoBehaviour
{
    private NativeArray<int> _array;
    private Job1 _job1;
    private JobHandle _jobHandle;
    private bool _waitJob1 = false;
    
    private NativeArray<Vector3> _arrayPositions;
    private NativeArray<Vector3> _arrayVelocities;
    private NativeArray<Vector3> _arrayFinalPositions;
    private Job2 _job2;
    private JobHandle _job2Handle;
    private bool _waitJob2 = false;

    private Job3 _job3;
    private JobHandle _job3Handle;
    private bool _waitJob3 = false;
    private TransformAccessArray _transformAccessArray;
    
    private void Start()
    {
        StartJob1();
        StartJob2();
        Transform[] transforms = new Transform[1];
        transforms[0] = transform;
        _transformAccessArray = new TransformAccessArray(transforms);
    }

    private void StartJob1()
    {
        _array = new NativeArray<int>(10, Allocator.Persistent);
        for (int i = 0; i < _array.Length; i++)
        {
            _array[i] = Random.Range(9, 12);
        }
        Debug.Log($"Job1: Start array: {ArrayToStr(_array)}");
        
        _job1 = new Job1();
        _job1.array = _array;
        _jobHandle = _job1.Schedule();
        _jobHandle.Complete();
        _waitJob1 = true;
    }

    private void StartJob2()
    {
        _arrayPositions = new NativeArray<Vector3>(10, Allocator.Persistent);
        FillVector3Array(_arrayPositions);
        Debug.Log($"Job2: Start positions array: {ArrayToStr(_arrayPositions)}");
        _arrayVelocities = new NativeArray<Vector3>(10, Allocator.Persistent);
        FillVector3Array(_arrayVelocities);
        Debug.Log($"Job2: Start velocities array: {ArrayToStr(_arrayVelocities)}");
        _arrayFinalPositions = new NativeArray<Vector3>(10, Allocator.Persistent);

        _job2 = new Job2()
        {
            positions = _arrayPositions,
            velocities = _arrayVelocities,
            finalPositions = _arrayFinalPositions
        };
        _job2Handle = _job2.Schedule(10, 2);
        _job2Handle.Complete();
        _waitJob2 = true;
    }

    private void StartJob3()
    {
        _job3 = new Job3()
        {
            velocity = 2.0f,
            axis = Vector3.forward
        };
        _job3Handle = _job3.Schedule(_transformAccessArray);
        _job3Handle.Complete();
    }
    private void FillVector3Array(NativeArray<Vector3> array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = new Vector3(Random.Range(0, 5), Random.Range(0, 5), Random.Range(0, 5));
        }
    } 
    
    private void Update()
    {
        if (_waitJob1)
        {
            if (_jobHandle.IsCompleted)
            {
                Debug.Log($"Job1: Processed array {ArrayToStr(_job1.array)}");
                _array.Dispose();
                _waitJob1 = false;
            }
            Debug.Log("Job1: Not ready yet.");
        }

        if (_waitJob2)
        {
            if (_job2Handle.IsCompleted)
            {
                Debug.Log($"Job2: Processed array {ArrayToStr(_job2.finalPositions)}");
                _arrayPositions.Dispose();
                _arrayVelocities.Dispose();
                _arrayFinalPositions.Dispose();
                _waitJob2 = false;
            }
            Debug.Log("Job2: Not ready yet.");
        }
        
        StartJob3();
    }

    private string ArrayToStr<T>(NativeArray<T> array) where T : struct
    {
        var resStr = "[";
        for (int i = 0; i < array.Length; i++)
        {
            resStr += array[i].ToString() + ", ";
        }

        resStr += "]";
        return resStr;
    }

    private void OnDestroy()
    {
        _transformAccessArray.Dispose();
    }
}

public struct Job1 : IJob
{
    public NativeArray<int> array;
    
    public void Execute()
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] > 10)
            {
                array[i] = 0;
            }
        }
    }
}

public struct Job2 : IJobParallelFor
{
    public NativeArray<Vector3> positions;
    public NativeArray<Vector3> velocities;
    public NativeArray<Vector3> finalPositions;
    
    public void Execute(int index)
    {
        finalPositions[index] = positions[index] + velocities[index];
    }
}

public struct Job3 : IJobParallelForTransform
{
    public float velocity;
    public Vector3 axis;
    
    public void Execute(int index, TransformAccess transform)
    {
        var rotation = transform.rotation;
        rotation.eulerAngles += axis * velocity;
        transform.rotation = rotation;
    }
}