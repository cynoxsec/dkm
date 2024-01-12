<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="SummaryPage.aspx.vb" Inherits="DkmOnlineWeb.SummaryPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <script type="text/javascript">
        function GetRadWindow() {
            var oWindow = null;
            oWindow = window.frameElement.radWindow;//IE (and Moz as well)     
            return oWindow;
        }
        function Clo1() {
            var wdw = GetRadWindow();
            wdw.Close();
            window.parent.location.href = window.parent.location.href;
        }
    </script>
    <style>
        body {
            color: #484646 !important;
            font-family: Verdana !important;
        }

        .rmLink {
            color: #484646 !important;
            font-family: Verdana !important;
        }

        a {
            color: #484646 !important;
            font-family: Verdana !important;
            cursor: pointer !important;
        }

        table tr td {
            color: #484646 !important;
            font-family: Verdana !important;
        }

        .RadGrid_Silk td {
            color: #484646 !important;
            font-family: Verdana !important;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <telerik:RadScriptManager runat="server" ID="radScriptMgr"></telerik:RadScriptManager>
        <div>
            <telerik:RadGrid runat="server" ID="radGridSummary" Skin="Metro" AllowFilteringByColumn="true"
                PageSize="50" AllowPaging="True" Width="950px">
                <MasterTableView TableLayout="Auto" AutoGenerateColumns="false">
                    <Columns>
                        <%--<telerik:GridBoundColumn UniqueName="Field_Name" HeaderText="Reimburse Type" DataField="Field_Name"></telerik:GridBoundColumn>--%>
                        <telerik:GridBoundColumn UniqueName="Month" HeaderText="Month" DataField="Month"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn UniqueName="Claimed_Amount" HeaderText="Claimed" DataField="Claimed_Amount"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn UniqueName="Reimbursed_Amount" HeaderText="Paid As Tax Free" DataField="Reimbursed_Amount"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn UniqueName="Taxable" HeaderText="Paid As Taxable" DataField="Taxable"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn UniqueName="Disallowed" HeaderText="Disallowed" DataField="Disallowed"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn UniqueName="Remark" HeaderText="Remark" DataField="Remark" ItemStyle-Wrap="false"></telerik:GridBoundColumn>
                    </Columns>
                </MasterTableView>

                <GroupingSettings CaseSensitive="false" />
                <HeaderStyle Width="150px" Font-Bold="true" CssClass="left" BackColor="#ff6b1c" Font-Size="12px" ForeColor="White"></HeaderStyle>
                <ItemStyle CssClass="left"></ItemStyle>
                <AlternatingItemStyle />
                <PagerStyle Mode="NextPrevAndNumeric" Position="Bottom"></PagerStyle>
            </telerik:RadGrid>
        </div>
        <div style="text-align: center; color: red; font-weight: bold; font-size: large;">
            <asp:Label runat="server" ID="lbl1"></asp:Label>
        </div>
    </form>
</body>
</html>
