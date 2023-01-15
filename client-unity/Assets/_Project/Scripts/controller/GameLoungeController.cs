﻿using System.Collections.Generic;
using com.tvd12.ezyfoxserver.client.entity;
using com.tvd12.ezyfoxserver.client.support;
using com.tvd12.ezyfoxserver.client.unity;
using UnityEngine;
using UnityEngine.Events;

public class GameLoungeController : EzyDefaultController
{
    [SerializeField]
    private UnityEvent<List<PlayerModel>> updateRoomPlayersEvent;
    
    [SerializeField]
    private UnityEvent<List<PlayerSpawnInfoModel>> gameStartedEvent;

    private void Awake()
    {
        base.Awake();
        addHandler<EzyObject>(Commands.GET_MMO_ROOM_PLAYERS, OnGetMMORoomPlayersResponse);
        addHandler<EzyObject>(Commands.ANOTHER_JOIN_MMO_ROOM, OnAnotherJoinMMORoom);
        addHandler<EzyObject>(Commands.ANOTHER_EXIT_MMO_ROOM, OnAnotherExitMMORoom);
        addHandler<EzyArray>(Commands.START_GAME, OnGameStarted);
        GetMMORoomPlayers();
    }

    private void GetMMORoomPlayers()
    {
        SocketRequest.GetInstance()
            .SendGetMMORoomPlayersRequest();
    }

    private void OnGetMMORoomPlayersResponse(EzyAppProxy proxy, EzyObject data) 
    {
        List<string> playerNames = data.get<EzyArray>("players").toList<string>();
        string masterName = data.get<string>("master");
        logger.debug("OnGetMMORoomPlayersResponse");
        logger.debug("Player Names: " + string.Join(",", playerNames));
        logger.debug("Master Name: " + masterName);
        List<PlayerModel> players = new();
        foreach (string playerName in playerNames)
        {
            bool isMaster = playerName.Equals(masterName);
            players.Add(new PlayerModel(playerName, isMaster));
        }
        updateRoomPlayersEvent?.Invoke(players);
    }

    private void OnAnotherJoinMMORoom(EzyAppProxy proxy, EzyObject data) 
    {
        string anotherPlayerName = data.get<string>("playerName");
        logger.debug("OnAnotherJoinMMORoom anotherPlayerName = " + anotherPlayerName);
        GetMMORoomPlayers();
    }

    private void OnAnotherExitMMORoom(EzyAppProxy proxy, EzyObject data) 
    {
        string anotherName = data.get<string>("playerName");
        logger.debug("OnAnotherExitMMORoom anotherPlayerName = " + anotherName);
        GetMMORoomPlayers();
    }

    private void OnGameStarted(EzyAppProxy proxy, EzyArray data)
    { 
        logger.debug("OnGameStart");
        List<PlayerSpawnInfoModel> spawnInfos = new List<PlayerSpawnInfoModel>();

        for (int i = 0; i < data.size(); i++)
        {
            EzyObject item = data.get<EzyObject>(i);
            string playerName = item.get<string>("playerName");
            EzyArray position = item.get<EzyArray>("position");
            EzyArray color = item.get<EzyArray>("color");
            spawnInfos.Add(
                new PlayerSpawnInfoModel(
                    playerName,
                    new Vector3(position.get<float>(0), position.get<float>(1), position.get<float>(2)),
                    new Vector3(color.get<float>(0), color.get<float>(1), color.get<float>(2))
                )
            );
        }
        gameStartedEvent?.Invoke(spawnInfos);
    }

    #region public methods

    public void StartGame() {
        SocketRequest.GetInstance()
            .SendStartGameRequest();
    }

    #endregion
}
