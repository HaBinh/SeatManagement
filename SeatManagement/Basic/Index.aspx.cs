﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
        public const int PAGE_SIZE = 5;
        public const string MESS_SEARCH_PARAM_REQUIRED = "「検索文字」は必須です。";
        public const string MESS_KATAKANA_REQUIRED = "カタカナを入力してください。";
        public const string MESS_NOT_FOUND = "対象データが見つかりません。";
        public string systemMessage = "";
        public List<Group> SEAT_MAP = new List<Group>();
        string[] SEAT_GROUP = new string[6] { "A", "D", "B", "E", "C", "F" };
        public List<SeatPosition> foundSeats = new List<SeatPosition>();
        string connetionString = ConfigurationManager.ConnectionStrings["SeatManagementConnectionString"].ConnectionString;
        SqlConnection cnn;
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
            SqlCommand command;
            SqlDataReader dataReader;
            string output = "";
            string sql = "SELECT employee_name FROM employee WHERE seat_group = '" + group 
                        + "' AND seat_position_x = " + x 
                        + " AND seat_position_y = " + y;
            command = new SqlCommand(sql, cnn);
            command.Prepare();
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                output += dataReader.GetValue(0);
            }
            output = output.Equals("") ? "　" : output.Split('　')[0];
            dataReader.Close();
            command.Dispose();
            return output;
        }

        /// <summary>
        /// 検索ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ButtonSearch_Click(object sender, EventArgs e)
        {
            this.ResetData();
            string param = SearchParam.Text;
            if (param.Length == 0)
            {
                systemMessage = MESS_SEARCH_PARAM_REQUIRED; 
                return;
            }
            if (IsKatakanaString(param))
            {
                systemMessage = MESS_KATAKANA_REQUIRED;
                return;
            }
            this.GetEmployeesPageWise(1, param);
        }

        private bool IsKatakanaString(string str)
        {
            for (int i = 0; i < str.Length; i++)
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
        private void GetEmployeesPageWise(int pageIndex, string param)
        {
            using (SqlCommand cmd = new SqlCommand("GetEmployeesPageWise", cnn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("@PageSize", PAGE_SIZE);
                cmd.Parameters.AddWithValue("@Param", param);
                cmd.Parameters.Add("@RecordCount", SqlDbType.Int, 4);
                cmd.Parameters["@RecordCount"].Direction = ParameterDirection.Output;
                SqlDataReader idr = cmd.ExecuteReader();
                if (!idr.HasRows)
                {
                    systemMessage = MESS_NOT_FOUND; 
                    idr.Close();
                    return;
                }
                GridView1.DataSource = idr;
                GridView1.DataBind();
                idr.Close(); 
                int recordCount = Convert.ToInt32(cmd.Parameters["@RecordCount"].Value);
                this.PopulatePager(recordCount, pageIndex);
                // tạo biến foundSeats lưu kết quả tìm kiếm
                foreach (GridViewRow row in GridView1.Rows)
                {
                    SeatPosition chair = new SeatPosition();
                    chair.group = row.Cells[6].Text; //seat_group
                    string xy = ((Label)row.FindControl("SeatPosition")).Text; //seat_group_x - seat_group_y
                    chair.x = int.Parse(xy.Split('-')[0]);  //seat_group_x 
                    chair.y = int.Parse(xy.Split('-')[1]);  //seat_group_y
                    foundSeats.Add(chair);
                }
            }
        }
        private void PopulatePager(int recordCount, int currentPage)
        {
            double dblPageCount = (double)((decimal)recordCount / PAGE_SIZE);
            int pageCount = (int)Math.Ceiling(dblPageCount);
            List<ListItem> pages = new List<ListItem>();
            if (pageCount > 0)
            {
                pages.Add(new ListItem("<<", "1", currentPage > 1));
                for (int i = 1; i <= pageCount; i++)
                {
                    pages.Add(new ListItem(i.ToString(), i.ToString(), i != currentPage));
                }
                pages.Add(new ListItem(">>", pageCount.ToString(), currentPage < pageCount));
            }
            rptPager.DataSource = pages;
            rptPager.DataBind();
        }
        protected void PageChanged(object sender, EventArgs e)
        {
            int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
            this.GetEmployeesPageWise(pageIndex, SearchParam.Text);
        }
        public bool CheckSearch(string group, int x, int y)
        {
            return this.foundSeats.FindIndex(item =>
            {
                return item.x == x &&
                item.y == y &&
                item.group == group;
            }) > -1;
        }
        private void ResetData()
        {
            GridView1.DataSource = null;
            GridView1.DataBind();
            rptPager.DataSource = null;
            rptPager.DataBind();
            foundSeats = new List<SeatPosition>();
        }
    }
}
