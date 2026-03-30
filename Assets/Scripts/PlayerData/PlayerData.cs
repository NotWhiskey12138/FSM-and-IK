using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("角色属性")]
    public float walkSpeed;
    public float runSpeed;
    public float turnSpeed;

    [Header("地面检测")]
    public float groundCheckRadius; //地面检测半径
    public LayerMask whatIsGround; //什么是地面
}
