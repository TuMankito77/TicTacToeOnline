namespace TicTacToeOnline.Ui.Views
{
    using UnityEngine;
    
    using TMPro;
    using System;

    public class MainMenuView : BaseView
    {
        [SerializeField]
        private BaseButton createSessionButton = null;

        [SerializeField]
        private BaseButton findSessionButton = null;

        [SerializeField]
        private TMP_InputField playerNameText = null;

        public Action<string> onPlayerNameChanged = null;

        #region Unity Methods

        private void OnEnable()
        {
            createSessionButton.onButtonPressed += OnCreateSessionButtonPressed;
            findSessionButton.onButtonPressed += OnFindSessionButtonPressed;
        }

        private void OnDestroy()
        {
            createSessionButton.onButtonPressed -= OnCreateSessionButtonPressed;
            findSessionButton.onButtonPressed -= OnFindSessionButtonPressed;
        }

        #endregion

        private void OnCreateSessionButtonPressed()
        {
            if(!IsPlayerNameValid())
            {
                return;
            }

            viewManager.DisplayView<CreateSessionView>();
            viewManager.RemoveView<MainMenuView>();
        }

        private void OnFindSessionButtonPressed()
        {
            if(!IsPlayerNameValid())
            {
                return;
            }

            viewManager.DisplayView<FindSessionView>();
            viewManager.RemoveView<MainMenuView>();
        }

        private bool IsPlayerNameValid()
        {
            if (string.IsNullOrEmpty(playerNameText.text) || string.IsNullOrWhiteSpace(playerNameText.text))
            {
                MessageView messageView = viewManager.DisplayView<MessageView>();

                if(messageView != null)
                {
                    messageView.SetMessageText($"The player name cannot be empty if you to create or find a session.");
                }

                return false;
            }
            else
            {
                onPlayerNameChanged?.Invoke(playerNameText.text);
                return true;
            }
        }
    }
}

