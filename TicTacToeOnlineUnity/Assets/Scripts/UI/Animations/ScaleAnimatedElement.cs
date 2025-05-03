namespace TicTacToeOnline.Ui.Animations
{
    using UnityEngine;

    public class ScaleAnimatedElement : BaseAnimatedElement
    {
        [SerializeField]
        private Vector3 startScale = Vector3.zero;

        [SerializeField]
        private Vector3 targetScale = Vector3.one;

        #region Unity Methods

        private void Start()
        {
            PlayAnimation();
        }

        #endregion

        protected override void PreAnimateElement()
        {
            base.PreAnimateElement();
            transform.localScale = startScale;
        }

        protected override void OnAnimateElementUpdate(float transitionValue)
        {
            base.OnAnimateElementUpdate(transitionValue);
            Vector3 updatedScale = Vector3.Lerp(startScale, targetScale, transitionValue);
            transform.localScale = updatedScale;
        }
    }
}

