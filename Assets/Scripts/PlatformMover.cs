using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    private float _moveSpeed = 10.0f;
    private Vector3 _direction;
    private bool _isMoving = false;
    
    private void Update()
    {
        if (!_isMoving) return;
        
        transform.Translate(_direction * (_moveSpeed * Time.deltaTime), Space.World);
    }

    public void SetPlatformMovingSettings(Vector3 targetPoint)
    {
        _isMoving = true;
        _direction = (targetPoint - transform.position).normalized;
    }

    public void StopPlatform()
    {
        _isMoving = false;
    }
}
