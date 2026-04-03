using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("角色参数")]
    [SerializeField] private Transform target;
    [SerializeField] private PlayerInputHandler inputHandler;

    [Header("相机参数")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, 0f);
    [SerializeField] private float distance = 5f;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 15f;
    [SerializeField] private float zoomSpeed = 0.5f;
    [SerializeField] private float rotationSpeed = 0.3f;
    [SerializeField] private float minPitch = -20f;
    [SerializeField] private float maxPitch = 60f;
    [SerializeField] private float followSmooth = 0.05f;
    [SerializeField] private float zoomSmooth = 8f;

    [Header("过肩瞄准参数")]
    [SerializeField] private float aimDistance = 2f;
    [SerializeField] private Vector3 aimOffset = new Vector3(0.5f, 1.6f, 0f);
    [SerializeField] private float aimTransitionSpeed = 8f;
    [SerializeField] private float aimMinPitch = -30f;   
    [SerializeField] private float aimMaxPitch = 40f;

    private float yaw;
    private float pitch = 20f;
    private float currentDistance;
    private Vector3 smoothVelocity;
    private Vector3 currentOffset;
    private float targetDistance;

    private void Start()
    {
        currentDistance = distance;
        currentOffset = offset;
        targetDistance = distance;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector2 delta = Mouse.current.delta.ReadValue();
        yaw += delta.x * rotationSpeed;
        pitch -= delta.y * rotationSpeed;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        bool isAiming = inputHandler != null && inputHandler.isAiming;

        if (isAiming)
        {
            pitch = Mathf.Clamp(pitch, aimMinPitch, aimMaxPitch);
            targetDistance = aimDistance;
            currentOffset = Vector3.Lerp(currentOffset, aimOffset, aimTransitionSpeed * Time.deltaTime);
        }
        else
        {
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            float scroll = Mouse.current.scroll.ReadValue().y;
            distance -= scroll * zoomSpeed * Time.deltaTime;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
            targetDistance = distance;
            currentOffset = Vector3.Lerp(currentOffset, offset, Time.deltaTime * aimTransitionSpeed);
        }

        currentDistance = Mathf.Lerp(currentDistance, targetDistance,
            Time.deltaTime * zoomSmooth);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 lookAt = target.position + rotation * currentOffset;
        Vector3 targetPos = lookAt + rotation * new Vector3(0f, 0f, -currentDistance);

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref smoothVelocity, followSmooth);
        transform.LookAt(lookAt);
    }
}