namespace TicTacToeOnline.Ui.Views
{
    using UnityEngine;
    
    using Unity.Services.Lobbies.Models;
    
    using TMPro;
   
    using TicTacToeOnline.Networking;
    using TicTacToeOnline.Gameplay;
    using TicTacToeOnline.Input;

    public class SessionView : BaseView
    {
        [SerializeField]
        private TextMeshProUGUI sessionNameText = null;

        [SerializeField]
        private TextMeshProUGUI playerANameText = null;

        [SerializeField]
        private TextMeshProUGUI playerBNameText = null;

        [SerializeField]
        private BaseButton startMatchButton = null;

        [SerializeField]
        private TextMeshProUGUI sessionStatusText = null;

        #region Unity Methods

        private void OnEnable()
        {
            startMatchButton.onButtonPressed += OnStartMatchButtonPressed;
            LobbyManager.Instance.onLobbyInformationUpdated += OnLobbyInformationUpdated;
            InputManager.Instance.onGoBackActionPerformed += OnGoBackActionPerformed;
        }

        private void OnDisable()
        {
            startMatchButton.onButtonPressed -= OnStartMatchButtonPressed;
            LobbyManager.Instance.onLobbyInformationUpdated -= OnLobbyInformationUpdated;
            InputManager.Instance.onGoBackActionPerformed -= OnGoBackActionPerformed;
        }

        #endregion

        public void UpdateSessionInformation(Lobby lobby)
        {
            sessionNameText.text = lobby.Name;

            playerANameText.text = lobby.Players[0].Data[LobbyManager.PLAYER_NAME_KEY].Value;

            if (lobby.Players.Count > 1)
            {
                playerBNameText.text = lobby.Players[1].Data[LobbyManager.PLAYER_NAME_KEY].Value;
                startMatchButton.enabled = true;
                sessionStatusText.text = $"You can start the match now!";
            }
            else
            {
                startMatchButton.enabled = false;
                sessionStatusText.text = $"Waiting for opponent to join.";
            }

            if (!LobbyManager.Instance.IsLobbyHost)
            {
                startMatchButton.gameObject.SetActive(false);
                sessionStatusText.text = $"Waiting for host to start the game.";
                LobbyManager.Instance.onKickedFromLobby += OnKickedFromLobby;
            }
        }

        private void OnStartMatchButtonPressed()
        {
            viewManager.DisplayView<LoadingView>();
            GameManager.Instance.StartMatch(OnMatchStartSuccess, OnMatchStartFailure);
        }

        private void OnMatchStartSuccess()
        {
            viewManager.RemoveView<LoadingView>();
            viewManager.RemoveView<SessionView>();
        }

        private void OnMatchStartFailure()
        {
            viewManager.RemoveView<LoadingView>();
            MessageView messageView = viewManager.DisplayView<MessageView>();

            if(messageView)
            {
                messageView.SetMessageText("Something went wrong, please try again.");
            }
        }

        private void OnLobbyInformationUpdated(Lobby lobby, LobbyUpdateType lobbyUpdateType)
        {
            switch(lobbyUpdateType)
            {
                case LobbyUpdateType.PlayerJoined:
                    {
                        UpdateSessionInformation(lobby);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }

        private void OnGoBackActionPerformed()
        {
            MessageView messageView = viewManager.DisplayView<MessageView>();
            messageView.SetMessageText("Are you sure you want to exit the match?");
            messageView.ActivateCloseMessageButtonCancel();

            void OnClosebuttonOkPressed()
            {
                messageView.onCloseButtonOkPressed -= OnClosebuttonOkPressed;
                
                if (LobbyManager.Instance.IsLobbyHost)
                {
                    LobbyManager.Instance.DestroyLobby(LobbyManager.Instance.Lobby.Id);
                    viewManager.DisplayView<CreateSessionView>();
                }
                else
                {
                    LobbyManager.Instance.onKickedFromLobby -= OnKickedFromLobby;
                    LobbyManager.Instance.DisconnectFromLobby(LobbyManager.Instance.Lobby.Id, OnlineServicesManager.Instance.GetPlayerId());
                    viewManager.DisplayView<FindSessionView>();
                }

                viewManager.RemoveView<SessionView>();
            }

            messageView.onCloseButtonOkPressed += OnClosebuttonOkPressed;
        }

        private void OnKickedFromLobby()
        {
            MessageView messageView = viewManager.DisplayView<MessageView>();
            messageView.SetMessageText($"Sorry, the match was closed. Please, try to find another match.");

            void OnCloseButtonPressed()
            {
                messageView.onCloseButtonOkPressed -= OnCloseButtonPressed;
                viewManager.DisplayView<FindSessionView>();
                viewManager.RemoveView<SessionView>();
            }

            messageView.onCloseButtonOkPressed += OnCloseButtonPressed;
        }
    }
}

