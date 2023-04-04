using UnityEngine;

public class WaitPat : MonoBehaviour
{
    [SerializeField]
    private GameObject menusObject;
    private PatonaryMenuController menusController;

    public bool waitngEnded = true;
    private int WaitngScreen;

    void OnEnable()
    {
        menusController = menusObject.GetComponent<PatonaryMenuController>();
        //if everyone´s pic/guess/vote is ready then
        WaitngScreen = PlayerPrefs.GetInt("WaitngScreen");
    }

    private void Update()
    {
        if (waitngEnded)
        {
            WaitingEnded();
        }
    }

    private void WaitingEnded()
    {
        Debug.Log("Waiting ended");
        //menusController.WaitEnded();
        if (WaitngScreen == 1)
        {
            // TODO: esperar hasta que re haya recibido la imagen para ir a adivinar
            
            Debug.Log("Everyone have drawn");
            menusController.GoToGuess();
        }
        else if (WaitngScreen == 2)
        {
            Debug.Log("Everyone have guessed");
            menusController.GoToVote();
        }
        else if (WaitngScreen == 3)
        {
            Debug.Log("Everyone have voted");
            
            // TODO: no debería jugar otro? sino en los rankings sólo 1 va a tener puntuación.
            menusController.GoToRankings();
        }
    }
    
    // TODO: A el que dibuja el envía la textura a B, B devuelve la palabra de lo que cree que es
    // TODO: y A dice que sí o que no, luego finaliza el juego. 
}
