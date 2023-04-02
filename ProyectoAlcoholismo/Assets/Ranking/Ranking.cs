using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UIElements;
using WebSocketSharp;
using Color32 = UnityEngine.Color32;

public class Ranking : MonoBehaviour
{
    public VisualTreeAsset jugadorTemplate;
    
    private string gameName;
    private int winnerIndex = -1;

    enum Modo
    {
        Rankings,
        Castigador,
        Castigado,
        Fin
    }

    private Modo modo = Modo.Rankings;
    private bool haSidoCastigado = false;

    private VisualElement root;
    private ListView jugadoresUI;
    private Button boton;
    
    private readonly string WAITSTR = "Esperando...";
    private readonly string STARTSTR = "Continuar!"; // quizá "Siguiente!"
    private readonly string ENDSTR = "Terminar!"; // quizá 2 botones con "Otra ronda!"

    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        jugadoresUI = root.Q<ListView>("Jugadores");

        gameName = GameState.Instance.CurrentGameName;
        var GameTitle = root.Q<Label>("NombreJuego");
        boton = root.Q<Button>("Boton");
        boton.clicked += ReadyButtonOnClicked;
        boton.text = STARTSTR;
        boton.SetEnabled(false);
        
        GameTitle.visible = !gameName.IsNullOrEmpty();
        if (GameTitle.visible)
        {
            Utils.GameConstants.GameNames.TryGetValue(gameName, out string nombre);
            GameTitle.text = nombre;
            boton.text = STARTSTR;
        }
        else
        {
            boton.text = ENDSTR;
        }

        fillPlayers();

