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
    [Tooltip("射线从脚向上偏移多高开始（要高于最大台阶高度）")]
    [SerializeField] private float rayStartHeight = 0.6f;
    [SerializeField] private float rayLength = 1.5f;

    [Header("平滑")]
    [SerializeField] private float footYSmooth = 15f;
    [Tooltip("身体跟随高度变化的速度")]
    [SerializeField] private float bodyYSmooth = 8f;

    [Header("身体高度补偿")]
    [SerializeField] private float maxBodyRaise = 0.5f;

    [Header("动画名")]
    [SerializeField] private string leftCurveName = "l_Foot_IK";
    [SerializeField] private string rightCurveName = "r_Foot_IK";

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
        float leftWeight = animator.GetFloat(leftCurveName);
        float rightWeight = animator.GetFloat(rightCurveName);

        if (!player.CanUseFootIK())
        {
            leftWeight = 0f;
            rightWeight = 0f;
        }

        if (leftIK != null) leftIK.weight = leftWeight;
        if (rightIK != null) rightIK.weight = rightWeight;

        ProcessFoot(leftFootBone, leftFootIKTarget, ref leftSmoothY, ref leftGroundY);
        ProcessFoot(rightFootBone, rightFootIKTarget, ref rightSmoothY, ref rightGroundY);
    }

    private void FixedUpdate()
    {
        if (!player.CanUseFootIK()) return;

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

#if UNITY_EDITOR
            Debug.DrawLine(rayOrigin, hit.point, Color.green);
#endif
        }
        else
        {
            smoothY = Mathf.Lerp(smoothY, bone.position.y, Time.deltaTime * footYSmooth);
        }

        ikTarget.position = new Vector3(bone.position.x, smoothY, bone.position.z);
        ikTarget.rotation = bone.rotation;
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