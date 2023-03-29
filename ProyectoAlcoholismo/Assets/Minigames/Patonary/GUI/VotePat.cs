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
    private Button YesVoteButton;
    private Button NoVoteButton;
    private Label GuessWord;

    public string Guess;

    private int VoteWaiting = 3;

    void OnEnable()
    {
        menusController = menusObject.GetComponent<PatonaryMenuController>();

        doc = GetComponent<UIDocument>();

        PlayerPrefs.SetInt("WaitngScreen", VoteWaiting);

        YesVoteButton = doc.rootVisualElement.Q<Button>("Yes");
        NoVoteButton = doc.rootVisualElement.Q<Button>("No");
        GuessWord = doc.rootVisualElement.Q<Label>("Guess");

        ///////////////////////////
        Guess = PlayerPrefs.GetString("Guess");
        GuessWord.text = Guess;
        ///////////////////////////

        YesVoteButton.clicked += YesVoteButtonOnClicked;
        NoVoteButton.clicked += NoVoteButtonOnClicked;
    }

    private void YesVoteButtonOnClicked()
    {
        Debug.Log("Yes button clicked");
    }

    private void NoVoteButtonOnClicked()
    {
        Debug.Log("No button clicked");
    }
}