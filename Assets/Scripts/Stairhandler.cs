using UnityEngine;

/// <summary>
/// 楼梯攀爬处理器 v3 - 速度驱动，平滑上下台阶
/// 
/// 原理：
/// 检测到台阶 → 给 Rigidbody 一个向上的速度（不是直接改位置）
/// 物理系统自己平滑地把角色推上去，不会有跳帧的感觉
/// </summary>
public class StairHandler : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private Player player;
    [SerializeField] private Rigidbody rb;

    [Header("台阶参数")]
    [SerializeField] private float maxStepHeight = 0.35f;
    [SerializeField] private float minStepHeight = 0.02f;
    [SerializeField] private float forwardCheckDist = 0.5f;

    [Header("平滑参数")]
    [Tooltip("上台阶的抬升速度")]
    [SerializeField] private float stepUpSpeed = 3f;
    [Tooltip("下台阶的吸附速度")]
    [SerializeField] private float stepDownSpeed = 5f;
    [Tooltip("下台阶检测距离")]
    [SerializeField] private float groundSnapDist = 0.5f;

    [Header("Layer")]
    [SerializeField] private LayerMask groundLayer;

    private readonly float[] sideOffsets = { 0f, -0.15f, 0.15f };

    private void Start()
    {
        if (player == null) player = GetComponent<Player>();
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float xIn = player.InputHandler.xInput;
        float yIn = player.InputHandler.yInput;

        if (Mathf.Abs(xIn) < 0.01f && Mathf.Abs(yIn) < 0.01f)
            return;
        if (!player.CanUseFootIK())
            return;

        float stepTarget = DetectStep();

        if (stepTarget > 0f)
        {
            ApplyStepUp(stepTarget);
        }
        else
        {
            SnapDown();
        }
    }

    /// <summary>
    /// 检测前方是否有台阶，返回台阶顶面Y坐标，没有则返回-1
    /// </summary>
    private float DetectStep()
    {
        Vector3 moveDir = transform.forward;
        Vector3 basePos = transform.position;
        float highestStep = -1f;

        foreach (float offset in sideOffsets)
        {
            Vector3 side = transform.right * offset;

            // 低处向前：检测有没有挡住
            Vector3 lowOrigin = basePos + side + Vector3.up * minStepHeight;
            if (!Physics.Raycast(lowOrigin, moveDir, out RaycastHit lowHit,
                    forwardCheckDist, groundLayer))
                continue;

            // 高处向前：如果也挡住了就是墙，不是台阶
            Vector3 highOrigin = basePos + side + Vector3.up * (maxStepHeight + 0.01f);
            if (Physics.Raycast(highOrigin, moveDir, forwardCheckDist, groundLayer))
                continue;

            // 从上往下找台阶顶面
            Vector3 topOrigin = lowHit.point + moveDir * 0.05f + Vector3.up * maxStepHeight;
            if (Physics.Raycast(topOrigin, Vector3.down, out RaycastHit topHit,
                    maxStepHeight + 0.1f, groundLayer))
            {
                float h = topHit.point.y - basePos.y;
                if (h >= minStepHeight && h <= maxStepHeight)
                {
                    highestStep = Mathf.Max(highestStep, topHit.point.y);
                }

#if UNITY_EDITOR
                Debug.DrawLine(topOrigin, topHit.point, Color.yellow);
#endif
            }

#if UNITY_EDITOR
            Debug.DrawLine(lowOrigin, lowHit.point, Color.red);
#endif
        }

        return highestStep;
    }

    /// <summary>
    /// 用速度平滑抬升角色到台阶高度
    /// </summary>
    private void ApplyStepUp(float targetY)
    {
        float currentY = transform.position.y;
        float diff = targetY - currentY;

        if (diff > 0.01f)
        {
            // 根据高度差计算需要的向上速度
            // diff 越大速度越快，但有上限
            float upVelocity = Mathf.Clamp(diff * stepUpSpeed / Time.fixedDeltaTime, 
                                            0.5f, 8f);

            Vector3 vel = rb.velocity;
            vel.y = Mathf.Max(vel.y, upVelocity);
            rb.velocity = vel;

#if UNITY_EDITOR
            Debug.DrawRay(new Vector3(transform.position.x, targetY, transform.position.z),
                Vector3.up * 0.3f, Color.green);
#endif
        }
    }

    /// <summary>
    /// 下台阶时平滑吸附地面
    /// </summary>
    private void SnapDown()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit,
                groundSnapDist + 0.1f, groundLayer))
        {
            float gap = transform.position.y - hit.point.y;

            if (gap > 0.05f && gap < groundSnapDist)
            {
                // 给一个向下的速度，让角色平滑贴地
                Vector3 vel = rb.velocity;
                vel.y = -gap * stepDownSpeed;
                rb.velocity = vel;
            }

#if UNITY_EDITOR
            Debug.DrawLine(rayOrigin, hit.point, Color.cyan);
#endif
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Vector3 center = transform.position + Vector3.up * maxStepHeight * 0.5f
                         + transform.forward * forwardCheckDist * 0.5f;
        Gizmos.DrawWireCube(center, new Vector3(0.4f, maxStepHeight, forwardCheckDist));
    }
#endif
}