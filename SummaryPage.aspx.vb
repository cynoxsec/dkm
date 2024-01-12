Imports DkmOnline.Lib
Imports DkmOnline.Common
Imports System.IO
Imports iTextSharp.text.pdf
Imports iTextSharp.text
Imports System.Data.SqlClient


Public Class SummaryPage
    Inherits System.Web.UI.Page

    Dim Year As Int32

    Dim OldNewConn As OldNewConnection = New OldNewConnection()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If IsNothing(Session("Emp_Code")) Then
                Response.Redirect("../Logout.aspx")
            End If
            DatabaseFacade.Instance.Enter_SiteLog(Session("Emp_Code"), Session("LoginType"), Session("LoginAccessType"), Session("Comp_Code"), 0, Request.Url.AbsolutePath, Request.Url.AbsolutePath, Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type, Session("WebTableID"), Session.SessionID, Session("MenuID"))

            OldNewConn.CheckDatabaseOldNew(Session("WebTableID"))

            Dim dtYear As DataTable = OldNewConn.GetDataTable2("select * from dbo.ReimYearMaster" & Session("WebTableID") & "")

            If (dtYear.Rows.Count > 0) Then
                Year = dtYear.Rows(0)("Year")
            End If

            If Not (IsPostBack) Then
                Title = "Reimbursement Details - " & StrConv(Request.QueryString("FieldName"), VbStrConv.ProperCase)
            End If

        Catch ex As Exception
            Response.Write("<script>alert('" & Server.HtmlEncode(ex.Message) & "')</script>")
        End Try
    End Sub

    Protected Sub radGridSummary_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles radGridSummary.NeedDataSource
        Dim str As String

        Dim dt1 As New DataTable

        str = "Select ISNULL(PSS.Print_Name,RM.Field_Name) as 'Field_Name',convert(nvarchar,PayDate,106) as 'Month',Convert(float,Claimed) as 'Claimed_Amount',Convert(float,Reimbursed) as 'Reimbursed_Amount',Convert(float,Taxable) as 'Taxable',Convert(float,BillPassed) as 'Disallowed',TransactionRemarks as 'Remark' from ReimburseTransaction" & Session("WebTableID") & " RT inner join reimbursemaster" & Session("WebTableID") & " RM on RT.ReimID=RM.ReimID Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RM.Field_Name where RM.Emp_Code='" & Request.QueryString("Emp_Code") & "' and  Budget_Wet='03/31/" & Year + 1 & "' and (PSS.Print_Name='" & Request.QueryString("FieldName") & "' Or RM.Field_Name='" & Request.QueryString("FieldName") & "')"

        dt1 = OldNewConn.GetDataTable2(str)

        If (dt1.Rows.Count = 0) Then
            lbl1.Text = "No Record Found."
        Else
            radGridSummary.DataSource = dt1
        End If
    End Sub
End Class