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
            Destroy(viewInstance.gameObject);
        }
    }
}

