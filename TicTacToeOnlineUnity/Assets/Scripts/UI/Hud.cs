namespace TicTacToeOnline.Ui
{
    using UnityEngine;
    
    using TMPro;
    using TicTacToeOnline.Gameplay;
    using UnityEngine.UI;

    public class Hud : MonoBehaviour
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
        private RectTransform verticalLayoutRectTransform = null;

        #region Unity Methods

        private void Awake()
        {
            circleImage.SetActive(false);
            crossImage.SetActive(false);
            playerTurnText.text = WAITING_FOR_PLAYER_TEXT;
        }

        private void Start()
        {
            GameManager.Instance.OnPlayerTurnUpdated += OnPlayerTurnUpdated;
        }

        #endregion

        private void OnPlayerTurnUpdated(object sender, System.EventArgs e)
        {
            UpdatePlayerTurnText();
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
        }
    }
}

