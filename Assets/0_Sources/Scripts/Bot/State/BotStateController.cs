using System;
using UnityEngine;

[RequireComponent(typeof(BotHealthStats))]
[RequireComponent(typeof(BotTargetDetector))]
[RequireComponent(typeof(BotCombatController))]
[RequireComponent(typeof(BotCoverPointDetector))]
public class BotStateController : MonoBehaviour
{
    private enum State
    {
        Patrol = 0,
        Attack = 1,
        Hide = 2,
        Retreat = 3,
        Idle = 4
    }

    private State _movementState;
    private State _towerActionState;

    private const float RetreatHealthThresholdPercent = 0.3f;
    
    private BotHealthStats _health;
    private BotCombatController _combatController;
    private BotCoverPointDetector _coverPointDetector;
    private BotTargetDetector _targetDetector;
    private BotMovementBase _movement;
    
    private Transform _currentEnemy;
    private Transform _currentCover;

    private void Start()
    {
        _movement = GetComponentInChildren<BotMovementBase>();
        _targetDetector = GetComponent<BotTargetDetector>();
        _coverPointDetector = GetComponent<BotCoverPointDetector>();
        _health = GetComponent<BotHealthStats>();
        _combatController = GetComponent<BotCombatController>();
        
        _combatController.Initialization(_movement, _targetDetector);
    }

    private void FixedUpdate()
    {
        UpdateState();
        HandleTowerActionState();
        HandleMovementState();
    }

    private void UpdateState()
    {
        _currentEnemy = _targetDetector.GetTarget();
        _currentCover = _coverPointDetector.GetTarget();

        if (IsHealthLow())
        {
            _movementState =  State.Retreat;
            _towerActionState = State.Attack;
        }
        else if (_currentEnemy)
        {
            _movementState =  State.Attack;
            _towerActionState = State.Attack;
        }
        else if (_currentCover)
        {
            _movementState = State.Hide;
            _towerActionState = State.Idle;
        }
        else
        {
            _movementState = State.Patrol;
            _towerActionState = State.Idle;
        }

    }
    
    private void HandleMovementState()
    {
        if(_movement == null) return;
        
        if (_movementState == State.Patrol)
        {
            _movement.MoveToRandomPointNearInterest();
        }
        else if(_movementState == State.Attack)
        {
             if(_currentEnemy)
                 _combatController.MoveToTarget();
        }
        else if (_movementState == State.Retreat)
        {
            _movement.Retreat();
        }
        else if (_movementState == State.Hide)
        {
            if(_currentCover)
                _movement.MoveTo(_currentCover.position);
        }
        else if (_movementState == State.Idle)
        {
            _movement.MoveTo(transform.position);
        }
    }
    
    private void HandleTowerActionState()
    {
        if (_towerActionState == State.Attack)
            _combatController.Attack(_currentEnemy);
        else
            _combatController.ResetRotate();
    }

    private bool IsHealthLow()
    {
        if(_health == null)
            return false;
        
        float healthPercent = _health.Health / _health.MaxHealth;
        return healthPercent < RetreatHealthThresholdPercent;
    }
}