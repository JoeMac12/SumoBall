using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
	public float moveSpeed = 5f;
	public float pushForce = 5f;

	private Rigidbody rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	void Update()
	{
		if (!IsOwner) return;

		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		Vector3 movement = new Vector3(horizontal, 0f, vertical) * moveSpeed;
		rb.AddForce(movement);
	}

	void OnCollisionStay(Collision collision)
	{
		if (!IsOwner) return;
		if (collision.gameObject.CompareTag("PlayerBall"))
		{
			Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();
			if (otherRb != null)
			{
				Vector3 pushDir = (collision.transform.position - transform.position).normalized;
				otherRb.AddForce(pushDir * pushForce, ForceMode.Impulse);
			}
		}
	}
}
