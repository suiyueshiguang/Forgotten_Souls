

public class EnemyStateMachine : IEnemyStateMachine
{
    public EnemyState currentState;

    public void Initialize(EnemyState _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }

    public void ChangeState(EnemyState _newState)
    {
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }

    public EnemyState GetCurrentState()
    {
        return currentState;
    }
}