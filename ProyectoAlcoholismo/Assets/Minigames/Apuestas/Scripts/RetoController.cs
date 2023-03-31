using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using Fusion;

public class RetoController : MonoBehaviour
{
    [SerializeField]
    private GameObject ApuestasObject;
    private ApuestasController apuestasController;
    public System.Random ran = new System.Random();
    private UIDocument doc;
    private Button reto1;
    private Button reto2;
    private Button reto3;
    public string yourPlayer = "Ana";
    public string CurrentPlayer = "Ana";


    [CreateAssetMenu]
    public class Challenge : ScriptableObject
    {
        public int chId { get; set; }
        public string chText { get; set; }
        public int chPrize { get; set; }
    }
    void OnEnable()
    {
        GameState.Instance.PlayerChangedData += OnPlayerChangedData;
        
        apuestasController = ApuestasObject.GetComponent<ApuestasController>();

        doc = GetComponent<UIDocument>();

        var i = -1;
        //apuestasController = apuestasObject.GetComponent<ApuestasController>();
        //Dictionary<int, string> challengeList = new Dictionary<int, string>();
        List<Challenge> challengeList = new List<Challenge>();
        challengeList.Add(new Challenge { chId = i++, chText = "Reto1 facilito", chPrize = 1 });
        challengeList.Add(new Challenge { chId = i++, chText = "Reto2 facilito", chPrize = 1 });
        challengeList.Add(new Challenge { chId = i++, chText = "Reto3 facilito", chPrize = 1 });
        challengeList.Add(new Challenge { chId = i++, chText = "Reto1 normalillo", chPrize = 2 });
        challengeList.Add(new Challenge { chId = i++, chText = "Reto2 normalillo", chPrize = 2 });
        challengeList.Add(new Challenge { chId = i++, chText = "Reto3 normalillo", chPrize = 2 });
        challengeList.Add(new Challenge { chId = i++, chText = "Reto1 chungo", chPrize = 3 });
        challengeList.Add(new Challenge { chId = i++, chText = "Reto2 chungo", chPrize = 3 });
        challengeList.Add(new Challenge { chId = i++, chText = "Reto3 chungo", chPrize = 3 });
        doc = GetComponent<UIDocument>();

        reto1 = doc.rootVisualElement.Q<Button>("Reto1");
        reto1.text = getRandomObject(challengeList.Where(x => x.chPrize == 1).ToList()).chText;
        reto1.RegisterCallback<ClickEvent>(GoToList);

        //reto1.clicked += new EventHandler(GoToList);//TODO: pasarle una variable como current player

        reto2 = doc.rootVisualElement.Q<Button>("Reto2");
        reto2.text = getRandomObject(challengeList.Where(x => x.chPrize == 2).ToList()).chText;
        reto2.RegisterCallback<ClickEvent>(GoToList);
        //reto2.clicked += GoToList;//TODO: pasarle una variable como current player;

        reto3 = doc.rootVisualElement.Q<Button>("Reto3");
        reto3.text = getRandomObject(challengeList.Where(x => x.chPrize == 3).ToList()).chText;
        reto3.RegisterCallback<ClickEvent>(GoToList);
        //reto3.clicked += GoToList;//TODO: pasarle una variable como current player;
    }

    private void OnPlayerChangedData(int id, NetworkDictionary<int, float> data)
    {
        // TODO: protocolo de valores con sentido para el juego
        if (id == 15) // GameState.Instance.Runner.SessionInfo.MaxPlayers - 1
        {
            // El servidor envia el dato
        }

        
        throw new System.NotImplementedException();
    }

    private void GoToList(ClickEvent evt)
    {
        var targetBox = evt.target as Button;
        apuestasController.challenge = targetBox.text;
        //apuestasController.GoTo("List");
        string screen = "espera";
        if (apuestasController.yourPlayer == apuestasController.CurrentPlayer)
        {
            screen = "lista";
        }
        apuestasController.GoTo(screen);
        //apuestasController.GoToMainMenu();
    }

    private Challenge getRandomObject(List<Challenge> list)
    {
        //Random rnd = new Random();
        //var index = random.Next(list.Count);
        //var randomItem = list[index];
        int index = ran.Next(list.Count);
        return list[index];
    }
}
