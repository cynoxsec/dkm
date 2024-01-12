<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SiteHrpMasterPage.Master" CodeBehind="ReimburseTypeAdmin.aspx.vb" Inherits="DkmOnlineWeb.ReimburseTypeAdmin" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">  
     <telerik:RadScriptBlock runat="server" ID="radScriptBlock">

        <script src="../Script/Jquery1.8min.js" type="text/javascript"></script>
        <script type="text/javascript">
            function openRadWindow1(Url) {
                var oWnd = window.$find("<%= windowmanager2.ClientID%>");
                oWnd.setUrl(Url);
                oWnd.show();
                var windowWidth = 550;
                var windowHeight = 550;
                oWnd.setSize(windowWidth, windowHeight); // var windowWidth = document.getElementsByTagName('body')[0].clientWidth; 
                oWnd.center();
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
        </style>

    </telerik:RadScriptBlock> 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
    <telerik:RadWindow runat="server" ID="windowmanager2" ShowContentDuringLoad="false" RegisterWithScriptManager="true" ReloadOnShow="true" 
        Title="Reimbursement Type Details" VisibleStatusbar="false" Modal="True" Behaviors="Close,Move" Width="1800" Height="500" Style="z-index: 99999;top:30px;">
    </telerik:RadWindow>

    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="radGrid1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="radGrid1"></telerik:AjaxUpdatedControl>
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>


    <div style="text-align: center; border: 1px solid #ddd; padding: 5px; border-bottom: none; background: #ff6b1c; color: white">
        <asp:Label runat="server" ID="lbl1" Text="Add/Update Reimbursement Type" Font-Bold="true"></asp:Label>
    </div>
    <div>
        <telerik:RadGrid runat="server" ID="radGrid1" Skin="Metro" AllowAutomaticDeletes="true"
            PageSize="50" AllowPaging="True" Style="margin-top: 0px" >
            <MasterTableView TableLayout="Auto" AutoGenerateColumns="False" DataKeyNames="ReimburseType_Code" ShowFooter="true" ShowHeadersWhenNoRecords="true" > 
                <NoRecordsTemplate> 
                    No records found.  
                </NoRecordsTemplate> 
                <Columns>
                    <telerik:GridBoundColumn UniqueName="ReimburseType_Code" DataField="ReimburseType_Code" HeaderText="ReimburseType_Code" Visible="false"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="Name" HeaderText="Reimburse Type Name" DataField="Name"></telerik:GridBoundColumn>
                    <telerik:GridCheckBoxColumn UniqueName="IsInactive" HeaderText="InActive" DataField="IsInactive"></telerik:GridCheckBoxColumn>
                    <telerik:GridEditCommandColumn UniqueName="Edit" HeaderText="Edit" ButtonType="ImageButton"></telerik:GridEditCommandColumn>
                    <telerik:GridButtonColumn UniqueName="Delete" CommandName="Delete" ConfirmText="Are you sure you want to delete this Detail ?" HeaderText="Delete Entire Claim" ButtonType="ImageButton">                        
                    </telerik:GridButtonColumn>                                                            
                    <telerik:GridTemplateColumn UniqueName="GridTemplateColumn">                        
                        <FooterTemplate>
                            <asp:Button ID="btnInsert" runat="server" CssClass="mybutton" Text="Insert New Reimbursement Type" ValidationGroup="contvalidate" />
                        </FooterTemplate>
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
