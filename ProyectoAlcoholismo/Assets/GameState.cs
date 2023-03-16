using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using Fusion.Sockets;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GameState : MonoBehaviour, INetworkRunnerCallbacks
{
    // singleton
    public static GameState Instance { get; private set; }
    
    // Local player data, not networked
    public string uniqueID = Utils.StringUtils.generateRandomString();
    public string myPlayerName = "No-named";

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
    public Dictionary<int, PlayerBehaviour> GameplayState = new Dictionary<int, PlayerBehaviour>();
    
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedObjects = new Dictionary<PlayerRef, NetworkObject>();
    
    // Buffer do not generate network traffic unless they change value.
    [Networked, Capacity(64), SerializeField] private NetworkArray<float> CommonArray { get; set; }
    
    private void Awake()
    {
	    if (Instance != null) { Destroy(gameObject); return; }
		Instance = this;

		CommonArray = new NetworkArray<float>();
		
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

    public void AddPlayer(PlayerRef player, PlayerBehaviour playerBehaviour)
    {
        if (GameplayState.ContainsKey(player.PlayerId)) return;

        GameplayState[player.PlayerId] = playerBehaviour;
    }

    public void RemovePlayer(int playerNum)
    {
        if (!GameplayState.ContainsKey(playerNum)) return;

        if(!GameplayState[playerNum].IsUnityNull())
			Destroy(GameplayState[playerNum]);
        
        GameplayState.Remove(playerNum);
    }
    
    public void RemovePlayer(string player)
    {
	    if (player.IsUnityNull()) return;
	    
	    if (GameplayState.Values.Any(x => x.playerName == player))
	    {
		    var key = GameplayState.FirstOrDefault(x => x.Value.playerName == player).Key;
		    RemovePlayer(key);
	    }
    }

    public void modifyPlayerScore(int playerNum, int valueToAdd)
    {
	    if (!GameplayState.ContainsKey(playerNum)) return;

	    var currentScore = GameplayState[playerNum].playerScore + valueToAdd;
	    currentScore = Math.Min(100, Math.Max(0, currentScore));

	    GameplayState[playerNum].playerScore = currentScore;
    }
    
    public void modifyPlayerScore(string playerName, int valueToAdd)
    {
	    if (playerName.IsUnityNull()) return;
	    
	    if (GameplayState.Values.Any(x => x.playerName == playerName))
	    {
		    var key = GameplayState.FirstOrDefault(x => x.Value.playerName == playerName).Key;
		    modifyPlayerScore(key, valueToAdd);
	    }
    }

    public void modifyPlayerColor(int playerNum, Vector3 color)
    {
	    if (!GameplayState.ContainsKey(playerNum)) return;

	    GameplayState[playerNum].playerColor = color;
    }
    
    public void modifyPlayerColor(string playerName, Vector3 color)
    {
	    if (playerName.IsUnityNull()) return;
	    
	    if (GameplayState.Values.Any(x => x.playerName == playerName))
	    {
		    var key = GameplayState.FirstOrDefault(x => x.Value.playerName == playerName).Key;
		    modifyPlayerColor(key, color);
	    }
    }
	    
    public void modifyPlayerName(int playerNum, string newName)
    {
	    if (GameplayState.ContainsKey(playerNum))
	    {
		    GameplayState[playerNum].playerName = newName;
	    }
    }

    public void modifyPlayerName(string oldName, string newName)
    {
	    if (GameplayState.Values.Any(x => x.playerName == oldName))
	    {
		    var key = GameplayState.FirstOrDefault(x => x.Value.playerName == oldName).Key;
		    modifyPlayerName(key, newName);
	    }
    }

    public void modifyPlayerTime(string playerName, float time)
    {
	    if (playerName.IsUnityNull()) return;
	    
	    if (GameplayState.Values.Any(x => x.playerName == playerName))
	    {
		    var key = GameplayState.FirstOrDefault(x => x.Value.playerName == playerName).Key;
		    modifyPlayerTime(key, time);
	    }
    }

    public void modifyPlayerTime(int playerNum, float time)
    {
	    if (GameplayState.ContainsKey(playerNum))
	    {
		    GameplayState[playerNum].playerTime = time;
	    }
    }

    public void modifyPlayerReadyValue(int playerNum, bool ready)
    {
	    if (GameplayState.ContainsKey(playerNum))
	    {
		    GameplayState[playerNum].isReady = ready;
	    }
    }

    public void modifyPlayerReadyValue(string playerName, bool ready)
    {
	    if (playerName.IsUnityNull()) return;
	    
	    if (GameplayState.Values.Any(x => x.playerName == playerName))
	    {
		    var key = GameplayState.FirstOrDefault(x => x.Value.playerName == playerName).Key;
		    modifyPlayerReadyValue(key, ready);
	    }
    }
    
    List<PlayerBehaviour> getGlobalScores()
    {
        var results = GameplayState.Values.ToList();
        results.Sort();

        return results;
    }

    List<PlayerBehaviour> getTimesList()
    {
	    var results = GameplayState.Values.ToList();
	    results.Sort(delegate(PlayerBehaviour x, PlayerBehaviour y)
		    {
			    if (x.playerTime < y.playerTime) return 1;
			    if (x.playerTime > y.playerTime) return -1;
			    return 0;
		    }
	    );

	    return results;
    }
	
	public void ResetScores()
	{
		foreach(var p in GameplayState.Values)
		{
			p.playerScore = 100;
			p.playerTime = 0;
		}
	}

	public void modifyMyName(string newName)
	{
		modifyPlayerName(Runner.LocalPlayer.PlayerId, newName);
	}

	public void modifyMyTime(float time)
	{
		modifyPlayerTime(Runner.LocalPlayer.PlayerId, time);
	}
	
	public void modifyMyScore(int valueToAdd)
	{
		modifyPlayerScore(Runner.LocalPlayer.PlayerId, valueToAdd);
	}

	public void modifyMyReadyValue(bool ready)
	{
		modifyPlayerReadyValue(Runner.LocalPlayer.PlayerId, ready);
	}

	public void modifyMyColor(Vector3 color)
	{
		modifyPlayerColor(Runner.LocalPlayer.PlayerId, color);
	}

	public void modifyCommonData(int key, float value)
	{
		this.CommonArray.Set(key, value);
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
		_spawnedObjects.Clear();
		
		if (shutdownReason != ShutdownReason.Ok)
		{
			Debug.Log("ERROR: Shutdown! Reason: " + shutdownReason);
		}
	}
	
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		Debug.Log("OnPlayerJoined: valid ? " + player.IsValid);
		Debug.Log(("Players in session: " + runner.ActivePlayers.Count()));
		
		// if(runner.LocalPlayer.PlayerId == player.PlayerId)
		// 	Debug.Log("soy yo");

		if (runner.IsServer)
		{
			// Spawn Player related things and store them
			NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity, player);
			runner.SetPlayerObject(player, networkPlayerObject);
			this.AddPlayer(player, networkPlayerObject.GetComponent<PlayerBehaviour>());
			_spawnedObjects.Add(player, networkPlayerObject);
		}
	}

	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		Debug.Log("OnPlayerLeft");

		if (runner.IsServer)
		{
			if (_spawnedObjects.TryGetValue(player, out NetworkObject networkObject))
			{
				this.RemovePlayer(player);
				runner.Despawn(networkObject);
				_spawnedObjects.Remove(player);
			}
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
