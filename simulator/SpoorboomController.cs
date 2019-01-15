using System.Linq;
using UnityEngine;

namespace Assets
{
	public class SpoorboomController : MonoBehaviour
	{
		private bool trainDirection;
		private GameObject Train;
		private GameObject[] Beams = new GameObject[0];
		private float currentBeamAngle = 85f;
		private GameObject[] lightsLeft = new GameObject[0];
		private GameObject[] lightsRight = new GameObject[0];
		private GameObject[] bells = new GameObject[0];
		private float lightSwitchTimer;
		public static TrainCrossState TrainCrossState { get; private set; }
		private const float TrainSpeed = 12f;
		private const float LightSwitchTime = 0.35f;
		private const float BeamSpeed = 10f;

		public static void CloseCrossing()
		{
			if (TrainCrossState == TrainCrossState.Open)
			{
				TrainCrossState = TrainCrossState.StartClosing;
			}
		}

		public static void OpenCrossing()
		{
			if (TrainCrossState == TrainCrossState.Closed)
			{
				TrainCrossState = TrainCrossState.StartOpening;
			}
		}

		// Use this for initialization
		void Start() {
			this.Beams = GameObject.FindGameObjectsWithTag("Spoorboom");
			this.lightsLeft = GameObject.FindGameObjectsWithTag("spoorlichtlinks");
			this.lightsRight = GameObject.FindGameObjectsWithTag("spoorlichtrechts");
			this.bells = GameObject.FindGameObjectsWithTag("NoiseBox");
			this.Train = GameObject.FindWithTag("Train");
			// Disable on startup to hide from view.
			this.Train.SetActive(false);
		}

		// Update is called once per frame
		void Update() {
			// Don't do anything in a rest state.
			if (TrainCrossState == TrainCrossState.Open)
			{
				return;
			}
			else if (TrainCrossState == TrainCrossState.Closed)
			{
				// Only update the lights when closed.
				UpdateTrainLights();
				MoveTrain();
				return;
			}

			if (TrainCrossState == TrainCrossState.StartClosing)
			{
				foreach (GameObject light in lightsRight)
				{
					Light l = light.GetComponent<Light>();
					l.enabled = true;
				}

				foreach (GameObject bell in bells)
				{
					AudioSource source = bell.GetComponent<AudioSource>();
					source.enabled = true;
				}

				lightSwitchTimer = 0f;
				TrainCrossState = TrainCrossState.Closing;
			}
			else if (TrainCrossState == TrainCrossState.StartOpening)
			{
				TrainCrossState = TrainCrossState.Opening;
			}

			UpdateTrainLights();
			UpdateBeams();

			if (TrainCrossState == TrainCrossState.Closing && currentBeamAngle <= 0)
			{
				TrainCrossState = TrainCrossState.Closed;
				currentBeamAngle = 0f;
				Train.SetActive(true);
			}
			else if (TrainCrossState == TrainCrossState.Opening && currentBeamAngle >= 85)
			{
				TrainCrossState = TrainCrossState.Open;
				// Only dim lights after the beams have returned to their upright position.
				foreach (GameObject light in lightsLeft.Concat(lightsRight))
				{
					Light l = light.GetComponent<Light>();
					l.enabled = false;
				}

				// Only quieten after the beams have returned to their upright position.
				foreach (GameObject bell in bells)
				{
					AudioSource source = bell.GetComponent<AudioSource>();
					source.enabled = false;
				}

				currentBeamAngle = 85f;
			}
		}

		private void UpdateBeams()
		{
			float beamSpeed = TrainCrossState == TrainCrossState.Opening ? BeamSpeed : -BeamSpeed;

			Vector3 vector3 = new Vector3(0, 0, beamSpeed * Time.deltaTime);

			foreach (GameObject beam in Beams)
			{
				beam.transform.Rotate(vector3);
			}

			currentBeamAngle += vector3.z;
		}

		private void UpdateTrainLights()
		{
			if (lightSwitchTimer <= LightSwitchTime)
			{
				lightSwitchTimer += Time.deltaTime;
				return;
			}
			else
			{
				lightSwitchTimer = 0f;
			}
			foreach (GameObject light in lightsLeft.Concat(lightsRight))
			{
				Light l = light.GetComponent<Light>();
				l.enabled = !l.enabled;
			}
		}

		private void MoveTrain()
		{
			float speed = TrainSpeed * Time.deltaTime;
			if (!trainDirection)
			{
				speed = -speed;
			}

			Train.transform.Translate(new Vector3(speed, 0, 0), Space.Self);

			// Despawn and reverse direction when beyond x -15 or x 150.
            if (Train.transform.position.x < -15f || Train.transform.position.x > 150f)
			{
                Train.SetActive(false);
				trainDirection = !trainDirection;

				// Reset position to ensure the train doesn't stay invisible on the return trip.
				Vector3 transformPosition = Train.transform.position;
				if (Train.transform.position.x > 150f)
				{
					transformPosition.x = 150f;
				}
				else if (Train.transform.position.x < -15f)
				{
					transformPosition.x = -15f;
				}
				Train.transform.position = transformPosition;
			}
		}
	}
}
