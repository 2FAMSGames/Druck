using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RankingFinal : MonoBehaviour
{
    public VisualTreeAsset jugadorTemplate;
    
    [SerializeField]
    private RankingMenu rootMenu;

    private VisualElement root;
    private ListView jugadoresUI;
    private Button boton;
    
    public void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        jugadoresUI = root.Q<ListView>("Jugadores");

        var GameTitle = root.Q<Label>("NombreJuego");
        GameTitle.text = "Actual";
        boton = root.Q<Button>("Boton");
        boton.clicked += OnReadyButtonOnClicked;
        boton.text = GameState.Instance.RemainingGamesCount() > 0 ? rootMenu.STARTSTR : rootMenu.ENDSTR;
        fillPlayers();
    }

    private void fillPlayers()
    {
        var scoresFin = PlayerRegistry.Instance.SortedScores();
        var GameTitleFin = root.Q<Label>("NombreJuego");
        GameTitleFin.text = "Generales"; 

        foreach (var (id, score) in scoresFin)
        {
            var playerValues = GameState.GetPlayer(id);
            var name = playerValues.playerName;
            if (!Utils.GameConstants.GameSuffix.TryGetValue("General", out string sufijo))
                throw new Exception("Sufijo inválido.");
            
            //var color = GetStyledColor(playerValues.playerColor);

            var jugadorContainer = jugadorTemplate.Instantiate();
            jugadorContainer.Q<Label>("Nombre").text = playerValues.playerName;
            jugadorContainer.Q<Label>("Nombre").style.fontSize = 16;
            jugadorContainer.Q<Label>("Puntuacion").text = ((score == -1) ? "0" : score.ToString() + sufijo);
            jugadorContainer.Q<Label>("Puntuacion").style.fontSize = 14;
            jugadorContainer.Q<Label>("Separador").style.fontSize = 14;
            //jugadorContainer.Q<VisualElement>("Icono").style.color = color;
            jugadorContainer.Q<VisualElement>("Icono").visible = true;
            jugadoresUI.hierarchy.Add(jugadorContainer);
        }
    }

    private void OnReadyButtonOnClicked()
    {
        boton.text = rootMenu.WAITSTR;
        boton.SetEnabled(false);
        
        GameState.GetMyPlayer().SetReady(true);
        
        // Necesario porque el server no recibe sus propios cambios.
        if (GameState.Instance.isServer)
        {
            GameState.Instance.PlayerHasChangedReady(GameState.GetMyPlayer().playerId, GameState.GetMyPlayer().isReady);
        }
    }
}
