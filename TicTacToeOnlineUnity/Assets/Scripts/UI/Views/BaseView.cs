namespace TicTacToeOnline.Ui.Views
{
    using System;
    using System.Collections.Generic;
    
    using UnityEngine;
    using UnityEngine.UI;
    
    using TicTacToeOnline.Ui.Animations;

    public abstract class BaseView : MonoBehaviour
    {
        public Action onEnterAnimationFinished;
        public Action onExitAnimationFinished;

        [SerializeField]
        private BaseAnimatedElement enterAnimatedElement = null;

        [SerializeField]
        private BaseAnimatedElement exitAnimatimatedElement = null;

        [SerializeField]
        private CanvasGroup canvasGroup = null;

        protected ViewManager viewManager = null;

        public bool IsPlayingAnimation { get; private set; }

        public virtual void Initialize(ViewManager viewManager)
        {
            this.viewManager = viewManager;
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0;
        }

        public void TransitionIn()
        {
            canvasGroup.alpha = 1;
            enterAnimatedElement.onAnimationFinished += OnEnterAnimationFinished;
            IsPlayingAnimation = true;
            enterAnimatedElement.PlayAnimation();
        }

        public void TransitionOut()
        {
            canvasGroup.interactable = false;
            exitAnimatimatedElement.onAnimationFinished += OnExitAnimationFinshed;
            IsPlayingAnimation = true;
            exitAnimatimatedElement.PlayAnimation();
        }

        public void StopTransitionAnimations()
        {
            enterAnimatedElement.StopAnimation();
            exitAnimatimatedElement.StopAnimation();
        }

        public List<RectTransform> GetAllRectTransforms()
        {
            List<RectTransform> rectTransforms = new List<RectTransform>();
            rectTransforms.Add(GetComponent<RectTransform>());

            foreach (RectTransform rectTransform in GetComponentsInChildren<RectTransform>())
            {
                rectTransforms.Add(rectTransform);
            }

            return rectTransforms;
        }

        public void ForceRebuildLayout()
        {
            List<RectTransform> rectTransforms = GetAllRectTransforms();

            foreach(RectTransform rectTransform in rectTransforms)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }
        }

        private void OnEnterAnimationFinished()
        {
            canvasGroup.interactable = true;
            onEnterAnimationFinished?.Invoke();
            enterAnimatedElement.onAnimationFinished -= OnEnterAnimationFinished;
            IsPlayingAnimation = false;
        }

        private void OnExitAnimationFinshed()
        {
            onExitAnimationFinished?.Invoke();
            exitAnimatimatedElement.onAnimationFinished -= OnExitAnimationFinshed;
            IsPlayingAnimation = false;
        }
    }
}
