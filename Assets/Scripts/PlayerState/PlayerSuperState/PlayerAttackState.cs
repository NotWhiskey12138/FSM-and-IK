using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerAbilityState
{
    protected bool canCombo;
    protected bool shouldCombo;

    protected int attackCounter;
    
    public PlayerAttackState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        canCombo = false;
        shouldCombo = false;

        if (attackCounter >= 3)
        {
            attackCounter = 0;
        }

        player.anim.SetInteger("attackCounter", attackCounter);
        
        player.SetVelocityZero();
    }

    public override void Exit()
    {
        base.Exit();

        attackCounter++;
        
        Debug.Log("attackCounter:" + attackCounter);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        //Debug.Log("canCombo"+canCombo);
        //Debug.Log("shouldcCombo"+shouldCombo);

        /*if (canCombo && player.InputHandler.attackInput)
        {
            shouldCombo = true;
        }
        */

        /*if (isAnimationFinished)
        {
            if (shouldCombo)
            {
                StateTransfer();
            }
            else
            {
                if (player.InputHandler.xInput != 0 || player.InputHandler.yInput != 0)
                {
                    if(player.InputHandler.isRunning)
                        stateMachine.ChangeState(player.RunState);
                    else
                        stateMachine.ChangeState(player.WalkState);
                }
                else
                {
                    stateMachine.ChangeState(player.IdleState);
                }
            }
        }*/
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
        canCombo = true;
    }

    public override void AnimationFinishedTrigger()
    {
        base.AnimationFinishedTrigger();

        isAbilityDone = true;
    }

    protected virtual void StateTransfer()
    {
        
    }
}
