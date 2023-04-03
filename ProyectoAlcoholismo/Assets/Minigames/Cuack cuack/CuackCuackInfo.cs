using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CuackCuackInfo : MonoBehaviour
{
    [SerializeField]
    private GameObject juego;

    private UIDocument doc;
    private Button jugarButton;

    void OnEnable()
    {
        doc = GetComponent<UIDocument>();

        jugarButton = doc.rootVisualElement.Q<Button>("Jugar");
        jugarButton.clicked += JugarButtonOnClicked;
    }

    private void JugarButtonOnClicked()
    {
        juego.SetActive(true);
        this.gameObject.SetActive(false);
    }
}