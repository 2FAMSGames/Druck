using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    int scoreToPassLevel = 3;
    [SerializeField]
    List<Lanzapato_LevelSettings> gameLevelList;
    
    [SerializeField]
    GameObject slingshot;
    SlingshotController slingshotController;

    [SerializeField]
    GameObject cupsContainer;
    CupsController cupsController;

    [Header("Debug")]
    [SerializeField]
    bool scoreDebug = false;

    [SerializeField]
    public int score = 0;
    [SerializeField]
    public int currentLevel = 0 ;
    int nTotalLevels;

    // Start is called before the first frame update
    void Start()
    {
        GameState.GetMyPlayer().ResetData();
        
        slingshotController = slingshot.GetComponent<SlingshotController>();
        cupsController = cupsContainer.GetComponent<CupsController>();
        nTotalLevels = gameLevelList.Count;
        SetupLevel(currentLevel);

        GameState.Instance.PlayerChangedData += OnPlayerFinished;
    }

    // Update is called once per frame
    void Update()
    {
        if(score >= scoreToPassLevel)
        {
            DestroyLevel();
            currentLevel++;
            if(currentLevel < nTotalLevels)
            {
                SetupLevel(currentLevel);
            }
            else
            {
                Debug.Log("Ganaste");
                GameState.GetMyPlayer().SetData(0, GetTotalScore());
                GameState.Instance.PlayerChangedData -= OnPlayerFinished;

                StartCoroutine(Utils.GameUtils.GoToRankings());
            }
        }
        OnCupEnteredDebugCheck();
    }

    void SetupLevel(int levelIndex)
    {
        Lanzapato_LevelSettings levelToLoad = gameLevelList[levelIndex];

        score = 0;
        cupsController.SetupEnviroment(levelToLoad.cupRows, levelToLoad.tableLength); //Valores placeholder
        //Establece parametros de fuerza
        slingshotController.DeleteAndCreateProjectile();
    }

    void DestroyLevel()
    {
        score = 0;
        //Borra/reinicia proyectil lanzado
        cupsController.ResetEnviroment();
    }

    public void OnCupEntered()
    {
        score++;
        StartCoroutine(slingshotController.ResetProjectileOnHit());
    }

    public void OnCupEnteredDebugCheck()
    {
        if (scoreDebug)
        {
            score++;
            scoreDebug = false;
        }
    }

    private int GetTotalScore()
    {
        return scoreToPassLevel * currentLevel + score;
    }

    private void OnPlayerFinished(int id, NetworkDictionary <int, float> data)
    {
        GameState.Instance.PlayerChangedData -= OnPlayerFinished;
        
        // Aqui va lo que se ejecuta cuando un jugador termina, se debera editar
        Debug.Log("Un jugador gano");
        GameState.GetMyPlayer().SetData(0, GetTotalScore());
        StartCoroutine(Utils.GameUtils.GoToRankings());
    }
}