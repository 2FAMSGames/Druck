using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CupsController : MonoBehaviour
{
    CupsPlacer cupsPlacer;
    GameManagerController gameManager;

    // Start is called before the first frame update
    void Start()
    {
        cupsPlacer = GetComponent<CupsPlacer>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerController>();
    }

    public void SetupEnviroment(int nRowsCups, float tableLength)
    {
        StartCoroutine(SetupEnviromentCoroutine(nRowsCups, tableLength));   
    }

    IEnumerator SetupEnviromentCoroutine(int nRowsCups, float tableLength)
    {
        yield return new WaitUntil(() => cupsPlacer.isInitialized);
        cupsPlacer.SetTableLength(tableLength);
        cupsPlacer.PlaceCups(nRowsCups);
    }

    public void ResetEnviroment()
    {
        cupsPlacer.DestroyCups();
    }

    public void OnCupEntered()
    {
        gameManager.OnCupEntered();
    }
}
