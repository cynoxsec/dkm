Imports Telerik.Web.UI
Imports DkmOnline.Lib
Imports DkmOnline.Common
Imports System.Data.SqlClient
Imports System.IO
Imports Ionic.Zip
Imports System.Threading

Public Class DownloadBills
    Inherits System.Web.UI.Page

    Dim commendText As String
    Dim constantValuesnew As New ConstantValues

    Dim type As String
    Public ReimbursementTypeID As Integer
    Dim Year As Int32

    Dim OldNewConn As OldNewConnection = New OldNewConnection()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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
            If Not IsNothing(Session("Emp_Code")) And Not IsNothing(Session("WebTableID")) And Not IsNothing(Session("MenuID")) Then
                Session("ReimbursetableTravel") = Nothing
                Session("ReimbursetableMultiple") = Nothing
                Session("TempDict") = Nothing
                Session("UploadedFile") = Nothing
                If Not (IsPostBack) Then
                    radGrid1.Rebind()

                    Dim dt As New DataTable

                    dt = OldNewConn.GetDataTable2("Select distinct ReimburseType_Code,ISNULL(PSS.Print_Name,R1.Name) as 'PrintName' from ReimburseMentTypeMaster" & Session("WebTableID") & " R1 Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =R1.Name  where isinactive=0")

                    ddlreimtype.DataTextField = "PrintName"
                    ddlreimtype.DataValueField = "ReimburseType_Code"
                    ddlreimtype.DataSource = dt
                    ddlreimtype.DataBind()
                    ddlreimtype.Items.Add(New ListItem("Please Select", "0"))
                    ddlreimtype.SelectedValue = 0
                End If
            Else
                Response.Redirect("../Logout.aspx")
            End If

        End If
    End Sub

    Protected Sub ddlViewStatus_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlViewStatus.SelectedIndexChanged
        ddlreimtype.SelectedValue = 0

        If (ddlViewStatus.SelectedItem.Value = 1) Then
            'radFromDate.MinDate = DateTime.Now.Year & "-" & DateTime.Now.Month.ToString("00") & "-01"
            'radFromDate.MaxDate = DateTime.Now.Date
            'radToDate.MinDate = DateTime.Now.Year & "-" & DateTime.Now.Month.ToString("00") & "-01"
            'radToDate.MaxDate = DateTime.Now.Date
        ElseIf (ddlViewStatus.SelectedItem.Value = 2) Then
            'Dim CurrentMonthFirstDate As DateTime = System.DateTime.Now.Year & " - " & System.DateTime.Now.Month & " - 01"
            'radFromDate.MaxDate = CurrentMonthFirstDate.AddDays(-1).Date
            'radToDate.MaxDate = CurrentMonthFirstDate.AddDays(-1).Date
        End If

        radGrid1.Rebind()
    End Sub

    Protected Sub radGrid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles radGrid1.NeedDataSource

        Dim sql_Query As String = ""
        Dim dsReimburseDetails As New DataTable

        Dim FromDate As New DateTime()
        Dim ToDate As New DateTime()

        Reimbursement_Card.Text = "Bill Entry Record"

        FromDate = DateTime.Now.Year & "-" & DateTime.Now.Month.ToString("00") & "-01"
        ToDate = DateTime.Now.Year & "-" & DateTime.Now.Month.ToString("00") & "-" & DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month.ToString("00"))

        If (ddlViewStatus.SelectedItem.Value = 1) Then
            tr1.Visible = True
            sql_Query = "select ReimburseDetailsID,ROW_Number() OVER (ORDER BY ReimburseDetailsID DESC) AS 'Sl.No',RD.Emp_code,EM.FirstName,IsNull(PSS.Print_Name,RTM.Name) as 'Field_Name',convert(nvarchar,EntryDate,106) as 'EntryDate',case when ClaimAmount=0 then '0' else ClaimAmount End as 'BillAmount' from reimbursementdetails" & Session("WebTableID") & " RD inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RTM.ReimburseType_Code=RD.ReimburseType_Code Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RTM.Name left join EmployeesMaster" & Session("WebTableID") & " EM on EM.Emp_Code=RD.Emp_Code where EntryDate Between '" & Format(FromDate, "MM/dd/yyyy") & "' and '" & Format(ToDate, "MM/dd/yyyy") & "' order by ReimburseDetailsID desc"
        ElseIf (ddlViewStatus.SelectedItem.Value = 2) Then
            tr1.Visible = True
            sql_Query = "select ReimburseDetailsID,ROW_Number() OVER (ORDER BY ReimburseDetailsID DESC) AS 'Sl.No',RD.Emp_code,EM.FirstName,IsNull(PSS.Print_Name,RTM.Name) as 'Field_Name',convert(nvarchar,EntryDate,106) as 'EntryDate',case when ClaimAmount=0 then '0' else ClaimAmount End as 'BillAmount' from reimbursementdetails" & Session("WebTableID") & " RD inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RTM.ReimburseType_Code=RD.ReimburseType_Code Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RTM.Name left join EmployeesMaster" & Session("WebTableID") & " EM on EM.Emp_Code=RD.Emp_Code where EntryDate < '" & Format(FromDate, "MM/dd/yyyy") & "' order by ReimburseDetailsID desc"
        End If

        dsReimburseDetails = OldNewConn.GetDataTable2(sql_Query)
        radGrid1.Visible = True
        radGrid1.DataSource = dsReimburseDetails

    End Sub

    Protected Sub radGrid1_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles radGrid1.ItemDataBound
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

    Protected Sub radGrid1_DeleteCommand(sender As Object, e As GridCommandEventArgs) Handles radGrid1.DeleteCommand
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

    Protected Sub btndownload_Click(sender As Object, e As EventArgs) Handles btndownload.Click

        Dim sql_Query As String = ""
        Dim dsReimburseDetails As New DataTable

        Dim FromDate As New DateTime()
        Dim ToDate As New DateTime()

        FromDate = DateTime.Now.Year & "-" & DateTime.Now.Month.ToString("00") & "-01"
        ToDate = DateTime.Now.Year & "-" & DateTime.Now.Month.ToString("00") & "-" & DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month.ToString("00"))


        If Not IsNothing(radFromDate.SelectedDate) And Not IsNothing(radToDate.SelectedDate) Then
            FromDate = radFromDate.SelectedDate.Value
            ToDate = radToDate.SelectedDate.Value
        End If

        If (ddlViewStatus.SelectedItem.Value = 1) Then
            sql_Query = "select ReimburseDetailsID,IsNull(PSS.Print_Name,RTM.Name) as 'Field_Name',convert(nvarchar,EntryDate,106) as 'EntryDate',case when ClaimAmount=0 then '0' else ClaimAmount End as 'BillAmount' from reimbursementdetails" & Session("WebTableID") & " RD inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RTM.ReimburseType_Code=" & ddlreimtype.SelectedValue.ToString & " And RD.ReimburseType_Code=" & ddlreimtype.SelectedValue.ToString & " Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RTM.Name where EntryDate Between '" & Format(FromDate, "MM/dd/yyyy") & "' and '" & Format(ToDate, "MM/dd/yyyy") & "' order by ReimburseDetailsID desc"
        ElseIf (ddlViewStatus.SelectedItem.Value = 2) Then
            sql_Query = "select ReimburseDetailsID,IsNull(PSS.Print_Name,RTM.Name) as 'Field_Name',convert(nvarchar,EntryDate,106) as 'EntryDate',case when ClaimAmount=0 then '0' else ClaimAmount End as 'BillAmount' from reimbursementdetails" & Session("WebTableID") & " RD inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RTM.ReimburseType_Code=" & ddlreimtype.SelectedValue.ToString & " And RD.ReimburseType_Code=" & ddlreimtype.SelectedValue.ToString & " Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RTM.Name where EntryDate Between '" & Format(FromDate, "MM/dd/yyyy") & "' and '" & Format(ToDate, "MM/dd/yyyy") & "' order by ReimburseDetailsID desc"
        End If

        dsReimburseDetails = OldNewConn.GetDataTable2(sql_Query)
        If (dsReimburseDetails.Rows.Count > 0) Then
            DownloadReimbursementProofAll(dsReimburseDetails)
        Else
            Response.Write("<script> alert ('There are no bills regarding " & ddlreimtype.SelectedItem.Text & ".')</script>")
        End If

    End Sub

    Public Sub DownloadReimbursementProofAll(ByVal dsReimburseDetails As DataTable)
        Using zip As New ZipFile()
            zip.AlternateEncodingUsage = ZipOption.AsNecessary
            zip.AddDirectoryByName("Files")

            For i As Int32 = 0 To dsReimburseDetails.Rows.Count - 1
                Dim Str As String = "Select *  from ReimbursementProofUpload" & Session("WebTableID") & " where ReimburseDetailsID=" & dsReimburseDetails.Rows(i)("ReimburseDetailsID").ToString() & ""
                Dim UploadedDocumentDetails As DataTable = OldNewConn.GetDataTable2(Str)
                Try
                    Dim path As String = ""
                    If (UploadedDocumentDetails.Rows.Count > 0) Then
                        For ReimiD As Integer = 0 To UploadedDocumentDetails.Rows.Count - 1
                            Thread.Sleep(1000)
                            Dim extraFileName As String = UploadedDocumentDetails.Rows(ReimiD)("Emp_code").ToString().Replace("/", "_")
                            If Not String.IsNullOrEmpty(UploadedDocumentDetails.Rows(ReimiD)("fileData").ToString) Then
                                extraFileName = extraFileName & "_" & dsReimburseDetails.Rows(i)("Field_Name") & "_" & dsReimburseDetails.Rows(i)("EntryDate") & "_" & (Now.TimeOfDay.Hours * 42) & "-" & CInt(Now.TimeOfDay.Minutes + 4) & "-" & CInt(Now.TimeOfDay.Seconds + 4) & "-" & Today.Date.Day & "-" & Format(Today.Date.Date, "ddmmyy")
                                Dim FileName As String = HttpContext.Current.Request.PhysicalApplicationPath & "OutPutFiles\" & extraFileName.ToString().Replace("/", "_") & "_" & ReimiD & "" & "" & UploadedDocumentDetails.Rows(ReimiD)("Ext").ToString() & ""
                                Dim bytes As Byte()

                                bytes = DirectCast(UploadedDocumentDetails.Rows(ReimiD)("fileData"), Byte())
                                Dim newFile As FileStream = New FileStream(FileName, FileMode.Create)
                                newFile.Write(bytes, 0, bytes.Length)
                                newFile.Close()
                                zip.AddFile(FileName, "Files")
                            End If
                            If Session("WebTableID").ToString = "497" Then
                                If Not String.IsNullOrEmpty(UploadedDocumentDetails.Rows(ReimiD)("broadbandfileData").ToString) Then
                                    extraFileName = extraFileName & "_" & dsReimburseDetails.Rows(i)("Field_Name") & "_" & dsReimburseDetails.Rows(i)("EntryDate") & "_" & (Now.TimeOfDay.Hours * 42) & "-" & CInt(Now.TimeOfDay.Minutes + 4) & "-" & CInt(Now.TimeOfDay.Seconds + 4) & "-" & Today.Date.Day & "-" & Format(Today.Date.Date, "ddmmyy")
                                    Dim FileName As String = HttpContext.Current.Request.PhysicalApplicationPath & "OutPutFiles\Tele_" & extraFileName.ToString().Replace("/", "_") & "_" & ReimiD & "" & "" & UploadedDocumentDetails.Rows(ReimiD)("Ext2").ToString() & ""
                                    Dim bytes As Byte()

                                    bytes = DirectCast(UploadedDocumentDetails.Rows(ReimiD)("broadbandfileData"), Byte())
                                    Dim newFile As FileStream = New FileStream(FileName, FileMode.Create)
                                    newFile.Write(bytes, 0, bytes.Length)
                                    newFile.Close()
                                    zip.AddFile(FileName, "Files")
                                End If
                            End If
                        
                        Next
                    End If
                Catch ex As SqlException
                End Try
            Next
            Response.Clear()
            Response.BufferOutput = False
            Dim zipName As String = [String].Format("Zip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"))
            Response.ContentType = "application/zip"
            Response.AddHeader("content-disposition", "attachment; filename=" + zipName)
            zip.Save(Response.OutputStream)
            Response.End()
        End Using
    End Sub

    Public Function whetherlinkvisiblebydate() As Boolean

        Dim strfbplink As String = "Select name, print_name from payslipsetup" & Session("WebTableID") & " where name in ('OpeningDateReim','ClosingDateReim') "

        Dim dtfbplink As DataTable
        dtfbplink = OldNewConn.GetDataTable2(strfbplink)

        If (dtfbplink.Rows.Count > 0) Then
            If (DateTime.Now.Day >= Convert.ToInt32(dtfbplink.Rows(0)("print_name")) And DateTime.Now.Day <= Convert.ToInt32(dtfbplink.Rows(1)("print_name"))) Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

End Class