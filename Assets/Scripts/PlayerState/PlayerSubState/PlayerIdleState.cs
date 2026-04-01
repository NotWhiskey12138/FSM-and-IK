using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        player.SetVelocityZero();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (stateMachine.currentState != this)
            return;
        
        if (xInput != 0 || yInput != 0)
        {
            if(isRunning)
                stateMachine.ChangeState(player.RunState);
            else if(!isRunning)
                stateMachine.ChangeState(player.WalkState);
            else if(isSprinting)
                stateMachine.ChangeState(player.SprintState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void DoCheck()
    {
        base.DoCheck();
    }
}
