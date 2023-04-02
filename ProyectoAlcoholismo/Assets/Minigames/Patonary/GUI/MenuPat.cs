using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuPat : MonoBehaviour
{
    [SerializeField]
    private GameObject menusObject;
    private PatonaryMenuController menusController;

    private UIDocument doc;
    private Button jugarButton;

    void OnEnable()
    {
        GameState.GetMyPlayer().ResetData();
        
        menusController = menusObject.GetComponent<PatonaryMenuController>();

        doc = GetComponent<UIDocument>();

        jugarButton = doc.rootVisualElement.Q<Button>("JugarButton");
        jugarButton.clicked += JugarButtonOnClicked;
    }

    private void JugarButtonOnClicked()
    {
        Debug.Log("Jugar button clicked");
        menusController.GoToTask();
    }
}
