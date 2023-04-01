using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class SimonMessages : MonoBehaviour
{ 
    [SerializeField]
    private GameObject menusObject;
    private SimonSceneController menusController;
    [SerializeField]
    private SimonGame gameController;
    private AudioSource audioSource;
    [SerializeField] private AudioClip failClip;
    [SerializeField] private AudioClip successClip;

    private UIDocument doc;
    private Button Boton;

    public string texto;

    private void Awake()
    {
        menusController = menusObject.GetComponent<SimonSceneController>();
        audioSource = menusObject.GetComponent<AudioSource>();
        doc = GetComponent<UIDocument>();
    }

    void OnEnable()
    {
        audioSource.Stop();
        doc.rootVisualElement.Q<Label>("Texto").text = texto;
        
        if (texto.Contains("perdido"))
            audioSource.clip = failClip;
        else
            audioSource.clip = successClip;
        StartCoroutine(PlaySound());
        
        Boton = doc.rootVisualElement.Q<Button>("Boton");
        Boton.clicked += ButtonOnClicked;
        
        if (gameController.finished)
            Boton.text = "Terminar";
    }

    private IEnumerator PlaySound()
    {
       audioSource.Play();
       yield return new WaitForSeconds(audioSource.clip.length);
    }

    private void ButtonOnClicked()
    {
        if (gameController.finished)
        {
            GameState.GetMyPlayer().SetData(0, gameController.failedRounds);
            GameState.GetMyPlayer().SetData(1, gameController.totalTime);
            
            if(GameState.isServer)
                GameState.Instance.PlayerHasChangedData(GameState.GetMyPlayer().playerId, GameState.GetMyPlayer().data);
            
            return;
            // TODO: ir a rankings
        }
        
        menusController.Jugar();
    }
}