using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AHuevoInfo : MonoBehaviour
{ 
    [SerializeField]
    private GameObject menusObject;
    private AHuevoSceneController menusController;

    private UIDocument doc;
    private Button jugarButton;

    void OnEnable()
    {
        menusController = menusObject.GetComponent<AHuevoSceneController>();

        doc = GetComponent<UIDocument>();

        jugarButton = doc.rootVisualElement.Q<Button>("JugarButton");
        jugarButton.clicked += JugarButtonOnClicked;
    }

    private void JugarButtonOnClicked()
    {
        GameState.GetMyPlayer().ResetData();
        GameState.GetMyPlayer().SetReady(false);
        
        Debug.Log("Jugar button clicked");
        menusController.GoToTask();
    }
}