using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SimonMessages : MonoBehaviour
{ 
    [SerializeField] private GameObject menusObject;
    
    private SimonSceneController menusController;
    
    [SerializeField] private SimonGame gameController;
    
    [SerializeField] private AudioClip failClip;
    [SerializeField] private AudioClip successClip;

    private UIDocument doc;
    private Button Boton;

    public string texto;
    private bool alreadyClicked;

    private void Awake()
    {
        menusController = menusObject.GetComponent<SimonSceneController>();
        doc = GetComponent<UIDocument>();
    }

    public void Update()
    {
        if(gameController.finished)
            CheckBarrier();
    }

    private void CheckBarrier()
    {
        Debug.Log(GameState.GetMyPlayer().playerId + " checks barrier");
        var players = PlayerRegistry.Instance.ObjectByRef;
        var inBarrier = players.Where(p => p.Value.data[5] == 1).ToList();
        Debug.Log("barrier 1 has " + inBarrier.Count);
        
        if (inBarrier.Count != GameState.CountPlayers) return;
        
        StartCoroutine(Utils.GameUtils.GoToRankings());
    }

    void OnEnable()
    {
        alreadyClicked = false;
        GameState.Instance.audioSource.Stop();
        doc.rootVisualElement.Q<Label>("Texto").text = texto;
        
        if (texto.Contains("perdido"))
            GameState.Instance.audioSource.clip = failClip;
        else
            GameState.Instance.audioSource.clip = successClip;
        StartCoroutine(PlaySound());
        
        Boton = doc.rootVisualElement.Q<Button>("Boton");
        Boton.clicked += ButtonOnClicked;
    }

    private IEnumerator PlaySound()
    {
        GameState.Instance.audioSource.pitch = 1.0f;
        GameState.Instance.audioSource.Play();
        yield return new WaitForSeconds(GameState.Instance.audioSource.clip.length);
    }

    private void ButtonOnClicked()
    {
        if (gameController.finished)
        {
            if (!alreadyClicked)
            {
                texto = "\n\nCanción completada!\n\nAciertos: " + (4 - gameController.failedRounds);
                doc.rootVisualElement.Q<Label>("Texto").text = texto;
                Boton.text = "Terminar";
                alreadyClicked = true;
            }
            else
            {
                GameState.GetMyPlayer().SetData(0, gameController.failedRounds == 0 ? -1 : gameController.failedRounds);
                GameState.GetMyPlayer().SetData(1, gameController.totalTime);
                Debug.Log("fin");
           
                Boton.SetEnabled(false);
                Boton.text = "Esperando...";
                
                IAmInBarrier();
            }
        }
        else
        {
            Debug.Log("seguir");
            menusController.Jugar();
        }
        
    }
    
    public void IAmInBarrier()
    {
        Debug.Log(GameState.GetMyPlayer().playerId + " is at barrier");
        // 5,6 y 7 son las barreras, no usar para otra cosa.
        GameState.GetMyPlayer().SetData(5, 1);
        //OnPlayerChangedData(GameState.GetMyPlayer().playerId, GameState.GetMyPlayer().data);
    }

}