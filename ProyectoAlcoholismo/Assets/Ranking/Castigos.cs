using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.UIElements;

public class Castigos : MonoBehaviour
{
    public VisualTreeAsset jugadorTemplateRadio;
    
    [SerializeField]
    private RankingMenu rootMenu;

    private WaitBarriers barriers;

    private VisualElement root;
    private ListView jugadoresUI;
    private Button boton;
    
    private Dictionary<string, Tuple<int, bool>> selectedPlayers = new Dictionary<string, Tuple<int, bool>>();
    private bool castigado = false;
    private bool soyCastigador = false;
    private int castigos = 3;
    private int numCastigos = 3;

    private Dictionary<int, Tuple<bool, Button>> buttons = new Dictionary<int, Tuple<bool, Button>>();
    private int botonPulsado;
    
    private bool alreadyClicked = false;
    private bool inBarrier = false;
    
    public void OnEnable()
    {
        barriers = rootMenu.GetComponent<WaitBarriers>();
        
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

        castigos = numCastigos = Math.Min(3, GameState.CountPlayers - 1);
        string plural = castigos > 1 ? "s" : "";
        
        if (soyCastigador)
        {
            Texto.text = "Has ganado!!\n\nSelecciona\nhasta " + castigos + " miembro" + plural + " de\nla bandada\npara castigar.";
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

        if (soyCastigador)
        {
            boton.SetEnabled(alreadyClicked);
            if (castigos > 0) return;
            
            Texto.text = "La bandada\nHa recibido\nsu castigo!";
            jugadoresUI.visible = false;
        }
        else
        {
            if(castigos > 0) --castigos;
            boton.SetEnabled(castigos == 0 && !alreadyClicked);
            alreadyClicked = true;
            
            for (int i = 0; i < numCastigos; ++i)
            {
                if (castigado || (int)(data[i] -1) == GameState.GetMyPlayer().playerId)
                {
                    castigado = true;
                    Texto.text = "Has sido castigado\npor " + GameState.GetPlayer(rootMenu.winnerIdx).playerName +
                                 "\n\nMás suerte la\npróxima vez!!";
                }
                else
                {
                    if (castigos > 0)
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

        boton.text = castigos == 0 ? rootMenu.STARTSTR : rootMenu.WAITSTR;
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
        boton.SetEnabled(pulsados.Count == numCastigos);
    }

    private void OnReadyButtonOnClicked()
    {
        var player = GameState.GetMyPlayer();
        
        boton.text = rootMenu.WAITSTR;   
        boton.SetEnabled(false);
       
        if (soyCastigador && castigos != 0)
        {
            var botonesPulsados = buttons.Where(t => t.Value.Item1 == true).ToList();
            if (botonesPulsados.Count != numCastigos) return;

            castigos = 0;
            int dataIndex = 0;
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
            barriers.IAmInBarrier();
            inBarrier = true;
            
            if(castigado)
                GameState.GetMyPlayer().SetScore(GameState.GetMyPlayer().playerScore - 10);
        }
    }

    public void Update()
    {
        if(inBarrier)
            barriers.CheckBarrier();
    }
}
