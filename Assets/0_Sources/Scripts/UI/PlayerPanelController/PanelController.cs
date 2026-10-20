using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [Space]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [Space]
    [SerializeField] private GameObject roundWinPanel;
    [SerializeField] private GameObject roundLosePanel;
    [Space]
    [SerializeField] private GameObject drawPanel;
    [SerializeField] private GameObject deathPanel;
    [Space]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button[] settingsButtons;
    [SerializeField] private Button[] restartButtons;
    [SerializeField] private Button[] menuButtons;
    
    private bool _isPaused;
    private PlayerHealthStats _playerHealthStats;
    private List<GameObject> _allPanels = new List<GameObject>();

    private const int SceneMenuID = 0;
    
    public static PanelController Instance { get; private set; }
    
    private void OnEnable()
    {
        CharacterSpawnerBase.PlayerInitializedContextEvent += PlayerInitialized;
    }

    private void OnDisable()
    {
        CharacterSpawnerBase.PlayerInitializedContextEvent -= PlayerInitialized;
        
        if(_playerHealthStats)
         _playerHealthStats.OnDeathEvent -= ActiveDeathPanel;
    }


    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("There is more than one PanelController in the scene");
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _allPanels.Add(pausePanel);
        _allPanels.Add(winPanel);
        _allPanels.Add(losePanel);
        _allPanels.Add(deathPanel);
        _allPanels.Add(drawPanel);
        _allPanels.Add(roundWinPanel);
        _allPanels.Add(roundLosePanel);
        _allPanels.Add(settingsPanel);
        
        continueButton.onClick.AddListener(SwitchActivePause);
        pauseButton.onClick.AddListener(SwitchActivePause);
        
        foreach (var button in settingsButtons)
            button.onClick.AddListener(SwitchActiveSettings);

        foreach (var button in restartButtons)
            button.onClick.AddListener(Restart);
        
        foreach (var button in menuButtons)
            button.onClick.AddListener(LoadMenu);
    }

    public void SwitchActivePause()
    {
        _isPaused  = !_isPaused;
        
        Time.timeScale = _isPaused ? 0 : 1;
        ActivePanel(pausePanel, _isPaused);
    }

    private void LoadMenu() => SceneLoadingManager.Instance.LoadScene(SceneMenuID);
    private void Restart() => SceneLoadingManager.Instance.LoadActiveScene();

    private void SwitchActiveSettings()
    {
        ActivePanel(settingsPanel, !settingsPanel.activeSelf);
        if (settingsPanel.activeSelf == false) ActivePausePanel();
    }
    public void ActiveRoundWinPanel() => ActivePanel(roundWinPanel);
    public void ActiveRoundLosePanel() => ActivePanel(roundLosePanel);
    
    public void ActiveWinPanel() => ActivePanel(winPanel);

    public void ActiveDeathPanel(GameObject player)
    {
        if(losePanel == false)
         ActivePanel(deathPanel);
    }
    
    public void ActivePausePanel() => ActivePanel(pausePanel);
    public void ActiveLosePanel() => ActivePanel(losePanel);
    public void ActiveDrawPanel() => ActivePanel(drawPanel);

    public void DeactivateAllPanels()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        foreach (var panel in _allPanels)
            panel.SetActive(false);
    }
    
    private void ActivePanel(GameObject currentPanel, bool isActive = true)
    {
        foreach (var panel in _allPanels)
            panel.SetActive(panel == currentPanel && isActive);
    }
    
    private void PlayerInitialized(GameObject player)
    {
        if (!player.TryGetComponent(out PlayerHealthStats playerHealthStats))
        {
            Debug.LogError("Player has no Health Stats");
            return;
        }
        
        _playerHealthStats = playerHealthStats;
        _playerHealthStats.OnDeathEvent += ActiveDeathPanel;
    }

}
