namespace TicTacToeOnline.Ui.Views
{
    using System;
    
    using UnityEngine;
    
    using TMPro;

    public class MessageView : BaseView
    {
        [SerializeField]
        private TextMeshProUGUI messageText = null;

        [SerializeField]
        private BaseButton closeMessageButton = null;

        public Action onCloseButtonPressed = null;

        #region Unity Methods

        private void OnEnable()
        {
            closeMessageButton.onButtonPressed += OnCloseMessageButtonPressed;
        }

        private void OnDisable()
        {
            closeMessageButton.onButtonPressed -= OnCloseMessageButtonPressed;
        }

        #endregion

        public void SetMessageText(string message)
        {
            messageText.text = message;
        }

        private void OnCloseMessageButtonPressed()
        {
            onCloseButtonPressed?.Invoke();
            viewManager.RemoveView<MessageView>();
        }
    }
}

