using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Build.Player;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerItemController
{
    Label playerNameLabel;

    public void SetVisualElement(VisualElement ve)
    {
        playerNameLabel = ve.Q<Label>("PlayerName");
    }

    public void SetPlayerData(PlayerBehaviour pd)
    {
        // PlayerBehaviour needs to be Spawned first, next line causes error.
        //playerNameLabel.text = pd.playerName;
        playerNameLabel.text = "dummy";
    }
}
