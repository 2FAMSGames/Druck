using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Fusion;
using UnityEngine;
using UnityEngine.UIElements;

public class RankingJuego : MonoBehaviour
{
    public VisualTreeAsset jugadorTemplate;
    
    [SerializeField]
    private RankingMenu rootMenu;

    private WaitBarriers barriers;

    private VisualElement root;
    private ListView jugadoresUI;
    private Button boton;

    private Dictionary<int, bool> pulsados = new Dictionary<int, bool>();
    
    private bool alreadyClicked = false;
    private bool inBarrier = false;

    public void OnEnable()
    {
        barriers = rootMenu.GetComponent<WaitBarriers>();
        
        root = GetComponent<UIDocument>().rootVisualElement;
        jugadoresUI = root.Q<ListView>("Jugadores");

        var GameTitle = root.Q<Label>("NombreJuego");
        boton = root.Q<Button>("Boton");
        boton.clicked += OnReadyButtonOnClicked;
        boton.text = rootMenu.STARTSTR;
        boton.SetEnabled(false);
        Utils.GameConstants.GameNames.TryGetValue(rootMenu.gameName, out string nombre);
        GameTitle.text = nombre;
        
        fillPlayers();
        GameState.Instance.PlayerChangedData += OnPlayerChangedData;
        GameState.Instance.playing = false;
    }
    
    private void OnPlayerChangedData(int id, NetworkDictionary<int, float> data)
    {
        if (id == GameState.GetMyPlayer().playerId) return;
        
        // Actualizar la lista si alguien "llega a rankings tarde"
        jugadoresUI.hierarchy.Clear();
        fillPlayers();
    }

    private void fillPlayers()
    {
        Utils.GameConstants.GameSuffix.TryGetValue(rootMenu.gameName, out string sufijo);
        var scores = GameState.Instance.SortedScores();
        var GameTitle = root.Q<Label>("NombreJuego");
        rootMenu.winnerIdx = scores[0].Item1;

        foreach (var (id, score) in scores)
        {
            var playerValues = GameState.GetPlayer(id);
            var name = playerValues.playerName;
            //var color = GetStyledColor(playerValues.playerColor);

            var jugadorContainer = jugadorTemplate.Instantiate();
            jugadorContainer.Q<Label>("Nombre").text = playerValues.playerName;
            jugadorContainer.Q<Label>("Nombre").style.fontSize = 16;
            
            // Caso especial Patonary
            if (PlayerRegistry.Instance.CurrentScene == "Patonary")
            {
                if (score == 0) continue;
                jugadorContainer.Q<Label>("Puntuacion").text = (score > 0) ? "acertó" : ((score < 0) ?  "falló" : "esperando...");
            }
            else
            {
                jugadorContainer.Q<Label>("Puntuacion").text = ((score == -1) ? "0" : score.ToString()) + sufijo;
            }

            jugadorContainer.Q<Label>("Puntuacion").style.fontSize = 14;
            jugadorContainer.Q<Label>("Separador").style.fontSize = 14;
            //jugadorContainer.Q<VisualElement>("Icono").style.color = color;
            jugadorContainer.Q<VisualElement>("Icono").visible = false;
            jugadoresUI.hierarchy.Add(jugadorContainer);
        }

        bool allIn = true;
        foreach (var (id, score) in scores)
        {
            allIn &= score != 0;
        }

        boton.SetEnabled(allIn && !alreadyClicked);
    }

    private void OnReadyButtonOnClicked()
    {
        boton.SetEnabled(false);
        boton.text = rootMenu.WAITSTR;
        alreadyClicked = true;
        
        barriers.IAmInBarrier();
        inBarrier = true;
    }

    public void Update()
    {
        if(inBarrier)
            barriers.CheckBarrier();
    }
}
