using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class RealizandoController : MonoBehaviour
{
    [SerializeField]
    private GameObject ApuestasObject;
    private ApuestasController apuestasController;
    //public System.Random ran = new System.Random();
    private UIDocument doc;
    private Label player;
    private Label challenge;
    //private Button reto1;
    //private Button reto2;

    [CreateAssetMenu]
    public class Challenge : ScriptableObject
    {
        public int chId { get; set; }
        public string chText { get; set; }
        public int chPrize { get; set; }
    }
    void OnEnable()
    {
        apuestasController = ApuestasObject.GetComponent<ApuestasController>();
        doc = GetComponent<UIDocument>();
        player = doc.rootVisualElement.Q<Label>("CurrentPlayer");
        player.text = apuestasController.challengedPlayer;
        challenge = doc.rootVisualElement.Q<Label>("CurrentChallenge");
        challenge.text = apuestasController.challenge;
    }

    private void GoToList()
    {
        Debug.Log("GoToList");
        //apuestasController.GoTo("List");
        //apuestasController.GoTo("espera");
        //apuestasController.GoToMainMenu();
    }

}
