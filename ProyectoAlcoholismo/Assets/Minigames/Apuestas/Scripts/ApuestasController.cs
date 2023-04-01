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
        GameState.Instance.PlayerChangedData += OnPlayerChangedData;
        
        if (GameState.isServer)
        {
            var id = Random.Range(0, GameState.CountPlayers - 1);
            GameState.GetMyPlayer().SetData(0, id);

            // El servidor no recibe "sus eventos"
            ChooseScreen(id);
        }
        //GoTo("inicio");
    }

    private void ChooseScreen(int id)
    {
        if (id == GameState.GetMyPlayer().playerId)
        {
            GoTo("reto");        
        }
        else 
        {
            GoTo("espera");
        }

        CurrentPlayer = GameState.GetPlayer(id).playerName;
    }
    
    private void OnPlayerChangedData(int id, NetworkDictionary<int, float> data)
    {
        // TODO: protocolo de valores con sentido para el juego
        if (id == 15) // GameState.Instance.Runner.SessionInfo.MaxPlayers - 1
        {
            // El servidor envia el dato
            ChooseScreen(id);
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
