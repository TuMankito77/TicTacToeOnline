namespace TicTacToeOnline.Ui.Animations
{
    using System;
    using System.Collections;

    using UnityEngine;

    public abstract class BaseAnimatedElement : MonoBehaviour
    {
        public Action onAnimationFinished = null;

        [SerializeField, Min(0)]
        private float duration = 0.5f;

        [SerializeField]
        private AnimationCurve transitionAnimCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Coroutine playMarkAnimationCoroutine = null;

        public void PlayAnimation()
        {
            if(playMarkAnimationCoroutine != null)
            {
                StopCoroutine(playMarkAnimationCoroutine);
                playMarkAnimationCoroutine = null;
            }

            PreAnimateElement();
            playMarkAnimationCoroutine = StartCoroutine(StartTransition());
        }

        protected virtual void PreAnimateElement()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transitionValue">A value between 0 and 1 that can be used as the transition value for going from state A to B.</param>
        protected virtual void OnAnimateElementUpdate(float transitionValue)
        {

        }

        private IEnumerator StartTransition()
        {
            if (duration <= 0 && playMarkAnimationCoroutine != null)
            {
                OnAnimateElementUpdate(1);
                onAnimationFinished?.Invoke();
                StopCoroutine(playMarkAnimationCoroutine);
                yield return null;
            }

            float timeTranscurred = 0;
            float transitionValue = 0;

            while (timeTranscurred < duration)
            {
                transitionValue = Mathf.Clamp01(transitionAnimCurve.Evaluate(timeTranscurred / duration));
                OnAnimateElementUpdate(transitionValue);
                timeTranscurred += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            int keysLength = transitionAnimCurve.keys.Length;
            transitionValue = Mathf.Clamp01(transitionAnimCurve.keys[keysLength - 1].value);
            OnAnimateElementUpdate(transitionValue);
            onAnimationFinished?.Invoke();
        }
    }
}

