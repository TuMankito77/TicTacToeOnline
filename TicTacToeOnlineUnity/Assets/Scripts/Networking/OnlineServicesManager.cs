namespace TicTacToeOnline.Networking
{
    using UnityEngine;

    using TicTacToeOnline.Core;
    using System;
    using Unity.Services.Authentication;
    using Unity.Services.Core;

    public class OnlineServicesManager : SingletonBehavior<OnlineServicesManager>
    {
        public Action OnAnonimousSignInSucess;
        public Action OnAnonimousSignInFail;

        private bool isPlayerSignedIn = false;
        public bool IsPlayerSignedIn => isPlayerSignedIn;

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
        }

        #endregion

        public async void SignInAnonymously()
        {
            try
            {
                await UnityServices.InitializeAsync();
                AuthenticationService.Instance.SignedIn += OnSignedIn;
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (Exception e)
            {
                OnAnonimousSignInFail?.Invoke();
                Debug.LogError($"{GetType().Name} - {e.Message}");
            }
        }

        public string GetPlayerId()
        {
            return AuthenticationService.Instance.PlayerId;
        }

        private void OnSignedIn()
        {
            isPlayerSignedIn = true;
            OnAnonimousSignInSucess?.Invoke();
            Debug.Log($"Signed in as: {AuthenticationService.Instance.PlayerId}");
        }
    }
}

