<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/reimburseMasterPage.Master" CodeBehind="ReimbursementLTA.aspx.vb" Inherits="DkmOnlineWeb.ReimbursementLTA" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <style type="text/css">
        input, textare {
            margin: 0px;
        }

        body {
            font-family: 'Segoe UI';
        }

        .radiolistcss input {
            display: inherit;
            float: left;
        }

        .radiolistcss label {
            padding-left: 25px;
            margin-top: -1px;
            font-size: 14px;
        }

        .radiolistcss label {
            font-weight: bold;
        }

        .SubmitReimbursement {
            margin-top: -1px !important;
            padding-top: 0px !important;
            border-top: 4px solid #2d4a98 !important;
            background-color: #fff;
            background-image: linear-gradient(#f0f0f0,#fff);
            height: 35px;
        }

            .SubmitReimbursement span {
                padding: 1px 15px !important;
            }
    </style>

    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js"></script>
    <script src="../Script/jquery-1.10.0.min.js"></script>
    <script src="../Script/jquery-ui.min.js"></script>
    <link href="../Content/query-ui.css" rel="stylesheet" />

    <telerik:RadScriptBlock runat="server" ID="radScriptBlock">

        <script type="text/javascript">

            $(document).ready(function () {
                $('#<% = ExtraField1.ClientID%>').blur(function () {

                    var m = $('#<% = ExtraField1.ClientID%>').val();   // car registration no
                    var l = $('#<% = hidmaxlen.ClientID%>').val();   // Reimbursement Type 
                    var n = $('#<% = hidwebtableid.ClientID%>').val();   // Company Web table id                    
                })
            });

            $(document).ready(function () {
                $('#<% = btnSave.ClientID%>').click(function () {
                    var re = Validate();
                    if (re == true) {
                        return true
                    }
                    else {
                        return false
                    }
                })
            });

            function Validate() {
                var sel = document.getElementById('<%=ExtraField4.ClientID%>');
                var text = sel.options[sel.selectedIndex].text;
                if (text == 'Please Select') {
                    alert("Please select the Mandatory Fields");
                    return false;
                }
                else {
                    return true;
                }
            }

            function Noopeningandwithalert() {
                alert("No more manual entries can be made. If you want then please go back to home page or click view/edit button below");
                return false;
            }

            function openRadWindow(Url) {
                var oWnd = window.$find("<%= windowmanager2.ClientID %>");
                oWnd.setUrl(Url);
                var urlfurther = Url;
                var n = urlfurther.toString().indexOf('whetherIsSaveEntry')

                if (n >= 0) {
                    var aftersplit = urlfurther.split("&");
                    var aftersplitfurther1 = aftersplit[1].split("=");
                    var aftersplitfurther1further = aftersplitfurther1[1]

                    if (aftersplitfurther1further == "N") {
                        oWnd.show();

                        if ((Url.indexOf("ReimbursementPolicy") != -1)) {
                            var windowWidth = 830;
                            var windowHeight = 650;
                        }
                        else {
                            var windowWidth = 600;
                            var windowHeight = 550;
                        }
                        oWnd.setSize(windowWidth, windowHeight); // var windowWidth = document.getElementsByTagName('body')[0].clientWidth; 
                        oWnd.center();
                        return false;
                    }
                    else {
                        Noopeningandwithalert();
                    }
                }

                else {
                    oWnd.show();

                    if ((Url.indexOf("ReimbursementPolicy") != -1)) {
                        var windowWidth = 830;
                        var windowHeight = 650;
                    }
                    else {
                        var windowWidth = 600;
                        var windowHeight = 550;
                    }
                    oWnd.setSize(windowWidth, windowHeight); // var windowWidth = document.getElementsByTagName('body')[0].clientWidth; 
                    oWnd.center();
                    return false;
                }
            }
            function openRadWindow12(Url) {
                window.location.href = Url
                return false;
            }
            function TextBoxBankAC(sender, args) {

                if (isNaN(document.getElementById("<%= txtTotalBillField.ClientID %>").value)) {
                    args.IsValid = false;
                }
            }

            function TextBoxBankAC1(sender, args) {
                if (isNaN(document.getElementById("<%= Amount.ClientID %>").value)) {
                    args.IsValid = false;
                }
            }
        </script>
    </telerik:RadScriptBlock>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="lnkTravel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="radGridTravel"></telerik:AjaxUpdatedControl>
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>

    </telerik:RadAjaxManager>



    <telerik:RadWindow runat="server" ID="windowmanager2" ShowContentDuringLoad="true" ReloadOnShow="true" VisibleStatusbar="false" Modal="True" Behaviors="Close,Move" Width="700" Height="500" Style="z-index: 99999;">
    </telerik:RadWindow>
    <asp:Panel ID="panel1" runat="server">
        <table width="100%" border="0" style="border-collapse: collapse;">
            <tr>
                <td colspan="5">
                    <div style="text-align: center; border: 1px solid #ddd; padding: 5px; border-bottom: none; margin-bottom: -8px; background: #ff6b1c; color: white">
                        <asp:Label ID="txtReimbursementType" runat="server"></asp:Label>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="5">
                    <h6>
                        <asp:Label ID="lblNote" runat="server" ForeColor="Red"></asp:Label></h6>
                </td>
            </tr>
            <tr>
                <td colspan="5">
                    <div class="BlueLabel">
                        <asp:Label ID="lblBalance" runat="server"></asp:Label>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="width: 200px">
                    <asp:Label ID="lblEntryDate" runat="server" Text="Claim date:" Visible="False"></asp:Label>
                </td>
                <td>

                    <telerik:RadDatePicker runat="server" ID="radDatepickerEntryDate" Width="150px" DateInput-DisplayDateFormat="dd/MM/yyyy" DateInput-DateFormat="dd/MM/yyyy" Visible="false" Enabled="false">
                    </telerik:RadDatePicker>


                </td>
                <td colspan="3" align="right">
                    <asp:HyperLink ID="linkPolicySection" runat="server"
                        NavigateUrl="#PolicySection" Visible="False" Style="color: #2D4A98 !important">Click to view reimbursement policy</asp:HyperLink>
                    &nbsp;<asp:Label ID="lstReimburseType" runat="server" Visible="False"></asp:Label>
                </td>
            </tr>
        </table>
    </asp:Panel>

    <hr />

    <asp:Panel ID="panelDates" Visible="false" runat="server">
        <table width="100%">
            <tr>
                <td style="width: 241px !important;">
                    <asp:Label ID="lblTransactionDate" runat="server"
                        Text="Date:"></asp:Label>
                    <asp:Label ID="lblDateFieldRequired" runat="server"
                        Text="*"></asp:Label>
                </td>
                <td style="width: 500px">
                    <telerik:RadDatePicker runat="server" ID="radDatePickerTransactionDate" Width="150px" DateInput-DisplayDateFormat="dd/MM/yyyy" DateInput-DateFormat="dd/MM/yyyy">
                    </telerik:RadDatePicker>
                    <asp:RequiredFieldValidator ID="validTransactionDate" runat="server" ValidationGroup="contvalidate"
                        ControlToValidate="radDatePickerTransactionDate" Enabled="false" ForeColor="Red" Display="Dynamic"
                        ErrorMessage="Field can not left blank !">
                    </asp:RequiredFieldValidator>
                </td>
                <td style="width: 241px !important;">
                    <asp:Label ID="lblExtraDateField" runat="server"
                        Visible="False">Extra Date</asp:Label>
                    <asp:Label ID="lblReqExtraDateField" CssClass="fontred" runat="server"
                        Text="*" Visible="False"></asp:Label>
                </td>
                <td style="margin-left: 40px; width: 500px">

                    <telerik:RadDatePicker runat="server" ID="radDatepickerExtraDateField" Width="150px" Visible="false" DateInput-DisplayDateFormat="dd/MM/yyyy" DateInput-DateFormat="dd/MM/yyyy">
                    </telerik:RadDatePicker>

                    <asp:RequiredFieldValidator ID="validExtraDateField" runat="server" ValidationGroup="contvalidate"
                        ControlToValidate="radDatepickerExtraDateField" Enabled="false" ForeColor="Red" Display="Dynamic"
                        ErrorMessage="Field can not left blank !"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td style="width: 200px">
                    <asp:Label ID="lblExtraField7" runat="server"
                        Text="ExtraField7" Visible="false"></asp:Label>
                    <asp:Label ID="lblReq7" CssClass="fontred" runat="server" Text="*" Visible="false"></asp:Label>

                </td>
                <td>
                    <telerik:RadDatePicker runat="server" ID="radExtraField7" Width="150px" Visible="false" DateInput-DisplayDateFormat="dd/MM/yyyy" DateInput-DateFormat="dd/MM/yyyy">
                    </telerik:RadDatePicker>
                    <asp:RequiredFieldValidator ID="validEF7" runat="server" ValidationGroup="contvalidate"
                        ControlToValidate="radExtraField7" Enabled="false" ForeColor="Red" Display="Dynamic"
                        ErrorMessage="Field can not left blank !">
                    </asp:RequiredFieldValidator>
                </td>
                <td style="width: 200px">
                    <asp:Label ID="lblExtraField8" runat="server"
                        Text="ExtraField8" Visible="false"></asp:Label>
                    <asp:Label ID="lblReq8" CssClass="fontred" runat="server"
                        Text="*" Visible="False"></asp:Label>
                </td>
                <td style="margin-left: 40px">

                    <telerik:RadDatePicker runat="server" ID="radExtraField8" Width="150px" Visible="false" DateInput-DisplayDateFormat="dd/MM/yyyy" DateInput-DateFormat="dd/MM/yyyy">
                    </telerik:RadDatePicker>

                    <asp:RequiredFieldValidator ID="validEF8" runat="server" ValidationGroup="contvalidate"
                        ControlToValidate="radExtraField8" Enabled="false" ForeColor="Red" Display="Dynamic"
                        ErrorMessage="Field can not left blank !"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="panelDetail" Visible="False" runat="server">
        <table width="100%">
            <tr>
                <td style="width: 300px">
                    <asp:Label ID="lblExtraField1" runat="server"
                        Text="ExtraField1" Visible="False"></asp:Label>
                    <asp:Label ID="lblReq1" CssClass="fontred" runat="server" Text="*"
                        Visible="False"></asp:Label>
                </td>
                <td style="width: 500px">
                    <asp:TextBox ID="ExtraField1" runat="server"
                        Font-Size="8pt" Height="25px" MaxLength="50"
                        TextMode="MultiLine" Width="223px"
                        Visible="False"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="validEF1" runat="server"
                        ControlToValidate="ExtraField1" Enabled="false" ForeColor="Red" Display="Dynamic"
                        ErrorMessage="Field can not left blank !" ValidationGroup="contvalidate"></asp:RequiredFieldValidator>
                    <cc1:FilteredTextBoxExtender ID="NumbervalidEF1" runat="server" FilterType="Numbers" TargetControlID="ExtraField1"
                        ValidChars="0123456789" Enabled="false">
                    </cc1:FilteredTextBoxExtender>
                    <cc1:FilteredTextBoxExtender ID="TextvalidEF1" runat="server" FilterType="UppercaseLetters, LowercaseLetters,custom"
                        ValidChars=" "
                        Enabled="false"
                        TargetControlID="ExtraField1" />
                    <cc1:FilteredTextBoxExtender ID="AllEF1" runat="server" FilterType="UppercaseLetters, LowercaseLetters,custom,Numbers"
                        ValidChars=" "
                        Enabled="false"
                        TargetControlID="ExtraField1" />
                </td>
                <td style="width: 300px">
                    <asp:Label ID="lblExtraField2" runat="server"
                        Text="ExtraField2" Visible="False"></asp:Label>
                    <asp:Label ID="lblReq2" CssClass="fontred" runat="server" Text="*"
                        Visible="False"></asp:Label>
                </td>
                <td style="margin-left: 40px; width: 500px">
                    <asp:TextBox ID="ExtraField2" runat="server"
                        Font-Size="8pt" Height="25px" MaxLength="50"
                        TextMode="MultiLine" Visible="False" Width="223px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="validEF2" runat="server"
                        ControlToValidate="ExtraField2" Enabled="false" ForeColor="Red" Display="Dynamic"
                        ErrorMessage="Field can not left blank !" ValidationGroup="contvalidate"></asp:RequiredFieldValidator>
                    <cc1:FilteredTextBoxExtender ID="NumbervalidEF2" runat="server" FilterType="Numbers" TargetControlID="ExtraField2"
                        ValidChars="0123456789" Enabled="false">
                    </cc1:FilteredTextBoxExtender>
                    <cc1:FilteredTextBoxExtender ID="TextvalidEF2" runat="server" FilterType="UppercaseLetters, LowercaseLetters,custom"
                        ValidChars=" "
                        Enabled="false"
                        TargetControlID="ExtraField2" />
                    <cc1:FilteredTextBoxExtender ID="AllEF2" runat="server" FilterType="UppercaseLetters, LowercaseLetters,custom,Numbers"
                        ValidChars=" "
                        Enabled="false"
                        TargetControlID="ExtraField2" />
                </td>
            </tr>
            <tr>
                <td style="width: 200px">
                    <asp:Label ID="lblExtraField3" runat="server"
                        Text="ExtraField3" Visible="False"></asp:Label>
                    <asp:Label ID="lblReq3" CssClass="fontred" runat="server" Text="*"
                        Visible="False"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="ExtraField3" runat="server"
                        Font-Size="8pt" Height="25px" MaxLength="50"
                        Visible="False" Width="223px"
                        TextMode="MultiLine"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="validEF3" runat="server"
                        ControlToValidate="ExtraField3" Enabled="false" ForeColor="Red" Display="Dynamic"
                        ErrorMessage="Field can not left blank !" ValidationGroup="contvalidate"></asp:RequiredFieldValidator>
                    <cc1:FilteredTextBoxExtender ID="NumbervalidEF3" runat="server" FilterType="Numbers" TargetControlID="ExtraField3"
                        ValidChars="0123456789" Enabled="false">
                    </cc1:FilteredTextBoxExtender>
                    <cc1:FilteredTextBoxExtender ID="TextvalidEF3" runat="server" FilterType="UppercaseLetters, LowercaseLetters,custom"
                        ValidChars=" "
                        Enabled="false"
                        TargetControlID="ExtraField3" />
                    <cc1:FilteredTextBoxExtender ID="AllEF3" runat="server" FilterType="UppercaseLetters, LowercaseLetters,custom,Numbers"
                        ValidChars=" "
                        Enabled="false"
                        TargetControlID="ExtraField3" />
                </td>
                <td>
                    <asp:Label ID="lblExtraField5" runat="server"
                        Visible="False" Text="ExtraField5"></asp:Label>
                    <asp:Label ID="lblReq5" CssClass="fontred" runat="server" Text="*"
                        Visible="False"></asp:Label>
                </td>
                <td style="width: 587px">
                    <asp:TextBox ID="ExtraField5" runat="server"
                        Font-Size="8pt" Height="25px" MaxLength="50"
                        TextMode="MultiLine" Visible="False" Width="223px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="validEF5" runat="server"
                        ControlToValidate="ExtraField5" CssClass="fontred" Enabled="false" ForeColor="Red" Display="Dynamic"
                        ErrorMessage="Field can not left blank !" ValidationGroup="contvalidate"></asp:RequiredFieldValidator>
                    <cc1:FilteredTextBoxExtender ID="NumbervalidEF5" runat="server" FilterType="Numbers" TargetControlID="ExtraField5"
                        ValidChars="0123456789" Enabled="false">
                    </cc1:FilteredTextBoxExtender>
                    <cc1:FilteredTextBoxExtender ID="TextvalidEF5" runat="server" FilterType="UppercaseLetters, LowercaseLetters,custom"
                        ValidChars=" "
                        Enabled="false"
                        TargetControlID="ExtraField5" />
                    <cc1:FilteredTextBoxExtender ID="AllEF5" runat="server" FilterType="UppercaseLetters, LowercaseLetters,custom,Numbers"
                        ValidChars=" "
                        Enabled="false"
                        TargetControlID="ExtraField5" />

                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblExtraField4" runat="server"
                        Text="ExtraField4" Visible="False"></asp:Label>
                    <asp:Label ID="lblReq4" CssClass="fontred" runat="server" Text="*"
                        Visible="False"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ExtraField4" runat="server"
                        Font-Size="8pt" Visible="False" Width="200px">
                        <asp:ListItem Value=" "> </asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="validEF4" runat="server"
                        ControlToValidate="ExtraField4" Enabled="false" ForeColor="Red" Display="Dynamic" InitialValue="0"
                        ErrorMessage="Field can not left blank !" ValidationGroup="contvalidate"></asp:RequiredFieldValidator>
                </td>
                <td>
                    <asp:Label ID="lblExtraField6" runat="server"
                        Text="ExtraField6" Visible="False"></asp:Label>
                    <asp:Label ID="lblReq6" CssClass="fontred" runat="server" Text="*"
                        Visible="False"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ExtraField6" runat="server" Font-Size="8pt" Visible="false"
                        Width="200px">
                        <asp:ListItem Value=" "> </asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="validEF6" runat="server"
                        ControlToValidate="ExtraField6" Enabled="false" ForeColor="Red" Display="Dynamic" InitialValue="0"
                        ErrorMessage="Field can not left blank !" ValidationGroup="contvalidate"></asp:RequiredFieldValidator>

                </td>
            </tr>
            <tr>
                <td style="width: 200px">
                    <asp:Label ID="lblTransactionDetail" runat="server"
                        Text="Detail:"></asp:Label>
                    <asp:Label ID="lblTransactionDetailMandatory" CssClass="fontred" runat="server"
                        Text="*"></asp:Label>
                </td>
                <td style="margin-left: 40px; width: 586px;">
                    <asp:TextBox ID="TransactionDetail" runat="server"
                        Font-Size="8pt" Height="25px" MaxLength="100"
                        Width="223px"
                        TextMode="MultiLine"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="validDetail" runat="server"
                        ControlToValidate="TransactionDetail" Enabled="false" ForeColor="Red" Display="Dynamic"
                        ErrorMessage="Field can not left blank !" ValidationGroup="contvalidate"></asp:RequiredFieldValidator>
                    <cc1:FilteredTextBoxExtender ID="NumbervalidEF" runat="server" FilterType="Numbers" TargetControlID="TransactionDetail"
                        ValidChars="0123456789" Enabled="false">
                    </cc1:FilteredTextBoxExtender>
                    <cc1:FilteredTextBoxExtender ID="TextvalidEF" runat="server" FilterType="UppercaseLetters, LowercaseLetters,custom"
                        ValidChars=" "
                        Enabled="false"
                        TargetControlID="TransactionDetail" />
                </td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="panelTotalBill" Visible="False" runat="server">
        <table>
            <tr>
                <td style="width: 200px">
                    <asp:Label ID="lblTotalBillField" runat="server">Total Bill Field</asp:Label>
                </td>
                <td colspan="2">
                    <asp:TextBox ID="txtTotalBillField" runat="server" Width="300px" AutoComplete="Off"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="validBillAmount0" runat="server" ControlToValidate="txtTotalBillField" Enabled="false" CssClass="fontred" Display="Dynamic" ErrorMessage="Field can not left blank !" ForeColor="Red" ValidationGroup="contvalidate"></asp:RequiredFieldValidator>
                    &nbsp;&nbsp;
                    <cc1:FilteredTextBoxExtender ID="NumbervalidBillAoumnt0" runat="server" FilterType="Numbers" TargetControlID="txtTotalBillField"
                        ValidChars="0123456789" Enabled="false">
                    </cc1:FilteredTextBoxExtender>
                </td>
            </tr>
            <tr>
                <td style="width: 204px">
                    <asp:Label ID="lblAmount" runat="server">Bill Amount</asp:Label>
                </td>
                <td colspan="2">
                    <asp:TextBox ID="Amount" runat="server" Width="300px" AutoComplete="Off"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="validBillAmount" runat="server"
                        ControlToValidate="Amount" Enabled="false" CssClass="fontred" Display="Dynamic" ForeColor="Red"
                        ErrorMessage="Field can not left blank !" ValidationGroup="contvalidate"></asp:RequiredFieldValidator>
                    &nbsp;&nbsp;
                        <cc1:FilteredTextBoxExtender ID="NumbervalidBillAoumnt" runat="server" FilterType="Numbers" TargetControlID="Amount"
                            ValidChars="0123456789" Enabled="false">
                        </cc1:FilteredTextBoxExtender>
                </td>
            </tr>
        </table>
    </asp:Panel>

    <hr />
    <table width="100%">
        <tr>
            <td colspan="3">
                <div>
                    <asp:LinkButton runat="server" ID="lnkTravel" ClientIDMode="AutoID" Style="color: #2D4A98 !important" Text="Click To Add Reimbursement Details Manually" Visible="false" ValidationGroup="contvalidate"></asp:LinkButton>
                </div>

                <div style="padding-top: 20px;">
                    <telerik:RadGrid runat="server" ID="radGridTravel" Skin="Metro" AllowAutomaticUpdates="true" AllowAutomaticDeletes="true" AllowAutomaticInserts="true"
                        Style="margin-top: 0px">

                        <MasterTableView TableLayout="Auto" AutoGenerateColumns="False" DataKeyNames="Sl.No">
                            <Columns>
                                <telerik:GridBoundColumn UniqueName="Sl.No" DataField="Sl.No" HeaderText="Sl.No"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn UniqueName="PersonsName" HeaderText="Name of the Member Travelled" DataField="PersonsName"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn UniqueName="Dependence" HeaderText="Dependent (Y/N)" DataField="Dependence"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn UniqueName="RelationshipwithEmployee" HeaderText="Relationship with employee" DataField="RelationshipwithEmployee"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn UniqueName="From" HeaderText="Travel from" DataField="From"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn UniqueName="To" HeaderText="Travel To" DataField="To"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn UniqueName="JourneyDate" HeaderText="Travel on" DataField="JourneyDate"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn UniqueName="ModeOfJourney" HeaderText="Travel mode" DataField="ModeOfJourney"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn UniqueName="TypeOfJourney" HeaderText="Travel type" DataField="TypeOfJourney"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn UniqueName="ClaimAmount" HeaderText="Travel fare in Rs" DataField="ClaimAmount"></telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn HeaderText="Edit" UniqueName="Edit" HeaderButtonType="LinkButton">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEditTravel" runat="server" Text="Edit" OnClick="btnEditTravel_Click">
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridButtonColumn UniqueName="Delete" CommandName="Delete" ConfirmText="Are you sure you want to delete this Detail ?" HeaderText="Delete" Text="Delete" ButtonType="LinkButton"></telerik:GridButtonColumn>
                                <telerik:GridBoundColumn DataField="ReimbursementPersonsDetailID" UniqueName="ReimbursementPersonsDetailID" HeaderText="Sub Claim ID"></telerik:GridBoundColumn>
                            </Columns>
                        </MasterTableView>
                        <ClientSettings AllowKeyboardNavigation="true">
                        </ClientSettings>
                        <GroupingSettings CaseSensitive="false" />

                        <HeaderStyle Font-Bold="true" CssClass="left" BackColor="#ff6b1c" Font-Size="13px" ForeColor="White"></HeaderStyle>
                        <ItemStyle ForeColor="Black" CssClass="left"></ItemStyle>
                        <AlternatingItemStyle ForeColor="Black" />
                        <PagerStyle Mode="NextPrevNumericAndAdvanced" Position="Bottom"></PagerStyle>
                    </telerik:RadGrid>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="3"></td>
        </tr>
    </table>
    <hr />
    <asp:Panel ID="panelCSVUpload" runat="server" Visible="true" Style="float: left; width: 55%;">
        <table width="100%">
            <tr>
                <td colspan="4" style="font-size: 14px;">
                    <div style="text-align: center; border: 1px solid #ddd; padding: 5px; border-bottom: none; margin-bottom: -8px; background: #ff6b1c; color: white">
                        <asp:Label ID="lbl" runat="server">Important Note : You can upload bill details through CSV file. !</asp:Label>
                    </div>
                </td>
            </tr>
            <tr>

                <td class="auto-style4" colspan="4">
                    <asp:LinkButton runat="server" ID="linkCSVTemplate" CausesValidation="false" Text="Click to download Template to Add Reimbursement details by CSV." Style="color: #2D4A98 !important"></asp:LinkButton>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblFileUpload" Width="250px" runat="server" Visible="True">Upload Bill and other documents as per the policy (CSV File)</asp:Label>
                </td>
                <td colspan="3">
                    <telerik:RadAsyncUpload runat="server" ID="radAsynUpload" Width="216px" AllowedFileExtensions=".csv" MultipleFileSelection="Disabled" MaxFileInputsCount="1"></telerik:RadAsyncUpload>
                </td>
            </tr>
            <tr>
                <td align="right" colspan="4">
                    <asp:Button runat="server" ID="btnUpload" Text="Upload File" CssClass="mybutton" ToolTip="Click to upload" ValidationGroup="contvalidate" /></td>
            </tr>
            <tr runat="server" id="trguidelines">
                <td style="font-size: 14px;" colspan="4">
                    <b><u>General Guidelines for csv template</u></b>
                    <br />
                    1. Column "Dependent" should include "Y" (for Yes) or "N" (for No) only
                    <br />
                    2. Dates should be in DD/MM/YYYY format only
                    <br />
                    3. Column "ModeofJourney" should include "Air", "Train","Taxi" and "Other" only
                    <br />
                    4. Column "TypeofJourney" should include "Inward", "Outward" and "Inward & Outward" only
                    <br />
                    5. Column "ClaimAmount" should include numeric values only
                    <br />
                    6. Column "RelationshipWithEmployee" should include "Self","Spouse","Father","Mother","Brother","Sister","Son","Daughter" only
                    <br />
                    7. Amount can be in numeric value only
                    <br />
                    8.PersonsName/Name of the Member Travelled, Travel from and Travel To can be in alphabetic value only
                    <br />
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" style="float: left; width: 25%;">
        <ContentTemplate>
            <asp:Panel ID="panelBillupload" runat="server" Visible="false">
                <table width="100%">
                    <tr>
                        <td colspan="2">
                            <div style="text-align: center; border: 1px solid #ddd; padding: 5px; border-bottom: none; margin-bottom: -8px; background: #ff6b1c; color: white">
                                <asp:Label ID="lbluploadheader" runat="server"></asp:Label>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2"></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" Width="280px" runat="server" Visible="True">Upload Bill details (jpg/pdf File)</asp:Label>
                        </td>
                        <td>
                            <telerik:RadAsyncUpload runat="server" ID="RadAsyncUpload1"
                                MultipleFileSelection="Automatic" Width="216px" AllowedFileExtensions=".jpeg,.jpg,.png,.pdf"
                                PersistConfiguration="true" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right" colspan="2">
                            <asp:Button runat="server" ID="btnUploadBill" Text="Upload Bill" CssClass="mybutton" ToolTip="Click to upload" OnClick="btnUploadBill_Click" ValidationGroup="contvalidate" /></td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Literal ID="lit1" runat="server"></asp:Literal>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSave" />
        </Triggers>
    </asp:UpdatePanel>

    <br />
    <br />
    <br />
    <div style="width: 100%">
        <table style="width: 100%">

            <tr>
                <td colspan="3">
                    <asp:CheckBox ID="chkDisclaimer" Style="font-size: 12px;" runat="server"
                        Visible="False" CssClass="radiolistcss" />
                </td>
            </tr>
            <tr>

                <td colspan="3" align="Center">
                    <asp:Button ID="btnSave" runat="server" CssClass="mybutton2" Text="Submit claim" ToolTip="Click to submit claim" ValidationGroup="contvalidate" />
                </td>
            </tr>
            <tr>
                <td style="text-align: center;" colspan="3">
                    <asp:Label Font-Bold="true" Font-Size="Large" runat="server" ID="lblsave" ForeColor="#ff6b1c"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="text-align: center;">
                    <h4>
                        <asp:Label ID="lblError" runat="server" Font-Size="Large" Font-Bold="true" ForeColor="#ff6b1c"></asp:Label></h4>
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField ID="hidmaxlen" runat="server" />
    <asp:HiddenField ID="hidwebtableid" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
