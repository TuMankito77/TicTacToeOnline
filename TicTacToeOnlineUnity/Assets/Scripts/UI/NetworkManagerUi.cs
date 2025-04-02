namespace TicTacToeOnline.Ui
{
    using UnityEngine;
    using UnityEngine.UI;

    using Unity.Netcode;
    
    public class NetworkManagerUi : MonoBehaviour
    {
        [SerializeField]
        private Button hostButton = null;

        [SerializeField]
        private Button clientButton = null;

        #region Unity Methods

        private void Awake()
        {
            hostButton.onClick.AddListener(OnHostButtonPressed);
            clientButton.onClick.AddListener(OnClientButtonPressed);
        }

        #endregion

        private void OnHostButtonPressed()
        {
            NetworkManager.Singleton.StartHost();
            gameObject.SetActive(false);
        }

        private void OnClientButtonPressed()
        {
            NetworkManager.Singleton.StartClient();
            gameObject.SetActive(false);
        }
    }
}
