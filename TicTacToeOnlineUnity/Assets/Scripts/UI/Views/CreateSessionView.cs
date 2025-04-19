namespace TicTacToeOnline.Ui.Views
{
    using UnityEngine;
    using UnityEngine.UI;
    
    using Unity.Services.Lobbies.Models;
    
    using TMPro;
    
    using TicTacToeOnline.Gameplay;
    using TicTacToeOnline.Networking;
    using System;

    public class CreateSessionView : BaseView
    {
        [SerializeField]
        private TMP_InputField matchNameInputField = null;

        [SerializeField]
        private Button createButton = null;

        #region Unity Methods

        private void Start()
        {
            createButton.onClick.AddListener(OnCreateButtonPressed);
        }

        #endregion

        private void OnCreateButtonPressed()
        {
            if(string.IsNullOrEmpty(matchNameInputField.text) || 
                string.IsNullOrWhiteSpace(matchNameInputField.text))
            {
                MessageView messageView = viewManager.DisplayView(typeof(MessageView)) as MessageView;

                if(messageView)
                {
                    messageView.SetMessageText($"The match name is empty, please make sure to type a match name.");
                }

                return;
            }

            viewManager.DisplayView(typeof(LoadingView));
            LobbyManager.Instance.CreateLobby(matchNameInputField.text, GameManager.Instance.PlayerName, 2, OnMatchCreationSuccess, OnMatchCreationFailure);
        }

        private void OnMatchCreationSuccess(Lobby lobby)
        {
            viewManager.RemoveView(typeof(LoadingView));
            SessionView sessionView = viewManager.DisplayView(typeof(SessionView)) as SessionView;
            sessionView.UpdateSessionInformation(lobby);
            GameManager.Instance.CheckForConnectedClients();
            viewManager.RemoveView(GetType());
        }

        private void OnMatchCreationFailure()
        {
            viewManager.RemoveView(typeof(LoadingView));
            MessageView messageView = viewManager.DisplayView(typeof(MessageView)) as MessageView;

            if(messageView)
            {
                messageView.SetMessageText("Failed to create session, please try again.");
            }
        }
    }
}
