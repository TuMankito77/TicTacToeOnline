namespace TicTacToeOnline.Ui.Views
{
    using System;
    
    using UnityEngine;
    using UnityEngine.UI;
    
    using TMPro;

    public class MessageView : BaseView
    {
        [SerializeField]
        private TextMeshProUGUI messageText = null;

        [SerializeField]
        private Button closeMessageButton = null;

        public Action onCloseButtonPressed = null;

        #region Unity Methods

        private void Start()
        {
            closeMessageButton.onClick.AddListener(OnCloseMessageButtonPressed);
        }

        #endregion

        public void SetMessageText(string message)
        {
            messageText.text = message;
        }

        private void OnCloseMessageButtonPressed()
        {
            onCloseButtonPressed?.Invoke();
            viewManager.RemoveView(this.GetType());
        }
    }
}

