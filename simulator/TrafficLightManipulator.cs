using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets
{
    public class TrafficLightManipulator
    {
        /// <summary>
        /// Colour code for red lights
        /// </summary>
        public const string RED = "RED";
        /// <summary>
        /// Colour code for orange lights
        /// </summary>
        public const string ORANGE = "ORANGE";
        /// <summary>
        /// Colour code for orange lights to ensure the same code is used consistency.
        /// </summary>
        public const string YELLOW = ORANGE;
        /// <summary>
        /// Colour code for green lights
        /// </summary>
        public const string GREEN = "GREEN";

        public static Color BusRedColor = new Color(255, 0, 0);
        public static Color BusOrangeColor = new Color(255, 150, 0);
        public static Color BusGreenColor = new Color(0, 255, 0);

        /// <summary>
        /// List of all traffic light applying to cars.
        /// </summary>
        public static readonly string[] CarLights =
        {
            "A1",
            "A2",
            "A3",
            "A4",
            "A5",
            "A6",
            "A7",
            "A8",
            "A9",
            "A10"
        };

        /// <summary>
        /// List of all traffic light applying to cyclists.
        /// </summary>
        public static readonly string[] CyclistLights =
        {
            "B1",
            "B2",
            "B3"
        };

        /// <summary>
        /// List of all traffic light applying to pedestrians.
        /// </summary>
        public static readonly string[] PedestrianLights =
        {
            "C1.1",
            "C1.2",
            "C2.1",
            "C2.2",
            "C3.1",
            "C3.2",
        };

        /// <summary>
        /// List of all traffic lights.
        /// </summary>
        public static readonly IEnumerable<string> AllTrafficLights = CarLights.Concat(CyclistLights).Concat(PedestrianLights);

        /// <summary>
        /// Store traffic light game object references here to prevent constant recreation of objects.
        /// </summary>
        private readonly Dictionary<string, GameObject[]> TrafficLightReferences;

        /// <summary>
        /// List of all queued traffic light changes.
        /// </summary>
        private readonly Queue<KeyValuePair<string, string>> ChangeQueue;

        public TrafficLightManipulator()
        {
            TrafficLightReferences = new Dictionary<string, GameObject[]>();
            ChangeQueue = new Queue<KeyValuePair<string, string>>();
            foreach (string code in CarLights.Concat(CyclistLights))
            {
                TrafficLightReferences[code + '.' + RED] = GameObject.FindGameObjectsWithTag(code + '.' + RED);
                TrafficLightReferences[code + '.' + ORANGE] = GameObject.FindGameObjectsWithTag(code + '.' + ORANGE);
                TrafficLightReferences[code + '.' + GREEN] = GameObject.FindGameObjectsWithTag(code + '.' + GREEN);
            }

            foreach (string code in PedestrianLights)
            {
                TrafficLightReferences[code + '.' + RED] = GameObject.FindGameObjectsWithTag(code + '.' + RED);
                TrafficLightReferences[code + '.' + GREEN] = GameObject.FindGameObjectsWithTag(code + '.' + GREEN);
            }

            TrafficLightReferences["D1"] = new[] { GameObject.FindWithTag("D1") };
        }


        /// <summary>
        /// Prepares all traffic lights on the junction. All (except the train crossing lights) will be set to light up red.
        /// </summary>
        public void PrepareTrafficLights()
        {
            foreach (string code in AllTrafficLights)
            {
                foreach (GameObject gameObject in TrafficLightReferences[code + '.' + RED])
                {
                    Light light = gameObject.GetComponent<Light>();
                    light.enabled = true;
                }
            }

            GameObject busTrafficLight = TrafficLightReferences["D1"][0];
            Light busLight = busTrafficLight.GetComponent<Light>();
            busLight.color = new Color(255, 0, 0);
            busLight.enabled = true;
        }

        /// <summary>
        /// Update a light with the new color
        /// </summary>
        /// <param name="light">Light to update</param>
        /// <param name="color">Color to use</param>
        private void UpdateLight(string light, string color)
        {
            IEnumerable<string> colors = new[] {GREEN, ORANGE, RED};

            if (light == "E1" || light == "F1" || light == "F2")
            {
                HandleTrainLights(color);
                return;
            }
            else if (light == "D1")
            {
                HandleBusLight(color);
                return;
            }
            else if (light[0] == 'C')
            {
                if (color == ORANGE)
                {
                    Debug.Log("Cannot set pedestrian lights to " + color);
                    return;
                }
                else
                {
                    colors = colors.Where(x => x != ORANGE);
                }
            }

            foreach (string s in colors)
            {
                GameObject[] lights = TrafficLightReferences[light + '.' + s];
                foreach (GameObject lightHolder in lights)
                {
                    Light lightComponent = lightHolder.GetComponent<Light>();
                    lightComponent.enabled = s == color;
                }
            }
        }

        /// <summary>
        /// Queue a new change.
        /// </summary>
        /// <param name="light">Light to queue a new status for</param>
        /// <param name="status">New status (color)</param>
        public void Queue(string light, string status)
        {
            light = (light ?? "").ToUpper();
            status = ConvertInputcolour(status);
            ChangeQueue.Enqueue(new KeyValuePair<string, string>(light, status));
        }

        /// <summary>
        /// Process the changes queued.
        /// </summary>
        public void ProcessQueue()
        {
            while (ChangeQueue.Count > 0)
            {
                KeyValuePair<string, string> pair = ChangeQueue.Dequeue();
                UpdateLight(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Update the Train light.
        /// </summary>
        /// <param name="color">New color</param>
        private void HandleTrainLights(string color)
        {
            switch (color)
            {
                case GREEN:
                    SpoorboomController.CloseCrossing();
                    return;
                case RED:
                    SpoorboomController.OpenCrossing();
                    return;
                default:
                    return;
            }
        }

        /// <summary>
        /// Update the bus light.
        /// </summary>
        /// <param name="color">New color</param>
        private void HandleBusLight(string color)
        {
            GameObject busLight = TrafficLightReferences["D1"][0];
            Light lightComponent = busLight.GetComponent<Light>();
            switch (color)
            {
                case RED:
                    lightComponent.color = BusRedColor;
                    break;
                case ORANGE:
                    lightComponent.color = BusOrangeColor;
                    break;
                case GREEN:
                    lightComponent.color = BusGreenColor;
                    break;
            }
        }

        /// <summary>
        /// Process all kinds of colour input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Either RED, GREEN or ORANGE. Invalid input returns ""</returns>
        private string ConvertInputcolour(string input)
        {
            input = input.ToUpper();

            switch (input)
            {
                case "R":
                case "ROOD":
                case TrafficLightManipulator.RED:
                    return TrafficLightManipulator.RED;
                case "GEEL":
                case "YELLOW":
                case "O":
                case "ORANJE":
                case TrafficLightManipulator.ORANGE:
                    return TrafficLightManipulator.ORANGE;
                case "G":
                case "GROEN":
                case TrafficLightManipulator.GREEN:
                    return TrafficLightManipulator.GREEN;
                default:
                    Debug.Log("Invalid input " + input);
                    break;
            }

            return "";
        }
    }
}
