using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets
{
    /// <summary>
    /// Various utility methods, that might be extension methods.
    /// Some are here to reinvent the wheel due to Unity's outdated Mono version, other are to exploit Unity's custom classes.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string GetFileInHome(params string[] paths)
        {
            string path = Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

            foreach (string s in paths)
            {
                path += Path.DirectorySeparatorChar + s;
            }

            return path;
        }

        public static string getWebSocketPath()
        {
            string configPath = GetFileInHome("TrafficSimConfig.json");
            string uri = "ws://localhost";
            string port = "5678";
            if (File.Exists(configPath))
            {
                JObject config = JObject.Parse(File.ReadAllText(configPath));
                if (config["uri"] != null)
                {
                    uri = config["uri"].ToString().TrimEnd('/');
                }

                if (config["port"] != null)
                {
                    port = config["port"].ToString();
                }
            }

            if (!uri.StartsWith("ws://") && !uri.StartsWith("wss://"))
            {
                uri = "ws://" + uri;
            }

            return uri + ':' + port;
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            if (value == null)
            {
                return true;
            }

            foreach (char t in value)
            {
                if (!char.IsWhiteSpace(t))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Emulates Enum.TryParse cuz of Mono reasons. Don't go Mono.
        /// </summary>
        /// <param name="s"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static T Parse<T>(string s) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            else if (IsNullOrWhiteSpace(s))
            {
                throw new ArgumentException("s must be a valid string");
            }

            foreach (T item in Enum.GetValues(typeof(T)))
            {
                if (string.Equals(item.ToString(CultureInfo.InvariantCulture), s, StringComparison.CurrentCultureIgnoreCase))
                {
                    return item;
                }
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// Get a random item from the given list.
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Random<T>(this IList<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        /// <summary>
        /// Determines the color of a traffic light.
        /// </summary>
        /// <param name="trafficLight"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static LightColor GetTrafficLightColor(string trafficLight)
        {
            if (IsNullOrWhiteSpace(trafficLight))
            {
                throw new ArgumentException("trafficLight must be a valid string");
            }

            if (trafficLight == "E1")
            {
                if (SpoorboomController.TrainCrossState != TrainCrossState.Open)
                {
                    return LightColor.Red;
                }
                else
                {
                    return LightColor.Green;
                }
            }
            else if (trafficLight == "D1")
            {
                GameObject busLight = GameObject.FindWithTag("D1");
                Color c = busLight.GetComponent<Light>().color;

                if (c == TrafficLightManipulator.BusRedColor)
                {
                    return LightColor.Red;
                }
                else if (c == TrafficLightManipulator.BusGreenColor)
                {
                    return LightColor.Green;
                }
                else if (c == TrafficLightManipulator.BusOrangeColor)
                {
                    return LightColor.Orange;
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
            else if (trafficLight[0] == 'C')
            {
                GameObject light = GameObject.FindWithTag(trafficLight + ".RED");
                if (light.GetComponent<Light>().enabled)
                {
                    return LightColor.Red;
                }
                else
                {
                    return LightColor.Green;
                }
            }
            else
            {
                GameObject redLight = GameObject.FindWithTag(trafficLight + ".RED");
                GameObject greenLight = GameObject.FindWithTag(trafficLight + ".GREEN");
                if (redLight.GetComponent<Light>().enabled)
                {
                    return LightColor.Red;
                }
                else if (greenLight.GetComponent<Light>().enabled)
                {
                    return LightColor.Green;
                }
                else
                {
                    return LightColor.Orange;
                }
            }
        }
    }
}
