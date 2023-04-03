using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LanzapatoInfo : MonoBehaviour
{
    [SerializeField]
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
        this.gameObject.SetActive(false);
    }
}
