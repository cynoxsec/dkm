Imports DkmOnline.Lib
Imports System.Data
Imports System.Data.SqlClient
Imports DkmOnline.Common

Public Class ShowPolicy
    Inherits System.Web.UI.Page

    Dim ReimbursementTypeID As Integer

    Dim OldNewConn As OldNewConnection = New OldNewConnection()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If IsNothing(Session("Emp_Code")) Then
            Response.Redirect("../Logout.aspx")
        End If

        DatabaseFacade.Instance.Enter_SiteLog(Session("Emp_Code"), Session("LoginType"), Session("LoginAccessType"), Session("Comp_Code"), 0, Request.Url.AbsolutePath, Request.Url.AbsolutePath, Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type, Session("WebTableID"), Session.SessionID, Session("MenuID"))

        OldNewConn.CheckDatabaseOldNew(Session("WebTableID"))

        ReimbursementTypeID = Request.QueryString("value")

        Dim dtValidDates As DataTable

        dtValidDates = OldNewConn.GetDataTable2("select BillValidStartDate,BillValidEndDate,ReimbursePolicy,IsNull(PSS.Print_Name,RTM.Name) as Name from ReimburseMentTypeMaster" & Session("WebTableID") & " RTM Left join PayslipSetup" & Session("WebTableID") & " PSS on RTM.Name=PSS.Name where ReimburseType_Code='" & ReimbursementTypeID & "'")

        If (dtValidDates.Rows.Count > 0) Then
            If Not (dtValidDates.Rows(0)("ReimbursePolicy") = "") Then
                lblPolicy.Text = "<div style=""display: inline;"">" & dtValidDates.Rows(0)("ReimbursePolicy") & "</div>"
            Else
            End If
            Title = String.Format("{0} Reimbursement Policy", StrConv(dtValidDates.Rows(0)("Name"), VbStrConv.ProperCase))
        Else
            Title = "Reimbursement Policy"
        End If
    End Sub

End Class