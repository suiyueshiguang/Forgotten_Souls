public class PlayerStateMachine : IPlayerStateMachine
{
    public PlayerState currentState { get; private set; }

    //��ʼ��
    public void Initialized(PlayerState _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }

    //�ı�
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
