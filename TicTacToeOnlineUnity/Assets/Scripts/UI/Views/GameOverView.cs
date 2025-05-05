namespace TicTacToeOnline.Ui.Views
{
    using UnityEngine;
    using UnityEngine.UI;
    
    using TMPro;
    
    using TicTacToeOnline.Gameplay;

    public class GameOverView : BaseView
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

        private void Awake()
        {
            rematchButton.onClick.AddListener(OnRematchButtonPressed);
        }

        #endregion

        public void UpdateWinnerInformation(PlayerType winner)
        {
            if (winner == PlayerType.None)
            {
                resultsText.text = "It's a tie!";
                resultsText.color = tieColor;
            }
            else if (winner == GameManager.Instance.LocalPlayerType)
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
        }

        private void OnRematchButtonPressed()
        {
            GameManager.Instance.SendRestartGameRpc();
        }
    }
}

