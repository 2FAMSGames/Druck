using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenuController : MonoBehaviour {

    [SerializeField]
    private GameObject menusObject;
    private MenusController menusController;

    private UIDocument doc;
    private Button goBackButton;
    private Button salirButton;
    private Slider volumeSlider;
    private Button creditsButton;


    [SerializeField] private AudioClip cuak;

    void OnEnable()
    {
        menusController = menusObject.GetComponent<MenusController>();

        doc = GetComponent<UIDocument>();

        goBackButton = doc.rootVisualElement.Q<Button>("GoBackButton");
        goBackButton.clicked += GoBackButtonOnClicked;
        
        salirButton = doc.rootVisualElement.Q<Button>("Abandonar");
        salirButton.clicked += ExitButtonOnClicked;
        
        volumeSlider = doc.rootVisualElement.Q<Slider>("Volume");
        volumeSlider.value = GameState.Instance.audioSource.volume * 100;

        creditsButton = doc.rootVisualElement.Q<Button>("Credits");
        creditsButton.clicked += CreditsButtonOnClicked;

        volumeSlider.RegisterValueChangedCallback(v =>
        {
            SliderValueChanged(v.newValue);
        });

        GameState.Instance.audioSource.clip = cuak;


    }

    private void ExitButtonOnClicked()
    {
        Debug.Log("Salir de la aplicación");
        Application.Quit();
    }

    private void SliderValueChanged(float value)
    {
        GameState.Instance.audioSource.volume = value/100f;
        GameState.Instance.audioSource.Play();
    }

    private void GoBackButtonOnClicked()
    {
        Debug.Log("Go back button clicked");
        menusController.GoToMainMenu();
    }

    private void CreditsButtonOnClicked()
    {
        Debug.Log("Go to credits button clicked");
        menusController.GoToCreditsMenu();
    }


}
