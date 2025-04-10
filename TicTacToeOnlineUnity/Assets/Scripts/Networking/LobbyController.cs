namespace TicTacToeOnline.Networking
{
    using UnityEngine;
    
    using Unity.Services.Core;
    using Unity.Services.Authentication;
    using Unity.Services.Lobbies;
    using Unity.Services.Lobbies.Models;

    public class LobbyController : MonoBehaviour
    {
        private bool isPlayerSignedIn = false;
        private bool wasLobbyCreated = false;

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
                if(Input.GetKeyDown(KeyCode.Space) && !wasLobbyCreated)
                {
                    CreateLobby();
                    wasLobbyCreated = true;
                }

                if(Input.GetKeyDown(KeyCode.L))
                {
                    ListLobbies();
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
                int maxPlayers = 4;
                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
                Debug.Log($"Lobby created successfully! Lobby name: {lobby.Name} - Max number of players: {lobby.MaxPlayers}");
            }
            catch(LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        private async void ListLobbies()
        {
            try
            {
                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
                Debug.Log($"Lobbies found: {queryResponse.Results.Count}");

                foreach(Lobby lobby in queryResponse.Results)
                {
                    Debug.Log($"Lobby name: {lobby.Name} - Max number of players: {lobby.MaxPlayers}");
                }
            }
            catch(LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
}

