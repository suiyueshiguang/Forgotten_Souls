public interface IGameManager
{
    public int lostCurrencyAmount { get; set; }

    public void RestartScene();
    public void PauseGame(bool _pause);

}
