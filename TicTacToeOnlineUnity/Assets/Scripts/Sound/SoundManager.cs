namespace TicTacToeOnline.Sound
{
    using UnityEngine;

    using TicTacToeOnline.Gameplay;
    
    public class SoundManager : MonoBehaviour
    {
        [SerializeField]
        private AudioSource audioSource = null;

        [SerializeField]
        private AudioClip markClip = null;

        [SerializeField]
        private AudioClip winClip = null;

        [SerializeField]
        private AudioClip loseClip = null;

        #region Unity Methods

        private void Start()
        {
            GameManager.Instance.OnMarkPlaced += OnMarkPlaced;
            GameManager.Instance.OnMatchFinished += OnMatchFinished;
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnMarkPlaced -= OnMarkPlaced;
            GameManager.Instance.OnMatchFinished -= OnMatchFinished;
        }

        #endregion

        private void OnMatchFinished(object sender, GameManager.OnMatchFinishedEventArgs e)
        {
            if(e.Winner == GameManager.Instance.LocalPlayerType)
            {
                audioSource.PlayOneShot(winClip);
            }
            else
            {
                audioSource.PlayOneShot(loseClip);
            }
        }

        private void OnMarkPlaced(object sender, System.EventArgs e)
        {
            audioSource.PlayOneShot(markClip);
        }
    }
}

