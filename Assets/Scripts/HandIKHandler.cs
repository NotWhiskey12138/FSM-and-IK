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

    void Update()
    {
        float target = inputHandler.ikActive ? 1 : 0;
        armRig.weight = Mathf.Lerp(armRig.weight, target, 
            Time.deltaTime * transitionSpeed);
        
    }
    
    public void SetSingleArmIK(bool isLeft, float weight)
    {
        if (isLeft && leftArmIK != null)
            leftArmIK.weight = weight;
        else if (!isLeft && rightArmIK != null)
            rightArmIK.weight = weight;
    }
}
