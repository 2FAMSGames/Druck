using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Collections;
using Fusion.Sockets;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerRegistry : NetworkBehaviour, INetworkRunnerCallbacks
{
    public static PlayerRegistry Instance { get; private set; }
    
    [Networked, Capacity(16)] public NetworkDictionary<PlayerRef, PlayerBehaviour> ObjectByRef => default;
    
    public static int CountPlayers => Instance != null ? Instance.ObjectByRef.Count : 0;
    public static bool AllReady => !Instance.IsUnityNull() && Instance.ObjectByRef.Count(kvp => kvp.Value && !kvp.Value.isReady) == Instance.ObjectByRef.Count;

    public override void Spawned()
    {
        Instance = this;
        Runner.AddCallbacks(this);
        DontDestroyOnLoad(gameObject);
    }
    
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        Instance = null;
        runner.RemoveCallbacks(this);
    }
    
    public static void Server_Add(NetworkRunner runner, PlayerRef pRef, PlayerBehaviour pObj)
    {
        Debug.Assert(runner.IsServer);

        if (Instance.ObjectByRef.Count < 16)
        {
            Instance.ObjectByRef.Add(pRef, pObj);
            DontDestroyOnLoad(pObj.gameObject);
        }
        else
        {
            Debug.LogWarning($"Unable to register player {pRef}, player limit reached!", pObj);
        }
    }
    
    public static void Server_Remove(NetworkRunner runner, PlayerRef pRef)
    {
        if (!runner.IsServer || !pRef.IsValid) return;

        if (Instance.ObjectByRef.Remove(pRef) == false)
        {
            Debug.LogWarning("Could not remove player from registry!");
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {}
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {}
    public void OnInput(NetworkRunner runner, NetworkInput input) {}
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {}
    public void OnConnectedToServer(NetworkRunner runner) {}
    public void OnDisconnectedFromServer(NetworkRunner runner) {}
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {}
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {}
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {}
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner){}
    public void OnSceneLoadStart(NetworkRunner runner) {}
}
