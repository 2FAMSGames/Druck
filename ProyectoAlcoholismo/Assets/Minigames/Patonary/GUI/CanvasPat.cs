using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CanvasPat : MonoBehaviour
{
    [SerializeField]
    private GameObject SavePic;
    private DrawLine _DrawLine;
    [SerializeField]
    private GameObject menusObject;
    private PatonaryMenuController menusController;

    public GameObject WordsRandom;
    private WordsRand _rndWord;

    private UIDocument doc;
    private Button doneButton;

    //timer
    private Label TimeTex;
    private string tiempo = "Tiempo: ";
    public float timeRemaining = 40;
    public bool timerIsRunning = false;
    //public Text timeText;

    //Word
    private Label WordRnd;
    private string palabra = "Pinta: ";

    private int CanvasWaitng = 1;

    private void Start()
    {
        // Starts the timer automatically
        timerIsRunning = true;
        WordRnd = doc.rootVisualElement.Q<Label>("RndWord");
        _rndWord = WordsRandom.GetComponent<WordsRand>();

        WordRnd.text = palabra + _rndWord.textWord;
    }
    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
                DoneButtonOnClicked();
            }
        }
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        TimeTex = doc.rootVisualElement.Q<Label>("Timer");
        TimeTex.text = tiempo + string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void OnEnable()
    {
        PlayerPrefs.SetInt("WaitngScreen", CanvasWaitng);

        menusController = menusObject.GetComponent<PatonaryMenuController>();
        _DrawLine = SavePic.GetComponent<DrawLine>();

        doc = GetComponent<UIDocument>();

        doneButton = doc.rootVisualElement.Q<Button>("DoneButton");
        doneButton.clicked += DoneButtonOnClicked;
    }

    private void DoneButtonOnClicked()
    {
        Debug.Log("Done button clicked");

        if (SavePic != null)
        {
            _DrawLine = SavePic.GetComponent<DrawLine>();

            if (_DrawLine != null)
            {
                //Debug.Log("Calling Save()...");
                _DrawLine.Save();
                
                // TODO: got to 
                StartCoroutine(DelayedGoToVotar());
            }
            //else
            //{
            //    Debug.LogWarning("SavePic doesn't have a DrawLine component!");
            //}
        }
        //else
        //{
        //    Debug.LogWarning("SavePic is not set!");
        //}
    }

    private IEnumerator DelayedGoToVotar()
    {
        yield return new WaitForSeconds(0.1f);
        //menusController.GoToGuess();

        PlayerPrefs.SetInt("WaitngScreen", CanvasWaitng);

        //if (CanvasWaitngEnded == 1)   //if everyone´s pic/guess/vote is ready then
        //{
        //    menusController.GoToGuess();
        //}
        menusController.GoToWait();

    }
}

