using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AHuevoSceneController : MonoBehaviour {
    
    [SerializeField]
    private GameObject InfoAHuevo;
    [SerializeField]
    private GameObject AHuevoScene;


    void OnEnable()
    {
        IntroAHuevo();
    }

    public void IntroAHuevo()
    {
        InfoAHuevo.SetActive(true);
        AHuevoScene.SetActive(false);
    }

    public void GoToTask()
    {
        InfoAHuevo.SetActive(false);
        AHuevoScene.SetActive(true);
    }
}



