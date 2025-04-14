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
                return;
            }

            viewManager.DisplayView(typeof(LoadingView));
            LobbyManager.Instance.CreateLobby(matchNameInputField.text, 2, OnMatchCreationSuccess, OnMatchCreationFailure);
        }

        private void OnMatchCreationSuccess(Lobby lobby)
        {
            viewManager.RemoveView(typeof(LoadingView));
            //Display view with session details.
            //viewManager.DisplayView();
            viewManager.RemoveView(GetType());
        }

        private void OnMatchCreationFailure()
        {
            viewManager.RemoveView(typeof(LoadingView));
            //Display view with failure message.
            //viewManager.DisplayView();
        }
    }
}
