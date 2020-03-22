<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="SeatManagement.Basic.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>座席管理システム</title>
    <style type="text/css">
        body {
            padding: 10px 5%
        }

        .grid-container-group {
            padding: 0 10px;
            display: grid;
            grid-template-columns: 500px 500px;
            grid-gap: 50px;
        }

        .grid-container {
            display: grid;
            grid-template-columns: 250px 250px;
            grid-template-rows: 80px auto auto auto;
            border: 1px solid rgba(0, 0, 0, 0.8);
        }

        .grid-item {
            border: 1px solid rgba(0, 0, 0, 0.8);
            padding: 15px;
            font-size: 30px;
            text-align: center;
        }

        .found {
            background: yellow;
        }

        .message {
            color: #9a3254;
            border: 1px solid;
            padding: 10px;
            background: antiquewhite;
        }

        .pagination {
            padding-top: 5px;
        }

            .pagination a {
                padding: 5px;
            }

                .pagination a:hover:not(.active) {
                    background-color: #ddd;
                }
    </style>
</head>
<body>
    <div class="<%= systemMessage != "" ?"message":null %>">
        <%=systemMessage %>
    </div>
    <form id="form1" runat="server">
        <div>
            <br />
            <h2>座席管理システム</h2>
            <br />
            <asp:Label ID="Label2" runat="server" Text="検索文字"></asp:Label>
            &nbsp;&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="SearchParam" runat="server"></asp:TextBox>
            &nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="ButtonSearch" runat="server" Text="検索" OnClick="ButtonSearch_Click" />
            <br />
            <br />
            <asp:GridView ID="GridView1" runat="server"
                AutoGenerateColumns="False" CellPadding="4" DataKeyNames="employee_code" ForeColor="#333333" >
                <AlternatingRowStyle BackColor="White" />
                <Columns>
                    <asp:BoundField DataField="employee_code" HeaderText="アカウントコード" />
                    <asp:BoundField DataField="employee_name" HeaderText="氏名 " />
                    <asp:BoundField DataField="katakana_name" HeaderText="カタカナ" />
                    <asp:TemplateField HeaderText="性別">
                        <ItemTemplate>
                            <asp:Label ID="SexLabel" runat="server"
                                Text='<%# ((string)Eval("sex_type")) == "1"?"男性":"女性" %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="account_type_name" HeaderText="アカウント種別" />
                    <asp:BoundField DataField="department_name" HeaderText="所属部門" />
                    <asp:BoundField DataField="seat_group" HeaderText="島" />
                    <asp:TemplateField HeaderText="座席">
                        <ItemTemplate>
                            <asp:Label ID="SeatPosition" runat="server"
                                Text='<%# Eval("seat_position_x") + "-" + Eval("seat_position_y") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="project" HeaderText="担当PJ" SortExpression="project" />
                </Columns>
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <RowStyle BackColor="#EFF3FB" />
            </asp:GridView>
            <div class="pagination">
                <asp:Repeater ID="rptPager" runat="server">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkPage" runat="server"
                            Text='<%#Eval("Text") %>'
                            CommandArgument='<%# Eval("Value") %>'
                            Enabled='<%# Eval("Enabled") %>'
                            OnClick="PageChanged">
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <br />
        </div>
    </form>

    <div class="grid-container-group">
        <% foreach (var group in SEAT_MAP)
            { %>
        <div class="grid-container">
            <% foreach (var seatPos in group.seatPostions)
                { %>
            <div class="grid-item <%= IsFoundSeat(group.group, seatPos.x, seatPos.y) ? "found" : null %>">
                <%=  getEmpNameBySeat(group.group, seatPos.x, seatPos.y)%>
                <%--<asp:Label runat="server" Text='<%# getEmpNameBySeat(group.group, seatPos.x, seatPos.y) %>'></asp:Label>--%>
            </div>
            <% } %>
        </div>
        <% } %>
    </div>
</body>
</html>
