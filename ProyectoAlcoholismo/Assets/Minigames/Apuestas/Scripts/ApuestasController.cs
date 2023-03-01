using UnityEngine;
using UnityEngine.UIElements;

public class ApuestasController : MonoBehaviour
{

    [SerializeField]
    private GameObject apuestasObject;
    private ApuestasController apuestasController;

    private UIDocument doc;
    //private Button createRoomButton;
    //private Button joinRoomButton;
    //private Button optionsButton;
    //private Button exitButton;

    void OnEnable()
    {
        apuestasController = apuestasObject.GetComponent<ApuestasController>();

        doc = GetComponent<UIDocument>();

        //createRoomButton = doc.rootVisualElement.Q<Button>("CreateRoomButton");
        //createRoomButton.clicked += CreateRoomButtonOnClicked;

        //joinRoomButton = doc.rootVisualElement.Q<Button>("JoinRoomButton");
        //joinRoomButton.clicked += JoinRoomButtonOnClicked;

        //optionsButton = doc.rootVisualElement.Q<Button>("OptionsButton");
        //optionsButton.clicked += OptionsButtonOnClicked;

        //exitButton = doc.rootVisualElement.Q<Button>("ExitButton");
        //exitButton.clicked += ExitButtonOnClicked;
    }

    //private void CreateRoomButtonOnClicked()
    //{
    //    Debug.Log("Create button clicked");
    //    menusController.GoToRoomCreateMenu();
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