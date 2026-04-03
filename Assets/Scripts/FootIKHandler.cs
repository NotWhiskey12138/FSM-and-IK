using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FootIKHandler : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private Player player;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;

    [Header("IK Targets")]
    [SerializeField] private Transform leftFootIKTarget;
    [SerializeField] private Transform rightFootIKTarget;

    [Header("脚骨骼")]
    [SerializeField] private Transform leftFootBone;
    [SerializeField] private Transform rightFootBone;

    [Header("脚上骨骼插件")]
    [SerializeField] private TwoBoneIKConstraint leftIK;
    [SerializeField] private TwoBoneIKConstraint rightIK;

    [Header("射线参数")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float footOffset = 0.1f;
    [SerializeField] private float rayStartHeight = 0.6f;
    [SerializeField] private float rayLength = 1.5f;

    [Header("平滑")]
    [SerializeField] private float footYSmooth = 15f;

    [Header("动画名")]
    [SerializeField] private string leftCurveName = "l_Foot_IK";
    [SerializeField] private string rightCurveName = "r_Foot_IK";

    private float leftSmoothY;
    private float rightSmoothY;
    private float leftGroundY;
    private float rightGroundY;

    private bool isFootIKActive = true;  // 外部控制

    private void Start()
    {
        if (player == null) player = GetComponent<Player>();
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody>();

        if (leftFootBone != null)
        {
            leftSmoothY = leftFootBone.position.y;
            leftGroundY = transform.position.y;
        }
        if (rightFootBone != null)
        {
            rightSmoothY = rightFootBone.position.y;
            rightGroundY = transform.position.y;
        }
    }

    // ===== 外部接口 =====

    /// <summary>
    /// 状态机调用：开关脚部IK
    /// </summary>
    public void SetFootIK(bool active)
    {
        isFootIKActive = active;
    }

    // ===== 内部逻辑 =====

    private void LateUpdate()
    {
        float leftWeight = 0f;
        float rightWeight = 0f;

        if (isFootIKActive)
        {
            leftWeight = animator.GetFloat(leftCurveName);
            rightWeight = animator.GetFloat(rightCurveName);
        }

        if (leftIK != null) leftIK.weight = leftWeight;
        if (rightIK != null) rightIK.weight = rightWeight;

        ProcessFoot(leftFootBone, leftFootIKTarget, ref leftSmoothY, ref leftGroundY);
        ProcessFoot(rightFootBone, rightFootIKTarget, ref rightSmoothY, ref rightGroundY);
    }

    private void ProcessFoot(Transform bone, Transform ikTarget,
                              ref float smoothY, ref float groundY)
    {
        if (bone == null || ikTarget == null) return;

        Vector3 rayOrigin = bone.position + Vector3.up * rayStartHeight;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit,
                rayLength, groundLayer))
        {
            groundY = hit.point.y;

            float targetY = hit.point.y + footOffset;
            smoothY = Mathf.Lerp(smoothY, targetY, Time.deltaTime * footYSmooth);
        }
        else
        {
            smoothY = Mathf.Lerp(smoothY, bone.position.y, Time.deltaTime * footYSmooth);
        }

        ikTarget.position = new Vector3(bone.position.x, smoothY, bone.position.z);

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit rotHit, rayLength, groundLayer))
        {
            Quaternion slopeRot = Quaternion.FromToRotation(Vector3.up, rotHit.normal);
            ikTarget.rotation = slopeRot * bone.rotation;
        }
    }
}