        // Castigos
        jugadoresUI.onSelectionChange += OnSelectionChanged;
        jugadoresUI.selectionType = SelectionType.None;
        GameState.Instance.PlayerChangedData += OnPlayerChangedData;
    }

    private void OnSelectionChanged(IEnumerable<object> obj)
    {
        if (modo == Modo.Rankings) return;

        var indices = jugadoresUI.selectedIndices;
        var enabled = indices.Count() == 3;
        boton.SetEnabled(enabled);
    }

    private void OnPlayerChangedData(int id, NetworkDictionary<int, float> data)
    {
        if (modo == Modo.Rankings)
        {
            // Actualizar la lista si alguien "llega a rankings tarde"
            jugadoresUI.hierarchy.Clear();
            fillPlayers();
            return;
        }
        
        var castigador = GameState.GetPlayer(id).playerName;
        var myId = GameState.GetMyPlayer().playerId;

        var maxIter = Math.Min(3, Math.Min(data.Count, GameState.CountPlayers - 1));

        int count = 0;
        for (var i = 0; i < maxIter; ++i) // tres castigos (recepción no completa?)
        {
            var valor = data[i];
            if (valor == 0) continue;
            
            int castigado = (int)data[i] - 1;
            if (GameState.isServer) // debería tener todos los castigos y aplicarlos 1 vez.
            {
                var player = GameState.GetPlayer(castigado);
                player.SetScore(player.playerScore - 5);
            }
            
            ++count;
            haSidoCastigado = (myId == castigado);

            if (haSidoCastigado)
                break;
        }

        if (modo == Modo.Castigador) // Por si el server es castigador
            return;

        if (count < maxIter) return;
        
        var Texto = root.Q<Label>("Ranking");
        Texto.style.fontSize = 18;

        if (haSidoCastigado)
        {
            Texto.text = "Has recibido\ntu merecido\ncastigo!!";
        }
        else
        {
            Texto.text = "Parece que\nte has librado\nesta vez!!";
        }
        
        boton.SetEnabled(true);
        boton.visible = true;
        boton.text = STARTSTR;
    }

    private void fillPlayers()
    {
        if (modo == Modo.Rankings) // GameTitle.visible = true
        {
            jugadoresUI.hierarchy.Clear();
            Utils.GameConstants.GameSuffix.TryGetValue(gameName, out string sufijo);
            var scores = GameState.Instance.SortedScores();
            var GameTitle = root.Q<Label>("NombreJuego");
            winnerIndex = scores[0].Item1;
            
            foreach (var (id, score) in scores)
            {
                var playerValues = GameState.GetPlayer(id);
                var name = playerValues.playerName;
                //var color = GetStyledColor(playerValues.playerColor);

                TemplateContainer jugadorContainer = jugadorTemplate.Instantiate();
                jugadorContainer.Q<Label>("Nombre").text = name;
                jugadorContainer.Q<Label>("Nombre").style.fontSize = 14;
                jugadorContainer.Q<Label>("Puntuacion").text = ((score == -1) ? "0" : score.ToString()) + sufijo;
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
            
            boton.SetEnabled(allIn);
        }
        else // Castigos modo = Modo.Castigador || Modo.Fin
        {
            if (!PlayerRegistry.Instance || PlayerRegistry.Instance.ObjectByRef.Count == 0) return;
            
            jugadoresUI.hierarchy.Clear();
            foreach (var (id, player)  in PlayerRegistry.Instance.ObjectByRef)
            {
                //var color = GetStyledColor(player.playerColor);
                // no me voy a castigar a mi mismo.
                if (id == GameState.GetMyPlayer().playerId) continue; 
               
                TemplateContainer jugadorContainer = jugadorTemplate.Instantiate();
                jugadorContainer.Q<Label>("Nombre").text = player.playerName;
                jugadorContainer.Q<Label>("Puntuacion").text = GameState.GetPlayer(id).playerScore.ToString();
                //jugadorContainer.Q<VisualElement>("Icono").style.color = color;
                jugadorContainer.Q<VisualElement>("Icono").visible = false;
                jugadoresUI.hierarchy.Add(jugadorContainer);
            }
        }
    }

    private StyleColor GetStyledColor(Vector3 color)
    {
        return new StyleColor(new Color32((byte)(color[0] * 255),
            (byte)(color[1] * 255),
            (byte)(color[2] * 255), 255));
    }

    private void ReadyButtonOnClicked()
    {
        var Texto = root.Q<Label>("Ranking");
        Texto.style.fontSize = 18;
        
        switch (modo)
        {
            case Modo.Rankings:
                var soyCastigador = GameState.GetMyPlayer().playerId == winnerIndex;
                jugadoresUI.visible = soyCastigador;
                var GameTitle = root.Q<Label>("NombreJuego");
                GameTitle.text = "Castigos";
        
                boton.SetEnabled(false);
                boton.visible = soyCastigador;
            
                if (soyCastigador)
                {
                    modo = Modo.Castigador;
                    Texto.text = "Has ganado!!\nSelecciona\nhasta 3 miembros de\nla bandada\npara castigar.";
                    jugadoresUI.hierarchy.Clear();
                    jugadoresUI.selectionType = SelectionType.Multiple;
                    fillPlayers();    
                }
                else
                {
                    modo = Modo.Castigado;
                    jugadoresUI.visible = false;
                    Texto.text = "No has ganado!!\nPodrías ser\ncastigado!!\n\nTendrás que\nesperar a ver\nqué pasa.\n\nCruza las alas!!";
                    // Molaría poner una animación de un pato nervioso dando vueltas.
                }
                break;
            case Modo.Castigador:
                var indices = jugadoresUI.selectedIndices;
                int idx = 0;
                foreach (int index in indices)
                {
                    // Castigar jugador index.
                    GameState.GetMyPlayer().SetData(idx++, index+1);
                    if (GameState.isServer)
                    {
                        var player = GameState.GetMyPlayer();
                        OnPlayerChangedData(player.playerId, player.data);
                    }
                }
                modo = Modo.Fin;
                boton.SetEnabled(true);
                boton.visible = true;
                boton.text = GameState.Instance.RemainingGamesCount() > 0 ? STARTSTR : ENDSTR;
                Texto.text = "La bandada\nHa recibido\nsu castigo!";
                jugadoresUI.hierarchy.Clear();
                jugadoresUI.selectionType = SelectionType.None;
                fillPlayers(); // TODO: Puede que no aparezcan los castigos -> test!    
                break;
            case Modo.Castigado:
                modo = Modo.Fin;
                boton.SetEnabled(true);
                boton.visible = true;
                boton.text = GameState.Instance.RemainingGamesCount() > 0 ? STARTSTR : ENDSTR;
                Texto.text = haSidoCastigado ? "Has recibido\ntu castigo!" : "Te has librado\nesta vez!";
                jugadoresUI.hierarchy.Clear();
                jugadoresUI.visible = true;
                jugadoresUI.selectionType = SelectionType.None;
                fillPlayers(); // Mostrar puntuaciones de castigados    
                break;
            case Modo.Fin:
                GameState.FlipReadyFlag();
                boton.text = WAITSTR;
                boton.SetEnabled(false);
        
                // Necesario porque el server no recibe sus propios cambios.
                if (GameState.isServer)
                {
                    GameState.Instance.PlayerHasChangedReady(GameState.GetMyPlayer().playerId, GameState.GetMyPlayer().isReady);
                }
                break;
            default:
                // No debería
                throw new NotImplementedException();
        }
    }
}
