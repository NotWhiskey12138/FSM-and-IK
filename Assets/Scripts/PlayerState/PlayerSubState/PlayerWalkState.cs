using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerGroundState
{
    public PlayerWalkState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        
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

        if (xInput == 0 && yInput == 0)
        {
            stateMachine.ChangeState(player.IdleState);
        }
        else if (isRunning)
        {
            stateMachine.ChangeState(player.RunState);
        }
        else if (isSprinting)
        {
            stateMachine.ChangeState(player.SprintState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.SetVelocityHorizontal(xInput * playerData.walkSpeed, yInput * playerData.walkSpeed);
        player.SetRotation(xInput, yInput, playerData.turnSpeed);
    }

    public override void DoCheck()
    {
        base.DoCheck();
    }
}
