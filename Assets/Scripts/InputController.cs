using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    private InputActions _inputActions;

    private void OnEnable()
    {
        _inputActions ??= new InputActions();
        _inputActions.GamePlay.Tap.performed += OnTapPerformed;

        _inputActions.Enable();
    }

    private void OnTapPerformed(InputAction.CallbackContext ctx)
    {
        Spawner.Instance.GenerateNewPlatform("NewPlatform", new Vector3(0.5f, 1.0f, 0.0f), 4f, 1.0f, 5f);
        Spawner.Instance.GenerateNewPlatform("NewPlatform", new Vector3(3.0f, 2.0f, 0.0f), 1f, 1.0f, 5f);
    }

    private void OnDisable()
    {
        if (_inputActions == null) return;

        _inputActions.GamePlay.Tap.performed -= OnTapPerformed;
        _inputActions.Disable();
    }

    private void OnDestroy()
    {
        if (_inputActions == null) return;
        
        _inputActions.GamePlay.Tap.performed -= OnTapPerformed;
        _inputActions.Disable();
    }
}