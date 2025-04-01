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

        #region Unity Methods

        private void Start()
        {
            GameManager.Instance.OnClickedOnGridPosition += OnClickedOnGridPosition;
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
