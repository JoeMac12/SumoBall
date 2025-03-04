using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnManager : MonoBehaviour
{
	[Header("Player Prefab")]
	[SerializeField] private GameObject playerPrefab;

	[Header("Spawn Points")]
	[SerializeField] private Transform[] spawnPoints;

	private void Start()
	{
		NetworkManager.Singleton.OnServerStarted += OnServerStarted;
		NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
	}

	private void OnServerStarted()
	{
		SpawnPlayer(NetworkManager.Singleton.LocalClientId, 0);
	}

	private void OnClientConnected(ulong clientId)
	{
		if (!NetworkManager.Singleton.IsServer) return;

		if (clientId == NetworkManager.Singleton.LocalClientId)
			return;

		SpawnPlayer(clientId, 1);
	}

	private void SpawnPlayer(ulong clientId, int spawnIndex)
	{
		if (spawnIndex < 0 || spawnIndex >= spawnPoints.Length)
		{
			return;
		}

		GameObject playerObj = Instantiate(
			playerPrefab,
			spawnPoints[spawnIndex].position,
			spawnPoints[spawnIndex].rotation
		);

		NetworkObject networkObject = playerObj.GetComponent<NetworkObject>();
		networkObject.SpawnAsPlayerObject(clientId);
	}
}
