using System;
using System.Collections.Generic;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using WebSocketSharp;
using Color32 = UnityEngine.Color32;

public class Ranking : MonoBehaviour
{
    public VisualTreeAsset jugadorTemplate;
    public VisualTreeAsset jugadorTemplateRadioButton;

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

    private Dictionary<string, Tuple<int, bool>> selectedPlayers = new Dictionary<string, Tuple<int, bool>>();
    
    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        jugadoresUI = root.Q<ListView>("Jugadores");

        gameName = PlayerRegistry.Instance.CurrentScene;
        var GameTitle = root.Q<Label>("NombreJuego");
        boton = root.Q<Button>("Boton");
        boton.clicked += OnReadyButtonOnClicked;
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
        GameState.Instance.PlayerChangedData += OnPlayerChangedData;
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
        switch (modo)
        {
            case Modo.Rankings:
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

                    var jugadorContainer = jugadorTemplate.Instantiate();
                    jugadorContainer.Q<Label>("Nombre").text = playerValues.playerName;
                    jugadorContainer.Q<Label>("Nombre").style.fontSize = 16;
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
                break;
            case Modo.Castigador:
                if (!PlayerRegistry.Instance || PlayerRegistry.Instance.ObjectByRef.Count == 0) return;
                jugadoresUI.hierarchy.Clear();

                foreach (var (id, player) in PlayerRegistry.Instance.ObjectByRef)
                {
                    //var color = GetStyledColor(player.playerColor);
                    // no me voy a castigar a mi mismo.
                    if (id == GameState.GetMyPlayer().playerId) continue;
                    var playerValues = GameState.GetPlayer(id);
                    var name = playerValues.playerName;
                
                    selectedPlayers.Add(name, new Tuple<int, bool>(playerValues.playerId, false));

                    var jugadorContainer = jugadorTemplateRadioButton.Instantiate();
                    var toggle = jugadorContainer.Q<Toggle>("Jugador");
                    toggle.label = name;
                    toggle.style.fontSize = 14;
                    toggle.value = false;
                    toggle.RegisterCallback<ChangeEvent<bool>>(OnToggle);
                    jugadoresUI.hierarchy.Add(jugadorContainer);
                }
                break;
            case Modo.Castigado:
                return; 
            case Modo.Fin:
                jugadoresUI.hierarchy.Clear();
                var scoresFin = PlayerRegistry.Instance.SortedScores();
                var GameTitleFin = root.Q<Label>("NombreJuego");
                GameTitleFin.text = "Generales"; 

                foreach (var (id, score) in scoresFin)
                {
                    var playerValues = GameState.GetPlayer(id);
                    var name = playerValues.playerName;
                    //var color = GetStyledColor(playerValues.playerColor);

                    var jugadorContainer = jugadorTemplate.Instantiate();
                    jugadorContainer.Q<Label>("Nombre").text = playerValues.playerName;
                    jugadorContainer.Q<Label>("Nombre").style.fontSize = 16;
                    jugadorContainer.Q<Label>("Puntuacion").text = ((score == -1) ? "0" : score.ToString());
                    jugadorContainer.Q<Label>("Puntuacion").style.fontSize = 14;
                    jugadorContainer.Q<Label>("Separador").style.fontSize = 14;
                    //jugadorContainer.Q<VisualElement>("Icono").style.color = color;
                    jugadorContainer.Q<VisualElement>("Icono").visible = false;
                    jugadoresUI.hierarchy.Add(jugadorContainer);
                }

                break;
        }
    }

    private void OnToggle(ChangeEvent<bool> evt)
    {
        if (modo != Modo.Castigador) return;
        
        if (evt.IsUnityNull()) return;
        evt.StopPropagation();
        
        var toggleWidget = (Toggle)evt.currentTarget;
        var item = selectedPlayers[toggleWidget.label];
        selectedPlayers[toggleWidget.label] = new Tuple<int, bool>(item.Item1, evt.newValue);
        
        int count = 0;
        foreach (var (name, value) in selectedPlayers)
        {
            if (value.Item2)
              ++count;
            Debug.Log(name + " = " + value);
        }

        var limit = Math.Min(3, GameState.CountPlayers - 1);
        if(count == limit)
            boton.SetEnabled(true);
    }

    private StyleColor GetStyledColor(Vector3 color)
    {
        return new StyleColor(new Color32((byte)(color[0] * 255),
            (byte)(color[1] * 255),
            (byte)(color[2] * 255), 255));
    }

    private void OnReadyButtonOnClicked()
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
                var limit = Math.Min(3, GameState.CountPlayers - 1);
                string plural = limit > 1 ? "s" : "";

                if (soyCastigador)
                {
                    modo = Modo.Castigador;
                    Texto.text = "Has ganado!!\n\nSelecciona\nhasta " + limit + " miembro" + plural + " de\nla bandada\npara castigar.";
                    jugadoresUI.hierarchy.Clear();
                    fillPlayers();
                }
                else
                {
                    modo = Modo.Castigado;
                    jugadoresUI.hierarchy.Clear();
                    jugadoresUI.visible = false;
                    Texto.text = "No has ganado!!\n\nPodrías ser\ncastigado!!\n\nTendrás que\nesperar a ver\nqué pasa.\n\nCruza las alas!!";
                    // Molaría poner una animación de un pato nervioso dando vueltas.
                }
                break;
            case Modo.Castigador:
                int count = 0;
                foreach (var (name, value) in selectedPlayers)
                    count += value.Item2 ? 1 : 0;
                if (count != Math.Min(3, GameState.CountPlayers)) return;

                int dataIndex = 0;
                foreach (var (name, value) in selectedPlayers)
                {
                    if (value.Item2)
                    {
                        GameState.GetMyPlayer().SetData(dataIndex++, value.Item1 + 1);
                        break;
                    }
                }
                
                if (GameState.isServer)
                {
                    var player = GameState.GetMyPlayer();
                    OnPlayerChangedData(player.playerId, player.data);
                }
                modo = Modo.Fin;
                boton.SetEnabled(true);
                boton.visible = true;
                boton.text = GameState.Instance.RemainingGamesCount() > 0 ? STARTSTR : ENDSTR;
                Texto.text = "La bandada\nHa recibido\nsu castigo!";
                jugadoresUI.hierarchy.Clear();
                fillPlayers();     
                break;
            case Modo.Castigado:
                modo = Modo.Fin;
                boton.SetEnabled(true);
                boton.visible = true;
                boton.text = GameState.Instance.RemainingGamesCount() > 0 ? STARTSTR : ENDSTR;
                Texto.text = haSidoCastigado ? "Has recibido\ntu castigo!" : "Te has librado\nesta vez!";
                jugadoresUI.hierarchy.Clear();
                jugadoresUI.visible = true;
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
