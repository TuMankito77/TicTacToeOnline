namespace TicTacToeOnline.Networking
{
    using UnityEngine;
    
    using Unity.Services.Relay;
    using Unity.Services.Relay.Models;
    using Unity.Netcode;
    using Unity.Netcode.Transports.UTP;

    public class RelayManager : MonoBehaviour
    {
        private static RelayManager instance = null;

        public static RelayManager Instance
        {
            get
            {
                if(instance == null)
                {
                    GameObject relayManagerGO = new GameObject(typeof(RelayManager).Name);
                    instance = relayManagerGO.AddComponent<RelayManager>();
                }

                return instance;
            }
        }

        #region Unity Methods

        private void Awake()
        {
            if(instance != null && instance != this)
            {
                Destroy(gameObject);
                Debug.LogError($"{GetType().Name} - There is an already existing instance of this singleton, this instance will be destroyed.");
                return;    
            }

            instance = this;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxConnections">Excluding the host. Eg. There are 4 players including the host, the value passsed is 3</param>
        public async void CreateRelay(int maxConnections)
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData
                (
                    allocation.RelayServer.IpV4,
                    (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.Key,
                    allocation.ConnectionData
                );
                Debug.Log(joinCode);
            }
            catch(RelayServiceException e)
            {
                Debug.LogError(e.Message);
            }
        }

        public async void JoinRelay(string joinCode)
        {
            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData
                (
                    joinAllocation.RelayServer.IpV4,
                    (ushort)joinAllocation.RelayServer.Port,
                    joinAllocation.AllocationIdBytes,
                    joinAllocation.Key,
                    joinAllocation.ConnectionData,
                    joinAllocation.HostConnectionData
                );
            }
            catch(RelayServiceException e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}


