using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class NetworkUI : MonoBehaviour
{
	[SerializeField] private Button hostButton;
	[SerializeField] private Button clientButton;
	[SerializeField] private InputField ipInputField;

	private void Start()
	{
		hostButton.onClick.AddListener(OnHostClicked);
		clientButton.onClick.AddListener(OnClientClicked);
	}

	private void OnHostClicked()
	{
		var unityTransport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
		unityTransport.ConnectionData.Address = "127.0.0.1";
		unityTransport.ConnectionData.Port = 7777;

		NetworkManager.Singleton.StartHost();
	}

	private void OnClientClicked()
	{
		var unityTransport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
		unityTransport.ConnectionData.Address = ipInputField.text;
		unityTransport.ConnectionData.Port = 7777;

		NetworkManager.Singleton.StartClient();
	}
}
