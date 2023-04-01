using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class ListaController : MonoBehaviour
{


    [SerializeField]
    private GameObject ApuestasObject;
    private ApuestasController apuestasController;
    private UIDocument doc;
    private ListView PlayerList;
    private List<string> AllPlayers;

    void OnEnable()
    {
        apuestasController = ApuestasObject.GetComponent<ApuestasController>();
        doc = GetComponent<UIDocument>();
        PlayerList = doc.rootVisualElement.Q<ListView>("PlayerList");

        List<string> AllPlayers = new List<string>();
        foreach (var pl in PlayerRegistry.Instance.ObjectByRef)
        {
            // Si soy yo no
            if (pl.Key.PlayerId == GameState.GetMyPlayer().playerId)
                continue;
            
            AllPlayers.Add(pl.Value.playerName);
        }

        PlayerList.itemsSource = AllPlayers;
        PlayerList.onSelectionChange += OnPlayerSelected;
    }

    void OnPlayerSelected(IEnumerable<object> selectedItems)
    {
        var selectedPlayer = PlayerList.selectedItem as string;
        apuestasController.challengedPlayer = selectedPlayer;
        apuestasController.GoTo("realizando");
    }

}
