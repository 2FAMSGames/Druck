using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GuessPat : MonoBehaviour
{
    [SerializeField]
    private GameObject menusObject;
    private PatonaryMenuController menusController;

    private UIDocument doc;
    private Button guessButton;
    private TextField guess;
    private VisualElement imageHolder;

    public string guessWord;

    void OnEnable()
    {
        menusController = menusObject.GetComponent<PatonaryMenuController>();

        doc = GetComponent<UIDocument>();

        guess = doc.rootVisualElement.Q<TextField>("GuessField");

        imageHolder = doc.rootVisualElement.Q<VisualElement>("Paint");
        
        guessButton = doc.rootVisualElement.Q<Button>("GuessButton");
        guessButton.clicked += GuessButtonOnClicked;
    }

    private void Start()
    {
        // Retrieve the string from PlayerPrefs
        string imageString = PlayerPrefs.GetString("TransferredImage");

        // Convert the string back to a byte array
        byte[] imageData = System.Convert.FromBase64String(imageString);

        // Create a new texture from the byte array
        Texture2D transferredTexture = new Texture2D(1, 1);
        transferredTexture.LoadImage(imageData);

        imageHolder.style.backgroundImage = transferredTexture;
    }

    private void GuessButtonOnClicked()
    {
        Debug.Log("Guess button clicked");

        //guess
        guessWord = guess.text;
        PlayerPrefs.SetString("Guess", guessWord);
        Debug.Log(guessWord);
        
        //go to next menu
        menusController.GoToVote();
    }
}