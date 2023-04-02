using System.Collections;
using System.Collections.Generic;
using Fusion;
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
        
        GameState.Instance.PlayerChangedData += OnPlayerChangedData;
    }
    
    private void OnPlayerChangedData(int arg1, NetworkDictionary<int, float> arg2)
    {
        bool allVoted = true;
        int yes = 0;
        int no = 0;
        foreach (var (key, player) in PlayerRegistry.Instance.ObjectByRef)
        {
            var vote = player.data[0];
            allVoted &= (vote != 0);
            if (vote != 0)
            {
                yes += vote == 1 ? 1 : 0;
                no += vote == -1 ? 1 : 0;
            }
        }

        if (allVoted)
        {
            if (yes >= no)
            {
                // TODO: asignar valor al ganador por el host, quién ha ganado y qué asignamos para
                // luego usarlo en PlayerRegistry.sortedScoresData??
                if (GameState.isServer)
                {
                    // id del ganador, score son los puntos.
                    //GameState.GetPlayer(id).SetData(5, score);
                }
            }
        }
    }

    private void YesVoteButtonOnClicked()
    {
        Debug.Log("Yes button clicked");
        GameState.GetMyPlayer().SetData(0, 1);
        
        // TODO: después de votar a donde vamos?
    }

    private void NoVoteButtonOnClicked()
    {
        Debug.Log("No button clicked");
        GameState.GetMyPlayer().SetData(0, -1);
        
        // TODO: después de votar a donde vamos?
    }
}