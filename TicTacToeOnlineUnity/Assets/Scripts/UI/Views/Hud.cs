namespace TicTacToeOnline.Ui.Views
{
    using UnityEngine;
    
    using TMPro;

    using TicTacToeOnline.Gameplay;

    public class Hud : BaseView
    {
        const string WAITING_FOR_PLAYER_TEXT = "Waiting for oponent to connect.";
        const string LOCAL_PLAYER_TURN_TEXT = "It's your turn:";
        const string OPONENT_TURN_TEXT = "It's your oponent's turn:";

        [SerializeField]
        private GameObject circleImage = null;

        [SerializeField]
        private GameObject crossImage = null;

        [SerializeField]
        private TextMeshProUGUI playerTurnText = null;

        [SerializeField]
        private TextMeshProUGUI crossesScoreText = null;

        [SerializeField]
        private TextMeshProUGUI circlesScoreText = null;

        [SerializeField]
        private BaseButton quitMachButton = null;

        #region Unity Methods

        private void Awake()
        {
            circleImage.SetActive(false);
            crossImage.SetActive(false);
            playerTurnText.text = WAITING_FOR_PLAYER_TEXT;
            crossesScoreText.text = $"{0}";
            circlesScoreText.text = $"{0}";
        }

        private void OnEnable()
        {
            GameManager.Instance.OnPlayerTurnUpdated += OnPlayerTurnUpdated;
            GameManager.Instance.OnScoresUpdated += OnScoresUpdated;
            quitMachButton.onButtonPressed += OnQuitMatchButtonPressed;
        }

        private void OnDisable()
        {
            
        }

        #endregion

        private void OnPlayerTurnUpdated(object sender, System.EventArgs e)
        {
            UpdatePlayerTurnText();
        }

        private void OnScoresUpdated(object sender, System.EventArgs e)
        {
            GameManager.Instance.GetPlayerScores(out int circlesScore, out int crossesScore);
            circlesScoreText.text = circlesScore.ToString();
            crossesScoreText.text = crossesScore.ToString();
        }

        private void UpdatePlayerTurnText()
        {
            if (GameManager.Instance.PlayerTypeTurn == GameManager.Instance.LocalPlayerType)
            {
                playerTurnText.text = LOCAL_PLAYER_TURN_TEXT;
            }
            else
            {
                playerTurnText.text = OPONENT_TURN_TEXT;
            }

            switch(GameManager.Instance.PlayerTypeTurn)
            {
                case PlayerType.Circle:
                    {
                        circleImage.SetActive(true);
                        crossImage.SetActive(false);
                        break;
                    }

                case PlayerType.Cross:
                    {
                        circleImage.SetActive(false);
                        crossImage.SetActive(true);
                        break;
                    }

                default:
                    {
                        circleImage.SetActive(false);
                        crossImage.SetActive(false);
                        break;
                    }
            }

            ForceRebuildLayout();
        }

        private void OnQuitMatchButtonPressed()
        {
            MessageView messageView = viewManager.DisplayView<MessageView>();
            messageView.ActivateCloseMessageButtonCancel();
            messageView.SetMessageText($"Are you sure you want to quit the match?");

            void OnCloseButtonOkPressed()
            {
                messageView.onCloseButtonOkPressed -= OnCloseButtonOkPressed;
                //Call an fuction on both client and server to leave the lobby and the relay.
            }

            messageView.onCloseButtonOkPressed += OnCloseButtonOkPressed;
        }
    }
}

