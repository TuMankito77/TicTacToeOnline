namespace TicTacToeOnline.Ui.Views
{
    using UnityEngine;
    
    using TMPro;
    using TicTacToeOnline.Gameplay;

    public class MainMenuView : BaseView
    {
        [SerializeField]
        private BaseButton createSessionButton = null;

        [SerializeField]
        private BaseButton findSessionButton = null;

        [SerializeField]
        private BaseButton settingsButton = null;

        [SerializeField]
        private BaseButton quitButton = null;

        [SerializeField]
        private TMP_InputField playerNameText = null;

        #region Unity Methods

        private void OnEnable()
        {
            createSessionButton.onButtonPressed += OnCreateSessionButtonPressed;
            findSessionButton.onButtonPressed += OnFindSessionButtonPressed;
            settingsButton.onButtonPressed += OnSettingsButtonPressed;
            quitButton.onButtonPressed += OnQuitButtonPressed;
        }

        private void OnDestroy()
        {
            createSessionButton.onButtonPressed -= OnCreateSessionButtonPressed;
            findSessionButton.onButtonPressed -= OnFindSessionButtonPressed;
            settingsButton.onButtonPressed -= OnSettingsButtonPressed;
            quitButton.onButtonPressed -= OnQuitButtonPressed;
        }

        #endregion

        protected override void OnGoBackActionPerformed()
        {
            base.OnGoBackActionPerformed();
            DisplayQuitAppMessage();   
        }

        private void OnCreateSessionButtonPressed()
        {
            if(!IsPlayerNameValid())
            {
                return;
            }

            viewManager.RemoveView<MainMenuView>();
            viewManager.DisplayView<CreateSessionView>();
        }

        private void OnFindSessionButtonPressed()
        {
            if(!IsPlayerNameValid())
            {
                return;
            }

            viewManager.RemoveView<MainMenuView>();
            viewManager.DisplayView<FindSessionView>();
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
                GameManager.Instance.PlayerName = playerNameText.text;
                return true;
            }
        }

        private void OnQuitButtonPressed()
        {
            DisplayQuitAppMessage();
        }

        private void DisplayQuitAppMessage()
        {
            MessageView messageView = viewManager.DisplayView<MessageView>();
            messageView.SetMessageText($"Are you sure you want to quit?");
            messageView.ActivateCloseMessageButtonCancel();

            void OnCloseButtonOkPressed()
            {
                messageView.onCloseButtonOkPressed -= OnCloseButtonOkPressed;
                Application.Quit();
            }

            messageView.onCloseButtonOkPressed += OnCloseButtonOkPressed;
        }

        private void OnSettingsButtonPressed()
        {
            viewManager.RemoveView<MainMenuView>();
            viewManager.DisplayView<SettingsView>();
        }
    }
}

