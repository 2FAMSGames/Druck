using UnityEngine;
using UnityEngine.UIElements;

public class SimonInfo : MonoBehaviour
{ 
    [SerializeField]
    private GameObject menusObject;
    private SimonSceneController menusController;

    private UIDocument doc;
    private Button Boton;

    void OnEnable()
    {
        menusController = menusObject.GetComponent<SimonSceneController>();
        doc = GetComponent<UIDocument>();

        Boton = doc.rootVisualElement.Q<Button>("Boton");
        Boton.clicked += JugarButtonOnClicked;
    }

    private void JugarButtonOnClicked()
    {
        GameState.GetMyPlayer().ResetData();
        GameState.GetMyPlayer().SetReady(false);

        menusController.Jugar();
    }
}