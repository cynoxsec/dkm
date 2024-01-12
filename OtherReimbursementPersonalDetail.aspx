<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="OtherReimbursementPersonalDetail.aspx.vb" Inherits="DkmOnlineWeb.OtherReimbursementPersonalDetail" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Reimbursement Personal Details</title>
    <script src="../Script/Jquery1.8min.js"></script>
    <link href="ReimburesmentStyleSheet.css" rel="stylesheet" />
    <telerik:RadScriptBlock runat="server" ID="radScriptBlock">
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

            function TextBoxBankAC(sender, args) {

                if (isNaN(document.getElementById("<%= txtJourney.ClientID %>").value)) {
                    args.IsValid = false;
                }
            }
        </script>
    </telerik:RadScriptBlock>

    <style type="text/css">
        .field-validation-error {
            color: #E51400;
            font-weight: bold;
            font-size: 1.3em;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />

        <table>
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblTravellerName" Text="Name of Traveller"></asp:Label>
                </td>
                <td>
                    <span style="padding-left: 10px;">
                        <asp:TextBox runat="server" ID="txtTravellerName"></asp:TextBox>
                    </span>
                </td>
                <td>
                    <span style="padding-left: 10px;">
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                            ControlToValidate="txtTravellerName"
                            ErrorMessage="*"></asp:RequiredFieldValidator>
                    </span>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label runat="server" ID="lblRelation" Text="Relationship with Employee"></asp:Label>
                </td>
                <td>
                    <span style="padding-left: 10px;">
                        <asp:DropDownList runat="server" ID="ddlRelation" AutoPostBack="true"
                            AppendDataBoundItems="true">
                        </asp:DropDownList>
                    </span>
                </td>
                <td>
                    <span style="padding-left: 10px;">
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server"
                            ControlToValidate="ddlRelation"
                            ErrorMessage="*" Enabled="true"></asp:RequiredFieldValidator></span>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label runat="server" ID="Label1" Text="Dependent (Y/N)"></asp:Label>
                </td>
                <td>
                    <span style="padding-left: 10px;">
                        <asp:DropDownList runat="server" ID="ddlDependence" AppendDataBoundItems="true"></asp:DropDownList>
                    </span>
                </td>
                <td>
                    <span style="padding-left: 10px;">
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server"
                            ControlToValidate="ddlDependence"
                            ErrorMessage="*" Enabled="true"></asp:RequiredFieldValidator></span>
                </td>
            </tr>



            <tr>
                <td>
                    <asp:Label runat="server" ID="lblFrom" Text="Travel From"></asp:Label>
                </td>
                <td>
                    <span style="padding-left: 10px;">
                        <asp:TextBox runat="server" ID="txtFrom"></asp:TextBox>
                    </span>
                </td>
                <td>
                    <span style="padding-left: 10px;">
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server"
                            ControlToValidate="txtFrom"
                            ErrorMessage="*"></asp:RequiredFieldValidator>
                    </span>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label runat="server" ID="lblTo" Text="Travel To"></asp:Label>
                </td>
                <td style="padding-left: 10px;">
                    <asp:TextBox runat="server" ID="txtTodate"></asp:TextBox>
                </td>
                <td><span style="padding-left: 10px;">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server"
                        ControlToValidate="txtTodate"
                        ErrorMessage="*"></asp:RequiredFieldValidator>
                </span>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label runat="server" ID="Label2" Text="Travel on"></asp:Label>
                </td>
                <td style="padding-left: 10px;">
                    <telerik:RadDatePicker runat="server" ID="radDatePicker" DateInput-DateFormat="dd/MM/yyyy" DateInput-DisplayDateFormat="dd/MM/yyyy">
                    </telerik:RadDatePicker>
                </td>
                <td><span style="padding-left: 10px;">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
                        ControlToValidate="radDatePicker"
                        ErrorMessage="*"></asp:RequiredFieldValidator>
                </span>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label runat="server" ID="Label3" Text="Mode of Journey"></asp:Label>
                </td>
                <td style="padding-left: 10px;">
                    <asp:DropDownList runat="server" ID="ddlMode" AppendDataBoundItems="true"></asp:DropDownList>
                </td>
                <td><span style="padding-left: 10px;">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server"
                        ControlToValidate="ddlMode"
                        ErrorMessage="*" Enabled="true"></asp:RequiredFieldValidator></span>
                </td>

            </tr>

            <tr>
                <td>
                    <asp:Label runat="server" ID="Label4" Text="Type of Journey"></asp:Label>
                </td>
                <td style="padding-left: 10px;">
                    <asp:DropDownList runat="server" ID="ddlType" AppendDataBoundItems="true"></asp:DropDownList>
                </td>
                <td><span style="padding-left: 10px;">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server"
                        ControlToValidate="ddlType"
                        ErrorMessage="*" Enabled="true"></asp:RequiredFieldValidator></span>
                </td>

            </tr>

            <tr>
                <td>
                    <asp:Label runat="server" ID="Label5" Text="Travel fare"></asp:Label>
                </td>
                <td style="padding-left: 10px;">
                    <asp:TextBox runat="server" ID="txtJourney"></asp:TextBox>
                     <cc1:filteredtextboxextender id="Filteredtextboxextender1" runat="server" filtertype="Numbers" TargetControlID="txtJourney"
                        validchars="0123456789" enabled="true"></cc1:filteredtextboxextender>
                </td>
                <td><span style="padding-left: 10px;">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                        ControlToValidate="txtJourney"
                        ErrorMessage="*"></asp:RequiredFieldValidator>

                    <asp:CustomValidator ID="CustomValidator2" runat="server"
                        ErrorMessage="*" ControlToValidate="txtJourney" Display="Dynamic" ClientValidationFunction="TextBoxBankAC" ForeColor="Red" ToolTip="Integer value is allowed."></asp:CustomValidator>
                </span>
                </td>
            </tr>
            
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>

            <tr>
                <td colspan="3" style="text-align: center">
                    <asp:Button runat="server" ID="btnSave" CssClass="mybutton" Style="padding: 3px 15px 5px 15px !important" Text="Submit" />
                    <asp:Label ID="messageLabel1" runat="server" Text="Edit Only Current Month" ForeColor="Red" Visible="false"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <asp:Label ID="lblError" runat="server" CssClass="field-validation-error"></asp:Label></td>
            </tr>
        </table>
    </form>
</body>
</html>
