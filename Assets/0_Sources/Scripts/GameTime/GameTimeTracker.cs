using System;
using System.Collections;
using UnityEngine;
using YG;

public class GameTimeTracker : MonoBehaviour
{
    public int TotalSeconds { get; private set; }
    
    public event Action<int> OnSecondsChangedEvent;
    
    public static GameTimeTracker Instance;

    private bool _loaded;
    private int _secondsSinceSave;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        YG2.onGetSDKData += LoadFromSaves;
    }

    private void OnDisable()
    {
        YG2.onGetSDKData -= LoadFromSaves;
    }

    private void Start()
    {
        if (YG2.isSDKEnabled)
            LoadFromSaves();

        StartCoroutine(EnsureLoaded());
        StartCoroutine(Timer());
    }

    private IEnumerator EnsureLoaded()
    {
        float elapsed = 0f;
        while (_loaded == false && elapsed < 4f)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        if (_loaded == false)
            LoadFromSaves();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            FlushSave();
    }

    private void OnApplicationQuit()
    {
        FlushSave();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            FlushSave();
    }

    private IEnumerator Timer()
    {
        var wait = new WaitForSeconds(1);

        while (true)
        {
            yield return wait;
            if (_loaded == false)
                continue;

            TotalSeconds++;
            WriteToSaves();
            OnSecondsChangedEvent?.Invoke(TotalSeconds);

            _secondsSinceSave++;
            if (_secondsSinceSave >= 15)
                FlushSave();
        }
    }

    private void LoadFromSaves()
    {
        if (_loaded)
            return;

        TotalSeconds = YG2.saves.playTimeSeconds;
        _loaded = true;
        OnSecondsChangedEvent?.Invoke(TotalSeconds);
    }

    private void WriteToSaves()
    {
        YG2.saves.playTimeSeconds = TotalSeconds;
        if (PlayerDataManager.Instance)
            PlayerDataManager.Instance.SetPlayTimeSeconds(TotalSeconds);
    }

    private void FlushSave()
    {
        if (_loaded == false)
            return;

        WriteToSaves();
        _secondsSinceSave = 0;

        if (PlayerDataManager.Instance)
            PlayerDataManager.Instance.Save();
        else
            YG2.SaveProgress();
    }
}
