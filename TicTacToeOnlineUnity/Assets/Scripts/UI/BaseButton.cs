namespace TicTacToeOnline.Ui.Views
{
    using System;
    
    using UnityEngine;
    using UnityEngine.UI;
    
    using TMPro;

    public class BaseButton : MonoBehaviour
    {
        [SerializeField]
        private Button buttonComponent = null;

        [SerializeField]
        private TextMeshProUGUI buttonText = null;

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
            onButtonPressed?.Invoke();
        }
    }
}

