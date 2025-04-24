namespace TicTacToeOnline.Ui.Views
{
    using System.Collections.Generic;
    
    using Unity.Services.Lobbies.Models;
    using UnityEngine;
    
    using TicTacToeOnline.Networking;
    using TicTacToeOnline.Gameplay;

    public class FindSessionView : BaseView
    {
        [SerializeField]
        private BaseButton buttonPrefab = null;

        [SerializeField]
        private Transform buttonsParentTransform = null;

        [SerializeField]
        private BaseButton refreshButton = null;

        private List<BaseButton> sessionButtons = null;

        #region Unity Methods

        private void Awake()
        {
            sessionButtons = new List<BaseButton>();
        }

        private void OnEnable()
        {
            refreshButton.onButtonPressed += OnRefreshButtonPressed;
        }

        private void OnDisable()
        {
            refreshButton.onButtonPressed -= OnRefreshButtonPressed;
        }

        private void OnDestroy()
        {
            sessionButtons.Clear();
            sessionButtons = null;
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

            foreach(BaseButton sessionButton in sessionButtons)
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
                BaseButton sessionButton = Instantiate(buttonPrefab, buttonsParentTransform);
                sessionButton.SetText(lobby.Name);

                void OnButtonPressed()
                {
                    sessionButton.onButtonPressed -= OnButtonPressed;
                    viewManager.DisplayView(typeof(LoadingView));
                    LobbyManager.Instance.ConnectToLobby(lobby.Id, GameManager.Instance.PlayerName, OnConnectToLobbySuccess, OnConnectToLobbyFailure);
                }

                sessionButton.onButtonPressed += OnButtonPressed;
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
            sessionView.UpdateSessionInformation(lobby);
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

