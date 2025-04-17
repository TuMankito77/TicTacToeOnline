namespace TicTacToeOnline.Ui.Views
{
    using System.Collections.Generic;
    
    using Unity.Services.Lobbies.Models;
    using UnityEngine;
    using UnityEngine.UI;
    
    using TicTacToeOnline.Networking;

    public class FindSessionView : BaseView
    {
        [SerializeField]
        private Button buttonPrefab = null;

        [SerializeField]
        private Transform buttonsParentTransform = null;

        [SerializeField]
        private Button refreshButton = null;

        private List<Button> sessionButtons = null;

        #region Unity Methods

        private void Awake()
        {
            sessionButtons = new List<Button>();
            refreshButton.onClick.AddListener(OnRefreshButtonPressed);
        }

        #endregion

        public override void Initialize(ViewManager viewManager)
        {
            base.Initialize(viewManager);
            UpdateSessionsList();
        }

        public void UpdateSessionsList()
        {
            viewManager.DisplayView(typeof(LoadingView));

            foreach(Button sessionButton in sessionButtons)
            {
                Destroy(sessionButton.gameObject);
            }

            sessionButtons.Clear();
            LobbyManager.Instance.GetAvailableLobbies(OnGetLobbiesSuccess, OnGetLobbiesFailure);
        }

        private void OnGetLobbiesSuccess(List<Lobby> lobbies)
        {
            viewManager.RemoveView(typeof(LoadingView));
            
            foreach(Lobby lobby in lobbies)
            {
                Button sessionButton = Instantiate(buttonPrefab, buttonsParentTransform);

                void OnButtonPressed()
                {
                    viewManager.DisplayView(typeof(LoadingView));
                    LobbyManager.Instance.ConnectToLobby(lobby.Id, OnConnectToLobbySuccess, OnConnectToLobbyFailure);
                    sessionButton.onClick.RemoveListener(OnButtonPressed);
                }

                sessionButton.onClick.AddListener(OnButtonPressed);
                sessionButtons.Add(sessionButton);
            }
        }

        private void OnGetLobbiesFailure()
        {
            viewManager.RemoveView(typeof(LoadingView));
            MessageView messageView = viewManager.DisplayView(typeof(MessageView)) as MessageView;

            void OnCloseButtonPressed()
            {
                messageView.onCloseButtonPressed -= OnCloseButtonPressed;
                UpdateSessionsList();
            }

            messageView.onCloseButtonPressed += OnCloseButtonPressed;
            messageView.SetMessageText("Failed to get the list from the server, click on the button to try again.");
        }

        private void OnConnectToLobbySuccess(Lobby lobby)
        {
            viewManager.RemoveView(typeof(LoadingView));
            SessionView sessionView = viewManager.DisplayView(typeof(SessionView)) as SessionView;

            if (sessionView == null)
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

            sessionView.SetPlayerBName(player.Data[LobbyManager.PLAYER_NAME_KEY].Value);

            viewManager.RemoveView(GetType());
        }

        private void OnConnectToLobbyFailure()
        {
            viewManager.RemoveView(typeof(LoadingView));
            MessageView messageView = viewManager.DisplayView(typeof(MessageView)) as MessageView;

            void OnCloseButtonPressed()
            {
                messageView.onCloseButtonPressed -= OnCloseButtonPressed;
                UpdateSessionsList();
            }

            messageView.onCloseButtonPressed += OnCloseButtonPressed;
            messageView.SetMessageText("Could not connect to server.");
        }

        private void OnRefreshButtonPressed()
        {
            UpdateSessionsList();
        }
    }
}

