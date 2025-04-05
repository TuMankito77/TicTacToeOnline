namespace TicTacToeOnline.Ui
{
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

        #region Unity Methods

        private void Start()
        {
            GameManager.Instance.OnClickedOnGridPosition += OnClickedOnGridPosition;
            GameManager.Instance.OnMatchFinished += OnMatchFinished;
        }

        private void OnMatchFinished(object sender, GameManager.OnMatchFinishedEventArgs e)
        {
            if(!IsServer)
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

            GameObject winLineInstance = Instantiate(winLinePrefab);
            winLineInstance.GetComponent<NetworkObject>().Spawn(true);
            RectTransform winLineRectTransform = winLineInstance.GetComponent<RectTransform>();
            winLineRectTransform.SetParent(transform, false);
            winLineRectTransform.position = e.MiddleCellCanvasPosition;
            winLineRectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, eulerZ));
        }

        #endregion

        private void OnClickedOnGridPosition(object sender, GameManager.OnClickedOnGridPositionEventArgs e)
        {
            SpawnObjectRpc(e.CanvasPosition, e.PlayerType);
        }

        [Rpc(SendTo.Server)]
        private void SpawnObjectRpc(Vector2 canvasPosition, PlayerType playerType)
        {
            GameObject visual = null;
            
            switch(playerType)
            {
                case PlayerType.Circle:
                    {
                        visual = Instantiate(circlePrefab);
                        break;
                    }

                case PlayerType.Cross:
                    {
                        visual = Instantiate(crossPrefab);
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
            
            visual.GetComponent<NetworkObject>().Spawn(true);
            RectTransform visualRectTransform = visual.GetComponent<RectTransform>();
            visualRectTransform.SetParent(transform, false);
            visualRectTransform.position = canvasPosition;
        }
    }
}
