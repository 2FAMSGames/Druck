using UnityEngine;
using UnityEngine.UIElements;

public class RealizandoController : MonoBehaviour
{
    [SerializeField]
    private GameObject ApuestasObject;
    private ApuestasController apuestasController;
    //public System.Random ran = new System.Random();
    private UIDocument doc;
    private Label player;
    private Label challenge;
    private Button y;
    private Button n;

    void OnEnable()
    {
        apuestasController = ApuestasObject.GetComponent<ApuestasController>();
        doc = GetComponent<UIDocument>();
        player = doc.rootVisualElement.Q<Label>("CurrentPlayer");
        player.text = apuestasController.challengedPlayer;
        challenge = doc.rootVisualElement.Q<Label>("CurrentChallenge");
        challenge.text = apuestasController.challenge;
        y = doc.rootVisualElement.Q<Button>("y");
        y.RegisterCallback<ClickEvent>(GoToWinner);
        n = doc.rootVisualElement.Q<Button>("n");
        n.RegisterCallback<ClickEvent>(GoToWinner);
    }

    private void GoToWinner(ClickEvent evt)
    {
        var targetBox = evt.target as Button;
        var result = targetBox.name;
        
        Debug.Log(result);
        
        // si es >-1, winner será apuestasController.challengedPlayer(el jugador retado) si no apuestasController.CurrentPlayer, el jugador con el turno
        int value = (result == "y") ? 1 : -1; // yes: 1 - no: -1
        GameState.GetMyPlayer().SetData(0, value);
        
        // No debería saberse el ganador hasta que hayan votado todos? Hecho en "ganadorController"
        apuestasController.winner = result == "y" ? apuestasController.challengedPlayer : apuestasController.CurrentPlayer;
        apuestasController.GoTo("ganador");
    }
}
