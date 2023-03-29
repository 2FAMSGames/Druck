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
        //doc = GetComponent<UIDocument>();
        PlayerList = doc.rootVisualElement.Q<ListView>("PlayerList");
        AllPlayers = new List<string>() { "Ana", "Marta", "Sofía", "Miguel" };
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
