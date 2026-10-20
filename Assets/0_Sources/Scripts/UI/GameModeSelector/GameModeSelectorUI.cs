using UnityEngine;
using UnityEngine.UI;

public class GameModeSelectorUI : MonoBehaviour
{
    [SerializeField] private GameMode[] gameModes;
    [SerializeField] private Button playButton;
    [SerializeField] private GameObject panelModes;

    private int _selectedModeIndex;
    
    private void Start()
    {
        playButton.onClick.AddListener(StartSelectMode);

        foreach (var gameMode in gameModes)
        {
            gameMode.selectButton.
                onClick.AddListener(() => SelectMode(gameMode.sceneID));
        }
    }

    private void SelectMode(int ID)
    {
        _selectedModeIndex = ID;
        panelModes.SetActive(false);
    }

    private void StartSelectMode()
    {
        if (_selectedModeIndex < 0)
        {
            Debug.LogError("Game mode not selected");
            return;
        }

        var sceneID = gameModes[_selectedModeIndex].sceneID;
        SceneLoadingManager.Instance.LoadScene(sceneID);
    }
    

    [System.Serializable] 
    private class GameMode
    {
        public int sceneID;
        public Button selectButton;
    }
}
