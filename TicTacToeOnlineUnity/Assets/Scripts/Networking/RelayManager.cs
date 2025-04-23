namespace TicTacToeOnline.Networking
{
    using UnityEngine;
    
    using Unity.Services.Relay;
    using Unity.Services.Relay.Models;
    using Unity.Netcode;
    using Unity.Netcode.Transports.UTP;

    using TicTacToeOnline.Core;
    using System;

    public class RelayManager : SingletonBehavior<RelayManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxConnections">Excluding the host. Eg. There are 4 players including the host, the value passsed is 3</param>
        public async void CreateRelay(int maxConnections, Action<string> OnSuccess, Action OnFailure)
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
                NetworkManager.Singleton.StartHost();
                Debug.Log($"Relay creater successfully, join code is: {joinCode}");
                OnSuccess?.Invoke(joinCode);
            }
            catch(RelayServiceException e)
            {
                Debug.LogError(e.Message);
            }
        }

        public async void JoinRelay(string joinCode, Action OnSuccess, Action OnFailure)
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
                NetworkManager.Singleton.StartClient();
                OnSuccess?.Invoke();
                Debug.Log("Relay joined successfullly.");
            }
            catch(RelayServiceException e)
            {
                Debug.LogError(e.Message);
                OnFailure?.Invoke();
            }
        }
    }
}


