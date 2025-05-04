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
            StopAnimation();
            PreAnimateElement();
            playMarkAnimationCoroutine = StartCoroutine(StartTransition());
        }

        public void StopAnimation()
        {
            if (playMarkAnimationCoroutine != null)
            {
                StopCoroutine(playMarkAnimationCoroutine);
                playMarkAnimationCoroutine = null;
            }
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
            int keysLength = transitionAnimCurve.keys.Length;
            
            if (duration <= 0 && playMarkAnimationCoroutine != null)
            {
                OnAnimateElementUpdate(transitionAnimCurve.keys[keysLength - 1].value);
                onAnimationFinished?.Invoke();
                StopCoroutine(playMarkAnimationCoroutine);
                yield return null;
            }

            float timeTranscurred = 0;
            float transitionValue = transitionAnimCurve.keys[0].value;

            float animCurveStartTime = transitionAnimCurve.keys[0].time;
            float animCurveEndTime = transitionAnimCurve.keys[keysLength - 1].time;
            float animCurveDuration = animCurveEndTime - animCurveStartTime;

            OnAnimateElementUpdate(transitionValue);

            yield return new WaitForEndOfFrame();

            while (timeTranscurred < duration)
            {
                timeTranscurred += Time.deltaTime;
                float animProgress = timeTranscurred / duration;
                transitionValue = transitionAnimCurve.Evaluate(animCurveStartTime + (animCurveDuration * animProgress));
                OnAnimateElementUpdate(transitionValue);
                yield return new WaitForEndOfFrame();
            }

            transitionValue = transitionAnimCurve.keys[keysLength - 1].value;
            OnAnimateElementUpdate(transitionValue);
            onAnimationFinished?.Invoke();
        }
    }
}

