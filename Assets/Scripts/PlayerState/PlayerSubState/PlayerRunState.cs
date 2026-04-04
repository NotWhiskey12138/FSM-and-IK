using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerGroundState
{
    public PlayerRunState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        player.FootIK?.SetFootIK(true);
        player.HandIK?.SetAimIK(false);
        player.SyncFootPhase("run");
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
        else if (!isRunning)
        {
            stateMachine.ChangeState(player.WalkState);
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
        player.SetVelocityHorizontal(moveDir.x * playerData.runSpeed, moveDir.z * playerData.runSpeed);
        player.SetRotation(moveDir.x, moveDir.z, playerData.turnSpeed);
    }

    public override void DoCheck()
    {
        base.DoCheck();
    }
}
