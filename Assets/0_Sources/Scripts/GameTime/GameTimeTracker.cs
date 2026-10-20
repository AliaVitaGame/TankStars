using System;
using System.Collections;
using UnityEngine;

public class GameTimeTracker : MonoBehaviour
{
    public int TotalSeconds { get; private set; }
    
    public event Action<int> OnSecondsChangedEvent;
    
    public static GameTimeTracker Instance;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        var baseDelay = 1;
        var wait = new WaitForSeconds(baseDelay);

        while (true)
        {
            yield return wait;
            TotalSeconds++;
            OnSecondsChangedEvent?.Invoke(TotalSeconds);
        }
    }
}
