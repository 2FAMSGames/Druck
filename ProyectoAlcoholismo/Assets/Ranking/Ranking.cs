using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using WebSocketSharp;
using Color32 = UnityEngine.Color32;

public class Ranking : MonoBehaviour
{
    public VisualTreeAsset jugadorTemplate;
    
    private Button readyButton;
    private string gameName;
    
    private readonly string WAITSTR = "Esperando...";
    private readonly string STARTSTR = "Continuar!"; // quizá "Siguiente!"
    private readonly string ENDSTR = "Terminar!"; // quizá 2 botons con "Otra ronda!"

    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        VisualElement jugadoresUI = root.Q("Jugadores");

        var scores = GameState.Instance.SortedScores();
        
        gameName = GameState.Instance.CurrentGameName;
        var GameTitle = root.Q<Label>("NombreJuego");
        readyButton = root.Q<Button>("Boton");
        readyButton.clicked += ReadyButtonOnClicked;
        readyButton.text = STARTSTR;
        
        GameTitle.visible = !gameName.IsNullOrEmpty();
        if (GameTitle.visible)
        {
            GameTitle.text = gameName;
            readyButton.text = STARTSTR;
        }
        else
            readyButton.text = ENDSTR;
            

        string sufijo;
        // TODO: actualizar cuando se metan mas juegos.
        switch (gameName)
        {
            case "AHuevo":
                sufijo = " puntos";
                break;
            case "Lanzapato":
                sufijo = " aciertos";
                break;
            default:
                sufijo = "";
                break;
        }
            
        foreach (var (id, score) in scores)
        {
            var playerValues = GameState.GetPlayer(id);
            var name = playerValues.playerName;
            var color = playerValues.playerColor;

            var styleColor = new StyleColor(new Color32((byte)(color[0] * 255),
                                                                (byte)(color[1] * 255),
                                                                (byte)(color[2] * 255), 255));
            
            TemplateContainer jugadorContainer = jugadorTemplate.Instantiate();
            jugadorContainer.Q<Label>("Nombre").text = name;
            jugadorContainer.Q<Label>("Separador").visible = GameTitle.visible;
            jugadorContainer.Q<Label>("Puntuacion").visible = GameTitle.visible;
            jugadorContainer.Q<Label>("Puntuacion").text = score.ToString() + sufijo;
            jugadorContainer.Q<VisualElement>("Icono").style.color = styleColor;
            jugadoresUI.Add(jugadorContainer);
        }
    }

    private void ReadyButtonOnClicked()
    {
        if (gameName.IsNullOrEmpty())
        {
            // TODO: terminar la aplicación
            return;
        }
        
        GameState.FlipReadyFlag();
        readyButton.text = WAITSTR;
        readyButton.SetEnabled(false);
        
        // Necesario porque el server no recibe sus propios cambios.
        if (GameState.isServer)
        {
            GameState.Instance.PlayerHasChangedReady(GameState.GetMyPlayer().playerId, GameState.GetMyPlayer().isReady);
        }
    }
}
