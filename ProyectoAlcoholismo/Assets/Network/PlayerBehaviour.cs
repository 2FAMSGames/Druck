using System;
using System.Collections.Generic;
using Fusion;
using NetworkBehaviour = Fusion.NetworkBehaviour;
using Vector3 = UnityEngine.Vector3;

public class PlayerBehaviour : NetworkBehaviour, IComparable<PlayerBehaviour>
{
    private NetworkCharacterControllerPrototype _cc;

    public static PlayerBehaviour Local { get; private set; }
    
    public event Action<int, string> ChangedName;
    public event Action<int, int> ChangedScore;
    public event Action<int, float> ChangedTime;
    public event Action<int, Vector3> ChangedColor;
    public event Action<int, bool> ChangedReady;
    
    public event Action<int, NetworkDictionary<int,float>> ChangedData;
    
    [Networked(OnChanged = nameof(OnPlayerNameChanged))]
    public string playerName { get; private set; }
    
    [Networked(OnChanged = nameof(OnPlayerScoreChanged))]
    public int playerScore { get; private set; }
    
    [Networked(OnChanged = nameof(OnPlayerColorChanged))]
    public Vector3 playerColor { get; private set; }

    [Networked(OnChanged = nameof(OnPlayerTimeChanged))]
    public float playerTime { get; private set; }

    [Networked(OnChanged = nameof(OnPlayerIdChanged))]
    public int playerId { get; private set; }
    
    [Networked(OnChanged = nameof(OnReadyChanged))]
    public NetworkBool isReady { get; private set; }
    
    [Networked(OnChanged = nameof(OnPlayerDataChanged)), Capacity(10)]
    public NetworkDictionary<int, float> data { get; } = MakeInitializer(new Dictionary<int, float> {{ 0,0 } });
    
    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterControllerPrototype>();
    }
    
    public override void Spawned()
    {
        GameState.Server_Add(Runner, Object.InputAuthority, this);
        
        if (Object.HasInputAuthority)
        {
            this.playerName = GameState.Instance.myPlayerName;
            this.playerId = Runner.LocalPlayer.PlayerId;
            this.playerScore = 100;
            this.playerTime = 0;
            this.playerColor = new Vector3(1, 1, 1);
            this.isReady = false;
            
            RpcSetPlayerTime(this.playerTime);
            RpcSetPlayerId(this.playerId);
            RpcSetPlayerReady(this.isReady);
            RpcSetPlayerName(this.playerName);
            RpcSetPlayerColor(this.playerColor);
            RpcSetPlayerScore(this.playerScore);
            
            ResetData();
        }
    }
    
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        // Same as spawned
        // --- Host
        // Initialized game specific settings
        if (Object.HasStateAuthority)
        {
            GameState.Server_Remove(Runner, Object.InputAuthority);
        }
    }

    public void SetScore(int pScore)
    {
        if (Object.HasInputAuthority)
        {
            this.playerScore = pScore;
            RpcSetPlayerScore(pScore);
        }
    }

    public void SetName(string pName)
    {
        if (Object.HasInputAuthority)
        {
            this.playerName = pName;
            RpcSetPlayerName(pName);
        }
    }

    public void SetColor(Vector3 pColor)
    {
        if (Object.HasInputAuthority)
        {
            this.playerColor = pColor;
            RpcSetPlayerColor(pColor);
        }
    }

    public void SetReady(bool pReady)
    {
        if (Object.HasInputAuthority)
        {
            this.isReady = pReady;
            RpcSetPlayerReady(pReady);
        }
    }

    public void SetTime(float pTime)
    {
        if (Object.HasInputAuthority)
        {
            this.playerTime = pTime;
            RpcSetPlayerTime(pTime);
        }
    }

    public void SetData(int pos, float value)
    {
        if (Object.HasInputAuthority)
        {
            this.data.Set(pos, value);
            RpcSetPlayerData(pos, value);
        }
    }

    public void ResetData()
    {
        if (Object.HasInputAuthority)
        {
            this.data.Clear();
            for (int i = 0; i < 10; ++i)
            {
                this.data.Set(i, 0);
                RpcSetPlayerData(i, 0);
            }
            
            this.SetReady(false);
            RpcSetPlayerReady(false);
        }
    }
   
    public override void FixedUpdateNetwork()
    {
        // Get input and apply to the object.
        // Para el juego del huevo.
    }

    public static void OnPlayerNameChanged(Changed<PlayerBehaviour> changedInfo)
    {
        changedInfo.Behaviour.ChangedName?.Invoke(changedInfo.Behaviour.playerId, changedInfo.Behaviour.playerName);
    }
    
    public static void OnPlayerScoreChanged(Changed<PlayerBehaviour> changedInfo)
    {
        changedInfo.Behaviour.ChangedScore?.Invoke(changedInfo.Behaviour.playerId, changedInfo.Behaviour.playerScore);
    }
    
    public static void OnPlayerColorChanged(Changed<PlayerBehaviour> changedInfo)
    {
        changedInfo.Behaviour.ChangedColor?.Invoke(changedInfo.Behaviour.playerId, changedInfo.Behaviour.playerColor);
    }

    public static void OnPlayerTimeChanged(Changed<PlayerBehaviour> changedInfo)
    {
        changedInfo.Behaviour.ChangedTime?.Invoke(changedInfo.Behaviour.playerId, changedInfo.Behaviour.playerTime);
    }

    public static void OnReadyChanged(Changed<PlayerBehaviour> changedInfo)
    {
        changedInfo.Behaviour.ChangedReady?.Invoke(changedInfo.Behaviour.playerId, changedInfo.Behaviour.isReady);
    }

    public static void OnPlayerIdChanged(Changed<PlayerBehaviour> changedInfo)
    {
        // nothing
    }
    
    public static void OnPlayerDataChanged(Changed<PlayerBehaviour> changedInfo)
    {
        changedInfo.Behaviour.ChangedData?.Invoke(changedInfo.Behaviour.playerId, changedInfo.Behaviour.data);
    }

    public int CompareTo(PlayerBehaviour other)
    {
        if ( this.playerScore < other.playerScore ) return 1;
        else if ( this.playerScore > other.playerScore ) return -1;
        else return 0;
    }
    
    // RPCs used to send player information to the Host
    //
    [Fusion.Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetPlayerName(string name)
    {
        if (string.IsNullOrEmpty(name)) return;
        this.playerName = name;
    }
    
    [Fusion.Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetPlayerColor(Vector3 color)
    {
        this.playerColor = color;
    }
    
    [Fusion.Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetPlayerScore(int score)
    {
        this.playerScore = score;
    }
    
    [Fusion.Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetPlayerTime(float time)
    {
        this.playerTime = time;
    }
    
    [Fusion.Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetPlayerId(int id)
    {
        this.playerId = id;
    }

    [Fusion.Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetPlayerReady(NetworkBool ready)
    {
        this.isReady = ready;
    }
    
    [Fusion.Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetPlayerData(int pos, float value)
    {
        this.data.Set(pos, value);
    }
}
