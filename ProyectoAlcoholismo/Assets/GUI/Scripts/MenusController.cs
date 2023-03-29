using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netcode.Transports.PhotonRealtime;

public class MenusController : MonoBehaviour
{
    public GameState _state;
    
    [Header("Menus")]
    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private GameObject roomCreateMenu;
    [SerializeField]
    private GameObject roomJoinMenu;
    [SerializeField]
    private GameObject settingsMenu;
    [SerializeField]
    private GameObject roomMenu;
    [SerializeField]
    private GameObject intro;
    [SerializeField]
    private GameObject helpMenu;

    float delayTime = 5f;
    public Camera newCamera; // La nueva cámara a la que queremos cambiar para que el fondo de atras sea verde

    void OnEnable()
    {
        Intro();
    }
    void Start()
    {
        Invoke("EnablePanelAfterDelay", delayTime);
    }

    void EnablePanelAfterDelay()
    {
        GoToMainMenu();
        ChangeToNewCamera();
    }

    public void Intro()
    {
        intro.SetActive(true);
        mainMenu.SetActive(false);
        roomCreateMenu.SetActive(false);
        roomJoinMenu.SetActive(false);
        settingsMenu.SetActive(false);
        helpMenu.SetActive(false);
        roomMenu.SetActive(false);


    }
    public void GoToMainMenu()
    {
        intro.SetActive(false);
        mainMenu.SetActive(true);
        roomCreateMenu.SetActive(false);
        roomJoinMenu.SetActive(false);
        settingsMenu.SetActive(false);
        helpMenu.SetActive(false);
        roomMenu.SetActive(false);
    }

    public void GoToRoomCreateMenu()
    {
        intro.SetActive(false);
        mainMenu.SetActive(false);
        roomCreateMenu.SetActive(true);
        roomJoinMenu.SetActive(false);
        settingsMenu.SetActive(false);
        helpMenu.SetActive(false);
        roomMenu.SetActive(false);
    }

    public void GoToRoomJoinMenu()
    {
        intro.SetActive(false);
        mainMenu.SetActive(false);
        roomCreateMenu.SetActive(false);
        roomJoinMenu.SetActive(true);
        settingsMenu.SetActive(false);
        helpMenu.SetActive(false);
        roomMenu.SetActive(false);
    }

    public void GoToSettingsMenu()
    {
        intro.SetActive(false);
        mainMenu.SetActive(false);
        roomCreateMenu.SetActive(false);
        roomJoinMenu.SetActive(false);
        settingsMenu.SetActive(true);
        helpMenu.SetActive(false);
        roomMenu.SetActive(false);
    }

    public void GoToRoomMenu()
    {
        intro.SetActive(false);
        mainMenu.SetActive(false);
        roomCreateMenu.SetActive(false);
        roomJoinMenu.SetActive(false);
        settingsMenu.SetActive(false);
        helpMenu.SetActive(false);
        roomMenu.SetActive(true);
    }

    public void ChangeToNewCamera()
    {
        // Desactivamos la cámara actual
        Camera.main.gameObject.SetActive(false);

        // Activamos la nueva cámara
        newCamera.gameObject.SetActive(true);
    }

    public void GoToHelpMenu()
    {
        intro.SetActive(false);
        mainMenu.SetActive(false);
        roomCreateMenu.SetActive(false);
        roomJoinMenu.SetActive(false);
        settingsMenu.SetActive(false);
        helpMenu.SetActive(true);
        roomMenu.SetActive(false);

    }
    public void ChangeNetworkRoomName(string newName)
    {
        //photonManager.RoomName = newName;
    }

    public void ChangeNetworkPlayerName(string newName)
    {
        //Todo: Setear donde corresponda el player name para que sea visible por otros jugadores
    }

    
}
