<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReimburseTypeDetail.aspx.vb" Inherits="DkmOnlineWeb.ReimburseTypeDetail" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <telerik:RadScriptBlock runat="server" ID="radScriptBlock">
        <script src="../Script/Jquery1.8min.js" type="text/javascript"></script>
        <link href="ReimburesmentStyleSheet.css" rel="stylesheet" />
        <script type="text/javascript">
            function UpdateClose() {
                alert('Reimbursement type updated successfully.');
                GetRadWindow().BrowserWindow.location.href = 'ReimburseTypeAdmin.aspx';
                GetRadWindow().close();
            }
            function InsertClose() {
                alert('Reimbursement type Add successfully.');
                GetRadWindow().BrowserWindow.location.href = 'ReimburseTypeAdmin.aspx';
                GetRadWindow().close();
            }
            function GetRadWindow() {
                var oWindow = null; if (window.radWindow)
                    oWindow = window.radWindow; else if (window.frameElement.radWindow)
                        oWindow = window.frameElement.radWindow; return oWindow;
            }
        </script>

    </telerik:RadScriptBlock>


</head>
<body>
    <form id="form1" runat="server">
        <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
        <div>
            <table id="Table2" class="mytablecss">
                <tr>
                    <td colspan="2">
                        <b>Reimbursement Master</b>
                    </td>
                </tr>

                <tr>
                    <td>
                        <table id="Table3" width="450px">
                            <tr>
                                <td>Name :
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlReimType" runat="server" TabIndex="1" AppendDataBoundItems="True">
                                        <asp:ListItem Selected="True" Text="Select" Value="0">
                                        </asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>Transaction Date Title :
                                </td>
                                <td>
                                    <asp:TextBox ID="txttranDatetitle" runat="server" Text='<%# Bind("TransactionDateTitle")%>' TabIndex="2">
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>Transaction Title :
                                </td>
                                <td>
                                    <asp:TextBox ID="txttransTitle" runat="server" Text='<%# Bind("TransactionTitle") %>' TabIndex="3">
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>Need Extra Date Field :
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkneedextdateField" runat="server" Checked='<%# Bind("NeedExtraDateField")%>' TabIndex="4" />
                                </td>
                            </tr>
                            <tr>
                                <td>Extra Date Field Description :
                                </td>
                                <td>
                                    <asp:TextBox ID="txtExtdateFieldDesc" runat="server" Text='<%# Bind("ExtraDateFieldDescription")%>' TabIndex="5">
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>ExtraField 1 :
                                </td>
                                <td>
                                    <asp:TextBox ID="txtExtrafield1" Text='<%# Bind("ExtraField1")%>' runat="server" TabIndex="6">
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>ExtraField 2 :
                                </td>
                                <td>
                                    <asp:TextBox ID="txtExtrafield2" Text='<%# Bind("ExtraField2")%>' runat="server" TabIndex="7">
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>ExtraField 3 :
                                </td>
                                <td>
                                    <asp:TextBox ID="txtExtrafield3" Text='<%# Bind("ExtraField3")%>' runat="server" TabIndex="8">
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>ExtraField 5 :
                                </td>
                                <td>
                                    <asp:TextBox ID="txtExtrafield5" Text='<%# Bind("ExtraField5")%>' runat="server" TabIndex="9">
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>ExtraField 4 (for Drop Down) :
                                </td>
                                <td>
                                    <asp:TextBox ID="txtExtrafield4" Text='<%# Bind("ExtraField4")%>' runat="server" TabIndex="10">
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>ExtraField4Description:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtExtrafield4desc" Text='<%# Bind("ExtraField4Description")%>' runat="server" TabIndex="11">
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>ExtraField 6 (for Drop Down) :
                                </td>
                                <td>
                                    <asp:TextBox ID="txtExtrafield6" Text='<%# Bind("ExtraField6")%>' runat="server" TabIndex="12">
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>ExtraField6Description :
                                </td>
                                <td>
                                    <asp:TextBox ID="txtExtrafield6desc" Text='<%# Bind("ExtraField6Description")%>' runat="server" TabIndex="13">
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>ExtraField 7 :
                                </td>
                                <td>
                                    <asp:TextBox ID="txtExtrafield7" Text='<%# Bind("ExtraField7")%>' runat="server" TabIndex="14">
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>ExtraField 8 :
                                </td>
                                <td>
                                    <asp:TextBox ID="txtExtrafield8" Text='<%# Bind("ExtraField8")%>' runat="server" TabIndex="15">
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>Bill Month :
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkbillMonth" runat="server" Checked='<%# Bind("BillMonth")%>' TabIndex="16" />
                                </td>
                            </tr>
                            <tr>
                                <td>Bill Valid Start Date :
                                </td>
                                <td>
                                    <telerik:RadDatePicker RenderMode="Lightweight" ID="radDatebillvalidstartDate" runat="server" MinDate="1/1/1900" DbSelectedDate='<%# Bind("BillValidStartDate")%>'
                                        TabIndex="17">
                                        <Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" RenderMode="Lightweight"></Calendar>

                                        <DateInput TabIndex="17" DisplayDateFormat="dd/MM/yyyy" DateFormat="dd/MM/yyyy" LabelWidth="40%" RenderMode="Lightweight">
                                            <EmptyMessageStyle Resize="None"></EmptyMessageStyle>

                                            <ReadOnlyStyle Resize="None"></ReadOnlyStyle>

                                            <FocusedStyle Resize="None"></FocusedStyle>

                                            <DisabledStyle Resize="None"></DisabledStyle>

                                            <InvalidStyle Resize="None"></InvalidStyle>

                                            <HoveredStyle Resize="None"></HoveredStyle>

                                            <EnabledStyle Resize="None"></EnabledStyle>
                                        </DateInput>

                                        <DatePopupButton ImageUrl="" HoverImageUrl="" TabIndex="17"></DatePopupButton>
                                    </telerik:RadDatePicker>
                                </td>
                            </tr>
                            <tr>
                                <td>Bill Valid End Date :
                                </td>
                                <td>
                                    <telerik:RadDatePicker RenderMode="Lightweight" ID="radDatebillvalidendDate" runat="server" MinDate="1/1/1900" DbSelectedDate='<%# Bind("BillValidEndDate")%>'
                                        TabIndex="18">
                                        <Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" RenderMode="Lightweight"></Calendar>

                                        <DateInput TabIndex="18" DisplayDateFormat="dd/MM/yyyy" DateFormat="dd/MM/yyyy" LabelWidth="40%" RenderMode="Lightweight">
                                            <EmptyMessageStyle Resize="None"></EmptyMessageStyle>

                                            <ReadOnlyStyle Resize="None"></ReadOnlyStyle>

                                            <FocusedStyle Resize="None"></FocusedStyle>

                                            <DisabledStyle Resize="None"></DisabledStyle>

                                            <InvalidStyle Resize="None"></InvalidStyle>

                                            <HoveredStyle Resize="None"></HoveredStyle>

                                            <EnabledStyle Resize="None"></EnabledStyle>
                                        </DateInput>

                                        <DatePopupButton ImageUrl="" HoverImageUrl="" TabIndex="18"></DatePopupButton>
                                    </telerik:RadDatePicker>
                                </td>
                            </tr>
                            <tr>
                                <td>Multiple Claim Required :
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkmulticlaimreq" runat="server" Checked='<%# Bind("isMultipleClaimDetails")%>' TabIndex="19" />
                                </td>
                            </tr>
                            <tr>
                                <td>Persons Detail Required :
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkpersondetailreq" runat="server" Checked='<%# Bind("isPersonsDetailRequired")%>' TabIndex="20" />
                                </td>
                            </tr>
                            <tr>
                                <td>Claim Amount Field :
                                </td>
                                <td>
                                    <asp:TextBox ID="txtClaimField" Text='<%# Bind("ClaimAmountField")%>' runat="server" TabIndex="21">
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>Total Bill Field :
                                </td>
                                <td>
                                    <asp:TextBox ID="txtTotalBillFiled" Text='<%# Bind("TotalBillField")%>' runat="server" TabIndex="22">
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>Disclaimer :
                                </td>
                                <td>
                                    <asp:TextBox ID="txtDisclaimer" Text='<%# Bind("Disclaimer")%>' runat="server" TabIndex="23">
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>Reimburse Apply Note :
                                </td>
                                <td>
                                    <asp:TextBox ID="txtReimburseApplyNote" Text='<%# Bind("ReimburseApplyNote")%>' runat="server" TabIndex="24">
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="auto-style1">Is Upload :
                                </td>
                                <td class="auto-style1">
                                    <asp:CheckBox ID="chkUpload" runat="server" Checked='<%# Bind("IsUpload")%>' TabIndex="25" />
                                </td>
                            </tr>
                            <tr>
                                <td>Is Inactive :
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkActive" runat="server" Checked='<%# Bind("IsInactive")%>' TabIndex="26" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Button ID="btnInsert" Text="Submit" runat="server" OnClick="btnInsert_Click" CssClass="mybutton" Visible="false"></asp:Button>&nbsp;
                        <asp:Button ID="btnUpdate" Text="Update" runat="server" OnClick="btnUpdate_Click" CssClass="mybutton" Visible="false"></asp:Button>&nbsp;
                        <asp:Button ID="btnCancel" Text="Cancel" runat="server" CausesValidation="False" CssClass="mybutton" OnClick="btnCancel_Click"></asp:Button>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
