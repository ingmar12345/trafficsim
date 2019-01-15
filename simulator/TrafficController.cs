using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using WebSocketSharp;
using Random = UnityEngine.Random;

namespace Assets
{
	/// <inheritdoc />
	/// <summary>
	/// Bound to Junction -> runs when Junction is active (which is immediately)
	/// </summary>
	public class TrafficController : MonoBehaviour
	{
		private TrafficLightManipulator Manipulator;
		private TrafficWaitStore WaitStore;
		private WebSocket SocketClient;
		private float trainCntr;
		private float cntr;
		private float TrainSpawnLimit = 45f;
		private bool trainOpen = true;
		private bool open = true;

		// Use this for initialization
		public void Start()
		{
			this.WaitStore = new TrafficWaitStore();
			// Example uri: ws://localhost:5678
			string uri = Utilities.getWebSocketPath();
			// Uncomment below when testing
			//uri = "ws://141.252.236.43:5678";

			Manipulator = new TrafficLightManipulator();
			Manipulator.PrepareTrafficLights();

			Debug.Log("Set to use " + uri);
			SocketClient = new WebSocket(uri);
			SocketClient.OnMessage += SocketClientOnOnMessage;
			try
			{
				SocketClient.Connect();
				// Init request (for controllers that need some kind of signal)
				SocketClient.Send(Encoding.UTF8.GetBytes("[]"));
				Debug.Log("Connected to " + uri);
			}
			catch (Exception ex)
			{
				if (ex is SocketException || ex is InvalidOperationException)
				{
					Debug.Log("Failure connecting to " + uri);
					SocketClient = null;
				}
				else
				{
					throw;
				}
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SocketClientOnOnMessage(object sender, MessageEventArgs e)
		{
			JArray lightChanges;
			if (e.IsText)
			{
				lightChanges = JArray.Parse(e.Data);
			}
			else if (e.IsBinary)
			{
				lightChanges = JArray.Parse(Encoding.UTF8.GetString(e.RawData));
            }
			else
			{
				Debug.Log("Did not receive text nor bytes.");
				return;
			}

			foreach (JToken change in lightChanges)
			{
				string light = change["light"].ToString();
				string status = change["status"].ToString();
				if (light.IsNullOrWhiteSpace() || status.IsNullOrWhiteSpace())
				{
					continue;
				}

				Manipulator.Queue(light, status);
			}
		}

		// Update is called once per frame
		public void Update()
		{
			HandleDeeldeliersMusic();
			Manipulator.ProcessQueue();

			// Don't attempt to send when the connection is dead.
			if (SocketClient == null || SocketClient.ReadyState != WebSocketState.Open)
			{
				// Debug for trains
				if (trainCntr > 90f)
				{
					trainCntr = 0f;
					string color = trainOpen ? "GREEN" : "RED";
					//Debug.Log("Set to " + color);
					Manipulator.Queue("E1", color);
					trainOpen = !trainOpen;
				}
				else if (cntr > Random.Range(0, 6))
				{
					cntr = 0f;
					string color = open ? "GREEN" : "RED";
					Manipulator.Queue(Dictionaries.Lights.Random(), color);
					open = !open;
				}
				else
				{
					trainCntr += Time.deltaTime;
					cntr += Time.deltaTime;
				}

				return;
			}

			List<string> lights = this.WaitStore.LightsWithTraffic.Distinct().ToList();

			if (trainCntr > TrainSpawnLimit)
			{
				trainCntr = 0f;
				TrainSpawnLimit = 120f;
				lights.Add("E1");
				// Some controllers don't use E1 anymore. Use F1 and F2 instead.
				lights.Add(trainOpen ? "F1" : "F2");
				trainOpen = !trainOpen;
			}
			else
			{
				trainCntr += Time.deltaTime;
			}

			if (lights.Count > 0)
			{
				// Update controller with new traffic.
				SocketClient.Send(
					Encoding.UTF8.GetBytes(
						JsonConvert.SerializeObject(lights)
					)
				);
			}

			this.WaitStore.LightsWithTraffic = new List<string>();
		}

		/// <summary>
		/// Toggles band music if a band is present.
		/// </summary>
		private void HandleDeeldeliersMusic()
		{
			GameObject[] deeldeliers = GameObject.FindGameObjectsWithTag("Deeldelier");
			AudioSource deeldelierMusic = GetComponent<AudioSource>();
			if (deeldeliers.Length > 0)
			{
				if (!deeldelierMusic.isPlaying)
				{
					deeldelierMusic.Play();
				}
			}
			else
			{
				if (deeldelierMusic.isPlaying)
				{
					deeldelierMusic.Stop();
				}
			}
		}

		public void OnApplicationQuit()
		{
			if (SocketClient == null || SocketClient.ReadyState != WebSocketState.Open)
			{
				return;
			}
			SocketClient.Close();
		}
	}
}
