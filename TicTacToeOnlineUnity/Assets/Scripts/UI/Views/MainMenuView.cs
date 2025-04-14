namespace TicTacToeOnline.Ui.Views
{
    using TicTacToeOnline.Networking;
    using UnityEngine;
    using UnityEngine.UI;

    public class MainMenuView : BaseView
    {
        [SerializeField]
        private Button createSessionButton = null;

        [SerializeField]
        private Button findSessionButton = null;

        #region Unity Methods

        private void Start()
        {
            createSessionButton.onClick.AddListener(OnCreateSessionButtonPressed);
            findSessionButton.onClick.AddListener(OnFindSessionButtonPressed);
        }

        #endregion

        private void OnCreateSessionButtonPressed()
        {
            viewManager.DisplayView(typeof(LoadingView));
        }

        private void OnFindSessionButtonPressed()
        {

        }
    }
}

