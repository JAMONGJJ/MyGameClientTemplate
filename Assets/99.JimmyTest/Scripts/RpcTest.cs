using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace ClientTemplate
{
    public class RpcTest : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            if (!IsServer && IsOwner)
            {
                TestServerRPC(0, NetworkObjectId);
            }
        }

        [ClientRpc]
        private void TestClientRPC(int value, ulong sourceNetworkObjectId)
        {
            Debug.Log($"Client Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
            if (IsOwner)
            {
                TestServerRPC(value + 1, sourceNetworkObjectId);
            }
        }

        [ServerRpc]
        private void TestServerRPC(int value, ulong sourceNetworkObjectId)
        {
            Debug.Log($"Server Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
            TestClientRPC(value, sourceNetworkObjectId);
        }
    }
}
