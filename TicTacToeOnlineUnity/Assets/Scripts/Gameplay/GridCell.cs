namespace TicTacToeOnline.Gameplay
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class GridCell : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField]
        private Vector2Int gridPosition = Vector2Int.zero;

        private RectTransform rectTransform = null;

        #region Unity Methods

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        #endregion

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log($"{GetType().Name} - You clicked the grid at position {gridPosition}");
            GameManager.Instance.ClickedOnGridPositionRpc(gridPosition, rectTransform.position, GameManager.Instance.LocalPlayerType);
        }
    }
}

