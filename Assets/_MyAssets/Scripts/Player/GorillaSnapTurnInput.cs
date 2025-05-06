using UnityEngine;
using UnityEngine.InputSystem;

public class GorillaSnapTurnInput : MonoBehaviour
{
    [SerializeField] private InputActionReference turnInput;
    [SerializeField] private float turnAmount = 45f;
    [SerializeField] private float debounceTime = 0.5f;

    private float lastTurnTime;

    private void OnEnable() => turnInput.action.performed += OnTurnInput;

    private void OnDisable() => turnInput.action.performed -= OnTurnInput;

    private void OnTurnInput(InputAction.CallbackContext context)
    {
        if (Time.time - lastTurnTime < debounceTime) return;

        Vector2 value = context.ReadValue<Vector2>();
        if (value.x > 0.5f)
        {
            transform.Rotate(Vector3.up, turnAmount);
            lastTurnTime = Time.time;
        }
        else if (value.x < -0.5f)
        {
            transform.Rotate(Vector3.up, -turnAmount);
            lastTurnTime = Time.time;
        }
    }
}
