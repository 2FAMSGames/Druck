using System;
using System.Linq;
using Fusion;
using UnityEngine;

public class WaitBarriers: MonoBehaviour
{
    [SerializeField]
    private RankingMenu rootMenu;
    
    private int currentBarrier = 1;
    
    public void OnEnable()
    {
        GameState.Instance.PlayerChangedData += OnPlayerChangedData;
    }

    private void OnPlayerChangedData(int id, NetworkDictionary<int, float> data)
    {
        CheckBarrier();
    }

    public void IAmInBarrier()
    {
        Debug.Log(GameState.GetMyPlayer().playerId + " is at barrier " + currentBarrier);
        // 5,6 y 7 son las barreras, no usar para otra cosa.
        GameState.GetMyPlayer().SetData(currentBarrier + 4, 1);
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
                ++currentBarrier;
                rootMenu.ToCastigos();
                break;
            case 2:
                ++currentBarrier;
                rootMenu.ToRankingFinal();
                break;
            default:
                break;
        }
    }
       
}
