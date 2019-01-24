using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    /// <summary>
    /// Holder to store traffic that is waiting for traffic lights.
    ///
    /// This is a semi-singleton to ensure there is only one instance and no data gets missed out on.
    /// It is not using the traditional singleton designs to prevent all kinds of singleton issues that arise with static constructors in C#, especially because we are dealing with Mono, not the CLR.
    /// Additionally, using a singleton benefits from a simple architecture design: Scripts don't have to jump through all kinds of hoops to get a reference to the store, as inter-script communication in Unity is rather difficult.
    /// </summary>
    public class TrafficWaitStore
    {
        /// <summary>
        /// Semi-singleton instance holder.
        /// </summary>
        public static TrafficWaitStore Instance { get; private set; }

        public TrafficWaitStore()
        {
            LightsWithTraffic = new List<string>();
            Instance = this;
        }

        /// <summary>
        /// List with all traffic lights where traffic is waiting.
        /// </summary>
        public List<string> LightsWithTraffic { get; set; }

        /// <summary>
        /// Register traffic at a traffic light.
        /// </summary>
        /// <param name="light">Light where traffic is waiting</param>
        public void Queue(string light)
        {
            if (!LightsWithTraffic.Contains(light))
            {
                //Debug.Log("Queueing " + light);
                LightsWithTraffic.Add(light);
            }
        }
    }
}
