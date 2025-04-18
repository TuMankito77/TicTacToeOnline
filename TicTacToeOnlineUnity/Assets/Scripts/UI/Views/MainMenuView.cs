namespace TicTacToeOnline.Ui.Views
{
    using UnityEngine;
    using UnityEngine.UI;
    
    using TMPro;
    using System;

    public class MainMenuView : BaseView
    {
        [SerializeField]
        private Button createSessionButton = null;

        [SerializeField]
        private Button findSessionButton = null;

        [SerializeField]
        private TMP_InputField playerNameText = null;

        public Action<string> onPlayerNameChanged = null;

        #region Unity Methods

        private void Start()
        {
            createSessionButton.onClick.AddListener(OnCreateSessionButtonPressed);
            findSessionButton.onClick.AddListener(OnFindSessionButtonPressed);
        }

        #endregion

        private void OnCreateSessionButtonPressed()
        {
            if(!IsPlayerNameValid())
            {
                return;
            }

            viewManager.DisplayView(typeof(CreateSessionView));
            viewManager.RemoveView(GetType());
        }

        private void OnFindSessionButtonPressed()
        {
            if(!IsPlayerNameValid())
            {
                return;
            }

            viewManager.DisplayView(typeof(FindSessionView));
            viewManager.RemoveView(GetType());
        }

        private bool IsPlayerNameValid()
        {
            if (string.IsNullOrEmpty(playerNameText.text) || string.IsNullOrWhiteSpace(playerNameText.text))
            {
                MessageView messageView = viewManager.DisplayView(typeof(MessageView)) as MessageView;

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

