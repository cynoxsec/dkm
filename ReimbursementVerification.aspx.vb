Imports DkmOnline.Lib
Imports Telerik.Web.UI
Imports DkmOnline.Common
Imports Ionic.Zip
Imports System.Threading
Imports System.Data.SqlClient
Imports System.IO

Public Class ReimbursementVerification
    Inherits System.Web.UI.Page

    Dim OldNewConn As OldNewConnection = New OldNewConnection()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If IsNothing(Session("Emp_Code")) Then
                Response.Redirect("../Logout.aspx")
            End If
            DatabaseFacade.Instance.Enter_SiteLog(Session("Emp_Code"), Session("LoginType"), Session("LoginAccessType"), Session("Comp_Code"), 0, Request.Url.AbsolutePath, Request.Url.AbsolutePath, Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type, Session("WebTableID"), Session.SessionID, Session("MenuID"))
        Catch ex As Exception
            Response.Write("<script>alert('" & Server.HtmlEncode(ex.Message) & "')</script>")
        End Try

    End Sub

    Protected Sub ddlViewStatus_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlViewStatus.SelectedIndexChanged
        radGrid2.Rebind()
    End Sub

    Protected Sub radGrid2_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles radGrid2.NeedDataSource
        Try

            Dim commandtext As String

            If (ddlViewStatus.SelectedItem.Value = 1) Then
                commandtext = "select ReimburseDetailsID,Emp_code,IsNull(PSS.Print_Name,RTM.Name) as 'Field_Name',convert(nvarchar,EntryDate,106) as 'EntryDate',case when ClaimAmount=0 And RD.ExtraField3 = 0 then RD.ExtraField5 when ClaimAmount=0 then RD.ExtraField2 else ClaimAmount End as 'BillAmount' from reimbursementdetails" & Session("WebTableID") & " RD inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RTM.ReimburseType_Code=RD.ReimburseType_Code Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RTM.Name where Rejected=3 order by ReimburseDetailsID desc"
            ElseIf (ddlViewStatus.SelectedItem.Value = 2) Then
                commandtext = "select ReimburseDetailsID,Emp_code,IsNull(PSS.Print_Name,RTM.Name) as 'Field_Name',convert(nvarchar,EntryDate,106) as 'EntryDate',case when ClaimAmount=0 And RD.ExtraField3 = 0 then RD.ExtraField5 when ClaimAmount=0 then RD.ExtraField2 else ClaimAmount End as 'BillAmount' from reimbursementdetails" & Session("WebTableID") & " RD inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RTM.ReimburseType_Code=RD.ReimburseType_Code Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RTM.Name where Rejected=1 order by ReimburseDetailsID desc"
            ElseIf (ddlViewStatus.SelectedItem.Value = 3) Then
                commandtext = "select ReimburseDetailsID,Emp_code,IsNull(PSS.Print_Name,RTM.Name) as 'Field_Name',convert(nvarchar,EntryDate,106) as 'EntryDate',case when ClaimAmount=0 And RD.ExtraField3 = 0 then RD.ExtraField5 when ClaimAmount=0 then RD.ExtraField2 else ClaimAmount End as 'BillAmount' from reimbursementdetails" & Session("WebTableID") & " RD inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RTM.ReimburseType_Code=RD.ReimburseType_Code Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RTM.Name where Rejected=2 order by ReimburseDetailsID desc"
            End If

            Dim dt As DataTable = OldNewConn.GetDataTable2(commandtext)
            radGrid2.DataSource = dt
        Catch ex As Exception
            Response.Write("<script>alert('" & Server.HtmlEncode(ex.Message) & "')</script>")
        End Try

    End Sub

    Protected Sub radGrid2_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles radGrid2.ItemDataBound
        Dim griditem As GridDataItem = TryCast(e.Item, GridDataItem)

        If Not IsNothing(griditem) Then

            Dim lnkForViewReport2 As LinkButton = DirectCast(griditem("ViewReport2").FindControl("lnkForViewReport2"), LinkButton)

            If Not IsNothing(lnkForViewReport2) Then

                Dim ReimbursementID As String = griditem.GetDataKeyValue("ReimburseDetailsID")

                Dim Str As String = "Select count(emp_code) as TotalBill from ReimbursementProofUpload" & Session("WebtableID") & " where reimbursedetailsid=" & ReimbursementID & ""

                Dim dt122 As New DataTable

                dt122 = OldNewConn.GetDataTable2(Str)

                If (dt122.Rows.Count > 0) Then
                    If (CInt(dt122.Rows(0)("TotalBill")) > 0) Then
                        lnkForViewReport2.Visible = True
                    Else
                        lnkForViewReport2.Visible = False
                    End If
                Else
                    lnkForViewReport2.Visible = False
                End If
            End If
        End If
    End Sub

    Protected Sub radGrid2_DeleteCommand(sender As Object, e As Telerik.Web.UI.GridCommandEventArgs) Handles radGrid2.DeleteCommand

    End Sub

    Protected Sub radGrid2_ItemCommand(sender As Object, e As GridCommandEventArgs)
        Dim str As String = ""

        If e.CommandName = "Accept" Then
            Dim ExpensesDetailsId As Object = e.CommandArgument

            Dim eitem As GridDataItem = TryCast(e.Item, GridDataItem)
            Try
                Dim txtReasonAccept As TextBox = DirectCast(eitem.FindControl("txtReasonAccept"), TextBox)
                Dim txtbillpassed As TextBox = DirectCast(eitem.FindControl("txtbillpassed"), TextBox)
                Dim emp_code As String = eitem("Emp_Code").Text
                Dim ExpensestypeName As String = eitem("Name").Text
                Dim ClaimAmount As Decimal = CDec(eitem("ClaimAmount").Text)
                Dim res As Integer = 0

                If (txtbillpassed.Text = "") Then
                    ScriptManager.RegisterStartupScript(Me, [GetType](), "alert", "billpassed();", True)
                    Exit Sub
                End If

                If (CDec(txtbillpassed.Text) > ClaimAmount) Then
                    ScriptManager.RegisterStartupScript(Me, [GetType](), "alert", "ClaimAmount();", True)
                    Exit Sub
                End If

                If (CDec(txtbillpassed.Text) <> ClaimAmount) Then
                    ScriptManager.RegisterStartupScript(Me, [GetType](), "alert", "ClaimNotEqualAmount();", True)
                    Exit Sub
                End If

                Dim dtFinanceDetails As DataTable = OldNewConn.GetDataTable2("Select E.Emp_Code as 'EmpCode',E.FirstName as 'EmpName',E2.Emp_code as 'FinanceCode',E2.FirstName as 'Financename',E2.Email as 'FinanceEmail' from employeesmaster" & Session("WebTableID") & " E Left join BUConfiguration" & Session("WebTableID") & " E1 on E.emp_code=E1.emp_code Left join Employeesmaster" & Session("WebTableID") & " E2 on E2.emp_code=E1.Finance_Code where E.emp_code='" & emp_code & "'")

                Dim dtFinanceApprovalMail As DataTable = OldNewConn.GetDataTable2("Select * from MailSetupfromat" & Session("WebtableID") & " where MailSubject='Expense Claim - Pending For Finance Approval'")

                Dim mailbody As String = ""
                Dim mailSubject As String = ""
                Dim sb As System.Text.StringBuilder = New StringBuilder()

                If (dtFinanceApprovalMail.Rows.Count > 0) Then
                    mailbody = dtFinanceApprovalMail.Rows(0)("MailBody")
                    mailSubject = dtFinanceApprovalMail.Rows(0)("MailSubject")
                End If

                If (dtFinanceDetails.Rows.Count > 0) Then
                    mailbody = mailbody.Replace("#Financename#", dtFinanceDetails.Rows(0)("Financename"))

                    Dim sql_Query As String = "select ED.ExpensesDetailsId,ED.Emp_Code,ED.Field2,ETM.Name,ED.ExpensesDate,ED.FieldtotalClaim from ExpensesDetails" & Session("WebtableID") & " ED inner join ExpenseTypeMaster" & Session("WebtableID") & " ETM on ETM.ExpenseTypeMasterID=ED.ExpenseTypeMasterID where ExpensesDetailsId=" & ExpensesDetailsId & " and Emp_Code='" & emp_code & "'  order by ExpensesDetailsId desc"
                    Dim dsExpenseDetails As DataTable = OldNewConn.GetDataTable2(sql_Query)

                    sb.Append("<TABLE border=1 cellPadding=0 width=100% cellSpacing=0 style=" & Chr(34) & "font-family: Segoe UI,Verdana,Arial,Georgia;font-size:10pt" & Chr(34) & " width=60%>")
                    sb.Append("<tr><th>Claim ID</th><th>Employee Code</th><th>Employee Name</th><th>Expense Type</th><th>Claim date</th><th>Claim Amount</th></tr>")

                    For j As Integer = 0 To dsExpenseDetails.Rows.Count - 1
                        sb.Append("<tr>")
                        sb.Append("<td>" & dsExpenseDetails.Rows(j)("ExpensesDetailsId").ToString & "</td>")
                        sb.Append("<td>" & dsExpenseDetails.Rows(j)("Emp_Code").ToString & "</td>")
                        sb.Append("<td>" & dsExpenseDetails.Rows(j)("Field2").ToString & "</td>")
                        sb.Append("<td>" & dsExpenseDetails.Rows(j)("Name").ToString & "</td>")
                        sb.Append("<td>" & CDate(dsExpenseDetails.Rows(j)("ExpensesDate")).Date.ToString("dd/MM/yyyy") & "</td>")
                        sb.Append("<td>" & CDec(dsExpenseDetails.Rows(j)("FieldtotalClaim")).ToString("####.00") & "</td>")
                        sb.Append("</tr>")
                    Next
                    sb.Append("</TABLE><br/>")

                    mailbody = Replace(mailbody, "#Expensedetails#", sb.ToString)

                    mailbody = MailDetails(dsExpenseDetails, ExpensesDetailsId, mailbody)

                    If Not IsDBNull(dtFinanceDetails.Rows(0)("FinanceEmail")) Then
                        If (dtFinanceDetails.Rows(0)("FinanceEmail") <> "") Then
                            Dim flag As Boolean = ConstantValues.SendEmail(dtFinanceDetails.Rows(0)("FinanceEmail"), mailSubject, mailbody, "", "", "from5", "subject5", "password5")

                            If flag = True Then
                                str = "update ExpensesDetails" & Session("WebTableID") & " set IsAdminApproved=1,AdminApprovalDate=CONVERT(date, getdate()),AdminRemarks='" & txtReasonAccept.Text & "',BillPassed='" & txtbillpassed.Text & "' where ExpensesDetailsID=" & ExpensesDetailsId & " and Emp_Code='" & emp_code & "'"
                                OldNewConn.ExecuteNonQuery(CommandType.Text, str, Nothing)

                                str = "update ExpensesDetails" & Session("WebTableID") & " set FinanceCode='" & dtFinanceDetails.Rows(0)("FinanceCode") & "',FinanceApprovalDate='" & Format(DateTime.Now, "MM/dd/yyyy") & "',IsFinanceApproved=3 where ExpensesdetailsId= " & ExpensesDetailsId & " and Emp_Code='" & emp_code & "'"
                                OldNewConn.ExecuteNonQuery(CommandType.Text, str, Nothing)
                            End If
                            Enter_EmailLog(Session("Emp_Code"), Session("WebTableID"), 1, "DKMWebApplication", mailSubject, "To : " & dtFinanceDetails.Rows(0)("FinanceEmail"), mailbody, "", Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type)
                            ScriptManager.RegisterStartupScript(Me, [GetType](), "alert", "approved();", True)
                        End If
                    End If
                End If
                radGrid2.Rebind()
            Catch ex As Exception
                ClientScript.RegisterStartupScript(Me.[GetType](), "alert", "HideLabel();", True)
            End Try
        ElseIf e.CommandName = "Reject" Then
            Dim ExpensesDetailsId As Object = e.CommandArgument
            Dim eitem As GridDataItem = TryCast(e.Item, GridDataItem)

            Try
                Dim txtReasonAccept As TextBox = DirectCast(eitem.FindControl("txtReasonAccept"), TextBox)
                Dim txtbillpassed As TextBox = DirectCast(eitem.FindControl("txtbillpassed"), TextBox)

                Dim emp_code As String = eitem("Emp_Code").Text

                Dim sql_Query As String = "select ED.ExpensesDetailsId,ED.Emp_Code,ED.Field2,ETM.Name,ED.ExpensesDate,ED.FieldtotalClaim,ED.AdvanceTakenID from ExpensesDetails" & Session("WebtableID") & " ED inner join ExpenseTypeMaster" & Session("WebtableID") & " ETM on ETM.ExpenseTypeMasterID=ED.ExpenseTypeMasterID where ExpensesDetailsId=" & ExpensesDetailsId & " and Emp_Code='" & emp_code & "'  order by ExpensesDetailsId desc"
                Dim dsExpenseDetails As DataTable = OldNewConn.GetDataTable2(sql_Query)

                If (txtbillpassed.Text = "") Then
                    ScriptManager.RegisterStartupScript(Me, [GetType](), "alert", "billpassed();", True)
                    Exit Sub
                End If

                If (txtReasonAccept.Text = "") Then
                    ScriptManager.RegisterStartupScript(Me, [GetType](), "alert", "Reasonneeded();", True)
                    Exit Sub
                End If

                Dim res As Integer = 0

                str = "update ExpensesDetails" & Session("WebTableID") & " set IsAdminApproved=2,AdminApprovalDate=CONVERT(date, getdate()),AdminRemarks='" & txtReasonAccept.Text & "',BillPassed='" & txtbillpassed.Text & "',IsFinanceApproved=2 where ExpensesDetailsID=" & ExpensesDetailsId & " and Emp_Code='" & emp_code & "'"
                res = OldNewConn.ExecuteNonQuery(CommandType.Text, str, Nothing)

                If Not IsDBNull(dsExpenseDetails.Rows(0)("AdvanceTakenID").ToString) Then
                    If (dsExpenseDetails.Rows(0)("AdvanceTakenID").ToString <> "") Then
                        str = "Update AdvanceTaken" & Session("WebTableID") & " set AmountClaimAgainst=AmountClaimAgainst-" & dsExpenseDetails.Rows(0)("FieldtotalClaim").ToString & ",isSettled=0 where AdvanceTakenID=" & dsExpenseDetails.Rows(0)("AdvanceTakenID").ToString & ""
                        OldNewConn.ExecuteNonQuery(CommandType.Text, str, Nothing)
                    End If
                End If

                If res > 0 Then
                    ScriptManager.RegisterStartupScript(Me, [GetType](), "alert", "Rejected();", True)
                End If

                radGrid2.Rebind()

            Catch ex As Exception
                ClientScript.RegisterStartupScript(Me.[GetType](), "alert", "HideLabel();", True)
            End Try

        ElseIf e.CommandName = "resubmission" Then
            Dim ExpensesDetailsId As Object = e.CommandArgument
            Dim eitem As GridDataItem = TryCast(e.Item, GridDataItem)

            Try
                Dim txtReasonAccept As TextBox = DirectCast(eitem.FindControl("txtReasonAccept"), TextBox)
                Dim txtbillpassed As TextBox = DirectCast(eitem.FindControl("txtbillpassed"), TextBox)
                Dim emp_code As String = eitem("Emp_Code").Text
                Dim ClaimAmount As Decimal = CDec(eitem("ClaimAmount").Text)
                Dim res As Integer = 0

                If (txtReasonAccept.Text = "") Then
                    ScriptManager.RegisterStartupScript(Me, [GetType](), "alert", "Reasonneeded();", True)
                    Exit Sub
                End If

                Dim dtEmployeeDetails As DataTable = OldNewConn.GetDataTable2("Select E.Emp_Code as 'EmpCode',E.FirstName as 'EmpName',E.Email as 'EmpEmail' from employeesmaster" & Session("WebTableID") & " E where E.emp_code='" & emp_code & "'")

                Dim dtEmployeeRejectionMail As DataTable = OldNewConn.GetDataTable2("Select * from MailSetupfromat" & Session("WebtableID") & " where MailSubject='Expense Claim – Claim re-submission'")

                Dim mailbody As String = ""
                Dim mailSubject As String = ""
                Dim sb As System.Text.StringBuilder = New StringBuilder()

                If (dtEmployeeRejectionMail.Rows.Count > 0) Then
                    mailbody = dtEmployeeRejectionMail.Rows(0)("MailBody")
                    mailSubject = dtEmployeeRejectionMail.Rows(0)("MailSubject")
                End If

                If (dtEmployeeDetails.Rows.Count > 0) Then
                    mailbody = mailbody.Replace("#EmployeeName#", dtEmployeeDetails.Rows(0)("EmpName"))
                    mailbody = mailbody.Replace("#ReasonOfRejection#", txtReasonAccept.Text)

                    Dim sql_Query As String = "select ED.ExpensesDetailsId,ED.Emp_Code,ED.Field2,ETM.Name,ED.ExpensesDate,ED.FieldtotalClaim,ED.AdvanceTakenID,ED.FieldAdvTaken from ExpensesDetails" & Session("WebtableID") & " ED inner join ExpenseTypeMaster" & Session("WebtableID") & " ETM on ETM.ExpenseTypeMasterID=ED.ExpenseTypeMasterID where ExpensesDetailsId=" & ExpensesDetailsId & " and Emp_Code='" & emp_code & "'  order by ExpensesDetailsId desc"
                    Dim dsExpenseDetails As DataTable = OldNewConn.GetDataTable2(sql_Query)

                    sb.Append("<TABLE border=1 cellPadding=0 width=100% cellSpacing=0 style=" & Chr(34) & "font-family: Segoe UI,Verdana,Arial,Georgia;font-size:10pt" & Chr(34) & " width=60%>")
                    sb.Append("<tr><th>Claim ID</th><th>Employee Code</th><th>Employee Name</th><th>Expense Type</th><th>Claim date</th><th>Claim Amount</th></tr>")

                    For j As Integer = 0 To dsExpenseDetails.Rows.Count - 1
                        sb.Append("<tr>")
                        sb.Append("<td>" & dsExpenseDetails.Rows(j)("ExpensesDetailsId").ToString & "</td>")
                        sb.Append("<td>" & dsExpenseDetails.Rows(j)("Emp_Code").ToString & "</td>")
                        sb.Append("<td>" & dsExpenseDetails.Rows(j)("Field2").ToString & "</td>")
                        sb.Append("<td>" & dsExpenseDetails.Rows(j)("Name").ToString & "</td>")
                        sb.Append("<td>" & CDate(dsExpenseDetails.Rows(j)("ExpensesDate")).Date.ToString("dd/MM/yyyy") & "</td>")
                        sb.Append("<td>" & CDec(dsExpenseDetails.Rows(j)("FieldtotalClaim")).ToString("####.00") & "</td>")
                        sb.Append("</tr>")
                    Next
                    sb.Append("</TABLE><br/>")

                    mailbody = Replace(mailbody, "#Expensedetails#", sb.ToString)

                    mailbody = MailDetails(dsExpenseDetails, ExpensesDetailsId, mailbody)

                    If Not IsDBNull(dtEmployeeDetails.Rows(0)("EmpEmail")) Then
                        If (dtEmployeeDetails.Rows(0)("EmpEmail") <> "") Then
                            Dim flag As Boolean = ConstantValues.SendEmail(dtEmployeeDetails.Rows(0)("EmpEmail"), mailSubject, mailbody, "", "", "from5", "subject5", "password5")

                            If flag = True Then
                                str = "update ExpensesDetails" & Session("WebTableID") & " set IsmanagerApproved=3,IsHODApproved=0,IsAdminApproved=4,AdminApprovalDate=CONVERT(date, getdate()),AdminRemarks='" & txtReasonAccept.Text & "' where ExpensesDetailsID=" & ExpensesDetailsId & " and Emp_Code='" & emp_code & "'"
                                res = OldNewConn.ExecuteNonQuery(CommandType.Text, str, Nothing)

                                If Not IsDBNull(dsExpenseDetails.Rows(0)("AdvanceTakenID").ToString) Then
                                    If (dsExpenseDetails.Rows(0)("AdvanceTakenID").ToString <> "") Then
                                        Dim AdvanceAmt As Decimal = 0
                                        If (CDec(dsExpenseDetails.Rows(0)("FieldtotalClaim")) > CDec(dsExpenseDetails.Rows(0)("FieldAdvTaken"))) Then
                                            AdvanceAmt = CDec(dsExpenseDetails.Rows(0)("FieldAdvTaken"))
                                        Else
                                            AdvanceAmt = CDec(dsExpenseDetails.Rows(0)("FieldtotalClaim"))
                                        End If

                                        str = "Update AdvanceTaken" & Session("WebTableID") & " set AmountClaimAgainst=AmountClaimAgainst-" & AdvanceAmt & ",isSettled=0 where AdvanceTakenID=" & dsExpenseDetails.Rows(0)("AdvanceTakenID").ToString & ""
                                        OldNewConn.ExecuteNonQuery(CommandType.Text, str, Nothing)
                                    End If
                                End If
                            End If

                            Enter_EmailLog(Session("Emp_Code"), Session("WebTableID"), 1, "DKMWebApplication", mailSubject, "To : " & dtEmployeeDetails.Rows(0)("EmpEmail"), mailbody, "", Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type)
                        End If
                    End If
                End If

                If res > 0 Then
                    ScriptManager.RegisterStartupScript(Me, [GetType](), "alert", "Partially();", True)
                End If

                radGrid2.Rebind()

            Catch ex As Exception
                ClientScript.RegisterStartupScript(Me.[GetType](), "alert", "HideLabel();", True)
            End Try

        End If
    End Sub

    Protected Sub lnkForViewReport2_Command(sender As Object, e As CommandEventArgs)
        Dim ReimburseDetailsID As String = e.CommandArgument.ToString()
        DownloadReimbursementProof(ReimburseDetailsID)
    End Sub

    Public Sub DownloadReimbursementProof(ByVal ReimburseDetailsID As String)
        Using zip As New ZipFile()
            zip.AlternateEncodingUsage = ZipOption.AsNecessary
            zip.AddDirectoryByName("Files")

            Dim Str As String = "Select *  from ReimbursementProofUpload" & Session("WebTableID") & " where ReimburseDetailsID=" & ReimburseDetailsID & ""
            Dim UploadedDocumentDetails As DataTable = OldNewConn.GetDataTable2(Str)

            Try
                Dim path As String = ""
                If (UploadedDocumentDetails.Rows.Count > 0) Then
                    For i As Integer = 0 To UploadedDocumentDetails.Rows.Count - 1
                        Thread.Sleep(1000)
                        Dim extraFileName As String = UploadedDocumentDetails.Rows(i)("Emp_code").ToString().Replace("/", "_")
                        extraFileName = extraFileName & "wl" & (Now.TimeOfDay.Hours * 42) & "-" & CInt(Now.TimeOfDay.Minutes + 4) & "-" & CInt(Now.TimeOfDay.Seconds + 4) & "-" & Today.Date.Day & "-" & Format(Today.Date.Date, "ddmmyy")
                        Dim FileName As String = HttpContext.Current.Request.PhysicalApplicationPath & "OutPutFiles\" & "" & HttpContext.Current.Session("WebTableID") & "Letter" & extraFileName & "" & UploadedDocumentDetails.Rows(i)("Ext").ToString() & ""
                        Dim bytes As Byte()
                        bytes = DirectCast(UploadedDocumentDetails.Rows(i)("fileData"), Byte())

                        Dim newFile As FileStream = New FileStream(FileName, FileMode.Create)
                        newFile.Write(bytes, 0, bytes.Length)
                        newFile.Close()
                        zip.AddFile(FileName, "Files")
                    Next
                End If
            Catch ex As SqlException
            End Try

            Response.Clear()
            Response.BufferOutput = False
            Dim zipName As String = [String].Format("Zip_{0}.zip", UploadedDocumentDetails.Rows(0)("Emp_code"))
            Response.ContentType = "application/zip"
            Response.AddHeader("content-disposition", "attachment; filename=" + zipName)
            zip.Save(Response.OutputStream)
            Response.End()
        End Using
    End Sub

    Protected Function MailDetails(ByVal dsExpenseDetails As DataTable, ByVal ExpensesDetailsId As String, ByVal mailbody As String)
        Dim sb As System.Text.StringBuilder = New StringBuilder()

        If (dsExpenseDetails.Rows(0)("Name").ToString.ToLower = "local conveyance") Then
            sb.Append("<TABLE border=1 cellPadding=0 width=100% cellSpacing=0 style=" & Chr(34) & "font-family: Segoe UI,Verdana,Arial,Georgia;font-size:10pt" & Chr(34) & " width=60%>")
            sb.Append("<tr><th>Date</th><th>From</th><th>To</th><th>Mode</th><th>Amount</th><th>Remarks</th></tr>")

            Dim dtReimbursetable As DataTable = OldNewConn.GetDataTable2("select * from LocalConveyance" & Session("WebTableID") & " where ExpensesDetailsID=" & ExpensesDetailsId & "  order by LocalConveyanceID desc")
            For j As Integer = 0 To dtReimbursetable.Rows.Count - 1
                sb.Append("<tr>")
                sb.Append("<td>" & CDate(dtReimbursetable.Rows(j)("Date")).Date.ToString("dd/MM/yyyy") & "</td>")
                sb.Append("<td>" & dtReimbursetable.Rows(j)("FromLoc").ToString & "</td>")
                sb.Append("<td>" & dtReimbursetable.Rows(j)("ToLoc").ToString & "</td>")
                sb.Append("<td>" & dtReimbursetable.Rows(j)("Mode").ToString & "</td>")
                sb.Append("<td>" & CDec(dtReimbursetable.Rows(j)("Amount")).ToString("####.00") & "</td>")
                sb.Append("<td>" & dtReimbursetable.Rows(j)("Remarks").ToString & "</td>")
                sb.Append("</tr>")
            Next
            sb.Append("</TABLE><br/>")
        ElseIf (dsExpenseDetails.Rows(0)("Name").ToString.ToLower = "mobile expenses") Then
            sb.Append("<TABLE border=1 cellPadding=0 width=100% cellSpacing=0 style=" & Chr(34) & "font-family: Segoe UI,Verdana,Arial,Georgia;font-size:10pt" & Chr(34) & " width=60%>")
            sb.Append("<tr><th>Nature Of Expense</th><th>Bill Number</th><th>Bill Date</th><th>Mobile Number</th><th>Amount</th><th>Remarks</th></tr>")

            Dim dtReimbursetable As DataTable = OldNewConn.GetDataTable2("select * from MobileExpense" & Session("WebTableID") & " where ExpensesDetailsID=" & ExpensesDetailsId & "  order by MobileExpenseID desc")
            For j As Integer = 0 To dtReimbursetable.Rows.Count - 1
                sb.Append("<tr>")
                sb.Append("<td>" & dtReimbursetable.Rows(j)("NatureOfExp").ToString & "</td>")
                sb.Append("<td>" & dtReimbursetable.Rows(j)("BillNumber").ToString & "</td>")
                sb.Append("<td>" & CDate(dtReimbursetable.Rows(j)("BillDate")).Date.ToString("dd/MM/yyyy") & "</td>")
                sb.Append("<td>" & dtReimbursetable.Rows(j)("MobileNumber").ToString & "</td>")
                sb.Append("<td>" & CDec(dtReimbursetable.Rows(j)("Amount")).ToString("####.00") & "</td>")
                sb.Append("<td>" & dtReimbursetable.Rows(j)("BillDetail").ToString & "</td>")
                sb.Append("</tr>")
            Next
            sb.Append("</TABLE><br/>")
        Else
            sb.Append("<TABLE border=1 cellPadding=0 width=100% cellSpacing=0 style=" & Chr(34) & "font-family: Segoe UI,Verdana,Arial,Georgia;font-size:10pt" & Chr(34) & " width=60%>")
            sb.Append("<tr><th>Nature Of Expense</th><th>Bill Number</th><th>Bill Date</th><th>Amount</th><th>Remarks</th></tr>")

            Dim dtReimbursetable As DataTable = OldNewConn.GetDataTable2("select * from OtherExpense" & Session("WebTableID") & " where ExpensesDetailsID=" & ExpensesDetailsId & "  order by OtherExpenseID desc")
            For j As Integer = 0 To dtReimbursetable.Rows.Count - 1
                sb.Append("<tr>")
                sb.Append("<td>" & dtReimbursetable.Rows(j)("NatureOfExp").ToString & "</td>")
                sb.Append("<td>" & dtReimbursetable.Rows(j)("BillNumber").ToString & "</td>")
                sb.Append("<td>" & CDate(dtReimbursetable.Rows(j)("BillDate")).Date.ToString("dd/MM/yyyy") & "</td>")
                sb.Append("<td>" & CDec(dtReimbursetable.Rows(j)("BillAmount")).ToString("####.00") & "</td>")
                sb.Append("<td>" & dtReimbursetable.Rows(j)("BillDetail").ToString & "</td>")
                sb.Append("</tr>")
            Next
            sb.Append("</TABLE><br/>")
        End If

        mailbody = Replace(mailbody, "#Summary#", sb.ToString)

        Return mailbody
    End Function

    Public Function Enter_EmailLog(ByVal UserID As String, ByVal WebTableID As Integer, ByVal ActivityStatus As Short, ByVal EmailInformation As String, ByVal EmailSubject As String, ByVal EmailIDInformation As String, ByVal EmailText As String, ByVal EmailActivityInformation As String, ByVal IPAddress As String, ByVal BrowserName As String)
        Dim commendText As String
        Try
            UserID = Left(Replace(Trim(UserID), "'", ""), 20)
            Dim ConnectionString As String = ""
            commendText = "insert into EmailInformationLog" & Format(Today, "MMMyyyy") & " values ('" & UserID & "' , '" & WebTableID & "' , '" & Format(Today, "MM/dd/yyyy") & "' ,'" & Format(Now, "hh:mm:ss") & "' ," & ActivityStatus & ", '" & EmailInformation & "','" & EmailSubject & "' ,  '" & EmailIDInformation & "','" _
             & EmailText & "' , '" & EmailActivityInformation & "','', '" & IPAddress & "','" & BrowserName & "')"

            Dim cmd As SqlCommand = New SqlCommand
            Dim conn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("DkmOnlineConnectionStringDKMErrorLog").ConnectionString)
            'Dim trans As SqlTransaction = conn.BeginTransaction("BuilderTransaction")
            Try
                PrepareCommand(cmd, conn, CommandType.Text, commendText, Nothing)
                Dim val As Integer = cmd.ExecuteNonQuery()
                cmd.Parameters.Clear()
                Return val
            Catch ex As SqlException
                Throw New Exception("SQL Exception1 " & ex.Message)
            Catch exx As Exception
                Throw New Exception("ExecuteNonQuery Function", exx)
            Finally                'Add this for finally closing the connection and destroying the command
                conn.Close()
                cmd = Nothing
            End Try
        Catch ex As Exception

        End Try

    End Function

    Public Shared Function PrepareCommand(ByRef cmd As SqlCommand, ByRef conn As SqlConnection, ByRef cmdType As CommandType, ByRef cmdText As String, ByRef cmdParms As SqlParameter()) As Boolean
        If Not conn.State = ConnectionState.Open Then
            conn.Open()
        End If
        Try
            cmd.Connection = conn
            cmd.CommandText = cmdText
            cmd.Parameters.Clear()
            cmd.CommandType = cmdType
            If Not (IsNothing(cmdParms)) Then
                Dim parm As SqlParameter
                For Each parm In cmdParms
                    cmd.Parameters.Add(parm)
                Next
            End If
        Catch ex As SqlException
            Throw New Exception("SQL Exception ", ex)
        Catch exx As Exception
            Throw New Exception("PrepareCommand : ", exx)
        End Try
    End Function
End Class