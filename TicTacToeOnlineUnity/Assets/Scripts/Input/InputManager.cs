namespace TicTacToeOnline.Input
{
    using System;
    using TicTacToeOnline.Core;
    using UnityEngine.InputSystem;

    public class InputManager : SingletonBehavior<InputManager>
    {
        public Action onGoBackActionPerformed = null;
        
        private DefaultInputActions inputActions = null;

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            inputActions = new DefaultInputActions();
        }

        private void Start()
        {
            inputActions.UI.Enable();
            inputActions.UI.Cancel.performed += OnGoBackActionPerformed;
        }

        private void OnDestroy()
        {
            inputActions.UI.Cancel.performed -= OnGoBackActionPerformed;
            inputActions.UI.Disable();
        }

        #endregion

        private void OnGoBackActionPerformed(InputAction.CallbackContext obj)
        {
            onGoBackActionPerformed?.Invoke();
        }
    }
}

