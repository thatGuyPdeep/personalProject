using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class JoinGame : MonoBehaviour
{
    List<GameObject> roomList = new List<GameObject>();

    [SerializeField]
    private Text status;

    [SerializeField]
    private GameObject roomListItemPrefab;

    [SerializeField]
    private Transform roomListParent;

    private NetworkManager networkManager;

    void Start()
    {
        networkManager = NetworkManager.singleton;
        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        RefreshRoomList();
    }

    public void RefreshRoomList()
    {
        networkManager.matchMaker.ListMatches(0, 20, "",false, 0, 0, OnMatchList);
        status.text = "Loading...";
    }

    public void OnMatchList (ListMatchResponse matchList)
    {
        status.text = "";

        if (matchList = null)
        {
            status.text = "Couldn't get room list.";
            return;
        }

        ClearRoomList();
        foreach (MatchDesc match in matchList.matches)
        {
            GameObject _roomListItemGo = Instantiate(roomListItemPrefab);
            _roomListItemGo.transform.SetParent(roomListParent);
            //Have a component sit on the gameobject that will 
            //take care of setting up the name/amount of users 
            //as well as setting up a callback function that will join the game.
        }
    }

    void ClearRoomList()
    {
        for(int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }

        roomList.Clear();
    }

}
