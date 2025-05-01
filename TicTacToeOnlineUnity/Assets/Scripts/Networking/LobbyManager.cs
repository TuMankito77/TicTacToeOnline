namespace TicTacToeOnline.Networking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    
    using Unity.Services.Lobbies;
    using Unity.Services.Lobbies.Models;

    using TicTacToeOnline.Core;

    public class LobbyManager : SingletonBehavior<LobbyManager>
    {
        public const string PLAYER_NAME_KEY = "PlayerName";
        public const string RELAY_CODE_KEY = "RelayCode";
        
        public Action<Lobby, LobbyUpdateType> onLobbyInformationUpdated = null;
        public Action onKickedFromLobby = null;

        private Coroutine keepLobbyAliveCoroutine = null;
        private LobbyEventCallbacks lobbyEventCallbacks = null;
        
        public bool IsLobbyHost { get; private set; } = false;
        public Lobby Lobby { get; private set; } = null;
        public string LobbyUpdateTypeKey => typeof(LobbyUpdateType).Name;

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            lobbyEventCallbacks = new LobbyEventCallbacks();
            lobbyEventCallbacks.LobbyChanged += OnLobbyChanged;
            lobbyEventCallbacks.KickedFromLobby += OnKickedFromLobby;
            lobbyEventCallbacks.LobbyEventConnectionStateChanged += OnLobbyEventConnectionStateChanged;
        }

        #endregion

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

                Lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
                await LobbyService.Instance.SubscribeToLobbyEventsAsync(Lobby.Id, lobbyEventCallbacks);
                keepLobbyAliveCoroutine = StartCoroutine(KeepLobbyAlive());
                IsLobbyHost = true;
                OnSucess?.Invoke(Lobby);
                Debug.Log($"Lobby created successfully! Lobby name: {Lobby.Name} - Max number of players: {Lobby.MaxPlayers}");
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

                Lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);
                await LobbyService.Instance.SubscribeToLobbyEventsAsync(Lobby.Id, lobbyEventCallbacks);
                OnSuccess?.Invoke(Lobby);
            }
            catch(Exception e)
            {
                OnFailure?.Invoke();
                Debug.LogError(e.Message);
            }
        }

        public async void SetLobbyRelayCode(string code, Action OnSuccess, Action OnFailure)
        {
            try
            {
                UpdateLobbyOptions updateLobbyOptions = new UpdateLobbyOptions()
                {
                    Data = new Dictionary<string, DataObject>()
                    {
                        { RELAY_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, code) }
                    }
                };

                await LobbyService.Instance.UpdateLobbyAsync(Lobby.Id, updateLobbyOptions);
                OnSuccess?.Invoke();
            }
            catch (LobbyServiceException e)
            {
                OnFailure?.Invoke();
                Debug.LogError(e.Message);
            }
        }

        public async void DestroyLobby(string lobbyId)
        {
            try
            {
                if(keepLobbyAliveCoroutine != null)
                {
                    StopCoroutine(keepLobbyAliveCoroutine);
                    keepLobbyAliveCoroutine = null;
                }

                Lobby = null;
                await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
            }
            catch(LobbyServiceException e)
            {
                Debug.LogError(e.Message);
            }
        }

        public async void DisconnectFromLobby(string lobbyId, string playerId)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(lobbyId, playerId);
            }
            catch(LobbyServiceException e)
            {
                Debug.LogError(e.Message);
            }
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

                Lobby = await LobbyService.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);

                Debug.Log($"Joined to lobby successfully! Lobby name: {Lobby.Name} - Max number of players: {Lobby.MaxPlayers}");
            }
            catch(LobbyServiceException e)
            {
                Debug.LogError(e.Message);
            }
        }

        private IEnumerator KeepLobbyAlive()
        {
            yield return new WaitForSeconds(15);

            if(Lobby != null)
            {
                SendLobbyHeartbeat();
            }
        }

        private async void SendLobbyHeartbeat()
        {
            try
            {
                await LobbyService.Instance.SendHeartbeatPingAsync(Lobby.Id);
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
                Lobby = await LobbyService.Instance.QuickJoinLobbyAsync();
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

        private async void UpdateLobbyInformation(LobbyUpdateType lobbyUpdateType)
        {
            try
            {
                Lobby = await LobbyService.Instance.GetLobbyAsync(Lobby.Id);
                onLobbyInformationUpdated?.Invoke(Lobby, lobbyUpdateType);
                Debug.Log("Lobby information updated.");
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e.Message);
            }
        }

        private void OnLobbyChanged(ILobbyChanges lobbyChanges)
        {
            if(lobbyChanges.PlayerJoined.Changed)
            {
                UpdateLobbyInformation(LobbyUpdateType.PlayerJoined);
            }
            if(lobbyChanges.Data.Changed)
            {
                UpdateLobbyInformation(LobbyUpdateType.DataChanged);
            }
            if(lobbyChanges.PlayerLeft.Changed)
            {
                UpdateLobbyInformation(LobbyUpdateType.PlayerLeft);
            }

            Debug.Log($"The lobby has changed.");
        }

        private void OnKickedFromLobby()
        {
            onKickedFromLobby?.Invoke();
            Debug.Log("You have been kicked out of the lobby.");
        }

        private void OnLobbyEventConnectionStateChanged(LobbyEventConnectionState lobbyEventConnectionState)
        {
            Debug.Log($"The connection state of the lobby changed. {lobbyEventConnectionState}");
        }
    }
}

