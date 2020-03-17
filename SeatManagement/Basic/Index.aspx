<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="SeatManagement.Basic.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <style type="text/css">
        .box-name {
            border: 1px green solid;
            padding: 10px;
        }

        .grid-container {
            display: grid;
            grid-template-columns: auto auto;
            background-color: #2196F3;
            padding: 10px;
        }

        .grid-item {
            background-color: rgba(255, 255, 255, 0.8);
            border: 1px solid rgba(0, 0, 0, 0.8);
            padding: 20px;
            font-size: 30px;
            text-align: center;
        }

        .grid-container-group {
            display: grid;
            grid-template-columns: auto auto;
            grid-gap: 20px;
        }
        .found {
            background: yellow;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <br />
            <asp:Label ID="Label1" runat="server" Text="座席管理システム"></asp:Label>
            <br />
            <br />
            <asp:Label ID="Label2" runat="server" Text="検索文字"></asp:Label>
            &nbsp;&nbsp;&nbsp;&nbsp;
            <%--            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:SeatManagementConnectionString %>">
                <FilterParameters>
                    <asp:ControlParameter Name="SearchParam" ControlID="SearchParam" PropertyName="Text" />
                </FilterParameters>
            </asp:SqlDataSource>--%>
            <asp:TextBox ID="SearchParam" runat="server"></asp:TextBox>
            &nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="ButtonSearch" runat="server" Text="検索" OnClick="ButtonSearch_Click" />
            <br />
            <br />
            <br />
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="EMPLOYEE_CODE" ForeColor="#333333" GridLines="None">
                <AlternatingRowStyle BackColor="White" />
                <Columns>
                    <asp:BoundField DataField="EMPLOYEE_CODE" HeaderText="アカウントコード" ReadOnly="True" SortExpression="EMPLOYEE_CODE" />
                    <asp:BoundField DataField="NAME " HeaderText="氏名 " SortExpression="NAME " />
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
                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#EFF3FB" />
                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                <SortedDescendingHeaderStyle BackColor="#4870BE" />
            </asp:GridView>
            <br />
            <br />
        </div>
    </form>
    <%=cMyValuex%>
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
