using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netcode.Transports.PhotonRealtime;
using WebSocketSharp;

public class MenusController : MonoBehaviour
{
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

    [SerializeField]
    private GameObject creditsMenu;

    [SerializeField] private SpriteRenderer fondo;
    
    float delayTime = 5f;
    public Camera newCamera; // La nueva c�mara a la que queremos cambiar para que el fondo de atras sea verde
    private Color bgColor;

    void OnEnable()
    {
        bgColor = fondo.color;
        fondo.color = Color.clear;
        
        if (!GameState.Instance.AlreadyPlayedIntro)
        {
            Intro();
        }
    }
    void Start()
    {
        if (GameState.Instance.AlreadyPlayedIntro) delayTime = 0;
        Invoke("EnablePanelAfterDelay", delayTime);
    }

    void EnablePanelAfterDelay()
    {
        GameState.Instance?.Disconnect();
        
        GoToMainMenu();
        ChangeToNewCamera();
        fondo.color = bgColor;
        fondo.enabled = true;
        Debug.Log(fondo.enabled);
        
        GameState.Instance.AlreadyPlayedIntro = true;
    }

    public void Intro()
    {
        GameState.Instance.PlayFanfare();
        
        intro.SetActive(true);
        mainMenu.SetActive(false);
        roomCreateMenu.SetActive(false);
        roomJoinMenu.SetActive(false);
        settingsMenu.SetActive(false);
        helpMenu.SetActive(false);
        roomMenu.SetActive(false);
        creditsMenu.SetActive(false);

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
        creditsMenu.SetActive(false);

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
        creditsMenu.SetActive(false);

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
        creditsMenu.SetActive(false);

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
        creditsMenu.SetActive(false);

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
        creditsMenu.SetActive(false);

    }

    public void ChangeToNewCamera()
    {
        // Desactivamos la c�mara actual
        Camera.main.gameObject.SetActive(false);

        // Activamos la nueva c�mara
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
        creditsMenu.SetActive(false);
    }
    public void GoToCreditsMenu()
    {
        intro.SetActive(false);
        mainMenu.SetActive(false);
        roomCreateMenu.SetActive(false);
        roomJoinMenu.SetActive(false);
        settingsMenu.SetActive(false);
        helpMenu.SetActive(false);
        roomMenu.SetActive(false);
        creditsMenu.SetActive(true);

    }
}