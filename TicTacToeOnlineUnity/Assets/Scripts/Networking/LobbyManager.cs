namespace TicTacToeOnline.Networking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    
    using Unity.Services.Core;
    using Unity.Services.Authentication;
    using Unity.Services.Lobbies;
    using Unity.Services.Lobbies.Models;

    public class LobbyManager : MonoBehaviour
    {
        public const string LOBBY_NAME_KEY = "LobbyName";

        private static LobbyManager instance = null;

        public static LobbyManager Instance
        {
            get
            {
                if(instance == null)
                {
                    GameObject lobbyManagerGO = new GameObject($"{typeof(LobbyManager).Name}");
                    DontDestroyOnLoad(lobbyManagerGO);
                    instance = lobbyManagerGO.AddComponent<LobbyManager>();
                }

                return instance;
            }
        }

        private bool isPlayerSignedIn = false;
        private Lobby lobbyCreated = null;
        private Lobby lobbyJoined = null;
        private Coroutine keepLobbyAliveCoroutine;

        public Action OnAnonimousSignInSucess;
        public Action OnAnonimousSignInFail;
        public Lobby LobbyCreated = null;
        public Lobby LobbyJoined = null;

        #region Unity Methods

        private void Start()
        {
            if (instance != null && instance != this)
            {
                Debug.LogError($"Another instance of the {GetType().Name} was found, this instance will be destroyed.");
                Destroy(gameObject);
            }

            instance = this;
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
            catch(Exception e)
            {
                OnAnonimousSignInFail?.Invoke();
                Debug.LogError($"{GetType().Name} - {e.Message}");
            }
        }

        public async void CreateLobby(string lobbyName, int maxPlayers, Action<Lobby> OnSucess, Action OnFailure)
        {
            try
            {
                lobbyCreated = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
                keepLobbyAliveCoroutine = StartCoroutine(KeepLobbyAlive());
                OnSucess?.Invoke(lobbyCreated);
                Debug.Log($"Lobby created successfully! Lobby name: {lobbyCreated.Name} - Max number of players: {lobbyCreated.MaxPlayers}");
            }
            catch(LobbyServiceException e)
            {
                OnFailure?.Invoke();
                Debug.LogError(e);
            }
        }

        public async void GetAvailableLobbies(Action<List<Lobby>> OnSuccess, Action OnFailure)
        {
            try
            {
                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(GetDefaultQueryOptions());
                OnSuccess(queryResponse.Results);
            }
            catch(LobbyServiceException e)
            {
                OnFailure();
                Debug.LogError(e.Message);
            }
        }

        public async void ConnectToLobby(string lobbyId, Action<Lobby> OnSuccess, Action OnFailure)
        {
            try
            {
                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(GetDefaultQueryOptions());
                Lobby lobbyFound = queryResponse.Results.Find((lobby) => lobby.Id == lobbyId);
                
                if (lobbyFound == null)
                {
                    OnFailure?.Invoke();
                    return;
                }

                lobbyJoined = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
                OnSuccess(lobbyJoined);
            }
            catch(Exception e)
            {
                OnFailure?.Invoke();
                Debug.LogError(e.Message);
            }
        }

        private void OnSignedIn()
        {
            isPlayerSignedIn = true;
            OnAnonimousSignInSucess?.Invoke();
            Debug.Log($"Signed in as: {AuthenticationService.Instance.PlayerId}");
        }

        private async void ListLobbies()
        {
            try
            {
                QueryLobbiesOptions queryLobbyOptions = new QueryLobbiesOptions
                {
                    Count = 25,
                    Filters = new List<QueryFilter>
                    {
                        new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "1", QueryFilter.OpOptions.GE)
                    },
                    Order = new List<QueryOrder>
                    {
                        new QueryOrder(false, QueryOrder.FieldOptions.Created)
                    }
                };

                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbyOptions);
                Debug.Log($"Lobbies found: {queryResponse.Results.Count}");

                foreach(Lobby lobby in queryResponse.Results)
                {
                    Debug.Log($"Lobby name: {lobby.Name} - Max number of players: {lobby.MaxPlayers}");
                }
            }
            catch(LobbyServiceException e)
            {
                Debug.LogError(e);
            }
        }

        private async void ConnectToFirstLobbyFound()
        {
            try
            {
                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
                Debug.Log($"Lobbies found: {queryResponse.Results.Count}");

                if(queryResponse.Results.Count <= 0)
                {
                    Debug.Log($"No lobbies were found :(");
                    return;
                }

                lobbyJoined = await LobbyService.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);

                Debug.Log($"Joined to lobby successfully! Lobby name: {lobbyJoined.Name} - Max number of players: {lobbyJoined.MaxPlayers}");
            }
            catch(LobbyServiceException e)
            {
                Debug.LogError(e.Message);
            }
        }

        private IEnumerator KeepLobbyAlive()
        {
            yield return new WaitForSeconds(15);

            if(lobbyCreated != null)
            {
                SendLobbyHeartbeat();
            }
        }

        private async void SendLobbyHeartbeat()
        {
            try
            {
                await LobbyService.Instance.SendHeartbeatPingAsync(lobbyCreated.Id);
                keepLobbyAliveCoroutine = StartCoroutine(KeepLobbyAlive());
                Debug.Log("Heartbeat sent");
            }
            catch(LobbyServiceException e)
            {
                Debug.LogError(e.Message);
            }
        }

        private async void QuickJoinLobby()
        {
            try
            {
                lobbyJoined = await LobbyService.Instance.QuickJoinLobbyAsync();
            }
            catch(LobbyServiceException e)
            {
                Debug.LogError(e.Message);
            }
        }

        private QueryLobbiesOptions GetDefaultQueryOptions()
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "1", QueryFilter.OpOptions.GE)
                }
            };

            return queryLobbiesOptions;
        }
    }
}

