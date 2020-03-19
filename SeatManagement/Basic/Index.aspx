<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="SeatManagement.Basic.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>座席管理システム</title>
    <style type="text/css">
        body {
            padding: 30px 5%
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
            grid-template-rows: 90px auto auto auto;
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
    </style>
</head>
<body>
    <div class="<%= systemMessage != "" ?"message":null %>">
        <%=systemMessage %>
    </div>
    <form id="form1" runat="server">
        <div>
            <br />
            <asp:Label ID="Label1" runat="server" Text="座席管理システム"></asp:Label>
            <br />
            <br />
            <br />
            <asp:Label ID="Label2" runat="server" Text="検索文字"></asp:Label>
            &nbsp;&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="SearchParam" runat="server"></asp:TextBox>
            &nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="ButtonSearch" runat="server" Text="検索" OnClick="ButtonSearch_Click" />
            <br />
            <br />
            <asp:GridView ID="GridView1" runat="server" 
                AutoGenerateColumns="False" 
                CellPadding="4" DataKeyNames="EMPLOYEE_CODE" ForeColor="#333333" 
                PageSize="2">
                <AlternatingRowStyle BackColor="White" />
                <Columns>
                    <asp:BoundField DataField="EMPLOYEE_CODE" HeaderText="アカウントコード" ReadOnly="True" SortExpression="EMPLOYEE_CODE" />
                    <asp:BoundField DataField="EMPLOYEE_NAME" HeaderText="氏名 " SortExpression="EMPLOYEE_NAME" />
                    <asp:BoundField DataField="KATAKANA_NAME" HeaderText="カタカナ" SortExpression="KATAKANA_NAME" />
                    <asp:TemplateField HeaderText="性別" SortExpression="SEX">
                        <ItemTemplate>
                            <asp:Label ID="SexLabel" runat="server"
                                Text='<%# ((string)Eval("SEX_TYPE")) == "1"?"男性":"女性" %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="ACCOUNT_TYPE_NAME" HeaderText="アカウント種別" SortExpression="ACCOUNT_TYPE_CODE" />
                    <asp:BoundField DataField="DEPARTMENT_NAME" HeaderText="所属部門" SortExpression="DEPARTMENT_CODE" />
                    <asp:BoundField DataField="SEAT_GROUP" HeaderText="島" SortExpression="SEAT_GROUP" />
                    <asp:TemplateField HeaderText="座席" SortExpression="SEAT_POSITION_X">
                        <ItemTemplate>
                            <asp:Label runat="server"
                                Text='<%# Eval("SEAT_POSITION_X")+"-"+Eval("SEAT_POSITION_Y") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="PROJECT" HeaderText="担当PJ" SortExpression="PROJECT" />
                </Columns>
                <EditRowStyle BackColor="#2461BF" />
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <PagerSettings PageButtonCount="5" />
                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#EFF3FB" />
                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                <SortedDescendingHeaderStyle BackColor="#4870BE" />
            </asp:GridView>
            <br />
            <asp:Repeater ID="rptPager" runat="server">
                <ItemTemplate>
                    <asp:LinkButton ID="lnkPage" runat="server" Text = '<%#Eval("Text") %>' CommandArgument = '<%# Eval("Value") %>' Enabled = '<%# Eval("Enabled") %>' OnClick = "Page_Changed"></asp:LinkButton>
                </ItemTemplate>
            </asp:Repeater>
            <br />
        </div>
    </form>
    
    <div class="grid-container-group">
        <% foreach (var group in SEAT_MAP)
            { %>
        <div class="grid-container">
            <% foreach (var seatPos in group.seatPostions)
                { %>
            <div class="grid-item <%= checkSearch(group.group, seatPos.x, seatPos.y) ? "found" : null %>">
                <%=  getEmpNameBySeat(group.group, seatPos.x, seatPos.y)%>
                <%--<asp:Label runat="server" Text='<%# getEmpNameBySeat(group.group, seatPos.x, seatPos.y) %>'></asp:Label>--%>
            </div>
            <% } %>
        </div>
        <% } %>
    </div>
</body>
</html>
