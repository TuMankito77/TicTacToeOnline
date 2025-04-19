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
        public const string PLAYER_NAME_KEY = "PlayerName";

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
        private bool isLobbyHost = false;
        private Lobby lobby = null;
        private Coroutine keepLobbyAliveCoroutine;

        public Action OnAnonimousSignInSucess;
        public Action OnAnonimousSignInFail;
        public Lobby LobbyCreated = null;
        public Lobby Lobby => lobby;
        public bool IsLobbyHost => isLobbyHost;
        public bool IsPlayerSignedIn => IsPlayerSignedIn;

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

        public async void CreateLobby(string lobbyName, string playerName, int maxPlayers, Action<Lobby> OnSucess, Action OnFailure)
        {
            if(keepLobbyAliveCoroutine != null)
            {
                StopCoroutine(keepLobbyAliveCoroutine);
                keepLobbyAliveCoroutine = null;
            }

            try
            {
                CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions()
                {
                    IsPrivate = false,
                    Player = new Player()
                    {
                        Data = new Dictionary<string, PlayerDataObject>()
                        {
                            {PLAYER_NAME_KEY, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
                        }
                    }
                };

                lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
                keepLobbyAliveCoroutine = StartCoroutine(KeepLobbyAlive());
                isLobbyHost = true;
                OnSucess?.Invoke(lobby);
                Debug.Log($"Lobby created successfully! Lobby name: {lobby.Name} - Max number of players: {lobby.MaxPlayers}");
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

        public async void ConnectToLobby(string lobbyId, string playerName, Action<Lobby> OnSuccess, Action OnFailure)
        {
            try
            {
                JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions()
                {
                    Player = new Player()
                    {
                        Data = new Dictionary<string, PlayerDataObject>()
                        {
                            { PLAYER_NAME_KEY, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
                        }
                    }
                };

                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(GetDefaultQueryOptions());
                Lobby lobbyFound = queryResponse.Results.Find((lobby) => lobby.Id == lobbyId);

                if (lobbyFound == null)
                {
                    OnFailure?.Invoke();
                    return;
                }

                lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);
                OnSuccess?.Invoke(lobby);
            }
            catch(Exception e)
            {
                OnFailure?.Invoke();
                Debug.LogError(e.Message);
            }
        }

        public async void UpdateLobbyInformation(Action<Lobby> OnSuccess, Action OnFailure)
        {
            try
            {
                lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);
                OnSuccess?.Invoke(lobby);
            }
            catch(LobbyServiceException e)
            {
                OnFailure?.Invoke();
                Debug.LogError(e.Message);
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

                lobby = await LobbyService.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);

                Debug.Log($"Joined to lobby successfully! Lobby name: {lobby.Name} - Max number of players: {lobby.MaxPlayers}");
            }
            catch(LobbyServiceException e)
            {
                Debug.LogError(e.Message);
            }
        }

        private IEnumerator KeepLobbyAlive()
        {
            yield return new WaitForSeconds(15);

            if(lobby != null)
            {
                SendLobbyHeartbeat();
            }
        }

        private async void SendLobbyHeartbeat()
        {
            try
            {
                await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);
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
                lobby = await LobbyService.Instance.QuickJoinLobbyAsync();
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

