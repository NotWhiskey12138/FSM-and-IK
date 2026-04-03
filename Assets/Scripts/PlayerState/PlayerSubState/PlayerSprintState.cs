using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprintState : PlayerGroundState
{
    private bool isDashDone;
    
    public PlayerSprintState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        isDashDone = false;
        player.useRootMotion = true;
        
        player.FootIK?.SetFootIK(false);
        player.HandIK?.SetAimIK(false);
        player.camTransform?.SetSprintCam(true);
    }

    public override void Exit()
    {
        base.Exit();
        player.camTransform?.SetSprintCam(false);
        player.useRootMotion = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (stateMachine.currentState != this)
            return;

        if (!isSprinting && isDashDone)
        {
            if (xInput == 0 && yInput == 0)
            {
                stateMachine.ChangeState(player.IdleState);
            }
            else if (xInput != 0 || yInput != 0)
            {
                if(isRunning)
                    stateMachine.ChangeState(player.RunState);
                else
                    stateMachine.ChangeState(player.WalkState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        
        Vector3 moveDir = player.GetCameraDirection(xInput, yInput);
        player.SetVelocityHorizontal(moveDir.x * playerData.sprintSpeed, moveDir.z * playerData.sprintSpeed);
        player.SetRotation(moveDir.x, moveDir.z, playerData.turnSpeed);
    }

    public override void DoCheck()
    {
        base.DoCheck();
    }

    public override void AnimationFinishedTrigger()
    {
        base.AnimationFinishedTrigger();
        
        isDashDone = true;
    }
}
