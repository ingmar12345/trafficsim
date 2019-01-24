using System.Collections.Generic;

namespace Assets
{
    /// <summary>
    /// Class with collections of waypoint traffic light association.
    /// </summary>
    public static class Dictionaries
    {
        /// <summary>
        /// dictionary that links the waypoints for cars with traffic lights
        /// key = traffic light code
        /// value = waypoint code
        /// </summary>
        public static readonly Dictionary<Wp, string> CarWaitpoints = new Dictionary<Wp, string>
        {
            { Wp.A2, "A1" },
            { Wp.A8, "A2" },
            { Wp.A15, "A3" },
            { Wp.A22, "A3" },
            { Wp.A29, "A4" },
            { Wp.A35, "A5" },
            { Wp.A38, "A6" },
            { Wp.A43, "A7" },
            { Wp.A50, "A8" },
            { Wp.A47, "A9" },
            { Wp.A54, "A10" },
        };

        /// <summary>
        /// dictionary that links both traffic lights and trigger buttons with the waypoints for bicycles
        /// key = waypoint code
        /// value = traffic light code
        /// </summary>
        public static readonly Dictionary<Wp, string> CycleWaitpoints = new Dictionary<Wp, string>
        {
            { Wp.B23a, "B1" },
            { Wp.B23b, "B1" },
            { Wp.B24a, "B1" },
            { Wp.B24b, "B1" },
            { Wp.B16, "B2" },
            { Wp.B26, "B2" },
            { Wp.B3a, "B3" },
            { Wp.B3b, "B3" },
            { Wp.B4a, "B3" },
            { Wp.B4b, "B3" },
        };

        /// <summary>
        /// dictionary that links both traffic lights and trigger buttons with the waypoints for pedestrians
        /// key = waypoint code
        /// value = traffic light code
        /// </summary>
        public static readonly Dictionary<Wp, string> PedestrianWaitpoints = new Dictionary<Wp, string>
        {
            { Wp.C3b, "C1.1" },
            { Wp.C4a, "C1.1" },
            { Wp.C4b, "C1.2" },
            { Wp.C5a, "C1.2" },
            { Wp.C11a, "C2.1" },
            { Wp.C12b, "C2.1" },
            { Wp.C12a, "C2.2" },
            { Wp.C13b, "C2.2" },
            { Wp.C26b, "C3.1" },
            { Wp.C27a, "C3.1" },
            { Wp.C27b, "C3.2" },
            { Wp.C28a, "C3.2" },
        };

        /// <summary>
        /// dictionary that links traffic lights with the waypoints for buses
        /// key = waypoint code
        /// value = traffic light code
        /// </summary>
        public static readonly Dictionary<Wp, string> BusWaitpoints = new Dictionary<Wp, string>
        {
            { Wp.D3, "D1" },
        };

        /// <summary>
        /// dictionary that links train lights with the waypoints for all traffic users
        /// key = waypoint code
        /// value = traffic light code
        /// </summary>
        public static readonly Dictionary<Wp, string> TrainWaitpoints = new Dictionary<Wp, string>
        {
            { Wp.A11, "E1" },
            { Wp.A38, "E1" },
            { Wp.A43, "E1" },
            { Wp.B14, "E1" },
            { Wp.B11a, "E1" },
            { Wp.B10b, "E1" },
            { Wp.C15a, "E1" },
            { Wp.C16b, "E1" },
            { Wp.C19b, "E1" },
            { Wp.C20b, "E1" },
        };

        /// <summary>
        /// List of all traffic lights.
        /// </summary>
        public static readonly List<string> Lights = new List<string>
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
            "A10",
            "B1",
            "B2",
            "B3",
            "C1.1",
            "C1.2",
            "C2.1",
            "C2.2",
            "C3.1",
            "C3.2",
            "D1"
        };
    }
}
