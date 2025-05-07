namespace TicTacToeOnline.Ui.Views
{
    using System;
    
    using UnityEngine;
    using UnityEngine.UI;
    
    using TMPro;
    using TicTacToeOnline.Ui.Animations;
    using UnityEngine.EventSystems;

    public class BaseButton : MonoBehaviour
    {
        [SerializeField]
        private Button buttonComponent = null;

        [SerializeField]
        private TextMeshProUGUI buttonText = null;

        [SerializeField]
        private BaseAnimatedElement animatedElement = null;

        [SerializeField]
        private bool triggerButtonActionBeforeAnimation = true;

        public Action onButtonPressed = null;

        #region Unity Methods

        private void Awake()
        {
            buttonComponent.onClick.AddListener(OnButtonPressed);        
        }

        private void OnDestroy()
        {
            buttonComponent.onClick.RemoveListener(OnButtonPressed);
        }

        #endregion

        public void SetText(string text)
        {
            buttonText.text = text;
        }
        
        private void OnButtonPressed()
        {
            if(EventSystem.current.currentSelectedGameObject == gameObject)
            {
                animatedElement.PlayAnimation();

                if(triggerButtonActionBeforeAnimation)
                {
                    onButtonPressed?.Invoke();
                }
                else
                {
                    animatedElement.onAnimationFinished += OnButtonAnimationFinished;
                }
            }
        }

        private void OnButtonAnimationFinished()
        {
            onButtonPressed?.Invoke();
        }
    }
}

