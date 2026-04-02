using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HandIKHandler : MonoBehaviour
{
    [Header("组件")] 
    [SerializeField] private PlayerInputHandler inputHandler;
    
    [Header("Rig")]
    [SerializeField] private Rig armRig;
    
    [Header("单独控制")]
    [SerializeField] private TwoBoneIKConstraint leftArmIK;
    [SerializeField] private TwoBoneIKConstraint rightArmIK;
    
    [Header("设置")]
    [SerializeField] private float transitionSpeed = 5f; 
    
    private bool ikActive = true;
    private float targetWeight = 1f;

    private void Start()
    {
        inputHandler.OnIKOpenInput += (ikActive)=> { };;
    }

    void Update()
    {
        // 平滑过渡，避免瞬间跳变
        armRig.weight = Mathf.Lerp(armRig.weight, targetWeight, 
            Time.deltaTime * transitionSpeed);
    }

    // 也可以外部调用，比如收刀时关闭IK
    public void SetIK(bool active)
    {
        ikActive = active;
        targetWeight = active ? 1f : 0f;
    }
    
    // 只控制某只手的IK（比如持武器的手开IK，另一只关）
    public void SetSingleArmIK(bool isLeft, float weight)
    {
        if (isLeft && leftArmIK != null)
            leftArmIK.weight = weight;
        else if (!isLeft && rightArmIK != null)
            rightArmIK.weight = weight;
    }
}
