namespace TicTacToeOnline.Ui
{
    using UnityEngine;
    
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

        #region Unity Methods

        private void Start()
        {
            Hide();
            GameManager.Instance.OnMatchFinished += OnMatchFinished;
        }

        private void OnMatchFinished(object sender, GameManager.OnMatchFinishedEventArgs e)
        {
            resultsText.text = e.Winner == GameManager.Instance.LocalPlayerType ? "YOU WON!" : "You lost.";
            
            switch(GameManager.Instance.LocalPlayerType)
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
            }

            Show();
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
    }
}

