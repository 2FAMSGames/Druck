using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ColliderManager : MonoBehaviour
{
    private int score = 100;
    public float tiempoDeJuego = 120; //Si en 2 minutos no has terminado termina tu juego
    VisualElement root;
    private UIDocument doc;


    void Start()
    {
        //GameState.Instance.PlayerChangedData += OnPlayerDataChanged;
        doc = GetComponent<UIDocument>();

    }

    private void Update()
    {

        tiempoDeJuego -= Time.deltaTime;
        if (tiempoDeJuego < 0)
        {
            Debug.Log("�HAS TARDADO MUCHO!");
            GameState.GetMyPlayer().SetData(0, -1);
            Ranking();
        }
        doc.rootVisualElement.Q<Label>("Tiempo").text = Mathf.Ceil(tiempoDeJuego).ToString();

    }

    private void OnPlayerDataChanged(int id, NetworkDictionary<int, float> data)
    {
        // Otro jugador ha llegado antes que t�
        score -= 10;
    }


    void OnTriggerEnter(Collider other)
    {
        Debug.Log("�HAS LLEGADO A LA META!");
        GameState.Instance.PlayerChangedData += OnPlayerDataChanged;
        GameState.GetMyPlayer().SetData(0, score);
        Ranking();
    }
    private void Ranking()
    {

        StartCoroutine(Utils.GameUtils.GoToRankings());
    }
}
