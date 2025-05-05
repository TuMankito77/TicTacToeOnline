namespace TicTacToeOnline.Ui.Views
{
    using System;
    using System.Collections.Generic;

    using UnityEngine;

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
            viewsDisplayed.Add(viewInstance);
            viewInstance.Initialize(this);
            viewInstance.ForceRebuildLayout();
            viewInstance.onEnterAnimationFinished += OnTransitionInFinished;

            void OnTransitionInFinished()
            {
                viewInstance.onEnterAnimationFinished -= OnTransitionInFinished;
                isPlayingAnimation = false;

                if (queuedAnimations.Count > 0)
                {
                    queuedAnimations.Dequeue()?.Invoke();
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

            void OnTransitionOutFinished()
            {
                viewInstance.onExitAnimationFinished -= OnTransitionOutFinished;
                isPlayingAnimation = false;
                Destroy(viewInstance.gameObject);

                if(queuedAnimations.Count > 0)
                {
                    queuedAnimations.Dequeue()?.Invoke();
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

