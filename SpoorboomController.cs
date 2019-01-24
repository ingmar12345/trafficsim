using System.Linq;
using UnityEngine;

namespace Assets
{
	/// <summary>
	/// Controls everything related to the train crossing.
	/// </summary>
	public class SpoorboomController : MonoBehaviour
	{
		/// <summary>
		/// Direction the train is going to.
		/// </summary>
		private bool trainDirection;
		/// <summary>
		/// Reference to the train.
		/// </summary>
		private GameObject Train;
		/// <summary>
		/// Reference to all overpass beams.
		/// </summary>
		private GameObject[] Beams = new GameObject[0];
		/// <summary>
		/// Current angle of the beam.
		/// </summary>
		private float currentBeamAngle = 85f;
		/// <summary>
		/// Reference to all lights on the left.
		/// </summary>
		private GameObject[] lightsLeft = new GameObject[0];
		/// <summary>
		/// Reference to all lights on the right.
		/// </summary>
		private GameObject[] lightsRight = new GameObject[0];
		/// <summary>
		/// Reference to all noise boxes for toggling the audio.
		/// </summary>
		private GameObject[] bells = new GameObject[0];
		/// <summary>
		/// Timer for switching the light between blinking the left and right light.
		/// </summary>
		private float lightSwitchTimer;
		/// <summary>
		/// Current state of the overpass.
		/// </summary>
		public static TrainCrossState TrainCrossState { get; private set; }
		/// <summary>
		/// Speed of the train.
		/// </summary>
		private const float TrainSpeed = 12f;
		/// <summary>
		/// Time the lights take to switch between left and right.
		/// </summary>
		private const float LightSwitchTime = 0.35f;
		/// <summary>
		/// Speed the beams will go up or down.
		/// </summary>
		private const float BeamSpeed = 10f;

		/// <summary>
		/// Closes the train crossing, only when it is open.
		/// </summary>
		public static void CloseCrossing()
		{
			if (TrainCrossState == TrainCrossState.Open)
			{
				TrainCrossState = TrainCrossState.StartClosing;
			}
		}

		/// <summary>
		/// Opens the train crossing, only when it is closed.
		/// </summary>
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
