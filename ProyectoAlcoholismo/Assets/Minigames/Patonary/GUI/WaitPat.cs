using System;
using UnityEngine;

public class WaitPat : MonoBehaviour
{
    [SerializeField]
    private GameObject menusObject;
    private PatonaryMenuController menusController;
    private RPCCalls rpcCalls;

    public bool waitngEnded;
    private int WaitngScreen;
    
    void OnEnable()
    {
        menusController = menusObject.GetComponent<PatonaryMenuController>();
        rpcCalls = menusObject.GetComponent<RPCCalls>();
        
        //if everyone´s pic/guess/vote is ready then
        waitngEnded = false;
        WaitngScreen = PlayerPrefs.GetInt("WaitngScreen");
        
        rpcCalls.IAmInBarrier(WaitngScreen);
    }

    private void Update()
    {
        if (WaitngScreen == 1 && GameState.isServer && rpcCalls.m_to == -1)
        {
            rpcCalls.SendTexture();
        }

        rpcCalls.CheckBarrier();

        if (waitngEnded)
        {
            WaitingEnded();
        }
    }

    private void WaitingEnded()
    {
        Debug.Log("Waiting ended");
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
            menusController.GoToRankings();
        }
    }
}
