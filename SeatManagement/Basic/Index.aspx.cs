using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace SeatManagement.Basic
{
    public class SeatPosition 
    {
        public SeatPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int x { get; set; }    
        public int y { get; set; }
    }

    public class RootObj
    {
        public RootObj(string group, List<SeatPosition> seatPostions)
        {
            this.group = group;
            this.seatPostions = seatPostions;
        }

        public string group { get; set; }
        public List<SeatPosition> seatPostions { get; set; }
    }

    public partial class Index : System.Web.UI.Page
    {
        public string[] SEAT_GROUP = new string[6] { "A", "D", "B", "E", "C", "F" };

        public List<RootObj> SEAT_MAP = new List<RootObj>();
        protected void Page_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                List<SeatPosition> SEAT_POSITIONS = new List<SeatPosition>();
                for (int y = 1; y <= 4; y++)
                {
                    for (int x = 1; x <= 2; x++)
                    {
                        SEAT_POSITIONS.Add(new SeatPosition(x, y));
                    }
                }
                SEAT_MAP.Add(new RootObj(SEAT_GROUP[i], SEAT_POSITIONS));
            }
            SeatPosition[] tmp = { new SeatPosition(1, 1) };
            List<SeatPosition> SEAT_POSITIONS_C_F = new List<SeatPosition>(tmp);
            SEAT_MAP.Add(new RootObj(SEAT_GROUP[4], SEAT_POSITIONS_C_F));
            SEAT_MAP.Add(new RootObj(SEAT_GROUP[5], SEAT_POSITIONS_C_F));

            SEAT_MAP.ForEach(eachGroup =>
            {
                //< div class="grid-container">
                System.Diagnostics.Debug.WriteLine(eachGroup.group + ",");
                List<SeatPosition> seatPositions = eachGroup.seatPostions;
                seatPositions.ForEach(eachPos =>
                {
                System.Diagnostics.Debug.WriteLine(eachPos.x + "-" + eachPos.y);
                    //< div class='grid-item'>
                });
            });

            //SEAT_MAP.ForEach(a => System.Diagnostics.Debug.WriteLine(a.ToString() + ","));
        }
    }
}