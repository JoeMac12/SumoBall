using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FallTrigger : NetworkBehaviour
{
	[Header("Respawn Points")]
	[SerializeField] private Transform[] respawnPoints;

	private void OnTriggerEnter(Collider other)
	{
		if (!IsServer) return;

		NetworkObject netObj = other.GetComponent<NetworkObject>();
		if (netObj != null && netObj.IsPlayerObject)
		{
			ulong clientId = netObj.OwnerClientId;

			int spawnIndex = (clientId == NetworkManager.Singleton.LocalClientId) ? 0 : 1;

			TeleportPlayer(other.transform, spawnIndex);
		}
	}

	private void TeleportPlayer(Transform playerTransform, int spawnIndex)
	{
		if (spawnIndex < 0 || spawnIndex >= respawnPoints.Length)
		{
			spawnIndex = 0;
		}

		playerTransform.position = respawnPoints[spawnIndex].position;
		playerTransform.rotation = respawnPoints[spawnIndex].rotation;

		if (playerTransform.TryGetComponent<Rigidbody>(out var rb))
		{
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}
	}
}
