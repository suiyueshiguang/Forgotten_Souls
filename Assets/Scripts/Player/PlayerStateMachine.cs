public class PlayerStateMachine : IPlayerStateMachine
{
    public PlayerState currentState { get; private set; }

    //初始化
    public void Initialized(PlayerState _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }

    //改变
    public void ChangeState(PlayerState _newState)
    {
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }

    public PlayerState GetCurrentState()
    {
        return currentState;
    }
}
