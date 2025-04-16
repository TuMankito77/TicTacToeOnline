namespace TicTacToeOnline.Ui.Views
{
    using UnityEngine;
    using UnityEngine.UI;
    
    using TMPro;

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
        }

        #endregion

        public void SetSessionName(string sessionName)
        {
            sessionNameText.text = sessionName;
        }

        public void SetPlayerAName(string playerAName)
        {
            playerANameText.text = playerAName;
        }

        private void OnStartMatchButtonPressed()
        {
            viewManager.RemoveView(this.GetType());
            //Send an event to the game manager letting it know that the game has started.
        }
    }
}

