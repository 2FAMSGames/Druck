using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class ApuestasController : MonoBehaviour
{

    [SerializeField]
    //private GameObject apuestasObject;
    private ApuestasController apuestasController;

    public System.Random ran = new System.Random();
    private UIDocument doc;
    private Button reto1;
    private Button reto2;
    private Button reto3;

    [CreateAssetMenu]
    public class Challenge : ScriptableObject
    {
        public int chId { get; set; }
        public string chText { get; set; }
        public int chPrize { get; set; }
    }
    void OnEnable()
    {
        var i = -1;
        //apuestasController = apuestasObject.GetComponent<ApuestasController>();
        //Dictionary<int, string> challengeList = new Dictionary<int, string>();
        List<Challenge> challengeList = new List<Challenge> ();
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
        reto1.clicked += CreateRoomButtonOnClicked;

        reto2 = doc.rootVisualElement.Q<Button>("Reto2");
        reto2.text = getRandomObject(challengeList.Where(x => x.chPrize == 2).ToList()).chText;
        reto2.clicked += CreateRoomButtonOnClicked;

        reto3 = doc.rootVisualElement.Q<Button>("Reto3");
        reto3.text = getRandomObject(challengeList.Where(x => x.chPrize == 3).ToList()).chText;
        reto3.clicked += CreateRoomButtonOnClicked;

        //joinRoomButton = doc.rootVisualElement.Q<Button>("JoinRoomButton");
        //joinRoomButton.clicked += JoinRoomButtonOnClicked;

        //optionsButton = doc.rootVisualElement.Q<Button>("OptionsButton");
        //optionsButton.clicked += OptionsButtonOnClicked;

        //exitButton = doc.rootVisualElement.Q<Button>("ExitButton");
        //exitButton.clicked += ExitButtonOnClicked;
    }

    private Challenge getRandomObject(List<Challenge> list)
    {
        //Random rnd = new Random();
        //var index = random.Next(list.Count);
        //var randomItem = list[index];
        int index = ran.Next(list.Count);
        return list[index];
    }

    private void CreateRoomButtonOnClicked()
    {
        Debug.Log("Create button clicked");
        //menusController.GoToRoomCreateMenu();
    }

    //private void JoinRoomButtonOnClicked()
    //{
    //    Debug.Log("Join button clicked");
    //    menusController.GoToRoomJoinMenu();
    //}

    //private void OptionsButtonOnClicked()
    //{
    //    Debug.Log("Options button clicked");
    //    menusController.GoToSettingsMenu();
    //}

    //private void ExitButtonOnClicked()
    //{
    //    Debug.Log("Exit button clicked");
    //    Application.Quit();
    //}
}
