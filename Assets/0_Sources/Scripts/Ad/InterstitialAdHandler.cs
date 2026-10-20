using UnityEngine;
using YG;

public class InterstitialAdHandler : MonoBehaviour
{
    public static InterstitialAdHandler Instance;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Debug.LogWarning("Multiple Interstitial Ads detected");
    }

    public void Show() => YG2.InterstitialAdvShow();
}
