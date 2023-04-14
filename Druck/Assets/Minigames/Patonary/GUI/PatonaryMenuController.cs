using UnityEngine;

public class PatonaryMenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject MenuPat;
    [SerializeField]
    private GameObject TaskPat;
    [SerializeField]
    private GameObject CanvasPat;
    [SerializeField]
    private GameObject GuessPat;
    [SerializeField]
    private GameObject VotePat;
    [SerializeField]
    private GameObject WaitPat;

    void OnEnable()
    {
        GameState.GetMyPlayer().ResetData();
        GameState.GetMyPlayer().SetReady(false);
        
        GoToMenu();
        PlayerPrefs.DeleteAll();
    }

    public void GoToMenu()
    {
        MenuPat.SetActive(true);
        TaskPat.SetActive(false);
        CanvasPat.SetActive(false);
        GuessPat.SetActive(false);
        VotePat.SetActive(false);
        WaitPat.SetActive(false);
    }

    public void GoToTask()
    {
        MenuPat.SetActive(false);
        TaskPat.SetActive(true);
        CanvasPat.SetActive(false);
        GuessPat.SetActive(false);
        VotePat.SetActive(false);
        WaitPat.SetActive(false);
    }

    public void GoToCanvas()
    {
        MenuPat.SetActive(false);
        TaskPat.SetActive(false);
        CanvasPat.SetActive(true);
        GuessPat.SetActive(false);
        VotePat.SetActive(false);
        WaitPat.SetActive(false);
    }

    public void GoToGuess()
    {
        MenuPat.SetActive(false);
        TaskPat.SetActive(false);
        CanvasPat.SetActive(false);
        GuessPat.SetActive(true);
        VotePat.SetActive(false);
        WaitPat.SetActive(false);
    }
    public void GoToVote()
    {
        MenuPat.SetActive(false);
        TaskPat.SetActive(false);
        CanvasPat.SetActive(false);
        GuessPat.SetActive(false);
        VotePat.SetActive(true);
        WaitPat.SetActive(false);
    }
    public void GoToWait()
    {
        MenuPat.SetActive(false);
        TaskPat.SetActive(false);
        CanvasPat.SetActive(false);
        GuessPat.SetActive(false);
        VotePat.SetActive(false);
        WaitPat.SetActive(true);
    }

    public void GoToRankings()
    {
        StartCoroutine(Utils.GameUtils.GoToRankings());
    }
}
