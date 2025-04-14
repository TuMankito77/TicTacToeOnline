namespace TicTacToeOnline.Ui.Views
{
    using UnityEngine;

    public class BaseView : MonoBehaviour
    {
        protected ViewManager viewManager = null;

        public void Initialize(ViewManager viewManager)
        {
            this.viewManager = viewManager;
        }
    }
}
