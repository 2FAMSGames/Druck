using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GuessPat : MonoBehaviour
{
    [SerializeField]
    private GameObject menusObject;
    private PatonaryMenuController menusController;
    private RPCCalls rpcCalls;

    private UIDocument doc;
    private Button guessButton;
    private TextField guess;
    private VisualElement imageHolder;

    public string guessWord;

    private int GuessWaitng = 2;

    void OnEnable()
    {
        PlayerPrefs.SetInt("WaitngScreen", GuessWaitng);

        menusController = menusObject.GetComponent<PatonaryMenuController>();
        rpcCalls = menusObject.GetComponent<RPCCalls>();

        doc = GetComponent<UIDocument>();

        guess = doc.rootVisualElement.Q<TextField>("GuessField");

        imageHolder = doc.rootVisualElement.Q<VisualElement>("Paint");
        
        guessButton = doc.rootVisualElement.Q<Button>("GuessButton");
        guessButton.clicked += GuessButtonOnClicked;
    }

    private void Start()
    {
        // Tenemos el id del que nos ha enviado la imagen, podríamos poner
        // el nombre en pantalla.
        // id = rpcCalls.m_from
        
        // Retrieve the string from PlayerPrefs
        string imageString = rpcCalls.m_texture;

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
        
        menusController.GoToWait();
    }
}