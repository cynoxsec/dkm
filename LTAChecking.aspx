<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/reimburseMasterPage.Master" CodeBehind="LTAChecking.aspx.vb" Inherits="DkmOnlineWeb.LTAChecking" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <telerik:RadScriptBlock runat="server" ID="radScriptBlock">

        <script src="../Script/Jquery1.8min.js" type="text/javascript"></script>
        <script type="text/javascript">
            function openRadWindow(Url) {
                window.location = Url;
                return false;
            }
               function LTAConfirm() {                
                if (confirm('You are not eligible for carry-over LTA exemption')) {                    
                    var modalPopupBehaviorCtrl = $find('ModalPopupExtender1');                                        
                }
                else {                                           
                }
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
     <div>  
            <label id="lblusr" />  
        </div> 
    <div style="text-align: center; border: 1px solid #ddd; padding: 5px; border-bottom: none; background: #ff6b1c; color: white">
        <asp:Label runat="server" ID="lbl1" Text="LTA Check"></asp:Label>
    </div>
    <div id="ChecknewHire1" runat="server" visible="false">
        <table class="mytablecss">
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblChecknewHire1"></asp:Label></td>
                <td>
                    <asp:DropDownList ID="ddlChecknewHire1" runat="server">
                        <asp:ListItem Text="0" Value="0"></asp:ListItem>
                        <asp:ListItem Text="1" Value="1"></asp:ListItem>
                        <asp:ListItem Text="2" Value="2"></asp:ListItem>
                    </asp:DropDownList></td>
                <td>
                    <asp:Button ID="btnChecknewHire1" runat="server" Text="Submit" OnClick="btnChecknewHire1_Click" CssClass="mybutton" /></td>
            </tr>
        </table>
    </div>
    <div id="ChecknewHire2" runat="server" visible="false">
        <table class="mytablecss">
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblChecknewHire2"></asp:Label></td>
                <td>
                    <asp:DropDownList ID="ddlChecknewHire2" runat="server">
                        <asp:ListItem Text="0" Value="0"></asp:ListItem>
                        <asp:ListItem Text="1" Value="1"></asp:ListItem>
                        <asp:ListItem Text="2" Value="2"></asp:ListItem>
                    </asp:DropDownList></td>
                <td>
                    <asp:Button ID="btnChecknewHire2" runat="server" Text="Submit" OnClick="btnChecknewHire2_Click" CssClass="mybutton" /></td>
            </tr>
        </table>
    </div>
    <div id="ChecknewHire3" runat="server" visible="false">
        <table class="mytablecss">
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblChecknewHire3"></asp:Label></td>
                <td>
                    <asp:DropDownList ID="ddlChecknewHire3" runat="server">
                        <asp:ListItem Text="0" Value="0"></asp:ListItem>
                        <asp:ListItem Text="1" Value="1"></asp:ListItem>
                        <asp:ListItem Text="2" Value="2"></asp:ListItem>
                    </asp:DropDownList></td>
                <td>
                    <asp:Button ID="btnChecknewHire3" runat="server" Text="Submit" OnClick="btnChecknewHire3_Click" CssClass="mybutton" /></td>
            </tr>
        </table>
    </div>

   <telerik:RadWindow runat="server" ID="radWindow" ShowContentDuringLoad="false" RegisterWithScriptManager="true" ReloadOnShow="true"
        VisibleStatusbar="false" Modal="True" Behaviors="Move" Width="500" Height="150" Style="z-index: 99999;">
        <ContentTemplate>
            <div class="body" style="background-color:white">                
            You are not eligible for further LTA exemption.
                <br />
                <br />
                <br />
                <center>
            <asp:Button ID="btnOk" runat="server" OnClick="btnOk_Click" Text="Ok"/>
            &nbsp;
            <asp:Button ID="btnHide" runat="server" OnClick="btnHide_Click" Text="Close"/>
                    </center>
        </div>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadWindow runat="server" ID="radWindow1" ShowContentDuringLoad="false" RegisterWithScriptManager="true" ReloadOnShow="true"
        VisibleStatusbar="false" Modal="True" Behaviors="Move" Width="500" Height="180" Style="z-index: 99999;">
        <ContentTemplate>
            <div class="body" style="background-color:white">                
            Do you want to resubmit the input furnished by you for LTA exemptions.
                <br />
                <br />
                <br />
                <center>
                    <asp:Button ID="btnOk1" runat="server" OnClick="btnOk1_Click" Text="Ok"/>
                    &nbsp;
                    <asp:Button ID="btnHide1" runat="server" OnClick="btnHide1_Click" Text="Close"/>
                </center>
            </div>
        </ContentTemplate>
    </telerik:RadWindow>


</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
