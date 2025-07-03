using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyStateMachine
{
    public void Initialize(EnemyState _startState);
    public void ChangeState(EnemyState _newState);
    public EnemyState GetCurrentState();
}
