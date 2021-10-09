package com.youngmonkeys.app.controller;

import com.tvd12.ezyfox.bean.annotation.EzyAutoBind;
import com.tvd12.ezyfox.core.annotation.EzyDoHandle;
import com.tvd12.ezyfox.core.annotation.EzyRequestController;
import com.tvd12.ezyfox.io.EzyLists;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.entity.EzyUser;
import com.tvd12.ezyfoxserver.support.factory.EzyResponseFactory;
import com.tvd12.gamebox.entity.Player;
import com.youngmonkeys.app.constant.Commands;
import com.youngmonkeys.app.game.GameRoom;
import com.youngmonkeys.app.game.shared.PlayerAttackData;
import com.youngmonkeys.app.game.shared.PlayerInputData;
import com.youngmonkeys.app.game.shared.PlayerSpawnData;
import com.youngmonkeys.app.request.JoinMMORoomRequest;
import com.youngmonkeys.app.request.PlayerAttackDataRequest;
import com.youngmonkeys.app.request.PlayerInputDataRequest;
import com.youngmonkeys.app.service.GamePlayService;
import com.youngmonkeys.app.service.LobbyService;
import com.youngmonkeys.app.service.RoomService;
import lombok.Setter;

import java.util.ArrayList;
import java.util.List;

@Setter
@EzyRequestController
public class GameRequestController extends EzyLoggable {
	
	@EzyAutoBind
	private LobbyService lobbyService;
	
	@EzyAutoBind
	private RoomService roomService;
	
	@EzyAutoBind
	private GamePlayService gamePlayService;
	
	@EzyAutoBind
	private EzyResponseFactory responseFactory;
	
	@EzyDoHandle(Commands.JOIN_LOBBY)
	public void joinLobby(EzyUser user) {
		logger.info("user {} join lobby room", user);
		
		lobbyService.addUser(user);
		long lobbyRoomId = lobbyService.getRoomId();
		
		responseFactory.newObjectResponse()
				.command(Commands.JOIN_LOBBY)
				.param("lobbyRoomId", lobbyRoomId)
				.user(user)
				.execute();
	}
	
	@EzyDoHandle(Commands.CREATE_MMO_ROOM)
	public void createMMORoom(EzyUser user) {
		logger.info("user {} create an MMO room", user);
		GameRoom room = roomService.newGameRoom(user);
		
		responseFactory.newObjectResponse()
				.command(Commands.CREATE_MMO_ROOM)
				.param("roomId", room.getId())
				.user(user)
				.execute();
	}
	
	@EzyDoHandle(Commands.GET_MMO_ROOM_ID_LIST)
	public void getMMORoomIdList(EzyUser user) {
		logger.info("user {} get MMO room list", user);
		List<Long> mmoRoomIdList = roomService.getMMORoomIdList();
		responseFactory.newArrayResponse()
				.command(Commands.GET_MMO_ROOM_ID_LIST)
				.param(mmoRoomIdList)
				.user(user)
				.execute();
	}
	
	@EzyDoHandle(Commands.GET_MMO_ROOM_PLAYERS)
	public void getMMORoomPlayers(EzyUser user) {
		logger.info("user {} getMMORoomPlayers", user);
		GameRoom currentRoom = (GameRoom) roomService.getCurrentRoom(user.getName());
		List<String> players = roomService.getRoomPlayerNames(currentRoom);
		Player master = roomService.getMaster(currentRoom);
		
		responseFactory.newObjectResponse()
				.command(Commands.GET_MMO_ROOM_PLAYERS)
				.param("players", players)
				.param("master", master.getName())
				.user(user)
				.execute();
	}
	
	@EzyDoHandle(Commands.JOIN_MMO_ROOM)
	public void joinMMORoom(EzyUser user, JoinMMORoomRequest request) {
		logger.info("user {} join room {}", user.getName(), request.getRoomId());
		long roomId = request.getRoomId();
		GameRoom room = roomService.playerJoinMMORoom(user.getName(), roomId);
		List<String> playerNames = roomService.getRoomPlayerNames(room);
		
		responseFactory.newObjectResponse()
				.command(Commands.JOIN_MMO_ROOM)
				.param("roomId", roomId)
				.user(user)
				.execute();
		
		responseFactory.newObjectResponse()
				.command(Commands.ANOTHER_JOIN_MMO_ROOM)
				.param("playerName", user.getName())
				.usernames(EzyLists.filter(playerNames, it -> !it.equals(user.getName())))
				.execute();
	}
	
	@EzyDoHandle(Commands.START_GAME)
	public void startGame(EzyUser user) {
		logger.info("user {} start game", user);
		GameRoom currentRoom = (GameRoom) roomService.getCurrentRoom(user.getName());
		List<String> playerNames = roomService.getRoomPlayerNames(currentRoom);
		
		List<PlayerSpawnData> data = gamePlayService.spawnPlayers(playerNames);
		
		responseFactory.newArrayResponse()
				.command(Commands.START_GAME)
				.data(data)
				.usernames(playerNames)
				.execute();
	}
	
	@EzyDoHandle(Commands.PLAYER_INPUT_DATA)
	public void handlePlayerInputData(EzyUser user, PlayerInputDataRequest request) {
		logger.info("user {} send input data {}", user.getName(), request);
		gamePlayService.handlePlayerInputData(user.getName(), new PlayerInputData(request.getK(), request.getT()), request.getR());
	}
	
	@EzyDoHandle(Commands.PLAYER_ATTACK_DATA)
	public void handlePlayerAttackData(EzyUser user, PlayerAttackDataRequest request) {
		logger.info("user {} send input data {}", user.getName(), request);
		// Handle attack
		List<String> playerNames = new ArrayList<>();
		List<String> playerBeingAttacked = new ArrayList<>();
		PlayerAttackData playerAttackData = new PlayerAttackData(request.getP(), request.getT());
		gamePlayService.handlePlayerAttack(
				user.getName(),
				playerAttackData,
				playerNames,
				playerBeingAttacked
		);
		
		// TODO: send to neighbourhood only
		responseFactory.newObjectResponse()
				.command(Commands.PLAYER_BEING_ATTACKED)
				.param("t", playerAttackData.getTime())
				.param("p", playerAttackData.getAttackPosition())
				.param("b", playerBeingAttacked)
				.usernames(playerNames)
				.execute();
	}
}
