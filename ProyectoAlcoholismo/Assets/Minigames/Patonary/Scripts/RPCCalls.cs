using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class RPCCalls : NetworkBehaviour
{
    [SerializeField]
    private GameObject waitScreen;

    private GameObject guessScreen;

    private WaitPat waitScript;

    public string m_texture = String.Empty;
    public string m_word = String.Empty;
    private List<int> availablePlayers = new List<int>();
    private int currentBarrier = 1;
    
    // Players to communicate, from is the player from whom we received the
    // texture, and the one we will send the word.
    // To is the chosen player we will send the texture and receive the word. 
    public int m_from = -1;
    public int m_to = -1;
    public bool sentTexture = false;
    private bool sentWord = false;
        
    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void Rpc_SendTextureTargeted([RpcTarget] PlayerRef toPlayer, string texture, int from)
    {
        Debug.Log("Received texture from " + from +  " texture length " + texture.Length);
        m_texture = texture;
        m_from = from;
    }
    
    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void Rpc_SendWordTargeted([RpcTarget] PlayerRef toPlayer, string word, int from)
    {
        Debug.Log("Received word from " + from +  " word is " + word);
        m_word = word;
        Debug.Assert(from == m_to);
    }

    public void SendWord(string word)
    {
        if (m_from == -1 || sentWord)
            return;

        var myId = GameState.GetMyPlayer().playerId;
        Debug.Log(myId  + " sends word to " + m_from);
        var player = PlayerRegistry.Instance.ObjectByRef.Where(p => p.Value.playerId == m_from).ToList();
        if (player.Count == 1)
        {
            sentWord = true;
            Rpc_SendWordTargeted(player.First().Key, word, myId);
            return;
        }
        
        Utils.GameUtils.Log("Invalid player in SendWord");
    }

    public void SendTexture()
    {
        if (sentTexture) return;
        
        var myPlayer = GameState.GetMyPlayer();
        var myId = myPlayer.playerId;
        if (m_to == -1)
        {
            m_to = ChooseRandomAvailablePlayer();
            myPlayer.SetData(2, m_to + 1); // Signal that i've chosen another player.
            Debug.Log(myId  + " will send texture to " + m_to);
        }
        
        var imageString = PlayerPrefs.GetString("TransferredImage");
        if (imageString.Length == 0)
            return;
        
        var player = PlayerRegistry.Instance.ObjectByRef.Where(p => p.Value.playerId == m_to).ToList();
        if (player.Count == 1)
        {
            sentTexture = true;
            Debug.Log(myId + "sends texture to " + player.First().Key);
            Rpc_SendTextureTargeted(player.First().Key, imageString, myId);
            return;
        }
        
        Utils.GameUtils.Log("Invalid player in SendTexture");
    }

    /** \brief Chooses a random player available to send the texture returns id.
     *  
     */
    public int ChooseRandomAvailablePlayer()
    {
        var myId = GameState.GetMyPlayer().playerId;
        var availableIds = availablePlayers.Where(v  => v != myId).ToList();

        if (availableIds.Count == 0)
        {
            // Si no hay "disponibles" significa que te toca el host = 15
            return 15; 
        }
        
        var idx = Random.Range(0, availableIds.Count);
        Debug.Log("escogido " + availableIds[idx]);
        return availableIds[idx];
    }

    public void ResetAvailablePlayersData()
    {
        availablePlayers.Clear();
        
        foreach(var (key, player) in PlayerRegistry.Instance.ObjectByRef)
        {
            availablePlayers.Add(player.playerId);
        }
    }

    public void OnEnable()
    {
        ResetAvailablePlayersData();
        Debug.Assert(availablePlayers.Count == GameState.CountPlayers);

        waitScript = waitScreen.GetComponent<WaitPat>();
        
        GameState.Instance.PlayerChangedData += OnPlayerChangedData;
    }

    /** \brief Maneja las transmisiones por parte de otros jugadores
     * [2] es el id de quien envías la textura
     * [5] barrera WaitPat Draw
     * [6] barrera WaitPat Guess
     * [7] barrera WaitPat Vote
     * 
     */
    private void OnPlayerChangedData(int id, NetworkDictionary<int, float> data)
    {
        // data[2] es el playerId+1 del escogido "por el otro" que es id
        if (data[2] != 0 && m_to == -1) // still not sent.
        {
            var other = (int)data[2] - 1; // escogido aleatoriamente por el otro.
            Debug.Log(id + " escogió a " + other); 
            availablePlayers.Remove(id);
            var myId = GameState.GetMyPlayer().playerId;
            if (other == myId && !GameState.isServer)
            {
                // El host envía la textura el primero sin necesitar que otro "le señale".
                availablePlayers.Remove(myId);
                SendTexture();
            }

            // 'other' será marcado como ocupado cuando envíe su selección.
            return;
        }

        CheckBarrier(); 
    }

    public void IAmInBarrier(int barrierNum)
    {
        currentBarrier = barrierNum;
        Debug.Log(GameState.GetMyPlayer().playerId + " is at barrier " + barrierNum);
        // 5,6 y 7 son las barreras, no usar para otra cosa.
        GameState.GetMyPlayer().SetData(barrierNum + 4, 1);
    }

    public void CheckBarrier()
    {
        Debug.Log(GameState.GetMyPlayer().playerId + " checks barrier " + currentBarrier);
        var players = PlayerRegistry.Instance.ObjectByRef;
        var inBarrier = players.Where(p  => p.Value.data[currentBarrier + 4] == 1).ToList();
        Debug.Log("barrier " + currentBarrier + " has " + inBarrier.Count);

        if (inBarrier.Count != GameState.CountPlayers) return;
        
        switch (currentBarrier)
        {
            case 1:
                if (m_from != -1 && m_to == -1 && !GameState.isServer && !sentTexture)
                {
                    SendTexture();
                    Debug.Log("envía textura a " + m_to);
                }

                if (m_from != -1 && m_to != -1)
                {
                    waitScript.waitngEnded = true;
                    ++currentBarrier;
                }
                break;
            case 2:
                var guessWord = PlayerPrefs.GetString("Guess");
                if (guessWord != String.Empty && !sentWord)
                {
                    SendWord(guessWord);
                    Debug.Log("envía palabra a " + m_from);
                }

                if (sentWord && m_word != String.Empty)
                {
                    waitScript.waitngEnded = true;
                    ++currentBarrier;
                }
                break;
            case 3:
                waitScript.waitngEnded = true;
                break;
        }
        
        if(waitScript.waitngEnded)
            Debug.Log(GameState.GetMyPlayer().playerId + " lifts barrier " + (currentBarrier-1));
        else
            Debug.Log(GameState.GetMyPlayer().playerId + " stays in barrier " + currentBarrier);
    }

}
