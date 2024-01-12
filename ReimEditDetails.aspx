<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/reimburseMasterPage.Master" CodeBehind="ReimEditDetails.aspx.vb" Inherits="DkmOnlineWeb.ReimEditDetails" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <telerik:RadScriptBlock runat="server" ID="radScriptBlock">

        <script src="../Script/Jquery1.8min.js" type="text/javascript"></script>
        <script type="text/javascript">
            function openRadWindow(Url) {
                window.location = Url;
                return false;
            }
        </script>


        <style type="text/css">
            .InnerHeaderStyle {
                background: #A9A9A9 !important;
                color: black !important; /*add more style definitions here*/
            }

            .InnerItemStyle {
                background: white !important;
                color: black !important; /*add more style definitions here*/
            }

            .InnerAlernatingItemStyle {
                background: white !important;
                color: black !important; /*add more style definitions here*/                            
            }
           .RadGrid_Metro .rgRow>td, .RadGrid_Metro .rgAltRow>td, .RadGrid_Metro .rgEditRow>td{              
    border-width: 1px 0px 1px 1px !important;
               
            }
            .MostInnerHeaderStyle {
                background: #D3D3D3 !important;
                font-size: 15px !important;
                font-weight: bold !important;
                color: black !important; /*add more style definitions here*/
            }

            .MostInnerItemStyle {
                background: white !important;
                color: black !important; /*add more style definitions here*/
            }

            .MostInnerAlernatingItemStyle {
                background: white !important;
                color: black !important; /*add more style definitions here*/
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
       <%-- <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="radGrid1">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="radGrid1"></telerik:AjaxUpdatedControl>
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>--%>

        <caption>
            <div align="left" style="background-color: peachpuff; padding: 5px;"><b>Note : Previous month's claims and processed claims can not be edited and deleted. Claims can not be edited or deleted post cut-off date of current month also.</b></div>
        </caption>
        <telerik:RadGrid runat="server" ID="radGrid1" Skin="Metro" AllowAutomaticDeletes="true" PageSize="50" AllowPaging="True" Style="margin-top: 0px">
            <MasterTableView TableLayout="Auto" AutoGenerateColumns="False" DataKeyNames="ReimburseDetailsID" Name="Parent" ClientDataKeyNames="ReimburseDetailsID">
                <DetailTables>
                    <telerik:GridTableView Name="ChildGrid" Width="100%" AllowSorting="false" DataKeyNames="ReimburseDetailsID" AutoGenerateColumns="false">
                        <Columns>
                            <telerik:GridBoundColumn UniqueName="MultipleClaimDetailsID" DataField="MultipleClaimDetailsID" HeaderText="Sub Claim ID"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn UniqueName="BillNumber" DataField="BillNumber" HeaderText="Bill Number"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn UniqueName="BillDated" DataField="BillDated" HeaderText="Bill Date"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn UniqueName="BillAmount" DataField="BillAmount" HeaderText="Bill Amount" DataFormatString="{0:F0}"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn UniqueName="Claimstatus" HeaderText=" Claim status" DataField="Claimstatus"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn UniqueName="Approvedamount" HeaderText="Approved Amount" DataField="Approvedamount" DataFormatString="{0:F0}"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn UniqueName="Remarks" HeaderText="Remarks" DataField="Remarks"></telerik:GridBoundColumn>
                        </Columns>
                        <ExpandCollapseColumn HeaderText="Claim"></ExpandCollapseColumn>
                        <HeaderStyle CssClass="MostInnerHeaderStyle" />
                        <ItemStyle CssClass="MostInnerItemStyle" />
                        <AlternatingItemStyle CssClass="MostInnerAlernatingItemStyle" />
                    </telerik:GridTableView>
                </DetailTables>
                <Columns>
                    <telerik:GridBoundColumn UniqueName="Sl.No" DataField="Sl.No" HeaderText="Sl.No"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="ReimburseDetailsID" DataField="ReimburseDetailsID" HeaderText="Claim ID"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="Field_Name" HeaderText="ReimburseType" DataField="Field_Name"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="EntryDate" HeaderText="Claim date" DataField="EntryDate"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="BillAmount" HeaderText="Claim Amount" DataField="BillAmount" DataFormatString="{0:F0}"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="Claimstatus" HeaderText=" Claim status" DataField="Claimstatus"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="Approvedamount" HeaderText="Approved Amount" DataField="Approvedamount" DataFormatString="{0:F0}"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="Remarks" HeaderText="Remarks" DataField="Remarks"></telerik:GridBoundColumn>
                    <telerik:GridEditCommandColumn UniqueName="Edit" HeaderText="Edit" ButtonType="ImageButton"></telerik:GridEditCommandColumn>
                    <telerik:GridButtonColumn UniqueName="Delete" CommandName="Delete" ConfirmText="Are you sure you want to delete this Detail ?" HeaderText="Delete Entire Claim" ButtonType="ImageButton"></telerik:GridButtonColumn>
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

            <HeaderStyle Width="200px" Font-Bold="true" CssClass="InnerHeaderStyle"></HeaderStyle>
            <ItemStyle ForeColor="Black" CssClass="InnerItemStyle"></ItemStyle>
            <AlternatingItemStyle ForeColor="Black" CssClass="InnerAlernatingItemStyle" />

            <PagerStyle Mode="NextPrevNumericAndAdvanced" Position="Bottom"></PagerStyle>
        </telerik:RadGrid>

        <br />
        <br />
        <caption>
            <div id="div1" runat="server" visible="false" align="left" style="background-color: peachpuff; padding: 5px;">
                <b>
                    <asp:Literal ID="Literal1" runat="server"></asp:Literal></b>
            </div>
        </caption>
        <telerik:RadGrid runat="server" ID="radGrid2" Skin="Metro" AllowAutomaticDeletes="true"
            PageSize="50" AllowPaging="True" Style="margin-top: 0px" Visible="false">
            <MasterTableView TableLayout="Auto" AutoGenerateColumns="False" DataKeyNames="ReimburseDetailsID" Name="Parent" ClientDataKeyNames="ReimburseDetailsID">
                <Columns>
                    <telerik:GridBoundColumn UniqueName="Sl.No" DataField="Sl.No" HeaderText="Sl.No"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="ReimburseDetailsID" DataField="ReimburseDetailsID" HeaderText="Claim ID"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="Field_Name" HeaderText="ReimburseType" DataField="Field_Name"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="EntryDate" HeaderText="Claim date" DataField="EntryDate"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="BillAmount" HeaderText="Claim Amount" DataField="BillAmount" DataFormatString="{0:F0}"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="Claimstatus" HeaderText=" Claim status" DataField="Claimstatus"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="Approvedamount" HeaderText="Approved Amount" DataField="Approvedamount" DataFormatString="{0:F0}"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="Remarks" HeaderText="Remarks" DataField="Remarks"></telerik:GridBoundColumn>
                    <telerik:GridEditCommandColumn UniqueName="Edit" HeaderText="Edit" ButtonType="ImageButton"></telerik:GridEditCommandColumn>
                    <telerik:GridButtonColumn UniqueName="Delete" CommandName="Delete" ConfirmText="Are you sure you want to delete this Detail ?" HeaderText="Delete Entire Claim" ButtonType="ImageButton"></telerik:GridButtonColumn>
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
