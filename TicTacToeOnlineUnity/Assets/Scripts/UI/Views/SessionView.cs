namespace TicTacToeOnline.Ui.Views
{
    using UnityEngine;
    using UnityEngine.UI;
    
    using Unity.Services.Lobbies.Models;
    
    using TMPro;
    using TicTacToeOnline.Networking;

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

        #region Unity Methods

        private void Start()
        {
            startMatchButton.onClick.AddListener(OnStartMatchButtonPressed);
            UpdateSessionInformation();
        }

        #endregion

        private void UpdateSessionInformation()
        {
            Lobby lobby = LobbyManager.Instance.Lobby;
            sessionNameText.text = lobby.Name;

            playerANameText.text = lobby.Players[0].Data[LobbyManager.PLAYER_NAME_KEY].Value;

            if (lobby.Players.Count > 1)
            {
                playerBNameText.text = lobby.Players[1].Data[LobbyManager.PLAYER_NAME_KEY].Value;
            }
        }

        public void SetSessionName(string sessionName)
        {
            sessionNameText.text = sessionName;
        }

        public void SetPlayerAName(string playerAName)
        {
            playerANameText.text = playerAName;
        }

        public void SetPlayerBName(string playerBName)
        {
            playerBNameText.text = playerBName;
        }

        private void OnStartMatchButtonPressed()
        {
            viewManager.RemoveView(this.GetType());
            //Send an event to the game manager letting it know that the game has started.
        }
    }
}

