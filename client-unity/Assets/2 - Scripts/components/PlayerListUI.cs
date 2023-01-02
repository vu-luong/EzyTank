using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListUI : MonoBehaviour
{
	[SerializeField]
	private GameObject playerButtonPrefab;

	public void UpdateRoomPlayers()
	{
		List<PlayerModel> players = RoomManager.GetInstance().CurrentRoomPlayers;
		Debug.Log("PlayerList.UpdateRoomPlayers: " + string.Join(",", players));
		gameObject.GetComponent<ListUI>().RemoveAllItems();
		foreach (PlayerModel player in players)

		{
			GameObject go = gameObject.GetComponent<ListUI>().AddItem(playerButtonPrefab);
			string displayName = player.IsMaster ?
				player.PlayerName + "(Master)" : player.PlayerName;
			go.GetComponentInChildren<Text>().text = displayName;
		}
	}
}
