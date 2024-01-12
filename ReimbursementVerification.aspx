<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SiteHrpMasterPage.Master" CodeBehind="ReimbursementVerification.aspx.vb" Inherits="DkmOnlineWeb.ReimbursementVerification" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../Content/Site.css" rel="stylesheet" />

    <telerik:RadScriptBlock runat="server" ID="radScriptBlock">
        <script src="../Script/Jquery1.8min.js"></script>
        <script type="text/javascript">

            function openRadWindow(Url) {
                window.location = Url;
                return false;
            }
        </script>
        <style>
            .Information1 {
                font-weight: bold;
                font-size: 1.3em;
                color: #2D4A98 !important;
            }

            .divCss {
                text-align: center;
                border: 1px solid #ddd;
                padding: 5px;
                border-bottom: none;
                margin-bottom: -8px;
                background: #ADC9F7;
            }
        </style>

        <script type="text/javascript">

            function openRadWindow(Url) {

                window.location = Url;
                return false;
            }
            function approved() {
                alert('Request Has Been Approved!!!');
            }

            function Rejected() {
                alert('Request Has Been Rejected!!!');
            }
            function Partially() {
                alert('Request Has Been Sent Back!!!');
            }
            function Reasonneeded() {
                alert('Please enter reason!!!');
            }

            function billpassed() {
                alert('Please enter bill passed amount!!!');
            }
            function ClaimAmount() {
                alert('Bill passed amount cannot be greater than claimed amount!!!');
            }
            function ClaimNotEqualAmount() {
                alert('Bill passed amount is not equal to claimed amount, please click either on Reject or Re-submission!!!');
            }
        </script>

        <style type="text/css">
            .header {
                background-color: #023A4D;
                color: white;
                text-align: center;
                padding: 5px;
            }
        </style>

        <style type="text/css">
            .radiolistcss input {
                display: inherit;
                float: left;
            }

            .radiolistcss td {
                padding-left: 25px;
            }

            .radiolistcss label {
                padding-left: 7px;
                margin-top: -1px;
                font-size: 14px;
            }

            .tablecss tr th {
                background: linear-gradient(to bottom, #669999 0%, #669999 100%);
                color: white;
                padding: 5px;
                font-size: 14px;
            }

            .tablecss tr td {
                padding: 5px;
            }

            .tditem {
                width: 50%;
            }

            .buttoncss:hover {
                background-color: #4CAF50 !important; /* Green */
                color: white !important;
            }

            .RadWindow {
                z-index: 999999 !important;
            }
        </style>
    </telerik:RadScriptBlock>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <telerik:RadScriptManager runat="server" ID="radscriptMgr"></telerik:RadScriptManager>

    <div style="text-align: center; border: 1px solid #ddd; padding: 5px; border-bottom: none; background: #ff6b1c; color: white">
        <asp:Label runat="server" ID="lblExpenseName" Text="View/Approve Reimbursement Details" CssClass="Information1"></asp:Label>
    </div>

    <hr />

    <div style="padding-left: 15px" runat="server" id="radiosesarchdiv">
        <asp:RadioButtonList ID="ddlViewStatus" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" CssClass="radiolistcss" Width="550px" Visible="false">
            <asp:ListItem Text="Pending" Value="1" Selected="True"></asp:ListItem>
            <asp:ListItem Text="Approved" Value="2"></asp:ListItem>
            <asp:ListItem Text="Rejected" Value="3"></asp:ListItem>
        </asp:RadioButtonList>
    </div>

    <div style="padding-top: 10px; font-weight: bold; font-size: large;">
        <telerik:RadGrid runat="server" ID="radGrid2" Skin="Outlook" AllowPaging="True" AutoGenerateColumns="false" MasterTableView-ShowHeadersWhenNoRecords="true"
            PageSize="100"
            MasterTableView-EnableNoRecordsTemplate="true" AllowFilteringByColumn="true" OnItemCommand="radGrid2_ItemCommand">

            <MasterTableView TableLayout="Auto" AutoGenerateColumns="False" AllowPaging="true" DataKeyNames="ExpensesDetailsId">
                <Columns>
                    <telerik:GridBoundColumn UniqueName="ReimburseDetailsID" DataField="ReimburseDetailsID" HeaderText="Claim Id"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="Emp_Code" HeaderText="Employee Code" DataField="Emp_code"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="Field_Name" HeaderText="ReimburseType" DataField="Field_Name"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="EntryDate" HeaderText="Claim date" DataField="EntryDate"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="BillAmount" HeaderText="Bill Amount" DataField="BillAmount" DataFormatString="{0:F0}"></telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn UniqueName="ViewReport2" DataField="Emp_code" AllowFiltering="false" HeaderText="Download Bills">
                        <ItemTemplate>
                            <asp:LinkButton Font-Bold="true" ForeColor="Blue" ID="lnkForViewReport2" runat="server" Text="Click Here" CommandArgument='<%# (Eval("ReimburseDetailsID").ToString())%>' OnCommand="lnkForViewReport2_Command"></asp:LinkButton>
                        </ItemTemplate>
                        <HeaderStyle />
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="billpassedTemplateColumn" HeaderText="Bill Passed" AllowFiltering="false">
                        <ItemTemplate>
                            <asp:TextBox ID="txtbillpassed" runat="server" placeholder="Amount" Width="70px"></asp:TextBox>
                            <cc1:FilteredTextBoxExtender ID="ftetxtbillpassed" runat="server" FilterType="Numbers" TargetControlID="txtbillpassed" ValidChars="0123456789"></cc1:FilteredTextBoxExtender>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>

                    <telerik:GridTemplateColumn UniqueName="RemarksTemplateColumn" HeaderText="Remarks" AllowFiltering="false">
                        <ItemTemplate>
                            <asp:TextBox ID="txtReasonAccept" runat="server" placeholder="Remarks" TextMode="MultiLine"></asp:TextBox>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>

                    <telerik:GridTemplateColumn UniqueName="AcceptTemplateColumn" HeaderText="Approved" HeaderStyle-Width="80px" AllowFiltering="false">
                        <ItemTemplate>
                            <div style="display: inline-block; margin: 5px; float: left">
                                <asp:Button ID="btnAccept" runat="server" CommandName="Accept" Text="Approved" ForeColor="#00CC00" CommandArgument='<%# Eval("ExpensesDetailsId")%>' />
                            </div>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>

                    <telerik:GridTemplateColumn UniqueName="RejectTemplateColumn" HeaderText="Reject" HeaderStyle-Width="80px" AllowFiltering="false">
                        <ItemTemplate>
                            <div style="display: inline-block; margin: 5px; float: left">
                                <asp:Button ID="btnReject" runat="server" CommandName="Reject" Text="Reject" ForeColor="Red" CommandArgument='<%# Eval("ExpensesDetailsId")%>' OnClientClick="return confirm('Are you sure you want to reject this expense?');" />
                            </div>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings AllowKeyboardNavigation="true">
            </ClientSettings>
            <GroupingSettings CaseSensitive="false" />

            <HeaderStyle Width="200px" Font-Bold="true" CssClass="left"></HeaderStyle>
            <ItemStyle ForeColor="Black" CssClass="left"></ItemStyle>
            <AlternatingItemStyle ForeColor="Black" />
            <PagerStyle Mode="NextPrevNumericAndAdvanced" Position="Bottom"></PagerStyle>
        </telerik:RadGrid>
    </div>

    <div style="padding-top: 10px;">
    </div>
    <input type="hidden" runat="server" id="hidden1" />
    <input type="hidden" runat="server" id="hidden2" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
