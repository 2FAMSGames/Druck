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
    private Button readyButton;
    private ListView playerList;
    private Label warningLabel;

    private readonly string WAITSTR = "Esperando...";
    private readonly string STARTSTR = "Empezar"; 

    void OnEnable()
    {
        menusController = menusObject.GetComponent<MenusController>();

        doc = GetComponent<UIDocument>();

        goBackButton = doc.rootVisualElement.Q<Button>("GoBackButton");
        goBackButton.clicked += GoBackButtonOnClicked;

        readyButton = doc.rootVisualElement.Q<Button>("ReadyButton");
        readyButton.clicked += ReadyButtonOnClicked;
        readyButton.text = STARTSTR;
        
        warningLabel = doc.rootVisualElement.Q<Label>("TextLabel");
        warningLabel.text = "Espera a que todos los jugadores estén conectados a la sala para empezar.";

        playerList = doc.rootVisualElement.Q<ListView>("PlayersList");
    }

    private void GoBackButtonOnClicked()
    {
        GameState.Instance.Disconnect();
        readyButton.SetEnabled(true);
        readyButton.text = STARTSTR;
        playerList.Clear();
        
        menusController.GoToMainMenu();
    }

    private void ReadyButtonOnClicked()
    {
        GameState.GetMyPlayer().SetReady(true);
        readyButton.text = WAITSTR;
        readyButton.SetEnabled(false);
        
        // Necesario porque el server no recibe sus propios cambios.
        if (GameState.isServer)
        {
            GameState.Instance.PlayerHasChangedReady(GameState.GetMyPlayer().playerId, GameState.GetMyPlayer().isReady);
        }
    }

    private void FixedUpdate()
    {
        List<string> players = new List<string>();
        if (PlayerRegistry.Instance != null && PlayerRegistry.Instance.ObjectByRef.Count > 0)
        {
            foreach(var pl in PlayerRegistry.Instance.ObjectByRef)
            {
                players.Add(pl.Value.playerName);
            }
            
            playerList.itemsSource = players;
        }
        
        readyButton.SetEnabled(players.Count > 0 && readyButton.text != WAITSTR);

        if (GameState.Instance != null && GameState.Instance.Runner != null && GameState.Instance.Runner.SessionInfo.IsValid)
        {
            warningLabel.text = "Espera a que todos los jugadores estén conectados a la sala \""
                                + GameState.Instance.Runner.SessionInfo.Name + "\" para empezar.";
        }
    }
}
