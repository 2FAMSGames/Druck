using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TaskPat : MonoBehaviour
{
    [SerializeField]
    private GameObject menusObject;
    private PatonaryMenuController menusController;
    [SerializeField]
    private GameObject wordObject;
    private WordsRand word;

    private UIDocument doc;
    private Button empezarButton;
    private Label randomWord;

    void OnEnable()
    {
        menusController = menusObject.GetComponent<PatonaryMenuController>();

        doc = GetComponent<UIDocument>();

        //button
        empezarButton = doc.rootVisualElement.Q<Button>("EmpezarButton");
        empezarButton.clicked += EmpezarButtonOnClicked;

    }

    void Start()
    {//text
        doc = GetComponent<UIDocument>();
        randomWord = doc.rootVisualElement.Q<Label>("RandomWord");
        word = wordObject.GetComponent<WordsRand>();
        randomWord.text = word.wordText.text;
    }

    private void EmpezarButtonOnClicked()
    {
        Debug.Log("Empezar button clicked");
        menusController.GoToCanvas();
    }
}
