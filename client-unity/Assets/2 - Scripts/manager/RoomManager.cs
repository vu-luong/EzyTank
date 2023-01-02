﻿using System.Collections.Generic;
using com.tvd12.ezyfoxserver.client.util;

public class RoomManager : EzyLoggable
{
    private static readonly RoomManager INSTANCE = new RoomManager();
    private long currentRoomId;

    public long CurrentRoomId { get => currentRoomId; set => currentRoomId = value; }
    public List<PlayerModel> CurrentRoomPlayers { get; private set; }

    public RoomManager()
    {
    }

    public static RoomManager GetInstance()
    {
        return INSTANCE;
    }

    public void SetCurrentRoomPlayers(List<string> playerNames, string master)
    {
        logger.debug("SetCurrentRoomPlayers");
        CurrentRoomPlayers = new List<PlayerModel>();
        foreach (string playerName in playerNames)
        {
            PlayerModel player;
            if (playerName.Equals(GameManager.getInstance().MyPlayer.PlayerName))
            {
                player = GameManager.getInstance().MyPlayer;
            } else
            { 
                player = new PlayerModel(playerName);
            }
            player.IsMaster = (playerName.Equals(master));
            CurrentRoomPlayers.Add(player);
        }
    }

    public void SetCurrentRoomId(long roomId)
    {
        logger.debug("SetCurrentRoomId");
        currentRoomId = roomId;
    }

    public void ExitCurrentRoom()
    { 
        // TODO
    }
}
