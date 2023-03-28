using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class ApuestasController : MonoBehaviour
{

    [SerializeField]
    //private GameObject apuestasObject;
    private ApuestasController apuestasController;

    //private Label CurrentPlayer;


    [SerializeField]
    private GameObject retoScreen;
    [SerializeField]
    private GameObject esperaScreen;
    //[SerializeField]
    //private GameObject roomJoinMenu;
    //[SerializeField]
    //private GameObject settingsMenu;

    void OnEnable()
    {
        retoScreen.GetComponent<RetoController>();
        GoTo("reto");
    }

    public void GoTo(string screen)
    {
        retoScreen.SetActive(screen == "reto");
        esperaScreen.SetActive(screen == "espera");
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
