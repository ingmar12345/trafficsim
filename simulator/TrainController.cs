using System.Linq;
using UnityEngine;

namespace Assets
{
	public class TrainController : MonoBehaviour
	{
		private GameObject[] lightsLinks = new GameObject[0];
		private GameObject[] lightsRechts = new GameObject[0];
		private int counter = 0;

		// Use this for initialization
		void Start()
		{
			this.lightsLinks = GameObject.FindGameObjectsWithTag("spoorlichtlinks");
			this.lightsRechts = GameObject.FindGameObjectsWithTag("spoorlichtrechts");
			foreach (GameObject light in lightsLinks)
			{
				Light l = light.GetComponent<Light>();
				l.enabled = true;
			}
		}

		// Update is called once per frame
		void Update()
		{
			if (counter < 20)
			{
				counter++;
				return;
			}
			else
			{
				counter = 0;
			}
			foreach (GameObject light in lightsLinks.Concat(lightsRechts))
			{
				Light l = light.GetComponent<Light>();
				l.enabled = !l.enabled;
			}
		}
	}
}
