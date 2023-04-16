using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Utils;

//Al unirse un jugador a la sala, se le añade a la lista de jugadores, y se le comparte una "copia de este objeto"

public class RoomManager : NetworkBehaviour
//public class RoomManager : MonoBehaviour
{
    public byte life { get; set; }
    private string roomName;
    private List<PlayerBehaviour> playerList;
    private List<Minigame> gamesList;
    private int currentGame;

    // Start is called before the first frame update
    void Start()
    {
        currentGame = 0;
        PopulateGameList();
    }

    
    void PopulateGameList()
    {
        gamesList = new List<Minigame>();
        gamesList.AddRange(Resources.LoadAll<Minigame>("Minigames"));

        ListUtils.Shuffle(gamesList);
    }

    private void LoadNextGameScene()
    {
        if(currentGame + 1 != gamesList.Count)
        {
            currentGame++;
            SceneManager.LoadScene(gamesList[currentGame].gameSceneIndex);
        }
        else
        {
            //TODO: update when start and end scenes are created to get main scene flow ready
            SceneManager.LoadScene(0);
        }
    }
}
