using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace SeatManagement.Basic
{
    public partial class Index : System.Web.UI.Page
    {
        public string[] SEAT_GROUP = new string[6] { "A", "D", "B", "E", "C", "F" };

        public List<JObject> SEAT_MAP = new List<JObject>();
        protected void Page_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                List<JObject> SEAT_POSITIONS = new List<JObject>();
                for (int y = 1; y <= 4; y++)
                {
                    for (int x = 1; x <= 2; x++)
                    {
                        SEAT_POSITIONS.Add(new JObject { { "X", x }, { "Y", y } });
                    }
                }
                SEAT_MAP.Add(new JObject { { "SEAT_GROUP", SEAT_GROUP[i] },
                                           { "SEAT_POSITIONS", JToken.FromObject(SEAT_POSITIONS) }
                        });
            }
            JObject[] tmp = { new JObject { { "X", 1 }, { "Y", 1 } } };
            List<JObject> SEAT_POSITIONS_C_F = new List<JObject>(tmp);
            SEAT_MAP.Add(new JObject { { "SEAT_GROUP", SEAT_GROUP[4] },
                                       { "SEAT_POSITIONS", JToken.FromObject(SEAT_POSITIONS_C_F) }
            });
            SEAT_MAP.Add(new JObject { { "SEAT_GROUP", SEAT_GROUP[5] },
                                       { "SEAT_POSITIONS", JToken.FromObject(SEAT_POSITIONS_C_F) }
            });

            SEAT_MAP.ForEach(eachGroup =>
            {
                //< div class="grid-container">
                List<JObject> seatPositions = eachGroup["SEAT_POSITIONS"].ToObject<List<JObject>>();
                seatPositions.ForEach(eachPos =>
                {
                    //< div class='grid-item'>
                });
            });

            //SEAT_MAP.ForEach(a => System.Diagnostics.Debug.WriteLine(a.ToString() + ","));
        }
    }
}