<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/reimburseMasterPage.Master" CodeBehind="ReimbursementPrintDetails.aspx.vb" Inherits="DkmOnlineWeb.ReimbursementPrintDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .PrintAllReimbursementClaimForms {
            margin-top: -1px !important;
            padding-top: 0px !important;
            border-top: 4px solid #2d4a98 !important;
            background-color: #fff;
            background-image: linear-gradient(#f0f0f0,#fff);
            height: 35px;
        }

            .PrintAllReimbursementClaimForms span {
                padding: 1px 15px !important;
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">


    <div style="text-align: center; border: 1px solid #ddd; padding: 5px; border-bottom: none; background: #ff6b1c; color: white">
        Print Reimbursement Details
    </div>
        
    <br />
    <b>
        <asp:Label ID="lbl" runat="server">You will get Claim Form for all the Reimbursements of a particular month in a single file.</asp:Label></b>
    <br />
    <br />
    <br />
        
    <asp:Panel ID="Panel1" runat="server" DefaultButton="downloadLinkButton1">
        <div style="font-weight: bold; font-size: large;">
            <table>
                <tr>
                    <td>Select Month and Year</td>
                    <td>
                        <telerik:RadMonthYearPicker runat="server" ID="radMonthYearPicker">
                            <DateInput DisplayDateFormat="MMMM, yyyy" DateFormat="MMMM, yyyy" LabelWidth=""></DateInput>
                            <DatePopupButton ImageUrl="" HoverImageUrl=""></DatePopupButton>
                        </telerik:RadMonthYearPicker>

                        <asp:RequiredFieldValidator runat="server" ID="Req" ErrorMessage="*" ForeColor="Red" ControlToValidate="radMonthYearPicker" ValidationGroup="downvalidation"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:LinkButton ID="downloadLinkButton1" runat="server" Text="Generate Forms" OnClick="downloadLinkButton1_Click" ValidationGroup="downvalidation" CssClass="mybutton" ToolTip="Click to Download"></asp:LinkButton></td>
                </tr>

            </table>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
