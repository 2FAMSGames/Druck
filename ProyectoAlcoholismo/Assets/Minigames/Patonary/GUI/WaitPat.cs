using UnityEngine;

public class WaitPat : MonoBehaviour
{
    [SerializeField]
    private GameObject menusObject;
    private PatonaryMenuController menusController;

    public bool waitngEnded = true;
    private int WaitngScreen;

    void OnEnable()
    {
        menusController = menusObject.GetComponent<PatonaryMenuController>();
        //if everyone�s pic/guess/vote is ready then
        WaitngScreen = PlayerPrefs.GetInt("WaitngScreen");
    }

    private void Update()
    {
        if (waitngEnded)
        {
            WaitingEnded();
        }
    }

    private void WaitingEnded()
    {
        Debug.Log("Waiting ended");
        //menusController.WaitEnded();
        if (WaitngScreen == 1)
        {
            Debug.Log("Everyone have drawn");
            menusController.GoToGuess();
        }
        else if (WaitngScreen == 2)
        {
            Debug.Log("Everyone have guessed");
            menusController.GoToVote();
        }
        else if (WaitngScreen == 3)
        {
            Debug.Log("Everyone have voted");
            
            // TODO: no deber�a jugar otro? sino en los rankings s�lo 1 va a tener puntuaci�n.
            menusController.GoToRankings();
        }
    }
}
