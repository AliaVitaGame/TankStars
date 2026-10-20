using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanelController : MonoBehaviour
{
    [Header("Buttons")] 
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button fightButton;
    [SerializeField] private Button modesButton;
    [SerializeField] private Button caseButton;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button myTechniqueButton;
    [SerializeField] private Button rewardButton;
    [SerializeField] private Button exerciseButton;
    
    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject modesPanel;
    [SerializeField] private GameObject casePanel;
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject myTechniquePanel;
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private GameObject exercisePanel;
    
    private List<GameObject> _allPanels = new List<GameObject>();

    private void Start()
    {
        _allPanels.Add(settingsPanel);
        _allPanels.Add(modesPanel);
        _allPanels.Add(casePanel);
        _allPanels.Add(leaderboardPanel);
        _allPanels.Add(shopPanel);
        _allPanels.Add(myTechniquePanel);
        _allPanels.Add(rewardPanel);
        _allPanels.Add(exercisePanel);

        InitializeButtons();
    }

    private void InitializeButtons()
    {
        fightButton.onClick.AddListener(StartFight);

        settingsButton.onClick.AddListener(() => ActivePanel(settingsPanel));
        modesButton.onClick.AddListener(() => ActivePanel(modesPanel));
        caseButton.onClick.AddListener(() => ActivePanel(casePanel));
        leaderboardButton.onClick.AddListener(() => ActivePanel(leaderboardPanel));
        shopButton.onClick.AddListener(() => ActivePanel(shopPanel));
        myTechniqueButton.onClick.AddListener(() => ActivePanel(myTechniquePanel));
        rewardButton.onClick.AddListener(() => ActivePanel(rewardPanel));
        exerciseButton.onClick.AddListener(() => ActivePanel(exercisePanel));
    }

    private void StartFight()
    {
        //
    }
    
    public void DeactivateAllPanels()
    {
        foreach (var panel in _allPanels)
            panel.SetActive(false);
    }
    
    private void ActivePanel(GameObject currentPanel, bool isActive = true)
    {
        foreach (var panel in _allPanels)
            panel.SetActive(panel == currentPanel && isActive);
    }
}
