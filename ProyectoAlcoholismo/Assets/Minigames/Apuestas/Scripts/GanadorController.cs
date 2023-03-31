using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class GanadorController : MonoBehaviour
{
    [SerializeField]
    private GameObject ApuestasObject;
    private ApuestasController apuestasController;
    private UIDocument doc;
    private Label winner;
    private Label nTragos;


    void OnEnable()
    {
        apuestasController = ApuestasObject.GetComponent<ApuestasController>();
        doc = GetComponent<UIDocument>();
        winner = doc.rootVisualElement.Q<Label>("winner");
        winner.text = apuestasController.winner;
        nTragos = doc.rootVisualElement.Q<Label>("number");
        
        //TODO: poner temporizador y que pase al turno siguiente
        // TODO: pasar el número de tragos para que el servidor los reste en el score 
    }

}
