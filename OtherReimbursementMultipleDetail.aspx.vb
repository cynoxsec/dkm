Imports Telerik.Web.UI
Imports DkmOnline.Lib
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports DkmOnline.Common


Public Class OtherReimbursementMultipleDetail
    Inherits System.Web.UI.Page

    Dim dicUploadFile As Dictionary(Of Integer, Byte())
    Dim dtUpload As DataTable
    Dim dtValidDates As DataTable
    Dim OldNewConn As OldNewConnection = New OldNewConnection()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If IsNothing(Session("Emp_Code")) Then
            Response.Redirect("../Logout.aspx")
        End If
        DatabaseFacade.Instance.Enter_SiteLog(Session("Emp_Code"), Session("LoginType"), Session("LoginAccessType"), Session("Comp_Code"), 0, Request.Url.AbsolutePath, Request.Url.AbsolutePath, Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type, Session("WebTableID"), Session.SessionID, Session("MenuID"))

        OldNewConn.CheckDatabaseOldNew(Session("WebTableID"))

        If Not IsNothing(Session("Emp_Code")) And Not IsNothing(Session("WebTableID")) And Not IsNothing(Session("MenuID")) Then

            Title = "Add Reimbursement Details Manually"

            If Not (IsPostBack) Then
                If Not IsNothing(Session("IsdriverorBroadband")) Then
                    If (Session("IsdriverorBroadband")) Then
                        txtBillDetail.Visible = False
                        lblBillDetail.Text = "Bill for the month"
                        ddlBillDetail.Visible = True
                        ddlBillDetail.DataSource = ReturnMonths()
                        ddlBillDetail.DataBind()
                    End If
                Else
                    txtBillDetail.Visible = True
                    ddlBillDetail.Visible = False
                End If
                bindControls()
            End If
        Else
            Response.Redirect("../Logout.aspx")
        End If
    End Sub

    Private Function ReturnMonths() As List(Of String)
        Dim listOfMonth As New List(Of String)
        listOfMonth.Add("January")
        listOfMonth.Add("Febuary")
        listOfMonth.Add("March")
        listOfMonth.Add("April")
        listOfMonth.Add("May")
        listOfMonth.Add("June")
        listOfMonth.Add("July")
        listOfMonth.Add("August")
        listOfMonth.Add("September")
        listOfMonth.Add("October")
        listOfMonth.Add("November")
        listOfMonth.Add("December")

        Return listOfMonth
    End Function

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Try
            Dim slNo As Integer = 0
            Dim dt As DataTable

            If Not IsNothing(Session("ReimbursetableMultiple")) Then
                dt = DirectCast(Session("ReimbursetableMultiple"), DataTable)
            Else
                dt = New DataTable
                dt.Columns.Add("Sl.No")
                dt.Columns.Add("BillNumber")
                dt.Columns.Add("BillDated")
                dt.Columns.Add("BillAmount")
                dt.Columns.Add("BillDetail")

            End If

            txtBillDetail.Text = txtBillDetail.Text.Replace("'", "")

            If Not IsNothing(Request.QueryString("SNo")) Then
                Dim dr As DataRow = dt.Select("Sl.No = '" & Request.QueryString("SNo") & "'")(0)

                slNo = Request.QueryString("SNo")

                dr("BillNumber") = Replace(txtBillNo.Text, "'", "")

                Dim BillDated As Date = radDatePickerBillDate.SelectedDate.Value.Date
                Dim dtBillDated As Date = radDatePickerBillDate.SelectedDate.Value.Date

                Dim d As Date
                Try
                    If Date.TryParseExact(BillDated.Date.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                        dtBillDated = d
                    Else
                        Me.lblError.Text = "Error : Bill date is not valid , please enter the date in DD/MM/YYYY format!"
                        Exit Sub
                    End If
                Catch ex As Threading.ThreadAbortException
                Catch ex As Exception
                    Me.lblError.Text = "Error : Bill date is not valid , please enter the date in DD/MM/YYYY format!"
                    Exit Sub
                End Try

                dr("BillDated") = dtBillDated.Date

                dr("BillAmount") = txtBillAmt.Text

                If Not IsNothing(Session("IsdriverorBroadband")) Then
                    If (Session("IsdriverorBroadband")) Then
                        dr("BillDetail") = ddlBillDetail.SelectedItem.Text
                    End If
                Else
                    dr("BillDetail") = Replace(txtBillDetail.Text, "'", "")
                End If
            Else
                Dim dr As DataRow = dt.NewRow()

                If (dt.Rows.Count = 0) Then
                    dr("Sl.No") = 1
                    slNo = 1
                Else
                    dr("Sl.No") = dt.Rows(dt.Rows.Count - 1)("Sl.No") + 1
                    slNo = dt.Rows(dt.Rows.Count - 1)("Sl.No") + 1
                End If

                dr("BillNumber") = Replace(txtBillNo.Text, "'", "")

                Dim BillDated As Date = radDatePickerBillDate.SelectedDate.Value.Date
                Dim dtBillDated As Date = radDatePickerBillDate.SelectedDate.Value.Date

                Dim d As Date
                Try
                    If Date.TryParseExact(BillDated.Date.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                        dtBillDated = d
                    Else
                        Me.lblError.Text = "Error : Bill date is not valid , please enter the date in DD/MM/YYYY format!"
                        Exit Sub
                    End If
                Catch ex As Threading.ThreadAbortException
                Catch ex As Exception
                    Me.lblError.Text = "Error : Bill date is not valid , please enter the date in DD/MM/YYYY format!"
                    Exit Sub
                End Try

                dr("BillDated") = dtBillDated.Date

                dr("BillAmount") = txtBillAmt.Text

                If Not IsNothing(Session("IsdriverorBroadband")) Then
                    If (Session("IsdriverorBroadband")) Then
                        dr("BillDetail") = ddlBillDetail.SelectedItem.Text
                    End If
                Else
                    dr("BillDetail") = Replace(txtBillDetail.Text, "'", "")
                End If

                dt.Rows.Add(dr)
            End If

            Session("ReimbursetableMultiple") = dt
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", "Clo1();", True)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub bindControls()
        If Not IsNothing(Session("ReimbursetableMultiple")) Then
            If Not IsNothing(Request.QueryString("SNo")) Then
                Dim dt As DataTable = DirectCast(Session("ReimbursetableMultiple"), DataTable)

                Dim dr As DataRow = dt.Select("Sl.No = '" & Request.QueryString("SNo") & "'")(0)

                txtBillNo.Text = dr("BillNumber")

                radDatePickerBillDate.DbSelectedDate = dr("BillDated")

                'dtValidDates = GetDataTable1("select BillValidStartDate,BillValidEndDate,ReimbursePolicy,Name from ReimburseMentTypeMaster" & Session("WebTableID") & " where Name='" & Request.QueryString("Type_Code").ToString() & "'")

                'If DateTime.ParseExact(CDate(dr("BillDated").ToString).ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) < Convert.ToDateTime(dtValidDates.Rows(0)("BillValidStartDate")).Date Or DateTime.ParseExact(CDate(dr("BillDated").ToString).ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) > Convert.ToDateTime(dtValidDates.Rows(0)("BillValidEndDate")).Date Then
                '    btnSave.Visible = False
                '    messageLabel1.Text = "Bill date should be between " & Convert.ToDateTime(dtValidDates.Rows(0)("BillValidStartDate")).Date & " to " & Convert.ToDateTime(dtValidDates.Rows(0)("BillValidEndDate")).Date & "."
                '    messageLabel1.Visible = True
                '    Exit Sub
                'Else
                '    btnSave.Visible = True
                '    messageLabel1.Visible = False
                'End If

                btnSave.Visible = True
                messageLabel1.Visible = False

                txtBillAmt.Text = dr("BillAmount")

                If Not IsNothing(Session("IsdriverorBroadband")) Then
                    If (Session("IsdriverorBroadband")) Then
                        If Not IsNothing(ddlBillDetail.Items.FindByText(dr("BillDetail"))) Then
                            ddlBillDetail.Items.FindByText(dr("BillDetail")).Selected = True
                        End If
                    End If
                Else
                    txtBillDetail.Text = If(IsDBNull(dr("BillDetail")), "", dr("BillDetail"))
                End If

            End If
        End If
    End Sub
End Class