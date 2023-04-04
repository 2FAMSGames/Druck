using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Unity.Profiling;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public class ApuestasController : MonoBehaviour
{
    [SerializeField]
    private GameObject apuestasObject;
    private ApuestasController apuestasController;
    public string prize = "0";
    public string yourPlayer = "Ana";
    public string CurrentPlayer = "Ana";
    public string challengedPlayer = "";
    public string challenge = "";
    public string winner = "";

    private bool yourTurn;
    private int rondas = 0;
    private readonly int NUMRONDAS = 3;

    public bool Votando = false;

    [SerializeField]
    private GameObject retoScreen;
    [SerializeField]
    private GameObject esperaScreen;
    [SerializeField]
    private GameObject listaScreen;
    [SerializeField]
    private GameObject realizandoScreen;
    [SerializeField]
    private GameObject ganadorScreen;
    [SerializeField]
    private GameObject inicioScreen;

    void OnEnable()
    {
        yourPlayer = GameState.GetMyPlayer().playerName;
        GameState.Instance.PlayerChangedData += OnPlayerChangedData;

        ChooseRandomPlayer();
    }

    public void ChooseRandomPlayer()
    {
        if (rondas < NUMRONDAS)
        {
            ++rondas;
            if (GameState.isServer)
            {
                // TODO: comprobar que no sea muchas veces el mismo??
                var id = Random.Range(0, GameState.CountPlayers);
                GameState.GetMyPlayer().SetData(0, id);

                ChooseScreen(id);
            }
        }
        else
        {
            StartCoroutine(Utils.GameUtils.GoToRankings());
        }
    }

    private void ChooseScreen(int id)
    {
        CurrentPlayer = GameState.GetPlayer(id).playerName;
        if (id == GameState.GetMyPlayer().playerId)
        {
            GoTo("reto");        
        }
        else 
        {
            GoTo("espera");
        }
    }
    
    public void OnPlayerChangedData(int id, NetworkDictionary<int, float> data)
    {
        if (Votando) return;
        // TODO: protocolo de valores con sentido para el juego
        if (id == 15) // host es GameState.Instance.Runner.SessionInfo.MaxPlayers - 1
        {
            ChooseScreen((int)GameState.GetPlayer(15).data[0]);
        }
    }

    public void GoTo(string screen)
    {
        retoScreen.SetActive(screen == "reto");
        esperaScreen.SetActive(screen == "espera");
        listaScreen.SetActive(screen == "lista");
        realizandoScreen.SetActive(screen == "realizando");
        ganadorScreen.SetActive(screen == "ganador");
        inicioScreen.SetActive(screen == "inicio");
    }
}
