using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody), typeof(NetworkObject))]
public class PlayerController : NetworkBehaviour
{
	[Header("Movement Settings")]
	public float speed = 5f;
	public float pushForce = 10f;
	private Rigidbody rb;

	private Vector3 spawnPosition = new Vector3(-12.5f, 5f, 12.5f);

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		rb.interpolation = RigidbodyInterpolation.Interpolate;
	}

	private void FixedUpdate()
	{
		if (!IsOwner) return;

		float moveX = Input.GetAxis("Horizontal");
		float moveZ = Input.GetAxis("Vertical");
		Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

		MoveServerRpc(moveDirection);
	}

	[ServerRpc]
	private void MoveServerRpc(Vector3 direction, ServerRpcParams rpcParams = default)
	{
		ulong clientId = rpcParams.Receive.SenderClientId;

		if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out NetworkClient client))
		{
			Rigidbody playerRb = client.PlayerObject.GetComponent<Rigidbody>();

			if (playerRb != null)
			{
				playerRb.AddForce(direction * speed, ForceMode.Force);
				UpdatePositionClientRpc(playerRb.position, playerRb.velocity);
			}
		}
	}

	[ClientRpc]
	private void UpdatePositionClientRpc(Vector3 position, Vector3 velocity)
	{
		if (!IsOwner)
		{
			rb.velocity = velocity;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!IsServer) return;

		PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
		if (otherPlayer != null && otherPlayer != this)
		{
			Rigidbody otherRb = otherPlayer.GetComponent<Rigidbody>();
			if (otherRb != null)
			{
				Vector3 pushDirection = (otherPlayer.transform.position - transform.position).normalized;
				otherRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!IsServer) return;

		if (other.CompareTag("FallZone"))
		{
			rb.position = spawnPosition;
			rb.velocity = Vector3.zero;
			UpdatePositionClientRpc(rb.position, rb.velocity);
		}
	}
}
