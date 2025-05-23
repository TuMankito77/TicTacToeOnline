namespace TicTacToeOnline.Gameplay
{
    using System;

    using UnityEngine;
    
    using Unity.Netcode;
    using TicTacToeOnline.Ui.Views;
    using TicTacToeOnline.Networking;
    using Unity.Services.Lobbies.Models;

    public class GameManager : NetworkBehaviour
    {
        public const string MUSIC_VOLUME_PREFS_KEY = "MusicVolume";
        public const string EFFECTS_VOLUME_PREFS_KEY = "EffectsVolume";

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

        public event EventHandler<OnMatchFinishedEventArgs> OnMatchFinished;

        public class OnMatchFinishedEventArgs : EventArgs
        {
            public Vector2 MiddleCellCanvasPosition { get; private set; }
            public LineOrientation LineOrientation { get; private set; }
            public PlayerType Winner { get; private set; }

            public OnMatchFinishedEventArgs(Vector2 middleCellCanvasPosition, LineOrientation lineOrientation, PlayerType winner)
            {
                MiddleCellCanvasPosition = middleCellCanvasPosition;
                LineOrientation = lineOrientation;
                Winner = winner;
            }
        }
        
        public event EventHandler OnPlayerTurnUpdated;
        public event EventHandler OnGameRestarted;
        public event EventHandler OnScoresUpdated;
        public event EventHandler OnMarkPlaced;

        [SerializeField]
        private ViewManager viewManager = null;

        [SerializeField]
        private AudioSource musicAudioSource = null;

        [SerializeField]
        private AudioSource effectsAudioSource = null;

        private static GameManager instance = null;
        private PlayerType localPlayerType = PlayerType.None;
        private GridCellInfo[,] gridCellsInfo = null;
        private Line[] lines = null;
        private NetworkVariable<PlayerType> playerTypeTurn = new NetworkVariable<PlayerType>(PlayerType.None, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private NetworkVariable<int> crossesScore = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private NetworkVariable<int> circlesScore = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        
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
        public string PlayerName { get; set; }

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
            gridCellsInfo = new GridCellInfo[3, 3];

            lines = new Line[]
            {
                new Line()
                {
                    gridPositions = new Vector2Int[]
                    {
                        new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2)
                    },
                    lineOrientation = LineOrientation.Horizontal
                },

                new Line()
                {
                    gridPositions = new Vector2Int[]
                    {
                        new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2)
                    },
                    lineOrientation = LineOrientation.Horizontal
                },

                new Line()
                {
                    gridPositions = new Vector2Int[]
                    {
                        new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(2, 2)
                    },
                    lineOrientation = LineOrientation.Horizontal
                },

                new Line()
                {
                    gridPositions = new Vector2Int[]
                    {
                        new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0)
                    },
                    lineOrientation = LineOrientation.Vertical
                },

                new Line()
                {
                    gridPositions = new Vector2Int[]
                    {
                        new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1)
                    },
                    lineOrientation = LineOrientation.Vertical
                },

                new Line()
                {
                    gridPositions = new Vector2Int[]
                    {
                        new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2)
                    },
                    lineOrientation = LineOrientation.Vertical
                },

                new Line()
                {
                    gridPositions = new Vector2Int[]
                    {
                        new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(2, 2)
                    },
                    lineOrientation = LineOrientation.DiagonalA
                },

                new Line()
                {
                    gridPositions = new Vector2Int[]
                    {
                        new Vector2Int(0, 2), new Vector2Int(1, 1), new Vector2Int(2, 0)
                    },
                    lineOrientation = LineOrientation.DiabonalB
                }
            };

            Application.targetFrameRate = 60;
            musicAudioSource.volume = PlayerPrefs.HasKey(MUSIC_VOLUME_PREFS_KEY) ? PlayerPrefs.GetFloat(MUSIC_VOLUME_PREFS_KEY) : 1.0f;
            effectsAudioSource.volume = PlayerPrefs.HasKey(EFFECTS_VOLUME_PREFS_KEY) ? PlayerPrefs.GetFloat(EFFECTS_VOLUME_PREFS_KEY) : 1.0f;
        }

        private void Start()
        {
            viewManager.DisplayView<LoadingView>();
            OnlineServicesManager.Instance.OnAnonimousSignInSucess += OnAnonimousSignInSuccess;
            OnlineServicesManager.Instance.OnAnonimousSignInFail += OnAnonimousSignInFailure;
            OnlineServicesManager.Instance.SignInAnonymously();
            LobbyManager.Instance.onLobbyInformationUpdated += OnLobbyInformationUpdated;
        }

        public void OnDisable()
        {
            OnlineServicesManager.Instance.OnAnonimousSignInSucess -= OnAnonimousSignInSuccess;
            OnlineServicesManager.Instance.OnAnonimousSignInFail -= OnAnonimousSignInFailure;
            LobbyManager.Instance.onLobbyInformationUpdated -= OnLobbyInformationUpdated;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (NetworkManager.Singleton.LocalClientId == 0)
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

            crossesScore.OnValueChanged +=
                (crossesPreviousScore, crossesUpdatedScore) =>
                {
                    OnScoresUpdated?.Invoke(this, EventArgs.Empty);
                };

            circlesScore.OnValueChanged +=
                (circlesPreviousScore, circlesUpdatedScore) =>
                {
                    OnScoresUpdated?.Invoke(this, EventArgs.Empty);
                };

            if (IsServer)
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

            if (gridCellsInfo[gridPosition.x, gridPosition.y].playerTypeOwner != PlayerType.None)
            {
                return;
            }

            SendOnMarkPlacedRpc();
            gridCellsInfo[gridPosition.x, gridPosition.y].playerTypeOwner = playerType;
            gridCellsInfo[gridPosition.x, gridPosition.y].canvasPosition = canvasPosition;

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

            SendOnClickedOnGridPositionEventRpc(canvasPosition, playerType);
            TestWinner();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void SendOnMarkPlacedRpc()
        {
            OnMarkPlaced?.Invoke(this, EventArgs.Empty);
        }

        [Rpc(SendTo.Server)]
        public void SendRestartGameRpc()
        {
            ResetGridCellInfo();
            playerTypeTurn.Value = PlayerType.Circle;
            TriggerRestartGameRpc();
        }

        public void GetPlayerScores(out int circlesScore, out int crossesScore)
        {
            circlesScore = this.circlesScore.Value;
            crossesScore = this.crossesScore.Value;
        }

        public void StartMatch(Action OnMatchStartSucess, Action OnMatchStartFailure)
        {
            RelayManager.Instance.CreateRelay
            (
                1,
                (joinCode) => 
                {
                    LobbyManager.Instance.SetLobbyRelayCode
                    (
                        joinCode, 
                        () => 
                        { 
                            OnMatchStartSucess?.Invoke();
                            viewManager.DisplayView<GridView>();
                            viewManager.DisplayView<Hud>();
                        }, 
                        () => { OnMatchStartFailure?.Invoke(); }
                    );
                },
                () => { OnMatchStartFailure?.Invoke();}
            );
        }

        public void FinishMatch()
        {
            ResetGridCellInfo();
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            viewManager.RemoveView<Hud>();
            viewManager.RemoveView<GridView>();
            viewManager.DisplayView<MainMenuView>();
        }

        public void UpdateMusicVolume(float volume)
        {
            musicAudioSource.volume = volume;
            PlayerPrefs.SetFloat(MUSIC_VOLUME_PREFS_KEY, volume);
        }

        public void UpdateEffectsVolume(float volume)
        {
            effectsAudioSource.volume = volume;
            PlayerPrefs.SetFloat(EFFECTS_VOLUME_PREFS_KEY, volume);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SendMatchFinishedInformationRpc(Vector2 middleLineCanvasPosition, LineOrientation lineOrientation, PlayerType winner)
        {
            GameOverView gameOverView = viewManager.DisplayView<GameOverView>();
            gameOverView.UpdateWinnerInformation(winner);
            
            OnMatchFinished?.Invoke(this, new OnMatchFinishedEventArgs
                (
                    middleLineCanvasPosition,
                    lineOrientation,
                    winner
                ));
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SendOnClickedOnGridPositionEventRpc(Vector2 canvasPosition, PlayerType playerType)
        {
            OnClickedOnGridPosition?.Invoke(this, new OnClickedOnGridPositionEventArgs(canvasPosition, playerType));
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void TriggerRestartGameRpc()
        {
            viewManager.RemoveView<GameOverView>();
            OnGameRestarted?.Invoke(this, EventArgs.Empty);
        }

        private void ResetGridCellInfo()
        {
            for (int i = 0; i < gridCellsInfo.GetLength(0); i++)
            {
                for (int j = 0; j < gridCellsInfo.GetLength(1); j++)
                {
                    gridCellsInfo[i, j] = new GridCellInfo()
                    {
                        canvasPosition = Vector2.zero,
                        playerTypeOwner = PlayerType.None
                    };
                }
            }
        }

        private void TestWinner()
        {
            foreach(Line line in lines)
            {
                if(IsThereAWinLine(line))
                {
                    Debug.Log($"{GetType().Name} - We have a winner!");
                    playerTypeTurn.Value = PlayerType.None;
                    
                    Vector2 middleLineCanvasPosition =
                        gridCellsInfo[line.gridPositions[1].x, line.gridPositions[1].y].canvasPosition;
                    
                    LineOrientation lineOrientation = line.lineOrientation;
                    
                    PlayerType winner =
                        gridCellsInfo[line.gridPositions[1].x, line.gridPositions[1].y].playerTypeOwner;

                    switch(winner)
                    {
                        case PlayerType.Circle:
                            {
                                circlesScore.Value++;
                                break;
                            }

                        case PlayerType.Cross:
                            {
                                crossesScore.Value++;
                                break;
                            }

                        default:
                            {
                                break;
                            }
                    }

                    SendMatchFinishedInformationRpc(middleLineCanvasPosition, lineOrientation, winner);
                    return;
                }
            }

            bool wasTieFound = true;

            for(int i = 0; i < gridCellsInfo.GetLength(0); i++)
            {
                for(int j = 0; j < gridCellsInfo.GetLength(1); j++)
                {
                    if (gridCellsInfo[i,j].playerTypeOwner == PlayerType.None)
                    {
                        wasTieFound = false;
                        break;
                    }
                }
            }

            if(wasTieFound)
            {
                SendMatchFinishedInformationRpc(Vector2.zero, LineOrientation.None, PlayerType.None);
            }
        }

        private bool IsThereAWinLine(Line line)
        {
            return gridCellsInfo[line.gridPositions[0].x, line.gridPositions[0].y].playerTypeOwner != PlayerType.None &&
                gridCellsInfo[line.gridPositions[0].x, line.gridPositions[0].y].playerTypeOwner == gridCellsInfo[line.gridPositions[1].x, line.gridPositions[1].y].playerTypeOwner &&
                gridCellsInfo[line.gridPositions[1].x, line.gridPositions[1].y].playerTypeOwner == gridCellsInfo[line.gridPositions[2].x, line.gridPositions[2].y].playerTypeOwner;
        }

        private void OnClientConnected(ulong obj)
        {
            if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
            {
                playerTypeTurn.Value = PlayerType.Circle;
            }
        }

        private void OnAnonimousSignInSuccess()
        {
            viewManager.RemoveView<LoadingView>();
            OnlineServicesManager.Instance.OnAnonimousSignInSucess -= OnAnonimousSignInSuccess;
            OnlineServicesManager.Instance.OnAnonimousSignInFail -= OnAnonimousSignInFailure;
            viewManager.DisplayView<MainMenuView>();
        }

        private void OnAnonimousSignInFailure()
        {
            viewManager.RemoveView<LoadingView>();
            MessageView messageView = viewManager.DisplayView<MessageView>();
            messageView.SetMessageText("Failed to connect to online services. Press the button below to try again.");

            void OnAnonimousSignInFailureMessageClosed()
            {
                messageView.onCloseButtonOkPressed -= OnAnonimousSignInFailureMessageClosed;
                OnlineServicesManager.Instance.SignInAnonymously();
            }

            messageView.onCloseButtonOkPressed += OnAnonimousSignInFailureMessageClosed;
        }
        
        private void OnLobbyInformationUpdated(Lobby lobby, LobbyUpdateType lobbyUpdateType)
        {
            switch(lobbyUpdateType)
            {
                case LobbyUpdateType.DataChanged:
                    {
                        if(OnlineServicesManager.Instance.GetPlayerId() != lobby.HostId)
                        {
                            if(lobby.Data.TryGetValue(LobbyManager.RELAY_CODE_KEY, out DataObject RelayCodeDO))
                            {
                                viewManager.DisplayView<LoadingView>();
                                RelayManager.Instance.JoinRelay
                                (
                                    RelayCodeDO.Value,
                                    () => 
                                    { 
                                        viewManager.RemoveView<LoadingView>();
                                        viewManager.RemoveView<SessionView>();
                                        viewManager.DisplayView<GridView>();
                                        viewManager.DisplayView<Hud>();
                                    },
                                    () => 
                                    { 
                                        viewManager.RemoveView<LoadingView>(); 
                                    }
                                );
                            }
                        }

                        break;
                    }
            }
        }
    }
}