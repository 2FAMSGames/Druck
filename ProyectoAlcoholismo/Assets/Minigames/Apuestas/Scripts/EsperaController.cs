using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class EsperaController : MonoBehaviour
{
    [SerializeField]
    private GameObject ApuestasObject;
    private ApuestasController apuestasController;
    private UIDocument doc;
    private Label currentPlayer;

    void OnEnable()
    {
        //TODO: linkar espera 
    }

    private void GoToList()
    {
        Debug.Log("GoToNada, a esperar");
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
