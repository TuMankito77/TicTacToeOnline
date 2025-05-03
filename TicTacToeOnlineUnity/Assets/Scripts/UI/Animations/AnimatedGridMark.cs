namespace TicTacToeOnline.Ui.Animations
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Image))]
    public class AnimatedGridMark : BaseAnimatedElement
    {
        private Image markImage = null;

        #region Unity Methods

        private void Awake()
        {
            markImage = GetComponent<Image>();
            markImage.fillAmount = 0;
        }

        private void Start()
        {
            PlayAnimation();
        }

        #endregion

        protected override void PreAnimateElement()
        {
            base.PreAnimateElement();
            markImage.fillAmount = 0;
        }

        protected override void OnAnimateElementUpdate(float transitionValue)
        {
            base.OnAnimateElementUpdate(transitionValue);
            markImage.fillAmount = transitionValue;
        }
    }
}

