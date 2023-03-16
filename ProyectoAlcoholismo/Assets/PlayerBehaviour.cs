using System;
using System.Diagnostics;
using System.Numerics;
using Fusion;
using Unity.VisualScripting;
using Debug = UnityEngine.Debug;
using Vector3 = UnityEngine.Vector3;

public class PlayerBehaviour : NetworkBehaviour, IComparable<PlayerBehaviour>
{
    private NetworkCharacterControllerPrototype _cc;
    
    [Networked(OnChanged = nameof(OnPlayerNameChanged))]
    public string playerName { get; set; }
    
    [Networked(OnChanged = nameof(OnPlayerScoreChanged))]
    public int playerScore { get; set; }
    
    [Networked(OnChanged = nameof(OnPlayerColorChanged))]
    public Vector3 playerColor { get; set; }

    [Networked(OnChanged = nameof(OnPlayerTimeChanged))]
    public float playerTime { get; set; }

    public string playerId { get; private set; }
    
    [Networked(OnChanged = nameof(OnReadyChanged))]
    public NetworkBool isReady { get; set; }
    
    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterControllerPrototype>();
    }

    public override void Spawned()
    {
        this.playerName = GameState.Instance.myPlayerName;
        this.playerId = GameState.Instance.uniqueID;
        this.playerScore = 100;
        this.playerTime = 0;
        this.playerColor = new Vector3(1, 1, 1);
        this.isReady = false;
        
        // --- Client
        // Find the local non-networked PlayerData to read the data and communicate it to the Host via a single RPC 
        if (Object.HasInputAuthority)
        {
            RpcSetPlayerName(this.playerName);
            RpcSetPlayerColor(this.playerColor);
            RpcSetPlayerScore(this.playerScore);
            RpcSetPlayerTime(this.playerTime);
            RpcSetPlayerId(this.playerId);
            RpcSetPlayerReady(this.isReady);
        }

        // --- Host
        // Initialized game specific settings
        if (Object.HasStateAuthority)
        {

        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        // Same as spawned
        // --- Host
        // Initialized game specific settings
        if (Object.HasStateAuthority)
        {
        }
    }
    
    public override void FixedUpdateNetwork()
    {
        // Get input and apply to the object.
        // Para el juego del huevo.
    }

    public static void OnPlayerNameChanged(Changed<PlayerBehaviour> changedInfo)
    {
        Debug.Log(changedInfo.Behaviour.playerName + " OnPLayerNameChanged to " + changedInfo.Behaviour.playerName);
    }
    
    public static void OnPlayerScoreChanged(Changed<PlayerBehaviour> changedInfo)
    {
        Debug.Log(changedInfo.Behaviour.playerName + " OnPlayerScoreChanged to " + changedInfo.Behaviour.playerScore);
    }
    
    public static void OnPlayerColorChanged(Changed<PlayerBehaviour> changedInfo)
    {
        Debug.Log(changedInfo.Behaviour.playerName + " OnPLayerColorChanged to " + changedInfo.Behaviour.playerColor);
    }

    public static void OnPlayerTimeChanged(Changed<PlayerBehaviour> changedInfo)
    {
        Debug.Log(changedInfo.Behaviour.playerName + " OnPlayerTimeChanged to " + changedInfo.Behaviour.playerTime);
    }

    public static void OnReadyChanged(Changed<PlayerBehaviour> changedInfo)
    {
        Debug.Log(changedInfo.Behaviour.playerName + " OnReadyChanged to " + changedInfo.Behaviour.isReady);
    }

    public int CompareTo(PlayerBehaviour other)
    {
        if ( this.playerScore < other.playerScore ) return 1;
        else if ( this.playerScore > other.playerScore ) return -1;
        else return 0;
    }
    
    // RPCs used to send player information to the Host
    //
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetPlayerName(string name)
    {
        if (string.IsNullOrEmpty(name)) return;
        Debug.Log("Player " + playerName + " received new name " + name);
        this.playerName = name;
    }
    
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetPlayerColor(Vector3 color)
    {
        Debug.Log("Player " + playerName + " received color " + color.ToString());
        this.playerColor = color;
    }
    
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetPlayerScore(int score)
    {
        Debug.Log("Player " + playerName + " received score " + score);
        this.playerScore = score;
    }
    
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetPlayerTime(float time)
    {
        Debug.Log("Player " + playerName + " received time " + time);
        this.playerTime = time;
    }
    
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetPlayerId(string id)
    {
        this.playerId = id;
    }

    private void RpcSetPlayerReady(NetworkBool ready)
    {
        Debug.Log("Player " + playerName + " ready ? " + ready);
        this.isReady = ready;
    }
}
