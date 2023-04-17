using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    private void Start()
    {
        GameState.Instance.DebugPrint();
    }

    [SerializeField]
    private GameObject menusObject;
    private MenusController menusController;

    private UIDocument doc;
    private Button createRoomButton;
    private Button joinRoomButton;
    private Button optionsButton;
    private Button exitButton;
    private Button helpButton;

    void OnEnable() {
        menusController = menusObject.GetComponent<MenusController>();

        doc = GetComponent<UIDocument>();

        createRoomButton = doc.rootVisualElement.Q<Button>("CreateRoomButton");
        createRoomButton.clicked += CreateRoomButtonOnClicked;

        joinRoomButton = doc.rootVisualElement.Q<Button>("JoinRoomButton");
        joinRoomButton.clicked += JoinRoomButtonOnClicked;

        optionsButton = doc.rootVisualElement.Q<Button>("OptionsButton");
        optionsButton.clicked += OptionsButtonOnClicked;

        helpButton = doc.rootVisualElement.Q<Button>("HelpButton");
        helpButton.clicked += HelpButtonOnClicked;
    }

    private void CreateRoomButtonOnClicked() {
        Debug.Log("Create button clicked");
        menusController.GoToRoomCreateMenu();
    }

    private void JoinRoomButtonOnClicked() {
        Debug.Log("Join button clicked");
        menusController.GoToRoomJoinMenu();
    }

    private void OptionsButtonOnClicked() {
        Debug.Log("Options button clicked");
        menusController.GoToSettingsMenu();
    }

    private void ExitButtonOnClicked() {
        Debug.Log("Exit button clicked");
        Application.Quit();
    }
    private void HelpButtonOnClicked()
    {
        Debug.Log("Help button clicked");
        menusController.GoToHelpMenu();
    }
}