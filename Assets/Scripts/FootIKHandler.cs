using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
/// 脚部IK + 台阶自适应
/// 
/// 原理：
/// 1. 两只脚分别向下射线找到各自的地面高度
/// 2. IK 把脚放到检测到的地面上（可能是台阶面）
/// 3. 取两只脚中更高的那个地面高度，平滑抬升身体
/// 4. 身体升上去后，CapsuleCollider 不再卡台阶边
/// 
/// 不需要单独的 StairHandler
/// </summary>
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

    [Header("Constraint（代码控制权重）")]
    [SerializeField] private TwoBoneIKConstraint leftIK;
    [SerializeField] private TwoBoneIKConstraint rightIK;

    [Header("射线参数")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float footOffset = 0.1f;
    [Tooltip("射线从脚向上偏移多高开始（要高于最大台阶高度）")]
    [SerializeField] private float rayStartHeight = 0.6f;
    [SerializeField] private float rayLength = 1.5f;

    [Header("平滑")]
    [SerializeField] private float footYSmooth = 15f;
    [Tooltip("身体跟随高度变化的速度")]
    [SerializeField] private float bodyYSmooth = 8f;

    [Header("身体高度补偿")]
    [Tooltip("身体最多抬升多少（防止飞天）")]
    [SerializeField] private float maxBodyRaise = 0.5f;

    [Header("动画曲线名")]
    [SerializeField] private string leftCurveName = "L_Foot_IK";
    [SerializeField] private string rightCurveName = "R_Foot_IK";

    private float leftSmoothY;
    private float rightSmoothY;
    private float leftGroundY;
    private float rightGroundY;
    private float bodyOffsetY;

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

    private void LateUpdate()
    {
        // 读动画曲线，设 Constraint 权重
        float leftWeight = animator.GetFloat(leftCurveName);
        float rightWeight = animator.GetFloat(rightCurveName);

        if (!player.CanUseFootIK())
        {
            leftWeight = 0f;
            rightWeight = 0f;
        }

        if (leftIK != null) leftIK.weight = leftWeight;
        if (rightIK != null) rightIK.weight = rightWeight;

        // 处理每只脚的 IK 位置
        ProcessFoot(leftFootBone, leftFootIKTarget, ref leftSmoothY, ref leftGroundY);
        ProcessFoot(rightFootBone, rightFootIKTarget, ref rightSmoothY, ref rightGroundY);
    }

    private void FixedUpdate()
    {
        if (!player.CanUseFootIK()) return;

        // 身体高度补偿：跟随较高的那只脚的地面
        AdjustBodyHeight();
    }

    private void ProcessFoot(Transform bone, Transform ikTarget,
                              ref float smoothY, ref float groundY)
    {
        if (bone == null || ikTarget == null) return;

        // 射线从脚骨骼上方开始向下
        Vector3 rayOrigin = bone.position + Vector3.up * rayStartHeight;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit,
                rayLength, groundLayer))
        {
            // 记录这只脚下方的实际地面高度
            groundY = hit.point.y;

            float targetY = hit.point.y + footOffset;
            smoothY = Mathf.Lerp(smoothY, targetY, Time.deltaTime * footYSmooth);

#if UNITY_EDITOR
            Debug.DrawLine(rayOrigin, hit.point, Color.green);
#endif
        }
        else
        {
            smoothY = Mathf.Lerp(smoothY, bone.position.y, Time.deltaTime * footYSmooth);
        }

        // XZ 跟骨骼，Y 用平滑值
        ikTarget.position = new Vector3(bone.position.x, smoothY, bone.position.z);
        ikTarget.rotation = bone.rotation;
    }

    /// <summary>
    /// 根据两只脚的地面高度调整身体高度
    /// 取较高的地面作为参考，平滑抬升身体
    /// </summary>
    private void AdjustBodyHeight()
    {
        // 两只脚的地面高度，取较高的那个
        float higherGroundY = Mathf.Max(leftGroundY, rightGroundY);

        // 身体当前底部位置
        float currentBaseY = rb.position.y;

        // 需要抬升多少
        float targetRaise = higherGroundY - currentBaseY;

        // 限制范围：不要抬太高，也不要在平地上乱动
        if (targetRaise > 0.02f && targetRaise <= maxBodyRaise)
        {
            float newY = Mathf.MoveTowards(currentBaseY, higherGroundY,
                                            bodyYSmooth * Time.fixedDeltaTime);
            Vector3 pos = rb.position;
            pos.y = newY;
            rb.position = pos;

            // 清掉重力的下拉速度
            Vector3 vel = rb.velocity;
            if (vel.y < 0f) vel.y = 0f;
            rb.velocity = vel;
        }

        // 下台阶：身体高于两只脚的地面
        float lowerGroundY = Mathf.Min(leftGroundY, rightGroundY);
        float gap = currentBaseY - higherGroundY;
        if (gap > 0.05f && gap < maxBodyRaise)
        {
            float newY = Mathf.MoveTowards(currentBaseY, higherGroundY,
                                            bodyYSmooth * Time.fixedDeltaTime);
            Vector3 pos = rb.position;
            pos.y = newY;
            rb.position = pos;

            Vector3 vel = rb.velocity;
            vel.y = 0f;
            rb.velocity = vel;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (leftFootBone != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(new Vector3(leftFootBone.position.x, leftGroundY, leftFootBone.position.z), 0.05f);
        }
        if (rightFootBone != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(new Vector3(rightFootBone.position.x, rightGroundY, rightFootBone.position.z), 0.05f);
        }
    }
#endif
}