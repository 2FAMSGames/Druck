using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomController : MonoBehaviour
{
    [SerializeField]
    private GameObject menusObject;
    private MenusController menusController;

    [SerializeField]
    VisualTreeAsset listEntryTemplate;

    private UIDocument doc;
    private Button goBackButton;
    private ListView playerList;

    void OnEnable()
    {
        menusController = menusObject.GetComponent<MenusController>();

        doc = GetComponent<UIDocument>();

        goBackButton = doc.rootVisualElement.Q<Button>("GoBackButton");
        goBackButton.clicked += GoBackButtonOnClicked;

        //playerList = doc.rootVisualElement.Q<ListView>("PlayerList");
        //var playerListController = new PlayerListController();
        //playerListController.InitPlayerList(listEntryTemplate, playerList);
    }

    private void GoBackButtonOnClicked()
    {
        Debug.Log("Go back button clicked");
        menusController.GoToMainMenu();
        //Desconectarse del host o eliminar la sala
    }

    //PlayerListController
}
