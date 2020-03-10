<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Index.aspx.cs" Inherits="SeatManagement.Basic.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
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
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:SeatManagementConnectionString %>"
                SelectCommand=" SELECT ACCOUNT_TYPE.ACCOUNT_TYPE_NAME AS ACCOUNT_TYPE_NAME, DEPARTMENT.[DEPARTMENT_NAME ] AS DEPARTMENT_NAME, EMPLOYEE.* FROM EMPLOYEE
                                JOIN ACCOUNT_TYPE
                                ON EMPLOYEE.ACCOUNT_TYPE_CODE = ACCOUNT_TYPE.ACCOUNT_TYPE_CODE
                                JOIN DEPARTMENT
                                ON EMPLOYEE.DEPARTMENT_CODE = DEPARTMENT.DEPARTMENT_CODE"
                FilterExpression="KATAKANA_NAME LIKE '%{0}%'">
                <FilterParameters>
                    <asp:ControlParameter Name="SearchParam" ControlID="SearchParam" PropertyName="Text" />
                </FilterParameters>
            </asp:SqlDataSource>
            <asp:TextBox ID="SearchParam" runat="server"></asp:TextBox>
            &nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="ButtonSearch" runat="server" Text="検索" />
            <br />
            <br />
            <br />
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="EMPLOYEE_CODE" DataSourceID="SqlDataSource1" ForeColor="#333333" GridLines="None">
                <AlternatingRowStyle BackColor="White" />
                <Columns>
                    <asp:BoundField DataField="EMPLOYEE_CODE" HeaderText="アカウントコード" ReadOnly="True" SortExpression="EMPLOYEE_CODE" />
                    <asp:BoundField DataField="NAME " HeaderText="氏名 " SortExpression="NAME " />
                    <asp:BoundField DataField="KATAKANA_NAME" HeaderText="カタカナ" SortExpression="KATAKANA_NAME" />
                    <asp:TemplateField HeaderText="性別" SortExpression="SEX">
                        <ItemTemplate>
                            <asp:Label ID="SexLabel" runat="server"
                                Text='<%# ((int)Eval("SEX")) == 1?"男性":"女性" %>'>
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
    <div class="grid-container-group">
        <% SEAT_MAP.ForEach(eachGroup =>{ %>
             <div class="grid-container">
                <% List<JObject> seatPositions = eachGroup["SEAT_POSITIONS"].ToObject<List<JObject>>();
                    seatPositions.ForEach(eachPos => {%>
                        <div class='grid-item'>
                            afddsfa
                        </div>
                    <%}); %>
              </div>
         <%}); %>
     </div>


            <%--<ng-container *ngFor="let group of data">
    <div class="grid-container">
      <ng-container *ngFor="let chair of group.chairs">
        <div class='grid-item'>
          {{callGetInfoStaffHere(group.group, chair.x, chair.y)}}
        </div>
      </ng-container>
    </div>
  </ng-container>--%>
</body>
</html>
