using System.Collections;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    private Vector3 _cameraStartPosition;
    private readonly float _moveDuration = 0.5f;
    
    private void Start()
    {
        if (CheckSpawnerInstanceExist()) return;
        Spawner.Instance.OnSpawnerMovedUp += OnSpawnerMovedUp;
        
        _cameraStartPosition = transform.position;
        var startGameCameraPositionY = 8.0f; 
        transform.position = new Vector3(transform.position.x, startGameCameraPositionY, transform.position.z);

        var startGameMoveUpDuration = 2.0f;
        StartCoroutine(MoveCameraUpCoroutine(_cameraStartPosition, startGameMoveUpDuration));
    }
    
    private static bool CheckSpawnerInstanceExist()
    {
        if (Spawner.Instance) return false;
        
        Debug.Log("Spawner Instance is not found!");
        Time.timeScale = 0.0f;
            
        return true;
    }

    private void OnSpawnerMovedUp(float offsetY)
    {
        var cameraTargetPosition = new Vector3(transform.position.x, transform.position.y + offsetY, transform.position.z);
        StartCoroutine(MoveCameraUpCoroutine(cameraTargetPosition, _moveDuration));
    }
    
    private IEnumerator MoveCameraUpCoroutine(Vector3 targetPosition, float duration)
    {
        var startPositon = transform.position;
        var time = 0f;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPositon, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        
        transform.position = targetPosition;
    }
    
    private void OnDisable()
    {
        if (CheckSpawnerInstanceExist()) return;
        Spawner.Instance.OnSpawnerMovedUp -= OnSpawnerMovedUp;
    }

    private void OnDestroy()
    {
        if (CheckSpawnerInstanceExist()) return;
        Spawner.Instance.OnSpawnerMovedUp -= OnSpawnerMovedUp;
    }
}
