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
        private BaseButton closeMessageButtonOk = null;

        [SerializeField]
        private BaseButton closeMessageButtonCancel = null;

        public Action onCloseButtonOkPressed = null;
        public Action onCloseButtonCancelPressed = null;

        #region Unity Methods

        private void OnEnable()
        {
            closeMessageButtonOk.onButtonPressed += OnCloseMessageButtonOkPressed;
            closeMessageButtonCancel.onButtonPressed += OnCloseMessageButtonCancelPressed;
        }

        private void OnDisable()
        {
            closeMessageButtonOk.onButtonPressed -= OnCloseMessageButtonOkPressed;
            closeMessageButtonCancel.onButtonPressed -= onCloseButtonCancelPressed;
        }

        #endregion

        public void SetMessageText(string message)
        {
            messageText.text = message;
        }

        public void ActivateCloseMessageButtonCancel()
        {
            closeMessageButtonCancel.gameObject.SetActive(true);
            ForceRebuildLayout();
        }

        private void OnCloseMessageButtonOkPressed()
        {
            viewManager.RemoveView<MessageView>();
            onCloseButtonOkPressed?.Invoke();
        }

        private void OnCloseMessageButtonCancelPressed()
        {
            viewManager.RemoveView<MessageView>();
            onCloseButtonCancelPressed?.Invoke();
        }
    }
}

