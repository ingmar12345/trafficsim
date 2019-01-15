using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class TrafficWaitStore
    {
        public static TrafficWaitStore Instance { get; private set; }

        public TrafficWaitStore()
        {
            LightsWithTraffic = new List<string>();
            Instance = this;
        }

        public List<string> LightsWithTraffic { get; set; }

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
