namespace TicTacToeOnline.Ui.Views
{
    using UnityEngine;
    
    using Unity.Services.Lobbies.Models;
    
    using TMPro;
    
    using TicTacToeOnline.Gameplay;
    using TicTacToeOnline.Networking;
    using TicTacToeOnline.Input;

    public class CreateSessionView : BaseView
    {
        [SerializeField]
        private TMP_InputField matchNameInputField = null;

        [SerializeField]
        private BaseButton createButton = null;

        #region Unity Methods

        private void OnEnable()
        {
            createButton.onButtonPressed += OnCreateButtonPressed;
            InputManager.Instance.onGoBackActionPerformed += OnGoBackActionPerformed;
        }

        private void OnDisable()
        {
            createButton.onButtonPressed -= OnCreateButtonPressed;
            InputManager.Instance.onGoBackActionPerformed -= OnGoBackActionPerformed;
        }

        #endregion

        private void OnCreateButtonPressed()
        {
            if(string.IsNullOrEmpty(matchNameInputField.text) || 
                string.IsNullOrWhiteSpace(matchNameInputField.text))
            {
                MessageView messageView = viewManager.DisplayView<MessageView>();

                if(messageView)
                {
                    messageView.SetMessageText($"The match name is empty, please make sure to type a match name.");
                }

                return;
            }

            viewManager.DisplayView<LoadingView>();
            LobbyManager.Instance.CreateLobby(matchNameInputField.text, GameManager.Instance.PlayerName, 2, OnMatchCreationSuccess, OnMatchCreationFailure);
        }

        private void OnMatchCreationSuccess(Lobby lobby)
        {
            viewManager.RemoveView<LoadingView>();
            SessionView sessionView = viewManager.DisplayView<SessionView>();
            sessionView.UpdateSessionInformation(lobby);
            viewManager.RemoveView<CreateSessionView>();
        }

        private void OnMatchCreationFailure()
        {
            viewManager.RemoveView<LoadingView>();
            MessageView messageView = viewManager.DisplayView<MessageView>();

            if(messageView)
            {
                messageView.SetMessageText("Failed to create session, please try again.");
            }
        }

        private void OnGoBackActionPerformed()
        {
            viewManager.DisplayView<MainMenuView>();
            viewManager.RemoveView<CreateSessionView>();
        }
    }
}
