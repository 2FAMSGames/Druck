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
        
        // --- Client
        // Find the local non-networked PlayerData to read the data and communicate it to the Host via a single RPC 
        if (Object.HasInputAuthority)
        {
            RpcSetPlayerName(this.playerName);
            RpcSetPlayerColor(this.playerColor);
            RpcSetPlayerScore(this.playerScore);
            RpcSetPlayerTime(this.playerTime);
            RpcSetPlayerId(this.playerId);
        }

        // --- Host
        // Initialized game specific settings
        if (Object.HasStateAuthority)
        {
            GameState.Instance.AddPlayer(this.playerName, this);
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        // Same as spawned
        // --- Host
        // Initialized game specific settings
        if (Object.HasStateAuthority)
        {
            // Things to do when this object despawns.
            GameState.Instance.RemovePlayer(this.playerName);
        }
    }
    
    public override void FixedUpdateNetwork()
    {
        // Get input and apply to the object.
        // Para el juego del huevo.
    }

    public static void OnPlayerNameChanged(Changed<PlayerBehaviour> changedInfo)
    {
        Debug.Log(changedInfo.Behaviour.playerName + " OnPLayerNameChanged");
    }
    
    public static void OnPlayerScoreChanged(Changed<PlayerBehaviour> changedInfo)
    {
        Debug.Log(changedInfo.Behaviour.playerName + " OnPlayerScoreChanged");
    }
    
    public static void OnPlayerColorChanged(Changed<PlayerBehaviour> changedInfo)
    {
        Debug.Log(changedInfo.Behaviour.playerName + " OnPLayerColorChanged");
    }

    public static void OnPlayerTimeChanged(Changed<PlayerBehaviour> changedInfo)
    {
        Debug.Log(changedInfo.Behaviour.playerName + " OnPlayerTimeChanged");
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
        Debug.Log("Player " + playerName + " received score " + time);
        this.playerTime = time;
    }
    
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetPlayerId(string id)
    {
        this.playerId = id;
    }
}
