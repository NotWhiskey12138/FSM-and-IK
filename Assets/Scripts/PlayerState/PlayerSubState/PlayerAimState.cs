using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimState : PlayerGroundState
{
    public PlayerAimState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.HandIK.SetAimIK(true);
        player.HandIK.ShowBow(true);
    }

    public override void Exit()
    {
        base.Exit();
        player.HandIK.SetAimIK(false);
        player.HandIK.ShowBow(false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (stateMachine.currentState != this)
            return;

        if (!isAiming)
        {
            if(xInput==0 || yInput==0)
                stateMachine.ChangeState(player.IdleState);
            else if(isRunning)
                stateMachine.ChangeState(player.RunState);
            else if(!isRunning)
                stateMachine.ChangeState(player.WalkState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        
        Vector3 camForward = player.GetCameraForward();
        player.FaceDirection(camForward);

        if (xInput != 0 || yInput != 0)
        {
            Vector3 moveDir = player.GetCameraDirection(xInput, yInput);
            player.SetVelocityHorizontal(
                moveDir.x * playerData.walkSpeed * 0.7f,  // 瞄准时移速降低
                moveDir.z * playerData.walkSpeed * 0.7f);
        }
        else
        {
            player.SetVelocityZero();
        }
    }

    public override void DoCheck()
    {
        base.DoCheck();
    }
}
