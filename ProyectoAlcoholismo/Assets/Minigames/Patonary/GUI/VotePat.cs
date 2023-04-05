using Fusion;
using UnityEngine;
using UnityEngine.UIElements;

public class VotePat : MonoBehaviour
{
    [SerializeField]
    private GameObject menusObject;
    private PatonaryMenuController menusController;
    private RPCCalls rpcCalls;

    private UIDocument doc;
    private Button YesVoteButton;
    private Button NoVoteButton;
    private Label GuessWord;

    public string Guess;

    private int VoteWaiting = 3;

    void OnEnable()
    {
        menusController = menusObject.GetComponent<PatonaryMenuController>();
        rpcCalls = menusObject.GetComponent<RPCCalls>();

        doc = GetComponent<UIDocument>();

        PlayerPrefs.SetInt("WaitngScreen", VoteWaiting);

        YesVoteButton = doc.rootVisualElement.Q<Button>("Yes");
        NoVoteButton = doc.rootVisualElement.Q<Button>("No");
        GuessWord = doc.rootVisualElement.Q<Label>("Guess");

        ///////////////////////////
        Guess = rpcCalls.m_word;
        GuessWord.text = Guess;
        ///////////////////////////

        YesVoteButton.clicked += YesVoteButtonOnClicked;
        NoVoteButton.clicked += NoVoteButtonOnClicked;
    }
    
    /** \brief Resultados Patonary:
     * [0] -1 NO, 1 SI acertado
     * [1] id de quien tenía que acertar.
     */
    private void YesVoteButtonOnClicked()
    {
        Debug.Log("Yes button clicked");
        GameState.GetMyPlayer().SetData(0, 1);
        GameState.GetMyPlayer().SetData(1, rpcCalls.m_to);
        
        GoToWait();
    }

    private void NoVoteButtonOnClicked()
    {
        Debug.Log("No button clicked");
        GameState.GetMyPlayer().SetData(0, -1);
        GameState.GetMyPlayer().SetData(1, rpcCalls.m_to);
        
        GoToWait();
    }

    private void GoToWait()
    {
        PlayerPrefs.SetInt("WaitngScreen", VoteWaiting);

        menusController.GoToWait();
    }
}