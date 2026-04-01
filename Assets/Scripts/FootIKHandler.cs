using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
/// 脚部IK v7
/// 
/// 代码读动画曲线值 → 手动设 Constraint 权重 + 定位 IK Target
/// 
/// 动画曲线：
///   L_Foot_IK = 1 时左脚着地，= 0 时左脚抬起
///   R_Foot_IK 同理
/// </summary>
public class FootIKHandler : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private Player player;
    [SerializeField] private Animator animator;

    [Header("IK Targets")]
    [SerializeField] private Transform leftFootIKTarget;
    [SerializeField] private Transform rightFootIKTarget;

    [Header("脚骨骼")]
    [SerializeField] private Transform leftFootBone;
    [SerializeField] private Transform rightFootBone;

    [Header("Constraint（用来控制权重）")]
    [SerializeField] private TwoBoneIKConstraint leftIK;
    [SerializeField] private TwoBoneIKConstraint rightIK;

    [Header("参数")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float footOffset = 0.1f;
    [SerializeField] private float rayLength = 1.5f;
    [SerializeField] private float ySmooth = 15f;

    [Header("动画曲线名")]
    [SerializeField] private string leftCurveName = "l_Foot_IK";
    [SerializeField] private string rightCurveName = "r_Foot_IK";

    private float leftSmoothY;
    private float rightSmoothY;

    private void Start()
    {
        if (player == null) player = GetComponent<Player>();
        if (animator == null) animator = GetComponent<Animator>();
        if (leftFootBone != null) leftSmoothY = leftFootBone.position.y;
        if (rightFootBone != null) rightSmoothY = rightFootBone.position.y;
    }

    private void LateUpdate()
    {
        // 从动画曲线读权重
        float leftWeight = animator.GetFloat(leftCurveName);
        float rightWeight = animator.GetFloat(rightCurveName);

        // 非IK状态（攻击等）权重归零
        if (!player.CanUseFootIK())
        {
            leftWeight = 0f;
            rightWeight = 0f;
        }

        // 设置 Constraint 权重
        if (leftIK != null) leftIK.weight = leftWeight;
        if (rightIK != null) rightIK.weight = rightWeight;

        // 更新 IK Target 位置
        ProcessFoot(leftFootBone, leftFootIKTarget, ref leftSmoothY);
        ProcessFoot(rightFootBone, rightFootIKTarget, ref rightSmoothY);
    }

    private void ProcessFoot(Transform bone, Transform ikTarget, ref float smoothY)
    {
        if (bone == null || ikTarget == null) return;

        Vector3 rayOrigin = bone.position + Vector3.up * 0.5f;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit,
                rayLength, groundLayer))
        {
            float targetY = hit.point.y + footOffset;
            smoothY = Mathf.Lerp(smoothY, targetY, Time.deltaTime * ySmooth);

#if UNITY_EDITOR
            Debug.DrawLine(rayOrigin, hit.point, Color.green);
#endif
        }
        else
        {
            smoothY = Mathf.Lerp(smoothY, bone.position.y, Time.deltaTime * ySmooth);
        }

        ikTarget.position = new Vector3(bone.position.x, smoothY, bone.position.z);
        ikTarget.rotation = bone.rotation;
    }
}