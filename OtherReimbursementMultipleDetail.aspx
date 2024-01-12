<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="OtherReimbursementMultipleDetail.aspx.vb" Culture="en-AU" Inherits="DkmOnlineWeb.OtherReimbursementMultipleDetail" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Multiple Reimbursement</title>
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
                    <asp:Label runat="server" ID="lblBillNumber" Text="Bill Number"></asp:Label>
                </td>
                <td>
                    <span style="padding-left: 10px;">
                        <asp:TextBox runat="server" ID="txtBillNo"></asp:TextBox>
                        <%--<cc1:filteredtextboxextender id="NumbervalidBillAoumnt0" runat="server" filtertype="Numbers" TargetControlID="txtBillNo"
                        validchars="0123456789" enabled="true">
                    </cc1:filteredtextboxextender>--%>
                    </span>
                </td>
                <td>
                    <span style="padding-left: 10px;">
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ForeColor="Red" ToolTip="Required Field"
                            ControlToValidate="txtBillNo"
                            ErrorMessage="*">
                        </asp:RequiredFieldValidator>
                    </span>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label runat="server" ID="lblBillDate" Text="Bill Date"></asp:Label>
                </td>
                <td>
                    <span style="padding-left: 10px;">                        
                        <telerik:RadDatePicker runat="server" ID="radDatePickerBillDate" DateInput-DateFormat="dd/MM/yyyy" DateInput-DisplayDateFormat="dd/MM/yyyy" >                        
                        </telerik:RadDatePicker>
                    </span>
                </td>
                <td>
                    <span style="padding-left: 10px;">
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ForeColor="Red" ToolTip="Required Field"
                            ControlToValidate="radDatePickerBillDate"
                            ErrorMessage="*"></asp:RequiredFieldValidator>
                    </span>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label runat="server" ID="lblBillAmount" Text="Bill Amount"></asp:Label>
                </td>
                <td>
                    <span style="padding-left: 10px;">
                        <asp:TextBox runat="server" ID="txtBillAmt"></asp:TextBox>
                        <cc1:filteredtextboxextender id="Filteredtextboxextender1" runat="server" filtertype="Numbers" TargetControlID="txtBillAmt"
                        validchars="0123456789" enabled="true"></cc1:filteredtextboxextender>
                    </span>
                </td>
                <td>
                    <span style="padding-left: 10px;">
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ForeColor="Red" ToolTip="Required Field"
                            ControlToValidate="txtBillAmt"
                            ErrorMessage="*"></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="CustomValidator2" runat="server"
                            ErrorMessage="*" ControlToValidate="txtBillAmt" Display="Dynamic" ClientValidationFunction="TextBoxBankAC" ForeColor="Red" ToolTip="Integer value is allowed."></asp:CustomValidator>
                    </span>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label runat="server" ID="lblBillDetail" Text="Enter password (if document is encrypted) or any other info"></asp:Label>
                </td>
                <td>
                    <span style="padding-left: 10px;">
                        <asp:TextBox runat="server" ID="txtBillDetail" Visible="false" TextMode="MultiLine"></asp:TextBox>
                    </span>
                </td>
                <td><span style="padding-left: 10px;">
                    <asp:DropDownList runat="server" ID="ddlBillDetail" Visible="false" AutoPostBack="true" AppendDataBoundItems="true"></asp:DropDownList>
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
                    <asp:Button runat="server" ID="btnSave" CssClass="mybutton" style="padding:3px 15px 5px 15px !important" Text="Submit" />  
                    <asp:Label ID="messageLabel1" runat="server" ForeColor="Red" Visible="false"></asp:Label>                   
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
