using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
        //if everyone´s pic/guess/vote is ready then
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
            //menusController.GoToScore();
        }
    }
}
