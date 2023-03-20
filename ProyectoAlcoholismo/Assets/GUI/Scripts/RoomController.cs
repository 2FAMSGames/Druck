using System.Collections;
using System.Collections.Generic;
using Fusion;
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
    private Button readyButton;
    private ListView playerList;

    void OnEnable()
    {
        menusController = menusObject.GetComponent<MenusController>();

        doc = GetComponent<UIDocument>();

        goBackButton = doc.rootVisualElement.Q<Button>("GoBackButton");
        goBackButton.clicked += GoBackButtonOnClicked;

        readyButton = doc.rootVisualElement.Q<Button>("ReadyButton");
        readyButton.clicked += ReadyButtonOnClicked;

        GameState.Instance.PlayerChangedScore += changedScore;
        
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

    private void ReadyButtonOnClicked()
    {
        Debug.Log("Ready button clicked");
        var score = GameState.GetMyPlayer().playerScore;
        var ready = GameState.GetMyPlayer().isReady;
        GameState.Instance.ModifyScore(score - 5);
        GameState.Instance.ModifyReadyFlag(!ready);
        GameState.Instance.DebugPrint();
    }

    private void changedScore(int id, int score)
    {
        Debug.Assert(GameState.HasPlayer(id));
       
        var playerData = GameState.GetPlayer(id);
        Debug.Log($"player {playerData.playerName} changed score to {score}");
        var allready = PlayerRegistry.AllReady;
    }
}
