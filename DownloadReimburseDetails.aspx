<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SiteHrpMasterPage.Master" CodeBehind="DownloadReimburseDetails.aspx.vb" Inherits="DkmOnlineWeb.DownloadReimburseDetails" %>


<asp:content id="Content1" contentplaceholderid="HeadContent" runat="server">
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
</asp:content>
<asp:content id="Content2" contentplaceholderid="FeaturedContent" runat="server">
</asp:content>
<asp:content id="Content3" contentplaceholderid="MainContent" runat="server">
<telerik:RadScriptManager runat="server" ID="radScriptMgr"></telerik:RadScriptManager>
            <div style="text-align: center; border: 1px solid #ddd; padding: 5px; border-bottom: none; background: #ff6b1c; color: white">
                <asp:Label runat="server" ID="Reimbursement_Card" Text="Reimbursement Report"></asp:Label>
            </div>

            <table>

                <tr id="Tr1" runat="server" >
                    <td></td>
                    <td>From Date :
                    <telerik:RadDatePicker runat="server" ID="radFromDate" Width="150px" DateInput-DisplayDateFormat="dd/MM/yyyy" DateInput-DateFormat="dd/MM/yyyy">
                    </telerik:RadDatePicker>
                    </td>
                    <td>To Date
                    <telerik:RadDatePicker runat="server" ID="radToDate" Width="150px" DateInput-DisplayDateFormat="dd/MM/yyyy" DateInput-DateFormat="dd/MM/yyyy">
                    </telerik:RadDatePicker>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td></td>
                    <td style="align: right;">
                        <asp:Button ID="btndownload" runat="server" Text="Download" CssClass="mybutton" /></td>
                </tr>
            </table>



    <div id="declarediv" runat="server" style="display:none">
         <br />
            <hr />
   <br />
        <div style="text-align: center; border: 1px solid #ddd; padding: 5px; border-bottom: none; background: #ff6b1c; color: white">
                <asp:Label runat="server" ID="Label1" Text="Reimbursement Declaration Report"></asp:Label>
            </div>
        <table>
            <tr>
                <td><b>Select Year</b></td>
                <td>
                    <asp:DropDownList runat="server" ID="ddlyear">
                    </asp:DropDownList></td>
                <td>
                    <asp:Button ID="btndownloadreimdeclaraton" runat="server" Text="Download" CssClass="mybutton" OnClick="btndownloadreimdeclaraton_Click" /></td>
            </tr>
        </table>
    </div>

</asp:content>
<asp:content id="Content4" contentplaceholderid="FooterContent" runat="server">
</asp:content>
