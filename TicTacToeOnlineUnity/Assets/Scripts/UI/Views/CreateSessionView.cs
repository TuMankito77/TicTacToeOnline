namespace TicTacToeOnline.Ui.Views
{
    using TicTacToeOnline.Networking;
    using TMPro;
    using Unity.Services.Lobbies.Models;
    using UnityEngine;
    using UnityEngine.UI;

    public class CreateSessionView : BaseView
    {
        [SerializeField]
        private TMP_InputField playerNameInputField = null;

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
                string.IsNullOrWhiteSpace(matchNameInputField.text)||
                string.IsNullOrEmpty(playerNameInputField.text) ||
                string.IsNullOrWhiteSpace(playerNameInputField.text))
            {
                MessageView messageView = viewManager.DisplayView(typeof(MessageView)) as MessageView;

                if(messageView)
                {
                    messageView.SetMessageText($"There are empty input fields, please make sure to fill them all.");
                }

                return;
            }

            viewManager.DisplayView(typeof(LoadingView));
            LobbyManager.Instance.CreateLobby(playerNameInputField.text, matchNameInputField.text, 2, OnMatchCreationSuccess, OnMatchCreationFailure);
        }

        private void OnMatchCreationSuccess(Lobby lobby)
        {
            viewManager.RemoveView(typeof(LoadingView));
            SessionView sessionView = viewManager.DisplayView(typeof(SessionView)) as SessionView;

            if(sessionView == null)
            {
                MessageView messageView = viewManager.DisplayView(typeof(MessageView)) as MessageView;
                messageView.SetMessageText("Something went wrong, please try again.");
                return;
            }


            sessionView.SetSessionName(lobby.Name);

            Player player = lobby.Players.Find((player) => player.Id == LobbyManager.Instance.GetPlayerId());

            if (player == null)
            {
                return;
            }

            sessionView.SetPlayerAName(player.Data[LobbyManager.PLAYER_NAME_KEY].Value);

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
