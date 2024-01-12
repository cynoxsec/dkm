Imports Telerik.Web.UI
Imports DkmOnline.Lib
Imports DkmOnline.Common
Imports System.Data.SqlClient
Imports System.IO
Imports Ionic.Zip
Imports System.Threading


Public Class ReimEditDetails
    Inherits System.Web.UI.Page

    Dim commendText As String
    Dim constantValuesnew As New ConstantValues

    Dim type As String
    Dim FromDate As Date
    Dim ToDate As Date
    Dim Year As Int32
    Dim dtOption As New DataTable

    Public ReimbursementTypeID As Integer

    Dim OldNewConn As OldNewConnection = New OldNewConnection()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If IsNothing(Session("Emp_Code")) Then
            Response.Redirect("../Logout.aspx")
        End If

        DatabaseFacade.Instance.Enter_SiteLog(Session("Emp_Code"), Session("LoginType"), Session("LoginAccessType"), Session("Comp_Code"), 0, Request.Url.AbsolutePath, Request.Url.AbsolutePath, Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type, Session("WebTableID"), Session.SessionID, Session("MenuID"))

        OldNewConn.CheckDatabaseOldNew(Session("WebTableID"))

        dtOption = OldNewConn.GetDataTable2("Select Print_Name from PayslipSetup" & Session("WebTableID") & " where Name ='ReimbursementOption'")

        Dim dtYear As DataTable = OldNewConn.GetDataTable2("select * from dbo.ReimYearMaster" & Session("WebTableID") & "")

        If (dtYear.Rows.Count > 0) Then
            Year = dtYear.Rows(0)("Year")
        End If

        If Not IsNothing(Request.QueryString("Type")) Then
            Dim s As String = Request.QueryString("Type").Replace(" ", "+")

            If s.Length Mod 4 > 0 Then
                s = s.PadRight(s.Length + 4 - s.Length Mod 4, "="c)
            End If

            type = EncryDecrypt.Decrypt(s, "a")

            s = Request.QueryString("IDD").Replace(" ", "+")
            If s.Length Mod 4 > 0 Then
                s = s.PadRight(s.Length + 4 - s.Length Mod 4, "="c)
            End If


            ReimbursementTypeID = EncryDecrypt.Decrypt(s, "a")
        End If

        Dim reim_Policy As DataTable = OldNewConn.GetDataTable2("select BillValidStartDate,BillValidEndDate,isMultipleClaimDetails,isPersonsDetailRequired from ReimburseMentTypeMaster" & Session("WebTableID") & " where Name = '" & type & "'")

        FromDate = reim_Policy.Rows(0)("BillValidStartDate")
        ToDate = reim_Policy.Rows(0)("BillValidEndDate")

        If Not (IsPostBack) Then
            If Not IsNothing(Session("Emp_Code")) And Not IsNothing(Session("WebTableID")) And Not IsNothing(Session("MenuID")) Then
                Session("ReimbursetableTravel") = Nothing
                Session("ReimbursetableMultiple") = Nothing
                Session("TempDict") = Nothing
                Session("UploadedFile") = Nothing

                Dim dtCSVwithOutSubClaimID As DataTable = OldNewConn.GetDataTable2("Select name, print_name from payslipsetup" & Session("WebTableID") & " where name in ('CSVwithSubClaimID')")

                If (dtCSVwithOutSubClaimID.Rows.Count > 0) Then
                    If (dtCSVwithOutSubClaimID.Rows(0)("print_name").ToString.ToLower = "n") Then
                        ViewState("WithOutSubClaimID") = "CSVwithOutSubClaimID"
                    End If
                End If

                If (ViewState("WithOutSubClaimID") = "CSVwithOutSubClaimID") Then
                Else
                    If reim_Policy.Rows.Count > 0 Then

                        'If Session("WebTableId").ToString = "805" Then
                        If (type.ToString.ToLower.Contains("lta")) Then
                            If reim_Policy.Rows(0)("isPersonsDetailRequired").ToString = "False" Then
                                ViewState("WithOutSubClaimID") = "CSVwithOutSubClaimID"
                                radGrid1.MasterTableView.GetColumn("Claimstatus").Display = True
                                radGrid1.MasterTableView.GetColumn("Approvedamount").Display = True
                                radGrid1.MasterTableView.GetColumn("Remarks").Display = True
                            Else
                                radGrid1.MasterTableView.GetColumn("Claimstatus").Display = False
                                radGrid1.MasterTableView.GetColumn("Approvedamount").Display = False
                                radGrid1.MasterTableView.GetColumn("Remarks").Display = False
                            End If
                        Else
                            If reim_Policy.Rows(0)("isMultipleClaimDetails").ToString = "False" Then
                                ViewState("WithOutSubClaimID") = "CSVwithOutSubClaimID"
                                radGrid1.MasterTableView.GetColumn("Claimstatus").Display = True
                                radGrid1.MasterTableView.GetColumn("Approvedamount").Display = True
                                radGrid1.MasterTableView.GetColumn("Remarks").Display = True
                            Else
                                radGrid1.MasterTableView.GetColumn("Claimstatus").Display = False
                                radGrid1.MasterTableView.GetColumn("Approvedamount").Display = False
                                radGrid1.MasterTableView.GetColumn("Remarks").Display = False
                            End If
                        End If
                        'Else
                        '    If reim_Policy.Rows(0)("isMultipleClaimDetails").ToString = "False" Then
                        '        ViewState("WithOutSubClaimID") = "CSVwithOutSubClaimID"
                        '        radGrid1.MasterTableView.GetColumn("Claimstatus").Display = True
                        '        radGrid1.MasterTableView.GetColumn("Approvedamount").Display = True
                        '        radGrid1.MasterTableView.GetColumn("Remarks").Display = True
                        '    Else
                        '        radGrid1.MasterTableView.GetColumn("Claimstatus").Display = False
                        '        radGrid1.MasterTableView.GetColumn("Approvedamount").Display = False
                        '        radGrid1.MasterTableView.GetColumn("Remarks").Display = False
                        '    End If
                        'End If

                    Else
                        radGrid1.MasterTableView.GetColumn("Claimstatus").Display = False
                        radGrid1.MasterTableView.GetColumn("Approvedamount").Display = False
                        radGrid1.MasterTableView.GetColumn("Remarks").Display = False
                    End If

                End If
            Else
                Response.Redirect("../Logout.aspx")
            End If
        End If
    End Sub

    Protected Sub radGrid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles radGrid1.NeedDataSource

        Dim sql_Query As String = ""
        Dim dsReimburseDetails As New DataTable

        Reimbursement_Card.Text = "Bill Entry Record for year " & Year & "-" & Year + 1 & " (" & Session("Emp_code") & ")"

        If (ViewState("WithOutSubClaimID") = "CSVwithOutSubClaimID") Then
            sql_Query = "select ReimburseDetailsID,ROW_Number() OVER (ORDER BY ReimburseDetailsID DESC) AS 'Sl.No',IsNull(PSS.Print_Name,RTM.Name) as 'Field_Name',convert(nvarchar,EntryDate,106) as 'EntryDate',case when ClaimAmount=0 And RD.ExtraField3 = 0 then RD.ExtraField5 when ClaimAmount=0 then RD.ExtraField2 else ClaimAmount End as 'BillAmount',"
            sql_Query += "(Select top 1 Status from ReimbursementProofrejection" & Session("WebTableID") & "_" & Year & " where ClaimID=RD.ReimburseDetailsID order by UploadedDate desc) as 'Claimstatus',(Select top 1 ApprovedAmount from ReimbursementProofrejection" & Session("WebTableID") & "_" & Year & " where ClaimID=RD.ReimburseDetailsID order by UploadedDate desc) as 'ApprovedAmount',(Select top 1 Remarks from ReimbursementProofrejection" & Session("WebTableID") & "_" & Year & " where ClaimID=RD.ReimburseDetailsID order by UploadedDate desc) as 'Remarks' "
            sql_Query += " from reimbursementdetails" & Session("WebTableID") & " RD inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RTM.ReimburseType_Code=RD.ReimburseType_Code Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RTM.Name where RD.emp_Code = '" & Session("Emp_code") & "' and EntryDate Between '" & FromDate.ToString("MM-dd-yyyy") & "' and '" & ToDate.ToString("MM/dd/yyyy") & "' And RTM.Name = '" & type & "' order by ReimburseDetailsID desc"
        Else

            Dim dtLastDateofrecord As DataTable = OldNewConn.GetDataTable2("Select name, print_name from payslipsetup" & Session("WebTableID") & " where name in ('LastDateofrecord') ")

            If (dtLastDateofrecord.Rows.Count > 0) Then
                radGrid2.Visible = True
                div1.Visible = True
                Dim lastdateofrecord As DateTime = DateTime.ParseExact(dtLastDateofrecord.Rows(0)("print_name"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                sql_Query = "select ReimburseDetailsID,ROW_Number() OVER (ORDER BY ReimburseDetailsID DESC) AS 'Sl.No',IsNull(PSS.Print_Name,RTM.Name) as 'Field_Name',convert(nvarchar,EntryDate,106) as 'EntryDate',case when ClaimAmount=0 And RD.ExtraField3 = 0 then RD.ExtraField5 when ClaimAmount=0 then RD.ExtraField2 else ClaimAmount End as 'BillAmount','' as 'Claimstatus','0' as 'ApprovedAmount','' as 'Remarks'"
                sql_Query += " from reimbursementdetails" & Session("WebTableID") & " RD inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RTM.ReimburseType_Code=RD.ReimburseType_Code Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RTM.Name where RD.emp_Code = '" & Session("Emp_code") & "' and EntryDate >= '" & lastdateofrecord.ToString("MM-dd-yyyy") & "' and EntryDate <='" & ToDate.ToString("MM-dd-yyyy") & "' And RTM.Name = '" & type & "' order by ReimburseDetailsID desc"
            Else
                sql_Query = "select ReimburseDetailsID,ROW_Number() OVER (ORDER BY ReimburseDetailsID DESC) AS 'Sl.No',IsNull(PSS.Print_Name,RTM.Name) as 'Field_Name',convert(nvarchar,EntryDate,106) as 'EntryDate',case when ClaimAmount=0 And RD.ExtraField3 = 0 then RD.ExtraField5 when ClaimAmount=0 then RD.ExtraField2 else ClaimAmount End as 'BillAmount','' as 'Claimstatus','0' as 'ApprovedAmount','' as 'Remarks'"
                sql_Query += " from reimbursementdetails" & Session("WebTableID") & " RD inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RTM.ReimburseType_Code=RD.ReimburseType_Code Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RTM.Name where RD.emp_Code = '" & Session("Emp_code") & "' and EntryDate Between '" & FromDate.ToString("MM-dd-yyyy") & "' and '" & ToDate.ToString("MM-dd-yyyy") & "' And RTM.Name = '" & type & "' order by ReimburseDetailsID desc"
            End If
        End If
        dsReimburseDetails = OldNewConn.GetDataTable2(sql_Query)
        radGrid1.DataSource = dsReimburseDetails
    End Sub

    Protected Sub radGrid1_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles radGrid1.ItemDataBound

        If TypeOf e.Item Is GridDataItem AndAlso e.Item.OwnerTableView.Name = "Parent" Then
            Dim griditem As GridDataItem = TryCast(e.Item, GridDataItem)

            If Not IsNothing(griditem) Then
                Dim lnkForViewReport2 As LinkButton = DirectCast(griditem("ViewReport2").FindControl("lnkForViewReport2"), LinkButton)
                Dim image As ImageButton = DirectCast(griditem("Edit").Controls(0), ImageButton)
                Dim image2 As ImageButton = DirectCast(griditem("Delete").Controls(0), ImageButton)
                Dim Claimstatus As String = griditem("Claimstatus").Text

                If Not IsNothing(image) Then

                    Dim ReimbursementID As String = griditem.GetDataKeyValue("ReimburseDetailsID")

                    Dim str As String = "Select EntryDate,RM.Name from reimbursementdetails" & Session("WebtableID") & " R inner join ReimburseMentTypeMaster" & Session("WebtableID") & " RM on R.ReimburseType_Code=RM.ReimburseType_Code where reimbursedetailsid=" & ReimbursementID & ""

                    Dim dt12 As DataTable

                    dt12 = OldNewConn.GetDataTable2(str)



                    If (dt12.Rows.Count > 0) Then
                        If Not IsDBNull(dt12.Rows(0)("EntryDate")) Then
                            Dim entryDate As DateTime = Convert.ToDateTime(dt12.Rows(0)("EntryDate"))

                            Dim variablebydate As Boolean = False
                            variablebydate = whetherlinkvisiblebydate()

                            Dim dtEmployeeDeclarationRequiredForInvestment As DataTable = OldNewConn.GetDataTable2("Select print_Name from payslipsetup" & Session("WebTableID") & " where Name ='isEmployeeDeclarationRequiredForInvestment'")

                            If (dtEmployeeDeclarationRequiredForInvestment.Rows.Count > 0) Then
                                If (dtEmployeeDeclarationRequiredForInvestment.Rows(0)("Print_Name").ToString.ToLower = "yes") Then

                                    Dim dtemployeeonlinedeclaration As DataTable = OldNewConn.GetDataTable2("Select * from employeeonlinedeclaration" & Session("WebTableID") & "  where Emp_Code='" & Session("Emp_Code") & "'")

                                    If (dtOption.Rows.Count > 0) Then
                                        If (dtOption.Rows(0)("Print_Name").ToString.ToLower = "withfbp" Or dtOption.Rows(0)("Print_Name").ToString.ToLower = "withoutbudget") Then
                                            If (dtemployeeonlinedeclaration.Rows.Count > 0) Then
                                                If (dtemployeeonlinedeclaration.Rows(0)("isLeavingEmployee") = True) Then
                                                    variablebydate = True
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If


                            If (Claimstatus <> "&nbsp;" And Claimstatus <> "") Then
                                image.Visible = False
                                image2.Visible = False
                            ElseIf Not (entryDate.Month = DateTime.Now.Month And entryDate.Year = DateTime.Now.Year) Then
                                image.Visible = False
                                image2.Visible = False
                            ElseIf (variablebydate) Then
                                Dim ID As String = griditem.GetDataKeyValue("ReimburseDetailsID") & ",119"
                                ID = EncryDecrypt.Encrypt(ID, "EncryptString01")

                                If (dt12.Rows(0)("Name").ToString.ToLower.Contains("deemed lta")) Then
                                    image.Attributes.Add("onclick", String.Format("return openRadWindow('{0}');", "Reimbursementdeemedlta.aspx?ID=" & ID & "&Needstoclearsession=an"))
                                ElseIf (dt12.Rows(0)("Name").ToString.ToLower.Contains("taxable lta")) Then
                                    image.Attributes.Add("onclick", String.Format("return openRadWindow('{0}');", "ReimbursementOthers.aspx?ID=" & ID & "&Needstoclearsession=an"))
                                ElseIf (dt12.Rows(0)("Name").ToString.ToLower.Contains("lta")) Then
                                    image.Attributes.Add("onclick", String.Format("return openRadWindow('{0}');", "ReimbursementLTA.aspx?ID=" & ID & "&Needstoclearsession=an"))
                                Else
                                    image.Attributes.Add("onclick", String.Format("return openRadWindow('{0}');", "ReimbursementOthers.aspx?ID=" & ID & "&Needstoclearsession=an"))
                                End If
                            Else
                                image.Visible = False
                                image2.Visible = False
                            End If
                        End If
                    End If

                    str = "Select count(emp_code) as TotalBill from ReimbursementProofUpload" & Session("WebtableID") & " where reimbursedetailsid=" & ReimbursementID & ""

                    Dim dt122 As New DataTable

                    dt122 = OldNewConn.GetDataTable2(str)

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
        End If

        If TypeOf e.Item Is GridDataItem AndAlso e.Item.OwnerTableView.Name = "ChildGrid" Then

            Dim Childitem As GridDataItem = TryCast(e.Item, GridDataItem)
            Dim Claimstatus As String = Childitem("Claimstatus").Text
            Dim parentItem As GridDataItem = TryCast(Childitem.OwnerTableView.ParentItem, GridDataItem)

            Dim image As ImageButton = DirectCast(parentItem("Edit").Controls(0), ImageButton)
            Dim image2 As ImageButton = DirectCast(parentItem("Delete").Controls(0), ImageButton)

            If (Claimstatus <> "&nbsp;" And Claimstatus <> "") Then
                image.Visible = False
                image2.Visible = False
            End If
        End If

    End Sub

    Protected Sub radGrid1_DeleteCommand(sender As Object, e As GridCommandEventArgs) Handles radGrid1.DeleteCommand
        Dim griditem As GridDataItem = TryCast(e.Item, GridDataItem)

        If Not IsNothing(griditem) Then
            Dim image As ImageButton = DirectCast(griditem("Delete").Controls(0), ImageButton)
            If Not IsNothing(image) Then
                Dim ReimbursementID As String = griditem.GetDataKeyValue("ReimburseDetailsID")

                Dim dt12 As DataTable = OldNewConn.GetDataTable2("Select * from reimbursementdetails" & Session("WebtableID") & " where reimbursedetailsid=" & ReimbursementID & "")

                If (dt12.Rows.Count > 0) Then
                    If Not IsDBNull(dt12.Rows(0)("EntryDate")) Then
                        Dim entryDate As DateTime = Convert.ToDateTime(dt12.Rows(0)("EntryDate"))
                        If Not (entryDate.Month = DateTime.Now.Month And entryDate.Year = DateTime.Now.Year) Then
                            Response.Write("<script>alert('Previous month\'s claims can not be deleted.');</script>")
                            Exit Sub
                        End If
                    End If
                End If

                Dim str As String = "insert into dbo.DeletedReimbursedetails" & Session("WebTableID") & " (ReimburseDetailsID,Emp_Code,ReimburseType_Code,TransactionDate,TransactionDetail,ClaimAmount,ExtraField1,ExtraField2,ExtraField3,ExtraField4,ExtraField5,ExtraField6,ExtraField7,ExtraField8,TotalBillField,Rejected,BillPassed,ExtraDateField,EntryDate,DeletedBy,DeletedDate) "
                str += " select ReimburseDetailsID,Emp_Code,ReimburseType_Code,TransactionDate,TransactionDetail,ClaimAmount,ExtraField1,ExtraField2,ExtraField3,ExtraField4,ExtraField5,ExtraField6,ExtraField7,ExtraField8,TotalBillField,Rejected,BillPassed,ExtraDateField,EntryDate,'" & Session("Emp_code").ToString & "',Getdate() from reimbursementdetails" & Session("WebtableID") & " where reimbursedetailsid=" & ReimbursementID & ""
                OldNewConn.ExecuteNonQuery(CommandType.Text, str, Nothing)

                commendText = "delete from reimbursementdetails" & Session("WebtableID") & "  where Emp_Code='" & Session("Emp_Code") & "' and ReimburseDetailsId='" & ReimbursementID & "'"
                OldNewConn.ExecuteNonQuery(CommandType.Text, commendText, Nothing)

                commendText = String.Format("delete from OtherMultipleReimburseClaimDetails{0}  where Emp_Code='" & Session("Emp_Code") & "' and ReimburseDetailsId='" & ReimbursementID & "'", HttpContext.Current.Session("WebTableID"))
                OldNewConn.ExecuteNonQuery(CommandType.Text, commendText, Nothing)

                commendText = String.Format("delete from ReimbursementProofUpload{0}  where Emp_Code='" & Session("Emp_Code") & "' and ReimburseDetailsId='" & ReimbursementID & "'", HttpContext.Current.Session("WebTableID"))
                OldNewConn.ExecuteNonQuery(CommandType.Text, commendText, Nothing)
            End If
        End If
    End Sub

    Protected Sub lnkForViewReport2_Command(sender As Object, e As CommandEventArgs)
        Dim ReimburseDetailsID As String = e.CommandArgument.ToString()
        DownloadReimbursementProof(ReimburseDetailsID)
    End Sub

    Public Sub DownloadReimbursementProof(ByVal ReimburseDetailsID As String)

        Dim ReimbursementType_Code As String = ""
        Dim comm As String = "Select *  from Reimbursementdetails" & Session("WebTableID") & " where ReimburseDetailsID=" & ReimburseDetailsID & ""
        Dim dttable As DataTable = OldNewConn.GetDataTable2(comm)
        If dttable.Rows.Count > 0 Then
            ReimbursementType_Code = dttable.Rows(0)("ReimburseType_Code").ToString
        End If


        If (Session("WebTableID").ToString = "805" Or Session("WebTableID").ToString = "11097" Or Session("WebTableID").ToString = "497") And ReimbursementType_Code = "2" Then
            Using zip As New ZipFile()
                zip.AlternateEncodingUsage = ZipOption.AsNecessary
                zip.AddDirectoryByName("Files")

                Try
                    Dim Str As String = "Select *  from ReimbursementProofUpload" & Session("WebTableID") & " where ReimburseDetailsID=" & ReimburseDetailsID & ""
                    Dim UploadedDocumentDetails As DataTable = OldNewConn.GetDataTable2(Str)
                    Dim path As String = ""
                    If (UploadedDocumentDetails.Rows.Count > 0) Then
                        For i As Integer = 0 To UploadedDocumentDetails.Rows.Count - 1
                            Thread.Sleep(1000)
                            Dim extraFileName As String = UploadedDocumentDetails.Rows(i)("Emp_code").ToString().Replace("/", "_")

                            If Not String.IsNullOrEmpty(UploadedDocumentDetails.Rows(i)("Ext").ToString) Then
                                extraFileName = extraFileName & "wl" & (Now.TimeOfDay.Hours * 42) & "-" & CInt(Now.TimeOfDay.Minutes + 4) & "-" & CInt(Now.TimeOfDay.Seconds + 4) & "-" & Today.Date.Day & "-" & Format(Today.Date.Date, "ddmmyy")
                                Dim FileName As String = HttpContext.Current.Request.PhysicalApplicationPath & "OutPutFiles\" & "" & HttpContext.Current.Session("WebTableID") & "Letter" & extraFileName & "" & UploadedDocumentDetails.Rows(i)("Ext").ToString() & ""
                                Dim bytes As Byte()
                                bytes = DirectCast(UploadedDocumentDetails.Rows(i)("fileData"), Byte())

                                Dim newFile As FileStream = New FileStream(FileName, FileMode.Create)
                                newFile.Write(bytes, 0, bytes.Length)
                                newFile.Close()
                                zip.AddFile(FileName, "Files")
                            End If
                            If Not String.IsNullOrEmpty(UploadedDocumentDetails.Rows(i)("Ext2").ToString) Then
                                extraFileName = extraFileName & "wl" & (Now.TimeOfDay.Hours * 42) & "-" & CInt(Now.TimeOfDay.Minutes + 4) & "-" & CInt(Now.TimeOfDay.Seconds + 4) & "-" & Today.Date.Day & "-" & Format(Today.Date.Date, "ddmmyy")
                                Dim FileName As String = HttpContext.Current.Request.PhysicalApplicationPath & "OutPutFiles\" & "" & HttpContext.Current.Session("WebTableID") & "Letter" & extraFileName & "" & UploadedDocumentDetails.Rows(i)("Ext2").ToString() & ""
                                Dim bytes As Byte()
                                bytes = DirectCast(UploadedDocumentDetails.Rows(i)("broadbandfileData"), Byte())

                                Dim newFile As FileStream = New FileStream(FileName, FileMode.Create)
                                newFile.Write(bytes, 0, bytes.Length)
                                newFile.Close()
                                zip.AddFile(FileName, "Files")
                            End If
                        Next
                    End If
                Catch ex As SqlException
                End Try
                Response.Clear()
                Response.BufferOutput = False
                Dim zipName As String = [String].Format("Zip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"))
                Response.ContentType = "application/zip"
                Response.AddHeader("content-disposition", "attachment; filename=" + zipName)
                zip.Save(Response.OutputStream)
                Response.End()
            End Using


        Else
            Using zip As New ZipFile()
                zip.AlternateEncodingUsage = ZipOption.AsNecessary
                zip.AddDirectoryByName("Files")

                Try
                    Dim Str As String = "Select *  from ReimbursementProofUpload" & Session("WebTableID") & " where ReimburseDetailsID=" & ReimburseDetailsID & ""
                    Dim UploadedDocumentDetails As DataTable = OldNewConn.GetDataTable2(Str)
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
                Dim zipName As String = [String].Format("Zip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"))
                Response.ContentType = "application/zip"
                Response.AddHeader("content-disposition", "attachment; filename=" + zipName)
                zip.Save(Response.OutputStream)
                Response.End()
            End Using
        End If


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

    Protected Sub radGrid1_DetailTableDataBind(sender As Object, e As GridDetailTableDataBindEventArgs) Handles radGrid1.DetailTableDataBind
        Dim dataItem As GridDataItem = TryCast(e.DetailTableView.ParentItem, GridDataItem)

        Dim Field_Name As String = dataItem("Field_Name").Text

        If (Field_Name.ToString.ToLower.Contains("lta")) Then
            Dim dttable As DataTable = OldNewConn.GetDataTable2("select ReimburseDetailsID,ReimbursementPersonsDetailID as MultipleClaimDetailsID,'' as BillNumber,'' as BillDated,ClaimAmount as BillAmount,(Select top 1 Status from ReimbursementProofrejection" & Session("WebTableID") & "_" & Year & " where SubClaimID=OC.ReimbursementPersonsDetailID and ClaimID=OC.ReimburseDetailsId order by UploadedDate desc) as 'Claimstatus',(Select top 1 ApprovedAmount from ReimbursementProofrejection" & Session("WebTableID") & "_" & Year & " where SubClaimID=OC.ReimbursementPersonsDetailID and ClaimID=OC.ReimburseDetailsId order by UploadedDate desc) as 'ApprovedAmount',(Select top 1 Remarks from ReimbursementProofrejection" & Session("WebTableID") & "_" & Year & " where SubClaimID=OC.ReimbursementPersonsDetailID and ClaimID=OC.ReimburseDetailsId order by UploadedDate desc) as 'Remarks' from OtherReimbursementPersonsDetail" & Session("WebTableID") & " OC where OC.ReimburseDetailsID=" & dataItem.GetDataKeyValue("ReimburseDetailsID") & "")
            e.DetailTableView.DataSource = dttable
            If dttable.Rows.Count > 0 Then
                e.DetailTableView.Visible = True
            Else
                e.DetailTableView.Visible = False
            End If
        Else
            Dim dttable As DataTable = OldNewConn.GetDataTable2("select ReimburseDetailsID,MultipleClaimDetailsID,BillNumber,Convert (varchar,BillDated,106) as BillDated,BillAmount,(Select top 1 Status from ReimbursementProofrejection" & Session("WebTableID") & "_" & Year & " where SubClaimID=OC.MultipleClaimDetailsID  and ClaimID=OC.ReimburseDetailsID order by UploadedDate desc) as 'Claimstatus',(Select top 1 ApprovedAmount from ReimbursementProofrejection" & Session("WebTableID") & "_" & Year & " where SubClaimID=OC.MultipleClaimDetailsID  and ClaimID=OC.ReimburseDetailsID order by UploadedDate desc) as 'ApprovedAmount',(Select top 1 Remarks from ReimbursementProofrejection" & Session("WebTableID") & "_" & Year & " where SubClaimID=OC.MultipleClaimDetailsID and ClaimID=OC.ReimburseDetailsID order by UploadedDate desc) as 'Remarks' from OtherMultipleReimburseClaimDetails" & Session("WebTableID") & " OC where OC.ReimburseDetailsID=" & dataItem.GetDataKeyValue("ReimburseDetailsID") & "")
            e.DetailTableView.DataSource = dttable
            If dttable.Rows.Count > 0 Then
                e.DetailTableView.Visible = True
            Else
                e.DetailTableView.Visible = False
            End If
        End If
    End Sub

    Public Sub HideExpandColumnRecursive(tableView As GridTableView)
        Dim nestedViewItems As GridItem() = tableView.GetItems(GridItemType.NestedView)
        For Each nestedViewItem As GridNestedViewItem In nestedViewItems
            For Each nestedView As GridTableView In nestedViewItem.NestedTableViews
                Dim cell As TableCell = nestedView.ParentItem("ExpandColumn")
                Dim cell1 As TableCell = nestedView.ParentItem("ReimburseDetailsID")

                If (ViewState("WithOutSubClaimID") = "CSVwithOutSubClaimID") Then
                    nestedView.ParentItem.Expanded = False
                    cell.Controls(0).Visible = False
                Else
                    nestedView.ParentItem.Expanded = True
                End If
            Next
        Next
    End Sub

    Protected Sub radGrid1_PreRender(sender As Object, e As EventArgs) Handles radGrid1.PreRender
        If Not Page.IsPostBack Then
            If (radGrid1.MasterTableView.Items.Count > 0) Then
                radGrid1.MasterTableView.Items(0).Expanded = True
                'radGrid1.MasterTableView.Items(0).ChildItem.NestedTableViews(0).Items(0).Expanded = True
                HideExpandColumnRecursive(radGrid1.MasterTableView)
            End If
        End If
    End Sub

    ''' <summary>
    ''' radgrid2
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>

    Protected Sub radGrid2_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles radGrid2.NeedDataSource

        Dim sql_Query As String = ""
        Dim dsReimburseDetails As New DataTable

        Dim dtLastDateofrecord As DataTable = OldNewConn.GetDataTable2("Select name, print_name from payslipsetup" & Session("WebTableID") & " where name in ('LastDateofrecord') ")

        radGrid2.Visible = True
        If (dtLastDateofrecord.Rows.Count > 0) Then
            Literal1.Text = "Claims prior to " & MonthName(CDate(dtLastDateofrecord.Rows(0)("print_name")).Month, True) & "-" & CDate(dtLastDateofrecord.Rows(0)("print_name")).Year

            sql_Query = "select ReimburseDetailsID,ROW_Number() OVER (ORDER BY ReimburseDetailsID DESC) AS 'Sl.No',IsNull(PSS.Print_Name,RTM.Name) as 'Field_Name',convert(nvarchar,EntryDate,106) as 'EntryDate',case when ClaimAmount=0 And RD.ExtraField3 = 0 then RD.ExtraField5 when ClaimAmount=0 then RD.ExtraField2 else ClaimAmount End as 'BillAmount',"
            sql_Query += "(Select top 1 Status from ReimbursementProofrejection" & Session("WebTableID") & "_" & Year & " where ClaimID=RD.ReimburseDetailsID order by UploadedDate desc) as 'Claimstatus',(Select top 1 ApprovedAmount from ReimbursementProofrejection" & Session("WebTableID") & "_" & Year & " where ClaimID=RD.ReimburseDetailsID order by UploadedDate desc) as 'ApprovedAmount',(Select top 1 Remarks from ReimbursementProofrejection" & Session("WebTableID") & "_" & Year & " where ClaimID=RD.ReimburseDetailsID order by UploadedDate desc) as 'Remarks' "
            sql_Query += " from reimbursementdetails" & Session("WebTableID") & " RD inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RTM.ReimburseType_Code=RD.ReimburseType_Code Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RTM.Name where RD.emp_Code = '" & Session("Emp_code") & "' and EntryDate >= '" & Format(FromDate, "MM/dd/yyyy") & "' and EntryDate <'" & Format(CDate(dtLastDateofrecord.Rows(0)("print_name")), "MM/dd/yyyy") & "' And RTM.Name = '" & type & "' order by ReimburseDetailsID desc"
        Else
            Literal1.Text = "Claims prior to " & MonthName(ToDate.Month, True) & "-" & ToDate.Year

            sql_Query = "select ReimburseDetailsID,ROW_Number() OVER (ORDER BY ReimburseDetailsID DESC) AS 'Sl.No',IsNull(PSS.Print_Name,RTM.Name) as 'Field_Name',convert(nvarchar,EntryDate,106) as 'EntryDate',case when ClaimAmount=0 And RD.ExtraField3 = 0 then RD.ExtraField5 when ClaimAmount=0 then RD.ExtraField2 else ClaimAmount End as 'BillAmount',"
            sql_Query += "(Select top 1 Status from ReimbursementProofrejection" & Session("WebTableID") & "_" & Year & " where ClaimID=RD.ReimburseDetailsID order by UploadedDate desc) as 'Claimstatus',(Select top 1 ApprovedAmount from ReimbursementProofrejection" & Session("WebTableID") & "_" & Year & " where ClaimID=RD.ReimburseDetailsID order by UploadedDate desc) as 'ApprovedAmount',(Select top 1 Remarks from ReimbursementProofrejection" & Session("WebTableID") & "_" & Year & " where ClaimID=RD.ReimburseDetailsID order by UploadedDate desc) as 'Remarks' "
            sql_Query += " from reimbursementdetails" & Session("WebTableID") & " RD inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RTM.ReimburseType_Code=RD.ReimburseType_Code Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RTM.Name where RD.emp_Code = '" & Session("Emp_code") & "' and EntryDate Between '" & Format(FromDate, "MM/dd/yyyy") & "' and '" & Format(ToDate, "MM/dd/yyyy") & "' And RTM.Name = '" & type & "' order by ReimburseDetailsID desc"
        End If

        dsReimburseDetails = OldNewConn.GetDataTable2(sql_Query)
        radGrid2.DataSource = dsReimburseDetails
    End Sub

    Protected Sub radGrid2_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles radGrid2.ItemDataBound
        If TypeOf e.Item Is GridDataItem AndAlso e.Item.OwnerTableView.Name = "Parent" Then
            Dim griditem As GridDataItem = TryCast(e.Item, GridDataItem)

            If Not IsNothing(griditem) Then
                Dim lnkForViewReport2 As LinkButton = DirectCast(griditem("ViewReport2").FindControl("lnkForViewReport2"), LinkButton)
                Dim image As ImageButton = DirectCast(griditem("Edit").Controls(0), ImageButton)
                Dim image2 As ImageButton = DirectCast(griditem("Delete").Controls(0), ImageButton)
                Dim Claimstatus As String = griditem("Claimstatus").Text

                If Not IsNothing(image) Then

                    Dim ReimbursementID As String = griditem.GetDataKeyValue("ReimburseDetailsID")

                    Dim str As String = "Select EntryDate,RM.Name from reimbursementdetails" & Session("WebtableID") & " R inner join ReimburseMentTypeMaster" & Session("WebtableID") & " RM on R.ReimburseType_Code=RM.ReimburseType_Code where reimbursedetailsid=" & ReimbursementID & ""

                    Dim dt12 As DataTable

                    dt12 = OldNewConn.GetDataTable2(str)

                    If (dt12.Rows.Count > 0) Then
                        If Not IsDBNull(dt12.Rows(0)("EntryDate")) Then
                            Dim entryDate As DateTime = Convert.ToDateTime(dt12.Rows(0)("EntryDate"))

                            Dim variablebydate As Boolean = False
                            variablebydate = whetherlinkvisiblebydate()

                            If (Claimstatus <> "&nbsp;" And Claimstatus <> "") Then
                                image.Visible = False
                                image2.Visible = False
                            ElseIf Not (entryDate.Month = DateTime.Now.Month And entryDate.Year = DateTime.Now.Year) Then
                                image.Visible = False
                                image2.Visible = False
                            ElseIf (variablebydate) Then
                                Dim ID As String = griditem.GetDataKeyValue("ReimburseDetailsID") & ",119"
                                ID = EncryDecrypt.Encrypt(ID, "EncryptString01")

                                If (dt12.Rows(0)("Name").ToString.ToLower.Contains("deemed lta")) Then
                                    image.Attributes.Add("onclick", String.Format("return openRadWindow('{0}');", "Reimbursementdeemedlta.aspx?ID=" & ID & "&Needstoclearsession=an"))
                                ElseIf (dt12.Rows(0)("Name").ToString.ToLower.Contains("taxable lta")) Then
                                    image.Attributes.Add("onclick", String.Format("return openRadWindow('{0}');", "ReimbursementOthers.aspx?ID=" & ID & "&Needstoclearsession=an"))
                                ElseIf (dt12.Rows(0)("Name").ToString.ToLower.Contains("lta")) Then
                                    image.Attributes.Add("onclick", String.Format("return openRadWindow('{0}');", "ReimbursementLTA.aspx?ID=" & ID & "&Needstoclearsession=an"))
                                Else
                                    image.Attributes.Add("onclick", String.Format("return openRadWindow('{0}');", "ReimbursementOthers.aspx?ID=" & ID & "&Needstoclearsession=an"))
                                End If
                            Else
                                image.Visible = False
                                image2.Visible = False
                            End If
                        End If
                    End If

                    str = "Select count(emp_code) as TotalBill from ReimbursementProofUpload" & Session("WebtableID") & " where reimbursedetailsid=" & ReimbursementID & ""

                    Dim dt122 As New DataTable

                    dt122 = OldNewConn.GetDataTable2(str)

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
        End If
    End Sub

    Protected Sub radGrid2_DeleteCommand(sender As Object, e As GridCommandEventArgs) Handles radGrid2.DeleteCommand
        Dim griditem As GridDataItem = TryCast(e.Item, GridDataItem)

        If Not IsNothing(griditem) Then
            Dim image As ImageButton = DirectCast(griditem("Delete").Controls(0), ImageButton)
            If Not IsNothing(image) Then
                Dim ReimbursementID As String = griditem.GetDataKeyValue("ReimburseDetailsID")

                Dim dt12 As DataTable = OldNewConn.GetDataTable2("Select * from reimbursementdetails" & Session("WebtableID") & " where reimbursedetailsid=" & ReimbursementID & "")

                If (dt12.Rows.Count > 0) Then
                    If Not IsDBNull(dt12.Rows(0)("EntryDate")) Then
                        Dim entryDate As DateTime = Convert.ToDateTime(dt12.Rows(0)("EntryDate"))
                        If Not (entryDate.Month = DateTime.Now.Month And entryDate.Year = DateTime.Now.Year) Then
                            Response.Write("<script>alert('Previous month\'s claims can not be deleted.');</script>")
                            Exit Sub
                        End If
                    End If
                End If

                Dim str As String = "insert into dbo.DeletedReimbursedetails" & Session("WebTableID") & " (ReimburseDetailsID,Emp_Code,ReimburseType_Code,TransactionDate,TransactionDetail,ClaimAmount,ExtraField1,ExtraField2,ExtraField3,ExtraField4,ExtraField5,ExtraField6,ExtraField7,ExtraField8,TotalBillField,Rejected,BillPassed,ExtraDateField,EntryDate,DeletedBy,DeletedDate) "
                str += " select ReimburseDetailsID,Emp_Code,ReimburseType_Code,TransactionDate,TransactionDetail,ClaimAmount,ExtraField1,ExtraField2,ExtraField3,ExtraField4,ExtraField5,ExtraField6,ExtraField7,ExtraField8,TotalBillField,Rejected,BillPassed,ExtraDateField,EntryDate,'" & Session("Emp_code").ToString & "',Getdate() from reimbursementdetails" & Session("WebtableID") & " where reimbursedetailsid=" & ReimbursementID & ""
                OldNewConn.ExecuteNonQuery(CommandType.Text, str, Nothing)

                commendText = "delete from reimbursementdetails" & Session("WebtableID") & "  where Emp_Code='" & Session("Emp_Code") & "' and ReimburseDetailsId='" & ReimbursementID & "'"
                OldNewConn.ExecuteNonQuery(CommandType.Text, commendText, Nothing)

                commendText = String.Format("delete from OtherMultipleReimburseClaimDetails{0}  where Emp_Code='" & Session("Emp_Code") & "' and ReimburseDetailsId='" & ReimbursementID & "'", HttpContext.Current.Session("WebTableID"))
                OldNewConn.ExecuteNonQuery(CommandType.Text, commendText, Nothing)

                commendText = String.Format("delete from ReimbursementProofUpload{0}  where Emp_Code='" & Session("Emp_Code") & "' and ReimburseDetailsId='" & ReimbursementID & "'", HttpContext.Current.Session("WebTableID"))
                OldNewConn.ExecuteNonQuery(CommandType.Text, commendText, Nothing)
            End If
        End If
    End Sub


End Class