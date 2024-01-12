Imports Telerik.Web.UI
Imports DkmOnline.Lib
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports DkmOnline.Common


Public Class OtherReimbursementPersonalDetail
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
            If Not (IsPostBack) Then
                bindDropDown()
                bindControls()
            End If
        Else
            Response.Redirect("../Logout.aspx")
        End If

    End Sub

    Private Sub bindControls()
        If Not IsNothing(Session("ReimbursetableTravel")) Then
            If Not IsNothing(Request.QueryString("SNo")) Then
                Dim dt As DataTable = DirectCast(Session("ReimbursetableTravel"), DataTable)
                Dim dr As DataRow = dt.Select("Sl.No = '" & Request.QueryString("SNo") & "'")(0)
                txtTravellerName.Text = dr("PersonsName")
                ddlDependence.SelectedValue = ddlDependence.Items.FindByText(dr("Dependence")).Value
                ddlRelation.SelectedValue = ddlRelation.Items.FindByText(dr("RelationshipwithEmployee")).Value
                txtFrom.Text = If(IsDBNull(dr("From")), "", dr("From"))
                txtTodate.Text = If(IsDBNull(dr("To")), "", dr("From"))
                txtJourney.Text = dr("ClaimAmount")
                ddlMode.SelectedValue = ddlMode.Items.FindByText(dr("ModeOfJourney")).Value
                ddlType.SelectedValue = ddlType.Items.FindByText(dr("TypeOfJourney")).Value
                RadDatePicker.DbSelectedDate = dr("JourneyDate")
            End If
        End If
    End Sub

    Private Sub bindDropDown()
        ddlRelation.DataSource = listOfRelation()
        ddlRelation.DataBind()

        ddlMode.DataSource = listOfModeOfJourney()
        ddlMode.DataBind()

        ddlDependence.DataSource = listOfDependency()
        ddlDependence.DataBind()

        ddlType.DataSource = listOfTypeOfJourney()
        ddlType.DataBind()

    End Sub

    Private Function listOfDependency() As List(Of String)
        Dim list As New List(Of String)
        list.Add("Please Select")
        list.Add("Y")
        list.Add("N")
        Return list
    End Function

    Private Function listOfTypeOfJourney() As List(Of String)
        Dim list As New List(Of String)
        list.Add("Please Select")
        list.Add("Inward")
        list.Add("Outward")
        list.Add("Inward & Outward")
        Return list
    End Function

    Private Function listOfModeOfJourney() As List(Of String)
        Dim list As New List(Of String)
        list.Add("Please Select")
        list.Add("Air")
        list.Add("Train")
        list.Add("Taxi")
        list.Add("Other")
        Return list
    End Function

    Private Function listOfRelation() As List(Of String)
        Dim list As New List(Of String)
        list.Add("Please Select")
        list.Add("Self")
        list.Add("Spouse")
        list.Add("Father")
        list.Add("Mother")
        list.Add("Brother")
        list.Add("Sister")
        list.Add("Son")
        list.Add("Daughter")
        Return list
    End Function

    Protected Sub ddlRelation_SelectedIndexChanged(sender As Object, e As EventArgs)
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Try
            Dim slNo As Integer = 0
            Dim dt As DataTable

            If (txtTravellerName.Text = "") Then
                lblError.Text = "Please Enter Name of Traveller"
                Exit Sub
            End If



            If (ddlRelation.SelectedItem.Text = "Please Select") Then
                lblError.Text = "Please Select Relationship with Employee"
                Exit Sub
            End If

            If (ddlDependence.SelectedItem.Text = "Please Select") Then
                lblError.Text = "Please Select Dependent (Y/N)"
                Exit Sub
            End If

            If (ddlMode.SelectedItem.Text = "Please Select") Then
                lblError.Text = "Please Select Mode of Journey"
                Exit Sub
            End If

            If (ddlType.SelectedItem.Text = "Please Select") Then
                lblError.Text = "Please Select Type of Journey"
                Exit Sub
            End If
          
            If (ddlRelation.SelectedItem.Text = "Father" Or ddlRelation.SelectedItem.Text = "Mother" Or ddlRelation.SelectedItem.Text = "Brother" Or ddlRelation.SelectedItem.Text = "Sister") Then
                If (ddlDependence.SelectedItem.Text = "N") Then
                    ddlDependence.ClearSelection()
                    lblError.Text = "LTA can be claimed for Father, Mother, Brother and Sister only if they are dependent on you"
                    Exit Sub
                End If
            End If


            If Not IsNothing(Session("ReimbursetableTravel")) Then
                dt = DirectCast(Session("ReimbursetableTravel"), DataTable)
            Else
                dt = New DataTable
                dt.Columns.Add("Sl.No")
                dt.Columns.Add("PersonsName")
                dt.Columns.Add("Dependence")
                dt.Columns.Add("RelationshipWithEmployee")
                dt.Columns.Add("From")
                dt.Columns.Add("To")
                dt.Columns.Add("JourneyDate")
                dt.Columns.Add("ModeOfJourney")
                dt.Columns.Add("TypeOfJourney")
                dt.Columns.Add("ClaimAmount")
            End If

            If Not IsNothing(Request.QueryString("SNo")) Then
                slNo = Request.QueryString("SNo")

                Dim dr As DataRow = dt.Select("Sl.No = '" & Request.QueryString("SNo") & "'")(0)

                dr("PersonsName") = Replace(txtTravellerName.Text, "'", "")
                dr("Dependence") = ddlDependence.SelectedItem.Text
                dr("RelationshipwithEmployee") = ddlRelation.SelectedItem.Text
                dr("From") = Replace(txtFrom.Text, "'", "")
                dr("To") = Replace(txtTodate.Text, "'", "")

                Dim JourneyDate As Date = radDatePicker.SelectedDate.Value.Date
                Dim dtJourneyDate As Date = radDatePicker.SelectedDate.Value.Date
                Dim d As Date
                Try
                    If Date.TryParseExact(JourneyDate.Date.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                        dtJourneyDate = d
                    Else
                        Response.Write("<script>alert('Error : Journey date is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                        Exit Sub
                    End If
                Catch ex As Threading.ThreadAbortException
                Catch ex As Exception
                    Response.Write("<script>alert('Error : Journey date is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                    Exit Sub
                End Try

                dr("JourneyDate") = dtJourneyDate.Date
                dr("ModeOfJourney") = ddlMode.SelectedItem.Text
                dr("typeOfJourney") = ddlType.SelectedItem.Text
                dr("ClaimAmount") = txtJourney.Text
            Else

                Dim dr As DataRow = dt.NewRow()

                If (dt.Rows.Count = 0) Then
                    dr("Sl.No") = 1
                    slNo = 1
                Else
                    dr("Sl.No") = dt.Rows(dt.Rows.Count - 1)("Sl.No") + 1
                    slNo = dt.Rows(dt.Rows.Count - 1)("Sl.No") + 1
                End If

                dr("PersonsName") = Replace(txtTravellerName.Text, "'", "")

                If ddlRelation.SelectedItem.Text = "Self" Then
                    dr("Dependence") = "N"
                Else
                    dr("Dependence") = ddlDependence.SelectedItem.Text
                End If

                dr("RelationshipwithEmployee") = ddlRelation.SelectedItem.Text
                dr("From") = Replace(txtFrom.Text, "'", "")
                dr("To") = Replace(txtTodate.Text, "'", "")

                Dim JourneyDate As Date = radDatePicker.SelectedDate.Value.Date
                Dim dtJourneyDate As Date = radDatePicker.SelectedDate.Value.Date

                Dim d As Date
                Try
                    If Date.TryParseExact(JourneyDate.Date.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                        dtJourneyDate = d
                    Else
                        Response.Write("<script>alert('Error : Journey date is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                        Exit Sub
                    End If
                Catch ex As Threading.ThreadAbortException
                Catch ex As Exception
                    Response.Write("<script>alert('Error : Journey date is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                    Exit Sub
                End Try

                dr("JourneyDate") = dtJourneyDate.Date
                dr("ModeOfJourney") = ddlMode.SelectedItem.Text
                dr("TypeOfJourney") = ddlType.SelectedItem.Text
                dr("ClaimAmount") = txtJourney.Text

                dt.Rows.Add(dr)
            End If
            Session("ReimbursetableTravel") = dt
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", "Clo1();", True)
        Catch ex As Exception

        End Try
    End Sub
End Class