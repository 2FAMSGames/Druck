using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.UIElements;

public class Castigos : MonoBehaviour
{
    [SerializeField]
    private RankingMenu rootMenu;

    private VisualElement root;
    private ListView jugadoresUI;
    private Button boton;
    
    private Dictionary<string, Tuple<int, bool>> selectedPlayers = new Dictionary<string, Tuple<int, bool>>();
    private bool castigado = false;
    private bool soyCastigador = false;
    private int maxCastigos = 3;

    private Dictionary<int, Tuple<bool, Button>> buttons = new Dictionary<int, Tuple<bool, Button>>();
    private int botonPulsado;
    
    private bool alreadyClicked = false;
    private bool inBarrier = false;
    
    public void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        jugadoresUI = root.Q<ListView>("Jugadores");
        
        var Texto = root.Q<Label>("Ranking");
        Texto.style.fontSize = 14;

        var GameTitle = root.Q<Label>("NombreJuego");
        GameTitle.text = "Castigos";

        boton = root.Q<Button>("Boton");
        boton.clicked += OnReadyButtonOnClicked;
        boton.SetEnabled(false);
        
        soyCastigador = GameState.GetMyPlayer().playerId == rootMenu.winnerIdx;
        boton.text = soyCastigador ? "Castigar" : rootMenu.WAITSTR;
        
        jugadoresUI.visible = soyCastigador;

        maxCastigos = Math.Min(3, GameState.CountPlayers - 1);
        string plural = maxCastigos > 1 ? "s" : "";
        
        if (soyCastigador)
        {
            Texto.text = "Has ganado!!\n\nSelecciona\nhasta " + maxCastigos + " miembro" + plural + " de\nla bandada\npara castigar.";
            fillPlayers();
        }
        else
        {
            Texto.text = "No has ganado!!\n\nPodrías ser\ncastigado!!\n\nTendrás que\nesperar a ver\nqué pasa.\n\nCruza las alas!!";
            // Molaría poner una animación de un pato nervioso dando vueltas.
        }
        
        GameState.Instance.PlayerChangedData += OnPlayerChangedData;
    }

    private void OnPlayerChangedData(int id, NetworkDictionary<int, float> data)
    {
        var Texto = root.Q<Label>("Ranking");
        Texto.style.fontSize = 14;

        int castigosDados = 0;
        if (soyCastigador)
        {
            if (!alreadyClicked) return;
            
            Texto.text = "La bandada\nHa recibido\nsu castigo!";
            jugadoresUI.visible = false;
            castigosDados = maxCastigos;
        }
        else
        {
            for (int i = 0; i < maxCastigos; ++i)
            {
                if (data[i + 10] != 0) ++castigosDados;
                
                if (castigado || (int)(data[i+10] -1) == GameState.GetMyPlayer().playerId)
                {
                    castigado = true;
                    Texto.text = "Has sido castigado\npor " + GameState.GetPlayer(rootMenu.winnerIdx).playerName +
                                 "\n\nMás suerte la\npróxima vez!!";
                }
                else
                {
                    if (castigosDados < maxCastigos)
                    {
                        Texto.text = "No has ganado!!\n\nPodrías ser\ncastigado!!\n\nTendrás que\nesperar a ver\nqué pasa.\n\nCruza las alas!!";
                    }
                    else
                    {
                        Texto.text = "Te has librado!!\n\n";
                    }
                }
            }

            
        }

        alreadyClicked = castigosDados == maxCastigos;
        boton.text = alreadyClicked ? rootMenu.STARTSTR : rootMenu.WAITSTR;
        boton.SetEnabled(alreadyClicked);
    }

    private void fillPlayers()
    {
        if (!PlayerRegistry.Instance || PlayerRegistry.Instance.ObjectByRef.Count == 0) return;

        // Si quitas la siguiente línea jugadoresUI casca con una excepción.
        // Buscando en Goggle pone que es un error de Unity... raro.
        jugadoresUI.itemsSource = new List<string>();
        jugadoresUI.style.color = new StyleColor(new Color(0, 0, 0, 0));
        foreach (var (id, player) in PlayerRegistry.Instance.ObjectByRef)
        {
            //var color = GetStyledColor(player.playerColor);
            // no me voy a castigar a mi mismo.
            if (id == GameState.GetMyPlayer().playerId) continue;
            var name = player.playerName;

            var button = new Button();
            button.text = name;
            button.style.color = new Color(1, 1, 1);
            button.style.backgroundColor = new Color(0.6f, 0.4f, 0);
            button.clicked += (() =>
            {
                 botonPulsado = player.playerId;
                 OnButtonClicked();
            });
            buttons.Add(player.playerId, new Tuple<bool, Button>(false, button));
            
            jugadoresUI.hierarchy.Add(button);
        }
    }

    private void OnButtonClicked()
    {
        var (value, b) = buttons[botonPulsado];
        value = !value;
        
        b.style.backgroundColor = value ? new Color(1, 0.6f, 0) : new Color(0.5f, 0.3f, 0);
        buttons[botonPulsado] = new Tuple<bool, Button>(value, b);

        var pulsados = buttons.Where(t => t.Value.Item1 == true).ToList();
        boton.SetEnabled(pulsados.Count == maxCastigos);
    }

    private void OnReadyButtonOnClicked()
    {
        var player = GameState.GetMyPlayer();
        
        boton.text = rootMenu.WAITSTR;   
        boton.SetEnabled(false);
       
        if (soyCastigador && !alreadyClicked)
        {
            var botonesPulsados = buttons.Where(t => t.Value.Item1 == true).ToList();
            if (botonesPulsados.Count != maxCastigos) return;

            int dataIndex = 10; // donde pondremos los castigos (max 3)
            alreadyClicked = true;
            foreach (var (key, value) in botonesPulsados) 
            {
                player.SetData(dataIndex++, key + 1);
            }
            
            OnPlayerChangedData(player.playerId, player.data);
            return;
        }

        if (alreadyClicked)
        {
            rootMenu.IAmInBarrier();
            inBarrier = true;
            
            if(castigado)
                GameState.GetMyPlayer().SetScore(GameState.GetMyPlayer().playerScore - 10);
        }
    }

    public void Update()
    {
        if(inBarrier)
            rootMenu.CheckBarrier();
    }
}
