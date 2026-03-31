using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundState : PlayerState
{
    protected float xInput;
    protected float yInput;
    protected bool isRunning;

    private bool isGrounded;
    
    public PlayerGroundState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
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

        xInput = player.InputHandler.xInput;
        yInput = player.InputHandler.yInput;
        isRunning =  player.InputHandler.isRunning;
        
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void DoCheck()
    {
        base.DoCheck();

        isGrounded = player.CheckIfTouchingGrounded();
    }
}
