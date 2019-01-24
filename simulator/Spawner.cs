using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    /// <summary>
    /// spawn logic for traffic users in the scene
    /// </summary>
    public class Spawner : MonoBehaviour
    {
        private const float SpawnInterval = 3f;
        private const float FanfareSpawnInterval = 2f;
        private const float BusSpawnInterval = 30f;

        /// <summary>
        /// Spawn timer.
        /// </summary>
        private float timer;
        /// <summary>
        /// Spawn timer for buses.
        /// </summary>
        private float busTimer;
        /// <summary>
        /// Spawn timer for the fanfare.
        /// </summary>
        private float fanfareTimer;
        /// <summary>
        /// Check to prevent spawning a fanfare at the same time.
        /// </summary>
        private bool fanfareIsActive;
        /// <summary>
        /// Index indicating which fanfare member is currently spawning.
        /// </summary>
        private int fanfareIndex;

        /// <summary>
        /// Contains the route the current fanfare instance should take
        /// </summary>
        private List<Wp> fanfareRoute = new List<Wp>();
        /// <summary>
        /// Starting point for the current fanfare instance.
        /// </summary>
        private Wp fanfareStart = Wp.C18b;

        /// <summary>
        /// Regular traffic participant references to use as template
        /// </summary>
        private GameObject traffic_object;
        private GameObject f150;
        private GameObject bicycle;
        private GameObject pedestrian;
        private GameObject bus;

        // fanfare references ot use as template
        private GameObject vertical_drum;
        private GameObject trombone;
        private GameObject horn;
        private GameObject horizontal_drum;
        private GameObject pingping;
        private List<Agent> fanfare = new List<Agent>();

        /// <summary>
        /// List of spawn points for cars.
        /// </summary>
        private readonly List<Wp> carSpawnPoints = new List<Wp> { Wp.A1, Wp.A7, Wp.A14, Wp.A21, Wp.A27, Wp.A33, Wp.A37, Wp.A42, Wp.A45, Wp.A52 };

        /// <summary>
        /// List of spawn points for cyclists.
        /// </summary>
        private readonly List<Wp> cycleSpawnPoints = new List<Wp> { Wp.B1a, Wp.B17a, Wp.B13, Wp.B12a, Wp.B5b };

        /// <summary>
        /// List of spawn points for pedestrians.
        /// </summary>
        private readonly List<Wp> pedestrianSpawnPoints = new List<Wp> { Wp.C1a, Wp.C7a, Wp.C10a, Wp.C17b, Wp.C18b, Wp.C25b, Wp.C32b, Wp.C30a };

        // for experimenting select routes
        private readonly List<Wp> test = new List<Wp> { Wp.C18b };

        // Use this for initialization
        void Start()
        {
            f150 = GameObject.Find("F150");
            bicycle = GameObject.Find("cycle_test");
            pedestrian = GameObject.Find("prothesis");
            // fanfare
            vertical_drum = GameObject.Find("deeldelier_verticale_trommel");
            trombone = GameObject.Find("deeldelier_trombone");
            horn = GameObject.Find("deeldelier_toeter");
            horizontal_drum = GameObject.Find("deeldelier_horizontale_trommel");
            pingping = GameObject.Find("deeldelier_pingping");
            bus = GameObject.Find("spokesman_mobile");

            bus.SetActive(false);
            f150.SetActive(false);
            bicycle.SetActive(false);
            pedestrian.SetActive(false);
            vertical_drum.SetActive(false);
            trombone.SetActive(false);
            horn.SetActive(false);
            horizontal_drum.SetActive(false);
            pingping.SetActive(false);

            fanfare = new List<Agent> { Agent.Fanfare_ping, Agent.Fanfare_hort, Agent.Fanfare_trombone, Agent.Fanfare_vert, Agent.Fanfare_horn };

            // test
            //RouteMultiplicator(Agent.Test);
        }

        // Update is called once per frame
        void Update()
        {
            timer += Time.deltaTime;
            fanfareTimer += Time.deltaTime;
            busTimer += Time.deltaTime;

            if (timer >= SpawnInterval)
            {
                RouteMultiplicator(Agent.Car);
                RouteMultiplicator(Agent.Bicycle);

                //RouteMultiplicator(Agent.Pedestrian);

                // spawn a fanfare once in a while
                int dice = Random.Range(1, 10);

                if (!fanfareIsActive && dice == 5)
                {
                    fanfareStart = pedestrianSpawnPoints[Random.Range(0, pedestrianSpawnPoints.Count)];
                    //Debug.Log("spawning fanfare at " + fanfareStart);
                    fanfareRoute = Waypoint_controller.RouteBuilder(Waypoint_controller.pedestrianRoutes, fanfareStart);
                    fanfareIsActive = true;
                }
                else if (fanfareTimer < FanfareSpawnInterval || !fanfareIsActive)
                {
                    RouteMultiplicator(Agent.Pedestrian);
                }

                timer = 0;
            }

            if (busTimer >= BusSpawnInterval)
            {
                //Debug.Log("Spawning bus");
                busTimer = 0f;
                RouteMultiplicator(Agent.Bus);
            }

            if (timer > 0 && fanfareTimer >= FanfareSpawnInterval && fanfareIsActive)
            {
                if (fanfareIndex < 5)
                {
                    fanfareTimer = 0;
                    RouteMultiplicator(fanfare[fanfareIndex]);
                    //Debug.Log("spawned a " + fanfare[fanfareIndex]);
                    fanfareIndex++;
                }
                else
                {
                    fanfareIsActive = false;
                    fanfareIndex = 0;
                    //Debug.Log("fanfare complete");
                }
            }
        }

        /// <summary>
        /// Creates a new traffic participant based on the given type on a random location with a random route.
        /// </summary>
        /// <param name="type"></param>
        private void RouteMultiplicator(Agent type)
        {
            Wp waypoint;

            switch (type)
            {
                case Agent.Car:
                    waypoint = carSpawnPoints.Random();
                    break;
                case Agent.Bicycle:
                    waypoint = cycleSpawnPoints.Random();
                    break;
                case Agent.Fanfare_horn:
                case Agent.Fanfare_vert:
                case Agent.Fanfare_trombone:
                case Agent.Fanfare_hort:
                case Agent.Fanfare_ping:
                case Agent.Pedestrian:
                    waypoint = pedestrianSpawnPoints.Random();
                    break;
                case Agent.Bus:
                    waypoint = Wp.D1;
                    break;
                // experimental purposes
                case Agent.Test:
                    waypoint = test.Random();
                    break;
                default:
                    // Compiler edge case. Shouldn't ever happen.
                    return;
            }

            List<Wp> route = new List<Wp>();
            Dictionary<Wp, string> waits = new Dictionary<Wp, string>();

            float speed = 0;

            switch (type)
            {
                case Agent.Car:
                    route = Waypoint_controller.RouteBuilder(Waypoint_controller.carRoutes, waypoint);
                    if (Random.Range(0, 15) == 7)
                    {
                        traffic_object = Instantiate(bus);
                    }
                    else
                    {
                        traffic_object = Instantiate(f150);
                    }
                    waits = Dictionaries.CarWaitpoints;
                    speed = 7f;
                    break;
                case Agent.Bicycle:
                    route = Waypoint_controller.RouteBuilder(Waypoint_controller.cycleRoutes, waypoint);
                    traffic_object = Instantiate(bicycle);
                    waits = Dictionaries.CycleWaitpoints;
                    speed = 3f;
                    break;
                case Agent.Pedestrian:
                    route = Waypoint_controller.RouteBuilder(Waypoint_controller.pedestrianRoutes, waypoint);
                    traffic_object = Instantiate(pedestrian);
                    waits = Dictionaries.PedestrianWaitpoints;
                    speed = 1f;
                    break;
                case Agent.Fanfare_horn:
                    route = fanfareRoute;
                    traffic_object = Instantiate(horn);
                    waits = Dictionaries.PedestrianWaitpoints;
                    speed = 1f;
                    break;
                case Agent.Fanfare_vert:
                    route = fanfareRoute;
                    traffic_object = Instantiate(vertical_drum);
                    waits = Dictionaries.PedestrianWaitpoints;
                    speed = 1f;
                    break;
                case Agent.Fanfare_trombone:
                    route = fanfareRoute;
                    traffic_object = Instantiate(trombone);
                    waits = Dictionaries.PedestrianWaitpoints;
                    speed = 1f;
                    break;
                case Agent.Fanfare_hort:
                    route = fanfareRoute;
                    traffic_object = Instantiate(horizontal_drum);
                    waits = Dictionaries.PedestrianWaitpoints;
                    speed = 1f;
                    break;
                case Agent.Fanfare_ping:
                    route = fanfareRoute;
                    traffic_object = Instantiate(pingping);
                    waits = Dictionaries.PedestrianWaitpoints;
                    speed = 1f;
                    break;
                case Agent.Bus:
                    route = Waypoint_controller.busRoutes[0];
                    traffic_object = Instantiate(bus);
                    waits = Dictionaries.BusWaitpoints;
                    speed = 6f;
                    break;

                // experimental purposes
                case Agent.Test:
                    route = Waypoint_controller.RouteBuilder(Waypoint_controller.pedestrianRoutes, waypoint);
                    traffic_object = Instantiate(pedestrian);
                    waits = Dictionaries.PedestrianWaitpoints;
                    speed = 6f;
                    //Debug.Log("traffic object instantiated");
                    break;
            }

            traffic_object.SetActive(true);

            Moving trafficUser = traffic_object.AddComponent<Moving>();
            trafficUser.MovingSpeed = speed;
            trafficUser.TypeSpecificWaitPoints = waits;

            foreach (Wp wp in route)
            {
                trafficUser.Waypoints.Enqueue(GameObject.Find(wp.ToString()));
            }
        }
    }
}
