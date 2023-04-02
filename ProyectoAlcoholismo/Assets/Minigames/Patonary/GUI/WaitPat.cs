using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UIElements;

public class WaitPat : MonoBehaviour
{
    [SerializeField]
    private GameObject menusObject;
    private PatonaryMenuController menusController;

    public bool waitngEnded = false;
    private int WaitngScreen;

    void OnEnable()
    {
        menusController = menusObject.GetComponent<PatonaryMenuController>();
        //if everyone´s pic/guess/vote is ready then
        WaitngScreen = PlayerPrefs.GetInt("WaitngScreen");

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

            waitngEnded = true;
        }
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
            
            // TODO: no debería jugar otro? sino en los rankings sólo 1 va a tener puntuación.
            menusController.GoToRankings();
        }
    }
}
