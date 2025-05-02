namespace TicTacToeOnline.Ui.Animations
{
    using System.Collections;
    
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Image))]
    public class GridMarkAnimation : MonoBehaviour
    {
        [SerializeField, Min(0)]
        private float duration = 0.5f;

        [SerializeField]
        private AnimationCurve fillAnimCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Image markImage = null;
        private Coroutine playMarkAnimationCoroutine = null;

        #region Unity Methods

        private void Awake()
        {
            markImage = GetComponent<Image>();
            markImage.fillAmount = 0;
        }

        private void Start()
        {
            playMarkAnimationCoroutine = StartCoroutine(PlayMarkAnimation());
        }

        #endregion

        private IEnumerator PlayMarkAnimation()
        {
            markImage.fillAmount = 0;

            if (duration <= 0 && playMarkAnimationCoroutine != null)
            {
                markImage.fillAmount = 1;
                StopCoroutine(playMarkAnimationCoroutine);
                yield return null;
            }

            float timeTranscurred = 0;

            while(timeTranscurred < duration)
            {
                float imageFillAmount = Mathf.Clamp01(fillAnimCurve.Evaluate(timeTranscurred / duration));
                markImage.fillAmount = imageFillAmount;
                timeTranscurred += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            int keysLength = fillAnimCurve.keys.Length;
            markImage.fillAmount = Mathf.Clamp01(fillAnimCurve.keys[keysLength - 1].value);
        }
    }
}

