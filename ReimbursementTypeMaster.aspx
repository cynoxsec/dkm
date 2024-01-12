<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/reimburseMasterPage.Master" CodeBehind="ReimbursementTypeMaster.aspx.vb" Inherits="DkmOnlineWeb.ReimbursementTypeMaster" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>Reimbursement Type</title>
    <link href="ReimburesmentStyleSheet.css" rel="stylesheet" type="text/css" />

    <telerik:RadScriptBlock runat="server" ID="radScriptBlock">

        <link rel="Stylesheet" href="../Content/MyStyleSheet.css" type="text/css" />
        <style type="text/css">
            .Home {
                margin-top: -1px !important;
                padding-top: 0px !important;
                border-top: 4px solid #2d4a98 !important;
                background-color: #fff;
                background-image: linear-gradient(#f0f0f0,#fff);
                height: 35px;
            }

                .Home span {
                    padding: 1px 15px !important;
                }
            /*.RadWindow {
                       top: 10px !important;
               }*/
              .textboxinput:focus {
    text-align:right;
}
               .radiolistcss {
        display:inline-table;
    }
 .radiolistcss input {
            display:inherit;
           
            
        }
.radiolistcss label {
            padding-left:8px;
            padding-right:15px;
            margin-top:-1px;
            font-size: 14px;
        }
        </style>

        <script type="text/javascript">

            function noteditable() {
                alert("Claim window is closed for current month processing, you can submit it later next month.");
            }

        </script>
        <script type="text/javascript">

            function openRadWindow1(Url) {
                var oWnd = window.$find("<%= windowmanager2.ClientID%>");
                oWnd.setUrl(Url);
                oWnd.show();
                var windowWidth = 1050;
                var windowHeight = 600;
                oWnd.setSize(windowWidth, windowHeight); // var windowWidth = document.getElementsByTagName('body')[0].clientWidth; 
                oWnd.center();
                return false;
            }
            
        </script>
       
    </telerik:RadScriptBlock>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <telerik:RadWindow runat="server" ID="windowmanager2" ShowContentDuringLoad="false" RegisterWithScriptManager="true" ReloadOnShow="true" VisibleStatusbar="false" Modal="True" Behaviors="Close,Move" Width="1800" Height="500" Style="z-index: 99999;">
    </telerik:RadWindow>

      
    <telerik:RadWindow runat="server" ID="RadWindow1declaration" ShowContentDuringLoad="false" RegisterWithScriptManager="false" ReloadOnShow="true" VisibleStatusbar="false" Modal="True" Behaviors="Move" Width="850" Height="500" Style="z-index: 999999999 !important">
      <ContentTemplate>
           <div style="padding:35px;">

               <asp:RadioButtonList ID="RadioButtonList1" runat="server" RepeatDirection="Horizontal"  CssClass="radiolistcss"  >
                   <asp:ListItem Text="Submit reimbursement declaration" Value="Submit reimbursement declaration"></asp:ListItem>
                   <asp:ListItem Text="Submit reimbursement proof" Value="Submit reimbursement proof"></asp:ListItem>
               </asp:RadioButtonList>
           
        </div>
          </ContentTemplate>
         
    </telerik:RadWindow>

    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="radGridSummary">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="radGridSummary"></telerik:AjaxUpdatedControl>
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="radGridSummary2">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="radGridSummary2"></telerik:AjaxUpdatedControl>
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <div align="center"><u><b>Reimbursement Summary</b></u></div>
    <div style="clear: both"></div>
    
    <div id="DataDiv">
        <div align="left" id="div1" runat="server" visible="false"><u><b>Reimbursement master basis last payroll run of current FY</b></u></div>
        <telerik:RadGrid runat="server" ID="radGridSummary" Skin="Metro"
            PageSize="50" AllowPaging="True">
            <MasterTableView TableLayout="Fixed" DataKeyNames="Field_Name" AutoGenerateColumns="false" ShowHeadersWhenNoRecords="true" NoDetailRecordsText="No records found">
                <Columns>
                    <telerik:GridTemplateColumn UniqueName="Edit" DataField="Field_Name" AllowFiltering="false" HeaderText="" HeaderStyle-Width="60px">
                        <ItemTemplate>
                            <asp:LinkButton ForeColor="Black" runat="server" ID="lnkForViewReport1" Text="View Details"></asp:LinkButton>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn UniqueName="Field_Name" HeaderText="Reimburse Type" DataField="Field_Name" HeaderStyle-Width="120px"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="Budget_WEF" HeaderText="Entitlement From" DataField="Budget_WEF" HeaderStyle-Width="65px"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="Budget_WET" HeaderText="Entitlement To" DataField="Budget_WET" HeaderStyle-Width="55px"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="Opening" HeaderText="Opening" DataField="Opening" HeaderStyle-Width="40px"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="Budget" HeaderText="Entitlement" DataField="Budget" HeaderStyle-Width="40px"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="Claimed_Amount" HeaderText="Claimed" DataField="Claimed_Amount" HeaderStyle-Width="40px"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="Reimbursed_Amount" HeaderText="Paid" DataField="Reimbursed_Amount" HeaderStyle-Width="40px"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="Balance_Amount" HeaderText="Balance" DataField="Balance_Amount" HeaderStyle-Width="40px"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="BillsCF" HeaderText="Bills C/F" DataField="BillsCF" HeaderStyle-Width="40px"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="BillsTobesubmit" HeaderText="Bills to be submitted/payable as taxable" DataField="BillsTobesubmit" HeaderStyle-Width="70px"></telerik:GridBoundColumn>

                
                    <telerik:GridTemplateColumn UniqueName="Abudget" AllowFiltering="false" HeaderText="" HeaderStyle-Width="40px" Visible="false">
                        <ItemTemplate>
                            <asp:LinkButton ForeColor="Black" runat="server" ID="lnkAbudget" Text='<%# Eval("aBudget")%>'>'></asp:LinkButton>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>

                    
                </Columns>
            </MasterTableView>

            <GroupingSettings CaseSensitive="false" />
            <HeaderStyle Font-Bold="true" CssClass="left" BackColor="#ff6b1c" Font-Size="13px" ForeColor="White"></HeaderStyle>
            <ItemStyle CssClass="left"></ItemStyle>
            <AlternatingItemStyle />
            <PagerStyle Mode="NextPrevAndNumeric" Position="Bottom"></PagerStyle>
        </telerik:RadGrid>
        <br />
        <hr />
        <div align="left" id="div2" runat="server" visible="false"><u><b>Addition in Reimbursement master basis new FBP declaration</b></u></div>
        <telerik:RadGrid runat="server" ID="radGridSummary2" Skin="Metro"
            PageSize="50" AllowPaging="True" OnNeedDataSource="radGridSummary2_NeedDataSource">
            <MasterTableView TableLayout="Fixed" DataKeyNames="Field_Name" AutoGenerateColumns="false" ShowHeadersWhenNoRecords="true" NoDetailRecordsText="No records found">
                <Columns>
                    <telerik:GridBoundColumn UniqueName="Field_Name" HeaderText="Reimburse Type" DataField="Field_Name" HeaderStyle-Width="140px"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="Budget_WEF" HeaderText="Entitlement From" DataField="Budget_WEF" HeaderStyle-Width="100px"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="Budget_WET" HeaderText="Entitlement To" DataField="Budget_WET" HeaderStyle-Width="100px"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="Budget" HeaderText="Entitlement Amount" DataField="Budget" HeaderStyle-Width="100px"></telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="Declaration Amount" HeaderStyle-Width="100px" UniqueName="reimdeclartionamt" >
                        <ItemTemplate>
                            <asp:TextBox ID="txtdeclaramt" runat="server" Text="0" placeholder="enter amount" CssClass="textboxinput" onfocus="this.select();"></asp:TextBox>
                             <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" Text="*" ToolTip="enter amount" ControlToValidate="txtdeclaramt" ValidationGroup="reimvalidation" SetFocusOnError="true" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                         <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" Enabled="True" TargetControlID="txtdeclaramt" ValidChars="1234567890" FilterMode="ValidChars">
</cc1:FilteredTextBoxExtender>
                            <asp:HiddenField ID="hidreimtypecode" runat="server" Value='<%# Eval("ReimburseType_Code")%>' />
                              <asp:HiddenField ID="hidreimfieldName" runat="server" Value='<%# Eval("reimfieldName")%>' />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
             <ValidationSettings ValidationGroup="reimvalidation"  />
            <GroupingSettings CaseSensitive="false" />
            <HeaderStyle Font-Bold="true" CssClass="left" BackColor="#ff6b1c" Font-Size="13px" ForeColor="White"></HeaderStyle>
            <ItemStyle CssClass="left"></ItemStyle>
            <AlternatingItemStyle />
            <PagerStyle Mode="NextPrevAndNumeric" Position="Bottom" ></PagerStyle>
        </telerik:RadGrid>
        <div>
            <asp:Button ID="btnreimdeclaration" runat="server" Text="Submit" CssClass="mybutton" style="float:right;margin-right:6%;" ValidationGroup="reimvalidation" OnClick="btnreimdeclaration_Click"/>
        </div>
        
    </div>
    <asp:Label ID="Label1" runat="server" Text="" Visible="false"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
