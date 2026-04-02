using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimIK : MonoBehaviour
{
    public Transform aimTarget;      // 空物体，放在准星位置
    public Transform camTransform;
    public float aimDistance = 10f;

    private Animator anim;
    private Player player;

    void Start()
    {
        anim = GetComponent<Animator>();
        player = GetComponent<Player>();
    }

    // Unity内置IK回调，Animator层需要勾选IK Pass
    void OnAnimatorIK(int layerIndex)
    {
        if (anim == null) return;

        bool isAiming = player.InputHandler.isAiming;

        if (isAiming)
        {
            // 把瞄准目标放在相机前方
            aimTarget.position = camTransform.position + camTransform.forward * aimDistance;

            // 上半身看向瞄准点
            anim.SetLookAtWeight(1f, 0.5f, 1f, 1f, 0.7f);
            anim.SetLookAtPosition(aimTarget.position);

            // 右手（拉弓手）指向目标
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.5f);
            anim.SetIKPosition(AvatarIKGoal.RightHand, aimTarget.position);
        }
        else
        {
            anim.SetLookAtWeight(0f);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
        }
    }
}
