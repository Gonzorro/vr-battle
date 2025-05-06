using UnityEngine;
using UnityEngine.InputSystem;

public class Debugger : MonoBehaviour
{
    [SerializeField] private InputActionReference inputA;
    [SerializeField] private InputActionReference inputB;

    private void OnEnable()
    {
        if (inputA != null) inputA.action.performed += OnInputA;
        if (inputB != null) inputB.action.performed += OnInputB;

        inputA.action.Enable();
        inputB.action.Enable();
        Debug.Log("Enabled");
    }

    private void OnDisable()
    {
        if (inputA != null) inputA.action.performed -= OnInputA;
        if (inputB != null) inputB.action.performed -= OnInputB;
        inputA.action.Disable();
        inputB.action.Disable();
    }

    private void OnInputA(InputAction.CallbackContext ctx) =>
        Debug.Log($"Input A triggered: {ctx.control}");

    private void OnInputB(InputAction.CallbackContext ctx) =>
        Debug.Log($"Input B triggered: {ctx.control}");
}
