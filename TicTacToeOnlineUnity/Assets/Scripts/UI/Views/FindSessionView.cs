namespace TicTacToeOnline.Ui.Views
{
    using System.Collections.Generic;
    
    using Unity.Services.Lobbies.Models;
    using UnityEngine;
    
    using TicTacToeOnline.Networking;
    using TicTacToeOnline.Gameplay;
    using TicTacToeOnline.Input;

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
            InputManager.Instance.onGoBackActionPerformed += OnGoBackActionPerformed;
        }

        private void OnDisable()
        {
            refreshButton.onButtonPressed -= OnRefreshButtonPressed;
            InputManager.Instance.onGoBackActionPerformed -= OnGoBackActionPerformed;
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
            viewManager.DisplayView<LoadingView>();

            foreach(BaseButton sessionButton in sessionButtons)
            {
                Destroy(sessionButton.gameObject);
            }

            sessionButtons.Clear();
            LobbyManager.Instance.GetAvailableLobbies(OnGetLobbiesSuccess, OnGetLobbiesFailure);
        }

        private void OnGetLobbiesSuccess(List<Lobby> lobbies)
        {
            viewManager.RemoveView<LoadingView>();
            
            foreach(Lobby lobby in lobbies)
            {
                BaseButton sessionButton = Instantiate(buttonPrefab, buttonsParentTransform);
                sessionButton.SetText(lobby.Name);

                void OnButtonPressed()
                {
                    sessionButton.onButtonPressed -= OnButtonPressed;
                    viewManager.DisplayView<LoadingView>();
                    LobbyManager.Instance.ConnectToLobby(lobby.Id, GameManager.Instance.PlayerName, OnConnectToLobbySuccess, OnConnectToLobbyFailure);
                }

                sessionButton.onButtonPressed += OnButtonPressed;
                sessionButtons.Add(sessionButton);
            }
        }

        private void OnGetLobbiesFailure()
        {
            viewManager.RemoveView<LoadingView>();
            MessageView messageView = viewManager.DisplayView<MessageView>();

            void OnCloseButtonPressed()
            {
                messageView.onCloseButtonOkPressed -= OnCloseButtonPressed;
                UpdateSessionsList();
            }

            messageView.onCloseButtonOkPressed += OnCloseButtonPressed;
            messageView.SetMessageText("Failed to get the list from the server, click on the button to try again.");
        }

        private void OnConnectToLobbySuccess(Lobby lobby)
        {
            viewManager.RemoveView<LoadingView>();
            viewManager.RemoveView<FindSessionView>();
            SessionView sessionView = viewManager.DisplayView<SessionView>();
            sessionView.UpdateSessionInformation(lobby);
        }

        private void OnConnectToLobbyFailure()
        {
            viewManager.RemoveView<LoadingView>();
            MessageView messageView = viewManager.DisplayView<MessageView>();

            void OnCloseButtonPressed()
            {
                messageView.onCloseButtonOkPressed -= OnCloseButtonPressed;
                UpdateSessionsList();
            }

            messageView.onCloseButtonOkPressed += OnCloseButtonPressed;
            messageView.SetMessageText("Could not connect to server.");
        }

        private void OnRefreshButtonPressed()
        {
            UpdateSessionsList();
        }

        private void OnGoBackActionPerformed()
        {
            viewManager.RemoveView<FindSessionView>();
            viewManager.DisplayView<MainMenuView>();
        }
    }
}

