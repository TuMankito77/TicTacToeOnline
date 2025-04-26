namespace TicTacToeOnline.Runtime.Input.TouchControls
{
    using UnityEngine;
    using UnityEngine.InputSystem.Layouts;
    using UnityEngine.InputSystem.OnScreen;

    using TicTacToeOnline.Ui.Views;
    
    public class InputActionFromOnScreenButton : OnScreenControl
    {
        [InputControl(layout = "Button")]
        [SerializeField]
        private string controlPathSelected = string.Empty;

        [SerializeField]
        private BaseButton linkedButton = null;

        protected override string controlPathInternal 
        { 
            get => controlPathSelected; 
            set => controlPathSelected = value; 
        }

        #region Unity Methods

        private void Awake()
        {
            linkedButton.onButtonPressed += OnButtonPressed;
        }

        private void OnDestroy()
        {
            linkedButton.onButtonPressed -= OnButtonPressed;
        }

        #endregion

        private void OnButtonPressed()
        {
            SendValueToControl(1.0f);
            SendValueToControl(0.0f);
        }
    }
}

