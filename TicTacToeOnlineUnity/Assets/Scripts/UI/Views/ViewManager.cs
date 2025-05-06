namespace TicTacToeOnline.Ui.Views
{
    using System;
    using System.Collections.Generic;
    
    using UnityEngine;
    
    using TicTacToeOnline.Input;

    public class ViewManager : MonoBehaviour
    {
        [SerializeField]
        private List<BaseView> viewPrefabs = new List<BaseView>(0);

        private List<BaseView> viewsDisplayed = new List<BaseView>(0);
        private Queue<Action> queuedAnimations = new Queue<Action>();
        private bool isPlayingAnimation = false;

        public T DisplayView<T>() where T : BaseView
        {
            BaseView viewPrefab = viewPrefabs.Find((view) => view.GetType() == typeof(T));
            
            if(viewPrefab == null)
            {
                Debug.LogError($"{typeof(T).Name} not found.");
                return null;
            }

            BaseView viewInstance = Instantiate(viewPrefab, transform);

            if(viewsDisplayed.Count > 0)
            {
                InputManager.Instance.onGoBackActionPerformed -= viewsDisplayed[viewsDisplayed.Count - 1].onGoBackActionPerformed;
            }

            viewsDisplayed.Add(viewInstance);
            viewInstance.Initialize(this);
            viewInstance.ForceRebuildLayout();
            viewInstance.onEnterAnimationFinished += OnTransitionInFinished;

            void OnTransitionInFinished()
            {
                viewInstance.onEnterAnimationFinished -= OnTransitionInFinished;

                if (queuedAnimations.Count > 0)
                {
                    queuedAnimations.Dequeue()?.Invoke();
                }
                else
                {
                    isPlayingAnimation = false;
                    InputManager.Instance.onGoBackActionPerformed += viewsDisplayed[viewsDisplayed.Count - 1].onGoBackActionPerformed;
                }
            }

            if(isPlayingAnimation)
            {
                queuedAnimations.Enqueue(() =>
                {
                    isPlayingAnimation = true;
                    viewInstance.TransitionIn();
                });
            }
            else
            {
                isPlayingAnimation = true;
                viewInstance.TransitionIn();
            }

            return viewInstance as T;
        }

        public void RemoveView<T>() where T : BaseView
        {
            BaseView viewInstance = viewsDisplayed.FindLast((view) => view.GetType() == typeof(T));

            if(viewInstance == null)
            {
                return;
            }

            viewsDisplayed.Remove(viewInstance);
            InputManager.Instance.onGoBackActionPerformed -= viewInstance.onGoBackActionPerformed;

            void OnTransitionOutFinished()
            {
                viewInstance.onExitAnimationFinished -= OnTransitionOutFinished;
                Destroy(viewInstance.gameObject);

                if(queuedAnimations.Count > 0)
                {
                    queuedAnimations.Dequeue()?.Invoke();
                }
                else
                {
                    isPlayingAnimation = false;
                    InputManager.Instance.onGoBackActionPerformed += viewsDisplayed[viewsDisplayed.Count - 1].onGoBackActionPerformed;
                }
            }

            viewInstance.onExitAnimationFinished += OnTransitionOutFinished;

            if(isPlayingAnimation)
            {
                queuedAnimations.Enqueue(() =>
                {
                    isPlayingAnimation = true;
                    viewInstance.TransitionOut();
                });
            }
            else
            {
                isPlayingAnimation = true;
                viewInstance.TransitionOut();
            }
        }
    }
}

