using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using UnityEngine;


public class PlayerRegistry : NetworkBehaviour, INetworkRunnerCallbacks
{
    public static PlayerRegistry Instance { get; private set; }
    
    [Networked, Capacity(16)] public NetworkDictionary<PlayerRef, PlayerBehaviour> ObjectByRef => default;
    
    [Networked(OnChanged = nameof(OnSceneChanged))]
    public string CurrentScene { get; set; }
    
    public static event Action<string> SceneChanged;
    
    public static int CountPlayers => Instance != null ? Instance.ObjectByRef.Count : 0;
    public static bool AllReady => Instance != null && Instance.ObjectByRef.Count(kvp => kvp.Value && kvp.Value.isReady) == CountPlayers;

    public override void Spawned()
    {
        Instance = this;
        Runner.AddCallbacks(this);
        DontDestroyOnLoad(gameObject);
        CurrentScene = "";
        SceneChanged += GameState.Instance.OnSceneChanged;
    }
    
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        Instance = null;
        runner.RemoveCallbacks(this);
    }

    public void SetScene(string sceneName)
    {
        CurrentScene = sceneName;
    }
    
    public static void Server_Add(NetworkRunner runner, PlayerRef pRef, PlayerBehaviour pObj)
    {
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

    public static void OnSceneChanged(Changed<PlayerRegistry> changedInfo)
    {
        SceneChanged?.Invoke(Instance.CurrentScene);	
    }
    
    public static void Server_Remove(NetworkRunner runner, PlayerRef pRef)
    {
        if (!pRef.IsValid || !Instance) return;

        if (Instance.ObjectByRef.Remove(pRef) == false)
        {
            Debug.LogWarning("Could not remove player from registry!");
        }
    }

    public List<Tuple<int, int>> SortedScores()
    {
        var result = new List<Tuple<int, int>>();

        var values = Instance.ObjectByRef.OrderByDescending(kp => kp.Value.playerScore);
        foreach (var (key, player) in values)
        {
            result.Add(new Tuple<int,int>(player.playerId, player.playerScore));
        }

        return result;
    }
    
    public List<Tuple<int, float>> SortedTimes()
    {
        var result = new List<Tuple<int, float>>();

        var values = Instance.ObjectByRef.OrderByDescending(kp => kp.Value.playerTime);
        foreach (var (key, player) in values)
        {
            result.Add(new Tuple<int,float>(player.playerId, player.playerTime));
        }

        return result;
    }
    
    public List<Tuple<int, int>> SortedScoresData0()
    {
        var result = new List<Tuple<int, int>>();

        var values = Instance.ObjectByRef.OrderByDescending(kp => kp.Value.data[0]);
        foreach (var (key, player) in values)
        {
            result.Add(new Tuple<int,int>(player.playerId, (int)(player.data[0])));
        }

        return result;
    }
    
    public List<Tuple<int, int>> SortedScoresApuestas()
    {
        var result = new List<Tuple<int, int>>();

        var values = Instance.ObjectByRef.OrderByDescending(kp => kp.Value.data[5]);
        foreach (var (key, player) in values)
        {
            result.Add(new Tuple<int,int>(player.playerId, (int)(player.data[0])));
        }

        return result;
    }



    #region INetworkRunnerCallbacks

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
    
    #endregion
}
