using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("角色参数")]
    [SerializeField] private Transform target;          
    
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

    private float _yaw;              
    private float _pitch = 20f;      
    private float _currentDistance;  
    private Vector3 _smoothVelocity; 

    private void Start()
    {
        _currentDistance = distance;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector2 delta = Mouse.current.delta.ReadValue();
        _yaw += delta.x * rotationSpeed;
        _pitch -= delta.y * rotationSpeed;  
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch); 

        float scroll = Mouse.current.scroll.ReadValue().y;
        distance -= scroll * zoomSpeed * Time.deltaTime;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        _currentDistance = Mathf.Lerp(_currentDistance, distance, Time.deltaTime * zoomSmooth);

        Vector3 lookAt = target.position + offset;

        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 targetPos = lookAt + rotation * new Vector3(0f, 0f, -_currentDistance);

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _smoothVelocity, followSmooth);
        transform.LookAt(lookAt);
    }
}