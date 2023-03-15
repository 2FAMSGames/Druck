using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using Fusion.Sockets;
using Unity.VisualScripting;
using UnityEngine;

public class GameState : MonoBehaviour, INetworkRunnerCallbacks
{
    // singleton
    public static GameState Instance { get; private set; }
    
    // Local player data
    public string uniqueID = Utils.StringUtils.generateRandomString();
    public string myPlayerName = "Nonamed";
    public int myPlayerScore = 100;
    public Vector3 myPlayerColor = new Vector3(1.0f, 1.0f, 1.0f);

    // possible, not used right now
    public Action<ulong> OnClientConnectedCallback;
    public Action<ulong> OnClientDisconnectedCallback;
    public Func<ulong> OnMasterClientSwitchedCallback;
    public Action StartHostCallback;
    public Action StartClientCallback;
    public Action RestoreHostCallback;
    public Action RestoreClientCallback;
	
    [SerializeField, ScenePath] string gameScene;
    
    // To be created on connection.
	public NetworkRunner runnerPrefab;
	public Fusion.NetworkObject managerPrefab;
	
	public NetworkRunner Runner { get; private set; }
    
	// Global game state, include networked players.
	// This should be Networked Behaviour and in another class.
    public Dictionary<string, PlayerBehaviour> GameplayState = new Dictionary<string, PlayerBehaviour>();
    

    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedObjects = new Dictionary<PlayerRef, NetworkObject>();
    
    private void Awake()
    {
	    if (Instance != null) { Destroy(gameObject); return; }
		Instance = this;
		
		DontDestroyOnLoad(this);
		Debug.Log("GameState.Awake() -> player unique id: " + uniqueID);
    }
	
	private void OnDestroy()
	{
		if (Instance == this) Instance = null;
	}
	
	public void CreateRoom(string roomName, System.Action successCallback = null)
	{
	    StartCoroutine(HostSessionRoutine(roomName, successCallback));
    }

    public void JoinRoom(string roomName, System.Action successCallback = null)
    {
	    StartCoroutine(JoinSessionRoutine(roomName, successCallback));
    }

    public void AddPlayer(string playerName, PlayerBehaviour playerBehaviour)
    {
        if (GameplayState.ContainsKey(playerName)) return;

        GameplayState[playerName] = playerBehaviour;
    }

    public void RemovePlayer(string playerName)
    {
        if (!GameplayState.ContainsKey(playerName)) return;

        if(!GameplayState[playerName].IsUnityNull())
			Destroy(GameplayState[playerName]);
        
        GameplayState.Remove(playerName);
    }

    public void modifyPlayerScore(string playerName, int score)
    {
        if (!GameplayState.ContainsKey(playerName)) return;
        
        var currentScore = GameplayState[playerName].playerScore + score;
        currentScore = Math.Min(100, Math.Max(0, currentScore));

        GameplayState[playerName].playerScore = currentScore;
    }

    public void modifyPlayerName(string oldName, string newName)
    {
        if (!GameplayState.ContainsKey(oldName) || GameplayState.ContainsKey(newName)) return;

        GameplayState[newName] = GameplayState[oldName];
        GameplayState[oldName] = null;
        GameplayState.Remove(oldName);
    }
    
    List<PlayerBehaviour> getScores()
    {
        var results = GameplayState.Values.ToList();
        results.Sort();

        return results;
    }
	
	public void ResetScores()
	{
		foreach(var p in GameplayState.Values)
		{
			GameplayState[p.playerId].playerScore = 100;
		}
	}

    public void DebugPrint()
    {
        Debug.Log("= GAME STATE DEBUG ===");
        Debug.Log("Players:");
        
        if(GameplayState.Count == 0)
            Debug.Log("No players");
        else
        {
            foreach(var p in GameplayState.Values)
            {
                Debug.Log("Name: " + p.playerName + " - id: " + p.playerId + " - score: " + p.playerScore + " - time " + p.playerTime);
            }
        }
        Debug.Log("= GAME STATE DEBUG END ===");
    }
    
