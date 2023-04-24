using System;
using System.Linq;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;


public class RankingMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject rankingJuego;
    [SerializeField]
    private GameObject castigos;
    [SerializeField]
    private GameObject rankingFinal;

    public string gameName;
    public int winnerIdx = -1;
    
    public readonly string WAITSTR = "Esperando...";
    public readonly string STARTSTR = "Continuar!"; // quizá "Siguiente!"
    public readonly string ENDSTR = "Terminar!"; // quizá 2 botones con "Otra ronda!"
    
    private int currentBarrier = 1;

    [SerializeField] private SpriteRenderer fondo;

    public void OnEnable()
    {
        var color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        var max = Math.Max(color.r, Math.Max(color.g, color.b));
        color.r += max;
        color.g += max;
        color.b += max;
        
        currentBarrier = 1;
        gameName = PlayerRegistry.Instance.CurrentScene;
        fondo.color = color;
        
        rankingJuego.SetActive(true);
        castigos.SetActive(false);
        rankingFinal.SetActive(false);
    }

    public void ToCastigos()
    {
        rankingJuego.SetActive(false);
        castigos.SetActive(true);
        rankingFinal.SetActive(false);
    }

    public void ToRankingFinal()
    {
        rankingJuego.SetActive(false);
        castigos.SetActive(false);
        rankingFinal.SetActive(true);
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
                currentBarrier = 2;
                ToCastigos();
                break;
            case 2:
                currentBarrier = 3;
                ToRankingFinal();
                break;
            default:
                break;
        }
    }
}
