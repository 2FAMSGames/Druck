using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ColliderManager : MonoBehaviour
{
    private int puntos = 100;

    private void Start()
    {
        GameState.Instance.PlayerChangedData += changedData;
    }


    void OnTriggerEnter(Collider other)
    {
        Debug.Log("¡HAS LLEGADO A LA META!");
        GameState.GetMyPlayer().SetData(0, puntos);
    }

    private void changedData(int id, NetworkDictionary<int, float> data)
    {
        if (id != GameState.GetMyPlayer().playerId)
        {
            puntos -= 10;
        }
        
        //GameState.
    }
}


