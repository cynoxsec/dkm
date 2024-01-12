Imports DkmOnline.Lib
Imports System.Net.Mail
Imports iTextSharp.text
Imports iTextSharp
Imports iTextSharp.text.pdf
Imports Telerik.Web.UI
Imports System.Data.Sql
Imports System.IO
Imports DkmOnline.Common
Imports System.Data.SqlClient
Imports System.Net
Imports System.Security.Cryptography
Imports System.Globalization
Imports ClosedXML.Excel

Public Class ApproveRejectReim
    Inherits System.Web.UI.Page

    Dim WebtableID As String
    Dim commendText As String

    Dim Year As Int32

    Dim OldNewConn As OldNewConnection = New OldNewConnection()

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsNothing(Session("Emp_Code")) Then
            Response.Redirect("../Logout.aspx")
        End If

        DatabaseFacade.Instance.Enter_SiteLog(Session("Emp_Code"), Session("LoginType"), Session("LoginAccessType"), Session("Comp_Code"), 0, Request.Url.AbsolutePath, Request.Url.AbsolutePath, Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type, Session("WebTableID"), Session.SessionID, Session("MenuID"))

        OldNewConn.CheckDatabaseOldNew(Session("WebTableID"))

        If Not IsNothing(Request.QueryString("WebtableID")) Then
            Try
                Dim s As String = Request.QueryString("WebtableID").Replace(" ", "+")
                If s.Length Mod 4 > 0 Then
                    s = s.PadRight(s.Length + 4 - s.Length Mod 4, "="c)
                End If

                WebtableID = EncryDecrypt.Decrypt(s, "a")

            Catch ex As Exception
                Response.Redirect("../Logout.aspx")
            End Try
        End If

        Dim dtYear As DataTable = OldNewConn.GetDataTable2("select * from dbo.ReimYearMaster" & Session("WebTableID") & "")

        If (dtYear.Rows.Count > 0) Then
            Year = dtYear.Rows(0)("Year")
        End If


        If Not (IsPostBack) Then

            Dim dt As DataTable = OldNewConn.GetDataTable2("select distinct right(table_name,4) as finyear from INFORMATION_SCHEMA.columns where table_name like 'ReimbursementProofrejection%' and table_name like '%" & WebtableID & "%'")

            If dt.Rows.Count > 0 Then
                ddlyear.Items.Clear()
                ddlyear.DataSource = dt
                ddlyear.DataTextField = "finyear"
                ddlyear.DataValueField = "finyear"
                ddlyear.DataBind()
                ddlyear.Items.Insert(0, New System.Web.UI.WebControls.ListItem("Please Select", "0"))
            End If

            Dim dtCSVwithOutSubClaimID As DataTable = OldNewConn.GetDataTable2("Select name, print_name from payslipsetup" & Session("WebTableID") & " where name in ('CSVwithSubClaimID')")

            If (dtCSVwithOutSubClaimID.Rows.Count > 0) Then
                If (dtCSVwithOutSubClaimID.Rows(0)("print_name").ToString.ToLower = "n") Then
                    ViewState("WithOutSubClaimID") = "CSVwithOutSubClaimID"
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Function to upload CSV
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>

    Protected Sub lnkViewTemplate_Click(sender As Object, e As EventArgs) Handles lnkViewTemplate.Click

        Dim filename As String = DateTime.Now.ToString("dd/MM/yyyy") + ".csv"

        Response.Clear()
        Response.Buffer = True
        Response.AddHeader("content-disposition", "attachment; filename=" & filename & "")
        Response.Charset = ""
        Response.ContentType = "application/text"
        Dim listForRewards As List(Of String) = createTemplateForRewards()

        Dim sb As New StringBuilder()
        For Each name As String In listForRewards
            sb.Append(name + ","c)
        Next
        sb.Append(vbCr & vbLf)

        Response.Write(sb.ToString())
        Response.Flush()
        Response.[End]()
    End Sub

    Private Function createTemplateForRewards() As List(Of String)
        Dim list As New List(Of String)
        list.Add("Emp_code")
        list.Add("ClaimID")
        list.Add("ReimbursementType")
        list.Add("ApprovedAmount")
        list.Add("Status_Date")
        list.Add("Status")
        list.Add("Remarks")
        If (ViewState("WithOutSubClaimID") = "CSVwithOutSubClaimID") Then
        Else
            list.Add("SubClaimID")
        End If
        Return list
    End Function

    Protected Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnUpload.Click

        Try
            Dim DateFormat As String = "dd/MM/yyyy"
            Dim dt As New DataTable

            dt.Columns.Add("Emp_code")
            dt.Columns.Add("ClaimID")
            dt.Columns.Add("ReimbursementType")
            dt.Columns.Add("ApprovedAmount")
            dt.Columns.Add("Status_Date", GetType(System.DateTime))
            dt.Columns.Add("Status")
            dt.Columns.Add("Remarks")
            dt.Columns.Add("UploadedBy")
            dt.Columns.Add("UploadedDate", GetType(System.DateTime))
            dt.Columns.Add("IsSent")
            dt.Columns.Add("MailSentDate", GetType(System.DateTime))
            If (ViewState("WithOutSubClaimID") = "CSVwithOutSubClaimID") Then
            Else
                dt.Columns.Add("SubClaimID")
            End If

            If (radAsynUpload.UploadedFiles.Count = 0) Then
                Response.Write("<script>alert('Please Select CSV File.');</script>")
                Exit Sub
            End If

            For Each rad As UploadedFile In radAsynUpload.UploadedFiles
                Dim sr As New StreamReader(rad.InputStream)

                Dim sr1 As New StreamReader(rad.InputStream)

                Dim line As String = sr.ReadLine()
                Dim value As String() = line.Split(","c)

                Dim line2 As String = sr1.ReadLine()

                Dim dr As DataRow

                For Each singleVal As String In value
                    If Not (dt.Columns.Contains(singleVal)) Then
                        lblError.Text = "Please check the CSV file."
                        Return
                    End If
                Next

                While Not sr1.EndOfStream
                    value = sr1.ReadLine().Split(","c)
                    If (value.Count > 1) Then
                        For i As Int32 = 0 To value.Count - 1

                            If (value(0) = "") Then
                                lblError.Text = "Please check the CSV file, employee code column can't be blank."
                                Return
                            End If

                            If (value(1) = "") Then
                                lblError.Text = "Please check the CSV file, claim ID column can't be blank."
                                Return
                            End If

                            If Not IsNumeric(value(1)) Then
                                lblError.Text = "Please check the CSV file, claim ID column should be numeric or may be invalid."
                                Return
                            End If

                            Dim dtClaimID As DataTable = OldNewConn.GetDataTable2("select IsNull(PSS.Print_Name,RTM.Name) as 'Field_Name' from reimbursementdetails" & Session("WebTableiD") & " RD inner join ReimburseMentTypeMaster" & Session("WebTableiD") & " RTM on RTM.ReimburseType_Code=RD.ReimburseType_Code Left join payslipsetup" & Session("WebTableiD") & " PSS on PSS.Name =RTM.Name where RD.emp_Code = '" & value(0) & "' And RD.ReimburseDetailsID='" & value(1) & "'")

                            If (dtClaimID.Rows.Count = 0) Then
                                lblError.Text = String.Format("Claim id ({0}) not pertain to that employee ({1}) and should not allow to upload", value(1).Trim, value(0).Trim)
                                Return
                            End If

                            If (value(2) = "") Then
                                lblError.Text = "Please check the CSV file, reimbursement type column can't be blank."
                                Return
                            End If

                            If (value(2).ToLower <> dtClaimID.Rows(0)("Field_Name").ToString.ToLower) Then
                                lblError.Text = String.Format("Please check the CSV file, claim id ({0}) should match with reimbursement type.", value(1).Trim)
                                Return
                            End If

                            If (value(4) = "") Then
                                lblError.Text = "Please check the CSV file, status date column can't be blank."
                                Return
                            End If

                            If (value(5).ToString.ToLower <> "approved" And value(5).ToString.ToLower <> "rejected" And value(5).ToString.ToLower <> "partially approved") Then
                                lblError.Text = "Please check the CSV file, status can be Approved/Rejected/Partially Approved only."
                                Return
                            End If

                            If (value(5).ToString.ToLower = "approved" And value(3) = "") Then
                                lblError.Text = "Please check the CSV file, approved amount column can't be blank."
                                Return
                            End If

                            If (value(6) = "" And (value(5).ToString.ToLower = "rejected" Or value(5).ToString.ToLower = "partially approved")) Then
                                lblError.Text = "Remarks can't be blank,if status is Rejected/Partially Approved"
                                Return
                            End If

                            If (ViewState("WithOutSubClaimID") = "CSVwithOutSubClaimID") Then
                            Else
                                Dim dtSubClaimID As New DataTable

                                If (value(2).ToLower.Contains("taxable lta")) Then
                                    dtSubClaimID = OldNewConn.GetDataTable2("select BillAmount from OtherMultipleReimburseClaimDetails" & Session("WebTableiD") & " where Emp_Code = '" & value(0) & "' And ReimburseDetailsID='" & value(1) & "'")
                                ElseIf (value(2).ToLower.Contains("lta")) Then
                                    dtSubClaimID = OldNewConn.GetDataTable2("select ClaimAmount as Billamount from OtherReimbursementPersonsDetail" & Session("WebTableID") & " where Emp_Code = '" & value(0) & "' And ReimburseDetailsID='" & value(1) & "'")
                                Else
                                    dtSubClaimID = OldNewConn.GetDataTable2("select BillAmount from OtherMultipleReimburseClaimDetails" & Session("WebTableiD") & " where Emp_Code = '" & value(0) & "' And ReimburseDetailsID='" & value(1) & "'")
                                End If

                                If (dtSubClaimID.Rows.Count > 0) Then

                                    dtSubClaimID.Dispose()

                                    If (value(7).ToString.ToLower = "") Then
                                        lblError.Text = "Please check the CSV file, sub claim ID column can't be blank."
                                        Return
                                    End If

                                    If (value(2).ToLower.Contains("taxable lta")) Then
                                        dtSubClaimID = OldNewConn.GetDataTable2("select BillAmount from OtherMultipleReimburseClaimDetails" & Session("WebTableiD") & " where MultipleClaimDetailsID='" & value(7) & "' and Emp_Code = '" & value(0) & "' And ReimburseDetailsID='" & value(1) & "'")
                                    ElseIf (value(2).ToLower.Contains("lta")) Then
                                        dtSubClaimID = OldNewConn.GetDataTable2("select ClaimAmount as Billamount from OtherReimbursementPersonsDetail" & Session("WebTableID") & " where ReimbursementPersonsDetailID='" & value(7) & "' and Emp_Code = '" & value(0) & "' And ReimburseDetailsID='" & value(1) & "'")
                                    Else
                                        dtSubClaimID = OldNewConn.GetDataTable2("select BillAmount from OtherMultipleReimburseClaimDetails" & Session("WebTableiD") & " where MultipleClaimDetailsID='" & value(7) & "' and Emp_Code = '" & value(0) & "' And ReimburseDetailsID='" & value(1) & "'")
                                    End If

                                    If (dtSubClaimID.Rows.Count = 0) Then
                                        lblError.Text = String.Format("Sub Claim id ({0}) not pertain to that employee ({1}) and should not allow to upload", value(7).Trim, value(0).Trim)
                                        Return
                                    End If

                                    If ((value(5).ToString.ToLower = "approved" Or value(5).ToString.ToLower = "partially approved") And CInt(value(3)) = 0) Then
                                        lblError.Text = String.Format("Sub Claim id ({0}) should either be rejected or if approved/partially approved, approved amount should not be 0", value(7).Trim)
                                        Return
                                    End If

                                    If (value(5).ToString.ToLower = "rejected" And CInt(value(3)) <> 0) Then
                                        lblError.Text = String.Format("Status for sub Claim id ({0}) is rejected, approved amount should be zero or status should be changed", value(7).Trim)
                                        Return
                                    End If

                                    If (value(5).ToString.ToLower = "approved" And CInt(dtSubClaimID.Rows(0)("BillAmount")) <> CInt(value(3))) Then
                                        lblError.Text = String.Format("Sub Claim id ({0}) should either be partially approved or if approved, approved amount should match with claim/bill amount.", value(7).Trim)
                                        Return
                                    End If
                                End If
                            End If
                        Next
                    End If
                End While

                While Not sr.EndOfStream
                    value = sr.ReadLine().Split(","c)

                    If (value.Count > 1) Then

                        dr = dt.NewRow()

                        dr("Emp_code") = value(0)
                        dr("ClaimID") = value(1).ToString
                        dr("ReimbursementType") = value(2).ToString.ToUpper
                        dr("ApprovedAmount") = value(3).ToString

                        Dim Status_Date As Date
                        Dim d As Date

                        If Date.TryParseExact(Trim(value(4)), "d/M/yyyy", CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                            Status_Date = d
                        Else
                            lblError.Text = "Error : Date : " & value(4) & ", please enter the date in " & StrConv(DateFormat, VbStrConv.Uppercase) & " format!"
                            Exit Sub
                        End If

                        dr("Status_Date") = Status_Date

                        dr("Status") = value(5).ToUpper
                        Dim remarks As String = value(6)

                        remarks = remarks.Replace("'", "")
                        remarks = remarks.Replace(";", "")
                        remarks = remarks.Replace("#", ",")
                        remarks = remarks.Replace("--", "")
                        remarks = remarks.Replace(">", "")
                        remarks = remarks.Replace("<", "")
                        remarks = remarks.Replace("[", "")
                        remarks = remarks.Replace("]", "")
                        remarks = remarks.Replace("@", "")
                        remarks = remarks.Replace("""", "")

                        dr("Remarks") = remarks
                        dr("UploadedBy") = Session("Emp_Code")
                        dr("UploadedDate") = System.DateTime.Now
                        dr("IsSent") = "0"
                        dr("MailSentDate") = System.DateTime.Now
                        If (ViewState("WithOutSubClaimID") = "CSVwithOutSubClaimID") Then
                        Else
                            dr("SubClaimID") = value(7)
                        End If
                        dt.Rows.Add(dr)
                    End If
                End While
            Next

            Dim dvFail As DataView = New DataView(dt)
            Dim distinct As New DataTable

            If (ViewState("WithOutSubClaimID") = "CSVwithOutSubClaimID") Then
                distinct = dvFail.ToTable(True, "Emp_code", "ClaimID", "ReimbursementType", "ApprovedAmount", "Status_Date", "Status", "Remarks", "UploadedBy", "UploadedDate", "IsSent", "MailSentDate")
            Else
                distinct = dvFail.ToTable(True, "Emp_code", "ClaimID", "ReimbursementType", "ApprovedAmount", "Status_Date", "Status", "Remarks", "UploadedBy", "UploadedDate", "IsSent", "MailSentDate", "SubClaimID")
            End If

            If (dt.Rows.Count <> distinct.Rows.Count) Then
                lblError.Text = "This file has duplicate claim id, please check and re-upload."
                Exit Sub
            End If

            Dim Str As String = "Select * from ReimbursementProofrejection" & WebtableID & "_" & Year & ""
            Dim dtReimbursementProofrejection = OldNewConn.GetDataTable2(Str)

            Dim workcolumn As DataColumn = distinct.Columns.Add("Sno", GetType(Int32))
            workcolumn.AutoIncrement = True
            workcolumn.AutoIncrementSeed = 1
            workcolumn.AutoIncrementStep = 1
            workcolumn.SetOrdinal(0)

            For incre As Int32 = 0 To distinct.Rows.Count - 1
                distinct.Rows(incre).Item("Sno") = incre + 1
            Next

            Dim firstlist As List(Of Integer) = New List(Of Integer)()

            For i As Int32 = distinct.Rows.Count - 1 To 0 Step -1
                For j As Int32 = 0 To dtReimbursementProofrejection.Rows.Count - 1
                    If (distinct.Rows.Count > 0) Then
                        If (ViewState("WithOutSubClaimID") = "CSVwithOutSubClaimID") Then
                            If (distinct.Rows(i)("Emp_code").ToString.Trim().ToLower() = dtReimbursementProofrejection.Rows(j)("Emp_code").ToString.Trim().ToLower() And distinct.Rows(i)("ClaimID").ToString.Trim().ToLower() = dtReimbursementProofrejection.Rows(j)("ClaimID").ToString.Trim().ToLower() And distinct.Rows(i)("ReimbursementType").ToString.Trim().ToLower() = dtReimbursementProofrejection.Rows(j)("ReimbursementType").ToString.Trim().ToLower() And distinct.Rows(i)("Status").ToString.Trim().ToLower() = dtReimbursementProofrejection.Rows(j)("Status").ToString.Trim().ToLower() And CDate(distinct.Rows(i)("Status_Date").ToString.Trim()).Date = CDate(dtReimbursementProofrejection.Rows(j)("Status_Date").ToString.Trim()).Date And distinct.Rows(i)("Remarks").ToString.Trim().ToLower() = dtReimbursementProofrejection.Rows(j)("Remarks").ToString.Trim().ToLower() And CDec(distinct.Rows(i)("ApprovedAmount")) = CDec(dtReimbursementProofrejection.Rows(j)("ApprovedAmount")) And dtReimbursementProofrejection.Rows(j)("IsSent").ToString.Trim().ToLower() = "1") Then
                                firstlist.Add(distinct.Rows(i)("Sno"))
                            ElseIf (distinct.Rows(i)("Emp_code").ToString.Trim().ToLower() = dtReimbursementProofrejection.Rows(j)("Emp_code").ToString.Trim().ToLower() And distinct.Rows(i)("ClaimID").ToString.Trim().ToLower() = dtReimbursementProofrejection.Rows(j)("ClaimID").ToString.Trim().ToLower() And distinct.Rows(i)("ReimbursementType").ToString.Trim().ToLower() = dtReimbursementProofrejection.Rows(j)("ReimbursementType").ToString.Trim().ToLower() And distinct.Rows(i)("Status").ToString.Trim().ToLower() = dtReimbursementProofrejection.Rows(j)("Status").ToString.Trim().ToLower() And CDate(distinct.Rows(i)("Status_Date").ToString.Trim()).Date = CDate(dtReimbursementProofrejection.Rows(j)("Status_Date").ToString.Trim()).Date And distinct.Rows(i)("Remarks").ToString.Trim().ToLower() = dtReimbursementProofrejection.Rows(j)("Remarks").ToString.Trim().ToLower() And CDec(distinct.Rows(i)("ApprovedAmount")) = CDec(dtReimbursementProofrejection.Rows(j)("ApprovedAmount")) And dtReimbursementProofrejection.Rows(j)("IsSent").ToString.Trim().ToLower() = "0") Then
                                Str = "Delete ReimbursementProofrejection" & WebtableID & "_" & Year & " where ReimbursementProofID='" & dtReimbursementProofrejection.Rows(j)("ReimbursementProofID") & "'"
                                OldNewConn.ExecuteNonQuery(CommandType.Text, Str, Nothing)
                            End If
                        Else
                            If (distinct.Rows(i)("Emp_code").ToString.Trim().ToLower() = dtReimbursementProofrejection.Rows(j)("Emp_code").ToString.Trim().ToLower() And distinct.Rows(i)("ClaimID").ToString.Trim().ToLower() = dtReimbursementProofrejection.Rows(j)("ClaimID").ToString.Trim().ToLower() And distinct.Rows(i)("ReimbursementType").ToString.Trim().ToLower() = dtReimbursementProofrejection.Rows(j)("ReimbursementType").ToString.Trim().ToLower() And distinct.Rows(i)("Status").ToString.Trim().ToLower() = dtReimbursementProofrejection.Rows(j)("Status").ToString.Trim().ToLower() And CDate(distinct.Rows(i)("Status_Date").ToString.Trim()).Date = CDate(dtReimbursementProofrejection.Rows(j)("Status_Date").ToString.Trim()).Date And distinct.Rows(i)("Remarks").ToString.Trim().ToLower() = dtReimbursementProofrejection.Rows(j)("Remarks").ToString.Trim().ToLower() And CDec(distinct.Rows(i)("ApprovedAmount")) = CDec(dtReimbursementProofrejection.Rows(j)("ApprovedAmount")) And dtReimbursementProofrejection.Rows(j)("IsSent").ToString.Trim().ToLower() = "1" And distinct.Rows(i)("SubClaimID").ToString.Trim().ToLower() = dtReimbursementProofrejection.Rows(j)("SubClaimID").ToString.Trim().ToLower()) Then
                                firstlist.Add(distinct.Rows(i)("Sno"))
                            ElseIf (distinct.Rows(i)("Emp_code").ToString.Trim().ToLower() = dtReimbursementProofrejection.Rows(j)("Emp_code").ToString.Trim().ToLower() And distinct.Rows(i)("ClaimID").ToString.Trim().ToLower() = dtReimbursementProofrejection.Rows(j)("ClaimID").ToString.Trim().ToLower() And distinct.Rows(i)("ReimbursementType").ToString.Trim().ToLower() = dtReimbursementProofrejection.Rows(j)("ReimbursementType").ToString.Trim().ToLower() And distinct.Rows(i)("Status").ToString.Trim().ToLower() = dtReimbursementProofrejection.Rows(j)("Status").ToString.Trim().ToLower() And CDate(distinct.Rows(i)("Status_Date").ToString.Trim()).Date = CDate(dtReimbursementProofrejection.Rows(j)("Status_Date").ToString.Trim()).Date And distinct.Rows(i)("Remarks").ToString.Trim().ToLower() = dtReimbursementProofrejection.Rows(j)("Remarks").ToString.Trim().ToLower() And CDec(distinct.Rows(i)("ApprovedAmount")) = CDec(dtReimbursementProofrejection.Rows(j)("ApprovedAmount")) And dtReimbursementProofrejection.Rows(j)("IsSent").ToString.Trim().ToLower() = "0" And distinct.Rows(i)("SubClaimID").ToString.Trim().ToLower() = dtReimbursementProofrejection.Rows(j)("SubClaimID").ToString.Trim().ToLower()) Then
                                Str = "Delete ReimbursementProofrejection" & WebtableID & "_" & Year & " where ReimbursementProofID='" & dtReimbursementProofrejection.Rows(j)("ReimbursementProofID") & "'"
                                OldNewConn.ExecuteNonQuery(CommandType.Text, Str, Nothing)
                            End If
                        End If
                    End If
                Next
            Next

            For Each element As Int32 In firstlist
                For incre2 As Int32 = distinct.Rows.Count - 1 To 0 Step -1
                    If (element = CInt(distinct.Rows(incre2)("Sno"))) Then
                        distinct.Rows(incre2).Delete()
                        distinct.AcceptChanges()
                    End If
                Next
            Next

            distinct.Columns.Remove("Sno")


            If distinct.Rows.Count > 0 Then
                Dim sqlParameter As SqlParameter() = New SqlParameter(0) {}

                sqlParameter(0) = New SqlParameter
                sqlParameter(0).ParameterName = "@dt"
                sqlParameter(0).SqlDbType = SqlDbType.Structured
                sqlParameter(0).Value = distinct

                If (ViewState("WithOutSubClaimID") = "CSVwithOutSubClaimID") Then
                    commendText = String.Format("Insert_ReimbursementProofrejection_Details{0}_{1}", WebtableID, Year)
                Else
                    commendText = String.Format("Insert_ReimbursementProofrejection_Details_WithSubClaimID{0}_{1}", WebtableID, Year)
                End If

                OldNewConn.ExecuteNonQuery(CommandType.StoredProcedure, commendText, sqlParameter)
                lblError.Text = "CSV file Upload Successfully."
            Else
                lblError.Text = "No new records found in the CSV file."
            End If
        Catch ex As Exception
            Enter_ErrorLog(Session("Emp_Code"), Session("WebTableID"), ex.Message, ex.StackTrace, Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type)
            lblError.Text = ex.Message
        End Try
    End Sub

    Protected Sub btnSendTo_Click(sender As Object, e As EventArgs) Handles btnSendTo.Click
        Try
            Dim Str As String = String.Empty
            Dim emailcc As String = String.Empty
            Dim dtTable As New DataTable
            Dim dtEmpCode As DataTable
            Dim dtEmployeeEmail As DataTable
            Dim sbmailCount As System.Text.StringBuilder = New StringBuilder()

            Str = "Select distinct Emp_code from ReimbursementProofrejection" & WebtableID & "_" & Year & " where IsSent='0'"
            dtEmpCode = OldNewConn.GetDataTable2(Str)

            Dim dtMailDetail As DataTable = OldNewConn.GetDataTable2("select Field_Detail as MailSubject, Print_Name as MailBody from PayslipSetup" & WebtableID & " where Name='Reimbursement ApprovedRejection'")

            For i As Integer = 0 To dtEmpCode.Rows.Count - 1
                Dim sb As System.Text.StringBuilder = New StringBuilder()

                Dim dtReimbursement As New DataTable

                Dim Mailsubject As String = dtMailDetail.Rows(0)("MailSubject").ToString()
                Dim MailBody As String = dtMailDetail.Rows(0)("MailBody").ToString()

                Str = "Select FirstName+' '+ LastName as EmployeeName, Email from EmployeesMaster" & WebtableID & " where Emp_Code='" & dtEmpCode.Rows(i)("Emp_code").ToString & "'"
                dtEmployeeEmail = OldNewConn.GetDataTable2(Str)

                Str = "Select distinct ClaimID from ReimbursementProofrejection" & WebtableID & "_" & Year & " where IsSent='0' and emp_code='" & dtEmpCode.Rows(i)("Emp_code").ToString & "'"
                dtTable = OldNewConn.GetDataTable2(Str)

                If (ViewState("WithOutSubClaimID") = "CSVwithOutSubClaimID") Then
                    dtReimbursement.Columns.Add("ClaimID")
                    dtReimbursement.Columns.Add("ReimbursementType")
                    dtReimbursement.Columns.Add("ApprovedAmount")
                    dtReimbursement.Columns.Add("Status_Date")
                    dtReimbursement.Columns.Add("Status")
                    dtReimbursement.Columns.Add("Remarks")

                    For Each drClaimID As DataRow In dtTable.Rows
                        Str = "Select Top 1 ClaimID,ReimbursementType,ApprovedAmount,Status_Date,Status,Remarks from ReimbursementProofrejection" & WebtableID & "_" & Year & " where IsSent='0' and emp_code='" & dtEmpCode.Rows(i)("Emp_code").ToString & "' and ClaimID='" & drClaimID("ClaimID") & "' order by uploadeddate desc"
                        Dim dt As DataTable = OldNewConn.GetDataTable2(Str)

                        For Each dr As DataRow In dt.Rows
                            dtReimbursement.Rows.Add(dr("ClaimID"), dr("ReimbursementType"), Format(dr("ApprovedAmount"), "###0"), dr("Status_Date"), dr("Status"), dr("Remarks"))
                        Next
                    Next

                    sb.Append("<table  border=1>")
                    sb.Append("<tr><th>Claim ID</th><th>Reimbursement Name</th><th>Approved Amount</th><th>Status Date</th><th>Status</th><th>Remarks</th></tr>")

                    For j As Integer = 0 To dtReimbursement.Rows.Count - 1
                        sb.Append("<tr>")
                        sb.Append("<td>" & dtReimbursement.Rows(j)("ClaimID").ToString & "</td>")
                        sb.Append("<td>" & StrConv(dtReimbursement.Rows(j)("ReimbursementType").ToString, VbStrConv.ProperCase) & "</td>")
                        sb.Append("<td>" & CDec(dtReimbursement.Rows(j)("ApprovedAmount")).ToString("####0") & "</td>")
                        sb.Append("<td>" & CDate(dtReimbursement.Rows(j)("Status_Date")).Date.ToString("dd/MM/yyyy") & "</td>")
                        sb.Append("<td>" & StrConv(dtReimbursement.Rows(j)("Status").ToString, VbStrConv.ProperCase) & "</td>")
                        sb.Append("<td>" & dtReimbursement.Rows(j)("Remarks").ToString & "</td>")
                        sb.Append("</tr>")
                    Next
                    sb.Append("</table><br/>")
                Else
                    dtReimbursement.Columns.Add("ClaimID")
                    dtReimbursement.Columns.Add("SubClaimID")
                    dtReimbursement.Columns.Add("ReimbursementType")
                    dtReimbursement.Columns.Add("ApprovedAmount")
                    dtReimbursement.Columns.Add("Status_Date")
                    dtReimbursement.Columns.Add("Status")
                    dtReimbursement.Columns.Add("Remarks")

                    For Each drClaimID As DataRow In dtTable.Rows
                        Str = "Select ClaimID,SubClaimID,ReimbursementType,ApprovedAmount,Status_Date,Status,Remarks from ReimbursementProofrejection" & WebtableID & "_" & Year & " where IsSent='0' and emp_code='" & dtEmpCode.Rows(i)("Emp_code").ToString & "' and ClaimID='" & drClaimID("ClaimID") & "' order by uploadeddate desc"
                        Dim dt As DataTable = OldNewConn.GetDataTable2(Str)

                        For Each dr As DataRow In dt.Rows
                            dtReimbursement.Rows.Add(dr("ClaimID"), dr("SubClaimID"), dr("ReimbursementType"), Format(dr("ApprovedAmount"), "###0"), dr("Status_Date"), dr("Status"), dr("Remarks"))
                        Next
                    Next

                    sb.Append("<table  border=1>")
                    sb.Append("<tr><th>Claim ID</th><th>Sub Claim ID</th><th>Reimbursement Name</th><th>Approved Amount</th><th>Status Date</th><th>Status</th><th>Remarks</th></tr>")

                    For j As Integer = 0 To dtReimbursement.Rows.Count - 1
                        sb.Append("<tr>")
                        sb.Append("<td>" & dtReimbursement.Rows(j)("ClaimID").ToString & "</td>")
                        sb.Append("<td>" & dtReimbursement.Rows(j)("SubClaimID").ToString & "</td>")
                        sb.Append("<td>" & StrConv(dtReimbursement.Rows(j)("ReimbursementType").ToString, VbStrConv.ProperCase) & "</td>")
                        sb.Append("<td>" & CDec(dtReimbursement.Rows(j)("ApprovedAmount")).ToString("####0") & "</td>")
                        sb.Append("<td>" & CDate(dtReimbursement.Rows(j)("Status_Date")).Date.ToString("dd/MM/yyyy") & "</td>")
                        sb.Append("<td>" & StrConv(dtReimbursement.Rows(j)("Status").ToString, VbStrConv.ProperCase) & "</td>")
                        sb.Append("<td>" & dtReimbursement.Rows(j)("Remarks").ToString & "</td>")
                        sb.Append("</tr>")
                    Next
                    sb.Append("</table><br/>")
                End If


                If (dtEmployeeEmail.Rows.Count > 0) Then
                    If (dtEmployeeEmail.Rows(0)("EMail").ToString() <> "") Then
                        MailBody = Replace(MailBody, "#EmployeeName#", StrConv(dtEmployeeEmail.Rows(0)("EmployeeName").ToString, VbStrConv.ProperCase))
                        MailBody = Replace(MailBody, "#CurrentStatus#", sb.ToString)

                        Dim flag As Boolean = SendEmail(dtEmployeeEmail.Rows(0)("EMail").ToString(), Mailsubject, MailBody, emailcc, "", "from", "subject", "password")

                        If (flag = True) Then
                            Dim str12 As String = ("Update ReimbursementProofrejection" & WebtableID & "_" & Year & "  Set isSent=1,MailSentDate=Getdate() where IsSent='0' and emp_code='" & dtEmpCode.Rows(i)("Emp_code").ToString & "'")
                            OldNewConn.ExecuteNonQuery(CommandType.Text, str12, Nothing)
                        End If
                        sbmailCount.Append("Mail sent to - Employee Code : " & dtEmpCode.Rows(i)("Emp_code").ToString & ", Email Address : " & dtEmployeeEmail.Rows(0)("EMail").ToString() & "</br>")
                        Enter_EmailLog(Session("Emp_Code"), Session("WebTableID"), 1, "DKMWebApplication", Mailsubject, "To : " & dtEmployeeEmail.Rows(0)("EMail").ToString(), MailBody, "", Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type)
                    Else
                        sbmailCount.Append("Mail not sent to - Employee Code : " & dtEmpCode.Rows(i)("Emp_code").ToString & ", Email Address : Not Found</br>")
                        Enter_EmailLog(Session("Emp_Code"), Session("WebTableID"), 0, "DKMWebApplication", Mailsubject, "Mail ID Not Found", MailBody, "Mail Not Sent", Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type)
                    End If
                End If
            Next

            If sbmailCount.ToString <> "" Then
                lblError.ForeColor = Drawing.Color.Green
                lblError.Font.Bold = False
                lblError.Text += sbmailCount.ToString
            End If

        Catch ex As Exception
            Enter_ErrorLog(Session("Emp_Code"), Session("WebTableID"), ex.Message, ex.StackTrace, Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type)
            Response.Write("<script>alert('" & Server.HtmlEncode(ex.Message) & "')</script>")
        End Try
    End Sub

    Protected Sub btndownload_Click(sender As Object, e As EventArgs) Handles btndownload.Click

        Dim sql_Query As String = ""
        Dim dsReimburseDetails As New DataTable

        Dim ds As New DataSet
        Dim dtother As New DataTable

        If (ViewState("WithOutSubClaimID") = "CSVwithOutSubClaimID") Then
            dtother = OldNewConn.GetDataTable2("select RPR.Emp_code as [Employee Code],EM.FirstName as [Employee Name], ClaimID as [Claim ID],convert(nvarchar,RD.EntryDate,106) as [Claim Date],ReimbursementType as [Reimbursement Type],CAST(ApprovedAmount  AS INT)as [Approved Amount],convert(nvarchar,Status_Date,106) as [Status Date],Status,RPR.Remarks,Case when IsSent=0 then '' else convert(nvarchar,MailSentDate,106) End as [Mail Sent Date] from ReimbursementProofrejection" & WebtableID & "_" & ddlyear.SelectedItem.Text & " RPR Left join reimbursementdetails" & WebtableID & " RD on RD.ReimburseDetailsID=RPR.ClaimID Left join EmployeesMaster" & WebtableID & " EM on RPR.Emp_code=EM.Emp_Code order by ClaimID desc")
        Else
            dtother = OldNewConn.GetDataTable2("select RPR.Emp_code as [Employee Code],EM.FirstName as [Employee Name], ClaimID as [Claim ID],convert(nvarchar,RD.EntryDate,106) as [Claim Date],ReimbursementType as [Reimbursement Type],CAST(ApprovedAmount  AS INT)as [Approved Amount],convert(nvarchar,Status_Date,106) as [Status Date],Status,RPR.Remarks,Case when SubClaimID is NULL then '' else SubClaimID End as [Sub ClaimID],Case when IsSent=0 then '' else convert(nvarchar,MailSentDate,106) End as [Mail Sent Date] from ReimbursementProofrejection" & WebtableID & "_" & ddlyear.SelectedItem.Text & " RPR Left join reimbursementdetails" & WebtableID & " RD on RD.ReimburseDetailsID=RPR.ClaimID Left join EmployeesMaster" & WebtableID & " EM on RPR.Emp_code=EM.Emp_Code order by ClaimID desc")
        End If

        Dim copyDtother As DataTable = dtother.Copy
        copyDtother.TableName = "Reimbursement Claim status"
        ds.Tables.Add(copyDtother)
        ExcelHelper.ToExcel(ds, "Report", Page.Response)
    End Sub

    Public Function Enter_EmailLog(ByVal UserID As String, ByVal WebTableID As Integer, ByVal ActivityStatus As Short, ByVal EmailInformation As String, ByVal EmailSubject As String, ByVal EmailIDInformation As String, ByVal EmailText As String, ByVal EmailActivityInformation As String, ByVal IPAddress As String, ByVal BrowserName As String)

        Try
            UserID = Left(Replace(Trim(UserID), "'", ""), 20)
            Dim ConnectionString As String = ""
            commendText = "insert into EmailInformationLog" & Format(Today, "MMMyyyy") & " values ('" & UserID & "' , '" & WebTableID & "' , '" & Format(Today, "MM/dd/yyyy") & "' ,'" & Format(Now, "hh:mm:ss") & "' ," & ActivityStatus & ", '" & EmailInformation & "','" & EmailSubject & "' ,  '" & EmailIDInformation & "','" _
             & EmailText & "' , '" & EmailActivityInformation & "','', '" & IPAddress & "','" & BrowserName & "')"

            Dim cmd As SqlCommand = New SqlCommand
            Dim conn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("DkmOnlineConnectionStringDKMErrorLog").ConnectionString)
            'Dim trans As SqlTransaction = conn.BeginTransaction("BuilderTransaction")
            Try
                OldNewConn.PrepareCommand(cmd, conn, CommandType.Text, commendText, Nothing)
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

    Public Function SendEmail(recepient As String, subject As String, body As String, cc As String, attachement As String, string1 As String, _
 string2 As String, password As String) As Boolean
        Dim flag As Boolean = False

        'Replace this with your own Gmail address
        Dim from As String = AppSetting(string1)
        Dim subject12 As String = AppSetting(string2)
        ' Replace this with Email address to whom you want to send mail
        Dim [to] As String = recepient

        Dim mail As MailMessage = New MailMessage()

        If (recepient.Contains(";")) Then
            Dim reci As String() = recepient.Split(";"c)

            For Each rc As String In reci
                If rc <> "" Then
                    mail.[To].Add(rc)
                End If

            Next
        Else
            mail.[To].Add(recepient)
        End If

        If Not (String.IsNullOrEmpty(cc)) Then
            mail.CC.Add(cc)
        End If

        mail.From = New MailAddress(from, subject12, Encoding.UTF8)
        mail.Subject = subject
        mail.SubjectEncoding = Encoding.UTF8
        mail.Body = body
        mail.BodyEncoding = Encoding.UTF8
        mail.IsBodyHtml = True
        mail.Priority = MailPriority.Normal
        If Not (String.IsNullOrEmpty(attachement)) Then
            mail.Attachments.Add(New Attachment(attachement))
        End If
        Dim smtpClient As SmtpClient = New SmtpClient("192.168.16.209")

        smtpClient.Credentials = New NetworkCredential(from, AppSetting("password"))

        '     smtpClient.UseDefaultCredentials = False
        smtpClient.Port = 25
        Try
            smtpClient.Send(mail)
            flag = True
        Catch exception As Exception
            Console.Out.WriteLine(exception.Message)
            flag = False
        End Try
        mail.Attachments.Clear()
        Return flag
    End Function

    Public Function AppSetting(key As String) As String
        Return ConfigurationManager.AppSettings(key)
    End Function

    ''' <summary>
    ''' Sql Execution ADO
    ''' </summary>
    ''' <param name="connString"></param>
    ''' <param name="cmdType"></param>
    ''' <param name="cmdText"></param>
    ''' <param name="cmdParms"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function Enter_ErrorLog(ByVal UserID As String, ByVal WebTableID As Integer, ByVal ActivityType As String, ByVal ErrorMessage As String, ByVal IPAddress As String, ByVal BrowserName As String)
        Try
            UserID = Left(Replace(Trim(UserID), "'", ""), 20)
            Dim ConnectionString As String = ""
            Dim SharedCommendText As String = ""

            SharedCommendText = "insert into DKMinfoWayWebsiteErrorLog" & Format(Today, "MMMyyyy") & " values ('" & UserID & "' , '" & WebTableID & "' , '" & Format(Today, "MM/dd/yyyy") & "' ,'" & Format(Now, "hh:mm:ss") & "' ,'" & ActivityType & "', '" & ErrorMessage & "','" _
             & IPAddress & "','" & BrowserName & "')"

            Dim cmd As SqlCommand = New SqlCommand
            Dim conn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("DkmOnlineConnectionStringDKMErrorLog").ConnectionString)
            'Dim trans As SqlTransaction = conn.BeginTransaction("BuilderTransaction")
            Try
                OldNewConn.PrepareCommand(cmd, conn, CommandType.Text, SharedCommendText, Nothing)
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

End Class