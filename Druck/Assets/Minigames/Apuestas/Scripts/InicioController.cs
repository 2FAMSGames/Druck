using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class InicioController : MonoBehaviour
{
    [SerializeField]
    private GameObject ApuestasObject;
    private ApuestasController apuestasController;
    private UIDocument doc;
    private Button inicio;

    void OnEnable()
    {
        apuestasController = ApuestasObject.GetComponent<ApuestasController>();

        doc = GetComponent<UIDocument>();
        inicio = doc.rootVisualElement.Q<Button>("Inicio");
        inicio.clicked += GoToStart;
        //inicio.RegisterCallback<>(GoToStart);

    }

    private void GoToStart()
    {
        GameState.GetMyPlayer().ResetData();
        GameState.GetMyPlayer().SetReady(false);

        apuestasController.GoTo("reto");
    }

}
