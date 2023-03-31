using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using Fusion;

public class EsperaController : MonoBehaviour
{
    [SerializeField]
    private GameObject ApuestasObject;
    private ApuestasController apuestasController;
    private UIDocument doc;
    private Label currentPlayer;

    void OnEnable()
    {
        apuestasController = ApuestasObject.GetComponent<ApuestasController>();
        doc = GetComponent<UIDocument>();
        currentPlayer = doc.rootVisualElement.Q<Label>("CurrentPlayer");
        currentPlayer.text = apuestasController.yourPlayer;
    }

    private void GoToList()
    {
        
        //apuestasController.GoTo("List");
        //apuestasController.GoTo("espera");
        //apuestasController.GoToMainMenu();
    }

    //private Challenge getRandomObject(List<Challenge> list)
    //{
    //    //Random rnd = new Random();
    //    //var index = random.Next(list.Count);
    //    //var randomItem = list[index];
    //    int index = ran.Next(list.Count);
    //    return list[index];
    //}
}
