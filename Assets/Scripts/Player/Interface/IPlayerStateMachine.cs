public interface IPlayerStateMachine
{
    public void Initialized(PlayerState _startState);

    public void ChangeState(PlayerState _newState);

    public PlayerState GetCurrentState();
}
