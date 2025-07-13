using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; protected set; }

    private InputSystem_Actions _inputActions;

    public InputSystem_Actions InputActions { get => _inputActions; }

    private void Awake()
    {
        Instance = this;

        _inputActions = new InputSystem_Actions();
        _inputActions.Player.Enable();
    }

    public Vector2 GetMovementVector()
    {
        return _inputActions.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetLookVector()
    {
        return _inputActions.Player.Look.ReadValue<Vector2>();
    }
}