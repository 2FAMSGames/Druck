using UnityEngine;
using UnityEngine.UIElements;

public class RoomCreateMenuController : MonoBehaviour {

    [SerializeField]
    private GameObject menusObject;
    private MenusController menusController;

    private UIDocument doc;
    private Button goBackButton;
    private Button createRoomButton;

    private TextField playerNameField;
    private TextField roomNameField;

    void OnEnable()
    {
        menusController = menusObject.GetComponent<MenusController>();

        doc = GetComponent<UIDocument>();

        goBackButton = doc.rootVisualElement.Q<Button>("GoBackButton");
        goBackButton.clicked += GoBackButtonOnClicked;

        createRoomButton = doc.rootVisualElement.Q<Button>("CreateButton");
        createRoomButton.clicked += CreateRoomButtonOnClicked;

        roomNameField = doc.rootVisualElement.Q<TextField>("RoomName");
        roomNameField.RegisterValueChangedCallback(OnEnterRoomName);

        playerNameField = doc.rootVisualElement.Q<TextField>("PlayerName");
        playerNameField.RegisterValueChangedCallback(OnEnterRoomName);
    }

    private void GoBackButtonOnClicked()
    {
        menusController.GoToMainMenu();
    }

    private void CreateRoomButtonOnClicked()
    {
        if (string.IsNullOrEmpty(playerNameField.text))
        {
            playerNameField.labelElement.style.color = Color.red;
            return;
        }
        playerNameField.labelElement.style.color = Color.black;

        if (string.IsNullOrEmpty(roomNameField.text))
        {
            roomNameField.labelElement.style.color = Color.red;
            return;
        }
        roomNameField.labelElement.style.color = Color.black;
        
        GameState.Instance.myPlayerName = playerNameField.text;
        GameState.Instance.CreateRoom(roomNameField.text);
        
        // TODO
        menusController.GoToRoomMenu();
    }

    private void OnEnterRoomName(ChangeEvent<string> changeEvent)
    {
        menusController.ChangeNetworkRoomName(changeEvent.newValue);
    }

    private void OnEnterPlayerName(ChangeEvent<string> changeEvent)
    {
        menusController.ChangeNetworkPlayerName(changeEvent.newValue);
    }
}
