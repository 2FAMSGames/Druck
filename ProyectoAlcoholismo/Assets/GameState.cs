using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using Unity.VisualScripting;
using UnityEngine;

public class GameState : MonoBehaviour, INetworkRunnerCallbacks
{
    // singleton
    public static GameState Instance { get; private set; }
    
    // Local player data, not networked
    public string uniqueID = Utils.StringUtils.generateRandomString();
    public string myPlayerName = "No-named";
    public int myPlayerNum;

    public static bool AllReady => PlayerRegistry.AllReady;
    public static int PlayerCount => PlayerRegistry.CountPlayers;
    public static bool Connected => Instance != null && Instance.Runner != null;
    public static bool isServer => Instance != null && Instance.Runner.IsServer;
    

    // Eventos a los que conectarse:
    // 
    public event Action<int, int> PlayerChangedScore;
    public event Action<int, float> PlayerChangedTime;
    public event Action<int, string> PlayerChangedName;
    public event Action<int, bool> PlayerChangedReady;
    public event Action<int, Vector3> PlayerChangedColor;
    public event Action<int, NetworkDictionary<int,float>> PlayerChangedData;
    
    [SerializeField, ScenePath] string gameScene;
    
    // To be created on connection.
	public NetworkRunner runnerPrefab;
	public NetworkObject managerPrefab;
	
	public NetworkRunner Runner { get; private set; }

	[SerializeField] private NetworkPrefabRef playerPrefab;
    
    private Dictionary<PlayerRef, NetworkObject> spawnedObjects = new Dictionary<PlayerRef, NetworkObject>();

   
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
	
	public static void Server_Add(NetworkRunner runner, PlayerRef pRef, PlayerBehaviour pObj)
	{
		if (runner.IsServer)
		{
			PlayerRegistry.Server_Add(runner, pRef, pObj);
		}

		if (!pObj.HasInputAuthority)
		{
			Instance.AddToEventCallbacks(pObj);
		}
	}
	
	public static void Server_Remove(NetworkRunner runner, PlayerRef pRef)
	{
		if(!pRef.IsValid) return;

		if (pRef != runner.LocalPlayer)
		{
			Instance.RemoveFromEventCallbacks(GetPlayer(pRef));
		}
		
		if (runner.IsServer)
		{
			PlayerRegistry.Server_Remove(runner, pRef);
		}
	}
	
	public static bool HasPlayer(PlayerRef pRef)
	{
		return PlayerRegistry.Instance.ObjectByRef.ContainsKey(pRef);
	}
	
	public static bool HasPlayer(int id)
	{
		var value = PlayerRegistry.Instance.ObjectByRef.Where(s => s.Key.PlayerId == id);
		return !value.IsUnityNull();
	}
	
	public static PlayerBehaviour GetPlayer(PlayerRef pRef)
	{
		if (HasPlayer(pRef))
			return PlayerRegistry.Instance.ObjectByRef.Get(pRef);
		return null;
	}

	public static PlayerBehaviour GetPlayer(int id)
	{
		if (HasPlayer(id))
		{
			var value = PlayerRegistry.Instance.ObjectByRef.Where(s => s.Key.PlayerId == id);
			return value.FirstOrDefault().Value;
		}
		return null;
	}

	public static PlayerBehaviour GetMyPlayer()
	{
		return PlayerRegistry.Instance.ObjectByRef.Get(Instance.Runner.LocalPlayer);
	}
	
	public void CreateRoom(string roomName, Action successCallback = null)
	{
	    StartCoroutine(HostSessionRoutine(roomName, successCallback));
    }

    public void JoinRoom(string roomName, Action successCallback = null)
    {
	    StartCoroutine(JoinSessionRoutine(roomName, successCallback));
    }

    public void ModifyScore(int value)
    {
	    value = Math.Min(100, Math.Max(0, value));
	    GetMyPlayer().SetScore(value);
    }

    public void ModifyName(string myName)
    {
	    GetMyPlayer().SetName(myName);
    }

    public void ModifyTime(float time)
    {
	    GetMyPlayer().SetTime(time);
    }

    public void ModifyReadyFlag(bool flag)
    {
	    GetMyPlayer().SetReady(flag);
    }

    public void ModifyColor(Vector3 color)
    {
	    GetMyPlayer().SetColor(color);
    }
    
