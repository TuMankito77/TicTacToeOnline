namespace TicTacToeOnline.Core
{
    using UnityEngine;

    public abstract class SingletonBehavior<T> : MonoBehaviour where T:SingletonBehavior<T>
    {
        private static T instance = null;

        public static T Instance
        {
            get
            {
                if(instance == null)
                {
                    GameObject instanceGO = new GameObject(typeof(T).Name);
                    instance = instanceGO.AddComponent<T>();
                    DontDestroyOnLoad(instanceGO);
                }

                return instance;
            }
        }

        #region Unity Methods

        protected virtual void Awake()
        {
            if(instance != null && instance != this)
            {
                Debug.LogError($"There is an already existing instance of the singleto {GetType().Name}, this instance will be destroyed.");
                Destroy(gameObject);
            }

            instance = this as T;
        }

        #endregion
    }
}

