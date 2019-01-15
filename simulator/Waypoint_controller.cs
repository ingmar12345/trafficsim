using Assets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Waypoint_controller : MonoBehaviour
{
    public static List<List<Wp>> carRoutes = new List<List<Wp>>
    {
        // North
        new List<Wp>{ Wp.A1, Wp.A2, Wp.A56, Wp.A3, Wp.A57, Wp.A4, Wp.A5, Wp.A58, Wp.A6, Wp.END },
        new List<Wp>{ Wp.A7, Wp.A8, Wp.A9, Wp.A81, Wp.A10, Wp.A11, Wp.A12, Wp.A13, Wp.END },
        new List<Wp>{ Wp.A14, Wp.A15, Wp.A62, Wp.A16, Wp.A63, Wp.A17, Wp.A64, Wp.A18, Wp.A65, Wp.A19,Wp.A20, Wp.END },
        new List<Wp>{ Wp.A21, Wp.A22, Wp.A66, Wp.A23, Wp.A24, Wp.A67, Wp.A25, Wp.A68, Wp.A26, Wp.A20, Wp.END },

        // East
        new List<Wp>{ Wp.A27, Wp.A69, Wp.A28, Wp.A29, Wp.D4, Wp.A30, Wp.A31, Wp.A32, Wp.END },
        new List<Wp>{ Wp.A33, Wp.A70, Wp.A34, Wp.A71, Wp.A35, Wp.A36, Wp.A9, Wp.A72, Wp.A4, Wp.A5, Wp.A58, Wp.A6, Wp.END },

        // South
        new List<Wp>{ Wp.A37, Wp.A38, Wp.A39 },
        new List<Wp>{ Wp.A42, Wp.A43, Wp.A44, Wp.A17, Wp.A63, Wp.A9, Wp.A72, Wp.A4, Wp.A5, Wp.A58, Wp.A6, Wp.END },

        // West
        new List<Wp>{ Wp.A45, Wp.A77 },
        new List<Wp>{ Wp.A52, Wp.A75, Wp.A53, Wp.A76, Wp.A54, Wp.A55, Wp.A16, Wp.A23, Wp.A31, Wp.A32, Wp.END },

        // Sequel A39
        new List<Wp>{ Wp.A39, Wp.A73, Wp.A40, Wp.A74, Wp.A18, Wp.A65, Wp.A19, Wp.A20, Wp.END },
        new List<Wp>{ Wp.A39, Wp.A41, Wp.A30, Wp.A31, Wp.A32, Wp.END },

        // Sequel A77
        new List<Wp>{ Wp.A77, Wp.A49, Wp.A80, Wp.A50, Wp.A51, Wp.A59, Wp.A60, Wp.A61, Wp.A11, Wp.A12, Wp.A13, Wp.END },
        new List<Wp>{ Wp.A77, Wp.A46, Wp.A78, Wp.A47, Wp.A79, Wp.A48, Wp.A81, Wp.A17, Wp.A64, Wp.A18, Wp.A65, Wp.A19, Wp.A20, Wp.END },
    };

    public static List<List<Wp>> cycleRoutes = new List<List<Wp>>
    {
        // North
        new List<Wp>{ Wp.B1a, Wp.B2a },

        // East
        new List<Wp>{ Wp.B17a, Wp.B18a, Wp.B19a, Wp.B30a, Wp.B20a, Wp.B21a },

        // South-East
        new List<Wp>{ Wp.B13, Wp.B14, Wp.B15, Wp.B16, Wp.B26, Wp.B20b },

        // South-West
        new List<Wp>{ Wp.B12a, Wp.B11a, Wp.B10a },

        // West
        new List<Wp>{ Wp.B5b, Wp.B6b, Wp.B27b, Wp.B7b },

        // Sequel B2a
        new List<Wp>{ Wp.B2a, Wp.B2b, Wp.B24b, Wp.B23b, Wp.B21b },
        new List<Wp>{ Wp.B2a, Wp.B3a, Wp.B4a, Wp.B7a },

        // Sequel B7a
        new List<Wp>{ Wp.B7a, Wp.B27a, Wp.B6a, Wp.B5a, Wp.END },
        new List<Wp>{ Wp.B7a, Wp.B7b, Wp.B8b, Wp.B28b, Wp.B9b, Wp.B29b, Wp.B10b, Wp.B11b, Wp.B12b, Wp.END },

        // Sequel B21a
        new List<Wp>{ Wp.B21a, Wp.B23a, Wp.B24a, Wp.B25 },
        new List<Wp>{ Wp.B21a, Wp.B22, Wp.END },

        // Sequel B2b
        new List<Wp>{ Wp.B2b, Wp.B1b, Wp.END },
        new List<Wp>{ Wp.B2b, Wp.B24b, Wp.B23b, Wp.B21b },

        // Sequel B21b
        new List<Wp>{ Wp.B21b, Wp.B30b, Wp.B20b, Wp.B19b, Wp.B18b, Wp.B17b, Wp.END },
        new List<Wp>{ Wp.B21b, Wp.B21a, Wp.B22, Wp.END },

        // Sequel B25
        new List<Wp>{ Wp.B25, Wp.B2a, Wp.B3a, Wp.B4a, Wp.B7a },
        new List<Wp>{ Wp.B25, Wp.B1b, Wp.END },

        // Sequel B20b
        new List<Wp>{ Wp.B20b, Wp.B19b, Wp.B18b, Wp.B17b, Wp.END },
        new List<Wp>{ Wp.B20b, Wp.B20a, Wp.B21a },

        // Sequel B10a
        new List<Wp>{ Wp.B10a, Wp.B29a, Wp.B9a, Wp.B28a, Wp.B8a, Wp.B7a, Wp.B27a, Wp.B6a, Wp.B5a, Wp.END },
        new List<Wp>{ Wp.B10a, Wp.B29c, Wp.B9c, Wp.B28c, Wp.B8c, Wp.B4b, Wp.B3b, Wp.B2b },

        // Sequel B7b
        new List<Wp>{ Wp.B7b, Wp.B8b, Wp.B28b, Wp.B9b, Wp.B29b, Wp.B10b, Wp.B11b, Wp.B12b, Wp.END },
        new List<Wp>{ Wp.B7b, Wp.B4b, Wp.B3b, Wp.B2b }
    };

    public static List<List<Wp>> pedestrianRoutes = new List<List<Wp>>
    {
        // North
        new List<Wp>{ Wp.C1a, Wp.C2a },

        // North-East
        new List<Wp>{ Wp.C7a, Wp.C6a },

        // East
        new List<Wp>{ Wp.C10a, Wp.C9a, Wp.C8c },

        // South-East
        new List<Wp>{ Wp.C17b, Wp.C16b, Wp.C15b, Wp.C14b, Wp.C13b, Wp.C12b, Wp.C11b, Wp.C8b },

        // South-West
        new List<Wp>{ Wp.C18b, Wp.C19b, Wp.C20a, Wp.C21a, Wp.C22a },

        // West-South
        new List<Wp>{ Wp.C25b, Wp.C24b, Wp.C23b, Wp.C22b },

        // West-North
        new List<Wp>{ Wp.C32b, Wp.C31b, Wp.C29b },

        // North-West
        new List<Wp>{ Wp.C30a, Wp.C29a },

        // Sequel C2a
        new List<Wp>{ Wp.C2a, Wp.C2b, Wp.C3b, Wp.C4b, Wp.C5b, Wp.C6b },
        new List<Wp>{ Wp.C2a, Wp.C29c },
        new List<Wp>{ Wp.C2a, Wp.C28a, Wp.C27a, Wp.C26a, Wp.C22c },

        // Sequal C2b
        new List<Wp>{ Wp.C2b, Wp.C3b, Wp.C4b, Wp.C5b, Wp.C6b },
        new List<Wp>{ Wp.C2b, Wp.C1b, Wp.END },
        new List<Wp>{ Wp.C2b, Wp.C2a, Wp.C29c },

        // Sequal C2c
        new List<Wp>{ Wp.C2c, Wp.C1b, Wp.END },
        new List<Wp>{ Wp.C2c, Wp.C29c },
        new List<Wp>{ Wp.C2c, Wp.C2d, Wp.C28a, Wp.C27a, Wp.C26a, Wp.C22c },

        // Sequal C2d
        new List<Wp>{ Wp.C2d, Wp.C2c, Wp.C1b, Wp.END },
        new List<Wp>{ Wp.C2d, Wp.C3b, Wp.C4b, Wp.C5b, Wp.C6b },
        new List<Wp>{ Wp.C2d, Wp.C28a, Wp.C27a, Wp.C26a, Wp.C22c },

        // Sequal C6a
        new List<Wp>{ Wp.C6a, Wp.C5a, Wp.C4a, Wp.C3a, Wp.C2c },
        new List<Wp>{ Wp.C6a, Wp.C8a },

        // Sequel C6b
        new List<Wp>{ Wp.C6b, Wp.C6c, Wp.C7b, Wp.END },
        new List<Wp>{ Wp.C6b, Wp.C8a },

        // Sequel C6c
        new List<Wp>{ Wp.C6c, Wp.C7b, Wp.END },
        new List<Wp>{ Wp.C6c, Wp.C6a, Wp.C5a, Wp.C4a, Wp.C3a, Wp.C2c },

        // Sequel C8a
        new List<Wp>{ Wp.C8a, Wp.C11a, Wp.C12a, Wp.C13a, Wp.C14a, Wp.C15a, Wp.C16a, Wp.C17a, Wp.END },
        new List<Wp>{ Wp.C8a, Wp.C8b, Wp.C9b, Wp.C10b, Wp.END },

        // Sequel C8b
        new List<Wp>{ Wp.C8b, Wp.C9b, Wp.C10b, Wp.END },
        new List<Wp>{ Wp.C8b, Wp.C8c, Wp.C6c },

        // Seqeul C8c
        new List<Wp>{ Wp.C8c, Wp.C11a, Wp.C12a, Wp.C13a, Wp.C14a, Wp.C15a, Wp.C16a, Wp.C17a, Wp.END },
        new List<Wp>{ Wp.C8c, Wp.C6c },

        // Sequal C22a
        new List<Wp>{ Wp.C22a, Wp.C23a, Wp.C24a, Wp.C25a, Wp.END },
        new List<Wp>{ Wp.C22a, Wp.C26b, Wp.C27b, Wp.C28b, Wp.C2b },

        // Sequal C22b
        new List<Wp>{ Wp.C22b, Wp.C21b, Wp.C20b, Wp.C19a, Wp.C18a, Wp.END },
        new List<Wp>{ Wp.C22b, Wp.C22a, Wp.C26b, Wp.C27b, Wp.C28b, Wp.C2b },

        // Sequal C22c
        new List<Wp>{ Wp.C22c, Wp.C23a, Wp.C24a, Wp.C25a, Wp.END },
        new List<Wp>{ Wp.C22c, Wp.C22b, Wp.C21b, Wp.C20b, Wp.C19a, Wp.C18a, Wp.END },

        // Sequal C29a
        new List<Wp>{ Wp.C29a, Wp.C31a, Wp.C32a, Wp.END },
        new List<Wp>{ Wp.C29a, Wp.C29b, Wp.C2d },

        // Sequal C29b
        new List<Wp>{ Wp.C29b, Wp.C30b, Wp.END },
        new List<Wp>{ Wp.C29b, Wp.C2d },

        // Sequal C29c
        new List<Wp>{ Wp.C29c, Wp.C29a, Wp.C31a, Wp.C32a, Wp.END },
        new List<Wp>{ Wp.C29c, Wp.C30b, Wp.END },
    };

    public static List<List<Wp>> busRoutes = new List<List<Wp>>
    {
        new List<Wp> { Wp.D1, Wp.D2, Wp.D3, Wp.D4, Wp.A72, Wp.A4, Wp.A5, Wp.A58, Wp.A6 },
    };

    public static List<Wp> RouteBuilder(List<List<Wp>> allRoutes, Wp start)
    {
        List<List<Wp>> possibleRoutes = allRoutes.Where(l => l.First() == start).ToList();

        List<Wp> route;

        // Sommige routes delen een begin punt
        if (possibleRoutes.Count > 1)
        {
            int i = Random.Range(0, possibleRoutes.Count);
            route = possibleRoutes[i];
        }
        else
        {
            route = possibleRoutes[0];
        }

        Wp last = route.Last();
        if (last != Wp.END)
        {
            //bool isValid;
            List<Wp> extension = RouteBuilder(allRoutes, last);
            //do
            //{
            //    extension = RouteBuilder(allRoutes, last);
            //    isValid = extension[1] != route[route.Count - 2];
            //} while (isValid);

            return route.Concat(extension).Distinct().ToList();
        }
        else
        {
            return route;
        }
    }




	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}
}
