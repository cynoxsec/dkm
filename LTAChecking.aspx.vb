Imports System.Web.UI.WebControls
Imports Telerik.Web.UI
Imports DkmOnline.Lib
Imports DkmOnline.Common
Imports System.Data.SqlClient
Imports System.IO

Public Class LTAChecking
    Inherits System.Web.UI.Page

    Dim commendText As String
    Dim constantValuesnew As New ConstantValues
    Dim reimburseType As String
    Dim NoOfexemptioninCurrBlockYear As Int32 = 0
    Dim NoOfexemptioninPreBlockYear As Int32 = 0
    Dim EmployeeJoining As String
    Dim OldNewConn As OldNewConnection = New OldNewConnection()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsNothing(Session("Emp_Code")) Then
            Response.Redirect("../Logout.aspx")
        End If

        DatabaseFacade.Instance.Enter_SiteLog(Session("Emp_Code"), Session("LoginType"), Session("LoginAccessType"), Session("Comp_Code"), 0, Request.Url.AbsolutePath, Request.Url.AbsolutePath, Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type, Session("WebTableID"), Session.SessionID, Session("MenuID"))

        OldNewConn.CheckDatabaseOldNew(Session("WebTableID"))

        If Not (IsPostBack) Then
            If Not IsNothing(Session("Emp_Code")) And Not IsNothing(Session("WebTableID")) And Not IsNothing(Session("MenuID")) Then

                BlockYear()
                SaveBlockYear()

                Dim s As String = Request.QueryString("IDD").Replace(" ", "+")

                If s.Length Mod 4 > 0 Then
                    s = s.PadRight(s.Length + 4 - s.Length Mod 4, "="c)
                End If

                reimburseType = EncryDecrypt.Decrypt(s, "a")

                Session("reimburseType") = reimburseType.ToString()

                Dim LTACheck As Boolean = False

                LTACheck = LTAChecking()

                If (LTACheck) Then
                    LTAOptionCheck()


                Else
                    Dim script As String = "function f(){$find(""" + RadWindow.ClientID + """).show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);"
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, True)
                End If
            Else
                Response.Redirect("../Logout.aspx")
            End If
        End If
    End Sub

    Public Sub LTAsubmitcountcheck()
        Dim currentyear As String = DateTime.Now.ToString("yyyy")

        Dim sqlquery = "Select RM.ReimID from reimbursemaster" & Session("WebTableID") & " RM inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RM.Field_Name=RTM.Name inner join ReimburseTransaction" & Session("WebTableID") & " RT on RM.ReimID=RT.ReimID where RM.Emp_Code='" & Session("Emp_Code") & "' And RTM.ReimburseType_Code='" & Session("reimburseType") & "' And Year(RT.PayDate) =" & DateTime.Now.Year & "  and RT.claimed > 0 and RT.reimbursed > 0 "
        Dim dt As DataTable
        dt = OldNewConn.GetDataTable2(sqlquery)

        If dt.Rows.Count > 0 Then
            Dim message As String = "You already availed one LTA exemption in calendar year " & DateTime.Now.Year & "."
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('" & message.ToString() & "');window.location='/ReimbursementManagement/ReimbursementTypeMaster.aspx';", True)
        Else
            Response.Redirect(String.Format("ReimbursementLTA.aspx?Type=" & Request.QueryString("Type") & "&IDD=" & Request.QueryString("IDD") & ""))
        End If


    End Sub


    Public Sub BlockYear()

        Dim dtBlockyearChangedate As DataTable = OldNewConn.GetDataTable2("Select name, print_name from payslipsetup" & Session("WebTableID") & " where name in ('BlockyearChangedate') ")

        If (dtBlockyearChangedate.Rows.Count > 0) Then
            If (CDate(dtBlockyearChangedate.Rows(0)("print_name")).Date < Date.Now) Then
                Dim dtblockYear As DataTable = OldNewConn.GetDataTable2("Select * from BlockYear" & Session("WebTableID") & " where Description = 'SecondBlockyear'")

                If (dtblockYear.Rows.Count > 0) Then
                    If (CInt(dtblockYear.Rows(0)("LastBlockYear")) < Date.Now.Year) Then
                        commendText = "update BlockYear" & Session("WebTableID") & " set FirstBlockYear='" & dtblockYear.Rows(0)("FirstBlockYear") & "',LastBlockYear='" & dtblockYear.Rows(0)("LastblockYear") & "' where Description='FirstBlockyear'"
                        OldNewConn.ExecuteNonQuery(CommandType.Text, commendText, Nothing)

                        commendText = "update BlockYear" & Session("WebTableID") & " set FirstBlockYear='" & CInt(dtblockYear.Rows(0)("FirstBlockYear")) + 4 & "',LastBlockYear='" & CInt(dtblockYear.Rows(0)("LastblockYear")) + 4 & "' where Description='SecondBlockyear'"
                        OldNewConn.ExecuteNonQuery(CommandType.Text, commendText, Nothing)
                    End If
                Else
                    commendText = "insert into BlockYear" & Session("WebTableID") & " values ('FirstBlockyear','2018','2021',GETDATE())"
                    OldNewConn.ExecuteNonQuery(CommandType.Text, commendText, Nothing)

                    commendText = "insert into BlockYear" & Session("WebTableID") & " values ('SecondBlockyear','2022','2025',GETDATE())"
                    OldNewConn.ExecuteNonQuery(CommandType.Text, commendText, Nothing)
                    BlockYear()
                End If
            End If
        Else
            Dim BlockyearChangedate As Date = New Date(Date.Now.Year, 3, 31)

            If (BlockyearChangedate.Date < Date.Now) Then
                Dim dtblockYear As DataTable = OldNewConn.GetDataTable2("Select * from BlockYear" & Session("WebTableID") & " where Description = 'SecondBlockyear'")

                If (dtblockYear.Rows.Count > 0) Then
                    If (CInt(dtblockYear.Rows(0)("LastBlockYear")) < Date.Now.Year) Then
                        commendText = "update BlockYear" & Session("WebTableID") & " set FirstBlockYear='" & dtblockYear.Rows(0)("FirstBlockYear") & "',LastBlockYear='" & dtblockYear.Rows(0)("LastblockYear") & "' where Description='FirstBlockyear'"
                        OldNewConn.ExecuteNonQuery(CommandType.Text, commendText, Nothing)

                        commendText = "update BlockYear" & Session("WebTableID") & " set FirstBlockYear='" & CInt(dtblockYear.Rows(0)("FirstBlockYear")) + 4 & "',LastBlockYear='" & CInt(dtblockYear.Rows(0)("LastblockYear")) + 4 & "' where Description='SecondBlockyear'"
                        OldNewConn.ExecuteNonQuery(CommandType.Text, commendText, Nothing)
                    End If
                Else
                    commendText = "insert into BlockYear" & Session("WebTableID") & " values ('FirstBlockyear','2018','2021',GETDATE())"
                    OldNewConn.ExecuteNonQuery(CommandType.Text, commendText, Nothing)

                    commendText = "insert into BlockYear" & Session("WebTableID") & " values ('SecondBlockyear','2022','2025',GETDATE())"
                    OldNewConn.ExecuteNonQuery(CommandType.Text, commendText, Nothing)
                    BlockYear()
                End If
            End If
        End If

    End Sub

    Public Sub SaveBlockYear()
        Dim dtblockYear As DataTable = OldNewConn.GetDataTable2("Select * from BlockYear" & Session("WebTableID") & "")

        If (dtblockYear.Rows.Count > 0) Then
            Session("PreFirstBlockYear") = dtblockYear.Rows(0)("FirstBlockYear")
            Session("PreSecBlockYear") = dtblockYear.Rows(0)("LastBlockYear")
            Session("CurrFirstBlockYear") = dtblockYear.Rows(1)("FirstBlockYear")
            Session("CurrSecBlockYear") = dtblockYear.Rows(1)("LastBlockYear")

            EmployeeJoining = employeeJoiningDate()

            If CDate(EmployeeJoining).Year < CInt(Session("CurrFirstBlockYear")) Then
            ElseIf CDate(EmployeeJoining).Year >= CInt(Session("CurrFirstBlockYear")) And CDate(EmployeeJoining).Year <= CInt(Session("CurrSecBlockYear")) Then
            ElseIf CDate(EmployeeJoining).Year = CInt(Session("CurrSecBlockYear")) + 1 Then
                Session("PreFirstBlockYear") = CInt(dtblockYear.Rows(0)("FirstBlockYear")) + 4
                Session("PreSecBlockYear") = CInt(dtblockYear.Rows(0)("LastBlockYear")) + 4
                Session("CurrFirstBlockYear") = CInt(dtblockYear.Rows(1)("FirstBlockYear")) + 4
                Session("CurrSecBlockYear") = CInt(dtblockYear.Rows(1)("LastBlockYear")) + 4
            End If

        End If
    End Sub

    Public Function checkLTAclaimedincurrentBlockYear(ByVal FirstBlockYear As String, ByVal SecBlockYear As String) As Int32

        'Dim strReimtrans As String = "select * from ReimburseTransaction" & Session("WebTableID") & " where ReimID in (Select R.ReimID from reimbursemaster" & Session("WebTableID") & " R inner join ReimburseMentTypeMaster" & Session("WebTableID") & " R1 on R1.Name=R.Field_Name where R.Emp_Code='" & Session("Emp_Code") & "' And R1.ReimburseType_Code='" & Session("reimburseType") & "') And Year(PayDate) between '" & CInt(FirstBlockYear) & "' and '" & CInt(SecBlockYear) & "'"
        Dim strReimtrans As String = "Select RM.ReimID from reimbursemaster" & Session("WebTableID") & " RM inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RM.Field_Name=RTM.Name inner join ReimburseTransaction" & Session("WebTableID") & " RT on RM.ReimID=RT.ReimID where RM.Emp_Code='" & Session("Emp_Code") & "' And RTM.ReimburseType_Code='" & Session("reimburseType") & "' And PayDate between '04/01/" & CInt(FirstBlockYear) & "' and '03/31/" & CInt(SecBlockYear) & "' and RT.claimed > 0 and RT.reimbursed > 0 And RT.PayDate = (SELECT MAX(PayDate) FROM ReimburseTransaction" & Session("WebTableID") & " WHERE ReimID = RM.ReimID)"

        Dim dtReimtrans As DataTable

        dtReimtrans = OldNewConn.GetDataTable2(strReimtrans)

        Return dtReimtrans.Rows.Count
    End Function

    Public Function employeeJoiningDate() As String
        Dim stremployeeJoiningDate As String = "Select * from employeesmaster" & Session("WebTableID") & " where emp_code='" & Session("Emp_Code") & "'"
        Dim dtemployeeJoiningDate As DataTable

        dtemployeeJoiningDate = OldNewConn.GetDataTable2(stremployeeJoiningDate)

        Return dtemployeeJoiningDate.Rows(0)("JoiningDate").ToString()
    End Function

    Public Function LTAChecking() As Boolean

        Dim strfbplink As String = "Select top 2 * from LTAChecking" & Session("WebTableID") & " where emp_Code='" & Session("Emp_Code") & "' And FirstBlockYear in (" & Session("PreFirstBlockYear") & "," & Session("CurrFirstBlockYear") & ") order by CreatedDate desc"

        Dim dtfbplink As DataTable

        dtfbplink = OldNewConn.GetDataTable2(strfbplink)

        If (dtfbplink.Rows.Count > 1) Then

            Dim NoOfClaims As Integer = Convert.ToInt32(dtfbplink.Rows(0)("NoOfClaims")) + Convert.ToInt32(dtfbplink.Rows(1)("NoOfClaims"))

            If (NoOfClaims <> 4) Then
                Return True
            Else
                Return False
            End If
        Else
            Return True
        End If
    End Function

    Public Function LTAOptionCheck()

        NoOfexemptioninCurrBlockYear = checkLTAclaimedincurrentBlockYear(Session("CurrFirstBlockYear"), Session("CurrSecBlockYear"))

        If NoOfexemptioninCurrBlockYear < 2 Then

            EmployeeJoining = employeeJoiningDate()

            If CDate(EmployeeJoining).Year < CInt(Session("CurrFirstBlockYear")) Then

                Dim Rowsnumber As Int32 = InsertIntoLTAChecking(Session("PreFirstBlockYear"), Session("PreSecBlockYear"), NoOfexemptioninPreBlockYear)
                Dim Rowsnumber2 As Int32 = InsertIntoLTAChecking(Session("CurrFirstBlockYear"), Session("CurrSecBlockYear"), NoOfexemptioninCurrBlockYear)
                If Session("WebTableID").ToString = "11046" Then
                    LTAsubmitcountcheck()
                Else
                    Response.Redirect(String.Format("ReimbursementLTA.aspx?Type=" & Request.QueryString("Type") & "&IDD=" & Request.QueryString("IDD") & ""))
                End If

            ElseIf CDate(EmployeeJoining).Year = CInt(Session("CurrFirstBlockYear")) And CDate(EmployeeJoining).Day = 1 Then

                Dim Rowsnumber As Int32 = InsertIntoLTAChecking(Session("PreFirstBlockYear"), Session("PreSecBlockYear"), NoOfexemptioninPreBlockYear)
                Dim Rowsnumber2 As Int32 = InsertIntoLTAChecking(Session("CurrFirstBlockYear"), Session("CurrSecBlockYear"), NoOfexemptioninCurrBlockYear)
                If Session("WebTableID").ToString = "11046" Then
                    LTAsubmitcountcheck()
                Else
                    Response.Redirect(String.Format("ReimbursementLTA.aspx?Type=" & Request.QueryString("Type") & "&IDD=" & Request.QueryString("IDD") & ""))
                End If


            ElseIf CDate(EmployeeJoining).Year >= CInt(Session("CurrFirstBlockYear")) And CDate(EmployeeJoining).Year <= CInt(Session("CurrSecBlockYear")) Then
                lblChecknewHire1.Text = "Number of LTA exemption taken (including current employer) in current block year (" & Session("CurrFirstBlockYear") & " - " & Session("CurrSecBlockYear") & ") : "
                ChecknewHire1.Visible = True
            End If
        Else

            NoOfexemptioninPreBlockYear = checkLTAclaimedincurrentBlockYear(Session("PreFirstBlockYear"), Session("PreSecBlockYear"))

            If NoOfexemptioninPreBlockYear < 2 Then

                EmployeeJoining = employeeJoiningDate()

                If CDate(EmployeeJoining).Year < CInt(Session("PreFirstBlockYear")) Then
                    If (System.DateTime.Now.Year = CInt(Session("CurrFirstBlockYear"))) Then
                        Dim Rowsnumber As Int32 = InsertIntoLTAChecking(Session("PreFirstBlockYear"), Session("PreSecBlockYear"), NoOfexemptioninPreBlockYear)
                        Dim Rowsnumber2 As Int32 = InsertIntoLTAChecking(Session("CurrFirstBlockYear"), Session("CurrSecBlockYear"), NoOfexemptioninCurrBlockYear)
                        If Session("WebTableID").ToString = "11046" Then
                            LTAsubmitcountcheck()
                        Else
                            Response.Redirect(String.Format("ReimbursementLTA.aspx?Type=" & Request.QueryString("Type") & "&IDD=" & Request.QueryString("IDD") & ""))
                        End If
                    Else
                        Dim message As String = "You are not eligible for carry-over LTA exemption as it is lapsed due to the reason of not claiming it in first calendar year (" & Session("CurrFirstBlockYear") & ") of the current block year (" & Session("CurrFirstBlockYear") & " -  " & Session("CurrSecBlockYear") & "). 2 exemptions of current block year (" & Session("CurrFirstBlockYear") & " -  " & Session("CurrSecBlockYear") & ") is also been availed by you already."
                        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('" & message.ToString() & "');window.location='/ReimbursementManagement/ReimbursementTypeMaster.aspx';", True)
                    End If
                Else
                    lblChecknewHire3.Text = "Number of LTA exemption taken in last block year (" & Session("PreFirstBlockYear") & " - " & Session("PreSecBlockYear") & ") : "
                    ChecknewHire3.Visible = True
                End If
            Else
                Dim message As String = "You already have taken the LTA exemption twice in current block year (" & Session("CurrFirstBlockYear") & " -  " & Session("CurrSecBlockYear") & ") and last block year (" & Session("PreFirstBlockYear") & " - " & Session("PreSecBlockYear") & "), hence not eligible for the same."
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('" & message.ToString() & "');window.location='/ReimbursementManagement/ReimbursementTypeMaster.aspx';", True)
            End If
        End If
    End Function

    Protected Sub btnChecknewHire1_Click(sender As Object, e As EventArgs)

        NoOfexemptioninCurrBlockYear = checkLTAclaimedincurrentBlockYear(Session("CurrFirstBlockYear"), Session("CurrSecBlockYear"))

        If (NoOfexemptioninCurrBlockYear > CInt(ddlChecknewHire1.SelectedValue)) Then
            Dim message As String = "You already have taken 1 exemption with current employer, please mention including that"
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('" & message.ToString() & "');", True)
        Else
            If (ddlChecknewHire1.SelectedValue = 0 Or ddlChecknewHire1.SelectedValue = 1) Then

                Dim Rowsnumber As Int32 = InsertIntoLTAChecking(Session("PreFirstBlockYear"), Session("PreSecBlockYear"), 0)
                Dim Rowsnumber2 As Int32 = InsertIntoLTAChecking(Session("CurrFirstBlockYear"), Session("CurrSecBlockYear"), ddlChecknewHire1.SelectedItem.Text)

                Response.Redirect(String.Format("ReimbursementLTA.aspx?Type=" & Request.QueryString("Type") & "&IDD=" & Request.QueryString("IDD") & ""))
            ElseIf (ddlChecknewHire1.SelectedValue = 2) Then
                ChecknewHire1.Visible = False
                lblChecknewHire2.Text = "Number of LTA exemption taken in last block year (" & Session("PreFirstBlockYear") & " - " & Session("PreSecBlockYear") & ") : "
                ChecknewHire2.Visible = True
            End If
        End If
    End Sub

    Protected Sub btnChecknewHire2_Click(sender As Object, e As EventArgs)
        EmployeeJoining = employeeJoiningDate()
        If (ddlChecknewHire2.SelectedValue = 0 Or ddlChecknewHire2.SelectedValue = 1) Then

            If (System.DateTime.Now.Year = CInt(Session("CurrFirstBlockYear"))) Then
                Dim Rowsnumber As Int32 = InsertIntoLTAChecking(Session("PreFirstBlockYear"), Session("PreSecBlockYear"), ddlChecknewHire2.SelectedItem.Text)
                Dim Rowsnumber2 As Int32 = InsertIntoLTAChecking(Session("CurrFirstBlockYear"), Session("CurrSecBlockYear"), 2)

                Response.Redirect(String.Format("ReimbursementLTA.aspx?Type=" & Request.QueryString("Type") & "&IDD=" & Request.QueryString("IDD") & ""))
            Else
                'Response.Write("<script>alert('You are not eligible for carry-over LTA exemption');</script>")
                Dim message As String = "You are not eligible for carry-over LTA exemption as it is lapsed due to the reason of not claiming it in first calendar year (" & Session("CurrFirstBlockYear") & ") of the current block year (" & Session("CurrFirstBlockYear") & " -  " & Session("CurrSecBlockYear") & ")."
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('" & message.ToString() & "');window.location='/ReimbursementManagement/ReimbursementTypeMaster.aspx';", True)
            End If
        ElseIf (ddlChecknewHire2.SelectedValue = 2) Then

            Dim Rowsnumber As Int32 = InsertIntoLTAChecking(Session("PreFirstBlockYear"), Session("PreSecBlockYear"), 2)
            Dim Rowsnumber2 As Int32 = InsertIntoLTAChecking(Session("CurrFirstBlockYear"), Session("CurrSecBlockYear"), 2)

            Dim message As String = "You already have taken the LTA exemption twice in current block year (" & Session("CurrFirstBlockYear") & " - " & Session("CurrSecBlockYear") & ") and last block year (" & Session("PreFirstBlockYear") & " - " & Session("PreSecBlockYear") & "), hence not eligible for the same."
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('" & message.ToString() & "');window.location='/ReimbursementManagement/ReimbursementTypeMaster.aspx';", True)
        Else
        End If
    End Sub

    Protected Sub btnChecknewHire3_Click(sender As Object, e As EventArgs)
        EmployeeJoining = employeeJoiningDate()

        If (ddlChecknewHire3.SelectedValue = 0 Or ddlChecknewHire3.SelectedValue = 1) Then
            If CDate(EmployeeJoining).Year > CInt(Session("PreFirstBlockYear")) And CDate(EmployeeJoining).Year < CInt(Session("PreSecBlockYear")) Then
                If (System.DateTime.Now.Year = CInt(Session("CurrFirstBlockYear"))) Then
                    Dim Rowsnumber As Int32 = InsertIntoLTAChecking(Session("PreFirstBlockYear"), Session("PreSecBlockYear"), ddlChecknewHire3.SelectedItem.Text)
                    Dim Rowsnumber2 As Int32 = InsertIntoLTAChecking(Session("CurrFirstBlockYear"), Session("CurrSecBlockYear"), 2)

                    Response.Redirect(String.Format("ReimbursementLTA.aspx?Type=" & Request.QueryString("Type") & "&IDD=" & Request.QueryString("IDD") & ""))
                Else
                    Dim message As String = "You are not eligible for carry-over LTA exemption as it is lapsed due to the reason of not claiming it in first calendar year (" & Session("CurrFirstBlockYear") & ") of the current block year (" & Session("CurrFirstBlockYear") & " -  " & Session("CurrSecBlockYear") & "). 2 exemptions of current block year (" & Session("CurrFirstBlockYear") & " -  " & Session("CurrSecBlockYear") & ") is also been availed by you already."
                    ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('" & message.ToString() & "');window.location='/ReimbursementManagement/ReimbursementTypeMaster.aspx';", True)
                End If
            Else
                'Response.Write("<script>alert('You are not eligible for carry-over LTA exemption');</script>")
                Dim message As String = "You are not eligible for carry-over LTA exemption as it is lapsed due to the reason of not claiming it in first calendar year (" & Session("CurrFirstBlockYear") & ") of the current block year (" & Session("CurrFirstBlockYear") & " -  " & Session("CurrSecBlockYear") & "). 2 exemptions of current block year (" & Session("CurrFirstBlockYear") & " -  " & Session("CurrSecBlockYear") & ") is also been availed by you already."
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('" & message.ToString() & "');window.location='/ReimbursementManagement/ReimbursementTypeMaster.aspx';", True)
            End If
        ElseIf (ddlChecknewHire3.SelectedValue = 2) Then

            Dim Rowsnumber As Int32 = InsertIntoLTAChecking(Session("PreFirstBlockYear"), Session("PreSecBlockYear"), 2)
            Dim Rowsnumber2 As Int32 = InsertIntoLTAChecking(Session("CurrFirstBlockYear"), Session("CurrSecBlockYear"), 2)

            Dim message As String = "You already have taken the LTA exemption twice in current block year (" & Session("CurrFirstBlockYear") & " - " & Session("CurrSecBlockYear") & ") and last block year (" & Session("PreFirstBlockYear") & " - " & Session("PreSecBlockYear") & "), hence not eligible for the same."
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('" & message.ToString() & "');window.location='/ReimbursementManagement/ReimbursementTypeMaster.aspx';", True)
        Else
        End If
    End Sub

    Protected Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        Dim script As String = "function f(){$find(""" + radWindow1.ClientID + """).show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);"
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, True)
    End Sub

    Protected Sub btnHide_Click(sender As Object, e As EventArgs) Handles btnHide.Click
        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "window.location='/ReimbursementManagement/ReimbursementTypeMaster.aspx';", True)
    End Sub

    Protected Sub btnOk1_Click(sender As Object, e As EventArgs) Handles btnOk1.Click

        NoOfexemptioninCurrBlockYear = checkLTAclaimedincurrentBlockYear(Session("CurrFirstBlockYear"), Session("CurrSecBlockYear"))

        If (System.DateTime.Now.Year = CInt(Session("CurrFirstBlockYear"))) Then
            LTAOptionCheck()
        ElseIf NoOfexemptioninCurrBlockYear < 2 Then
            LTAOptionCheck()
        Else
            Dim message As String = "You are not eligible for carry-over LTA exemption as it is lapsed due to the reason of not claiming it in first calendar year (" & Session("CurrFirstBlockYear") & ") of the current block year (" & Session("CurrFirstBlockYear") & " -  " & Session("CurrSecBlockYear") & "). 2 exemptions of current block year (" & Session("CurrFirstBlockYear") & " -  " & Session("CurrSecBlockYear") & ") is also been availed by you already."
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('" & message.ToString() & "');window.location='/ReimbursementManagement/ReimbursementTypeMaster.aspx';", True)
        End If
    End Sub

    Protected Sub btnHide1_Click(sender As Object, e As EventArgs) Handles btnHide1.Click
        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "window.location='/ReimbursementManagement/ReimbursementTypeMaster.aspx';", True)
    End Sub

    Public Function InsertIntoLTAChecking(ByVal FirstBlockYear As Int32, ByVal LastBlockYear As Int32, ByVal NoOfClaims As Int32) As Integer

        Dim strfbplink As String = "Select * from LTAChecking" & Session("WebTableID") & " where emp_Code='" & Session("Emp_Code") & "' And FirstBlockYear='" & FirstBlockYear & "' And LastBlockYear='" & LastBlockYear & "'"

        Dim dtfbplink As DataTable

        dtfbplink = OldNewConn.GetDataTable2(strfbplink)

        If (dtfbplink.Rows.Count = 0) Then
            commendText = "Insert into LTAChecking" & Session("WebTableID") & " (Emp_code,FirstBlockYear,LastBlockYear,NoOfClaims,CreatedDate) values ('" & Session("Emp_Code") & "'," & FirstBlockYear & "," & LastBlockYear & "," & NoOfClaims & ",Getdate());Select Scope_Identity();"
        Else
            commendText = "update LTAChecking" & Session("WebTableID") & " set FirstBlockYear='" & FirstBlockYear & "',LastBlockYear='" & LastBlockYear & "',NoOfClaims='" & NoOfClaims & "',CreatedDate=Getdate() where CheckingID='" & dtfbplink.Rows(0)("CheckingID").ToString & "'"
        End If

        Return OldNewConn.ExecuteScalar(CommandType.Text, commendText, Nothing)

    End Function
End Class