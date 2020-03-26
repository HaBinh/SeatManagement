using System;
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
        public string name { get; set; }
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
        public List<Group> SEAT_MAP = new List<Group>();  //座席図を書くため
        string[] SEAT_GROUP = new string[6] { "A", "D", "B", "E", "C", "F" };
        public List<SeatPosition> allEmpNameData = new List<SeatPosition>();
        public List<SeatPosition> foundSeats = new List<SeatPosition>();
        string connectionString = ConfigurationManager.ConnectionStrings["SeatManagementConnectionString"].ConnectionString;
        SqlConnection cnn;
        protected void Page_Load(object sender, EventArgs e)
        {
            cnn = new SqlConnection(connectionString);
            this.InitSeatLayoutData();
            this.GetAllEmpNameData();
            SearchParam.Focus();
        }

        /// <summary>
        /// DBにすべて氏名を取得する。
        /// </summary>
        private void GetAllEmpNameData()
        {
            try
            {
                SqlCommand command;
                SqlDataReader dataReader;
                string sql = "SELECT seat_position_x, seat_position_y, seat_group, employee_name FROM employee";
                command = new SqlCommand(sql, cnn);
                command.Prepare();
                cnn.Open();
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    allEmpNameData.Add(new SeatPosition
                    {
                        x = int.Parse(dataReader.GetValue(0).ToString()),
                        y = int.Parse(dataReader.GetValue(1).ToString()),
                        group = dataReader.GetValue(2).ToString(),
                        name = dataReader.GetValue(3).ToString(),
                    });
                }
                dataReader.Close();
                command.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex);
            }
            finally
            {
                cnn.Close();
            }
        }

        /// <summary>
        /// SEAT_MAP = [{   group,
        ///                 seatPostions: [ {x, y}, ... ]
        ///             }, ... ]
        /// を作る
        /// </summary>
        protected void InitSeatLayoutData()
        {
            for (int i = 0; i < 4; i++)
            {
                List<SeatPosition> seatPostions = new List<SeatPosition>();
                for (int y = 1; y <= 4; y++)
                {
                    for (int x = 1; x <= 2; x++)
                    {
                        seatPostions.Add(new SeatPosition { x = x, y = y });
                    }
                }
                SEAT_MAP.Add(new Group { group = SEAT_GROUP[i], seatPostions = seatPostions });
            }
            SeatPosition[] tmp = { new SeatPosition { x = 1, y = 1 } };
            List<SeatPosition> seatPostionsCF = new List<SeatPosition>(tmp);
            SEAT_MAP.Add(new Group { group = SEAT_GROUP[4], seatPostions = seatPostionsCF });
            SEAT_MAP.Add(new Group { group = SEAT_GROUP[5], seatPostions = seatPostionsCF });
        }

        /// <summary>
        /// 氏名を取得する
        /// </summary>
        /// <param name="group">島</param>
        /// <param name="x">座席X</param>
        /// <param name="y">座席Y</param>
        /// <returns></returns>
        public string getEmpNameBySeat(string group, int x, int y)
        {
            int resultIndex = this.allEmpNameData.FindIndex(item =>
            {
                return item.x == x &&
                       item.y == y &&
                       item.group == group;
            });
            if (resultIndex > -1)
            {
                string output = allEmpNameData[resultIndex].name;
                return output.Split('　')[0];
            }
            return "　";
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
            hiddenSearchParam.Value = param;
            if (param.Length == 0)
            {
                systemMessage = MESS_SEARCH_PARAM_REQUIRED;
                return;
            }
            if (!IsKatakanaString(param))
            {
                systemMessage = MESS_KATAKANA_REQUIRED;
                return;
            }
            this.GetEmployeesPageWise(1, param);
        }

        /// <summary>
        /// 文字列に含まれている文字がすべてカタカナかをチェックする
        /// </summary>
        /// <param name="str">文字列</param>
        /// <returns></returns>
        private bool IsKatakanaString(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!IsKatakana(str[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 文字がカタカナかをチェックする
        /// </summary>
        /// <param name="c">文字</param>
        /// <returns></returns>
        private static bool IsKatakana(char c)
        {
            return ('\u30A0' <= c && c <= '\u30FF')
                || ('\u31F0' <= c && c <= '\u31FF')
                || ('\u3099' <= c && c <= '\u309C')
                || ('\uFF65' <= c && c <= '\uFF9F');
        }

        /// <summary>
        /// DBに社員を検索する
        /// </summary>
        /// <param name="pageIndex">ページ目</param>
        /// <param name="param">検索文字</param>
        private void GetEmployeesPageWise(int pageIndex, string param)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("GetEmployeesPageWise", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", PAGE_SIZE);
                    cmd.Parameters.AddWithValue("@Param", param);
                    cmd.Parameters.Add("@RecordCount", SqlDbType.Int, 4);
                    cmd.Parameters["@RecordCount"].Direction = ParameterDirection.Output;
                    cnn.Open();
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
                    // 検索結果の座席を保存するため、foundSeats変数を作る
                    foreach (GridViewRow row in GridView1.Rows)
                    {
                        SeatPosition chair = new SeatPosition();
                        chair.group = ((Label)row.FindControl("SeatGroup")).Text; //seat_group
                        string xy = ((Label)row.FindControl("SeatPosition")).Text; //seat_group_x - seat_group_y
                        chair.x = int.Parse(xy.Split('-')[0]);  //seat_group_x 
                        chair.y = int.Parse(xy.Split('-')[1]);  //seat_group_y
                        foundSeats.Add(chair);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex);
            }
            finally
            {
                cnn.Close();
            }
        }

        /// <summary>
        /// ページングにデータを入力する
        /// </summary>
        /// <param name="recordCount">レコードの合計</param>
        /// <param name="currentPage">現在のページ</param>
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

        /// <summary>
        /// 他のページをクリックする時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PageChanged(object sender, EventArgs e)
        {
            int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
            this.GetEmployeesPageWise(pageIndex, hiddenSearchParam.Value);
            SearchParam.Text = hiddenSearchParam.Value;
        }

        /// <summary>
        /// 座席が検索結果と一致するかをチェックする。
        /// </summary>
        /// <param name="group">島</param>
        /// <param name="x">座席X</param>
        /// <param name="y">座席Y</param>
        /// <returns></returns>
        public bool IsFoundSeat(string group, int x, int y)
        {
            return this.foundSeats.FindIndex(item =>
            {
                return item.x == x &&
                item.y == y &&
                item.group == group;
            }) > -1;
        }

        /// <summary>
        /// レイアウトをリセットする
        /// </summary>
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
