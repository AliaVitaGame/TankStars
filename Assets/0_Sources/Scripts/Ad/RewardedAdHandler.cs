using UnityEngine;
using YG;

public class RewardedAdHandler : MonoBehaviour
{
    public static RewardedAdHandler Instance;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Debug.LogError("Rewarded Ad Handler already exists!");
    }

    public void Show(string ID) => YG2.RewardedAdvShow(ID);
}
