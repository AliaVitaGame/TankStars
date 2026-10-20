using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoundResultControllerBase : MonoBehaviour
{
    [SerializeField] private CharacterSpawnerBase characterSpawner;
    [SerializeField] private TeamCollection teamCollection;

    public Action<TeamData> OnTeamWinEvent;
    public Action<TeamData> OnTeamLoseEvent;
    public Action OnDrawEvent;
    
    public Action<GameObject> OnUnitDeathEvent;

    public static Action<bool> OnMatchResult;

    protected int[] CurrentAliveCountPerTeam;
    protected int[] AliveCountPerTeam;
    protected bool IsRoundActive;
    
    protected TeamData PlayerTeam;
    
    protected TeamCollection TeamCollection => teamCollection;
    
    private List<IUnitHealth> _allUnitHealth = new List<IUnitHealth>();

    private void Start()
    {
        IsRoundActive = true;
    }

    protected virtual void OnEnable()
    {
        characterSpawner.OnCharacterSpawned += CharacterSpawned;
        characterSpawner.OnPlayerSpawned += PlayerSpawned;
    }

    protected virtual void OnDisable()
    {
        characterSpawner.OnCharacterSpawned -= CharacterSpawned;
        characterSpawner.OnPlayerSpawned -= PlayerSpawned;
    }

    protected virtual void OnDestroy()
    {
        foreach (IUnitHealth unitHealth in _allUnitHealth)
            unitHealth.OnDeathEvent  -= OnUnitDeathContext;
    }

    private void CharacterSpawned(GameObject character, TeamData team)
    {
        if (!character.TryGetComponent(out IUnitHealth health)) return;

        int ID = GetTeamID(team);
        if (ID < 0) return;

        if (AliveCountPerTeam == null || AliveCountPerTeam.Length == 0)
            InitializeAliveCountPerTeam();
        
       AliveCountPerTeam[ID]++;
       CurrentAliveCountPerTeam[ID]++;
       health.OnDeathEvent += OnUnitDeathContext;
       _allUnitHealth.Add(health);
    }

    private void InitializeAliveCountPerTeam()
    {
        AliveCountPerTeam = new int[teamCollection.Teams.Length];
        CurrentAliveCountPerTeam = (int[])AliveCountPerTeam.Clone();
    }

    private void PlayerSpawned(GameObject player, TeamData team) => PlayerTeam = team;

    private void OnUnitDeathContext(GameObject character)
    {
        if (!character.TryGetComponent(out IUnitHealth health))
        {
            Debug.LogError($"Character {character.name} has no health component");
            return;
        }

        if (!character.TryGetComponent(out TeamProfile teamProfile))
        {
            Debug.LogError($"Character {character.name} has no team profile");
            return;
        }
        
        OnUnitDeathEvent?.Invoke(character);
        
        int ID = GetTeamID(teamProfile.MyTeam);
        CurrentAliveCountPerTeam[ID]--;
        
        EvaluateAliveTeams();
    }

    private void EvaluateAliveTeams()
    {
        int aliveTeam = 0;
        TeamData lastAliveTeam = null;

        for (int i = 0; i < teamCollection.Teams.Length; i++)
        {
            if (CurrentAliveCountPerTeam[i] > 0)
            {
                aliveTeam++;
                lastAliveTeam = teamCollection.Teams[i]; 
          
                if (aliveTeam > 1) return;
            }
        }

        if (aliveTeam == 0)
        {
            OnDrawEvent?.Invoke();
            OnRoundDraw();
        }
        else if(aliveTeam == 1)
        {
            OnTeamWinEvent?.Invoke(lastAliveTeam);

            foreach (var team in teamCollection.Teams)
            {
                if(team !=  lastAliveTeam)
                    OnTeamLoseEvent?.Invoke(team);
            }

            RoundWinningTeam(lastAliveTeam);
        }
    }

    protected abstract void OnRoundDraw();
    protected abstract void RoundWinningTeam(TeamData winnerTeam);

    protected virtual void StartNextRound(TeamData winnerTeam)
    {
        CurrentAliveCountPerTeam = (int[])AliveCountPerTeam.Clone();
    }

    protected int GetTeamID(TeamData team)
    {
        for (int i = 0; i < teamCollection.Teams.Length; i++)
        {
            if (teamCollection.Teams[i] == team)
                return i;
        }

        return -1;
    }
}