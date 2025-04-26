namespace TicTacToeOnline.Ui.Views
{
    using System.Collections.Generic;
    
    using UnityEngine;
    using UnityEngine.UI;

    public class BaseView : MonoBehaviour
    {
        protected ViewManager viewManager = null;

        public virtual void Initialize(ViewManager viewManager)
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

        public void ForceRebuildLayout()
        {
            List<RectTransform> rectTransforms = GetAllRectTransforms();

            foreach(RectTransform rectTransform in rectTransforms)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }
        }
    }
}
