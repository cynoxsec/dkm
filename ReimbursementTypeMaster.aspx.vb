Imports Telerik.Web.UI
Imports DkmOnline.Lib
Imports DkmOnline.Common
Imports System.Data.SqlClient
Imports System.IO


Public Class ReimbursementTypeMaster
    Inherits System.Web.UI.Page

    Dim dtOption As New DataTable
    Dim dtActReim As New DataTable
    Dim dtPSS As New DataTable

    Dim OldNewConn As OldNewConnection = New OldNewConnection()

    Dim Year As Int32

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If IsNothing(Session("Emp_Code")) Then
            Response.Redirect("../Logout.aspx")
        End If

        DatabaseFacade.Instance.Enter_SiteLog(Session("Emp_Code"), Session("LoginType"), Session("LoginAccessType"), Session("Comp_Code"), 0, Request.Url.AbsolutePath, Request.Url.AbsolutePath, Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type, Session("WebTableID"), Session.SessionID, Session("MenuID"))

        OldNewConn.CheckDatabaseOldNew(Session("WebTableID"))

        Dim commendText As String = "IF OBJECT_ID('dbo.ReimYearMaster" & Session("WebTableID") & "', 'U') IS NULL "
        commendText += "BEGIN "
        commendText += "CREATE TABLE [dbo].[ReimYearMaster" & Session("WebTableID") & "]("
        commendText += "[Year] [char](9) NOT NULL,"
        commendText += "[Remarks] [varchar](200) NOT NULL,"
        commendText += "[CloseYear] [bit] NOT NULL Default 0,"
        commendText += ")"
        commendText += " END"

        OldNewConn.ExecuteNonQuery(CommandType.Text, commendText, Nothing)

        Dim dtYear As DataTable = OldNewConn.GetDataTable2("select * from dbo.ReimYearMaster" & Session("WebTableID") & "")

        If (dtYear.Rows.Count > 0) Then
            Year = dtYear.Rows(0)("Year")
        End If

        dtOption = OldNewConn.GetDataTable2("Select Print_Name from PayslipSetup" & Session("WebTableID") & " where Name ='ReimbursementOption'")
        dtActReim = OldNewConn.GetDataTable2("Select Print_Name from PayslipSetup" & Session("WebTableID") & " where Name ='ActivateReimbursementDeclaration'")

        If Not (IsPostBack) Then
            If Not IsNothing(Session("Emp_Code")) And Not IsNothing(Session("WebTableID")) And Not IsNothing(Session("MenuID")) Then
                Session("ReimbursetableTravel") = Nothing
                Session("ReimbursetableMultiple") = Nothing
                Session("TempDict") = Nothing
                Session("UploadedFile") = Nothing

                If Not (IsPostBack) Then
                End If
            Else
                Response.Redirect("../Logout.aspx")
            End If

            dtPSS = OldNewConn.GetDataTable2("Select print_Name from payslipsetup" & Session("WebTableID") & " where Name ='ReimbursementBillsCarryForward'")

            If (dtOption.Rows.Count > 0) Then
                If (dtOption.Rows(0)("Print_Name").ToString.ToLower = "withoutbudget") Then
                    radGridSummary.Visible = False
                    'If dtActReim.Rows.Count > 0 Then
                    '    If String.IsNullOrEmpty(dtActReim.Rows(0)("print_Name").ToString) Then
                    '        radGridSummary2.MasterTableView.Columns.FindByUniqueName("reimdeclartionamt").Visible = False
                    '        btnreideclar.Style.Add("display", "none")
                    '    Else
                    '        If dtActReim.Rows(0)("print_Name").ToString.ToUpper = "Y" Or dtActReim.Rows(0)("print_Name").ToString.ToUpper = "YES" Then

                    '        Else
                    '            radGridSummary2.MasterTableView.Columns.FindByUniqueName("reimdeclartionamt").Visible = False
                    '            btnreideclar.Style.Add("display", "none")
                    '        End If

                    '    End If
                    'Else
                    '    radGridSummary2.MasterTableView.Columns.FindByUniqueName("reimdeclartionamt").Visible = False
                    '    btnreideclar.Style.Add("display", "none")
                    'End If

                ElseIf (dtOption.Rows(0)("Print_Name").ToString.ToLower = "withfbp") Then
                    If (dtPSS.Rows.Count > 0) Then
                        If (dtPSS.Rows(0)("print_Name").ToString.ToLower = "y") Then
                            radGridSummary.MasterTableView.GetColumn("BillsCF").Display = True
                            radGridSummary.MasterTableView.GetColumn("BillsTobesubmit").Display = True
                        Else
                            radGridSummary.MasterTableView.GetColumn("BillsCF").Display = False
                            radGridSummary.MasterTableView.GetColumn("BillsTobesubmit").Display = False
                        End If
                    Else
                        radGridSummary.MasterTableView.GetColumn("BillsCF").Display = False
                        radGridSummary.MasterTableView.GetColumn("BillsTobesubmit").Display = False
                    End If
                ElseIf (dtOption.Rows(0)("Print_Name").ToString.ToLower = "with&withoutbudget") Then
                    If (dtPSS.Rows.Count > 0) Then
                        If (dtPSS.Rows(0)("print_Name").ToString.ToLower = "y") Then
                            radGridSummary.MasterTableView.GetColumn("BillsCF").Display = True
                            radGridSummary.MasterTableView.GetColumn("BillsTobesubmit").Display = True
                        Else
                            radGridSummary.MasterTableView.GetColumn("BillsCF").Display = False
                            radGridSummary.MasterTableView.GetColumn("BillsTobesubmit").Display = False
                        End If
                    Else
                        radGridSummary.MasterTableView.GetColumn("BillsCF").Display = False
                        radGridSummary.MasterTableView.GetColumn("BillsTobesubmit").Display = False
                    End If
                Else
                    If (dtPSS.Rows.Count > 0) Then
                        If (dtPSS.Rows(0)("print_Name").ToString.ToLower = "y") Then
                            radGridSummary.MasterTableView.GetColumn("BillsCF").Display = True
                            radGridSummary.MasterTableView.GetColumn("BillsTobesubmit").Display = True
                        Else
                            radGridSummary.MasterTableView.GetColumn("BillsCF").Display = False
                            radGridSummary.MasterTableView.GetColumn("BillsTobesubmit").Display = False
                        End If
                    Else
                        radGridSummary.MasterTableView.GetColumn("BillsCF").Display = False
                        radGridSummary.MasterTableView.GetColumn("BillsTobesubmit").Display = False
                    End If
                    radGridSummary2.Visible = False
                End If
            Else
                If (dtPSS.Rows.Count > 0) Then
                    If (dtPSS.Rows(0)("print_Name").ToString.ToLower = "y") Then
                        radGridSummary.MasterTableView.GetColumn("BillsCF").Display = True
                        radGridSummary.MasterTableView.GetColumn("BillsTobesubmit").Display = True
                    Else
                        radGridSummary.MasterTableView.GetColumn("BillsCF").Display = False
                        radGridSummary.MasterTableView.GetColumn("BillsTobesubmit").Display = False
                    End If
                Else
                    radGridSummary.MasterTableView.GetColumn("BillsCF").Display = False
                    radGridSummary.MasterTableView.GetColumn("BillsTobesubmit").Display = False
                End If
                radGridSummary2.Visible = False
            End If
        End If
    End Sub

    Protected Sub radGridSummary_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles radGridSummary.NeedDataSource
        Dim str As String

        Dim dt1 As New DataTable

        If (dtOption.Rows.Count > 0) Then
            If (dtOption.Rows(0)("Print_Name").ToString.ToLower = "withfbp") Then
                str = "Select Distinct ISNULL(PSS.Print_Name,RM.Field_Name) as 'Field_Name', convert(nvarchar,Budget_Wef,106) as 'Budget_WEF',convert(nvarchar,Budget_Wet,106) as 'Budget_WET',convert(float,Opening) as Opening,convert(float,prorata) as Budget,convert(float,ISNULL(SUM(Claimed),0)) as 'Claimed_Amount',convert(float,ISNULL(SUM(Reimbursed+Taxable),0)) as 'Reimbursed_Amount',convert(float,isnull(prorata+Opening,0)-isnull(SUM(Reimbursed+Taxable),0)) as [Balance_Amount],convert(float,isnull((sum(Claimed)-(sum(Reimbursed+BillPassed+Taxable))),0)) as BillsCF,convert(float,isnull(prorata+Opening,0)-isnull(SUM(Reimbursed+Taxable),0)-isnull((sum(Claimed)-(sum(Reimbursed+BillPassed+Taxable))),0)) as [BillsTobesubmit],rm.Budget as aBudget from reimbursemaster" & Session("WebTableID") & " RM inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RTM.Name=RM.Field_Name Left join ReimburseTransaction" & Session("WebTableID") & " RT  on RM.ReimID=RT.ReimID Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RM.Field_Name where RM.Emp_Code='" & Session("Emp_Code") & "' and Budget_Wet between '04/01/" & Year & "' and '03/31/" & Year + 1 & "' and isinactive=0 group by  RM.Field_Name,PSS.Print_Name, Budget_Wef ,Budget_Wet ,Opening,prorata,rm.Budget"
                dt1 = OldNewConn.GetDataTable2(str)
                If (dt1.Rows.Count > 0) Then
                    radGridSummary.DataSource = dt1
                    div1.Visible = True
                Else
                    radGridSummary.Visible = False
                End If
            ElseIf (dtOption.Rows(0)("Print_Name").ToString.ToLower = "with&withoutbudget") Then

                Dim dtWithBudgetComp As DataTable = OldNewConn.GetDataTable2("Select print_Name from payslipsetup" & Session("WebTableID") & " where Name ='WithBudgetComp'")
                Dim StrWithBudgetComp As String = ""

                If (dtWithBudgetComp.Rows.Count > 0) Then
                    StrWithBudgetComp = "and RM.Field_Name in (" & dtWithBudgetComp.Rows(0)("print_Name").Replace("#", "'") & ")"
                End If

                str = "Select Distinct ISNULL(PSS.Print_Name,RM.Field_Name) as 'Field_Name', convert(nvarchar,Budget_Wef,106) as 'Budget_WEF',convert(nvarchar,Budget_Wet,106) as 'Budget_WET',convert(float,Opening) as Opening,convert(float,prorata) as Budget,convert(float,ISNULL(SUM(Claimed),0)) as 'Claimed_Amount',convert(float,ISNULL(SUM(Reimbursed+Taxable),0)) as 'Reimbursed_Amount',convert(float,isnull(prorata+Opening,0)-isnull(SUM(Reimbursed+Taxable),0)) as [Balance_Amount],convert(float,isnull((sum(Claimed)-(sum(Reimbursed+BillPassed+Taxable))),0)) as BillsCF,convert(float,isnull(prorata+Opening,0)-isnull(SUM(Reimbursed+Taxable),0)-isnull((sum(Claimed)-(sum(Reimbursed+BillPassed+Taxable))),0)) as [BillsTobesubmit],rm.Budget as aBudget from reimbursemaster" & Session("WebTableID") & " RM inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RTM.Name=RM.Field_Name Left join ReimburseTransaction" & Session("WebTableID") & " RT  on RM.ReimID=RT.ReimID Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RM.Field_Name where RM.Emp_Code='" & Session("Emp_Code") & "' and Budget_Wet between '04/01/" & Year & "' and '03/31/" & Year + 1 & "' and isinactive=0 " & StrWithBudgetComp & " group by RM.Field_Name,PSS.Print_Name, Budget_Wef ,Budget_Wet ,Opening,prorata,rm.Budget"

                dt1 = OldNewConn.GetDataTable2(str)

                If (dt1.Rows.Count > 0) Then
                    radGridSummary.DataSource = dt1
                    'div1.Visible = True
                Else
                    radGridSummary.Visible = False
                End If
            Else
                Dim dtBudgetZero As DataTable = OldNewConn.GetDataTable2("Select name, print_name from payslipsetup" & Session("WebTableID") & " where name in ('ShowBudgetZero') ")
                Dim strBudgetZeroCondition = ""

                If (dtBudgetZero.Rows.Count > 0) Then
                    If (dtBudgetZero.Rows(0)("Print_Name").ToString.ToLower = "n") Then
                        strBudgetZeroCondition = "and Prorata > 0"
                    End If
                End If
                str = "Select Distinct ISNULL(PSS.Print_Name,RM.Field_Name) as 'Field_Name', convert(nvarchar,Budget_Wef,106) as 'Budget_WEF',convert(nvarchar,Budget_Wet,106) as 'Budget_WET',convert(float,Opening) as Opening,convert(float,prorata) as Budget,convert(float,ISNULL(SUM(Claimed),0)) as 'Claimed_Amount',convert(float,ISNULL(SUM(Reimbursed+Taxable),0)) as 'Reimbursed_Amount',convert(float,isnull(prorata+Opening,0)-isnull(SUM(Reimbursed+Taxable),0)) as [Balance_Amount],convert(float,isnull((sum(Claimed)-(sum(Reimbursed+BillPassed+Taxable))),0)) as BillsCF,convert(float,isnull(prorata+Opening,0)-isnull(SUM(Reimbursed+Taxable),0)-isnull((sum(Claimed)-(sum(Reimbursed+BillPassed+Taxable))),0)) as [BillsTobesubmit],rm.Budget as aBudget from reimbursemaster" & Session("WebTableID") & " RM inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RTM.Name=RM.Field_Name Left join ReimburseTransaction" & Session("WebTableID") & " RT  on RM.ReimID=RT.ReimID Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RM.Field_Name where RM.Emp_Code='" & Session("Emp_Code") & "' and Budget_Wet between '04/01/" & Year & "' and '03/31/" & Year + 1 & "' and isinactive=0 " & strBudgetZeroCondition & " group by  RM.Field_Name,PSS.Print_Name, Budget_Wef ,Budget_Wet ,Opening,prorata,rm.Budget"
                dt1 = OldNewConn.GetDataTable2(str)
                If (dt1.Rows.Count > 0) Then
                    radGridSummary.DataSource = dt1
                End If
            End If
        Else

            Dim dtBudgetZero As DataTable = OldNewConn.GetDataTable2("Select name, print_name from payslipsetup" & Session("WebTableID") & " where name in ('ShowBudgetZero') ")
            Dim strBudgetZeroCondition = ""

            If (dtBudgetZero.Rows.Count > 0) Then
                If (dtBudgetZero.Rows(0)("Print_Name").ToString.ToLower = "n") Then
                    strBudgetZeroCondition = "and Prorata > 0"
                End If
            End If

            str = "Select Distinct ISNULL(PSS.Print_Name,RM.Field_Name) as 'Field_Name', convert(nvarchar,Budget_Wef,106) as 'Budget_WEF',convert(nvarchar,Budget_Wet,106) as 'Budget_WET',convert(float,Opening) as Opening,convert(float,prorata) as Budget,convert(float,ISNULL(SUM(Claimed),0)) as 'Claimed_Amount',convert(float,ISNULL(SUM(Reimbursed+Taxable),0)) as 'Reimbursed_Amount',convert(float,isnull(prorata+Opening,0)-isnull(SUM(Reimbursed+Taxable),0)) as [Balance_Amount],convert(float,isnull((sum(Claimed)-(sum(Reimbursed+BillPassed+Taxable))),0)) as BillsCF,convert(float,isnull(prorata+Opening,0)-isnull(SUM(Reimbursed+Taxable),0)-isnull((sum(Claimed)-(sum(Reimbursed+BillPassed+Taxable))),0)) as [BillsTobesubmit],rm.Budget as aBudget from reimbursemaster" & Session("WebTableID") & " RM inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RTM.Name=RM.Field_Name Left join ReimburseTransaction" & Session("WebTableID") & " RT  on RM.ReimID=RT.ReimID Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RM.Field_Name where RM.Emp_Code='" & Session("Emp_Code") & "' and Budget_Wet between '04/01/" & Year & "' and '03/31/" & Year + 1 & "' and isinactive=0 " & strBudgetZeroCondition & " group by  RM.Field_Name,PSS.Print_Name, Budget_Wef ,Budget_Wet ,Opening,prorata,rm.Budget"

            dt1 = OldNewConn.GetDataTable2(str)
            If (dt1.Rows.Count > 0) Then
                radGridSummary.DataSource = dt1
            End If
        End If
    End Sub

    Protected Sub radGridSummary_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles radGridSummary.ItemDataBound
        Dim item As GridDataItem = TryCast(e.Item, GridDataItem)
        If Not IsNothing(item) Then

            Dim BillsTobesubmit = item("BillsTobesubmit").Text

            If (dtOption.Rows.Count > 0) Then
                If (dtOption.Rows(0)("Print_Name").ToString.ToLower = "withoutbudget") Then
                ElseIf (dtOption.Rows(0)("Print_Name").ToString.ToLower = "withfbp") Then
                    If (dtPSS.Rows.Count > 0) Then
                        If (dtPSS.Rows(0)("print_Name").ToString.ToLower = "y") Then
                            If (BillsTobesubmit.ToString().Contains("-")) Then
                                item("BillsTobesubmit").Text = 0
                            End If
                        End If
                    End If
                ElseIf (dtOption.Rows(0)("Print_Name").ToString.ToLower = "with&withoutbudget") Then
                    If (dtPSS.Rows.Count > 0) Then
                        If (dtPSS.Rows(0)("print_Name").ToString.ToLower = "y") Then
                            If (BillsTobesubmit.ToString().Contains("-")) Then
                                item("BillsTobesubmit").Text = 0
                            End If
                        End If
                    End If
                Else
                    If (dtPSS.Rows.Count > 0) Then
                        If (dtPSS.Rows(0)("print_Name").ToString.ToLower = "y") Then
                            If (BillsTobesubmit.ToString().Contains("-")) Then
                                item("BillsTobesubmit").Text = 0
                            End If
                        End If
                    End If
                End If
            Else
                If (dtPSS.Rows.Count > 0) Then
                    If (dtPSS.Rows(0)("print_Name").ToString.ToLower = "y") Then
                        If (BillsTobesubmit.ToString().Contains("-")) Then
                            item("BillsTobesubmit").Text = 0
                        End If
                    End If
                End If
            End If


            Dim hyper1 As LinkButton = DirectCast(item("Edit").FindControl("lnkForViewReport1"), LinkButton)
            hyper1.Attributes.Add("Onclick", String.Format("return openRadWindow1('{0}');", String.Format("Employee/SummaryPage.aspx?Emp_Code=" & Session("Emp_code").ToString() & "&FieldName=" & item.DataItem("Field_Name") & "")))
        End If
    End Sub

    Protected Sub radGridSummary2_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles radGridSummary2.NeedDataSource

        If (dtOption.Rows.Count > 0) Then
            If (dtOption.Rows(0)("Print_Name").ToString.ToLower = "withoutbudget") Then
                Dim str As String

                Dim dtCheckForEntitlementAmount As DataTable = OldNewConn.GetDataTable2("Select print_Name from payslipsetup" & Session("WebTableID") & " where Name ='CheckForEntitlementAmount'")
                Dim str_Condition As String = ""

                If (dtCheckForEntitlementAmount.Rows.Count > 0) Then
                    str_Condition = dtCheckForEntitlementAmount.Rows(0)("print_Name")
                    str_Condition = Replace(str_Condition, "#Emp_code#", Session("Emp_Code"))
                    str_Condition = Replace(str_Condition, "#WebTableID#", Session("WebTableID"))
                    str_Condition = Replace(str_Condition, "#Year#", Year)
                    str_Condition = Replace(str_Condition, "#", "'")

                    str = "Select Distinct ISNULL(PSS.Print_Name,RTM.Name) as 'Field_Name', convert(nvarchar,BillValidStartDate,106) as 'Budget_WEF',convert(nvarchar,BillValidEndDate,106) as 'Budget_WET'," & str_Condition & "  as 'Budget',RTM.ReimburseType_Code,RTM.Name as reimfieldName from ReimburseMentTypeMaster" & Session("WebTableID") & " RTM Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RTM.Name where isinactive=0"
                Else
                    str = "Select Distinct ISNULL(PSS.Print_Name,RTM.Name) as 'Field_Name', convert(nvarchar,BillValidStartDate,106) as 'Budget_WEF',convert(nvarchar,BillValidEndDate,106) as 'Budget_WET','0' as 'Budget',RTM.ReimburseType_Code,RTM.Name as reimfieldName from ReimburseMentTypeMaster" & Session("WebTableID") & " RTM Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RTM.Name where isinactive=0"
                End If

                Dim dt1 As DataTable = OldNewConn.GetDataTable2(str)

                For i As Int16 = 0 To dt1.Rows.Count - 1
                    If Not IsDBNull(dt1.Rows(i)("Budget")) Then
                        If (dt1.Rows(i)("Budget") = 0) Then
                            dt1.Rows(i).Delete()
                        End If
                    End If
                Next

                dt1.AcceptChanges()

                If (dt1.Rows.Count > 0) Then
                    radGridSummary2.DataSource = dt1
                End If
            ElseIf (dtOption.Rows(0)("Print_Name").ToString.ToLower = "withfbp") Then
                Dim dt As DataTable = OldNewConn.GetDataTable2("select ReimburseType_Code,R1.Name,ISNULL(PSS.Print_Name,R1.Name) as 'Field_Name',PSS.Field_Detail,convert(nvarchar,BillValidEndDate,106) as 'Budget_WET' from ReimburseMentTypeMaster" & Session("WebTableID") & " R1 Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =R1.Name where isInactive=0 And R1.Name<>'Deemed LTA' Order by Field_Name")
                Dim dtonlinectcdeclare As DataTable = OldNewConn.GetDataTable2("select * from onlinectcdeclare" & Session("WebTableID") & " where Emp_code='" & Session("Emp_Code") & "'")
                Dim dt1 As DataTable = OldNewConn.GetDataTable2("Select JoiningDate from employeesmaster" & Session("WebTableID") & " where Emp_Code='" & Session("Emp_code").ToString() & "'")

                Dim dtFBPCalc As DataTable = OldNewConn.GetDataTable2("Select print_Name from payslipsetup" & Session("WebTableID") & " where Name ='FBPAnnual'")

                Dim FirstDayofmonth As Date = New Date(DateTime.Now.Year, DateTime.Now.Month, 1)

                If (dtonlinectcdeclare.Rows.Count > 0) Then
                    Dim dtFBP As New DataTable
                    Dim dr As DataRow
                    dtFBP.Columns.Add("Field_Name")
                    dtFBP.Columns.Add("Budget_WEF")
                    dtFBP.Columns.Add("Budget_WET")
                    dtFBP.Columns.Add("Budget")
                    dtFBP.Columns.Add("ReimburseType_Code")
                    dtFBP.Columns.Add("reimfieldName")

                    For element As Int32 = dt.Rows.Count - 1 To 0 Step -1
                        If (dt.Rows.Count > 0) Then
                            If Not IsDBNull(dt.Rows(element)("Field_Detail")) Then
                                If (dtonlinectcdeclare.Rows(0)(dt.Rows(element)("Field_Detail")) >= 0) Then
                                    dr = dtFBP.NewRow()
                                    dr("Field_Name") = dt.Rows(element)("Field_Name")
                                    dr("Budget_WEF") = ""
                                    dr("Budget_WET") = dt.Rows(element)("Budget_WET")

                                    If (dt1.Rows.Count > 0) Then
                                        Dim DOJ As Date = CDate(dt1.Rows(0)("JoiningDate").ToString)
                                        If (dtFBPCalc.Rows.Count > 0) Then
                                            If (dtFBPCalc.Rows(0)("print_Name").ToString.ToLower = "y") Then
                                                If DOJ.Date > FirstDayofmonth.Date Then
                                                    Dim GetProrata As Double = 0
                                                    GetProrata = GetProrata + (CDec(dtonlinectcdeclare.Rows(0)(dt.Rows(element)("Field_Detail")) / 12) * (Date.DaysInMonth(FirstDayofmonth.Year, FirstDayofmonth.Month) - (DOJ.Day - 1)))
                                                    GetProrata = GetProrata / (Date.DaysInMonth(FirstDayofmonth.Year, FirstDayofmonth.Month))
                                                    dr("Budget") = GetProrata.ToString("###0")
                                                Else
                                                    Dim GetProrata As Double = 0
                                                    GetProrata = CDec(dtonlinectcdeclare.Rows(0)(dt.Rows(element)("Field_Detail"))) / 12
                                                    GetProrata = GetProrata * LeftMonth2(System.DateTime.Now.Month)
                                                    dr("Budget") = GetProrata.ToString("###0")
                                                End If
                                            End If
                                        Else
                                            If DOJ.Date > FirstDayofmonth.Date Then
                                                Dim GetProrata As Double = 0
                                                GetProrata = GetProrata + (CDec(dtonlinectcdeclare.Rows(0)(dt.Rows(element)("Field_Detail"))) * (Date.DaysInMonth(FirstDayofmonth.Year, FirstDayofmonth.Month) - (DOJ.Day - 1)))
                                                GetProrata = GetProrata / (Date.DaysInMonth(FirstDayofmonth.Year, FirstDayofmonth.Month))
                                                dr("Budget") = GetProrata.ToString("###0")
                                            Else
                                                dr("Budget") = CDec(dtonlinectcdeclare.Rows(0)(dt.Rows(element)("Field_Detail")) * LeftMonth2(System.DateTime.Now.Month)).ToString("###0")
                                            End If
                                        End If
                                    End If
                                    dr("ReimburseType_Code") = dt.Rows(element)("Field_Name")
                                    dr("reimfieldName") = ""
                                    dtFBP.Rows.Add(dr)
                                End If
                            End If
                        End If
                    Next
                    If (dtFBP.Rows.Count > 0) Then
                        radGridSummary2.Visible = True
                        dtFBP.DefaultView.Sort = "Field_Name"
                        radGridSummary2.DataSource = dtFBP
                        div2.Visible = True
                    Else
                        radGridSummary2.Visible = False
                    End If
                Else
                    radGridSummary2.Visible = False
                End If
                If (radGridSummary.Visible = False And radGridSummary2.Visible = False) Then
                    Label1.Visible = True
                    Label1.Text = "You have not opted for any reimbursment component in salary structure/FBP declaration"
                End If
            ElseIf (dtOption.Rows(0)("Print_Name").ToString.ToLower = "with&withoutbudget") Then
                Dim str As String

                Dim dtCheckForEntitlementAmount As DataTable = OldNewConn.GetDataTable2("Select print_Name from payslipsetup" & Session("WebTableID") & " where Name ='CheckForEntitlementAmount'")
                Dim str_Condition As String = ""

                If (dtCheckForEntitlementAmount.Rows.Count > 0) Then
                    str_Condition = dtCheckForEntitlementAmount.Rows(0)("print_Name")
                    str_Condition = Replace(str_Condition, "#Emp_code#", Session("Emp_Code"))
                    str_Condition = Replace(str_Condition, "#WebTableID#", Session("WebTableID"))
                    str_Condition = Replace(str_Condition, "#Year#", Year)
                    str_Condition = Replace(str_Condition, "#", "'")

                    str = "Select Distinct ISNULL(PSS.Print_Name,RTM.Name) as 'Field_Name', convert(nvarchar,BillValidStartDate,106) as 'Budget_WEF',convert(nvarchar,BillValidEndDate,106) as 'Budget_WET'," & str_Condition & "  as 'Budget','' as ReimburseType_Code,'' as reimfieldName from ReimburseMentTypeMaster" & Session("WebTableID") & " RTM Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RTM.Name where isinactive=0"
                Else
                    str = "Select Distinct ISNULL(PSS.Print_Name,RTM.Name) as 'Field_Name', convert(nvarchar,BillValidStartDate,106) as 'Budget_WEF',convert(nvarchar,BillValidEndDate,106) as 'Budget_WET','0' as 'Budget','' as ReimburseType_Code,'' as reimfieldName from ReimburseMentTypeMaster" & Session("WebTableID") & " RTM Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RTM.Name where isinactive=0"
                End If

                Dim dt1 As DataTable = OldNewConn.GetDataTable2(str)

                For i As Int16 = 0 To dt1.Rows.Count - 1
                    If Not IsDBNull(dt1.Rows(i)("Budget")) Then
                        If (dt1.Rows(i)("Budget") = 0) Then
                            dt1.Rows(i).Delete()
                        End If
                    End If
                Next

                dt1.AcceptChanges()

                If (dt1.Rows.Count > 0) Then
                    radGridSummary2.DataSource = dt1
                End If
            End If
        End If
    End Sub

    Protected Sub radGridSummary2_PreRender(ByVal sender As Object, ByVal e As EventArgs) Handles radGridSummary2.PreRender
        If Not Page.IsPostBack Then

            Dim values As Int32
            For Each r As GridDataItem In radGridSummary2.MasterTableView.Items
                If (r("Budget").Text <> "") And (r("Budget").Text <> "&nbsp;") Then
                    values = values + r("Budget").Text
                End If
            Next

            For Each itemnew In radGridSummary2.MasterTableView.Items
                If (itemnew("Budget").Text = "0") Or (itemnew("Budget").Text = "&nbsp;") Then
                    itemnew.Display = False
                End If
            Next

            If (values = 0) Then
                radGridSummary2.Visible = False
                div2.Visible = False
            End If
        End If
    End Sub

    Protected Sub radGridSummary2_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles radGridSummary2.ItemDataBound
        Dim item As GridDataItem = TryCast(e.Item, GridDataItem)
        If Not IsNothing(item) Then

            If (dtOption.Rows.Count > 0) Then

                Dim FromDate As New Date(DateTime.Now.Year, DateTime.Now.Month, 1)

                If FromDate.Month > 0 And FromDate.Month < 4 Then
                    FromDate = New Date(DateTime.Now.Year - 1, 4, 1)
                Else
                    FromDate = New Date(DateTime.Now.Year, 4, 1)
                End If

                If (dtOption.Rows(0)("Print_Name").ToString.ToLower = "withoutbudget") Then
                    Dim Str As String = "Select JoiningDate from employeesmaster" & Session("WebTableID") & " where Emp_Code='" & Session("Emp_code").ToString() & "'"
                    Dim dt1 As DataTable = OldNewConn.GetDataTable2(Str)
                    If (dt1.Rows.Count > 0) Then
                        Dim DOJ As Date = CDate(dt1.Rows(0)("JoiningDate").ToString)

                        If DOJ.Date >= FromDate.Date Then
                            item("Budget_WEF").Text = DOJ.ToString("dd MMM yyyy")
                        End If
                    End If
                    If Session("Reimbursementdeclaration") = "Yes" Then
                        Try
                            Dim hidreimtypecode As HiddenField = DirectCast(item("reimdeclartionamt").FindControl("hidreimtypecode"), HiddenField)
                            Dim hidreimfieldName As HiddenField = DirectCast(item("reimdeclartionamt").FindControl("hidreimfieldName"), HiddenField)
                            Dim txtdeclaramt As TextBox = DirectCast(item("reimdeclartionamt").FindControl("txtdeclaramt"), TextBox)
                            Dim comm As String = "select * from Reimbursementdeclaration" & Session("WebTableID") & "_" & Year & " where Emp_code='" & Session("Emp_Code") & "' and ReimburseType_Code=" & hidreimtypecode.Value & ""
                            Dim dt As DataTable = OldNewConn.GetDataTable2(comm)
                            If dt.Rows.Count > 0 Then
                                txtdeclaramt.Text = Convert.ToInt64(dt.Rows(0)("DeclarationAmount"))
                            End If

                            comm = "select * from payslipsetup" & Session("WebTableID") & " where Name='ReimDeclaredisable'"
                            dt = OldNewConn.GetDataTable2(comm)
                            If dt.Rows.Count > 0 Then
                                If Not String.IsNullOrEmpty(dt.Rows(0)("Print_Name").ToString) Then
                                    Dim fieldspllit() As String = dt.Rows(0)("Print_Name").ToString().Split("#")
                                    If fieldspllit.Length > 0 Then
                                        For i As Integer = 1 To fieldspllit.Length - 2
                                            Dim Field_name As String = fieldspllit(i).ToString
                                            If Field_name.ToUpper = hidreimfieldName.Value.ToUpper Then
                                                txtdeclaramt.Enabled = False
                                                txtdeclaramt.Style.Add("border", "none !important")
                                            End If
                                        Next
                                    End If

                                End If
                            End If
                            If Not IsNothing(Session("Reimbursementdeclarationstatus")) Then
                                If Session("Reimbursementdeclarationstatus") = "closed" Then
                                    txtdeclaramt.Enabled = False
                                    txtdeclaramt.Style.Add("border", "none !important")
                                End If
                            End If
                        Catch ex As Exception

                        End Try

                    End If

                ElseIf (dtOption.Rows(0)("Print_Name").ToString.ToLower = "withfbp") Then

                    Dim Str As String = "Select JoiningDate from employeesmaster" & Session("WebTableID") & " where Emp_Code='" & Session("Emp_code").ToString() & "'"
                    Dim dt1 As DataTable = OldNewConn.GetDataTable2(Str)
                    Dim FirstDayofmonth As Date = New Date(DateTime.Now.Year, DateTime.Now.Month, 1)

                    Dim Budgettext As Decimal = CDec(item("Budget").Text)

                    For Each item2 In radGridSummary.Items
                        If Not IsNothing(item2) Then

                            Dim lnkAbudget As LinkButton = TryCast(item2.FindControl("lnkAbudget"), LinkButton)

                            Dim item2text As String = item2("Field_Name").Text.ToString.ToLower
                            Dim itemtext As String = item("Field_Name").Text.ToString.ToLower

                            If (item2("Field_Name").Text.ToString.ToLower = item("Field_Name").Text.ToString.ToLower) Then
                                If (CDec(item2("Budget").Text.ToString) <> CDec(item("Budget").Text.ToString)) Then

                                    If (dt1.Rows.Count > 0) Then
                                        Dim DOJ As Date = CDate(dt1.Rows(0)("JoiningDate").ToString)
                                        If DOJ.Date > FirstDayofmonth.Date Then
                                            Dim GetProrata As Double = 0
                                            GetProrata = GetProrata + ((CDec(item2("Budget").Text.ToString) / 12) * (Date.DaysInMonth(FirstDayofmonth.Year, FirstDayofmonth.Month) - (DOJ.Day - 1))) / (Date.DaysInMonth(FirstDayofmonth.Year, FirstDayofmonth.Month))
                                            item("Budget").Text = ((CDec(item("Budget").Text.ToString) - GetProrata)).ToString("###0")
                                        Else

                                            Dim itemBudget As Decimal = CDec(item("Budget").Text)
                                            Dim item2Budget As Decimal = CDec(lnkAbudget.Text)
                                            Dim item3Budget As Decimal = CDec(lnkAbudget.Text) / 12
                                            Dim item4Budget As Decimal = LeftMonth2(System.DateTime.Now.Month)
                                            Dim item5Budget As Decimal = item3Budget * item4Budget
                                            item("Budget").Text = ((Budgettext - ((CDec(lnkAbudget.Text.ToString) / 12) * LeftMonth2(System.DateTime.Now.Month)))).ToString("###0")
                                            'item("Budget").Text = ((CDec(item("Budget").Text.ToString) - ((CDec(lnkAbudget.Text.ToString) / 12) * LeftMonth2(System.DateTime.Now.Month)))).ToString("###0")
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    Next

                    If (dt1.Rows.Count > 0) Then
                        If Not IsDBNull(dt1.Rows(0)("JoiningDate")) Then
                            If (CDate(dt1.Rows(0)("JoiningDate")).Date >= FirstDayofmonth.Date) Then
                                Dim DOJ As Date = CDate(dt1.Rows(0)("JoiningDate").ToString)
                                item("Budget_WEF").Text = DOJ.ToString("dd MMM yyyy")
                            Else
                                item("Budget_WEF").Text = New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("dd MMM yyyy")
                            End If
                        End If
                    End If
                ElseIf (dtOption.Rows(0)("Print_Name").ToString.ToLower = "with&withoutbudget") Then
                    Dim Str As String = "Select JoiningDate from employeesmaster" & Session("WebTableID") & " where Emp_Code='" & Session("Emp_code").ToString() & "'"
                    Dim dt1 As DataTable = OldNewConn.GetDataTable2(Str)
                    If (dt1.Rows.Count > 0) Then
                        Dim DOJ As Date = CDate(dt1.Rows(0)("JoiningDate").ToString)

                        If DOJ.Date >= FromDate.Date Then
                            item("Budget_WEF").Text = DOJ.ToString("dd MMM yyyy")
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Sql Execution ADO
    ''' </summary>
    ''' <param name="connString"></param>
    ''' <param name="cmdType"></param>
    ''' <param name="cmdText"></param>
    ''' <param name="cmdParms"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Friend Function LeftMonth2(ByVal PayMonth As Int16) As Integer

        LeftMonth2 = 0
        If PayMonth < 4 Then
            LeftMonth2 = PayMonth - 4
        Else
            LeftMonth2 = 12 - (PayMonth - 4)
        End If
        LeftMonth2 = Math.Abs(LeftMonth2)

    End Function


    Protected Sub btnreimdeclaration_Click(sender As Object, e As EventArgs)



        For Each item As GridDataItem In radGridSummary2.Items
            Dim txtdeclaramt As TextBox = TryCast(item.FindControl("txtdeclaramt"), TextBox)
            Dim oldamt As Integer = 0
            oldamt = Convert.ToInt64(item("Budget").Text)
            Dim declareamt As Integer = 0
            declareamt = Convert.ToInt64(txtdeclaramt.Text)
            If declareamt > oldamt Then
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Declaration amount should not be greater than entitlement amount.');", True)
                txtdeclaramt.Text = "0"
                txtdeclaramt.Focus()
                Return
            End If
        Next


        Dim updatecount As Integer = 0
        For Each item As GridDataItem In radGridSummary2.Items
            Dim txtdeclaramt As TextBox = TryCast(item.FindControl("txtdeclaramt"), TextBox)
            Dim hidreimtypecode As HiddenField = TryCast(item.FindControl("hidreimtypecode"), HiddenField)

            Dim query1 As String = ""
            Dim query2 As String = ""
            Dim comm As String = "select * from Reimbursementdeclaration" & Session("WebTableID") & "_" & Year & " where Emp_code='" & Session("Emp_Code") & "' and ReimburseType_Code=" & hidreimtypecode.Value & ""
            Dim dt As DataTable = OldNewConn.GetDataTable2(comm)
            If dt.Rows.Count > 0 Then
                query1 = "update Reimbursementdeclaration" & Session("WebTableID") & "_" & Year & " set DeclarationAmount=" & txtdeclaramt.Text & ", ModifiedDate='" & DateTime.Now.ToString("MM-dd-yyyy HH:mm") & "', ModifiedBy='" & Session("Emp_Code") & "' where Emp_code='" & Session("Emp_Code") & "' and ReimburseType_Code=" & hidreimtypecode.Value & ""
                query2 = "insert into ReimbursementdeclarationLog" & Session("WebTableID") & "_" & Year & "(Emp_Code,ReimburseType_Code,DeclarationAmount,DeclareDate,DeclareBy,ModifiedDate,ModifiedBy)values('" & Session("Emp_Code") & "','" & hidreimtypecode.Value & "'," & txtdeclaramt.Text & ",'" & Convert.ToDateTime(dt.Rows(0)("DeclareDate")).ToString("MM-dd-yyyy HH:mm") & "','" & dt.Rows(0)("DeclareBy").ToString & "','" & DateTime.Now.ToString("MM-dd-yyyy HH:mm") & "','" & Session("Emp_Code") & "')"

            Else
                query1 = "insert into Reimbursementdeclaration" & Session("WebTableID") & "_" & Year & "(Emp_Code,ReimburseType_Code,DeclarationAmount,DeclareDate,DeclareBy) values('" & Session("Emp_Code") & "','" & hidreimtypecode.Value & "'," & txtdeclaramt.Text & ",'" & DateTime.Now.ToString("MM-dd-yyyy HH:mm") & "','" & Session("Emp_Code") & "')"
                query2 = "insert into ReimbursementdeclarationLog" & Session("WebTableID") & "_" & Year & "(Emp_Code,ReimburseType_Code,DeclarationAmount,DeclareDate,DeclareBy) values('" & Session("Emp_Code") & "','" & hidreimtypecode.Value & "'," & txtdeclaramt.Text & ",'" & DateTime.Now.ToString("MM-dd-yyyy HH:mm") & "','" & Session("Emp_Code") & "')"
            End If
            Dim val = OldNewConn.ExecuteNonQuery(CommandType.Text, query1, Nothing)
            Dim val2 = OldNewConn.ExecuteNonQuery(CommandType.Text, query2, Nothing)
            If val > 0 And val2 > 0 Then
                updatecount = updatecount + 1
            End If
        Next
        If updatecount > 0 Then

            Dim comm As String = "select * from EmployeesMaster" & Session("WebTableID") & " where Emp_code='" & Session("Emp_Code") & "'"
            Dim empdt As DataTable = OldNewConn.GetDataTable2(comm)

            Dim mailflag As String = ""
            Dim MailSubject As String = ""
            Dim mailbody As String = ""
            Dim MailTo As String = ""
            Dim MailCc As String = ""
            comm = "select * from payslipsetup" & Session("WebTableID") & " where Name in('ReimbursementDeclarationMailSubject','ReimbursementDeclarationMailbody')"
            Dim dtMaiBody As DataTable = OldNewConn.GetDataTable2(comm)
            If dtMaiBody.Rows.Count > 0 And empdt.Rows.Count > 0 Then

                For Each dr As DataRow In dtMaiBody.Rows
                    If dr("Name").ToString.ToLower() = "reimbursementdeclarationmailsubject" Then
                        If Not String.IsNullOrEmpty(dr("print_name").ToString()) Then
                            MailSubject = dr("print_name").ToString()
                        End If
                    ElseIf dr("Name").ToString.ToLower() = "reimbursementdeclarationmailbody" Then
                        If Not String.IsNullOrEmpty(dr("print_name").ToString()) Then
                            mailbody = dr("print_name").ToString()
                        End If
                    End If
                Next
                If Not String.IsNullOrEmpty(empdt.Rows(0)("Email").ToString) Then
                    MailTo = empdt.Rows(0)("Email").ToString

                    mailbody = Replace(mailbody, "#EmployeeName#", empdt.Rows(0)("FirstName").ToString.ToUpper)
                    mailbody = Replace(mailbody, "#EmployeeID#", Session("Emp_Code").ToString.ToUpper)
                    'mailbody = Replace(mailbody, "#EmployeeName#", EmpNamelbl.Text.ToString)
                    Try
                        Dim List As List(Of String) = New List(Of String)
                        Dim mailsend As String = mailclass.SendEmail(MailTo, MailSubject, mailbody, "", "", List, "from5", "subject5", "password5")
                        If mailflag = "1" Then
                            mailclass.Enter_EmailLog(Session("Emp_Code"), Session("WebTableID"), 1, "DKMWebApplication", MailSubject, "To : " & MailTo, mailbody, "", Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type)
                        Else
                            mailclass.Enter_EmailLog(Session("Emp_Code"), Session("WebTableID"), 0, "DKMWebApplication", MailSubject, "To : " & MailTo, mailbody, mailsend, Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type)
                        End If

                    Catch ex As Exception

                    End Try
                End If
            End If
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Declaration Amount has been updated.');", True)
        Else
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Error occured while saving data. Please contact your system administrator.');", True)
        End If
    End Sub
End Class