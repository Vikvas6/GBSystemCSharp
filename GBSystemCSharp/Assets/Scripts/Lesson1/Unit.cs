using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class Unit : MonoBehaviour
{
    [SerializeField] private int _health = 0;
    [SerializeField] private int _maxHealth = 100;

    [SerializeField] private bool _currentlyHealing = false;
    
    private async void Start()
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;
        Task task1 = Task1(cancellationToken);
        Task task2 = Task2(cancellationToken);
        Debug.Log("Tasks created");
        var result = await WhatTaskFasterAsync(cancellationToken, task1, task2);
        Debug.Log("After WhatTaskFasterAsync");
        Debug.Log($"Result = {result}");
        cancellationTokenSource.Cancel();
        //var task2 = Task1(cancellationToken);
        cancellationTokenSource.Dispose();
    }

    public void ReceiveHealing()
    {
        StartCoroutine(Healing(5, 3.0f, 0.5f));
    }

    IEnumerator Healing(int amount, float duration, float delay)
    {
        if (_currentlyHealing)
        {
            yield break;
        }

        _currentlyHealing = true;
        Debug.Log("Healing started.");
        float total_duration = 0.0f;
        while (total_duration <= duration)
        {
            if (Heal(amount))
            {
                _currentlyHealing = false;
                Debug.Log("Fully healed.");
                yield break;
            }
            yield return new WaitForSeconds(delay);
            total_duration += delay;
        }
        _currentlyHealing = false;
        Debug.Log("Healing effect ended.");
    }

    private bool Heal(int amount)
    {
        _health += amount;
        if (_health > _maxHealth)
        {
            _health = _maxHealth;
            return true;
        }
        return false;
    }

    private async Task Task1(CancellationToken cancelToken)
    {
        await Task.Delay(1000, cancelToken);
        Debug.Log("Task 1 finished.");
    }

    private async Task Task2(CancellationToken cancelToken)
    {
        for (int i = 0; i < 60; i++)
        {
            await Task.Yield();
            if (cancelToken.IsCancellationRequested)
            {
                Debug.Log("Cancelled by cancellation token");
                return;
            }
        }
        Debug.Log("Task 2 finished.");
    }

    public static async Task<bool> WhatTaskFasterAsync(CancellationToken ct, Task task1, Task task2)
    {
        await Task.WhenAny(task1, task2);
        if (ct.IsCancellationRequested)
        {
            Debug.Log("Tasks were cancelled.");
            return false;
        }
        Debug.Log("Some task is finished.");
        if (task1.IsCompleted)
        {
            Debug.Log("Tasks 1 IsCompleted");
            return true;
        }
        if (task2.IsCompleted)
        {
            Debug.Log("Tasks 2 IsCompleted");
            return false;
        }
        Debug.Log("Shouldn't happen");
        return false;
        // try
        // {
        //     Task.WaitAny(new [] {task1, task2}, ct);
        // }
        // catch (OperationCanceledException)
        // {
        //     Debug.Log("Cancel by token.");
        //     return Task.FromResult(false);
        // }
        //
        // if (task1.IsCompleted)
        // {
        //     task2.Dispose();
        //     return Task.FromResult(true);
        // }
        //
        // task1.Dispose();
        // return  Task.FromResult(false);
    }
}
