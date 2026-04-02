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

        Vector3 moveDir = player.GetCameraDirection(xInput, yInput);
        player.SetVelocityHorizontal(moveDir.x * playerData.walkSpeed, moveDir.z * playerData.walkSpeed);
        player.SetRotation(moveDir.x, moveDir.z, playerData.turnSpeed);
    }

    public override void DoCheck()
    {
        base.DoCheck();
    }
}
