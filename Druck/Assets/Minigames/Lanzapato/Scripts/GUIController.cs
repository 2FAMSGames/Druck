using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GUIController : MonoBehaviour
{
    private UIDocument doc;
    private Label levelCounter;
    private Label scoreCounter;

    private GameManagerController gameManagerController;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerController = GetComponent<GameManagerController>();
        doc = GetComponent<UIDocument>();

        levelCounter = doc.rootVisualElement.Q<Label>("Level");
        scoreCounter = doc.rootVisualElement.Q<Label>("Score");
    }

    // Update is called once per frame
    void Update()
    {
        levelCounter.text = (1 + gameManagerController.currentLevel).ToString();
        scoreCounter.text = gameManagerController.score.ToString();
    }
}
