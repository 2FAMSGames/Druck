using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ColliderManager : MonoBehaviour
{
    private static int score = 100;

    void Start()
    {
        GameState.Instance.PlayerChangedData += OnPlayerDataChanged;
    }

    private void OnPlayerDataChanged(int id, NetworkDictionary<int, float> data)
    {
        // Otro jugador ha llegado antes que tú
        score -= 10;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("¡HAS LLEGADO A LA META!");
        GameState.GetMyPlayer().SetData(0, score);
        GameState.Instance.PlayerChangedData -= OnPlayerDataChanged;

        StartCoroutine(Utils.GameUtils.GoToRankings());
    }
}
