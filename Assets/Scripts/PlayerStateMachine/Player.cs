using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerStateMachine stateMachine { get; private set; }
    
    public PlayerIdleState IdleState { get; private set; }
    public PlayerWalkState WalkState { get; private set; }
    public PlayerRunState RunState { get; private set; }
    public PlayerAttackState Attack1State { get; private set; }
    public PlayerAttackState Attack2State { get; private set; }
    public PlayerAttackState Attack3State { get; private set; }
    
    [Header("可视数据")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private GameObject groundCheckPoint;
    
    public Animator anim { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public Rigidbody rigid { get; private set; }
    public CapsuleCollider MovementCollider { get; private set; }

    private Vector3 workspace;
    private Vector3 Direction;
    private float turnAmount;
    public Vector3 currentVelocity { get; private set; }
    
    

    private void Awake()
    {
        stateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, stateMachine, playerData, "idle");
        WalkState = new PlayerWalkState(this, stateMachine, playerData, "walk");
        RunState = new PlayerRunState(this, stateMachine, playerData, "run");

        Attack1State = new PlayerAttackState(this, stateMachine, playerData, "attack");
        Attack2State = new PlayerAttackState(this, stateMachine, playerData, "attack");
        Attack3State = new PlayerAttackState(this, stateMachine, playerData, "attack");
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        InputHandler = GetComponent<PlayerInputHandler>();
        rigid = GetComponent<Rigidbody>();
        MovementCollider = GetComponent<CapsuleCollider>();
        
        stateMachine.InitializeState(IdleState);
        
    }

    private void Update()
    {
        stateMachine.currentState.LogicUpdate();
        
        //Debug.Log(currentVelocity);
        Debug.Log(stateMachine.currentState);
    }

    private void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }
    
    #region CheckFuctions
    
    /// <summary>
    /// 检测地面并设置是否与地面接触
    /// </summary>
    public bool CheckIfTouchingGrounded()
    {
        var collider = Physics.OverlapSphere(groundCheckPoint.transform.position, playerData.groundCheckRadius, playerData.whatIsGround);
    
        if (collider.Length != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    /// <summary>
    /// 将检测体在Unity引擎中显示出来
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheckPoint.transform.position, playerData.groundCheckRadius);
    }
    
    #endregion
    
    #region setFuction

    /// <summary>
    /// 设置速度为0
    /// </summary>
    public void SetVelocityZero()
    {
        workspace = Vector3.zero;
        rigid.velocity = workspace;
        currentVelocity = workspace;
    }
    
    /// <summary>
    /// 设置水平速度或位移
    /// </summary>
    /// <param name="xInput">x轴输入</param>
    /// <param name="zInput">z轴输入</param>
    public void SetVelocityHorizontal(float xInput, float zInput)
    {
        workspace.Set(xInput, currentVelocity.y, zInput);
        rigid.velocity = workspace;
        currentVelocity = workspace;
    }
    
    /// <summary>
    /// 设置垂直速度或位移
    /// </summary>
    /// <param name="yInput">垂直变量</param>
    public void SetVelocityVertical(float yInput)
    {
        workspace.Set(currentVelocity.x, yInput, currentVelocity.z);
        rigid.velocity = workspace;
        currentVelocity = workspace;
    }

    /// <summary>
    /// 设置旋转
    /// </summary>
    /// <param name="xInput"></param>
    /// <param name="yInput"></param>
    public void SetRotation(float xInput, float yInput, float turnSpeed)
    {
        Direction = transform.InverseTransformVector(new Vector3(xInput, 0, yInput));
        turnAmount = Mathf.Atan2(Direction.x, Direction.z);
        rigid.MoveRotation(rigid.rotation * Quaternion.Euler(0, turnAmount * turnSpeed, 0));
    }
    #endregion
    
    private void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();
        
    private void AnimationFinishedTrigger() => stateMachine.currentState.AnimationFinishedTrigger();
}
