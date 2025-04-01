namespace TicTacToeOnline.Gameplay
{
    using System;

    using UnityEngine;
    
    using Unity.Netcode;

    public class GameManager : NetworkBehaviour
    {
        public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;

        public class OnClickedOnGridPositionEventArgs : EventArgs
        {
            public Vector2 CanvasPosition { get; private set; }
            public PlayerType PlayerType { get; private set; }

            public OnClickedOnGridPositionEventArgs(Vector2 canvasPosition, PlayerType playerType)
            {
                CanvasPosition = canvasPosition;
                PlayerType = playerType;
            }
        }

        public event EventHandler OnGameStarted;
        public event EventHandler OnPlayerTurnUpdated;

        private static GameManager instance = null;
        private PlayerType localPlayerType = PlayerType.None;
        private NetworkVariable<PlayerType> playerTypeTurn = new NetworkVariable<PlayerType>(PlayerType.None, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public static GameManager Instance
        {
            get
            {
                if(instance == null)
                {
                    GameObject gameManagerGO = new GameObject("Game Manager");
                    DontDestroyOnLoad(gameManagerGO);
                    instance = gameManagerGO.AddComponent<GameManager>();
                }

                return instance;
            }
        }

        public PlayerType LocalPlayerType => localPlayerType;
        public PlayerType PlayerTypeTurn => playerTypeTurn.Value;

        #region Unity Methods

        private void Awake()
        {
            if(instance != null)
            {
                Debug.LogError($"{GetType()} - There is more than one instance of this singleton, all of them will be deleted exept for one.");
                Destroy(this.gameObject);
                return;
            }

            instance = this;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if(NetworkManager.Singleton.LocalClientId == 0)
            {
                localPlayerType = PlayerType.Cross;
            }
            else
            {
                localPlayerType = PlayerType.Circle;
            }

            playerTypeTurn.OnValueChanged += 
                (previousValue, updatedValue) => 
                { 
                    OnPlayerTurnUpdated?.Invoke(this, EventArgs.Empty); 
                };
            
            if(IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            }
        }


        #endregion

        [Rpc(SendTo.Server)]
        public void ClickedOnGridPositionRpc(Vector2Int gridPosition, Vector2 canvasPosition, PlayerType playerType)
        {
            Debug.Log($"{GetType().Name} - You clicked in the position {gridPosition}");
            
            if(playerTypeTurn.Value != playerType)
            {
                return;
            }

            switch(playerType)
            {
                case PlayerType.Circle:
                    {
                        playerTypeTurn.Value = PlayerType.Cross;
                        break;
                    }

                case PlayerType.Cross:
                    {
                        playerTypeTurn.Value = PlayerType.Circle;
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            OnClickedOnGridPosition?.Invoke(this, new OnClickedOnGridPositionEventArgs(canvasPosition, playerType));
        }

        private void OnClientConnected(ulong obj)
        {
            if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
            {
                playerTypeTurn.Value = PlayerType.Circle;
            }
        }
    }
}
