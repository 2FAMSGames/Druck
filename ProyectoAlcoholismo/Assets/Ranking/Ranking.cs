using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Ranking : MonoBehaviour
{
    public List<string> jugadores;
    public VisualTreeAsset jugadorTemplate;

    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        VisualElement jugadoresUI = root.Q("Jugadores");

        foreach (var jugador in jugadores)
        {
            TemplateContainer jugadorContainer = jugadorTemplate.Instantiate();
            jugadoresUI.Add(jugadorContainer);
            root.Q<Label>("Nombre").text = jugador;
            root.Q<Label>("Nombre").name = jugador;

        }
    }
}
