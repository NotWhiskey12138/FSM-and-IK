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
    public PlayerSprintState SprintState { get; private set; }
    public PlayerAttackState Attack1State { get; private set; }
    public PlayerAimState AimState { get; private set; }
    public bool useRootMotion { get; set; }
    
    [Header("可视数据")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private GameObject groundCheckPoint;
    
    [Header("攻击属性")]
    [HideInInspector] public int attackCounter = 0;

    [Header("相机")] 
    public CameraController camTransform;

    [Header("武器")]
    public WeaponHolder weapon;
    
    public Animator anim { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public Rigidbody rigid { get; private set; }
    public CapsuleCollider MovementCollider { get; private set; }
    public HandIKHandler HandIK { get; private set; }
    public FootIKHandler FootIK { get; private set; }
    

    private Vector3 workspace;
    private Vector3 Direction;
    private float turnAmount;
    public Vector3 currentVelocity { get; private set; }
    
    [Header("脚步同步")]
    private int footPhaseHash = Animator.StringToHash("FootPhase");

    private Dictionary<int, float> leftFootLandTimes = new Dictionary<int, float>();
    public bool isLeftFootLanding { get; private set; }
    public float lastFootLandTime { get; private set; }
    public float walkLeftLand { get; private set; }
    public float runLeftLand { get; private set; }
    
    

    private void Awake()
    {
        stateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, stateMachine, playerData, "idle");
        WalkState = new PlayerWalkState(this, stateMachine, playerData, "walk");
        RunState = new PlayerRunState(this, stateMachine, playerData, "run");
        SprintState = new PlayerSprintState(this, stateMachine, playerData, "sprint");
        AimState = new PlayerAimState(this, stateMachine, playerData, "aim");

        Attack1State = new PlayerAttackState(this, stateMachine, playerData, "attack");
        
        HandIK = GetComponent<HandIKHandler>();
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        InputHandler = GetComponent<PlayerInputHandler>();
        rigid = GetComponent<Rigidbody>();
        MovementCollider = GetComponent<CapsuleCollider>();
        camTransform = Camera.main.GetComponent<CameraController>();
        
        
        HandIK = GetComponent<HandIKHandler>();
        FootIK = GetComponent<FootIKHandler>();
        
        stateMachine.InitializeState(IdleState);
        
    }

    private void Update()
    {
        stateMachine.currentState.LogicUpdate();
        
        //Debug.Log(currentVelocity);
        //Debug.Log(stateMachine.currentState);
        anim.SetFloat("velocity", currentVelocity.magnitude);
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
        // 用rigidbody实际的y速度，让重力正常累积
        workspace.Set(xInput, rigid.velocity.y, zInput);
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

    public Vector3 GetCameraDirection(float xInput, float yInput)
    {
        Vector3 camForward = camTransform.transform.forward;
        Vector3 camRight = camTransform.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();
        
        return (camForward * yInput + camRight * xInput).normalized;
    }

    /// <summary>
    /// 让Root接管角色位移
    /// </summary>
    private void OnAnimatorMove()
    {
        if (useRootMotion)
        {
            Vector3 deltaPos = anim.deltaPosition;
            
            deltaPos.y = rigid.velocity.y * Time.deltaTime;
            rigid.MovePosition(rigid.position + deltaPos);

            rigid.MoveRotation(rigid.rotation * anim.deltaRotation);
        }
    }
    
    #region Camera

    public Vector3 GetCameraForward()
    {
        Vector3 camForward = camTransform.transform.forward;
        camForward.y = 0f;
        return camForward.normalized;
    }

    public void FaceDirection(Vector3 dir)
    {
        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion newRotation = Quaternion.LookRotation(dir);
            rigid.MoveRotation(Quaternion.Slerp(rigid.rotation, newRotation, 
                Time.fixedDeltaTime * 10f));
        }
    }
    
    #endregion

    #region IK synchronous

    public void OnLeftFootLand()
    {
        isLeftFootLanding = true;
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float normalizedTime = stateInfo.normalizedTime % 1f;
        lastFootLandTime = normalizedTime;
    
        int hash = stateInfo.shortNameHash;
        leftFootLandTimes[hash] = normalizedTime;
    }

    public void OnRightFootLand()
    {
        isLeftFootLanding = false;
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        lastFootLandTime = stateInfo.normalizedTime % 1f;
    }
    
    /// <summary>
    /// 同步脚步到目标动画
    /// </summary>
    public void SyncFootPhase(string targetStateName)
    {
        int targetHash = Animator.StringToHash(targetStateName);
        AnimatorStateInfo currentInfo = anim.GetCurrentAnimatorStateInfo(0);
        int currentHash = currentInfo.shortNameHash;
    
        float currentPhase = currentInfo.normalizedTime % 1f;
    
        if (leftFootLandTimes.ContainsKey(currentHash) && leftFootLandTimes.ContainsKey(targetHash))
        {
            float fromLand = leftFootLandTimes[currentHash];
            float toLand = leftFootLandTimes[targetHash];
            float relativePhase = (currentPhase - fromLand + 1f) % 1f;
            float targetPhase = (relativePhase + toLand) % 1f;
            anim.SetFloat(footPhaseHash, targetPhase);
        }
        else
        {
            anim.SetFloat(footPhaseHash, currentPhase);
        }
    }

    #endregion
}
