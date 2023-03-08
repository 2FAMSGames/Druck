using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomJoinMenuController : MonoBehaviour {

    [SerializeField]
    private GameObject menusObject;
    private MenusController menusController;

    private UIDocument doc;
    private Button goBackButton;
    private Button joinRoomButton;

    private TextField playerNameField;
    private TextField roomNameField;

    void OnEnable()
    {
        menusController = menusObject.GetComponent<MenusController>();

        doc = GetComponent<UIDocument>();

        goBackButton = doc.rootVisualElement.Q<Button>("GoBackButton");
        goBackButton.clicked += GoBackButtonOnClicked;

        joinRoomButton = doc.rootVisualElement.Q<Button>("JoinButton");
        joinRoomButton.clicked += JoinRoomButtonOnClicked;

        roomNameField = doc.rootVisualElement.Q<TextField>("RoomName");
        roomNameField.RegisterValueChangedCallback(OnEnterRoomName);

        playerNameField = doc.rootVisualElement.Q<TextField>("PlayerName");
        playerNameField.RegisterValueChangedCallback(OnEnterRoomName);
    }

    private void GoBackButtonOnClicked()
    {
        menusController.GoToMainMenu();
    }

    private void JoinRoomButtonOnClicked()
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

        GameState.Instance.JoinRoom(roomNameField.text);
        
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
