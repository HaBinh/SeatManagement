using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

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
        public string systemMessage = "";
        public List<Group> SEAT_MAP = new List<Group>();
        public List<SeatPosition> chairSearch = new List<SeatPosition>();
        string connetionString = ConfigurationManager.ConnectionStrings["SeatManagementConnectionString"].ConnectionString;
        SqlConnection cnn;
        SqlCommand command;
        SqlDataReader dataReader;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.ConnectDB();
            this.InitData();
            this.InitSeatLayoutData();
        }

        private void InitData()
        {
            GridView1.DataSource = null;
            GridView1.DataBind();
            chairSearch = new List<SeatPosition>();
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
        }
        public string getEmpNameBySeat(string group, int x, int y)
        {
            string output = "";
            string sql = "Select EMPLOYEE_NAME from employee where SEAT_GROUP = '" + group + "' and SEAT_POSITION_X = " + x + " and SEAT_POSITION_Y = " + y;
            command = new SqlCommand(sql, cnn);
            command.Prepare();
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                output += dataReader.GetValue(0);
            }
            output = output.Equals("")? "　" : output.Split('　')[0] ;
            dataReader.Close();
            command.Dispose();
            return output;
        }
       
        protected　void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            //refreshdata();
        }
        protected void ButtonSearch_Click(object sender, EventArgs e)
        {
            string param = SearchParam.Text;
            if (param.Length == 0)
            {
                systemMessage = "「検索文字」は必須です。";
                return;
            }
            if (IsKatakanaString(param))
            {
                systemMessage = "カタカナを入力してください。";
                return;
            }
            string sql = @" SELECT ACCOUNT_TYPE.ACCOUNT_TYPE_NAME AS ACCOUNT_TYPE_NAME, DEPARTMENT.[DEPARTMENT_NAME ] AS DEPARTMENT_NAME, EMPLOYEE.* FROM EMPLOYEE
                                JOIN ACCOUNT_TYPE
                                ON EMPLOYEE.ACCOUNT_TYPE_CODE = ACCOUNT_TYPE.ACCOUNT_TYPE_CODE
                                JOIN DEPARTMENT
                                ON EMPLOYEE.DEPARTMENT_CODE = DEPARTMENT.DEPARTMENT_CODE
                                WHERE KATAKANA_NAME LIKE '%" + param + "%' ORDER BY EMPLOYEE_CODE ASC";
            command = new SqlCommand(sql, cnn);
            command.Prepare();
            dataReader = command.ExecuteReader();
            if (!dataReader.HasRows)
            {
                systemMessage = "対象データが見つかりません。";
                dataReader.Close();
                command.Dispose();
                return;
            }
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
                chairSearch.Add(chair);
            }
            dataReader.Close();
            command.Dispose();
        }

        private bool IsKatakanaString(string str)
        {
            for (int i=0; i< str.Length; i++)
            {
                if (IsKatakana(str[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsKatakana(char c)
        {
            return ('\u30A0' <= c && c <= '\u30FF')
                || ('\u31F0' <= c && c <= '\u31FF')
                || ('\u3099' <= c && c <= '\u309C')
                || ('\uFF65' <= c && c <= '\uFF9F');
        }

        public bool checkSearch(string group, int x, int y)
        {
            return this.chairSearch.FindIndex(item =>
            {
                return item.x == x && 
                item.y == y && 
                item.group == group;
            }) > -1;
        }
    }
}