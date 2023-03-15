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
    int score = 0;
    [SerializeField]
    int currentLevel = 0;
    int nTotalLevels;

    // Start is called before the first frame update
    void Start()
    {
        slingshotController = slingshot.GetComponent<SlingshotController>();
        cupsController = cupsContainer.GetComponent<CupsController>();
        nTotalLevels = gameLevelList.Count;
        SetupLevel(currentLevel);
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
}