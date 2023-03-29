using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerListController
{
    VisualTreeAsset listEntryTemplate;

    ListView playerList;

    List<PlayerBehaviour> playerDataList;

    void PopulatePlayerList()
    {
        playerDataList = new List<PlayerBehaviour>();
        playerDataList.AddRange(Resources.LoadAll<PlayerBehaviour>("SamplePlayers"));
    }

    //Asociates father element that contains a list with item template
    public void InitPlayerList(VisualTreeAsset listElementTemplate, ListView playerListTemplate)
    {
        PopulatePlayerList();

        listEntryTemplate = listElementTemplate;

        playerList = playerListTemplate;

        FillPlayerList();
    }

    //Set functions for filling the list
    void FillPlayerList()
    {
        playerList.makeItem = () =>
        {
            var newListEntry = listEntryTemplate.Instantiate();

            var newListEntryLogic = new PlayerItemController();
            newListEntry.userData = newListEntryLogic;
            newListEntryLogic.SetVisualElement(newListEntry);

            return newListEntry;
        };

        playerList.bindItem = (item, index) =>
        {
            (item.userData as PlayerItemController).SetPlayerData(playerDataList[index]);
        };

        playerList.itemsSource = playerDataList;
    }
}
