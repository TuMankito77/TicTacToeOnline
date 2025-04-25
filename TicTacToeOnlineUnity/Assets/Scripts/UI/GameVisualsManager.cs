namespace TicTacToeOnline.Ui
{
    using System.Collections.Generic;

    using UnityEngine;
    
    using Unity.Netcode;

    using TicTacToeOnline.Gameplay;

    public class GameVisualsManager : NetworkBehaviour
    {
        [SerializeField]
        private GameObject crossPrefab = null;

        [SerializeField]
        private GameObject circlePrefab = null;

        [SerializeField]
        private GameObject winLinePrefab = null;

        private List<GameObject> gridVisuals = null;

        #region Unity Methods

        private void Awake()
        {
            gridVisuals = new List<GameObject>();
        }

        private void Start()
        {
            GameManager.Instance.OnClickedOnGridPosition += OnClickedOnGridPosition;
            GameManager.Instance.OnMatchFinished += OnMatchFinished;
            GameManager.Instance.OnGameRestarted += OnGameRestarted;
        }

        private void OnGameRestarted(object sender, System.EventArgs e)
        {
            for(int i = 0; i < gridVisuals.Count; i++)
            {
                Destroy(gridVisuals[i]);
            }

            gridVisuals.Clear();
        }

        private void OnMatchFinished(object sender, GameManager.OnMatchFinishedEventArgs e)
        {
            if(e.Winner == PlayerType.None)
            {
                return;
            }

            float eulerZ = 0;

            switch(e.LineOrientation)
            {
                case LineOrientation.Horizontal:
                    {
                        eulerZ = 90;
                        break;
                    }

                case LineOrientation.Vertical:
                    {
                        eulerZ = 0;
                        break;
                    }

                case LineOrientation.DiagonalA:
                    {
                        eulerZ = -45;
                        break;
                    }

                case LineOrientation.DiabonalB:
                    {
                        eulerZ = 45;
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            GameObject winLineInstance = Instantiate(winLinePrefab, transform);
            RectTransform winLineRectTransform = winLineInstance.GetComponent<RectTransform>();
            winLineRectTransform.position = e.MiddleCellCanvasPosition;
            winLineRectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, eulerZ));
            gridVisuals.Add(winLineInstance);
        }

        #endregion

        private void OnClickedOnGridPosition(object sender, GameManager.OnClickedOnGridPositionEventArgs e)
        {
            SpawnObjectRpc(e.CanvasPosition, e.PlayerType);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SpawnObjectRpc(Vector2 canvasPosition, PlayerType playerType)
        {
            GameObject visual = null;
            
            switch(playerType)
            {
                case PlayerType.Circle:
                    {
                        visual = Instantiate(circlePrefab, transform);
                        break;
                    }

                case PlayerType.Cross:
                    {
                        visual = Instantiate(crossPrefab, transform);
                        break;
                    }

                default:
                    {
                        Debug.LogError($"{GetType()} - Invalid player type for mark visual.");
                        break;
                    }
            }

            if(visual == null)
            {
                return;
            }
            
            RectTransform visualRectTransform = visual.GetComponent<RectTransform>();
            visualRectTransform.position = canvasPosition;
            gridVisuals.Add(visual);
        }
    }
}
