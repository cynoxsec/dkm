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

Public Class ReimbursementLTA
    Inherits System.Web.UI.Page

    Dim IsEntrySaved As Boolean = False
    Dim constantValuesnew As New ConstantValues
    Dim type As String
    Public Property showPersonalDetails As Boolean
    Public Property IsPersonalDetails As Boolean
    Public ReimbursementID As Integer
    Dim ID As Integer
    Public ReimbursementType_Code As Integer
    Public ReimbursementTypeID As Integer
    Dim dtValidDates As DataTable
    Dim dtUpload As DataTable
    Dim Code As String = ""
    Dim commendText As String
    Dim dtOption As New DataTable
    Dim OldNewConn As OldNewConnection = New OldNewConnection()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If IsNothing(Session("Emp_Code")) Then
            Response.Redirect("../Logout.aspx")
        End If

        DatabaseFacade.Instance.Enter_SiteLog(Session("Emp_Code"), Session("LoginType"), Session("LoginAccessType"), Session("Comp_Code"), 0, Request.Url.AbsolutePath, Request.Url.AbsolutePath, Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type, Session("WebTableID"), Session.SessionID, Session("MenuID"))

        OldNewConn.CheckDatabaseOldNew(Session("WebTableID"))

        If Not IsNothing(Session("Emp_Code")) And Not IsNothing(Session("WebTableID")) Then

            lblError.Text = ""

            If Not IsNothing(Request.QueryString("Type")) Then
                Dim s As String = Request.QueryString("Type").Replace(" ", "+")

                If s.Length Mod 4 > 0 Then
                    s = s.PadRight(s.Length + 4 - s.Length Mod 4, "="c)
                End If

                Dim reimburseType As String = EncryDecrypt.Decrypt(s, "a")

                s = Request.QueryString("IDD").Replace(" ", "+")
                If s.Length Mod 4 > 0 Then
                    s = s.PadRight(s.Length + 4 - s.Length Mod 4, "="c)
                End If
                Dim reimburseTypeValue As String = EncryDecrypt.Decrypt(s, "a")
                type = reimburseType
                ReimbursementTypeID = reimburseTypeValue
            End If

            radGridTravel.MasterTableView.GetColumn("ReimbursementPersonsDetailID").Display = False

            dtOption = OldNewConn.GetDataTable2("Select Print_Name from PayslipSetup" & Session("WebTableID") & " where Name ='ReimbursementOption'")

            Dim dtUploadlabel As DataTable = OldNewConn.GetDataTable2("Select Print_Name from PayslipSetup" & Session("WebTableID") & " where Name ='Uploadlabel'")

            If (dtUploadlabel.Rows.Count > 0) Then
                Literal1.Text = dtUploadlabel.Rows(0)("Print_Name").ToString
            End If


            If Not IsNothing(Request.QueryString("ID")) Then

                Dim s As String = Request.QueryString("ID").Replace(" ", "+")
                If s.Length Mod 4 > 0 Then
                    s = s.PadRight(s.Length + 4 - s.Length Mod 4, "="c)
                End If
                ReimbursementID = EncryDecrypt.Decrypt(s, "EncryptString01")

                If IsNothing(Request.QueryString("s")) Then
                    ReimbursementID = ReimbursementID.ToString.Substring(0, ReimbursementID.ToString.Length - 3)
                Else
                    radGridTravel.MasterTableView.GetColumn("Edit").Display = False
                    radGridTravel.MasterTableView.GetColumn("Delete").Display = False
                    btnSave.Visible = False

                    Dim s1 As String = Request.QueryString("Code").Replace(" ", "+")


                    If s1.Length Mod 4 > 0 Then
                        s1 = s1.PadRight(s1.Length + 4 - s1.Length Mod 4, "="c)
                    End If
                    Code = EncryDecrypt.Decrypt(s1, "abc")
                End If

                Dim dt12 As DataTable

                dt12 = OldNewConn.GetDataTable2("Select EntryDate from reimbursementdetails" & Session("WebtableID") & " where reimbursedetailsid=" & ReimbursementID & "")

                If (dt12.Rows.Count > 0) Then
                    If Not IsDBNull(dt12.Rows(0)("EntryDate")) Then
                        Dim entryDate As DateTime = Convert.ToDateTime(dt12.Rows(0)("EntryDate"))

                        If Not (entryDate.Month = DateTime.Now.Month And entryDate.Year = DateTime.Now.Year) Then
                            radGridTravel.MasterTableView.GetColumn("Edit").Display = False
                            radGridTravel.MasterTableView.GetColumn("Delete").Display = False
                            btnSave.Visible = False
                            lbl.Visible = False
                            trguidelines.Visible = False
                            linkCSVTemplate.Visible = False
                            radAsynUpload.Visible = False
                            btnUpload.Visible = False
                            lblFileUpload.Visible = False
                        End If
                    End If
                End If

                Dim dt As DataTable

                dt = OldNewConn.GetDataTable2("select Rm.Name,Rm.ReimburseType_Code from reimbursementdetails" & Session("WebtableID") & " rd inner join ReimburseMentTypeMaster" & Session("WebtableID") & " Rm on rd.ReimburseType_Code=Rm.ReimburseType_Code where ReimburseDetailsId=" & ReimbursementID & "")

                If (dt.Rows.Count > 0) Then
                    type = dt.Rows(0)("Name")
                    ReimbursementType_Code = dt.Rows(0)("ReimburseType_Code")
                End If

            End If

            Dim Str As String = "Select JoiningDate from employeesmaster" & Session("WebTableID") & " where Emp_Code='" & Session("Emp_code").ToString() & "'"
            Dim dtempDOJ As DataTable = OldNewConn.GetDataTable2(Str)

            Dim Strtype As String = "select ISNULL(PSS.Print_Name,RTM.Name) as 'Field Name' from ReimburseMentTypeMaster" & Session("WebTableID") & " RTM Left join PayslipSetup" & Session("WebTableID") & " PSS on RTM.Name=PSS.Name where (PSS.Name ='" & type & "' Or RTM.Name='" & type & "')"

            Dim dttype As DataTable

            dttype = OldNewConn.GetDataTable2(Strtype)

            txtReimbursementType.Text = StrConv(dttype.Rows(0)("Field Name"), VbStrConv.ProperCase).ToString & " Claim Form"

            If (ReimbursementType_Code > 0) Then
                dtValidDates = OldNewConn.GetDataTable2("select BillValidStartDate,BillValidEndDate,ReimbursePolicy,Name from ReimburseMentTypeMaster" & Session("WebTableID") & " where ReimburseType_Code='" & ReimbursementType_Code & "'")
            ElseIf (ReimbursementTypeID > 0) Then
                dtValidDates = OldNewConn.GetDataTable2("select BillValidStartDate,BillValidEndDate,ReimbursePolicy,Name from ReimburseMentTypeMaster" & Session("WebTableID") & " where ReimburseType_Code='" & ReimbursementTypeID & "'")
            End If

            If Not (IsPostBack) Then
                IsEntrySaved = False
                bindTable()
            End If

            Dim dtCheckISCSVViewable As DataTable
            dtCheckISCSVViewable = OldNewConn.GetDataTable2("select * from PayslipSetup" & Session("WebTableID") & " where Name='CheckISCSVViewable'")

            Dim flagCheckISCSVViewable As Boolean = False

            If (dtCheckISCSVViewable.Rows.Count > 0) Then

                Dim FieldName As String() = dtCheckISCSVViewable.Rows(0)("Print_Name").ToString().Split(",")

                For i As Int32 = 0 To FieldName.Count - 1
                    If (type.ToString().ToLower() = FieldName(i).ToString().ToLower()) Then
                        flagCheckISCSVViewable = True
                    End If
                Next
            End If

            If (flagCheckISCSVViewable = True) Then
                lbl.Visible = False
                linkCSVTemplate.Visible = False
                radAsynUpload.Visible = False
                btnUpload.Visible = False
                lblFileUpload.Visible = False
                panelCSVUpload.Visible = False
                trguidelines.Visible = False
            End If

            If Not (radGridTravel.Visible) Then
                trguidelines.Visible = False
                lbl.Visible = False
                linkCSVTemplate.Visible = False
                lblFileUpload.Visible = False
                radAsynUpload.Visible = False
                btnUpload.Visible = False
            End If

            If (dtValidDates.Rows.Count > 0) Then
                If Not (dtValidDates.Rows(0)("ReimbursePolicy") = "") Then
                    linkPolicySection.Visible = True
                    If (ReimbursementType_Code > 0) Then
                        linkPolicySection.Attributes.Add("onclick", String.Format("return openRadWindow('{0}');", "ShowPolicy.aspx?value=" & ReimbursementType_Code & ""))
                    ElseIf (ReimbursementTypeID > 0) Then
                        linkPolicySection.Attributes.Add("onclick", String.Format("return openRadWindow('{0}');", "ShowPolicy.aspx?value=" & ReimbursementTypeID & ""))
                    End If
                End If
            End If

            If (ReimbursementType_Code > 0) Then
                dtUpload = OldNewConn.GetDataTable2("select IsUpload from ReimburseMentTypeMaster" & Session("WebTableID") & " where ReimburseType_Code='" & ReimbursementType_Code & "'")
            ElseIf (ReimbursementTypeID > 0) Then
                dtUpload = OldNewConn.GetDataTable2("select IsUpload from ReimburseMentTypeMaster" & Session("WebTableID") & " where ReimburseType_Code='" & ReimbursementTypeID & "'")
            End If

            If Not IsDBNull(dtUpload.Rows(0)("IsUpload")) Then
                If (dtUpload.Rows(0)("IsUpload")) Then
                    panelBillupload.Visible = True
                    radGridTravel.MasterTableView.GetColumn("ReimbursementPersonsDetailID").Display = True
                End If
            End If

            Dim year As String
            If (DateTime.Now.Month <= 3) Then
                year = DateTime.Now.Year - 1
            Else
                year = DateTime.Now.Year
            End If

            If (Session("WebtableID") = 977) Then
                lblBalance.Text = String.Format("<table width=""100%"" style=""color:red"" class=""mytablecss""><tr><th>To claim LTA exemption, signed copy of claim form along with signed copy of From 12BB (generated within LTA claim form file itself) should be submitted</th></tr></table>")
            End If
        Else
            Response.Redirect("../Logout.aspx")
        End If
    End Sub

    ''' <summary>
    ''' Bind the Controls
    ''' </summary>
    ''' <remarks></remarks>

    Private Sub bindTable()
        Try
            Dim dict As New Dictionary(Of String, String)

            Dim dtReimburse As DataTable
            dtReimburse = OldNewConn.GetDataTable2("select * from ReimburseMentTypeMaster" & Session("WebTableID") & " where Name='" & type & "'")

            Dim dtReimburseFullDetail As DataTable

            dtReimburseFullDetail = OldNewConn.GetDataTable2("select * from reimbursementdetails" & Session("WebTableID") & " where ReimburseDetailsID=" & ReimbursementID & "")

            dict = DirectCast(Session("TempDict"), Dictionary(Of String, String))

            If IsNothing(Request.QueryString("ID")) Then
                lbluploadheader.Text = "Upload your Bills here"
                If Session("WebTableID").ToString = "510" Then
                    If type = "RIM_DRIVER" Then
                        lbluploadheader.Text = "Upload your bills here along with Driver License copy (in case of any change only)"
                    ElseIf type = "RIM_VEHICLE" Then
                        lbluploadheader.Text = "Upload your bills here along with RC copy (in case of any change only)"
                    End If
                Else
                    Try
                        If Not String.IsNullOrEmpty(dtReimburse.Rows(0)("ReimburseUploadheader").ToString) Then
                            lbluploadheader.Text = dtReimburse.Rows(0)("ReimburseUploadheader").ToString
                        End If
                    Catch ex As Exception

                    End Try
                End If
            Else
                lbluploadheader.Text = "Upload your Bills here (when editing the claim, to upload all the bills pertaining this claim)"
                Try
                    If Not String.IsNullOrEmpty(dtReimburse.Rows(0)("ReimburseUploadheader").ToString) Then
                        lbluploadheader.Text = dtReimburse.Rows(0)("ReimburseUploadheader").ToString
                    End If
                Catch ex As Exception

                End Try
            End If

            For Each dr As DataRow In dtReimburse.Rows

                lblEntryDate.Visible = True
                radDatepickerEntryDate.Visible = True
                radDatepickerEntryDate.SelectedDate = DateTime.Now

                If Not (dr("TransactionDateTitle") = "") Then

                    panelDates.Visible = True
                    lblTransactionDate.Text = dr("TransactionDateTitle")
                    validTransactionDate.Enabled = True

                    Dim TransactionDate As Date
                    Dim d As Date

                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then ' If this page is being redirected not from edit entry page.
                        Else
                            If Date.TryParseExact(CDate(dtReimburseFullDetail.Rows(0)("TransactionDate")).ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                                TransactionDate = d
                            Else
                                Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                                Exit Sub
                            End If
                            radDatePickerTransactionDate.SelectedDate = TransactionDate.Date
                        End If
                    End If

                    If Not IsNothing(dict) Then
                        If (dict.Count > 0) Then
                            If (dict.ContainsKey("TransactionDate")) Then
                                If Date.TryParseExact(dict("TransactionDate"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                                    TransactionDate = d
                                Else
                                    Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                                    Exit Sub
                                End If
                                radDatePickerTransactionDate.SelectedDate = TransactionDate.Date
                            End If
                        End If
                    End If
                End If

                If (dr("BillMonth")) Then
                    lblTransactionDetailMandatory.Text = ""
                    Me.lblTransactionDate.Text = "Bill Month"
                    TransactionDetail.Visible = False
                    lblTransactionDetail.Visible = False
                    lblTransactionDetailMandatory.Visible = False
                Else
                    TransactionDetail.Visible = False
                    lblTransactionDetailMandatory.Visible = False
                    lblTransactionDetail.Visible = False
                End If

                If (dr("NeedExtraDateField")) Then
                    lblExtraDateField.Visible = True
                    radDatepickerExtraDateField.Visible = True
                    lblReqExtraDateField.Visible = True
                    validExtraDateField.Enabled = True
                    lblExtraDateField.Text = dr("ExtraDateFieldDescription")

                    Dim TransactionDate As Date
                    Dim d As Date

                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then ' If this page is being redirected not from edit entry page.
                        Else
                            If Date.TryParseExact(CDate(dtReimburseFullDetail.Rows(0)("ExtraDateField")).ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                                TransactionDate = d
                            Else
                                Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                                Exit Sub
                            End If
                            radDatepickerExtraDateField.SelectedDate = TransactionDate.Date
                        End If
                    End If

                    If Not IsNothing(dict) Then
                        If (dict.Count > 0) Then
                            If (dict.ContainsKey("ExtraDateField")) Then
                                If Date.TryParseExact(dict("ExtraDateField"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                                    TransactionDate = d
                                Else
                                    Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                                    Exit Sub
                                End If
                                radDatepickerExtraDateField.SelectedDate = TransactionDate.Date
                            End If
                        End If
                    End If
                End If

                If Not (dr("TransactionTitle") = "") Then
                    panelDetail.Visible = True
                    lblTransactionDetail.Visible = True
                    validDetail.Enabled = True
                    lblTransactionDetail.Text = dr("TransactionTitle")

                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then ' If this page is being redirected not from edit entry page.
                        Else
                            TransactionDetail.Text = dtReimburseFullDetail.Rows(0)("TransactionDetail")
                        End If
                    End If

                    If Not IsNothing(dict) Then
                        If (dict.Count > 0) Then
                            If (dict.ContainsKey("TransactionDetail")) Then
                                TransactionDetail.Text = dict("TransactionDetail")
                            End If

                        End If
                    End If
                End If

                If Not (dr("ExtraField1") = "") Then
                    panelDetail.Visible = True
                    lblExtraField1.Visible = True
                    lblReq1.Visible = True
                    validEF1.Enabled = True
                    lblExtraField1.Text = dr("ExtraField1")
                    ExtraField1.Visible = True

                    If (dr("ExtraField1").ToString.ToLower.Contains("number")) Then
                        NumbervalidEF1.Enabled = True
                    ElseIf (dr("ExtraField1").ToString.ToLower.Contains("registration") Or dr("ExtraField1").ToString.ToLower.Contains("license no")) Then
                        AllEF1.Enabled = True
                    Else
                        TextvalidEF1.Enabled = True
                    End If

                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then ' If this page is being redirected not from edit entry page.
                        Else
                            ExtraField1.Text = dtReimburseFullDetail.Rows(0)("ExtraField1")
                        End If
                    End If

                    If Not IsNothing(dict) Then
                        If (dict.Count > 0) Then
                            If (dict.ContainsKey("ExtraField1")) Then
                                ExtraField1.Text = dict("ExtraField1")
                            End If

                        End If
                    End If
                End If

                If Not (dr("ExtraField2") = "") Then
                    panelDetail.Visible = True
                    lblExtraField2.Visible = True
                    lblReq2.Visible = True
                    lblExtraField2.Text = dr("ExtraField2")
                    ExtraField2.Visible = True
                    validEF2.Enabled = True

                    If (dr("ExtraField2").ToString.ToLower.Contains("number")) Then
                        NumbervalidEF2.Enabled = True
                    ElseIf (dr("ExtraField2").ToString.ToLower.Contains("registration") Or dr("ExtraField2").ToString.ToLower.Contains("license no")) Then
                        AllEF2.Enabled = True
                    Else
                        TextvalidEF2.Enabled = True
                    End If

                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then
                        Else
                            ExtraField2.Text = dtReimburseFullDetail.Rows(0)("ExtraField2")
                        End If
                    End If

                    If Not IsNothing(dict) Then
                        If (dict.Count > 0) Then
                            If (dict.ContainsKey("ExtraField2")) Then
                                ExtraField2.Text = dict("ExtraField2")
                            End If
                        End If
                    End If
                End If

                If Not (dr("ExtraField3") = "") Then
                    panelDetail.Visible = True
                    lblReq3.Visible = True
                    lblExtraField3.Visible = True
                    ExtraField3.Visible = True
                    lblExtraField3.Text = dr("ExtraField3")
                    validEF3.Enabled = True

                    If (dr("ExtraField3").ToString.ToLower.Contains("number")) Then
                        NumbervalidEF3.Enabled = True
                    ElseIf (dr("ExtraField3").ToString.ToLower.Contains("registration") Or dr("ExtraField3").ToString.ToLower.Contains("license no")) Then
                        AllEF3.Enabled = True
                    Else
                        TextvalidEF3.Enabled = True
                    End If

                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then ' If this page is being redirected not from edit entry page.
                        Else
                            ExtraField3.Text = dtReimburseFullDetail.Rows(0)("ExtraField3")
                        End If
                    End If

                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        ExtraField3.Enabled = True
                        ExtraField3.BackColor = Drawing.Color.White
                        ExtraField3.Text = dtReimburseFullDetail.Rows(0)("ExtraField3")
                    End If

                    If Not IsNothing(dict) Then
                        If (dict.Count > 0) Then
                            If (dict.ContainsKey("ExtraField3")) Then
                                ExtraField3.Text = dict("ExtraField3")
                            End If
                        End If
                    End If
                End If

                If Not (dr("ExtraField5") = "") Then
                    panelDetail.Visible = True
                    lblExtraField5.Visible = True
                    lblExtraField5.Text = dr("ExtraField5")
                    lblReq5.Visible = True
                    ExtraField5.Visible = True
                    validEF5.Enabled = True

                    If (dr("ExtraField5").ToString.ToLower.Contains("number")) Then
                        NumbervalidEF5.Enabled = True
                    ElseIf (dr("ExtraField5").ToString.ToLower.Contains("registration") Or dr("ExtraField5").ToString.ToLower.Contains("license no")) Then
                        AllEF5.Enabled = True
                    Else
                        TextvalidEF5.Enabled = True
                    End If

                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then ' If this page is being redirected not from edit entry page.
                        Else
                            ExtraField5.Text = dtReimburseFullDetail.Rows(0)("ExtraField5")
                        End If
                    End If


                    If Not IsNothing(dict) Then
                        If (dict.Count > 0) Then
                            If (dict.ContainsKey("ExtraField5")) Then
                                ExtraField5.Text = dict("ExtraField5")
                            End If
                        End If
                    End If
                End If

                If Not (dr("ExtraField4") = "") Then

                    panelDetail.Visible = True
                    lblReq4.Visible = True
                    lblExtraField4.Visible = True
                    ExtraField4.Visible = True
                    validEF4.Enabled = True

                    lblExtraField4.Text = dr("ExtraField4Description").ToString()

                    Dim strstrold As String() = dr("ExtraField4").ToString.Split(",")
                    ExtraField4.DataSource = strstrold
                    ExtraField4.DataBind()

                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then ' If this page is being redirected not from edit entry page.
                        Else
                            ExtraField4.SelectedValue = ExtraField4.Items.FindByText(dtReimburseFullDetail.Rows(0)("ExtraField4")).Value
                        End If
                    End If

                    If Not IsNothing(dict) Then
                        If (dict.Count > 0) Then
                            If (dict.ContainsKey("ExtraField4")) Then
                                ExtraField4.SelectedValue = ExtraField4.Items.FindByText(dict("ExtraField4")).Value
                            End If
                        End If
                    End If
                End If

                If Not (dr("ExtraField6") = "") Then

                    panelDetail.Visible = True
                    lblReq6.Visible = True
                    lblExtraField6.Visible = True
                    ExtraField6.Visible = True
                    validEF6.Enabled = True

                    lblExtraField6.Text = dr("ExtraField6Description").ToString()

                    Dim strstrold As String() = dr("ExtraField6").ToString.Split(",")
                    ExtraField6.DataSource = strstrold
                    ExtraField6.DataBind()

                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then ' If this page is being redirected not from edit entry page.
                        Else
                            ExtraField6.SelectedValue = ExtraField6.Items.FindByText(dtReimburseFullDetail.Rows(0)("ExtraField6")).Value
                        End If
                    End If

                    If Not IsNothing(dict) Then
                        If (dict.Count > 0) Then
                            If (dict.ContainsKey("ExtraField6")) Then
                                ExtraField6.SelectedValue = ExtraField6.Items.FindByText(dict("ExtraField6")).Value
                            End If
                        End If
                    End If
                End If

                If Not (dr("ExtraField7") = "") Then

                    panelDates.Visible = True
                    lblReq7.Visible = True
                    lblExtraField7.Visible = True
                    radExtraField7.Visible = True
                    validEF7.Visible = True
                    validEF7.Enabled = True

                    Dim ExtraField7Date As Date
                    Dim d As Date

                    lblExtraField7.Text = dr("ExtraField7").ToString()

                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then
                        Else
                            If Date.TryParseExact(CDate(dtReimburseFullDetail.Rows(0)("ExtraField7")).ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                                ExtraField7Date = d
                            Else
                                Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                                Exit Sub
                            End If
                            radExtraField7.SelectedDate = ExtraField7Date.Date
                        End If
                    End If

                    If Not IsNothing(dict) Then
                        If (dict.Count > 0) Then
                            If (dict.ContainsKey("ExtraField7")) Then
                                If Date.TryParseExact(dict("ExtraField7"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                                    ExtraField7Date = d
                                Else
                                    Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                                    Exit Sub
                                End If
                                radExtraField7.SelectedDate = ExtraField7Date.Date
                            End If
                        End If
                    End If
                End If

                If Not (dr("ExtraField8") = "") Then

                    panelDates.Visible = True
                    lblReq8.Visible = True
                    lblExtraField8.Visible = True
                    radExtraField8.Visible = True
                    validEF8.Visible = True
                    validEF8.Enabled = True

                    Dim ExtraField8Date As Date
                    Dim d As Date

                    lblExtraField8.Text = dr("ExtraField8").ToString()

                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then
                        Else
                            If Date.TryParseExact(CDate(dtReimburseFullDetail.Rows(0)("ExtraField8")).ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                                ExtraField8Date = d
                            Else
                                Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                                Exit Sub
                            End If
                            radExtraField8.SelectedDate = ExtraField8Date.Date
                        End If
                    End If

                    If Not IsNothing(dict) Then
                        If (dict.Count > 0) Then
                            If (dict.ContainsKey("ExtraField8")) Then
                                If Date.TryParseExact(dict("ExtraField8"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                                    ExtraField8Date = d
                                Else
                                    Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                                    Exit Sub
                                End If
                                radExtraField8.SelectedDate = ExtraField8Date.Date
                            End If
                        End If
                    End If
                End If

                If Not (dr("TotalBillField") = "") Then
                    panelTotalBill.Visible = True
                    validBillAmount0.Enabled = True
                    NumbervalidBillAoumnt0.Enabled = True
                    lblTotalBillField.Text = dr("TotalBillField")

                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then ' If this page is being redirected not from edit entry page.
                        Else
                            txtTotalBillField.Text = CInt(dtReimburseFullDetail.Rows(0)("TotalBillField"))
                        End If
                    End If

                    If Not IsNothing(dict) Then
                        If (dict.Count > 0) Then
                            If (dict.ContainsKey("TotalBillField")) Then
                                txtTotalBillField.Text = CInt(dict("TotalBillField"))
                            End If
                        End If
                    End If
                End If


                If Not (dr("ClaimAmountField") = "") Then
                    panelTotalBill.Visible = True
                    lblAmount.Visible = True
                    validBillAmount.Enabled = True
                    NumbervalidBillAoumnt.Enabled = True
                    lblAmount.Text = dr("ClaimAmountField")

                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then ' If this page is being redirected not from edit entry page.
                        Else
                            Amount.Text = CInt(dtReimburseFullDetail.Rows(0)("ClaimAmount"))
                        End If
                    End If

                    If Not IsNothing(dict) Then
                        If (dict.Count > 0) Then
                            If (dict.ContainsKey("ClaimAmount")) Then
                                Amount.Text = CInt(dict("ClaimAmount"))
                            End If
                        End If
                    End If
                End If

                If Not IsDBNull(dr("ReimburseApplyNote")) Then
                    If dr("ReimburseApplyNote") <> "" Then
                        Me.lblNote.Text = "Important Note : " & dr("ReimburseApplyNote")
                    End If
                Else
                    Me.lblNote.Text = ""
                End If

                If dr("Disclaimer") <> "" Then
                    Me.chkDisclaimer.Visible = True
                    Me.chkDisclaimer.Text = dr("Disclaimer")
                Else
                    Me.chkDisclaimer.Visible = False
                    Me.chkDisclaimer.Text = ""
                End If

                Session("IsPersonalDetails") = dr("isPersonsDetailRequired")

                If (dr("isPersonsDetailRequired")) Then
                    radGridTravel.Visible = True
                    lnkTravel.Visible = True

                Else
                    radGridTravel.Visible = False
                    lnkTravel.Visible = False
                End If

                Dim dicUploadFile As New Dictionary(Of Integer, UploadedFile)

                If Not IsNothing(Session("UploadedFile")) Then
                    dicUploadFile = Session("UploadedFile")
                    Dim sb As StringBuilder = New StringBuilder()
                    Dim i As Int32 = 0
                    For Each rad As UploadedFile In dicUploadFile.Values
                        i += 1
                        sb.Append("(" & i & ")  ")
                        sb.Append(rad.GetName)
                        sb.Append("   ")
                    Next
                    lit1.Text = sb.ToString()
                End If
            Next
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Save Session
    ''' </summary>
    ''' <remarks></remarks>

    Private Sub SaveInSession()

        Dim dtReimburse As DataTable

        dtReimburse = OldNewConn.GetDataTable2("select * from ReimburseMentTypeMaster" & Session("WebTableID") & " where Name='" & type & "'")

        Dim dict As New Dictionary(Of String, String)

        For Each dr As DataRow In dtReimburse.Rows
            Dim TransactionDate As Date
            Dim strTransactionDate As Date

            Dim d As Date

            If Not (dr("TransactionDateTitle") = "") Then
                strTransactionDate = radDatePickerTransactionDate.SelectedDate.Value.Date
            Else
                strTransactionDate = radDatepickerEntryDate.SelectedDate.Value.Date
            End If

            Try
                If Date.TryParseExact(strTransactionDate.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                    TransactionDate = d
                Else
                    Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                    Exit Sub
                End If
            Catch ex As Threading.ThreadAbortException
            Catch ex As Exception
                Me.lblError.Text = "" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!"
                Exit Sub
            End Try

            dict.Add("TransactionDate", TransactionDate.Date.ToString("dd/MM/yyyy"))

            If (dr("NeedExtraDateField")) Then
                Dim ExtraDateField As Date = radDatepickerExtraDateField.SelectedDate.Value.Date
                Dim d2 As Date
                Dim dtExtraDateField As Date
                Try

                    If Date.TryParseExact(ExtraDateField.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d2) Then
                        dtExtraDateField = d2
                    Else
                        Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                        Exit Sub
                    End If
                Catch ex As Threading.ThreadAbortException
                Catch ex As Exception
                    Me.lblError.Text = "" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!"
                    Exit Sub
                End Try

                dict.Add("ExtraDateField", dtExtraDateField.Date.ToString("dd/MM/yyyy"))
            End If

            If Not (dr("TransactionTitle") = "") Then
                dict.Add("TransactionDetail", TransactionDetail.Text)
            Else
                dict.Add("TransactionDetail", String.Empty)
            End If

            If Not (dr("ExtraField1") = "") Then
                dict.Add("ExtraField1", ExtraField1.Text)
            Else
                dict.Add("ExtraField1", String.Empty)
            End If

            If Not (dr("ExtraField2") = "") Then
                dict.Add("ExtraField2", ExtraField2.Text)
            Else
                dict.Add("ExtraField2", String.Empty)
            End If

            If Not (dr("ExtraField3") = "") Then
                dict.Add("ExtraField3", ExtraField3.Text)
            Else
                dict.Add("ExtraField3", String.Empty)
            End If

            If Not (dr("ExtraField4") = "") Then
                dict.Add("ExtraField4", ExtraField4.SelectedItem.Text)
            Else
                dict.Add("ExtraField4", String.Empty)
            End If

            If Not (dr("ExtraField5") = "") Then
                dict.Add("ExtraField5", ExtraField5.Text)
            Else
                dict.Add("ExtraField5", String.Empty)
            End If

            If Not (dr("ExtraField6") = "") Then
                dict.Add("ExtraField6", ExtraField6.SelectedItem.Text)
            Else
                dict.Add("ExtraField6", String.Empty)
            End If

            If Not (dr("ExtraField7") = "") Then
                Dim ExtraDateField As Date = radExtraField7.SelectedDate.Value.Date
                Dim d3 As Date
                Dim dtExtraDateField As Date
                Try

                    If Date.TryParseExact(ExtraDateField.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d3) Then
                        dtExtraDateField = d3
                    Else
                        Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                        Exit Sub
                    End If
                Catch ex As Threading.ThreadAbortException
                Catch ex As Exception
                    Me.lblError.Text = "" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!"
                    Exit Sub
                End Try

                dict.Add("ExtraField7", dtExtraDateField.Date.ToString("dd/MM/yyyy"))
            End If


            If Not (dr("ExtraField8") = "") Then
                Dim ExtraDateField As Date = radExtraField8.SelectedDate.Value.Date
                Dim d4 As Date
                Dim dtExtraDateField As Date
                Try

                    If Date.TryParseExact(ExtraDateField.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d4) Then
                        dtExtraDateField = d4
                    Else
                        Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                        Exit Sub
                    End If
                Catch ex As Threading.ThreadAbortException
                Catch ex As Exception
                    Me.lblError.Text = "" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!"
                    Exit Sub
                End Try
                dict.Add("ExtraField8", dtExtraDateField.Date.ToString("dd/MM/yyyy"))
            End If

            If Not (dr("TotalBillField") = "") Then
                dict.Add("TotalBillField", txtTotalBillField.Text)
            Else
                dict.Add("TotalBillField", String.Empty)
            End If

            Dim totalAmt1 As Decimal = 0.0

            If Not (dr("ClaimAmountField") = "") Then
                dict.Add("ClaimAmount", Amount.Text)
            Else
                dict.Add("ClaimAmount", totalAmt1)
            End If

            dict.Add("BillPassed", 0)

            Dim dtEntryDate As Date = radDatepickerEntryDate.SelectedDate.Value
            dict.Add("EntryDate", dtEntryDate.Date.ToString("dd/MM/yyyy"))

            dict.Add("Rejected", 0)
        Next

        Dim dicUploadFile As New Dictionary(Of Integer, UploadedFile)

        If (RadAsyncUpload1.UploadedFiles.Count > 0) Then
            Dim i As Int32 = 0
            For Each rad As UploadedFile In RadAsyncUpload1.UploadedFiles
                i += 1
                dicUploadFile.Add(i, rad)
            Next
            Session("UploadedFile") = dicUploadFile
        End If

        Session("TempDict") = dict
    End Sub

    ''' <summary>
    ''' RadGridTravel
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    ''' 

    Protected Sub radGridTravel_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles radGridTravel.NeedDataSource
        Try
            Dim dtTable As New DataTable
            If Not IsNothing(Session("ReimbursetableTravel")) Then
                radGridTravel.DataSource = Session("ReimbursetableTravel")
            Else
                dtTable.Columns.Add("Sl.No")
                dtTable.Columns.Add("PersonsName")
                dtTable.Columns.Add("Dependence")
                dtTable.Columns.Add("RelationshipWithEmployee")
                dtTable.Columns.Add("From")
                dtTable.Columns.Add("To")
                dtTable.Columns.Add("JourneyDate")
                dtTable.Columns.Add("ModeOfJourney")
                dtTable.Columns.Add("TypeOfJourney")
                dtTable.Columns.Add("ClaimAmount")
                dtTable.Columns.Add("ReimbursementPersonsDetailID")

                Dim dt As DataTable = GetPersonalDetailsofReimbursement(ReimbursementID)

                Dim dicUploadFile As New Dictionary(Of Integer, Integer)

                Dim dict12 As New Dictionary(Of Integer, Byte())

                For Each dr As DataRow In dt.Rows
                    dtTable.Rows.Add(dr("Sl.No"), dr("PersonsName"), dr("Dependence"), dr("RelationshipWithEmployee"), dr("From"), dr("To"), dr("JourneyDate"), dr("ModeOfJourney"), dr("TypeOfJourney"), Format(dr("ClaimAmount"), "###0"), dr("ReimbursementPersonsDetailID"))
                    dicUploadFile.Add(dr("Sl.No"), dr("ReimbursementPersonsDetailID"))
                Next

                radGridTravel.DataSource = dtTable
                dtTable = dtTable.DefaultView.ToTable()
                Session("ReimbursetableTravel") = dtTable
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub radGridTravel_DeleteCommand(sender As Object, e As Telerik.Web.UI.GridCommandEventArgs) Handles radGridTravel.DeleteCommand

        Dim griditem As GridDataItem = TryCast(e.Item, GridDataItem)
        Dim dicUploadFile As Dictionary(Of Integer, Byte())
        If Not IsNothing(griditem) Then
            If Not IsNothing(Session("ReimbursetableTravel")) Then
                Dim dt As DataTable = DirectCast(Session("ReimbursetableTravel"), DataTable)
                Dim dr As DataRow = dt.Select("Sl.No = '" & griditem.GetDataKeyValue("Sl.No") & "'")(0)

                If Not IsNothing(dr) Then
                    dt.Rows.Remove(dr)
                End If
            End If
        End If
    End Sub

    Protected Sub radGridTravel_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles radGridTravel.ItemDataBound
        Dim griditem As GridDataItem = TryCast(e.Item, GridDataItem)
        If Not IsNothing(griditem) Then
            griditem("JourneyDate").Text = Convert.ToDateTime(griditem("JourneyDate").Text).ToString("dd/MM/yyyy")
        End If
    End Sub

    Protected Sub btnEditTravel_Click(sender As Object, e As EventArgs)
        SaveInSession()
        Dim btnEditTravel As LinkButton = TryCast(sender, LinkButton)
        Dim item As GridDataItem = TryCast(btnEditTravel.NamingContainer, GridDataItem)
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", String.Format("openRadWindow('{0}');", "OtherReimbursementPersonalDetail.aspx?SNo=" & item.GetDataKeyValue("Sl.No") & ""), True)
    End Sub

    ''' <summary>
    ''' CSV Upload
    ''' </summary>
    ''' <remarks></remarks>
    ''' 

    Protected Sub linkCSVTemplate_Click(sender As Object, e As EventArgs) Handles linkCSVTemplate.Click
        If IsNothing(Request.QueryString("ID")) Then
            If IsEntrySaved Then
                lblsave.Visible = True
                Response.Write("<script>alert('No more CSV templates can be uploaded. If you want then please go back to home page or click view/edit button below');</script>")
                Exit Sub
            Else
                csvtemplate()
            End If
        Else
            csvtemplate()
        End If
    End Sub

    Public Sub csvtemplate()
        Dim filename As String = DateTime.Now.ToString("dd/MM/yyyy") + ".csv"

        Response.Clear()
        Response.Buffer = True
        Response.AddHeader("content-disposition", "attachment; filename=" & filename & "")
        Response.Charset = ""
        Response.ContentType = "application/text"
        Dim listForReimburse As List(Of String) = createTemplateForReimburse()

        Dim sb As New StringBuilder()
        For Each name As String In listForReimburse
            sb.Append(name + ","c)
        Next
        sb.Append(vbCr & vbLf)

        Response.Write(sb.ToString())
        Response.Flush()
        Response.[End]()
    End Sub

    Private Function createTemplateForReimburse() As List(Of String)
        Dim list As New List(Of String)

        Dim dtReimburse As DataTable

        dtReimburse = OldNewConn.GetDataTable2("select * from ReimburseMentTypeMaster" & Session("WebTableID") & " where Name='" & type & "'")

        If (dtReimburse.Rows.Count > 0) Then
            If (dtReimburse.Rows(0)("isPersonsDetailRequired")) Then
                list.Add("PersonsName")
                list.Add("Dependent")
                list.Add("RelationshipWithEmployee")
                list.Add("From")
                list.Add("To")
                list.Add("JourneyDate")
                list.Add("ModeOfJourney")
                list.Add("TypeOfJourney")
                list.Add("ClaimAmount")
            End If
        End If
        Return list
    End Function

    Protected Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnUpload.Click

        If (radAsynUpload.UploadedFiles.Count = 0) Then
            Response.Write("<script>alert('Please Select CSV File.');</script>")
            Exit Sub
        End If


        'Leave from 
        Dim ExtraField7 As Date = radExtraField7.SelectedDate.Value
        Dim dtExtraField7 As Date
        Dim dExtraField7 As Date

        If Date.TryParseExact(ExtraField7.Date.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, dExtraField7) Then
            dtExtraField7 = dExtraField7
        End If

        Dim ExtraField8 As Date = radExtraField8.SelectedDate.Value
        Dim dtExtraField8 As Date
        Dim dExtraField8 As Date

        If Date.TryParseExact(ExtraField8.Date.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, dExtraField8) Then
            dtExtraField8 = dExtraField8
        End If
        Dim weeklyoffcheck As Boolean = WeeklyOffchecking()
        If weeklyoffcheck = True Then
            If (dtExtraField7.Date.DayOfWeek.ToString = DayOfWeek.Saturday.ToString) Then
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave from date should not be weekly off');", True)
                Exit Sub
            End If

            If (dtExtraField8.Date.DayOfWeek.ToString = DayOfWeek.Saturday.ToString) Then
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave taken to date should not be weekly off');", True)
                Exit Sub
            End If
        End If

        If (dtExtraField7.Date.DayOfWeek.ToString = DayOfWeek.Sunday.ToString) Then
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave from date should not be weekly off');", True)
            Exit Sub
        End If
        If (dtExtraField8.Date.DayOfWeek.ToString = DayOfWeek.Sunday.ToString) Then
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave taken to date should not be weekly off');", True)
            Exit Sub
        End If
        'leave to date


        'Journey from
        Dim Journeyfrom As Date = radDatePickerTransactionDate.SelectedDate.Value
        Dim dtJourneyfrom As Date
        Dim d7 As Date

        If Date.TryParseExact(Journeyfrom.Date.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d7) Then
            dtJourneyfrom = d7
        End If


        Dim Journeyto As Date = radDatepickerExtraDateField.SelectedDate.Value
        Dim dtJourneyto As Date
        Dim d8 As Date

        If Date.TryParseExact(Journeyto.Date.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d8) Then
            dtJourneyto = d8
        End If

        If (radDatePickerTransactionDate.SelectedDate.Value.Date.DayOfWeek.ToString = DayOfWeek.Saturday.ToString) Then
            dtJourneyfrom = dtJourneyfrom.AddDays(+2)

            If dtExtraField7.Date = dtJourneyfrom.Date Then
            ElseIf dtExtraField7.Date < dtJourneyfrom.Date Then
            Else
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave dates do not match with journey period');", True)
                Exit Sub
            End If

        ElseIf (radDatePickerTransactionDate.SelectedDate.Value.Date.DayOfWeek.ToString = DayOfWeek.Sunday.ToString) Then
            dtJourneyfrom = dtJourneyfrom.AddDays(+1)

            If dtExtraField7.Date = dtJourneyfrom.Date Then
            ElseIf dtExtraField7.Date < dtJourneyfrom.Date Then
            Else
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave dates do not match with journey period');", True)
                Exit Sub
            End If
        ElseIf dtExtraField7.Date <= dtJourneyfrom.Date Then
        Else
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave dates do not match with journey period');", True)
            Exit Sub
        End If

        If (radDatepickerExtraDateField.SelectedDate.Value.Date.DayOfWeek.ToString = DayOfWeek.Saturday.ToString) Then
            dtJourneyto = dtJourneyto.AddDays(-1)

            If dtExtraField8.Date = dtJourneyto.Date Then
            ElseIf dtExtraField8.Date > dtJourneyto.Date Then
            Else
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave dates do not match with journey period');", True)
                Exit Sub
            End If


        ElseIf (radDatepickerExtraDateField.SelectedDate.Value.Date.DayOfWeek.ToString = DayOfWeek.Sunday.ToString) Then
            dtJourneyto = dtJourneyto.AddDays(-2)

            If dtExtraField8.Date = dtJourneyto.Date Then
            ElseIf dtExtraField8.Date > dtJourneyto.Date Then
            Else
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave dates do not match with journey period');", True)
                Exit Sub
            End If
        ElseIf dtExtraField8.Date >= dtJourneyto.Date Then
        Else
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave dates do not match with journey period');", True)
            Exit Sub
        End If


        'Journey to end


        If IsNothing(Request.QueryString("ID")) Then
            If IsEntrySaved Then
                lblsave.Visible = True
                Response.Write("<script>alert('No more CSV entries can be uploaded. If you want then please go back to home page or click view/edit button below');</script>")
            Else
                CSVUpload()
            End If
        Else
            CSVUpload()
        End If
    End Sub

    Public Sub CSVUpload()
        Try
            Dim DateFormat As String = "d/M/yyyy"
            Dim ifcomma As Integer = 0

            Dim dt As New DataTable

            If Not IsNothing(Session("ReimbursetableTravel")) Then
                If (Session("IsPersonalDetails")) Then
                    dt = DirectCast(Session("ReimbursetableTravel"), DataTable)
                End If
            Else
                If (Session("IsPersonalDetails")) Then
                    dt.Columns.Add("Sl.No")
                    dt.Columns.Add("PersonsName")
                    dt.Columns.Add("Dependence")
                    dt.Columns.Add("RelationshipWithEmployee")
                    dt.Columns.Add("From")
                    dt.Columns.Add("To")
                    dt.Columns.Add("JourneyDate", GetType(System.String))
                    dt.Columns.Add("ModeOfJourney")
                    dt.Columns.Add("TypeOfJourney")
                    dt.Columns.Add("ClaimAmount")
                End If
            End If

            For Each rad As UploadedFile In radAsynUpload.UploadedFiles

                Dim sr As New StreamReader(rad.InputStream)
                Dim line As String = sr.ReadLine()
                Dim value As String() = line.Split(","c)

                Dim count As Integer = 0

                Dim dr As DataRow

                While Not sr.EndOfStream

                    value = sr.ReadLine().Split(","c)
                    dr = dt.NewRow()
                    If (Session("IsPersonalDetails")) Then

                        If (dt.Rows.Count = 0) Then
                            dr("Sl.No") = 1
                        Else
                            dr("Sl.No") = dt.Rows(dt.Rows.Count - 1)("Sl.No") + 1
                        End If

                        If Not IsAlphaNum(value(0).Replace(" ", "")) Then
                            Response.Write("<script>alert('PersonsName/Name of the Member Travelled can be in alphabetic value only');</script>")
                            Exit Sub
                        Else
                            dr("PersonsName") = value(0)
                        End If

                        If (value(1).ToString <> "Y" And value(1).ToString <> "N") Then
                            Me.lblError.Text = "Column ""Dependent"" should include ""Y"" (for Yes) or ""N"" (for No) only"
                            Exit Sub
                        End If

                        dr("Dependence") = value(1)

                        If (value(2).ToString <> "Self" And value(2).ToString <> "Spouse" And value(2).ToString <> "Father" And value(2).ToString <> "Mother" And value(2).ToString <> "Brother" And value(2).ToString <> "Sister" And value(2).ToString <> "Son" And value(2).ToString <> "Daughter") Then
                            Me.lblError.Text = "Column ""RelationshipWithEmployee"" should include ""Self"",""Spouse"",""Father"",""Mother"",""Brother"",""Sister"",""Son"",""Daughter"" only"
                            Exit Sub
                        End If

                        dr("RelationshipwithEmployee") = value(2)

                        If Not IsAlphaNum(value(3).Replace(" ", "")) Then
                            Response.Write("<script>alert('Travel from can be in alphabetic value only');</script>")
                            Exit Sub
                        Else
                            dr("From") = value(3)
                        End If


                        If Not IsAlphaNum(value(4).Replace(" ", "")) Then
                            Response.Write("<script>alert('Travel To can be in alphabetic value only');</script>")
                            Exit Sub
                        Else
                            dr("To") = value(4)
                        End If


                        Dim JourneyDate As Date
                        Dim d As Date
                        Try
                            If Date.TryParseExact(Trim(value(5)), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                                JourneyDate = d
                            Else
                                Me.lblError.Text = "Journey Date is not valid for traveler : " & value(0) & ", Date : " & value(5) & ", please enter the date either in DD/MM/YYYY format!"
                                Exit Sub
                            End If
                        Catch ex As Threading.ThreadAbortException
                        Catch ex As Exception
                            Me.lblError.Text = "Journey Date is not valid for traveler : " & value(0) & ", Date : " & value(5) & ", please enter the date either in DD/MM/YYYY format!"
                            Exit Sub
                        End Try

                        dr("JourneyDate") = JourneyDate.Date

                        If (value(6).ToString <> "Air" And value(6).ToString <> "Train" And value(6).ToString <> "Taxi" And value(6).ToString <> "Other") Then
                            Me.lblError.Text = "Column ""ModeofJourney"" should include ""Air"", ""Train"",""Taxi"" and ""Other"" only"
                            Exit Sub
                        End If

                        dr("ModeOfJourney") = value(6)

                        If (value(7).ToString <> "Inward" And value(7).ToString <> "Outward" And value(7).ToString <> "Inward & Outward") Then
                            Me.lblError.Text = "Column ""TypeofJourney"" should include ""Inward"", ""Outward"" and ""Inward & Outward"" only"
                            Exit Sub
                        End If

                        dr("TypeOfJourney") = value(7)

                        If Not IsNumeric(value(8)) Then
                            Response.Write("<script>alert('Amount can be in numeric value only');</script>")
                            Exit Sub
                        Else
                            dr("ClaimAmount") = Math.Round(CDec(value(8)))
                        End If

                        dt.Rows.Add(dr)
                    End If
                End While
            Next

            If (Session("IsPersonalDetails")) Then
                Session("ReimbursetableTravel") = dt
                radGridTravel.Rebind()
            End If

            SaveInSession()

            Response.Write("<script>alert('Bill detail saved, please upload bills and submit claim.');</script>")

        Catch ex As Exception
            lblError.Text = "Please check the CSV properly."
        End Try
    End Sub

    Protected Sub lnkTravel_Click(sender As Object, e As EventArgs) Handles lnkTravel.Click

        'Leave from 
        Dim ExtraField7 As Date = radExtraField7.SelectedDate.Value
        Dim dtExtraField7 As Date
        Dim dExtraField7 As Date

        If Date.TryParseExact(ExtraField7.Date.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, dExtraField7) Then
            dtExtraField7 = dExtraField7
        End If

        Dim ExtraField8 As Date = radExtraField8.SelectedDate.Value
        Dim dtExtraField8 As Date
        Dim dExtraField8 As Date

        If Date.TryParseExact(ExtraField8.Date.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, dExtraField8) Then
            dtExtraField8 = dExtraField8
        End If
        Dim weeklyoffcheck As Boolean = WeeklyOffchecking()
        If weeklyoffcheck = True Then
            If (dtExtraField7.Date.DayOfWeek.ToString = DayOfWeek.Saturday.ToString) Then
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave from date should not be weekly off');", True)
                Exit Sub
            End If

            If (dtExtraField8.Date.DayOfWeek.ToString = DayOfWeek.Saturday.ToString) Then
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave taken to date should not be weekly off');", True)
                Exit Sub
            End If
        End If
        If (dtExtraField7.Date.DayOfWeek.ToString = DayOfWeek.Sunday.ToString) Then
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave from date should not be weekly off');", True)
            Exit Sub
        End If
        If (dtExtraField8.Date.DayOfWeek.ToString = DayOfWeek.Sunday.ToString) Then
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave taken to date should not be weekly off');", True)
            Exit Sub
        End If
        'leave to date



        'Journey from
        Dim Journeyfrom As Date = radDatePickerTransactionDate.SelectedDate.Value
        Dim dtJourneyfrom As Date
        Dim d7 As Date

        If Date.TryParseExact(Journeyfrom.Date.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d7) Then
            dtJourneyfrom = d7
        End If


        Dim Journeyto As Date = radDatepickerExtraDateField.SelectedDate.Value
        Dim dtJourneyto As Date
        Dim d8 As Date

        If Date.TryParseExact(Journeyto.Date.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d8) Then
            dtJourneyto = d8
        End If

        If (radDatePickerTransactionDate.SelectedDate.Value.Date.DayOfWeek.ToString = DayOfWeek.Saturday.ToString) Then
            dtJourneyfrom = dtJourneyfrom.AddDays(+2)

            If dtExtraField7.Date = dtJourneyfrom.Date Then
            ElseIf dtExtraField7.Date < dtJourneyfrom.Date Then
            Else
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave dates do not match with journey period');", True)
                Exit Sub
            End If

        ElseIf (radDatePickerTransactionDate.SelectedDate.Value.Date.DayOfWeek.ToString = DayOfWeek.Sunday.ToString) Then
            dtJourneyfrom = dtJourneyfrom.AddDays(+1)

            If dtExtraField7.Date = dtJourneyfrom.Date Then
            ElseIf dtExtraField7.Date < dtJourneyfrom.Date Then
            Else
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave dates do not match with journey period');", True)
                Exit Sub
            End If
        ElseIf dtExtraField7.Date <= dtJourneyfrom.Date Then
        Else

            Dim checkholidayfrom As Boolean = holidaylistcheck(Journeyfrom.Date)
            If checkholidayfrom = True Then
            Else
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave dates do not match with journey period');", True)
                Exit Sub
            End If


        End If

        If (radDatepickerExtraDateField.SelectedDate.Value.Date.DayOfWeek.ToString = DayOfWeek.Saturday.ToString) Then
            dtJourneyto = dtJourneyto.AddDays(-1)

            If dtExtraField8.Date = dtJourneyto.Date Then
            ElseIf dtExtraField8.Date > dtJourneyto.Date Then
            Else
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave dates do not match with journey period');", True)
                Exit Sub
            End If


        ElseIf (radDatepickerExtraDateField.SelectedDate.Value.Date.DayOfWeek.ToString = DayOfWeek.Sunday.ToString) Then
            dtJourneyto = dtJourneyto.AddDays(-2)

            If dtExtraField8.Date = dtJourneyto.Date Then
            ElseIf dtExtraField8.Date > dtJourneyto.Date Then
            Else
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave dates do not match with journey period');", True)
                Exit Sub
            End If
        ElseIf dtExtraField8.Date >= dtJourneyto.Date Then
        Else

            Dim checkholidayto As Boolean = holidaylistcheck(Journeyto.Date)
            If checkholidayto = True Then
            Else
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave dates do not match with journey period');", True)
                Exit Sub
            End If

        End If

        Dim checkholidaylist As Boolean = holidaylistcheck(ExtraField7.Date)
        If checkholidaylist = True Then
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave From date is falling on holiday, please enter dates excluding holiday');", True)
            Exit Sub
        End If

        checkholidaylist = holidaylistcheck(ExtraField8.Date)
        If checkholidaylist = True Then
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave To date is falling on holiday, please enter dates excluding holiday');", True)
            Exit Sub
        End If




        'Journey to end

        If IsNothing(Request.QueryString("ID")) Then
            If IsEntrySaved Then
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", String.Format("openRadWindow('{0}');", "OtherReimbursementPersonalDetail.aspx?Type_Code=" & type & "&whetherIsSaveEntry =" & "Y" & ""), True)
                'image.Attributes.Add("onclick", String.Format("return openRadWindow('{0}');", "OtherReimbursementMultipleDetail.aspx?SNo=" & griditem.GetDataKeyValue("Sl.No") & "&Type_Code=" & type & ""))
            Else
                Session("TempDict") = Nothing
                SaveInSession()
                bindTable()
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", String.Format("openRadWindow('{0}');", "OtherReimbursementPersonalDetail.aspx?Type_Code=" & type & "&whetherIsSaveEntry =" & "N" & ""), True)
            End If
        Else
            Session("TempDict") = Nothing
            SaveInSession()
            bindTable()
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", String.Format("openRadWindow('{0}');", "OtherReimbursementPersonalDetail.aspx?Type_Code=" & type & "&whetherIsSaveEntry =" & "N" & ""), True)
        End If
    End Sub

    Protected Function holidaylistcheck(holidaydate As Date) As Boolean
        Dim payslipdt As DataTable = OldNewConn.GetDataTable2("select * from Payslipsetup" & Session("WebTableID") & " where Name='ReimbursementBillsHolidayListChecking'")
        If payslipdt.Rows.Count > 0 Then
            If Not String.IsNullOrEmpty(payslipdt.Rows(0)("Print_Name").ToString) Then
                If payslipdt.Rows(0)("Print_Name").ToString.ToUpper = "Y" Or payslipdt.Rows(0)("Print_Name").ToString.ToUpper = "YES" Then
                    Dim leavesetupdt As DataTable = OldNewConn.GetDataTable2("Select * from Leavesetup" & Session("WebTableID") & "  where Field_Name='HolidayList' and (Field_Calc<>'' and Field_Calc is not null)")
                    If leavesetupdt.Rows.Count > 0 Then
                        Dim fieldname As String = leavesetupdt.Rows(0)("Field_Calc").ToString & ""
                        Dim holidaytablename As String = ""
                        Dim holidaycodename As String = ""
                        If fieldname.ToUpper = "SECTION" Then
                            holidaytablename = "Sectionmaster"
                            holidaycodename = "section_code"
                        ElseIf fieldname.ToUpper = "LOCATION" Then
                            holidaytablename = "Locmaster"
                            holidaycodename = "Loc_code"
                        ElseIf fieldname.ToUpper = "Division" Then
                            holidaytablename = "DiviMaster"
                            holidaycodename = "divi_code"
                        ElseIf fieldname.ToUpper = "Department" Then
                            holidaytablename = "deptmaster"
                            holidaycodename = "dept_code"
                        ElseIf fieldname.ToUpper = "Designation" Then
                            holidaytablename = "DSGmaster"
                            holidaycodename = "dsg_code"
                        ElseIf fieldname.ToUpper = "Designation" Then
                            holidaytablename = "DSGmaster"
                            holidaycodename = "dsg_code"
                        End If

                        Dim dtcheckHolidayIndia As DataTable = OldNewConn.GetDataTable2("Select S.Name from employeesmaster" & Session("WebTableID") & " E inner join " & holidaytablename & "" & Session("WebTableID") & " S on S." & holidaycodename & "=E." & holidaycodename & " where E.emp_code='" & Session("Emp_Code") & "'")
                        If (dtcheckHolidayIndia.Rows.Count > 0) Then
                            Dim dtCheckHoliday As DataTable = OldNewConn.GetDataTable2("Select * from Holidaylist" & Session("WebTableID") & "_" & holidaydate.Year & "   Where HolidayDate='" & Format(holidaydate.Date, "MM/dd/yyyy") & "' and HolidayFor in ('" & dtcheckHolidayIndia.Rows(0)("Name") & "','')")
                            If (dtCheckHoliday.Rows.Count > 0) Then
                                ' ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You can not apply leave from date falling on holiday');", True)
                                '  Exit Function
                                Return True
                            End If
                        End If



                    Else
                        Dim dtCheckHoliday As DataTable = OldNewConn.GetDataTable2("Select * from Holidaylist" & Session("WebTableID") & "_" & holidaydate.Year & "   Where HolidayDate='" & Format(holidaydate.Date, "MM/dd/yyyy") & "' ")
                        If (dtCheckHoliday.Rows.Count > 0) Then
                            ' ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You can not apply leave from date falling on holiday');", True)
                            ' Exit Function
                            Return True
                        End If
                    End If

                End If
            End If
        End If
    End Function

    Protected Function WeeklyOffchecking() As Boolean
        Dim payslipdt As DataTable = OldNewConn.GetDataTable2("select * from Payslipsetup" & Session("WebTableID") & " where Name='ReimbursementWeeklyOffChecking'")
        If payslipdt.Rows.Count > 0 Then
            If Not String.IsNullOrEmpty(payslipdt.Rows(0)("Print_Name").ToString) Then
                If payslipdt.Rows(0)("Print_Name").ToString.ToUpper = "N" Or payslipdt.Rows(0)("Print_Name").ToString.ToUpper = "NO" Then
                    Return False
                End If
            End If
        End If
        Return True
    End Function

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

        If IsNothing(Request.QueryString("ID")) Then

            If IsEntrySaved Then
                lblsave.Visible = True
                btnSave.Enabled = False
                Response.Write("<script>alert('No more entries can be saved. If you want then please go back to home page or click view/edit button below');</script>")
            Else
                entriestobesaved()
            End If
        Else
            entriestobesaved()
        End If
    End Sub

    Protected Sub btnUploadBill_Click(sender As Object, e As EventArgs)
        Dim dicUploadFile As New Dictionary(Of Integer, UploadedFile)
        Dim sb As StringBuilder = New StringBuilder()

        If (RadAsyncUpload1.UploadedFiles.Count = 0) Then
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please Select jpeg/jpg/png/pdf File.');", True)
            Exit Sub
        End If

        If (RadAsyncUpload1.UploadedFiles.Count > 0) Then
            Dim i As Int32 = 0
            For Each rad As UploadedFile In RadAsyncUpload1.UploadedFiles
                i += 1
                dicUploadFile.Add(i, rad)
                sb.Append("(" & i & ")  ")
                sb.Append(rad.GetName)
                sb.Append("   ")
            Next
            lit1.Text = sb.ToString()
            Session("UploadedFile") = dicUploadFile
        End If
        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Bills uploaded, please proceed to submit claim by clicking on ""Submit Claim""');", True)
    End Sub

    Public Sub entriestobesaved()
        Try
            Dim dtReimburse As DataTable

            dtReimburse = OldNewConn.GetDataTable2("select * from ReimburseMentTypeMaster" & Session("WebTableID") & " where Name='" & type & "'")

            Dim dict As New Dictionary(Of String, String)

            dict.Add("Emp_Code", Session("Emp_Code"))
            If IsNothing(Request.QueryString("IDD")) Then
                dict.Add("ReimburseType_Code", ReimbursementType_Code)
            Else
                dict.Add("ReimburseType_Code", ReimbursementTypeID)
            End If

            For Each dr As DataRow In dtReimburse.Rows  ' 1st (To retereive reimbursetype from table)

                Dim TransactionDate As Date
                Dim strTransactionDate As Date
                Dim strTransactionDatereturningdate As String

                Dim d As Date

                Dim FromDate As Date
                Dim ToDate As Date

                FromDate = dr("BillValidStartDate")
                ToDate = dr("BillValidEndDate")

                If Not (dr("TransactionDateTitle") = "") Then
                    strTransactionDate = radDatePickerTransactionDate.SelectedDate.Value.Date
                Else
                    strTransactionDate = radDatepickerEntryDate.SelectedDate.Value.Date
                End If

                If strTransactionDate.Date >= FromDate.Date And strTransactionDate.Date <= ToDate.Date Then
                Else
                    Response.Write("<script>alert('JOURNEY DATE SHOULD BE BETWEEN " & FromDate.ToString("dd/MM/yyyy") & " TO " & ToDate.ToString("dd/MM/yyyy") & "');</script>")
                    Me.lblError.Text = ""
                    Exit Sub
                End If

                If (ReimbursementID = 0) Then  '' If new entry is there 
                    If (dr("BillMonth")) Then
                        If IsNothing(Request.QueryString("ID")) Then
                            Dim policyentry As String
                            policyentry = "Select * from reimbursementdetails" & Session("WebtableID") & " where transactiondate Between '" & FromDate.ToString("dd/MM/yyyy") & "' and '" & ToDate.ToString("dd/MM/yyyy") & "' and Month(TransactionDate)=" & radDatePickerTransactionDate.SelectedDate.Value.Month & " and Reimbursetype_code=" & ReimbursementTypeID & " and Emp_Code='" & Session("Emp_Code") & "'"

                            Dim dtcheckmonth As DataTable

                            dtcheckmonth = OldNewConn.GetDataTable2(policyentry)

                            If (dtcheckmonth.Rows.Count > 0) Then
                                Me.lblError.Text = "Already claimed for this month."
                                Exit Sub
                            End If
                        End If
                    End If
                End If

                Try
                    If Date.TryParseExact(strTransactionDate.Date.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                        TransactionDate = d
                    Else
                        Me.lblError.Text = "" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!"
                        Exit Sub
                    End If
                Catch ex As Threading.ThreadAbortException
                Catch ex As Exception
                    Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                    Exit Sub
                End Try

                dict.Add("TransactionDate", TransactionDate.Date.ToString("MM/dd/yyyy"))

                If (dr("NeedExtraDateField")) Then

                    Dim ExtraDateField As Date = radDatepickerExtraDateField.SelectedDate.Value.Date
                    Dim dtExtraDateField As Date
                    Try
                        If Date.TryParseExact(ExtraDateField.Date.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                            dtExtraDateField = d
                        Else
                            Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                            Exit Sub
                        End If
                    Catch ex As Threading.ThreadAbortException
                    Catch ex As Exception
                        Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                        Exit Sub
                    End Try
                    dict.Add("ExtraDateField", dtExtraDateField.Date.ToString("MM/dd/yyyy"))
                End If

                If Not (dr("TransactionTitle") = "") Then
                    dict.Add("TransactionDetail", TransactionDetail.Text)
                Else
                    dict.Add("TransactionDetail", String.Empty)
                End If

                If Not (dr("ExtraField1") = "") Then
                    dict.Add("ExtraField1", ExtraField1.Text)
                Else
                    dict.Add("ExtraField1", String.Empty)
                End If

                If Not (dr("ExtraField2") = "") Then
                    dict.Add("ExtraField2", ExtraField2.Text)
                Else
                    dict.Add("ExtraField2", String.Empty)
                End If

                If Not (dr("ExtraField3") = "") Then
                    dict.Add("ExtraField3", ExtraField3.Text)
                Else
                    dict.Add("ExtraField3", String.Empty)
                End If

                If Not (dr("ExtraField5") = "") Then
                    dict.Add("ExtraField5", ExtraField5.Text)
                Else
                    dict.Add("ExtraField5", String.Empty)
                End If

                If Not (dr("ExtraField4") = "") Then
                    dict.Add("ExtraField4", ExtraField4.SelectedItem.Text)
                Else
                    dict.Add("ExtraField4", String.Empty)
                End If

                If Not (dr("ExtraField6") = "") Then
                    dict.Add("ExtraField6", ExtraField6.SelectedItem.Text)
                Else
                    dict.Add("ExtraField6", String.Empty)
                End If

                If Not (dr("ExtraField7") = "") Then

                    Dim ExtraDateField As Date = radExtraField7.SelectedDate.Value.Date
                    Dim dtExtraField7new As Date
                    Try
                        If Date.TryParseExact(ExtraDateField.Date.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                            dtExtraField7new = d
                        Else
                            Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                            Exit Sub
                        End If
                    Catch ex As Threading.ThreadAbortException
                    Catch ex As Exception
                        Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                        Exit Sub
                    End Try

                    dict.Add("ExtraField7", dtExtraField7new.Date.ToString("MM/dd/yyyy"))
                End If

                If Not (dr("ExtraField8") = "") Then

                    Dim ExtraDateField As Date = radExtraField8.SelectedDate.Value.Date
                    Dim dtExtraField8new As Date
                    Try
                        If Date.TryParseExact(ExtraDateField.Date.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                            dtExtraField8new = d
                        Else
                            Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                            Exit Sub
                        End If
                    Catch ex As Threading.ThreadAbortException
                    Catch ex As Exception
                        Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                        Exit Sub
                    End Try

                    dict.Add("ExtraField8", dtExtraField8new.Date.ToString("MM/dd/yyyy"))
                End If

                'Leave from 
                Dim ExtraField7 As Date = radExtraField7.SelectedDate.Value
                Dim dtExtraField7 As Date
                Dim dExtraField7 As Date

                If Date.TryParseExact(ExtraField7.Date.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, dExtraField7) Then
                    dtExtraField7 = dExtraField7
                End If

                Dim ExtraField8 As Date = radExtraField8.SelectedDate.Value.Date
                Dim dtExtraField8 As Date
                Dim dExtraField8 As Date

                If Date.TryParseExact(ExtraField8.Date.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, dExtraField8) Then
                    dtExtraField8 = dExtraField8
                End If
                Dim weeklyoffcheck As Boolean = WeeklyOffchecking()
                If weeklyoffcheck = True Then
                    If (dtExtraField7.Date.DayOfWeek.ToString = DayOfWeek.Saturday.ToString) Then
                        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave from date should not be weekly off');", True)
                        Exit Sub
                    End If
                    If (dtExtraField8.Date.DayOfWeek.ToString = DayOfWeek.Saturday.ToString) Then
                        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave taken to date should not be weekly off');", True)
                        Exit Sub
                    End If
                End If

                If (dtExtraField7.Date.DayOfWeek.ToString = DayOfWeek.Sunday.ToString) Then
                    ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave from date should not be weekly off');", True)
                    Exit Sub
                End If
                If (dtExtraField8.Date.DayOfWeek.ToString = DayOfWeek.Sunday.ToString) Then
                    ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave taken to date should not be weekly off');", True)
                    Exit Sub
                End If
                'leave to date

                'Journey from
                Dim Journeyfrom As Date = radDatePickerTransactionDate.SelectedDate.Value
                Dim dtJourneyfrom As Date
                Dim d7 As Date

                If Date.TryParseExact(Journeyfrom.Date.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d7) Then
                    dtJourneyfrom = d7
                End If


                Dim Journeyto As Date = radDatepickerExtraDateField.SelectedDate.Value
                Dim dtJourneyto As Date
                Dim d8 As Date

                If Date.TryParseExact(Journeyto.Date.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d8) Then
                    dtJourneyto = d8
                End If

                If (radDatePickerTransactionDate.SelectedDate.Value.Date.DayOfWeek.ToString = DayOfWeek.Saturday.ToString) Then
                    dtJourneyfrom = dtJourneyfrom.AddDays(+2)

                    If dtExtraField7.Date = dtJourneyfrom.Date Then
                    ElseIf dtExtraField7.Date < dtJourneyfrom.Date Then
                    Else
                        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave dates do not match with journey period');", True)
                        Exit Sub
                    End If

                ElseIf (radDatePickerTransactionDate.SelectedDate.Value.Date.DayOfWeek.ToString = DayOfWeek.Sunday.ToString) Then
                    dtJourneyfrom = dtJourneyfrom.AddDays(+1)

                    If dtExtraField7.Date = dtJourneyfrom.Date Then
                    ElseIf dtExtraField7.Date < dtJourneyfrom.Date Then
                    Else
                        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave dates do not match with journey period');", True)
                        Exit Sub
                    End If
                ElseIf dtExtraField7.Date <= dtJourneyfrom.Date Then
                Else
                    Dim checkholidayfrom As Boolean = holidaylistcheck(dtJourneyfrom.Date)
                    If checkholidayfrom = True Then
                    Else
                        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave dates do not match with journey period');", True)
                        Exit Sub
                    End If

                End If

                If (radDatepickerExtraDateField.SelectedDate.Value.Date.DayOfWeek.ToString = DayOfWeek.Saturday.ToString) Then
                    dtJourneyto = dtJourneyto.AddDays(-1)

                    If dtExtraField8.Date = dtJourneyto.Date Then
                    ElseIf dtExtraField8.Date > dtJourneyto.Date Then
                    Else
                        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave dates do not match with journey period');", True)
                        Exit Sub
                    End If


                ElseIf (radDatepickerExtraDateField.SelectedDate.Value.Date.DayOfWeek.ToString = DayOfWeek.Sunday.ToString) Then
                    dtJourneyto = dtJourneyto.AddDays(-2)

                    If dtExtraField8.Date = dtJourneyto.Date Then
                    ElseIf dtExtraField8.Date > dtJourneyto.Date Then
                    Else
                        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave dates do not match with journey period');", True)
                        Exit Sub
                    End If
                ElseIf dtExtraField8.Date >= dtJourneyto.Date Then
                Else
                    Dim checkholidayto As Boolean = holidaylistcheck(dtJourneyto.Date)
                    If checkholidayto = True Then
                    Else
                        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave dates do not match with journey period');", True)
                        Exit Sub
                    End If
                End If
                Dim checkholidaylist As Boolean = holidaylistcheck(dtExtraField7.Date)
                If checkholidaylist = True Then
                    ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave From date is falling on holiday, please enter dates excluding holiday');", True)
                    Exit Sub
                End If

                checkholidaylist = holidaylistcheck(dtExtraField8.Date)
                If checkholidaylist = True Then
                    ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Leave To date is falling on holiday, please enter dates excluding holiday');", True)
                    Exit Sub
                End If


                Dim dtCheckLeaveDetails As DataTable
                dtCheckLeaveDetails = OldNewConn.GetDataTable2("select * from PayslipSetup" & Session("WebTableID") & " where Name='CheckLeaveDetails'")

                If (dtCheckLeaveDetails.Rows.Count > 0) Then
                    If (dtCheckLeaveDetails.Rows(0)("Print_name").ToString.ToLower = "y") Then
                        Dim startDate As Date = radExtraField7.SelectedDate
                        Dim endDate As Date = radExtraField8.SelectedDate
                        While (startDate <= endDate)
                            Dim dtLeaveCheck As DataTable = OldNewConn.GetDataTable2("Select * from leavedetails" & Session("WebTableID") & "_" & startDate.Year & " where emp_code='" & Session("Emp_Code") & "' and (Fromdate <= '" & Format(startDate, "MM/dd/yyyy") & "'  and Todate >= '" & Format(startDate, "MM/dd/yyyy") & "' ) and status=1")
                            If (dtLeaveCheck.Rows.Count = 0) Then
                                Dim dtcheckHolidayIndia As DataTable = OldNewConn.GetDataTable2("Select S.Name from employeesmaster" & Session("WebTableID") & " E inner join Sectionmaster" & Session("WebTableID") & " S on S.Section_Code=E.Section_Code where E.emp_code='" & Session("Emp_Code") & "'")
                                If (dtcheckHolidayIndia.Rows.Count > 0) Then
                                    Dim dtCheckHoliday As DataTable = OldNewConn.GetDataTable2("Select * from Holidaylist" & Session("WebTableID") & "_" & startDate.Year & "   Where HolidayDate='" & Format(startDate, "MM/dd/yyyy") & "' and HolidayFor in ('" & dtcheckHolidayIndia.Rows(0)("Name") & "','')")
                                    If (dtCheckHoliday.Rows.Count = 0) Then
                                        If Not (startDate.DayOfWeek = DayOfWeek.Saturday Or startDate.DayOfWeek = DayOfWeek.Sunday) Then
                                            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('YOU DO NOT HAVE APPROVED LEAVES FOR THE JOURNEY PERIOD HENCE TAX FREE LTA CAN NOT BE CLAIMED');", True)
                                            Exit Sub
                                        End If
                                    End If
                                End If
                            End If
                            startDate = startDate.AddDays(1)
                        End While
                    End If
                End If


                'Journey to end



                If Not (dr("TotalBillField") = "") Then
                    dict.Add("TotalBillField", txtTotalBillField.Text)
                Else
                    dict.Add("TotalBillField", String.Empty)
                End If


                Dim totalAmt1 As Decimal = 0.0

                If Not (dr("ClaimAmountField") = "") Then
                    dict.Add("ClaimAmount", Amount.Text)
                Else
                    dict.Add("ClaimAmount", totalAmt1)
                End If

                dict.Add("BillPassed", 0)

                If dr("Disclaimer") <> "" Then
                    If Not (chkDisclaimer.Checked) Then
                        Response.Write("<script>alert('Please check the Declaration');</script>")
                        Return
                    End If
                End If

                Dim dtEntryDate As Date = radDatepickerEntryDate.SelectedDate.Value.Date

                dict.Add("EntryDate", Format(dtEntryDate, "MM/dd/yyyy"))

                dict.Add("Rejected", 0)

                Dim hash As New HashSet(Of String)

                Dim totalAmt As Decimal

                If (dr("isPersonsDetailRequired")) Then

                    If (radGridTravel.MasterTableView.Items.Count = 0) Then
                        Response.Write("<script>alert('PLEASE SUBMIT JOURNEY DETAIL ABOVE OR YOU CAN UPLOAD THE SAME BY USING CSV TEMPLATE');</script>")
                        Exit Sub
                    End If

                    Dim noOfchildren As Int32 = 0
                    Dim Cmdgender As String = ""
                    Dim noOffather As Int32 = 0
                    Dim noOfmother As Int32 = 0
                    Dim Self As Int32 = 0
                    Dim spouse As Int32 = 0
                    Dim dt As New DataTable
                    Dim distinctTable As New DataTable

                    dt.Columns.Add("PersonsName")
                    dt.Columns.Add("RelationshipWithEmployee")

                    Dim drNew As DataRow

                    If (panelTotalBill.Visible) Then

                        For Each gridDataItem As GridDataItem In radGridTravel.Items
                            If Not IsNothing(gridDataItem("RelationshipWithEmployee").Text) Then
                                Dim a1 As String = gridDataItem("PersonsName").Text
                                Dim a2 As String = gridDataItem("RelationshipWithEmployee").Text
                                Dim Dependence As String = gridDataItem("Dependence").Text
                                drNew = dt.NewRow()
                                Try
                                    If (a2.ToString.ToLower <> "self" And a2.ToString.ToLower <> "spouse" And a2.ToString.ToLower <> "son" And a2.ToString.ToLower <> "daughter") Then
                                        If (Dependence.ToString.ToLower = "y") Then
                                            drNew("PersonsName") = a1
                                            drNew("RelationshipWithEmployee") = a2
                                        ElseIf (Dependence.ToString.ToLower = "n") Then
                                            Response.Write("<script>alert('Can claim for father, mother, brother and sister only if dependent on you.');</script>")
                                            Exit Sub
                                        End If
                                    Else
                                        drNew("PersonsName") = a1
                                        drNew("RelationshipWithEmployee") = a2
                                    End If
                                    dt.Rows.Add(drNew)
                                Catch ex As Exception
                                    Response.Write("<script>alert('" & String.Format("{0}", ex.Message) & "');</script>")
                                End Try
                            End If
                        Next

                        distinctTable = dt.DefaultView.ToTable(True)

                        If Not (distinctTable.Rows.Count = txtTotalBillField.Text) Then
                            Response.Write("<script>alert('" & String.Format("{0} is not matching", lblTotalBillField.Text) & "');</script>")
                            Exit Sub
                        Else
                            If (txtTotalBillField.Text > distinctTable.Rows.Count) Then
                                Response.Write("<script>alert('Total Member Travelled is not matching.');</script>")
                                Exit Sub
                            End If
                        End If
                    End If


                    Cmdgender = "select Sex as Gender,MaritalStatus from employeesmaster" & Session("WebTableID") & "  where emp_code='" & Session("Emp_Code").ToString() & "'"

                    Dim dtGender As DataTable

                    dtGender = OldNewConn.GetDataTable2(Cmdgender)

                    For i As Integer = 0 To distinctTable.Rows.Count - 1

                        Dim RelationshipwithEmployee As String = distinctTable.Rows(i)("RelationshipWithEmployee").ToString().ToLower()

                        If (RelationshipwithEmployee = "self") Then
                            Self = Self + 1
                        End If

                        If (RelationshipwithEmployee = "spouse") Then
                            spouse = spouse + 1
                        End If

                        If (RelationshipwithEmployee = "son") Then
                            noOfchildren = noOfchildren + 1
                        End If

                        If (RelationshipwithEmployee = "daughter") Then
                            noOfchildren = noOfchildren + 1
                        End If

                        If (RelationshipwithEmployee = "father") Then
                            noOffather = noOffather + 1
                        End If

                        If (RelationshipwithEmployee = "mother") Then
                            noOfmother = noOfmother + 1
                        End If
                    Next

                    If (Self = 0) Then
                        Response.Write("<script>alert('As per LTA Rules, LTA exemption can be taken only if employee is part of the journey');</script>")
                        Exit Sub
                    End If

                    If (spouse > 1) Then
                        Response.Write("<script>alert('Spouse should not exceed 1');</script>")
                        Exit Sub
                    End If

                    If (noOfchildren > 2) Then
                        Response.Write("<script>alert('Children should not exceed 2 (inluding both son and daughter)');</script>")
                        Exit Sub
                    End If

                    If (noOffather > 1) Then
                        Response.Write("<script>alert('Father should not execeed 1');</script>")
                        Exit Sub
                    End If

                    If (noOfmother > 1) Then
                        Response.Write("<script>alert('Mother should not execeed 1');</script>")
                        Exit Sub
                    End If

                    If (noOffather + noOfmother > 2) Then
                        Response.Write("<script>alert('Parent should not execeed 2');</script>")
                        Exit Sub
                    End If

                    If (dtGender.Rows(0)("Gender").ToString() = "F") Then
                        'If (dtGender.Rows(0)("MaritalStatus").ToString() = "M") Then
                        '    If (noOffather + noOfmother > 0) Then
                        '        Response.Write("<script>alert('Married women can not claim for father and mother');</script>")
                        '        Exit Sub
                        '    End If
                        'ElseIf (dtGender.Rows(0)("MaritalStatus").ToString() = "S") Then
                        '    If ((spouse > 0 Or noOfchildren > 0) And (noOffather + noOfmother) > 0) Then
                        '        Response.Write("<script>alert('Married women can not claim for father and mother');</script>")
                        '        Exit Sub
                        '    End If
                        'End If
                    ElseIf (dtGender.Rows(0)("Gender").ToString() = "M") Then

                    End If

                    For Each r As GridDataItem In radGridTravel.MasterTableView.Items

                        Dim JourneyDate As Date = DateTime.ParseExact(r("JourneyDate").Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                        Dim ExtraDateField As Date = radDatepickerExtraDateField.SelectedDate.Value.Date

                        If JourneyDate.Date > Today.Date Then
                            Response.Write("<script>alert('You can not submit any post dated claims.');</script>")
                            Exit Sub
                        End If
                        If Not (JourneyDate.Date >= strTransactionDate.Date And JourneyDate.Date <= ExtraDateField.Date) Then
                            Response.Write("<script>alert('Journey date does not match with what is specified above');</script>")
                            Exit Sub
                        End If
                        totalAmt = totalAmt + r("ClaimAmount").Text
                    Next  'End of 6th

                    If Not (totalAmt = Amount.Text) Then
                        Response.Write("<script>alert('" & String.Format("Total amount spent on journey is not matching with the journey detail entered.") & "');</script>")
                        Exit Sub
                    End If

                    If Not (totalAmt = Amount.Text) Then
                        Response.Write("<script>alert('" & String.Format("{0} is not matching", lblAmount.Text) & "');</script>")
                        Exit Sub
                    End If
                End If

                'Dim dtClaimAmountField As DataTable
                'dtClaimAmountField = OldNewConn.GetDataTable2("select * from PayslipSetup" & Session("WebTableID") & " where Name='RemoveClaimAmountField'")

                'Dim flag As Boolean = False

                'If (dtClaimAmountField.Rows.Count > 0) Then

                '    Dim FieldName As String() = dtClaimAmountField.Rows(0)("Print_Name").ToString().Split(",")

                '    For i As Int32 = 0 To FieldName.Count - 1
                '        If (type.ToString().ToLower() = FieldName(i).ToString().ToLower()) Then
                '            flag = True
                '        End If
                '    Next
                'End If

                'If (flag = True) Then

                'ElseIf flag = False Then
                '    If Not (dr("ClaimAmountField") = "") Then
                '        If (CInt(Session("BalancePaid")) < CInt(Amount.Text)) Then
                '            Response.Write("<script>alert('" & Session("BalancePaidMsg") & "');</script>")
                '            Exit Sub
                '        End If
                '    ElseIf (CInt(Session("BalancePaid")) < CInt(totalAmt1)) Then
                '        Response.Write("<script>alert('" & Session("BalancePaidMsg") & "');</script>")
                '        Exit Sub
                '    End If
                'End If

                If (dr("IsUpload")) Then
                    If (ReimbursementID = 0) Then
                        If (IsNothing(Session("UploadedFile"))) Then
                            Response.Write("<script>alert('Please upload bills to submit claim.');</script>")
                            Exit Sub
                        End If
                    End If
                End If

                ID = InsertIntoReimburseDetails(dict, ReimbursementID)    'Insert into reimbursedetails

                If (dr("isPersonsDetailRequired")) Then

                    Dim sqlParameter(11) As SqlParameter

                    If (ID <> 0) Then
                        DeleteOtherReimbursementPersonsDetail(ReimbursementID)
                    End If

                    For Each r As GridDataItem In radGridTravel.MasterTableView.Items '7th

                        sqlParameter(0) = New SqlParameter
                        sqlParameter(0).DbType = DbType.String
                        sqlParameter(0).ParameterName = "@ReimburseDetailsId"
                        If (ReimbursementID = 0) Then
                            sqlParameter(0).Value = ID
                        Else
                            sqlParameter(0).Value = ReimbursementID
                        End If

                        sqlParameter(1) = New SqlParameter
                        sqlParameter(1).DbType = DbType.String
                        sqlParameter(1).ParameterName = "@Emp_code"
                        sqlParameter(1).Value = Session("Emp_code").ToString()

                        sqlParameter(2) = New SqlParameter
                        sqlParameter(2).DbType = DbType.String
                        sqlParameter(2).ParameterName = "@PersonsName"
                        sqlParameter(2).Value = r("PersonsName").Text

                        sqlParameter(3) = New SqlParameter
                        sqlParameter(3).DbType = DbType.String
                        sqlParameter(3).ParameterName = "@Dependence"

                        If (r("Dependence").Text.ToLower() = "y") Then
                            sqlParameter(3).Value = "Y"
                        ElseIf (r("Dependence").Text.ToLower() = "n") Then
                            sqlParameter(3).Value = "N"
                        End If

                        sqlParameter(4) = New SqlParameter
                        sqlParameter(4).DbType = DbType.String
                        sqlParameter(4).ParameterName = "@RelationshipwithEmployee"
                        sqlParameter(4).Value = r("RelationshipwithEmployee").Text

                        sqlParameter(5) = New SqlParameter
                        sqlParameter(5).DbType = DbType.String
                        sqlParameter(5).ParameterName = "@From"
                        sqlParameter(5).Value = r("From").Text

                        sqlParameter(6) = New SqlParameter
                        sqlParameter(6).DbType = DbType.String
                        sqlParameter(6).ParameterName = "@To"
                        sqlParameter(6).Value = r("To").Text

                        sqlParameter(7) = New SqlParameter
                        sqlParameter(7).DbType = DbType.DateTime
                        sqlParameter(7).ParameterName = "@JourneyDate"
                        sqlParameter(7).Value = DateTime.ParseExact(r("JourneyDate").Text, "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture)

                        sqlParameter(8) = New SqlParameter
                        sqlParameter(8).DbType = DbType.String
                        sqlParameter(8).ParameterName = "@ModeOfJourney"
                        sqlParameter(8).Value = r("ModeOfJourney").Text

                        sqlParameter(9) = New SqlParameter
                        sqlParameter(9).DbType = DbType.String
                        sqlParameter(9).ParameterName = "@TypeOfJourney"
                        sqlParameter(9).Value = r("TypeOfJourney").Text

                        sqlParameter(10) = New SqlParameter
                        sqlParameter(10).DbType = DbType.String
                        sqlParameter(10).ParameterName = "@ClaimAmount"
                        sqlParameter(10).Value = r("ClaimAmount").Text

                        sqlParameter(11) = New SqlParameter
                        sqlParameter(11).DbType = DbType.Binary
                        sqlParameter(11).ParameterName = "@fileData"
                        Dim buffer As Byte() = New Byte() {}
                        sqlParameter(11).Value = buffer

                        If (ID <> 0) Then
                            Dim claimId As Integer = InsertIntoReimbursementPersonsDetail(sqlParameter)
                        End If
                    Next
                End If

                'Upload bill
                Dim count As Integer = 0
                If (dr("IsUpload")) Then
                    Dim dicUploadFile As New Dictionary(Of Integer, UploadedFile)

                    Dim sqlParameter(7) As SqlParameter

                    If Not IsNothing(Session("UploadedFile")) Then

                        If (ID <> 0) Then
                            DeleteReimbursementProofUpload(ReimbursementID)
                        End If

                        dicUploadFile = Session("UploadedFile")

                        For Each rad As UploadedFile In dicUploadFile.Values

                            sqlParameter(0) = New SqlParameter
                            sqlParameter(0).DbType = DbType.String
                            sqlParameter(0).ParameterName = "@Emp_code"
                            sqlParameter(0).Value = Session("Emp_code").ToString()

                            Dim fStream As FileStream = rad.InputStream

                            Dim contents As Byte() = New Byte(fStream.Length - 1) {}

                            fStream.Read(contents, 0, CInt(fStream.Length))

                            fStream.Close()

                            sqlParameter(1) = New SqlParameter
                            sqlParameter(1).DbType = DbType.Binary
                            sqlParameter(1).ParameterName = "@fileData"
                            sqlParameter(1).Value = DirectCast(contents, Byte())

                            sqlParameter(2) = New SqlParameter
                            sqlParameter(2).DbType = DbType.Int32
                            sqlParameter(2).ParameterName = "@Reimbursetype_Code"
                            sqlParameter(2).Value = ReimbursementTypeID

                            sqlParameter(3) = New SqlParameter
                            sqlParameter(3).DbType = DbType.Int32
                            sqlParameter(3).ParameterName = "@ReimburseDetailsId"
                            If (ReimbursementID = 0) Then
                                sqlParameter(3).Value = ID
                            Else
                                sqlParameter(3).Value = ReimbursementID
                            End If

                            sqlParameter(4) = New SqlParameter
                            sqlParameter(4).DbType = DbType.Int32
                            sqlParameter(4).ParameterName = "@IsApproved"
                            sqlParameter(4).Value = 0

                            sqlParameter(5) = New SqlParameter
                            sqlParameter(5).DbType = DbType.String
                            sqlParameter(5).ParameterName = "@Remarks"
                            sqlParameter(5).Value = ""

                            sqlParameter(6) = New SqlParameter
                            sqlParameter(6).DbType = DbType.DateTime
                            sqlParameter(6).ParameterName = "@SentDate"
                            sqlParameter(6).Value = DBNull.Value

                            sqlParameter(7) = New SqlParameter
                            sqlParameter(7).DbType = DbType.String
                            sqlParameter(7).ParameterName = "@Ext"
                            sqlParameter(7).Value = rad.GetExtension()

                            If (ID <> 0) Then
                                Dim uploadId As Integer = InsertIntoReimbursementProofUpload(sqlParameter)
                            End If
                        Next
                    End If
                End If
            Next

            If IsNothing(Request.QueryString("ID")) Then  ' if not redirected from view/edit page
                Dim dtMailDetail As DataTable = OldNewConn.GetDataTable2("select Field_Detail as MailSubject, Print_Name as MailBody from PayslipSetup" & Session("WebTableID") & " where Name='ReimburseMail'")
                If (dtMailDetail.Rows.Count > 0) Then
                    Dim sb As System.Text.StringBuilder = New StringBuilder()

                    Dim Mailsubject As String = dtMailDetail.Rows(0)("MailSubject").ToString()
                    Dim MailBody As String = dtMailDetail.Rows(0)("MailBody").ToString()

                    Dim sql_Query As String = "select Top 1 ReimburseDetailsID,IsNull(PSS.Print_Name,RTM.Name) as 'Field_Name',convert(nvarchar,EntryDate,106) as 'EntryDate',case when ClaimAmount=0 And RD.ExtraField3 = 0 then RD.ExtraField5 when ClaimAmount=0 then RD.ExtraField2 else ClaimAmount End as 'BillAmount' from reimbursementdetails" & Session("WebTableID") & " RD inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RTM.ReimburseType_Code=RD.ReimburseType_Code Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RTM.Name where emp_Code = '" & Session("Emp_code") & "' And RTM.Name = '" & type & "' And ReimburseDetailsID=" & ID & "  order by ReimburseDetailsID desc"
                    Dim dsReimburseSummary As DataTable = OldNewConn.GetDataTable2(sql_Query)

                    sb.Append("<TABLE border=1 cellPadding=0 width=100% cellSpacing=0 style=" & Chr(34) & "font-family: Segoe UI,Verdana,Arial,Georgia;font-size:10pt" & Chr(34) & " width=60%>")
                    sb.Append("<tr><th>Claim ID</th><th>Reimburse Type</th><th>Claim date</th><th>Claim Amount</th></tr>")

                    For j As Integer = 0 To dsReimburseSummary.Rows.Count - 1
                        sb.Append("<tr>")
                        sb.Append("<td>" & dsReimburseSummary.Rows(j)("ReimburseDetailsID").ToString & "</td>")
                        sb.Append("<td>" & dsReimburseSummary.Rows(j)("Field_Name").ToString & "</td>")
                        sb.Append("<td>" & CDate(dsReimburseSummary.Rows(j)("EntryDate")).Date.ToString("dd/MM/yyyy") & "</td>")
                        sb.Append("<td>" & CDec(dsReimburseSummary.Rows(j)("BillAmount")).ToString("####.00") & "</td>")
                        sb.Append("</tr>")
                    Next
                    sb.Append("</TABLE><br/>")

                    MailBody = Replace(MailBody, "#Summary#", sb.ToString)

                    sb.Clear()

                    Dim Str As String = "Select FirstName+' '+ LastName as EmployeeName, Email from EmployeesMaster" & Session("WebTableID") & " where Emp_Code='" & Session("Emp_code").ToString & "'"
                    Dim dtEmployeeEmail As DataTable = OldNewConn.GetDataTable2(Str)

                    sb.Append("<TABLE border=1 cellPadding=0 width=100% cellSpacing=0 style=" & Chr(34) & "font-family: Segoe UI,Verdana,Arial,Georgia;font-size:10pt" & Chr(34) & " width=60%>")
                    sb.Append("<tr><th>Name of the Member Travelled</th><th>Dependent (Y/N)</th><th>Relationship with employee</th><th>Travel from</th><th>Travel To</th><th>Travel on</th><th>Travel mode</th><th>Travel type</th><th>Travel fare in Rs</th></tr>")

                    Dim dtReimbursetable As DataTable = OldNewConn.GetDataTable2("select * from OtherReimbursementPersonsDetail" & Session("WebTableID") & " where emp_Code = '" & Session("Emp_code") & "' And ReimburseDetailsID=" & ID & "  order by ReimbursementPersonsDetailID desc")

                    For j As Integer = 0 To dtReimbursetable.Rows.Count - 1
                        sb.Append("<tr>")
                        sb.Append("<td>" & dtReimbursetable.Rows(j)("PersonsName").ToString & "</td>")
                        sb.Append("<td>" & dtReimbursetable.Rows(j)("Dependence").ToString & "</td>")
                        sb.Append("<td>" & dtReimbursetable.Rows(j)("RelationshipwithEmployee").ToString & "</td>")
                        sb.Append("<td>" & dtReimbursetable.Rows(j)("From").ToString & "</td>")
                        sb.Append("<td>" & dtReimbursetable.Rows(j)("To").ToString & "</td>")
                        sb.Append("<td>" & CDate(dtReimbursetable.Rows(j)("JourneyDate")).Date.ToString("dd/MM/yyyy") & "</td>")
                        sb.Append("<td>" & dtReimbursetable.Rows(j)("ModeOfJourney").ToString & "</td>")
                        sb.Append("<td>" & dtReimbursetable.Rows(j)("TypeOfJourney").ToString & "</td>")
                        sb.Append("<td>" & CDec(dtReimbursetable.Rows(j)("ClaimAmount")).ToString("####.00") & "</td>")
                        sb.Append("</tr>")
                    Next
                    sb.Append("</TABLE><br/>")

                    If (dtEmployeeEmail.Rows.Count > 0) Then

                        MailBody = Replace(MailBody, "#EmployeeName#", StrConv(dtEmployeeEmail.Rows(0)("EmployeeName").ToString, VbStrConv.ProperCase))
                        MailBody = Replace(MailBody, "#CurrentStatus#", sb.ToString)

                        If (dsReimburseSummary.Rows.Count > 0 And dtReimbursetable.Rows.Count > 0) Then
                            If Not IsDBNull(dtEmployeeEmail.Rows(0)("EMail").ToString()) Then
                                If (dtEmployeeEmail.Rows(0)("EMail").ToString() <> "") Then
                                    Dim flag As Boolean = SendEmail(Session("Emp_code").ToString, dtEmployeeEmail.Rows(0)("EMail").ToString(), Mailsubject, MailBody, "", "", "from5", "subject5", "password5")
                                    If (flag = True) Then
                                        Enter_EmailLog(Session("Emp_code").ToString, Session("WebTableID"), 1, "DKMWebApplication", Mailsubject, "To : " & dtEmployeeEmail.Rows(0)("EMail").ToString(), MailBody, "", Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type)
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

                lblsave.Visible = True
                btnSave.Enabled = False
                lnkTravel.Enabled = False
                btnUpload.Enabled = False
                linkCSVTemplate.Enabled = False
                radAsynUpload.Enabled = False
                Session("ReimbursetableTravel") = Nothing
                Session("ReimbursetableMultiple") = Nothing
                Session("TempDict") = Nothing
                If Session("WebTableID") = 772 Or Session("WebTableID") = 773 Then
                    ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Reimbursement is submitted but claim will be processed with next month payroll if submitted between 16th to 31st of the month');window.location='ReimEditDetails.aspx?Type=" & EncryDecrypt.Encrypt(type, "a") & "&IDD=" & EncryDecrypt.Encrypt(ReimbursementTypeID, "a") & "';", True)
                Else
                    ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Claim submitted, if to submit any further claim, that can be done from ""submit your claim"" or ""view/edit claim"" tab.');window.location='ReimEditDetails.aspx?Type=" & EncryDecrypt.Encrypt(type, "a") & "&IDD=" & EncryDecrypt.Encrypt(ReimbursementTypeID, "a") & "';", True)
                End If

            Else
                Dim dtMailDetail As DataTable = OldNewConn.GetDataTable2("select Field_Detail as MailSubject, Print_Name as MailBody from PayslipSetup" & Session("WebTableID") & " where Name='ReimburseMail'")
                If (dtMailDetail.Rows.Count > 0) Then
                    Dim sb As System.Text.StringBuilder = New StringBuilder()

                    Dim Mailsubject As String = dtMailDetail.Rows(0)("MailSubject").ToString()
                    Dim MailBody As String = dtMailDetail.Rows(0)("MailBody").ToString()

                    Dim sql_Query As String = "select Top 1 ReimburseDetailsID,IsNull(PSS.Print_Name,RTM.Name) as 'Field_Name',convert(nvarchar,EntryDate,106) as 'EntryDate',case when ClaimAmount=0 And RD.ExtraField3 = 0 then RD.ExtraField5 when ClaimAmount=0 then RD.ExtraField2 else ClaimAmount End as 'BillAmount' from reimbursementdetails" & Session("WebTableID") & " RD inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RTM.ReimburseType_Code=RD.ReimburseType_Code Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RTM.Name where emp_Code = '" & Session("Emp_code") & "' And  ReimburseDetailsID=" & ReimbursementID & ""
                    Dim dsReimburseSummary As DataTable = OldNewConn.GetDataTable2(sql_Query)

                    sb.Append("<TABLE border=1 cellPadding=0 width=100% cellSpacing=0 style=" & Chr(34) & "font-family: Segoe UI,Verdana,Arial,Georgia;font-size:10pt" & Chr(34) & " width=60%>")
                    sb.Append("<tr><th>Claim ID</th><th>Reimburse Type</th><th>Claim date</th><th>Claim Amount</th></tr>")

                    For j As Integer = 0 To dsReimburseSummary.Rows.Count - 1
                        sb.Append("<tr>")
                        sb.Append("<td>" & dsReimburseSummary.Rows(j)("ReimburseDetailsID").ToString & "</td>")
                        sb.Append("<td>" & dsReimburseSummary.Rows(j)("Field_Name").ToString & "</td>")
                        sb.Append("<td>" & CDate(dsReimburseSummary.Rows(j)("EntryDate")).Date.ToString("dd/MM/yyyy") & "</td>")
                        sb.Append("<td>" & CDec(dsReimburseSummary.Rows(j)("BillAmount")).ToString("####.00") & "</td>")
                        sb.Append("</tr>")
                    Next
                    sb.Append("</TABLE><br/>")

                    MailBody = Replace(MailBody, "#Summary#", sb.ToString)

                    sb.Clear()

                    Dim Str As String = "Select FirstName+' '+ LastName as EmployeeName, Email from EmployeesMaster" & Session("WebTableID") & " where Emp_Code='" & Session("Emp_code").ToString & "'"
                    Dim dtEmployeeEmail As DataTable = OldNewConn.GetDataTable2(Str)

                    sb.Append("<TABLE border=1 cellPadding=0 width=100% cellSpacing=0 style=" & Chr(34) & "font-family: Segoe UI,Verdana,Arial,Georgia;font-size:10pt" & Chr(34) & " width=60%>")
                    sb.Append("<tr><th>Name of the Member Travelled</th><th>Dependent (Y/N)</th><th>Relationship with employee</th><th>Travel from</th><th>Travel To</th><th>Travel on</th><th>Travel mode</th><th>Travel type</th><th>Travel fare in Rs</th></tr>")

                    Dim dtReimbursetable As DataTable = OldNewConn.GetDataTable2("select * from OtherReimbursementPersonsDetail" & Session("WebTableID") & " where emp_Code = '" & Session("Emp_code") & "' And ReimburseDetailsID=" & ReimbursementID & "  order by ReimbursementPersonsDetailID desc")

                    For j As Integer = 0 To dtReimbursetable.Rows.Count - 1
                        sb.Append("<tr>")
                        sb.Append("<td>" & dtReimbursetable.Rows(j)("PersonsName").ToString & "</td>")
                        sb.Append("<td>" & dtReimbursetable.Rows(j)("Dependence").ToString & "</td>")
                        sb.Append("<td>" & dtReimbursetable.Rows(j)("RelationshipwithEmployee").ToString & "</td>")
                        sb.Append("<td>" & dtReimbursetable.Rows(j)("From").ToString & "</td>")
                        sb.Append("<td>" & dtReimbursetable.Rows(j)("To").ToString & "</td>")
                        sb.Append("<td>" & CDate(dtReimbursetable.Rows(j)("JourneyDate")).Date.ToString("dd/MM/yyyy") & "</td>")
                        sb.Append("<td>" & dtReimbursetable.Rows(j)("ModeOfJourney").ToString & "</td>")
                        sb.Append("<td>" & dtReimbursetable.Rows(j)("TypeOfJourney").ToString & "</td>")
                        sb.Append("<td>" & CDec(dtReimbursetable.Rows(j)("ClaimAmount")).ToString("####.00") & "</td>")
                        sb.Append("</tr>")
                    Next

                    sb.Append("</TABLE><br/>")

                    If (dtEmployeeEmail.Rows.Count > 0) Then

                        MailBody = Replace(MailBody, "#EmployeeName#", StrConv(dtEmployeeEmail.Rows(0)("EmployeeName").ToString, VbStrConv.ProperCase))
                        MailBody = Replace(MailBody, "#CurrentStatus#", sb.ToString)

                        If (dsReimburseSummary.Rows.Count > 0 And dtReimbursetable.Rows.Count > 0) Then
                            If Not IsDBNull(dtEmployeeEmail.Rows(0)("EMail").ToString()) Then
                                If (dtEmployeeEmail.Rows(0)("EMail").ToString() <> "") Then
                                    Dim flag As Boolean = SendEmail(Session("Emp_code").ToString, dtEmployeeEmail.Rows(0)("EMail").ToString(), Mailsubject, MailBody, "", "", "from5", "subject5", "password5")
                                    If (flag = True) Then
                                        Enter_EmailLog(Session("Emp_code").ToString, Session("WebTableID"), 1, "DKMWebApplication", Mailsubject, "To : " & dtEmployeeEmail.Rows(0)("EMail").ToString(), MailBody, "", Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type)
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

                lblsave.Visible = True
                btnSave.Enabled = True
                lnkTravel.Enabled = True
                btnUpload.Enabled = True
                linkCSVTemplate.Enabled = True
                radAsynUpload.Enabled = True
                Session("ReimbursetableTravel") = Nothing
                Session("ReimbursetableMultiple") = Nothing
                Session("TempDict") = Nothing
                If Session("WebTableID") = 772 Or Session("WebTableID") = 773 Then
                    ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Reimbursement is submitted but claim will be processed with next month payroll if submitted between 16th to 31st of the month');window.location='ReimEditDetails.aspx?Type=" & EncryDecrypt.Encrypt(type, "a") & "&IDD=" & EncryDecrypt.Encrypt(ReimbursementTypeID, "a") & "';", True)
                Else
                    ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Revised claim submitted.');window.location='ReimEditDetails.aspx?Type=" & EncryDecrypt.Encrypt(type, "a") & "&IDD=" & EncryDecrypt.Encrypt(ReimbursementTypeID, "a") & "';", True)
                End If

            End If

            If IsNothing(Request.QueryString("ID")) Then  ' if not redirected from view/edit page
                IsEntrySaved = True
                radGridTravel.Enabled = False
            Else
                IsEntrySaved = False
            End If
        Catch ex As Exception
            Enter_ErrorLog(Session("Emp_Code"), Session("WebTableID"), ex.Message, ex.StackTrace, Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type)
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('" & ex.Message & "');", True)
        End Try
    End Sub

    ''' <summary>
    ''' All Function
    ''' </summary>
    ''' <param name="MultipleClaimDetailsID"></param>
    ''' <param name="ShowPersonDetail"></param>
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

    Private Function IsAlphaNum(ByVal strInputText As String) As Boolean
        Return System.Text.RegularExpressions.Regex.IsMatch(strInputText, "^[a-z|A-Z]+$")
        '[a-z|A-Z]+$
    End Function

    Public Function GetPersonalDetailsofReimbursement(ReimbursementPersonsDetailID As Integer) As DataTable
        Try
            If ReimbursementPersonsDetailID = 0 Then
                commendText = "select ROW_NUMBER() OVER (ORDER BY ReimbursementPersonsDetailID) AS 'Sl.No',PersonsName,Dependence,RelationshipWithEmployee,[From],[To],JourneyDate,ModeOfJourney,TypeOfJourney, Round(ClaimAmount,2) as  ClaimAmount,ReimbursementPersonsDetailID from OtherReimbursementPersonsDetail" & HttpContext.Current.Session("WebTableID") & " where ReimburseDetailsID =0 ORDER BY ReimbursementPersonsDetailID"
            Else
                commendText = "select ROW_NUMBER() OVER (ORDER BY ReimbursementPersonsDetailID) AS 'Sl.No',PersonsName,Dependence,RelationshipWithEmployee,[From],[To],JourneyDate,ModeOfJourney,TypeOfJourney, Round(ClaimAmount,2) as  ClaimAmount,ReimbursementPersonsDetailID from OtherReimbursementPersonsDetail" & HttpContext.Current.Session("WebTableID") & " where ReimburseDetailsID = " & ReimbursementPersonsDetailID & " ORDER BY ReimbursementPersonsDetailID"
            End If
            Return OldNewConn.GetDataTable2(commendText)
        Catch ex As Exception
            Return New DataTable
        End Try
    End Function

    Public Function DeleteOtherReimbursementPersonsDetail(ReimbursementID As Integer) As Integer
        Try
            commendText = String.Format("delete  from  OtherReimbursementPersonsDetail{0}  where Emp_Code='" & Session("Emp_Code") & "' and ReimburseDetailsId='" & ReimbursementID & "'", HttpContext.Current.Session("WebTableID"))
            Return OldNewConn.ExecuteNonQuery(CommandType.Text, commendText, Nothing)
        Catch ex As Exception
            Return 0
        End Try
    End Function

    Public Function DeleteReimbursementProofUpload(ReimbursementID As Integer) As Integer
        Try
            commendText = String.Format("delete  from  ReimbursementProofUpload{0}  where Emp_Code='" & Session("Emp_Code") & "' and ReimburseDetailsId='" & ReimbursementID & "'", HttpContext.Current.Session("WebTableID"))
            Return OldNewConn.ExecuteNonQuery(CommandType.Text, commendText, Nothing)
        Catch ex As Exception
            Return 0
        End Try
    End Function

    Public Function InsertIntoReimbursementPersonsDetail(ByVal SqlParameter As SqlParameter()) As Integer
        commendText = "Insert into OtherReimbursementPersonsDetail" & Session("WebTableID") & " (ReimburseDetailsId,Emp_Code,PersonsName,Dependence,RelationshipWithEmployee,[From],[To],JourneyDate,ModeOfJourney,TypeOfJourney,ClaimAmount,fileData) values (@ReimburseDetailsId,@Emp_Code,@PersonsName,@Dependence,@RelationshipWithEmployee,@From,@To,@JourneyDate,@ModeOfJourney,@TypeOfJourney,@ClaimAmount,@fileData);Select Scope_Identity();"
        Return OldNewConn.ExecuteScalar(CommandType.Text, commendText, SqlParameter)
    End Function

    Public Function InsertIntoReimbursementProofUpload(ByVal SqlParameter As SqlParameter()) As Integer
        commendText = "insert into ReimbursementProofUpload" & Session("WebTableID") & " (Emp_Code,fileData,Reimbursetype_Code,ReimburseDetailsId,IsApproved,Remarks,SentDate,Ext) values (@Emp_Code,@fileData,@Reimbursetype_Code,@ReimburseDetailsId,@IsApproved,@Remarks,@SentDate,@Ext);Select Scope_Identity();"
        Return OldNewConn.ExecuteNonQuery(CommandType.Text, commendText, SqlParameter)
    End Function

    Public Function InsertIntoReimburseDetails(dict As Dictionary(Of String, String), ReimbursementID As Integer) As Integer
        Try
            If (ReimbursementID = 0) Then

                Dim str As String = ""
                Dim strKeys As String = ""

                For Each keys As String In dict.Keys
                    If (strKeys = "") Then

                        strKeys = String.Format("[{0}]", keys)
                    Else
                        strKeys = strKeys & "," & String.Format("[{0}]", keys)
                    End If
                Next
                For Each value1 As String In dict.Values
                    If (str = "") Then
                        str = String.Format("'{0}'", value1)
                    Else
                        str = str + "," + String.Format("'{0}'", value1)
                    End If
                Next

                commendText = String.Format("Insert into reimbursementdetails" & Session("WebtableID") & " ({1}) values({2}) Select SCOPE_IDENTITY()", HttpContext.Current.Session("WebTableID"), strKeys, str)
                Return OldNewConn.ExecuteScalar(CommandType.Text, commendText, Nothing)
            Else
                Dim strKeys As String = ""

                For Each keyValue As KeyValuePair(Of String, String) In dict

                    If (strKeys = "") Then

                        strKeys = String.Format("{0}='{1}'", keyValue.Key, keyValue.Value)
                    Else
                        strKeys = strKeys & "," & String.Format("{0}='{1}'", keyValue.Key, keyValue.Value)
                    End If
                Next
                commendText = String.Format("Update  reimbursementdetails" & Session("WebtableID") & " set {1} where  ReimburseDetailsID=" & ReimbursementID & "", HttpContext.Current.Session("WebTableID"), strKeys)
                Return OldNewConn.ExecuteNonQuery(CommandType.Text, commendText, Nothing)
            End If

        Catch ex As Exception
            Return 0
        End Try
    End Function

    ''' <summary>
    ''' Mail Function
    ''' </summary>
    ''' <param name="Emp_code"></param>
    ''' <param name="recepient"></param>
    ''' <param name="subject"></param>
    ''' <param name="body"></param>
    ''' <param name="cc"></param>
    ''' <param name="attachement"></param>
    ''' <param name="string1"></param>
    ''' <param name="string2"></param>
    ''' <param name="password"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function SendEmail(Emp_code As String, recepient As String, subject As String, body As String, cc As String, attachement As String, string1 As String, _
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

        smtpClient.Credentials = New NetworkCredential(from, AppSetting(password))

        '     smtpClient.UseDefaultCredentials = False
        smtpClient.Port = 25
        Try
            smtpClient.Send(mail)
            flag = True
        Catch exception As Exception
            Console.Out.WriteLine(exception.Message)
            flag = False
            Enter_EmailLog(Emp_code, Session("WebTableID"), 0, "DKMWebApplication", subject & "Mail ID Not Found", "To : " & recepient, body, exception.Message, Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type)
        End Try
        mail.Attachments.Clear()
        Return flag
    End Function

    Public Function AppSetting(key As String) As String
        Return ConfigurationManager.AppSettings(key)
    End Function

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