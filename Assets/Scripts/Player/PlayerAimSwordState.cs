using UnityEngine;

public class PlayerAimSwordState : PlayerState
{
    public PlayerAimSwordState(Player _player, IPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.skill.GetSword().DotsActive(true);
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .2f);
    }

    public override void Update()
    {
        base.Update();

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        player.SetZeroVelocity();

        if (Input.GetButtonUp("Skill_Sword"))
        {
            player.playerStateFactory.ChangePlayerState(PlayerStates.idleState);
        }

        //鼠标在角色左边，但角色面向右边
        if (player.transform.position.x > mousePosition.x && player.facingDir == 1)
        {
            player.Filp();
        }
        //鼠标在角色右边，但角色面向左边
        else if (player.transform.position.x < mousePosition.x && player.facingDir == -1)
        {
            player.Filp();
        }
    }
}