	IEnumerator HostSessionRoutine(string roomName, System.Action successCallback)
	{
		if (!Runner)
		{
			Runner = Instantiate(runnerPrefab);
			Runner.GetComponent<NetworkEvents>().PlayerJoined.AddListener((runner, player) =>
			{
				if (runner.IsServer && runner.LocalPlayer == player)
				{
					runner.Spawn(managerPrefab);
				}
			});
			Runner.AddCallbacks(this);
		}

		var sceneManager = Runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();
		if (sceneManager == null)
		{
			sceneManager = Runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
		}
		
		Task<StartGameResult> task = Runner.StartGame(new StartGameArgs()
		{
			GameMode = GameMode.Host,
			SessionName = roomName,
			SceneManager = sceneManager
		});
		while (!task.IsCompleted)
		{
			yield return null;
		}
		StartGameResult result = task.Result;

		if (result.Ok)
		{
			if (successCallback != null)
				successCallback.Invoke();
			else
				Runner.SetActiveScene(gameScene);
			
			Debug.Log("Host initiated: " + roomName);
		}
		else
		{
			// A room with that same name could exist -> join
			Debug.Log("ERROR: Unable to Host to room: " + roomName + ". Reason: " + result.ShutdownReason);
			Debug.Log("Trying to join instead");
			StartCoroutine(JoinSessionRoutine(roomName, successCallback));
		}
	}

	IEnumerator JoinSessionRoutine(string roomName, System.Action successCallback)
	{
		if (Runner) Runner.Shutdown();
		Runner = Instantiate(runnerPrefab);
	
		var sceneManager = Runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();
		if (sceneManager == null)
		{
			sceneManager = Runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
		}

		Task<StartGameResult> task = Runner.StartGame(new StartGameArgs()
		{
			GameMode = GameMode.Client,
			SessionName = roomName,
			SceneManager = sceneManager,
			DisableClientSessionCreation = true
		});
		while (!task.IsCompleted)
		{
			yield return null;
		}
		StartGameResult result = task.Result;

		if (result.Ok)
		{
			if (successCallback != null)
				successCallback.Invoke();
			else
				Runner.SetActiveScene(gameScene);
			
			Debug.Log("Joined: " + roomName);
		}
		else
		{
			// Most common error, room doesn't exist -> host
			Debug.Log("ERROR: Unable to join to room: " + roomName + ". Reason: " + result.ShutdownReason);
			Debug.Log("Trying to host instead");
			StartCoroutine(HostSessionRoutine(roomName, successCallback));
		}
	}
	
	#region INetworkRunnerCallbacks

	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
		Runner = null;
		
		if (shutdownReason != ShutdownReason.Ok)
		{
			Debug.Log("ERROR: Shutdown! Reason: " + shutdownReason);
		}
	}
	
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		Debug.Log("OnPlayerJoined: valid ? " + player.IsValid);
		Debug.Log(("Players in session: " + runner.ActivePlayers.Count()));

		if(runner.LocalPlayer.PlayerId == player.PlayerId)
			Debug.Log("soy yo");
		
		if (runner.IsServer)
		{
			// Spawn Player related things and store them
			NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity, player);
			_spawnedObjects.Add(player, networkPlayerObject);
		}
	}

	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		Debug.Log("OnPlayerLeft");
		
		if (_spawnedObjects.TryGetValue(player, out NetworkObject networkObject))
		{
			runner.Despawn(networkObject);
			_spawnedObjects.Remove(player);
		}
	}

	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
		//Debug.Log("OnInput");
		
		// Collects local input for network transmission to host, like this
		//var data = new NetworkInputData();

		//if (Input.GetKey(KeyCode.W))
		//	data.direction += Vector3.forward;

		//input.Set(data);
	}

	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
		Debug.Log("OnInputMissing");
	}


	public void OnConnectedToServer(NetworkRunner runner)
	{
		Debug.Log("OnConnectedToServer" + runner.SessionInfo.ToString());
	}

	public void OnDisconnectedFromServer(NetworkRunner runner)
	{
		Debug.Log("OnDisconnectedFromServer");
	}

	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
		Debug.Log("OnConnectRequest" + request);
	}

	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
		Debug.Log("OnConnectFailed, reason: " + reason.ToString());
	}

	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
		Debug.Log("OnUserSimulationMessage, message: " + message.ToString());
	}

	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
		Debug.Log("OnSessionListUpdated");
	}

	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
		Debug.Log("OnCustomAuthenticationResponse");
	}

	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
		Debug.Log("OnHostMigration");
	}

	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
	{
		Debug.Log("OnReliableDataReceived");
	}

	public void OnSceneLoadDone(NetworkRunner runner)
	{
		Debug.Log("OnSceneLoadDone");
	}

	public void OnSceneLoadStart(NetworkRunner runner)
	{
		Debug.Log("OnSceneLoadStart");
	}
	#endregion

}
