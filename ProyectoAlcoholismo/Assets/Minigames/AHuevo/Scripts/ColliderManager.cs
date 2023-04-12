using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ColliderManager : MonoBehaviour
{
    private static int score = 100;
    public float tiempoDeJuego = 120; //Si en 2 minutos no has terminado termina tu juego

    void Start()
    {
        //GameState.Instance.PlayerChangedData += OnPlayerDataChanged;
    }

    private void Update()
    {
        tiempoDeJuego -= Time.deltaTime;
        if (tiempoDeJuego < 0)
        {
            Debug.Log("¡HAS TARDADO MUCHO!");
            GameState.GetMyPlayer().SetData(0, -1);
            Ranking();
        }
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
        Ranking();
    }
    private void Ranking()
    {

        StartCoroutine(Utils.GameUtils.GoToRankings());
    }
}