	public void ModifyData(int pos, float value)
	{
		GetMyPlayer().SetData(pos, value);
	}
    
	public void ResetPlayerScores()
	{
		GetMyPlayer().SetScore(100);
		GetMyPlayer().SetTime(0);
	}

	public void ResetPlayerData()
	{
		GetMyPlayer().ResetData();
	}

	private void AddToEventCallbacks(in PlayerBehaviour p)
	{
		p.ChangedColor += this.PlayerHasChangedColor;
		p.ChangedName += this.PlayerHasChangedName;
		p.ChangedReady += this.PlayerHasChangedReady;
		p.ChangedScore += this.PlayerHasChangedScore;
		p.ChangedTime += this.PlayerHasChangedTime;
		p.ChangedData += this.PlayerHasChangedData;
	}
	
	private void RemoveFromEventCallbacks(in PlayerBehaviour p)
	{
		p.ChangedColor -= this.PlayerHasChangedColor;
		p.ChangedName -= this.PlayerHasChangedName;
		p.ChangedReady -= this.PlayerHasChangedReady;
		p.ChangedScore -= this.PlayerHasChangedScore;
		p.ChangedTime -= this.PlayerHasChangedTime;
		p.ChangedData -= this.PlayerHasChangedData;
	}

	private void PlayerHasChangedTime(int id, float time)
	{
		PlayerChangedTime?.Invoke(id, time);
	}
	
	private void PlayerHasChangedScore(int id, int score)
	{
		PlayerChangedScore?.Invoke(id, score);	
	}
	
	private void PlayerHasChangedColor(int id, Vector3 color)
	{
		PlayerChangedColor?.Invoke(id, color);
	}
	
	private void PlayerHasChangedName(int id, string playerName)
	{
		PlayerChangedName?.Invoke(id, playerName);
	}

	private void PlayerHasChangedReady(int id, bool ready)
	{
		PlayerChangedReady?.Invoke(id, ready);
	}

	private void PlayerHasChangedData(int id, NetworkDictionary<int,float> data)
	{
		PlayerChangedData?.Invoke(id, data);
	}
	
	public List<Tuple<int,int>> SortedScores()
	{
		return PlayerRegistry.Instance.SortedScores();
	}
	
	public List<Tuple<int,int>> SortedTimes()
	{
		return PlayerRegistry.Instance.SortedTimes();
	}

    public void DebugPrint()
    {
        Debug.Log("= GAME STATE DEBUG ===");
        Debug.Log("Players:");
        
        if(Runner.IsUnityNull() || PlayerRegistry.Instance.ObjectByRef.Count == 0)
            Debug.Log("No players");
        else
        {
            foreach(var p in PlayerRegistry.Instance.ObjectByRef)
            {
	            var pl = PlayerRegistry.Instance.ObjectByRef.Get(p.Key);
                Debug.Log("Name: " + pl.playerName + " - id: " + p.Key.PlayerId + " - score: " +
                          pl.playerScore + " - time " + pl.playerTime + " - ready " + pl.isReady + 
                          " - color " + pl.playerColor);
            }
        }
        Debug.Log("= GAME STATE DEBUG END ===");
    }
    
	IEnumerator HostSessionRoutine(string roomName, Action successCallback)
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
			PlayerCount = 16,
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

	IEnumerator JoinSessionRoutine(string roomName, Action successCallback)
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
		Runner  = null;
		spawnedObjects.Clear();
		
		if (shutdownReason != ShutdownReason.Ok)
		{
			Debug.Log("ERROR: Shutdown! Reason: " + shutdownReason);
		}
	}
	
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		Debug.Log("OnPlayerJoined");
		
		if (runner.IsServer)
		{
			// Spawn Player related things and store them
			NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);
			networkPlayerObject.ReleaseStateAuthority();
			runner.SetPlayerObject(player, networkPlayerObject);
			spawnedObjects.Add(player, networkPlayerObject);
		}
		
		DebugPrint();
	}

	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		Debug.Log("OnPlayerLeft");

		Instance.RemoveFromEventCallbacks(GetPlayer(player));
		
		if (runner.IsServer)
		{
			if (spawnedObjects.TryGetValue(player, out NetworkObject networkObject))
			{
				runner.Despawn(networkObject);
				spawnedObjects.Remove(player);
				Server_Remove(runner, player);
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
		Debug.Log("OnConnectedToServer" + runner.SessionInfo);
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