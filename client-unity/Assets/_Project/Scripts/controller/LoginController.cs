﻿using com.tvd12.ezyfoxserver.client.support;
using com.tvd12.ezyfoxserver.client.unity;
using UnityEngine;
using UnityEngine.Events;
using Object = System.Object;

public class LoginController : EzyDefaultController
{
	[SerializeField]
	private StringVariable username;
	
	[SerializeField]
	private StringVariable password;
	
	[SerializeField]
	private string host;
	
	[SerializeField]
	private int udpPort;

	[SerializeField]
	private UnityEvent<string> myPlayerJoinedLobbyEvent;

	[SerializeField]
	private SocketConfigVariable socketConfig;

	private void Awake()
	{
		AddHandler<Object>(Commands.JOIN_LOBBY, OnJoinedLobby);
	}

	public void Login()
	{
		// Login to socket server
		EzyDefaultSocketManager.GetInstance()
			.Login(host, username.Value, password.Value, HandleLoginSuccess, HandleAppAccessed);
	}
	
	private void HandleLoginSuccess(EzySocketProxy proxy, object data)
	{
		logger.debug("Log in successfully");
		proxy.getClient().udpConnect(udpPort);
	}
    
	private void HandleAppAccessed(EzyAppProxy proxy, object data)
	{
		logger.debug("App access successfully");
		SocketRequest.getInstance().SendJoinLobbyRequest();
	}

	void OnJoinedLobby(EzyAppProxy appProxy, Object data)
	{
		myPlayerJoinedLobbyEvent?.Invoke(username.Value);
	}
}