using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace SeatManagement.Basic
{
    public class SeatPosition
    {
        public int x { get; set; }
        public int y { get; set; }
        public string group { get; set; }
    }
    public class Group
    {
        public string group { get; set; }
        public List<SeatPosition> seatPostions { get; set; }
    }

    public partial class Index : System.Web.UI.Page
    {
        public List<Group> SEAT_MAP = new List<Group>();
        public List<SeatPosition> chairSearch = new List<SeatPosition>();
        public string cMyValuex = "some string debug here";
        string connetionString = ConfigurationManager.ConnectionStrings["SeatManagementConnectionString"].ConnectionString;
        SqlConnection cnn;
        SqlCommand command;
        SqlDataReader dataReader;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.ConnectDB();
            this.InitSeatLayoutData();
        }

        protected void ConnectDB()
        {
            cnn = new SqlConnection(connetionString);
            cnn.Open();
        }
        protected void InitSeatLayoutData()
        {
            string[] SEAT_GROUP = new string[6] { "A", "D", "B", "E", "C", "F" };
            for (int i = 0; i < 4; i++)
            {
                List<SeatPosition> SEAT_POSITIONS = new List<SeatPosition>();
                for (int y = 1; y <= 4; y++)
                {
                    for (int x = 1; x <= 2; x++)
                    {
                        SEAT_POSITIONS.Add(new SeatPosition { x = x, y = y });
                    }
                }
                SEAT_MAP.Add(new Group { group = SEAT_GROUP[i], seatPostions = SEAT_POSITIONS });
            }
            SeatPosition[] tmp = { new SeatPosition { x = 1, y = 1 } };
            List<SeatPosition> SEAT_POSITIONS_C_F = new List<SeatPosition>(tmp);
            SEAT_MAP.Add(new Group { group = SEAT_GROUP[4], seatPostions = SEAT_POSITIONS_C_F });
            SEAT_MAP.Add(new Group { group = SEAT_GROUP[5], seatPostions = SEAT_POSITIONS_C_F });
            //SEAT_MAP.ForEach(a => System.Diagnostics.Debug.WriteLine(a.ToString() + ","));
        }
        public string getEmpNameBySeat(string group, int x, int y)
        {
            string output = "";
            string sql = "Select NAME from employee where SEAT_GROUP = '" + group + "' and SEAT_POSITION_X = " + x + " and SEAT_POSITION_Y = " + y;
            command = new SqlCommand(sql, cnn);
            command.Prepare();
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                output += dataReader.GetValue(0);
            }
            output = output.Split('　')[0];
            dataReader.Close();
            command.Dispose();
            return output;
        }
        protected void ButtonSearch_Click(object sender, EventArgs e)
        {
            string sql = @" SELECT ACCOUNT_TYPE.ACCOUNT_TYPE_NAME AS ACCOUNT_TYPE_NAME, DEPARTMENT.[DEPARTMENT_NAME ] AS DEPARTMENT_NAME, EMPLOYEE.* FROM EMPLOYEE
                                JOIN ACCOUNT_TYPE
                                ON EMPLOYEE.ACCOUNT_TYPE_CODE = ACCOUNT_TYPE.ACCOUNT_TYPE_CODE
                                JOIN DEPARTMENT
                                ON EMPLOYEE.DEPARTMENT_CODE = DEPARTMENT.DEPARTMENT_CODE
                                WHERE KATAKANA_NAME LIKE '%" + SearchParam.Text + "%'";
            command = new SqlCommand(sql, cnn);
            command.Prepare();
            dataReader = command.ExecuteReader();
            GridView1.DataSource = dataReader;
            GridView1.DataBind();
            command.Dispose();
            dataReader.Close();

            command = new SqlCommand(sql, cnn);
            command.Prepare(); 
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                SeatPosition chair = new SeatPosition();
                chair.group = dataReader["SEAT_GROUP"].ToString();
                chair.x = int.Parse(dataReader["SEAT_POSITION_X"].ToString());
                chair.y = int.Parse(dataReader["SEAT_POSITION_Y"].ToString());
                cMyValuex += chair.group+ chair.x+ chair.y;
                chairSearch.Add(chair);
            }
            dataReader.Close();
            command.Dispose();
        }
        public bool checkSearch(string group, int x, int y)
        {
            var foundChair = new SeatPosition
            {
                x = x,
                y = y,
                group = group
            };
            return this.chairSearch.FindIndex(item =>
            {
                return item.x == foundChair.x && 
                item.y == foundChair.y && 
                item.group == foundChair.group;
            }) > -1;
        }
    }
}