namespace TicTacToeOnline.Ui.Views
{
    using UnityEngine;
    using UnityEngine.UI;
    
    using Unity.Services.Lobbies.Models;
    
    using TMPro;
    using TicTacToeOnline.Networking;
    using Unity.Services.Authentication;
    using TicTacToeOnline.Gameplay;

    public class SessionView : BaseView
    {
        [SerializeField]
        private TextMeshProUGUI sessionNameText = null;

        [SerializeField]
        private TextMeshProUGUI playerANameText = null;

        [SerializeField]
        private TextMeshProUGUI playerBNameText = null;

        [SerializeField]
        private Button startMatchButton = null;

        [SerializeField]
        private TextMeshProUGUI sessionStatusText = null;

        #region Unity Methods

        private void Start()
        {
            startMatchButton.onClick.AddListener(OnStartMatchButtonPressed);
            LobbyManager.Instance.onLobbyInformationUpdated += OnLobbyInformationUpdated;
        }

        private void OnDestroy()
        {
            startMatchButton.onClick.RemoveListener(OnStartMatchButtonPressed);
            LobbyManager.Instance.onLobbyInformationUpdated -= OnLobbyInformationUpdated;
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

            if (AuthenticationService.Instance.PlayerId != lobby.HostId)
            {
                startMatchButton.gameObject.SetActive(false);
                sessionStatusText.text = $"Waiting for host to start the game.";
            }
        }

        private void OnStartMatchButtonPressed()
        {
            viewManager.RemoveView(GetType());
            GameManager.Instance.StartMatch();
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

    }
}

