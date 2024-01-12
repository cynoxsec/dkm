<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SiteHrpMasterPage.Master" CodeBehind="ApproveRejectReim.aspx.vb" Inherits="DkmOnlineWeb.ApproveRejectReim" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <telerik:RadScriptBlock runat="server" ID="radScriptBlock">
        <link rel="Stylesheet" href="../Content/MyStyleSheet.css" type="text/css" />

        <style type="text/css">
            .header {
                background-color: #023A4D;
                color: white;
                text-align: center;
                padding: 5px;
            }
        </style>
    </telerik:RadScriptBlock>
    <style type="text/css">
        .auto-style1 {
            width: 396px;
        }

        .linkbtn {
            color: blue;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager runat="server" ID="scripmamager"></asp:ScriptManager>
    <div style="padding-top: 10px; font-size: large; font-weight: bold; text-align: center;">
        <u>
            <asp:Label runat="server" ID="lbldeclaration" Font-Size="25px" ForeColor="#333333">Reimbursement Claim status</asp:Label></u>
    </div>
    <div>
        <table>
            <tr>
                <td>
                    <asp:LinkButton runat="server" ID="lnkViewTemplate" CausesValidation="false" Text="Click to download Template."
                        CssClass="linkbtn"></asp:LinkButton>
                </td>
            </tr>
            <tr>

                <td class="auto-style4">
                    <asp:Label ID="lblFileUpload" runat="server" Visible="True">Upload Reimbursement Claim status ( CSV File )</asp:Label>
                </td>
                <td class="auto-style2">
                    <telerik:RadAsyncUpload runat="server" ID="radAsynUpload" Height="45px" Width="216px" MaxFileInputsCount="1" MultipleFileSelection="Disabled"></telerik:RadAsyncUpload>
                </td>
                <td>
                    <asp:Button runat="server" ID="btnUpload" Text="Upload Csv file" CssClass="mybutton" /></td>
            </tr>

            <tr>
                <td class="auto-style4">&nbsp;</td>
                <td class="auto-style2">&nbsp;</td>
                <td>
                    <asp:Button ID="btnSendTo" runat="server" Text="Send E-Mail To Employees" CssClass="mybutton" />
                </td>
            </tr>
        </table>
    </div>

    <hr />
    <asp:Label ID="lblError" runat="server" CssClass="field-validation-error"></asp:Label>
    <hr />
    <div style="padding-top: 10px;">
        <table>
            <tr>
                <td><b>Select Year</b></td>
                <td>
                    <asp:DropDownList runat="server" ID="ddlyear">
                    </asp:DropDownList></td>
                <td>
                    <asp:Button ID="btndownload" runat="server" Text="Download" CssClass="mybutton" /></td>
            </tr>
        </table>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
