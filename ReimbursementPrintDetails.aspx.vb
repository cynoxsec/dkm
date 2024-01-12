Imports DkmOnline.Lib
Imports System.Data
Imports System.Data.SqlClient
Imports DkmOnline.Common

Public Class ReimbursementPrintDetails
    Inherits System.Web.UI.Page

    Dim OldNewConn As OldNewConnection = New OldNewConnection()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsNothing(Session("Emp_Code")) Then
            Response.Redirect("../Logout.aspx")
        End If
        DatabaseFacade.Instance.Enter_SiteLog(Session("Emp_Code"), Session("LoginType"), Session("LoginAccessType"), Session("Comp_Code"), 0, Request.Url.AbsolutePath, Request.Url.AbsolutePath, Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type, Session("WebTableID"), Session.SessionID, Session("MenuID"))
        OldNewConn.CheckDatabaseOldNew(Session("WebTableID"))
    End Sub

    Protected Sub downloadLinkButton1_Click(sender As Object, e As EventArgs)
        If (radMonthYearPicker.SelectedDate.Value.Month <> Nothing) Then
            Response.Redirect("~/ReimbursementManagement/DownloadReimburseDetails.aspx?Month=" & radMonthYearPicker.SelectedDate.Value.Month & "&Year=" & radMonthYearPicker.SelectedDate.Value.Year & "")
        Else
            Response.Redirect("~/ReimbursementManagement/DownloadReimburseDetails.aspx")
        End If
    End Sub
End Class