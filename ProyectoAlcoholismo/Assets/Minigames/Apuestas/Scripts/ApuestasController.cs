using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

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

        //retoScreen.GetComponent<RetoController>();
        //esperaScreen.GetComponent<EsperaController>();
        GoTo("reto");
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
