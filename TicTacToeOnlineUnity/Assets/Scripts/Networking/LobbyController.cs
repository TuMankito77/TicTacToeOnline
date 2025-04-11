namespace TicTacToeOnline.Networking
{
    using System.Collections.Generic;

    using UnityEngine;
    
    using Unity.Services.Core;
    using Unity.Services.Authentication;
    using Unity.Services.Lobbies;
    using Unity.Services.Lobbies.Models;
    using System.Collections;

    public class LobbyController : MonoBehaviour
    {
        private bool isPlayerSignedIn = false;
        private Lobby lobbyCreated = null;
        private Lobby lobbyJoined = null;
        private Coroutine keepLobbyAliveCoroutine;

        #region Unity Methods

        private async void Start()
        {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += OnSignedIn;
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        private void Update()
        {
            if(isPlayerSignedIn)
            {
                if(Input.GetKeyDown(KeyCode.Space) && lobbyCreated == null)
                {
                    CreateLobby();
                }

                if(Input.GetKeyDown(KeyCode.L) && lobbyJoined == null)
                {
                    ListLobbies();
                }

                if(Input.GetKeyDown(KeyCode.J) && lobbyJoined == null)
                {
                    ConnectToFirstLobbyFound();
                }

                if(Input.GetKeyDown(KeyCode.Q) && lobbyJoined == null)
                {
                    QuickJoinLobby();
                }
            }
        }

        #endregion

        private void OnSignedIn()
        {
            isPlayerSignedIn = true;
            Debug.Log($"Signed in as: {AuthenticationService.Instance.PlayerId}");
        }

        private async void CreateLobby()
        {
            try
            {
                string lobbyName = "My lobby";
                int maxPlayers = 2;
                lobbyCreated = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
                keepLobbyAliveCoroutine = StartCoroutine(KeepLobbyAlive());
                Debug.Log($"Lobby created successfully! Lobby name: {lobbyCreated.Name} - Max number of players: {lobbyCreated.MaxPlayers}");
            }
            catch(LobbyServiceException e)
            {
                Debug.LogError(e);
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
    }
}

