using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Color32 = UnityEngine.Color32;

public class Ranking : MonoBehaviour
{
    public List<string> jugadores;
    public VisualTreeAsset jugadorTemplate;

    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        VisualElement jugadoresUI = root.Q("Jugadores");
        
        // TODO: get scores from GameState according to current game
        var scores = PlayerRegistry.Instance.SortedScores();
        // TODO: get current game name from GameState too.
        //var gameName = GameState.currentGameName();
        //var GameTitle = root.Q<Label>("NombreJuego").text = gameName;
        
        foreach (var player in scores)
        {
            var playerValues = GameState.GetPlayer(player.Item1);
            var name = playerValues.playerName;
            var color = playerValues.playerColor;
            var score = playerValues.playerScore;

            var styleColor = new StyleColor(new Color32((byte)(color[0] * 255),
                                                                (byte)(color[1] * 255),
                                                                (byte)(color[2] * 255), 255));
            
            TemplateContainer jugadorContainer = jugadorTemplate.Instantiate();
            jugadorContainer.Q<Label>("Nombre").text = name;
            jugadorContainer.Q<VisualElement>("Icono").style.color = styleColor;
            jugadoresUI.Add(jugadorContainer);
        }
    }
}
