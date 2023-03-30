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
        
        //retoScreen.GetComponent<RetoController>();
        //esperaScreen.GetComponent<EsperaController>();
//        GoTo("reto");
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
        //roomJoinMenu.SetActive(false);
        //settingsMenu.SetActive(false);
    }



    //private void CreateRoomButtonOnClicked()
    //{
    //    Debug.Log("Create button clicked");
    //    //menusController.GoToRoomCreateMenu();
    //}

    //private void JoinRoomButtonOnClicked()
    //{
    //    Debug.Log("Join button clicked");
    //    menusController.GoToRoomJoinMenu();
    //}

    //private void OptionsButtonOnClicked()
    //{
    //    Debug.Log("Options button clicked");
    //    menusController.GoToSettingsMenu();
    //}

    //private void ExitButtonOnClicked()
    //{
    //    Debug.Log("Exit button clicked");
    //    Application.Quit();
    //}
}
