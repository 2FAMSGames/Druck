using System;
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

    [SerializeField] private SpriteRenderer fondo;

    public void OnEnable()
    {
        var color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        var max = Math.Max(color.r, Math.Max(color.g, color.b));
        color.r += max;
        color.g += max;
        color.b += max;
        
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
}
