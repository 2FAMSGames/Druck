using UnityEngine;

public class SimonSceneController: MonoBehaviour
{
    [SerializeField]
    private GameObject InfoScreen;
    [SerializeField]
    private GameObject SceneScreen;
    [SerializeField]
    private GameObject MessageScreen;

    void OnEnable()
    {
        GameState.GetMyPlayer().ResetData();
        
        InfoScreen.SetActive(true);
        SceneScreen.SetActive(false);
        MessageScreen.SetActive(false);
    }

    public void Jugar()
    {
        InfoScreen.SetActive(false);
        SceneScreen.SetActive(true);
        MessageScreen.SetActive(false);
    }

    public void ShowMessage()
    {
        InfoScreen.SetActive(false);
        SceneScreen.SetActive(false);
        MessageScreen.SetActive(true);
    }
    
}
