using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;

namespace Enthalpy.Input
{
    [CreateAssetMenu(fileName = "PlayerInputReader", menuName = "Scriptable Objects/PlayerInputReader")]
    public class PlayerInputReader : ScriptableObject, IPlayerInputReader, IPlayerActions
    {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<bool> Jump = delegate { };

        private PlayerInputActions _inputActions;

        public Vector2 Direction => _inputActions.Player.Move.ReadValue<Vector2>();
        public bool IsJumpPressed => _inputActions.Player.Jump.IsPressed();

        public void EnablePlayerInputActions()
        {
            if (_inputActions == null)
            {
                _inputActions = new PlayerInputActions();
                _inputActions.Player.SetCallbacks(this);
            }

            _inputActions.Enable();
        }

        public void DisablePlayerInputActions()
        {
            _inputActions?.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Move.Invoke(context.ReadValue<Vector2>());
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Jump.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Jump.Invoke(false);
                    break;
            }
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            // nope
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            // nope
        }

        public void OnMouseControlCamera(InputAction.CallbackContext context)
        {
            // nope
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            // nope
        }
    }
}
