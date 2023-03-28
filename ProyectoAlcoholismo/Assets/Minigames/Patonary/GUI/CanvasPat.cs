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

    private UIDocument doc;
    private Button doneButton;
    private Label TimeTex;

    //timer
    private string tiempo = "Tiempo: ";
    public float timeRemaining = 40;
    public bool timerIsRunning = false;
    //public Text timeText;
    private void Start()
    {
        // Starts the timer automatically
        timerIsRunning = true;
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
        menusController.GoToGuess();
    }
}

