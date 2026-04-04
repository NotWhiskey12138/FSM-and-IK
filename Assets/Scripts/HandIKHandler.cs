using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HandIKHandler : MonoBehaviour
{
    [Header("组件")]
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private Transform camTransform;

    [Header("Rig")]
    [SerializeField] private Rig armRig;

    [Header("单独控制")]
    [SerializeField] private TwoBoneIKConstraint leftArmIK;
    [SerializeField] private TwoBoneIKConstraint rightArmIK;

    [Header("瞄准IK")]
    [SerializeField] private MultiAimConstraint headAim;
    [SerializeField] private MultiAimConstraint bodyAim;
    [SerializeField] private float aimDistance = 10f;

    [Header("瞄准手部位置")]
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform rightHandTarget;
    [SerializeField] private Transform bowGrip;          // 弓握把
    [SerializeField] private Transform bowStringGrip;    // 弓弦位置

    [Header("弓")]
    [SerializeField] private Transform bowTransform;
    [SerializeField] private Vector3 bowAimPos;
    [SerializeField] private Vector3 bowAimRot;

    [Header("设置")]
    [SerializeField] private float transitionSpeed = 5f;

    private Transform aimTarget;
    private bool isAimIKActive = false;

    void Start()
    {
        if (headAim != null)
        {
            var sources = headAim.data.sourceObjects;
            if (sources.Count > 0)
                aimTarget = sources[0].transform;
        }
    }

    void Update()
    {
        if (!isAimIKActive)
        {
            float globalTarget = inputHandler.ikActive ? 1f : 0f;
            armRig.weight = Mathf.Lerp(armRig.weight, globalTarget,
                Time.deltaTime * transitionSpeed);
            armRig.weight = Mathf.Clamp01(armRig.weight);
        }
        else
        {
            armRig.weight = Mathf.Lerp(armRig.weight, 1f,
                Time.deltaTime * transitionSpeed);
            armRig.weight = Mathf.Clamp01(armRig.weight);
        }
        
        HandleAimIK();
    }

    // ===== 外部接口 =====

    /// <summary>
    /// 状态机调用：开关瞄准IK
    /// </summary>
    public void SetAimIK(bool active)
    {
        isAimIKActive = active;
    }

    /// <summary>
    /// 单独控制某只手的IK权重
    /// </summary>
    public void SetSingleArmIK(bool isLeft, float weight)
    {
        if (isLeft && leftArmIK != null)
            leftArmIK.weight = weight;
        else if (!isLeft && rightArmIK != null)
            rightArmIK.weight = weight;
    }

    /// <summary>
    /// 直接设置整个ArmRig的权重（攻击等状态用）
    /// </summary>
    public void SetArmRigWeight(float weight)
    {
        armRig.weight = weight;
    }

    // ===== 内部逻辑 =====

    private void HandleAimIK()
    {
        bool isAiming = isAimIKActive;
        
        if (!isAiming)
        {
            leftArmIK.weight = Mathf.Lerp(leftArmIK.weight, 0f, Time.deltaTime * transitionSpeed);
            leftArmIK.weight = Mathf.Clamp01(leftArmIK.weight);
            rightArmIK.weight = Mathf.Lerp(rightArmIK.weight, 0f, Time.deltaTime * transitionSpeed);
            rightArmIK.weight = Mathf.Clamp01(rightArmIK.weight);
            headAim.weight = Mathf.Lerp(headAim.weight, 0f, Time.deltaTime * transitionSpeed);
            headAim.weight = Mathf.Clamp01(headAim.weight);
            bodyAim.weight = Mathf.Lerp(bodyAim.weight, 0f, Time.deltaTime * transitionSpeed);
            bodyAim.weight = Mathf.Clamp01(bodyAim.weight);
            return;
        }

        // 左手 — 握弓
        leftArmIK.weight = Mathf.Lerp(leftArmIK.weight, 1f, Time.deltaTime * transitionSpeed);
        leftArmIK.weight = Mathf.Clamp01(leftArmIK.weight);
        leftHandTarget.position = bowGrip.position;
        leftHandTarget.rotation = bowGrip.rotation;

        // 右手 — 拉弦
        rightArmIK.weight = Mathf.Lerp(rightArmIK.weight, 1f, Time.deltaTime * transitionSpeed);
        rightArmIK.weight = Mathf.Clamp01(rightArmIK.weight);
        rightHandTarget.position = bowStringGrip.position;
        rightHandTarget.rotation = bowStringGrip.rotation;

        // 瞄准点
        Vector3 aimPoint = camTransform.position + camTransform.forward * aimDistance;
        aimTarget.position = Vector3.Lerp(aimTarget.position, aimPoint, Time.deltaTime * transitionSpeed);

        // 头部跟随
        headAim.weight = Mathf.Lerp(headAim.weight, 1f, Time.deltaTime * transitionSpeed);
        headAim.weight = Mathf.Clamp01(headAim.weight);

        // 弓旋转
        bowTransform.localPosition = bowAimPos;
        bowTransform.localEulerAngles = bowAimRot;
    }
    
    public void ShowBow(bool show)
    {
        bowTransform.gameObject.SetActive(show);
    }
}