using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    /// <summary>
    /// Moves a traffic user along a queue of waypoints
    /// </summary>
    public class Moving : MonoBehaviour
    {
        /// <summary>
        /// The given route of waypoints generated in the Spawner class based on the graph theory 
        /// </summary>
        public Queue<GameObject> Waypoints = new Queue<GameObject>();

        /// <summary>
        /// Potential traffic light stop points for the current agent type
        /// </summary>
        public Dictionary<Wp, string> TypeSpecificWaitPoints;

        /// <summary>
        /// Indicates if collision prevents the current object from moving.
        /// </summary>
        private bool IsColliding;

        public bool waitForTrafficLight = false;
        private double explodeTimer = 0;

        private bool IsInIdleAnimation;

        public float MovingSpeed = 7f;
        private GameObject target;

        // Use this for initialization
        void Start()
        {
            target = Waypoints.Dequeue();
            transform.position = target.transform.position;
            AnimateMoving();
            DoRotation();
            IsInIdleAnimation = false;
        }

        // Update is called once per frame
        void Update()
        {
            // if there are no more waypoint targets to visit destroy the game object
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            float distanceToWaypoint = Vector3.Distance(transform.position, target.transform.position);

            // if the queue of waypoints is empty destroy the game object
            if (Waypoints.Count == 0 && distanceToWaypoint < 0.2f)
            {
                target = null;
                return;
            }

            waitForTrafficLight = false;

            // only check for potential red light within a small range of that traffic light stop point
            if (distanceToWaypoint < 0.2f)
            {
                Wp wayPoint = Utilities.Parse<Wp>(target.name);

                // check if an agent is waiting for a traffic light specificly for this type
                if (this.TypeSpecificWaitPoints.ContainsKey(wayPoint))
                {
                    string trafficLight = this.TypeSpecificWaitPoints[wayPoint];
                    LightColor c = Utilities.GetTrafficLightColor(trafficLight);
                    // If light is orange and within less than 0.1f of the way point, make the target stop.
                    // if object approaches target that is linked to a red / orange traffic light
                    if (c == LightColor.Red || c == LightColor.Orange && distanceToWaypoint < 0.1f)
                    {
                        waitForTrafficLight = true;
                        // Don't queue the A series, these have a different trigger point.
                        if (trafficLight[0] != 'A')
                        {
                            TrafficWaitStore.Instance.Queue(trafficLight);
                        }
                    }
                }

                // also check if an agent is waiting for a train crossing
                if (Dictionaries.TrainWaitpoints.ContainsKey(wayPoint))
                {
                    string trafficLight = Dictionaries.TrainWaitpoints[wayPoint];
                    LightColor c = Utilities.GetTrafficLightColor(trafficLight);
                    // If light is orange and within less than 0.1f of the way point, make the target stop.
                    if (c == LightColor.Red || c == LightColor.Orange && distanceToWaypoint < 0.1f)
                    {
                        waitForTrafficLight = true;
                    }
                }
            }

            // prevend unintended waits by measuring the time how long an agent is idle
            // if the agent is idle for too long without a legitimate reason, let the agent explode to prevend a deadlock
            if (IsInIdleAnimation)
            {
                explodeTimer += Time.deltaTime;
                if (explodeTimer > 3)
                {
                    RaiseToHeaven();
                }
                // check every second if the agent is waiting behind a traffic light (legitimate wait, no need to destroy)
                else if (explodeTimer > 1)
                {
                    Wp wayPoint = Utilities.Parse<Wp>(target.name);
                    if (this.TypeSpecificWaitPoints.ContainsKey(wayPoint) || Dictionaries.TrainWaitpoints.ContainsKey(wayPoint))
                    {
                        //Debug.Log("explosion cancelled because legitimate wait");
                        explodeTimer = 0;
                        IsColliding = false;
                        goto Spoker;
                    }
                }
            }
            //cancel explosion as the agent can move again within the 3 seconds
            else if (explodeTimer > 0 && !IsInIdleAnimation)
            {
                explodeTimer = 0;
            }

            if (waitForTrafficLight || IsColliding)
            {
                if (!IsInIdleAnimation)
                {
                    AnimateIdle();
                    IsInIdleAnimation = true;

                }
                return;
            }

            // move logic
            Spoker:
            
            if (IsInIdleAnimation)
            {
                AnimateMoving();
                IsInIdleAnimation = false;
            }           

            DoRotation();

            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, MovingSpeed * Time.deltaTime);

            if (distanceToWaypoint < 0.2f && Waypoints.Count > 0)
            {
                target = Waypoints.Dequeue();
            }
        }

        private void RaiseToHeaven()
        {
            transform.Translate(0, 2 * Time.deltaTime, 0);
            if (transform.position.y > 15)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Rotates the current game objects towards the current target
        /// </summary>
        private void DoRotation()
        {
            Vector3 relative = target.transform.position - transform.position;

            // Don't rotate when the relative position is 0, as it will cause a 90 degree rotation.
            if (relative != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(relative);
            }
        }

        /// <summary>
        /// Sets the right moving animation for the specific agent type
        /// </summary>
        private void AnimateMoving()
        {
            string animationName = "";
            switch (gameObject.name)
            {
                case "prothesis(Clone)":
                    animationName = "prothesis";
                    break;
                case "cycle_test(Clone)":
                    animationName = "spokesman";
                    break;
                case "spokesman_mobile(Clone)":
                    animationName = "spokesman_mobile";
                    break;
                case "F150":
                    animationName = "f150(Clone)";
                    break;
                case "deeldelier_verticale_trommel(Clone)":
                    animationName = "deeldelier_verticale_trommel";
                    break;
                case "deeldelier_trombone(Clone)":
                    animationName = "deeldelier_trombone";
                    break;
                case "deeldelier_toeter(Clone)":
                    animationName = "deeldelier_toeter";
                    break;
                case "deeldelier_horizontale_trommel(Clone)":
                    animationName = "deeldelier_horizontale_trommel";
                    break;
                case "deeldelier_pingping(Clone)":
                    animationName = "deeldelier_pingping";
                    break;
            }

            gameObject.GetComponent<Animator>().Play(animationName);
        }

        /// <summary>
        /// Sets the right idle animation for the specific agent type
        /// </summary>
        private void AnimateIdle()
        {
            string animationName = "";
            //Debug.Log(gameObject.name + " called idle animation");
            switch (gameObject.name)
            {
                case "prothesis(Clone)":
                    animationName = "prothesis_idle";
                    break;
                case "cycle_test(Clone)":
                    animationName = "spokesman_idle";
                    break;
                case "spokesman_mobile(Clone)":
                    animationName = "spokesman_mobile_idle";
                    break;
                case "F150(Clone)":
                    animationName = "f150_idle";
                    break;
                case "deeldelier_verticale_trommel(Clone)":
                    animationName = "deeldelier_verticale_trommel_idle";
                    break;
                case "deeldelier_trombone(Clone)":
                    animationName = "deeldelier_trombone_idle";
                    break;
                case "deeldelier_toeter(Clone)":
                    animationName = "deeldelier_toeter_idle";
                    break;
                case "deeldelier_horizontale_trommel(Clone)":
                    animationName = "deelder_horizontale_trommel_idle";
                    break;
                case "deeldelier_pingping(Clone)":
                    animationName = "deeldelier_pingping_idle";
                    break;
            }

            gameObject.GetComponent<Animator>().Play(animationName);
        }

        /// <summary>
        /// Imagine it like this: you're looking through your front window (or just straight ahead if you have none)
        /// What you see is the tag of the other (Car, Bus, Cyclist, Pedestrian), what part you'll collide with is the
        /// name of the collider.
        /// </summary>
        /// <param name="other">The triggering collider (AKA Front of the other traffic participant)</param>
        private void OnTriggerStay(Collider other)
        {
            string side = other.GetComponent<Collider>().name;

            // Always stop behind other traffic.
            if (side == "Back" || side == "Left" || side == "Right")
            {
                IsColliding = true;
            }

            
        }

        /// <summary>
        /// Is executed when the collission ends
        /// </summary>
        /// <param name="other">The triggering collider (AKA Front of the other traffic participant)</param>
        private void OnTriggerExit(Collider other)
        {
            IsColliding = false;

            string colliderName = other.GetComponent<Collider>().name;

            if (colliderName.StartsWith("T"))
            {
                TrafficWaitStore.Instance.Queue('A' + colliderName.Substring(1));
            }
        }
    }
}
