using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack3State : PlayerAttackState
{
    public PlayerAttack3State(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    protected override void StateTransfer()
    {
        base.StateTransfer();
    }
}
