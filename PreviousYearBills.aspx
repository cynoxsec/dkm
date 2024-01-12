﻿<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/reimburseMasterPage.Master" CodeBehind="PreviousYearBills.aspx.vb" Inherits="DkmOnlineWeb.PreviousYearBills" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <telerik:RadScriptBlock runat="server" ID="radScriptBlock">

        <script src="../Script/Jquery1.8min.js" type="text/javascript"></script>
        <script type="text/javascript">
            function openRadWindow(Url) {
                window.location = Url;
                return false;
            }
        </script>


        <style>
            .rgEditForm {
                display: block;
            }

            .rgHeader {
                border: 1px solid #ddd;
                padding: 5px !important;
            }

            .rgRow td {
                color: #484646 !important;
                font-family: Verdana !important;
            }

            .ViewEditBillDetails {
                margin-top: -1px !important;
                padding-top: 0px !important;
                border-top: 4px solid #2d4a98 !important;
                background-color: #fff;
                background-image: linear-gradient(#f0f0f0,#fff);
                height: 35px;
            }

                .ViewEditBillDetails span {
                    padding: 1px 15px !important;
                }

            .radiolistcss input {
                display: inherit;
                float: left;
            }

            .radiolistcss label {
                padding-left: 25px;
                font-size: 14px;
            }
        </style>

    </telerik:RadScriptBlock>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <div style="text-align: center; border: 1px solid #ddd; padding: 5px; border-bottom: none; background: #ff6b1c; color: white">
        <asp:Label runat="server" ID="Reimbursement_Card"></asp:Label>
    </div>

    <div>

        <telerik:RadGrid runat="server" ID="radGrid1" Skin="Metro" AllowAutomaticDeletes="true"
            PageSize="50" AllowPaging="True" Style="margin-top: 0px" Visible="false" AllowSorting="true" AllowFilteringByColumn="true">
            <MasterTableView TableLayout="Auto" AutoGenerateColumns="False" DataKeyNames="ReimburseDetailsID">
                <Columns>
                    <telerik:GridBoundColumn UniqueName="Sl.No" DataField="Sl.No" HeaderText="Sl.No"></telerik:GridBoundColumn>
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

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
