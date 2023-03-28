using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VotePat : MonoBehaviour
{
    [SerializeField]
    private GameObject menusObject;
    private PatonaryMenuController menusController;

    private UIDocument doc;
    private Button VoteButton1;
    private Button VoteButton2;
    private Button VoteButton3;
    private Button VoteButton4;

    public string Guess;

    void OnEnable()
    {
        menusController = menusObject.GetComponent<PatonaryMenuController>();

        doc = GetComponent<UIDocument>();

        VoteButton1 = doc.rootVisualElement.Q<Button>("Guess1");
        VoteButton2 = doc.rootVisualElement.Q<Button>("Guess2");
        VoteButton3 = doc.rootVisualElement.Q<Button>("Guess3");
        VoteButton4 = doc.rootVisualElement.Q<Button>("Guess4");

        ///////////////////////////
        Guess = PlayerPrefs.GetString("Guess");
        VoteButton1.text = Guess;
        VoteButton2.text = Guess;
        VoteButton3.text = Guess;
        VoteButton4.text = Guess;
        ///////////////////////////

        VoteButton1.clicked += Vote1ButtonOnClicked;
        VoteButton2.clicked += Vote2ButtonOnClicked;
        VoteButton3.clicked += Vote3ButtonOnClicked;
        VoteButton4.clicked += Vote4ButtonOnClicked;
    }

    private void Vote1ButtonOnClicked()
    {
        Debug.Log("Vote1 button clicked");
    }

    private void Vote2ButtonOnClicked()
    {
        Debug.Log("Vote2 button clicked");
    }

    private void Vote3ButtonOnClicked()
    {
        Debug.Log("Vote3 button clicked");
    }

    private void Vote4ButtonOnClicked()
    {
        Debug.Log("Vote4 button clicked");
    }

}