using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundState : PlayerState
{
    protected float xInput;
    protected float yInput;
    protected bool isRunning;
    protected bool isSprinting;

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
        isSprinting = player.InputHandler.isSprinting;

        if (player.InputHandler.attackInput)
        {
            player.attackCounter = 0;
            stateMachine.ChangeState(player.Attack1State);
        }
        
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
