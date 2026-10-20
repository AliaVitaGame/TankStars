using TMPro;
using UnityEngine;
using YG;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text[] coinCountText;
    [SerializeField] private TMP_Text[] diamondCountText;

    private void OnEnable()
    {
        PlayerDataManager.OnSaveProgressEvent += RefreshUI;
        YG2.onGetSDKData += RefreshUI;
    }

    private void OnDisable()
    {
        PlayerDataManager.OnSaveProgressEvent -= RefreshUI;
        YG2.onGetSDKData -= RefreshUI;
    }

    private void Start()
    {
        if(YG2.isSDKEnabled) 
            RefreshUI();
    }

    private void RefreshUI()
    {
        if(YG2.isSDKEnabled == false) return;
        
        var data = YG2.saves;

        foreach (var text in coinCountText)
            text.text = data.coins.ToString();
        
        foreach (var text in diamondCountText)
            text.text = data.diamonds.ToString();
    }
}
