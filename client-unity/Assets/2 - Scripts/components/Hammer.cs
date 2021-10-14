using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hammer : MonoBehaviour
{
	public ClientPlayer clientPlayer;
	private bool hasEntered;
	HashSet<string> playersBeingAttacked = new HashSet<string>();

	public static UnityAction<string, Vector3, int> playerHitEvent;

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			if (!playersBeingAttacked.Contains(other.gameObject.name))
			{
				Debug.Log("OnCollisionEnter");
				Debug.Log(other.gameObject.name);
				playersBeingAttacked.Add(other.gameObject.name);
				// Invoke event
				playerHitEvent?.Invoke(other.gameObject.name, clientPlayer.AttackPoint.position, clientPlayer.ClientTick);
			}
		}
	}

	private void FixedUpdate()
	{
		if (!clientPlayer.Anim.GetCurrentAnimatorStateInfo(0).IsName("Slash") && playersBeingAttacked.Count > 0)
		{
			playersBeingAttacked.Clear();
		}
	}
}
