using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerAbilityState
{
    protected bool canCombo;
    protected bool shouldCombo;
    
    public PlayerAttackState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        canCombo = false;
        shouldCombo = false;

        player.anim.SetInteger("attackCounter", player.attackCounter);

        player.useRootMotion = true;
    }

    public override void Exit()
    {
        base.Exit();

        player.useRootMotion = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (stateMachine.currentState != this)
            return;
 
        if (canCombo && player.InputHandler.attackInput)
        {
            shouldCombo = true;
        }
 
        if (isAnimationFinished)
        {
            if (shouldCombo && player.attackCounter < 2)
            {
                player.attackCounter++;
                canCombo = false;
                shouldCombo = false;
                isAnimationFinished = false;
 
                player.anim.SetInteger("attackCounter", player.attackCounter);
                player.anim.SetTrigger("attackNext");
            }
            else
            {
                player.attackCounter = 0;
 
                if (player.InputHandler.xInput != 0 || player.InputHandler.yInput != 0)
                {
                    if (player.InputHandler.isRunning)
                        stateMachine.ChangeState(player.RunState);
                    else
                        stateMachine.ChangeState(player.WalkState);
                }
                else
                {
                    stateMachine.ChangeState(player.IdleState);
                }
            }
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

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
        Debug.Log("=== AnimationTrigger(canCombo) 被调用了 ===");
        canCombo = true;
    }

    public override void AnimationFinishedTrigger()
    {
        base.AnimationFinishedTrigger();
        Debug.Log("=== AnimationFinished 被调用了 ===");
    }

    protected virtual void StateTransfer()
    {
        
    }
}
