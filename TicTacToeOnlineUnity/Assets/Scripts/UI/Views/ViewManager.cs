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

        public BaseView DisplayView(Type viewType)
        {
            BaseView viewPrefab = viewPrefabs.Find((view) => view.GetType() == viewType);
            
            if(viewPrefab == null)
            {
                Debug.LogError($"{viewType} not found.");
                return null;
            }

            BaseView viewInstance = Instantiate(viewPrefab, transform);
            viewsDisplayed.Add(viewInstance);
            viewInstance.Initialize(this);
            return viewInstance;
        }

        public void RemoveView(Type viewType)
        {
            BaseView viewInstance = viewsDisplayed.FindLast((view) => view.GetType() == viewType);

            if(viewInstance == null)
            {
                return;
            }

            viewsDisplayed.Remove(viewInstance);
            Destroy(viewInstance);
        }
    }
}

