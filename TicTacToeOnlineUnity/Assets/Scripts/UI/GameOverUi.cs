namespace TicTacToeOnline.Ui
{
    using UnityEngine;
    using UnityEngine.UI;
    
    using TMPro;
    
    using TicTacToeOnline.Gameplay;

    public class GameOverUi : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI resultsText = null;

        [SerializeField]
        private Color circlesColor = Color.white;

        [SerializeField]
        private Color crossesColor = Color.white;

        [SerializeField]
        private Color tieColor = Color.white;

        [SerializeField]
        private Button rematchButton = null;

        #region Unity Methods

        private void Start()
        {
            Hide();
            GameManager.Instance.OnMatchFinished += OnMatchFinished;
            GameManager.Instance.OnGameRestarted += OnGameRestarted;
            rematchButton.onClick.AddListener(OnRematchButtonPressed);
        }

        #endregion

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnMatchFinished(object sender, GameManager.OnMatchFinishedEventArgs e)
        {
            if(e.Winner == PlayerType.None)
            {
                resultsText.text =  "It's a tie!";
                resultsText.color = tieColor;
            }
            else if(e.Winner == GameManager.Instance.LocalPlayerType)
            {
                resultsText.text = "YOU WON!";
            }
            else
            {
                resultsText.text = "You lost.";
            }

            switch (GameManager.Instance.LocalPlayerType)
            {
                case PlayerType.Circle:
                    {
                        resultsText.color = circlesColor;
                        break;
                    }

                case PlayerType.Cross:
                    {
                        resultsText.color = crossesColor;
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            Show();
        }

        private void OnGameRestarted(object sender, System.EventArgs e)
        {
            Hide();
        }

        private void OnRematchButtonPressed()
        {
            GameManager.Instance.SendRestartGameRpc();
        }
    }
}

