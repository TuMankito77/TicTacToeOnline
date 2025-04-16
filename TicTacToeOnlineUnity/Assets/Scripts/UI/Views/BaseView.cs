namespace TicTacToeOnline.Ui.Views
{
    using System.Collections.Generic;
    
    using UnityEngine;

    public class BaseView : MonoBehaviour
    {
        protected ViewManager viewManager = null;

        public void Initialize(ViewManager viewManager)
        {
            this.viewManager = viewManager;
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
    }
}
