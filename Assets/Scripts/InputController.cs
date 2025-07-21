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
        Spawner.Instance.CurrentPlatformObj.GetComponent<PlatformMover>().StopPlatform();
        PlatformCutter.Instance.TryCutCurrentPlatform();
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