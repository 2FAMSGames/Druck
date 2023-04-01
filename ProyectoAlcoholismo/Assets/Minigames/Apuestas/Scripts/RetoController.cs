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
    private Label prize1;
    private Label prize2;
    private Label prize3;
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

        List<Challenge> challengeList = new List<Challenge>();
        challengeList.Add(new Challenge { chId = i++, chText = "Enseñar tus tres últimas conversaciones de whatsapp/telegram", chPrize = 1 });
        challengeList.Add(new Challenge { chId = i++, chText = "Decir quién te parece la persona más guapa del grupo (no vale decir a tu pareja)", chPrize = 1 });
        challengeList.Add(new Challenge { chId = i++, chText = "Decir lo que más te gusta de la persona que el retador elija (puede ser el retador)", chPrize = 1 });
        challengeList.Add(new Challenge { chId = i++, chText = "Quitarte un zapato", chPrize = 1 });
        challengeList.Add(new Challenge { chId = i++, chText = "Bailar el YMCA", chPrize = 1 });
        challengeList.Add(new Challenge { chId = i++, chText = "Imitar la película que te diga el retador y que el resto lo adivine antes de 1 minuto", chPrize = 1 });
        challengeList.Add(new Challenge { chId = i++, chText = "Contar un chiste malo y conseguir que alguien se ría", chPrize = 1 });
        challengeList.Add(new Challenge { chId = i++, chText = "Déjarte peinar y maquillar por el resto", chPrize = 1 });
        challengeList.Add(new Challenge { chId = i++, chText = "Estar a la pata coja durante 1 minuto", chPrize = 1 });
        challengeList.Add(new Challenge { chId = i++, chText = "Dar me gusta a una publicación de tu ex", chPrize = 2 });
        challengeList.Add(new Challenge { chId = i++, chText = "Chuparle la oreja a un jugador que elija el retador (puede ser el retador)", chPrize = 2 });
        challengeList.Add(new Challenge { chId = i++, chText = "Decir lo que menos te gusta de quien diga el retador (puede ser el retador)", chPrize = 2 });
        challengeList.Add(new Challenge { chId = i++, chText = "Quitarse la camiseta", chPrize = 2 });
        challengeList.Add(new Challenge { chId = i++, chText = "Cantar la canción que elija el retador a grito pelado", chPrize = 2 });
        challengeList.Add(new Challenge { chId = i++, chText = "Imitar a alguien del grupo que diga el retador y que el resto adivinen quién es en menos de 1 minuto", chPrize = 2 });
        challengeList.Add(new Challenge { chId = i++, chText = "Vendarte los ojos y adivinar a cada uno del grupo sólo oliéndoles", chPrize = 2 });
        challengeList.Add(new Challenge { chId = i++, chText = "Aguantar sin moverte mientras que el resto te haga lo que quiera durante 1 minuto", chPrize = 2 });
        challengeList.Add(new Challenge { chId = i++, chText = "Dar 10 vueltas sobre ti mismo", chPrize = 2 });
        challengeList.Add(new Challenge { chId = i++, chText = "Dejarle tu móvil a quien quiera el retador y que mande un mensaje que quiera a quien quiera", chPrize = 3 });
        challengeList.Add(new Challenge { chId = i++, chText = "Darle un beso pasional en la boca a quien quiera el retador (puede ser el retador)", chPrize = 3 });
        challengeList.Add(new Challenge { chId = i++, chText = "Decir con quién te acostarías del grupo (no vale tu pareja)", chPrize = 3 });
        challengeList.Add(new Challenge { chId = i++, chText = "Darle tu ropa interior al quien quiera el retador (puede ser el retador)", chPrize = 3 });
        challengeList.Add(new Challenge { chId = i++, chText = "Hacer un baile erótico a quien quiera el retador (puede ser el retador) durante 1 minuto", chPrize = 3 });
        challengeList.Add(new Challenge { chId = i++, chText = "Dejar tu móvil a quien diga el retador y que mande el mensaje que quiera a quien quiera", chPrize = 3 });
        challengeList.Add(new Challenge { chId = i++, chText = "Con los ojos vendados identificar al resto de jugadores tocándoles", chPrize = 3 });
        challengeList.Add(new Challenge { chId = i++, chText = "Con los ojos vendados y las manos atadas (sin usarlas) identificar al resto de jugadores rozándoles", chPrize = 3 });
        challengeList.Add(new Challenge { chId = i++, chText = "Dejar tu móvil desbloqueado y que el resto haga lo que quiera con él durante 1 minuto", chPrize = 3 });
        doc = GetComponent<UIDocument>();

        reto1 = doc.rootVisualElement.Q<Button>("Reto1");
        prize1 = doc.rootVisualElement.Q<Label>("Prize1");
        var r1 = getRandomObject(challengeList.Where(x => x.chPrize == 1).ToList());
        reto1.text = r1.chText;
        prize1.text = r1.chPrize.ToString();
        reto1.RegisterCallback<ClickEvent, string>(GoToList, r1.chPrize.ToString());

        reto2 = doc.rootVisualElement.Q<Button>("Reto2");
        prize2 = doc.rootVisualElement.Q<Label>("Prize2");
        var r2 = getRandomObject(challengeList.Where(x => x.chPrize == 2).ToList());
        reto2.text = r2.chText;
        prize2.text = r2.chPrize.ToString();
        reto2.RegisterCallback<ClickEvent, string>(GoToList, r2.chPrize.ToString());

        reto3 = doc.rootVisualElement.Q<Button>("Reto3");
        prize3 = doc.rootVisualElement.Q<Label>("Prize3");
        var r3 = getRandomObject(challengeList.Where(x => x.chPrize == 3).ToList());
        reto3.text = r3.chText;
        prize3.text = r3.chPrize.ToString();
        reto3.RegisterCallback<ClickEvent,string>(GoToList, r3.chPrize.ToString());
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

    private void GoToList(ClickEvent evt, string p)
    {
        var targetBox = evt.target as Button;
        apuestasController.challenge = targetBox.text;
        apuestasController.prize = p;
        string screen = "espera";
        if (apuestasController.yourPlayer == apuestasController.CurrentPlayer)
        {
            screen = "lista";
        }
        apuestasController.GoTo(screen);
    }

    private Challenge getRandomObject(List<Challenge> list)
    {
        int index = ran.Next(list.Count);
        return list[index];
    }
}
