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

Public Class ReimbursementOthers
    Inherits System.Web.UI.Page

    Dim IsEntrySaved As Boolean = False
    Dim constantValuesnew As New ConstantValues
    Dim type As String
    Public Property showPersonalDetails As Boolean
    Public Property IsPersonalDetails As Boolean
    Public Property IsMultipleDetails As Boolean
    Public ReimbursementID As Integer
    Dim ID As Integer
    Public ReimbursementType_Code As Integer
    Public ReimbursementTypeID As Integer
    Dim dtValidDates As DataTable
    Dim dtUpload As DataTable
    Dim Code As String = ""
    Dim commendText As String
    Dim dtOption As New DataTable
    Dim dtValidTele As DataTable
    Dim ValidTele As Boolean = False
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

            radGrid1.MasterTableView.GetColumn("MultipleClaimDetailsID").Display = False

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
                    radGrid1.MasterTableView.GetColumn("Edit").Display = False
                    radGrid1.MasterTableView.GetColumn("Delete").Display = False
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
                            radGrid1.MasterTableView.GetColumn("Edit").Display = False
                            radGrid1.MasterTableView.GetColumn("Delete").Display = False
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

            dtValidTele = DatabaseFacade.Instance.GetGivenTable("select Name from ValidTele where Name='" & type & "'")

            If (dtValidTele.Rows.Count > 0) Then
                ValidTele = True
            End If

            If Not (IsPostBack) Then
                IsEntrySaved = False
                If (Session("WebTableID").ToString = "805" Or Session("WebTableID").ToString = "11097" Or Session("WebTableID").ToString = "497") And (ReimbursementType_Code = 2 Or ReimbursementTypeID = 2) Then
                    lblExtraField4.Style.Add("width", "400px")
                    lblExtraField6.Style.Add("width", "400px")
                    trupload2.Visible = True
                    truploadbtn2.Visible = True
                    Label4.Text = "Upload mobile Bill and other documents as per the policy (jpg/pdf File)"
                    Label1.Text = "Upload broadband Bill and other documents as per the policy (jpg/pdf File)"
                    ExtraField11.AutoPostBack = True
                    Session("UploadedFile") = ""
                    Session("UploadedFile2") = ""
                    lit1.Text = ""
                Else
                    trupload2.Visible = False
                    truploadbtn2.Visible = False
                    ExtraField11.AutoPostBack = False
                    Session("UploadedFile") = ""
                    Session("UploadedFile2") = ""
                End If

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

            If Not (radGrid1.Visible) Then
                panelCSVUpload.Visible = False
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
                    radGrid1.MasterTableView.GetColumn("MultipleClaimDetailsID").Display = True
                End If
            End If

            Dim dtCSVwithOutSubClaimID As DataTable = OldNewConn.GetDataTable2("Select name, print_name from payslipsetup" & Session("WebTableID") & " where name in ('CSVwithSubClaimID')")

            If (dtCSVwithOutSubClaimID.Rows.Count > 0) Then
                If (dtCSVwithOutSubClaimID.Rows(0)("print_name").ToString.ToLower = "n") Then
                    ViewState("WithOutSubClaimID") = "CSVwithOutSubClaimID"
                End If
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
            Dim dtReimburseAutodetails As DataTable

            dtReimburseFullDetail = OldNewConn.GetDataTable2("select * from reimbursementdetails" & Session("WebTableID") & " where ReimburseDetailsID=" & ReimbursementID & "")
            dtReimburseAutodetails = OldNewConn.GetDataTable2("select * from reimbursementdetails" & Session("WebTableID") & " where ReimburseType_Code=" & dtReimburse.Rows(0)("ReimburseType_Code") & " and Emp_code='" & Session("Emp_code").ToString & "'  order by ReimburseDetailsID desc")

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

            If Not IsNothing(Request.QueryString("Type")) Or Not IsNothing(Request.QueryString("ID")) Then

                If (type.Contains("VEHICLE")) Then
                    hidmaxlen.Value = type
                    hidwebtableid.Value = Session("WebTableID")

                    If dtReimburseFullDetail.Rows.Count > 0 Then
                        Session("ReimburseDetailsID") = dtReimburseFullDetail.Rows(0)("ReimburseDetailsID")
                    Else
                        Session("ReimburseDetailsID") = "Not Applicable"
                    End If
                End If

                If (type.Contains("DRIVER")) Then
                    If dtReimburseFullDetail.Rows.Count > 0 Then
                        Session("ReimburseDetailsID") = dtReimburseFullDetail.Rows(0)("ReimburseDetailsID")
                    Else
                        Session("ReimburseDetailsID") = "Not Applicable"
                    End If
                End If
            Else

            End If

            dict = DirectCast(Session("TempDict"), Dictionary(Of String, String))

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


                    If (dtReimburseAutodetails.Rows.Count > 0) Then
                        If Date.TryParseExact(CDate(dtReimburseAutodetails.Rows(0)("TransactionDate")).ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                            TransactionDate = d
                        Else
                            Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                            Exit Sub
                        End If
                        radDatePickerTransactionDate.SelectedDate = TransactionDate.Date
                    End If


                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then
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

                    If (dtReimburseAutodetails.Rows.Count > 0) Then

                        If Date.TryParseExact(CDate(dtReimburseAutodetails.Rows(0)("ExtraDateField")).ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                            TransactionDate = d
                        Else
                            Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                            Exit Sub
                        End If
                        radDatepickerExtraDateField.SelectedDate = TransactionDate.Date

                    End If

                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then
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

                    If (dtReimburseAutodetails.Rows.Count > 0) Then
                        TransactionDetail.Text = dtReimburseAutodetails.Rows(0)("TransactionDetail")
                    End If
                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then
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

                    If (dr("ExtraField1").ToString.Contains("*")) Then
                        lblReq1.Visible = False
                        validEF1.Enabled = False
                        lblExtraField1.Text = dr("ExtraField1").ToString.Replace("*", "")
                    Else
                        lblReq1.Visible = True
                        validEF1.Enabled = True
                        lblExtraField1.Text = dr("ExtraField1")
                    End If

                    panelDetail.Visible = True
                    lblExtraField1.Visible = True
                    ExtraField1.Visible = True

                    If (dr("ExtraField1").ToString.ToLower.Contains("number")) Then
                        NumbervalidEF1.Enabled = True
                    ElseIf (dr("ExtraField1").ToString.ToLower.Contains("registration") Or dr("ExtraField1").ToString.ToLower.Contains("license no")) Then
                        AllEF1.Enabled = True
                    Else
                        TextvalidEF1.Enabled = True
                    End If
                    If (dtReimburseAutodetails.Rows.Count > 0) Then
                        ExtraField1.Text = dtReimburseAutodetails.Rows(0)("ExtraField1")
                    End If

                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then
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
                    If (dr("ExtraField2").ToString.Contains("*")) Then
                        lblReq2.Visible = False
                        validEF2.Enabled = False
                        lblExtraField2.Text = dr("ExtraField2").ToString.Replace("*", "")
                    Else
                        lblReq2.Visible = True
                        validEF2.Enabled = True
                        lblExtraField2.Text = dr("ExtraField2")
                    End If

                    panelDetail.Visible = True
                    lblExtraField2.Visible = True
                    ExtraField2.Visible = True

                    If (dr("ExtraField2").ToString.ToLower.Contains("number")) Then
                        NumbervalidEF2.Enabled = True
                    ElseIf (dr("ExtraField2").ToString.ToLower.Contains("registration") Or dr("ExtraField2").ToString.ToLower.Contains("license no")) Then
                        AllEF2.Enabled = True
                    Else
                        TextvalidEF2.Enabled = True
                    End If
                    If (dtReimburseAutodetails.Rows.Count > 0) Then

                        ExtraField2.Text = dtReimburseAutodetails.Rows(0)("ExtraField2")

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

                    If (dr("ExtraField3").ToString.Contains("*")) Then
                        lblReq3.Visible = False
                        validEF3.Enabled = False
                        lblExtraField3.Text = dr("ExtraField3").ToString.Replace("*", "")
                    Else
                        lblReq3.Visible = True
                        validEF3.Enabled = True
                        lblExtraField3.Text = dr("ExtraField3")
                    End If

                    panelDetail.Visible = True
                    lblExtraField3.Visible = True
                    ExtraField3.Visible = True

                    If (dr("ExtraField3").ToString.ToLower.Contains("number")) Then
                        NumbervalidEF3.Enabled = True
                    ElseIf (dr("ExtraField3").ToString.ToLower.Contains("registration") Or dr("ExtraField3").ToString.ToLower.Contains("license no")) Then
                        AllEF3.Enabled = True
                    Else
                        TextvalidEF3.Enabled = True
                    End If

                    If (dtReimburseAutodetails.Rows.Count > 0) Then
                        ExtraField3.Text = dtReimburseAutodetails.Rows(0)("ExtraField3")
                    End If

                    If (dtReimburseAutodetails.Rows.Count > 0) Then
                        ExtraField3.Enabled = True
                        ExtraField3.BackColor = Drawing.Color.White
                        ExtraField3.Text = dtReimburseAutodetails.Rows(0)("ExtraField3")
                    End If

                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then
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

                    If (dr("ExtraField5").ToString.Contains("*")) Then
                        lblReq5.Visible = False
                        validEF5.Enabled = False
                        lblExtraField5.Text = dr("ExtraField5").ToString.Replace("*", "")
                    Else
                        lblReq5.Visible = True
                        validEF5.Enabled = True
                        lblExtraField5.Text = dr("ExtraField5")
                    End If

                    panelDetail.Visible = True
                    lblExtraField5.Visible = True
                    ExtraField5.Visible = True


                    If (dr("ExtraField5").ToString.ToLower.Contains("number")) Then
                        NumbervalidEF5.Enabled = True
                    ElseIf (dr("ExtraField5").ToString.ToLower.Contains("registration") Or dr("ExtraField5").ToString.ToLower.Contains("license no")) Then
                        AllEF5.Enabled = True
                    Else
                        TextvalidEF5.Enabled = True
                    End If
                    If (dtReimburseAutodetails.Rows.Count > 0) Then
                        ExtraField5.Text = dtReimburseAutodetails.Rows(0)("ExtraField5")
                    End If
                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then
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

                    If (dtReimburseAutodetails.Rows.Count > 0) Then
                        Try
                            ExtraField4.ClearSelection()
                            ExtraField4.SelectedValue = ExtraField4.Items.FindByText(dtReimburseAutodetails.Rows(0)("ExtraField4")).Value
                        Catch ex As Exception

                        End Try


                    End If
                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then
                        Else
                            ExtraField4.ClearSelection()
                            ExtraField4.SelectedValue = ExtraField4.Items.FindByText(dtReimburseFullDetail.Rows(0)("ExtraField4")).Value
                        End If
                    End If

                    If Not IsNothing(dict) Then
                        If (dict.Count > 0) Then
                            If (dict.ContainsKey("ExtraField4")) Then
                                ExtraField4.ClearSelection()
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

                    If (dtReimburseAutodetails.Rows.Count > 0) Then
                        Try
                            ExtraField6.ClearSelection()
                            ExtraField6.SelectedValue = ExtraField6.Items.FindByText(dtReimburseAutodetails.Rows(0)("ExtraField6")).Value
                        Catch ex As Exception

                        End Try


                    End If
                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then
                        Else
                            ExtraField6.ClearSelection()
                            ExtraField6.SelectedValue = ExtraField6.Items.FindByText(dtReimburseFullDetail.Rows(0)("ExtraField6")).Value
                        End If
                    End If

                    If Not IsNothing(dict) Then
                        If (dict.Count > 0) Then
                            If (dict.ContainsKey("ExtraField6")) Then
                                ExtraField6.ClearSelection()
                                ExtraField6.SelectedValue = ExtraField6.Items.FindByText(dict("ExtraField6")).Value
                            End If
                        End If
                    End If

                End If

                If (Session("WebTableID").ToString = "805" Or Session("WebTableID").ToString = "11097" Or Session("WebTableID").ToString = "497") And (ReimbursementType_Code = 2 Or ReimbursementTypeID = 2) Then
                    If Not (dr("ExtraField9") = "") Then

                        panelDetail.Visible = True
                        lblReq9.Visible = True
                        lblExtraField9.Visible = True
                        ExtraField9.Visible = True
                        validEF9.Enabled = True

                        lblExtraField9.Text = dr("ExtraField9Description").ToString()

                        Dim strstrold As String() = dr("ExtraField9").ToString.Split(",")
                        ExtraField9.DataSource = strstrold
                        ExtraField9.DataBind()

                        If (dtReimburseAutodetails.Rows.Count > 0) Then
                            Try
                                ExtraField9.ClearSelection()
                                ExtraField9.SelectedValue = ExtraField9.Items.FindByText(dtReimburseAutodetails.Rows(0)("ExtraField9")).Value
                            Catch ex As Exception

                            End Try


                        End If
                        If (dtReimburseFullDetail.Rows.Count > 0) Then
                            If IsNothing(Request.QueryString("ID")) Then
                            Else
                                ExtraField9.ClearSelection()
                                ExtraField9.SelectedValue = ExtraField9.Items.FindByText(dtReimburseFullDetail.Rows(0)("ExtraField9")).Value
                            End If
                        End If

                        If Not IsNothing(dict) Then
                            If (dict.Count > 0) Then
                                If (dict.ContainsKey("ExtraField9")) Then
                                    ExtraField9.ClearSelection()
                                    ExtraField9.SelectedValue = ExtraField9.Items.FindByText(dict("ExtraField9")).Value
                                End If
                            End If
                        End If

                    End If
                    If Not (dr("ExtraField10") = "") Then

                        panelDetail.Visible = True
                        lblReq10.Visible = True
                        lblExtraField10.Visible = True
                        ExtraField10.Visible = True
                        validEF10.Enabled = True

                        lblExtraField10.Text = dr("ExtraField10Description").ToString()

                        Dim strstrold As String() = dr("ExtraField10").ToString.Split(",")
                        ExtraField10.DataSource = strstrold
                        ExtraField10.DataBind()

                        If (dtReimburseAutodetails.Rows.Count > 0) Then
                            Try
                                ExtraField10.ClearSelection()
                                ExtraField10.SelectedValue = ExtraField10.Items.FindByText(dtReimburseAutodetails.Rows(0)("ExtraField10")).Value
                            Catch ex As Exception

                            End Try


                        End If
                        If (dtReimburseFullDetail.Rows.Count > 0) Then
                            If IsNothing(Request.QueryString("ID")) Then
                            Else
                                ExtraField10.ClearSelection()
                                ExtraField10.SelectedValue = ExtraField10.Items.FindByText(dtReimburseFullDetail.Rows(0)("ExtraField10")).Value
                            End If
                        End If

                        If Not IsNothing(dict) Then
                            If (dict.Count > 0) Then
                                If (dict.ContainsKey("ExtraField10")) Then
                                    ExtraField10.ClearSelection()
                                    ExtraField10.SelectedValue = ExtraField10.Items.FindByText(dict("ExtraField10")).Value
                                End If
                            End If
                        End If

                    End If

                    If Not (dr("ExtraField11") = "") Then

                        panelDetail.Visible = True
                        lblReq11.Visible = True
                        lblExtraField11.Visible = True
                        ExtraField11.Visible = True
                        validEF11.Enabled = True

                        lblExtraField11.Text = dr("ExtraField11Description").ToString()

                        Dim strstrold As String() = dr("ExtraField11").ToString.Split(",")
                        ExtraField11.DataSource = strstrold
                        ExtraField11.DataBind()

                        If (dtReimburseAutodetails.Rows.Count > 0) Then
                            Try
                                ExtraField11.ClearSelection()
                                ExtraField11.SelectedValue = ExtraField11.Items.FindByText(dtReimburseAutodetails.Rows(0)("ExtraField11")).Value
                            Catch ex As Exception

                            End Try


                        End If
                        If (dtReimburseFullDetail.Rows.Count > 0) Then
                            If IsNothing(Request.QueryString("ID")) Then
                            Else
                                ExtraField11.ClearSelection()
                                ExtraField11.SelectedValue = ExtraField11.Items.FindByText(dtReimburseFullDetail.Rows(0)("ExtraField11")).Value
                            End If
                        End If

                        If Not IsNothing(dict) Then
                            If (dict.Count > 0) Then
                                If (dict.ContainsKey("ExtraField11")) Then
                                    ExtraField11.ClearSelection()
                                    ExtraField11.SelectedValue = ExtraField11.Items.FindByText(dict("ExtraField11")).Value
                                End If
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

                    lblExtraField7.Text = dr("ExtraField7").ToString()

                    Dim ExtraField7Date As Date
                    Dim d As Date
                    If (dtReimburseAutodetails.Rows.Count > 0) Then

                        If Date.TryParseExact(CDate(dtReimburseAutodetails.Rows(0)("ExtraField7")).ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                            ExtraField7Date = d
                        Else
                            Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                            Exit Sub
                        End If
                        radExtraField7.SelectedDate = ExtraField7Date.Date

                    End If

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

                    lblExtraField8.Text = dr("ExtraField8").ToString()

                    Dim ExtraField8Date As Date
                    Dim d As Date
                    If (dtReimburseAutodetails.Rows.Count > 0) Then
                        If Date.TryParseExact(CDate(dtReimburseAutodetails.Rows(0)("ExtraField8")).ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                            ExtraField8Date = d
                        Else
                            Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                            Exit Sub
                        End If
                        radExtraField8.SelectedDate = ExtraField8Date.Date
                    End If

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
                    txtTotalBillField.Visible = True
                    lblTotalBillField.Visible = True

                    lblTotalBillField.Text = dr("TotalBillField")

                    If (dtReimburseAutodetails.Rows.Count > 0) Then

                        txtTotalBillField.Text = CInt(dtReimburseAutodetails.Rows(0)("TotalBillField"))

                    End If
                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then
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
                    Amount.Visible = True
                    lblAmount.Text = dr("ClaimAmountField")

                    If (dtReimburseAutodetails.Rows.Count > 0) Then
                        Amount.Text = CInt(dtReimburseAutodetails.Rows(0)("ClaimAmount"))
                    End If

                    If (dtReimburseFullDetail.Rows.Count > 0) Then
                        If IsNothing(Request.QueryString("ID")) Then
                        Else
                            Amount.Text = CInt(dtReimburseFullDetail.Rows(0)("ClaimAmount"))
                        End If
                    ElseIf (type.ToString().ToUpper() = "TAXABLE LTA") Then
                        Dim dt1 As DataTable = OldNewConn.GetDataTable2("Select Top 1 Prorata from ReimburseMaster" & Session("WebTableID") & " where Budget_Wef >= '04/01/" & Year & "' and Budget_Wet<='04/01/" & Year + 1 & "' and Emp_Code='" & Session("Emp_code") & "'")

                        If (dt1.Rows.Count > 0) Then
                            Amount.Text = CInt(dt1.Rows(0)("Prorata"))
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

                Session("IsMultipleDetails") = dr("isMultipleClaimDetails")

                If (dr("isMultipleClaimDetails")) Then
                    radGrid1.Visible = True
                    lnkMultiple.Visible = True
                Else
                    radGrid1.Visible = False
                    lnkMultiple.Visible = False
                End If

                Dim dicUploadFile As New Dictionary(Of Integer, UploadedFile)

                ' If Not IsNothing(Session("UploadedFile")) Then
                If Not (Session("UploadedFile")).ToString = "" Then
                    dicUploadFile = Session("UploadedFile")
                    Dim sb As StringBuilder = New StringBuilder()
                    Dim i As Int32 = 0
                    For Each rad As UploadedFile In dicUploadFile.Values
                        i += 1
                        sb.Append("(" & i & ")  ")
                        sb.Append(rad.GetName)
                        sb.Append("   ")
                    Next

                    If String.IsNullOrEmpty(lit1.Text) Then
                        lit1.Text = sb.ToString()
                    Else
                        lit1.Text = lit1.Text & "<br/>" & sb.ToString()
                    End If


                End If

                If (Session("WebTableID").ToString = "805" Or Session("WebTableID").ToString = "11097" Or Session("WebTableID").ToString = "497") And (ReimbursementType_Code = 2 Or ReimbursementTypeID = 2) Then
                    Dim dicUploadFile2 As New Dictionary(Of Integer, UploadedFile)

                    If Not IsNothing(Session("UploadedFile2")) Then
                        dicUploadFile2 = Session("UploadedFile2")
                        Dim sb As StringBuilder = New StringBuilder()
                        Dim i As Int32 = 0
                        For Each rad As UploadedFile In dicUploadFile2.Values
                            i += 1
                            sb.Append("(" & i & ")  ")
                            sb.Append(rad.GetName)
                            sb.Append("   ")
                        Next

                        If String.IsNullOrEmpty(lit1.Text) Then
                            lit1.Text = sb.ToString()
                        Else
                            lit1.Text = lit1.Text & "<br/>" & sb.ToString()
                        End If
                    End If
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
                Dim dtExtraDateField As Date
                Try
                    If Date.TryParseExact(ExtraDateField.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                        dtExtraDateField = d
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

            If (Session("WebTableID").ToString = "805" Or Session("WebTableID").ToString = "11097" Or Session("WebTableID").ToString = "497") And (ReimbursementType_Code = 2 Or ReimbursementTypeID = 2) Then
                If Not (dr("ExtraField9") = "") Then
                    dict.Add("ExtraField9", ExtraField9.SelectedItem.Text)
                Else
                    dict.Add("ExtraField9", String.Empty)
                End If
                If Not (dr("ExtraField10") = "") Then
                    dict.Add("ExtraField10", ExtraField10.SelectedItem.Text)
                Else
                    dict.Add("ExtraField10", String.Empty)
                End If
                If Not (dr("ExtraField11") = "") Then
                    dict.Add("ExtraField11", ExtraField11.SelectedItem.Text)
                Else
                    dict.Add("ExtraField11", String.Empty)
                End If
            End If

            If Not (dr("ExtraField7") = "") Then
                Dim ExtraDateField As Date = radExtraField7.SelectedDate.Value.Date
                Dim dtExtraDateField As Date
                Try

                    If Date.TryParseExact(ExtraDateField.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                        dtExtraDateField = d
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
                Dim dtExtraDateField As Date
                Try

                    If Date.TryParseExact(ExtraDateField.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                        dtExtraDateField = d
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

            For Each r As GridDataItem In radGrid1.MasterTableView.Items
                totalAmt1 = totalAmt1 + r("BillAmount").Text
            Next


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
        Dim dicUploadFile2 As New Dictionary(Of Integer, UploadedFile)
        If (Session("WebTableID").ToString = "805" Or Session("WebTableID").ToString = "11097" Or Session("WebTableID").ToString = "497") And (ReimbursementType_Code = 2 Or ReimbursementTypeID = 2) Then
            If (RadAsyncUpload2.UploadedFiles.Count > 0) Then
                Dim i As Int32 = 0
                For Each rad As UploadedFile In RadAsyncUpload2.UploadedFiles
                    i += 1
                    dicUploadFile2.Add(i, rad)
                Next
                Session("UploadedFile2") = dicUploadFile2
            End If
        End If


        Session("TempDict") = dict
    End Sub

    ''' <summary>
    ''' RadGrid1 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    ''' 

    Protected Sub radGrid1_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs) Handles radGrid1.NeedDataSource
        Try
            Dim dtTable As New DataTable

            If Not IsNothing(Session("ReimbursetableMultiple")) Then
                radGrid1.DataSource = Session("ReimbursetableMultiple")
            Else
                Dim extraSql As String = ""
                Dim extraSql2 As String = ""
                dtTable.Columns.Add("Sl.No")
                dtTable.Columns.Add("BillNumber")
                dtTable.Columns.Add("BillDated")
                dtTable.Columns.Add("BillAmount")
                dtTable.Columns.Add("BillDetail")
                dtTable.Columns.Add("MultipleClaimDetailsID")

                Dim dicUploadFile As New Dictionary(Of Integer, Integer)

                Dim dt As DataTable = GetPersonalDetailsofReimbursementMultiple(ReimbursementID)

                Dim dict12 As New Dictionary(Of Integer, Byte())


                For Each dr As DataRow In dt.Rows
                    dtTable.Rows.Add(dr("Sl.No"), dr("BillNumber"), dr("BillDated"), Format(dr("BillAmount"), "###0"), dr("BillDetail"), dr("MultipleClaimDetailsID"))
                Next
                radGrid1.DataSource = dtTable
                dtTable = dtTable.DefaultView.ToTable()
                Session("ReimbursetableMultiple") = dtTable
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub radGrid1_DeleteCommand(sender As Object, e As GridCommandEventArgs) Handles radGrid1.DeleteCommand
        Dim griditem As GridDataItem = TryCast(e.Item, GridDataItem)
        Dim dicUploadFile As Dictionary(Of Integer, Byte())
        If Not IsNothing(griditem) Then
            If Not IsNothing(Session("ReimbursetableMultiple")) Then
                Dim dt As DataTable = DirectCast(Session("ReimbursetableMultiple"), DataTable)
                Dim dr As DataRow = dt.Select("Sl.No = '" & griditem.GetDataKeyValue("Sl.No") & "'")(0)
                If Not IsNothing(dr) Then
                    dt.Rows.Remove(dr)
                End If
            End If
        End If
    End Sub

    Protected Sub radGrid1_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles radGrid1.ItemDataBound
        Dim griditem As GridDataItem = TryCast(e.Item, GridDataItem)

        If Not IsNothing(griditem) Then
            griditem("BillDated").Text = Convert.ToDateTime(griditem("BillDated").Text).ToString("dd/MM/yyyy")
        End If
    End Sub

    Protected Sub btnEditOther_Click(sender As Object, e As EventArgs)
        SaveInSession()
        Dim btnEditOther As LinkButton = TryCast(sender, LinkButton)
        Dim item As GridDataItem = TryCast(btnEditOther.NamingContainer, GridDataItem)
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", String.Format("openRadWindow('{0}');", "OtherReimbursementMultipleDetail.aspx?SNo=" & item.GetDataKeyValue("Sl.No") & "&Type_Code=" & type & ""), True)
    End Sub

    ''' <summary>
    ''' CSV Upload
    ''' </summary>
    ''' <remarks></remarks>

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
            If (dtReimburse.Rows(0)("isMultipleClaimDetails")) Then
                list.Add("BillNumber")
                list.Add("BillDated")
                list.Add("BillAmount")
                list.Add("Password/ Any other info")
            End If
        End If
        Return list
    End Function

    Protected Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnUpload.Click

        If (radAsynUpload.UploadedFiles.Count = 0) Then
            Response.Write("<script>alert('Please Select CSV File.)');</script>")
            Exit Sub
        End If

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

            If Not IsNothing(Session("ReimbursetableMultiple")) Then
                If (Session("IsMultipleDetails")) Then
                    dt = DirectCast(Session("ReimbursetableMultiple"), DataTable)
                End If
            Else
                If (Session("IsMultipleDetails")) Then
                    dt.Columns.Add("Sl.No")
                    dt.Columns.Add("BillNumber")
                    dt.Columns.Add("BillDated", GetType(System.String))
                    dt.Columns.Add("BillAmount")
                    dt.Columns.Add("BillDetail")
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
                    If (Session("IsMultipleDetails")) Then

                        If (dt.Rows.Count = 0) Then
                            dr("Sl.No") = 1
                        Else
                            dr("Sl.No") = dt.Rows(dt.Rows.Count - 1)("Sl.No") + 1
                        End If

                        'If Not IsNumeric(value(0)) Then
                        '    Response.Write("<script>alert('Bill number can be in numeric value only');</script>")
                        '    Exit Sub
                        'Else
                        dr("BillNumber") = value(0)
                        'End If

                        Dim d As Date
                        Dim BillDate As Date
                        Try
                            If Date.TryParseExact(Trim(value(1)), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                                BillDate = d
                            Else
                                Me.lblError.Text = "Bill Date is not valid for employee : " & Session("Emp_Code") & " Date : " & value(1) & ", please enter the date either in DD/MM/YYYY format!"
                                Exit Sub
                            End If
                        Catch ex As Threading.ThreadAbortException
                        Catch ex As Exception
                            Me.lblError.Text = "Bill Date is not valid for employee : " & Session("Emp_Code") & " Date : " & value(1) & ", please enter the date either in DD/MM/YYYY format!"
                            Exit Sub
                        End Try

                        dr("BillDated") = BillDate.Date

                        If Not IsNumeric(value(2)) Then
                            Response.Write("<script>alert('Amount can be in numeric value only');</script>")
                            Exit Sub
                        Else
                            dr("BillAmount") = Math.Round(CDec(value(2)))
                        End If

                        dr("BillDetail") = Replace(value(3), "'", "")

                        dt.Rows.Add(dr)
                    End If
                End While

            Next

            If (Session("IsMultipleDetails")) Then
                Session("ReimbursetableMultiple") = dt
                radGrid1.Rebind()
            End If

            SaveInSession()
            Response.Write("<script>alert('Bill detail saved, please upload bills and submit claim.');</script>")

        Catch ex As Exception
            lblError.Text = "Please check the CSV properly."
        End Try


    End Sub

    Protected Sub lnkMultiple_Click(sender As Object, e As EventArgs) Handles lnkMultiple.Click

        Dim dtReimburse As DataTable

        dtReimburse = OldNewConn.GetDataTable2("select * from ReimburseMentTypeMaster" & Session("WebTableID") & " where Name='" & type & "'")

        For Each dr As DataRow In dtReimburse.Rows
            If (ValidTele) Then

                validEF1.Enabled = False
                validEF3.Enabled = False
                If Not (dr("ExtraField1") = "") And Not (dr("ExtraField3") = "") Then
                    If (ExtraField1.Text = "" And ExtraField3.Text = "") Then
                        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('It is mandatory to fill " & dr("ExtraField1").ToString.Replace("*", "") & " Or " & dr("ExtraField3").ToString.Replace("*", "") & "');", True)
                        Exit Sub
                    End If
                ElseIf Not (dr("ExtraField1") = "") Then
                    If (ExtraField1.Text = "") Then
                        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('It is mandatory to fill " & dr("ExtraField1").ToString.Replace("*", "") & "');", True)
                        Exit Sub
                    End If
                ElseIf Not (dr("ExtraField3") = "") Then
                    If (ExtraField3.Text = "") Then
                        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('It is mandatory to fill " & dr("ExtraField3").ToString.Replace("*", "") & "');", True)
                        Exit Sub
                    End If
                End If

                If Not (dr("ExtraField1") = "") Then
                    If (Session("WebTableID").ToString = "805" Or Session("WebTableID").ToString = "11097" Or Session("WebTableID").ToString = "497") And (ReimbursementType_Code = 2 Or ReimbursementTypeID = 2) Then


                        If (ExtraField1.Text <> "") Then
                            If (ExtraField1.Text.Length > 10 Or ExtraField1.Text.Length < 10) Then
                                lblsave.Visible = False
                                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('" & dr("ExtraField1").ToString.Replace("*", "") & " should be of 10 digits.');", True)
                                Exit Sub
                            End If
                        End If
                    Else
                        If (ExtraField1.Text <> "" And ExtraField2.Text = "") Then
                            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('It is mandatory to fill " & dr("ExtraField2").ToString.Replace("*", "") & "');", True)
                            Exit Sub
                        ElseIf (ExtraField1.Text = "" And ExtraField2.Text <> "") Then
                            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('It is mandatory to fill " & dr("ExtraField1").ToString.Replace("*", "") & "');", True)
                            Exit Sub
                        End If

                        If (ExtraField1.Text <> "") Then
                            If (ExtraField1.Text.Length > 10 Or ExtraField1.Text.Length < 10) Then
                                lblsave.Visible = False
                                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('" & dr("ExtraField1").ToString.Replace("*", "") & " should be of 10 digits.');", True)
                                Exit Sub
                            End If
                        End If
                    End If

                End If

                If Not (dr("ExtraField3") = "") Then
                    If (ExtraField3.Text <> "" And ExtraField5.Text = "") Then
                        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('It is mandatory to fill " & dr("ExtraField5").ToString.Replace("*", "") & "');", True)
                        Exit Sub
                    ElseIf (ExtraField3.Text = "" And ExtraField5.Text <> "") Then
                        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('It is mandatory to fill " & dr("ExtraField3").ToString.Replace("*", "") & "');", True)
                        Exit Sub
                    End If

                    If (ExtraField3.Text <> "") Then
                        If (ExtraField3.Text.Length > 30 Or ExtraField3.Text.Length < 1) Then
                            lblsave.Visible = False
                            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('" & dr("ExtraField3").ToString.Replace("*", "") & " should be in between 1 to 30 digits.');", True)
                            Exit Sub
                        End If
                    End If
                End If
            End If
        Next

        If IsNothing(Request.QueryString("ID")) Then
            If IsEntrySaved Then
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", String.Format("openRadWindow('{0}');", "OtherReimbursementMultipleDetail.aspx?Type_Code=" & type & "&whetherIsSaveEntry =" & "Y" & ""), True)
                'image.Attributes.Add("onclick", String.Format("return openRadWindow('{0}');", "OtherReimbursementMultipleDetail.aspx?SNo=" & griditem.GetDataKeyValue("Sl.No") & "&Type_Code=" & type & ""))
            Else
                Session("TempDict") = Nothing
                SaveInSession()
                bindTable()
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", String.Format("openRadWindow('{0}');", "OtherReimbursementMultipleDetail.aspx?Type_Code=" & type & "&whetherIsSaveEntry =" & "N" & ""), True)
            End If
        Else
            Session("TempDict") = Nothing
            SaveInSession()
            bindTable()
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", String.Format("openRadWindow('{0}');", "OtherReimbursementMultipleDetail.aspx?Type_Code=" & type & "&whetherIsSaveEntry =" & "N" & ""), True)
        End If
    End Sub

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
        Session("UploadedFile") = ""
        Session("file1name") = ""
        lit1.Text = ""
        Dim dicUploadFile As New Dictionary(Of Integer, UploadedFile)
        Dim sb As StringBuilder = New StringBuilder()

        If (RadAsyncUpload1.UploadedFiles.Count = 0) Then
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please Select jpeg/jpg/png/pdf File.');", True)
            Exit Sub
        End If
        If (Session("WebTableID").ToString = "805" Or Session("WebTableID").ToString = "11097" Or Session("WebTableID").ToString = "497") And (ReimbursementType_Code = 2 Or ReimbursementTypeID = 2) Then

            If ExtraField4.SelectedItem.Text = "Yes" And ExtraField6.SelectedItem.Text = "No" And ExtraField9.SelectedItem.Text = "No" Then
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You can not upload personal mobile phone bill.You can upload only personal broadband bill as company has already provided you benefit.');", True)
                Exit Sub
            ElseIf ExtraField4.SelectedItem.Text = "No" And ExtraField6.SelectedItem.Text = "Yes" And ExtraField9.SelectedItem.Text = "No" Then
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You can not upload personal mobile phone bill.You can upload only personal broadband bill as company has already provided you benefit.');", True)
                Exit Sub
            ElseIf ExtraField4.SelectedItem.Text = "Yes" And ExtraField6.SelectedItem.Text = "Yes" And ExtraField9.SelectedItem.Text = "Yes" Then
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You can not upload personal mobile phone bill and personal broadband bill as company has already provided you benefit.');", True)
                Exit Sub

            ElseIf ExtraField4.SelectedItem.Text = "Yes" And ExtraField6.SelectedItem.Text = "No" And ExtraField9.SelectedItem.Text = "Yes" Then
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You can not upload personal broadband bill and personal mobile phone bill as company has already provided you benefit.');", True)
                Exit Sub
            ElseIf ExtraField4.SelectedItem.Text = "No" And ExtraField6.SelectedItem.Text = "Yes" And ExtraField9.SelectedItem.Text = "Yes" Then
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You can not upload personal broadband bill and personal mobile phone bill as company has already provided you benefit.');", True)
                Exit Sub
            End If

        End If

        If (RadAsyncUpload1.UploadedFiles.Count > 0) Then
            Dim i As Int32 = 0
            For Each rad As UploadedFile In RadAsyncUpload1.UploadedFiles
                i += 1
                dicUploadFile.Add(i, rad)
                sb.Append("(" & i & ")  ")
                sb.Append(rad.GetName)
                sb.Append("   ")
                Session("file1name") = rad.GetName
            Next

            If Not IsNothing(Session("file1name")) Then
                If Not String.IsNullOrEmpty(Session("file1name").ToString) Then
                    If String.IsNullOrEmpty(lit1.Text) Then
                        lit1.Text = Session("file1name").ToString
                    Else
                        lit1.Text = lit1.Text & "<br/>" & Session("file1name").ToString
                    End If
                End If

            End If
            If Not IsNothing(Session("file1name2")) Then
                If Not String.IsNullOrEmpty(Session("file1name2").ToString) Then
                    If String.IsNullOrEmpty(lit1.Text) Then
                        lit1.Text = Session("file1name2").ToString
                    Else
                        lit1.Text = lit1.Text & "<br/>" & Session("file1name2").ToString
                    End If
                End If

            End If



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

            For Each dr As DataRow In dtReimburse.Rows
                If (ValidTele) Then

                    validEF1.Enabled = False
                    validEF3.Enabled = False
                    If Not (dr("ExtraField1") = "") And Not (dr("ExtraField3") = "") Then
                        If (ExtraField1.Text = "" And ExtraField3.Text = "") Then
                            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('It is mandatory to fill " & dr("ExtraField1").ToString.Replace("*", "") & " Or " & dr("ExtraField3").ToString.Replace("*", "") & "');", True)
                            Exit Sub
                        End If
                    ElseIf Not (dr("ExtraField1") = "") Then
                        If (ExtraField1.Text = "") Then
                            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('It is mandatory to fill " & dr("ExtraField1").ToString.Replace("*", "") & "');", True)
                            Exit Sub
                        End If
                    ElseIf Not (dr("ExtraField3") = "") Then
                        If (ExtraField3.Text = "") Then
                            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('It is mandatory to fill " & dr("ExtraField3").ToString.Replace("*", "") & "');", True)
                            Exit Sub
                        End If
                    End If

                    If Not (dr("ExtraField1") = "") Then
                        If (Session("WebTableID").ToString = "805" Or Session("WebTableID").ToString = "11097" Or Session("WebTableID").ToString = "497") And (ReimbursementType_Code = 2 Or ReimbursementTypeID = 2) Then


                            If (ExtraField1.Text <> "") Then
                                If (ExtraField1.Text.Length > 10 Or ExtraField1.Text.Length < 10) Then
                                    lblsave.Visible = False
                                    ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('" & dr("ExtraField1").ToString.Replace("*", "") & " should be of 10 digits.');", True)
                                    Exit Sub
                                End If
                            End If
                        Else
                            If (ExtraField1.Text <> "" And ExtraField2.Text = "") Then
                                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('It is mandatory to fill " & dr("ExtraField2").ToString.Replace("*", "") & "');", True)
                                Exit Sub
                            ElseIf (ExtraField1.Text = "" And ExtraField2.Text <> "") Then
                                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('It is mandatory to fill " & dr("ExtraField1").ToString.Replace("*", "") & "');", True)
                                Exit Sub
                            End If

                            If (ExtraField1.Text <> "") Then
                                If (ExtraField1.Text.Length > 10 Or ExtraField1.Text.Length < 10) Then
                                    lblsave.Visible = False
                                    ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('" & dr("ExtraField1").ToString.Replace("*", "") & " should be of 10 digits.');", True)
                                    Exit Sub
                                End If
                            End If
                        End If
                    End If
                    If Not (dr("ExtraField3") = "") Then
                        If (ExtraField3.Text <> "" And ExtraField5.Text = "") Then
                            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('It is mandatory to fill " & dr("ExtraField5").ToString.Replace("*", "") & "');", True)
                            Exit Sub
                        ElseIf (ExtraField3.Text = "" And ExtraField5.Text <> "") Then
                            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('It is mandatory to fill " & dr("ExtraField3").ToString.Replace("*", "") & "');", True)
                            Exit Sub
                        End If

                        If (ExtraField3.Text <> "") Then
                            If (ExtraField3.Text.Length > 12 Or ExtraField3.Text.Length < 8) Then
                                lblsave.Visible = False
                                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('" & dr("ExtraField3").ToString.Replace("*", "") & " should be in between 8 to 12 digits.');", True)
                                Exit Sub
                            End If
                        End If
                    End If

                    If (Session("WebTableID").ToString = "805" Or Session("WebTableID").ToString = "11097" Or Session("WebTableID").ToString = "497") And (ReimbursementType_Code = 2 Or ReimbursementTypeID = 2) Then

                        Dim uploadingcheck As Boolean = uploadcheck()
                        If uploadingcheck = False Then
                            Exit Sub
                        End If


                    End If




                    If (Session("WebtableID") = 510) Then
                        If Not (dr("ExtraField1") = "") And Not (dr("ExtraField3") = "") Then
                            If (ExtraField1.Text <> "") Then
                                Dim dtNumbersvalidation As DataTable
                                dtNumbersvalidation = OldNewConn.GetDataTable2("Select numbers from Numbersvalidation" & Session("WebTableID") & " where Emp_Code='" & Session("Emp_Code") & "' and Numbers in ('" & ExtraField1.Text & "')")

                                If dtNumbersvalidation.Rows.Count > 0 Then
                                    ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You have already claimed against this number " & ExtraField1.Text & " under non-ctc reimbursement, hence this can not be re-claimed.');", True)
                                    Exit Sub
                                End If
                            End If
                            If (ExtraField3.Text <> "") Then
                                Dim dtNumbersvalidation As DataTable
                                dtNumbersvalidation = OldNewConn.GetDataTable2("Select numbers from Numbersvalidation" & Session("WebTableID") & " where Emp_Code='" & Session("Emp_Code") & "' and Numbers in ('" & ExtraField3.Text & "')")

                                If dtNumbersvalidation.Rows.Count > 0 Then
                                    ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You have already claimed against this number " & ExtraField3.Text & " under non-ctc reimbursement, hence this can not be re-claimed.');", True)
                                    Exit Sub
                                End If
                            End If
                        End If
                    End If
                End If
            Next

            For Each dr As DataRow In dtReimburse.Rows

                Dim TransactionDate As Date
                Dim strTransactionDate As Date

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
                    Response.Write("<script>alert('" & dr("TransactionDateTitle") & " should be between " & FromDate.ToString("dd MMM yyyy") & " to " & ToDate.ToString("dd MMM yyyy") & " hence can not be claimed.');</script>")
                    Exit Sub
                End If

                If (ReimbursementID = 0) Then
                    If (dr("BillMonth")) Then
                        If IsNothing(Request.QueryString("ID")) Then
                            Dim policyentry As String
                            policyentry = "Select * from reimbursementdetails" & Session("WebtableID") & " where transactiondate Between '" & Format(FromDate, "MM/dd/yyyy") & "' and '" & Format(ToDate, "MM/dd/yyyy") & "' and Month(TransactionDate)=" & radDatePickerTransactionDate.SelectedDate.Value.Month & " and Reimbursetype_code=" & ReimbursementTypeID & " and Emp_Code='" & Session("Emp_Code") & "'"

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

                    If ExtraDateField.Date >= FromDate.Date And ExtraDateField.Date <= ToDate.Date Then
                    Else
                        Response.Write("<script>alert('" & dr("ExtraDateFieldDescription") & " should be between " & FromDate.ToString("dd MMM yyyy") & " to " & ToDate.ToString("dd MMM yyyy") & " hence can not be claimed.');</script>")
                        Exit Sub
                    End If

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
                If (Session("WebtableID") = 805) Or (Session("WebtableID") = 11097) Then
                    If Not (dr("ExtraField9") = "") Then
                        dict.Add("ExtraField9", ExtraField9.SelectedItem.Text)
                    Else
                        dict.Add("ExtraField9", String.Empty)
                    End If

                    If Not (dr("ExtraField10") = "") Then
                        dict.Add("ExtraField10", ExtraField10.SelectedItem.Text)
                    Else
                        dict.Add("ExtraField10", String.Empty)
                    End If

                    If Not (dr("ExtraField11") = "") Then
                        dict.Add("ExtraField11", ExtraField11.SelectedItem.Text)
                    Else
                        dict.Add("ExtraField11", String.Empty)
                    End If
                End If


                If Not (dr("ExtraField7") = "") Then

                    Dim ExtraDateField As Date = radExtraField7.SelectedDate.Value.Date
                    Dim dtExtraField7 As Date
                    Try
                        If Date.TryParseExact(ExtraDateField.Date.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                            dtExtraField7 = d
                        Else
                            Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                            Exit Sub
                        End If
                    Catch ex As Threading.ThreadAbortException
                    Catch ex As Exception
                        Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                        Exit Sub
                    End Try

                    dict.Add("ExtraField7", dtExtraField7.Date.ToString("MM/dd/yyyy"))
                End If

                If Not (dr("ExtraField8") = "") Then

                    Dim ExtraDateField As Date = radExtraField8.SelectedDate.Value.Date
                    Dim dtExtraField8 As Date
                    Try
                        If Date.TryParseExact(ExtraDateField.Date.ToString("d/M/yyyy"), "d/M/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, d) Then
                            dtExtraField8 = d
                        Else
                            Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                            Exit Sub
                        End If
                    Catch ex As Threading.ThreadAbortException
                    Catch ex As Exception
                        Response.Write("<script>alert('" & Me.lblTransactionDate.Text & " is not valid , please enter the date in DD/MM/YYYY format!');</script>")
                        Exit Sub
                    End Try

                    dict.Add("ExtraField8", dtExtraField8.Date.ToString("MM/dd/yyyy"))
                End If


                If Not (dr("TotalBillField") = "") Then
                    dict.Add("TotalBillField", txtTotalBillField.Text)
                Else
                    dict.Add("TotalBillField", String.Empty)
                End If

                Dim totalAmt1 As Decimal = 0.0

                For Each r As GridDataItem In radGrid1.MasterTableView.Items ' 2nd
                    totalAmt1 = totalAmt1 + r("BillAmount").Text
                Next

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

                For Each r As GridDataItem In radGrid1.MasterTableView.Items ' 3rd

                    Dim BillDated As Date = DateTime.ParseExact(r("BillDated").Text.ToString, "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture)

                    hash.Add(r("BillNumber").Text)

                    If (dtValidDates.Rows.Count > 0) Then

                        If BillDated.Date < Convert.ToDateTime(dtValidDates.Rows(0)("BillValidStartDate")).Date Or BillDated.Date > Convert.ToDateTime(dtValidDates.Rows(0)("BillValidEndDate")).Date Then
                            Response.Write("<script>alert('Bill date is not valid, it should be of Current F.Y. i.e. " & Format(dtValidDates.Rows(0)("BillValidStartDate"), "dd MMM yy") & " to " & Format(dtValidDates.Rows(0)("BillValidEndDate"), "dd MMM yy") & "');</script>")
                            Exit Sub
                        End If
                    End If

                    If BillDated.Date > Today.Date Then
                        Response.Write("<script>alert('You can not submit any post dated claims.');</script>")
                        Exit Sub
                    End If

                    If IsNothing(Request.QueryString("ID")) Then

                        If (ValidTele Or dr("IsUpload")) Then
                            Dim checkDuplicateBills As String = ""

                            If (ViewState("WithOutSubClaimID") = "CSVwithOutSubClaimID") Then

                                checkDuplicateBills = "Select RD.ReimburseDetailsID,MONTH(BillDated) as Month,MONTH(TransactionDate) as 'TransactionMonth',ISNULL((select top 1 status from ReimbursementProofrejection" & Session("WebTableID") & "_" & Year & " where ClaimID=OC.ReimburseDetailsID order by MailSentDate desc),'') as Status from OtherMultipleReimburseClaimDetails" & Session("WebTableID") & " OC inner join reimbursementdetails" & Session("WebTableID") & " RD On OC.ReimburseDetailsId=RD.ReimburseDetailsID "
                                checkDuplicateBills += "where RD.ReimburseType_Code=" & dict("ReimburseType_Code") & " And RD.Emp_Code='" & Session("Emp_code") & "' And OC.BillNumber='" & r("BillNumber").Text & "' And OC.BillDated between '" & Convert.ToDateTime(dtValidDates.Rows(0)("BillValidStartDate")).Date.ToString("MM/dd/yyyy") & "' And '" & Convert.ToDateTime(dtValidDates.Rows(0)("BillValidEndDate")).Date.ToString("MM/dd/yyyy") & "' order by RD.ReimburseDetailsId desc"

                                Dim dtcheckDuplicateBills As DataTable = OldNewConn.GetDataTable2(checkDuplicateBills)

                                If (dtcheckDuplicateBills.Rows.Count > 0) Then
                                    If (dtcheckDuplicateBills.Rows(0)("Status").ToString().ToLower() = "approved" Or dtcheckDuplicateBills.Rows(0)("Status").ToString().ToLower() = "") Then
                                        lblsave.Visible = False
                                        Response.Write("<script>alert('You already have claimed this bill (bill no " & r("BillNumber").Text & ") in the month of " & [Enum].GetName(GetType(EnumMonth1), dtcheckDuplicateBills.Rows(0)("TransactionMonth")).ToString() & ", hence cannot be re-claimed.');</script>")
                                        Exit Sub
                                    End If
                                End If
                            Else
                                checkDuplicateBills = "Select RD.ReimburseDetailsID,MONTH(BillDated) as Month,MONTH(TransactionDate) as 'TransactionMonth',ISNULL((select top 1 status from ReimbursementProofrejection" & Session("WebTableID") & "_" & Year & " where SubClaimID=OC.MultipleClaimDetailsID order by MailSentDate desc),'') as Status from OtherMultipleReimburseClaimDetails" & Session("WebTableID") & " OC inner join reimbursementdetails" & Session("WebTableID") & " RD On OC.ReimburseDetailsId=RD.ReimburseDetailsID "
                                checkDuplicateBills += "where RD.ReimburseType_Code=" & dict("ReimburseType_Code") & " And RD.Emp_Code='" & Session("Emp_code") & "' And OC.BillNumber='" & r("BillNumber").Text & "' And OC.BillDated between '" & Convert.ToDateTime(dtValidDates.Rows(0)("BillValidStartDate")).Date.ToString("MM/dd/yyyy") & "' And '" & Convert.ToDateTime(dtValidDates.Rows(0)("BillValidEndDate")).Date.ToString("MM/dd/yyyy") & "' order by RD.ReimburseDetailsId desc"

                                Dim dtcheckDuplicateBills As DataTable = OldNewConn.GetDataTable2(checkDuplicateBills)

                                If (dtcheckDuplicateBills.Rows.Count > 0) Then
                                    If (dtcheckDuplicateBills.Rows(0)("Status").ToString().ToLower() = "approved" Or dtcheckDuplicateBills.Rows(0)("Status").ToString().ToLower() = "partially approved" Or dtcheckDuplicateBills.Rows(0)("Status").ToString().ToLower() = "") Then
                                        lblsave.Visible = False
                                        Response.Write("<script>alert('You already have claimed this bill (bill no " & r("BillNumber").Text & ") in the month of " & [Enum].GetName(GetType(EnumMonth1), dtcheckDuplicateBills.Rows(0)("TransactionMonth")).ToString() & ", hence cannot be re-claimed.');</script>")
                                        Exit Sub
                                    End If
                                End If
                            End If
                        End If
                    End If

                    If (ValidTele Or dr("IsUpload")) Then
                        If Not String.IsNullOrEmpty(r("MultipleClaimDetailsID").Text) And Not r("MultipleClaimDetailsID").Text = "&nbsp;" Then
                            Dim checkDuplicateBills As String = ""

                            If (ViewState("WithOutSubClaimID") = "CSVwithOutSubClaimID") Then

                                checkDuplicateBills = "Select RD.ReimburseDetailsID,MONTH(BillDated) as Month,MONTH(TransactionDate) as 'TransactionMonth',ISNULL((select top 1 Status from ReimbursementProofrejection" & Session("WebTableID") & "_" & Year & " where ClaimID=OC.ReimburseDetailsID order by MailSentDate desc),'') as Status from "
                                checkDuplicateBills += "OtherMultipleReimburseClaimDetails" & Session("WebTableID") & " OC inner join reimbursementdetails" & Session("WebTableID") & " RD On OC.ReimburseDetailsId=RD.ReimburseDetailsID where RD.ReimburseType_Code=" & dict("ReimburseType_Code") & " And RD.Emp_Code='" & Session("Emp_code") & "' And OC.BillNumber='" & r("BillNumber").Text & "' And "
                                checkDuplicateBills += "OC.BillDated between '" & Convert.ToDateTime(dtValidDates.Rows(0)("BillValidStartDate")).Date.ToString("MM/dd/yyyy") & "' And '" & Convert.ToDateTime(dtValidDates.Rows(0)("BillValidEndDate")).Date.ToString("MM/dd/yyyy") & "' And OC.MultipleClaimDetailsID!=" & r("MultipleClaimDetailsID").Text & " order by RD.ReimburseDetailsId desc"

                                Dim dtcheckDuplicateBills As DataTable = OldNewConn.GetDataTable2(checkDuplicateBills)

                                If (dtcheckDuplicateBills.Rows.Count > 0) Then
                                    If (dtcheckDuplicateBills.Rows(0)("Status").ToString().ToLower() = "approved" Or dtcheckDuplicateBills.Rows(0)("Status").ToString().ToLower() = "") Then
                                        lblsave.Visible = False
                                        Response.Write("<script>alert('You already have claimed this bill (bill no " & r("BillNumber").Text & ") in the month of " & [Enum].GetName(GetType(EnumMonth1), dtcheckDuplicateBills.Rows(0)("TransactionMonth")).ToString() & ", hence cannot be re-claimed.');</script>")
                                        Exit Sub
                                    End If
                                End If
                            Else
                                checkDuplicateBills = "Select RD.ReimburseDetailsID,MONTH(BillDated) as Month,MONTH(TransactionDate) as 'TransactionMonth',ISNULL((select top 1 Status from ReimbursementProofrejection" & Session("WebTableID") & "_" & Year & " where SubClaimID=OC.MultipleClaimDetailsID order by MailSentDate desc),'') as Status from "
                                checkDuplicateBills += "OtherMultipleReimburseClaimDetails" & Session("WebTableID") & " OC inner join reimbursementdetails" & Session("WebTableID") & " RD On OC.ReimburseDetailsId=RD.ReimburseDetailsID where RD.ReimburseType_Code=" & dict("ReimburseType_Code") & " And RD.Emp_Code='" & Session("Emp_code") & "' And OC.BillNumber='" & r("BillNumber").Text & "' And "
                                checkDuplicateBills += "OC.BillDated between '" & Convert.ToDateTime(dtValidDates.Rows(0)("BillValidStartDate")).Date.ToString("MM/dd/yyyy") & "' And '" & Convert.ToDateTime(dtValidDates.Rows(0)("BillValidEndDate")).Date.ToString("MM/dd/yyyy") & "' And OC.MultipleClaimDetailsID!=" & r("MultipleClaimDetailsID").Text & " order by RD.ReimburseDetailsId desc"

                                Dim dtcheckDuplicateBills As DataTable = OldNewConn.GetDataTable2(checkDuplicateBills)

                                If (dtcheckDuplicateBills.Rows.Count > 0) Then
                                    If (dtcheckDuplicateBills.Rows(0)("Status").ToString().ToLower() = "approved" Or dtcheckDuplicateBills.Rows(0)("Status").ToString().ToLower() = "partially approved" Or dtcheckDuplicateBills.Rows(0)("Status").ToString().ToLower() = "") Then
                                        lblsave.Visible = False
                                        Response.Write("<script>alert('You already have claimed this bill (bill no " & r("BillNumber").Text & ") in the month of " & [Enum].GetName(GetType(EnumMonth1), dtcheckDuplicateBills.Rows(0)("TransactionMonth")).ToString() & ", hence cannot be re-claimed.');</script>")
                                        Exit Sub
                                    End If
                                End If
                            End If


                        Else
                            Dim checkDuplicateBills As String = ""

                            If (ViewState("WithOutSubClaimID") = "CSVwithOutSubClaimID") Then
                                checkDuplicateBills = "Select RD.ReimburseDetailsID,MONTH(BillDated) as Month,MONTH(TransactionDate) as 'TransactionMonth',ISNULL((select top 1 status from ReimbursementProofrejection" & Session("WebTableID") & "_" & Year & " where ClaimID=OC.ReimburseDetailsID order by MailSentDate desc),'') as Status from OtherMultipleReimburseClaimDetails" & Session("WebTableID") & " OC inner join reimbursementdetails" & Session("WebTableID") & " RD On OC.ReimburseDetailsId=RD.ReimburseDetailsID "
                                checkDuplicateBills += "where RD.ReimburseType_Code=" & dict("ReimburseType_Code") & " And RD.Emp_Code='" & Session("Emp_code") & "' And OC.BillNumber='" & r("BillNumber").Text & "' And OC.BillDated between '" & Convert.ToDateTime(dtValidDates.Rows(0)("BillValidStartDate")).Date.ToString("MM/dd/yyyy") & "' And '" & Convert.ToDateTime(dtValidDates.Rows(0)("BillValidEndDate")).Date.ToString("MM/dd/yyyy") & "' order by RD.ReimburseDetailsId desc"

                                Dim dtcheckDuplicateBills As DataTable = OldNewConn.GetDataTable2(checkDuplicateBills)

                                If (dtcheckDuplicateBills.Rows.Count > 0) Then
                                    If (dtcheckDuplicateBills.Rows(0)("Status").ToString().ToLower() = "approved" Or dtcheckDuplicateBills.Rows(0)("Status").ToString().ToLower() = "") Then
                                        lblsave.Visible = False
                                        Response.Write("<script>alert('You already have claimed this bill (bill no " & r("BillNumber").Text & ") in the month of " & [Enum].GetName(GetType(EnumMonth1), dtcheckDuplicateBills.Rows(0)("TransactionMonth")).ToString() & ", hence can not be re-claimed.');</script>")
                                        Exit Sub
                                    End If
                                End If
                            Else
                                checkDuplicateBills = "Select RD.ReimburseDetailsID,MONTH(BillDated) as Month,MONTH(TransactionDate) as 'TransactionMonth',ISNULL((select top 1 status from ReimbursementProofrejection" & Session("WebTableID") & "_" & Year & " where SubClaimID=OC.MultipleClaimDetailsID order by MailSentDate desc),'') as Status from OtherMultipleReimburseClaimDetails" & Session("WebTableID") & " OC inner join reimbursementdetails" & Session("WebTableID") & " RD On OC.ReimburseDetailsId=RD.ReimburseDetailsID "
                                checkDuplicateBills += "where RD.ReimburseType_Code=" & dict("ReimburseType_Code") & " And RD.Emp_Code='" & Session("Emp_code") & "' And OC.BillNumber='" & r("BillNumber").Text & "' And OC.BillDated between '" & Convert.ToDateTime(dtValidDates.Rows(0)("BillValidStartDate")).Date.ToString("MM/dd/yyyy") & "' And '" & Convert.ToDateTime(dtValidDates.Rows(0)("BillValidEndDate")).Date.ToString("MM/dd/yyyy") & "' order by RD.ReimburseDetailsId desc"

                                Dim dtcheckDuplicateBills As DataTable = OldNewConn.GetDataTable2(checkDuplicateBills)

                                If (dtcheckDuplicateBills.Rows.Count > 0) Then
                                    If (dtcheckDuplicateBills.Rows(0)("Status").ToString().ToLower() = "approved" Or dtcheckDuplicateBills.Rows(0)("Status").ToString().ToLower() = "partially approved" Or dtcheckDuplicateBills.Rows(0)("Status").ToString().ToLower() = "") Then
                                        lblsave.Visible = False
                                        Response.Write("<script>alert('You already have claimed this bill (bill no " & r("BillNumber").Text & ") in the month of " & [Enum].GetName(GetType(EnumMonth1), dtcheckDuplicateBills.Rows(0)("TransactionMonth")).ToString() & ", hence can not be re-claimed.');</script>")
                                        Exit Sub
                                    End If
                                End If
                            End If
                        End If
                    End If
                Next

                If (type.ToString().ToUpper() = "NON-TAXABLE MEDICAL") Then
                Else
                    If Not (radGrid1.MasterTableView.Items.Count = hash.Count) Then
                        Response.Write("<script>alert('Bill Number should be unique.');</script>")
                        Exit Sub
                    End If
                End If

                If (ValidTele Or dr("IsUpload")) Then
                    If Not (radGrid1.MasterTableView.Items.Count = hash.Count) Then
                        Response.Write("<script>alert('Bill Number should be unique.');</script>")
                        Exit Sub
                    End If
                End If

                If (dr("isMultipleClaimDetails")) Then
                    If (radGrid1.MasterTableView.Items.Count = 0) Then
                        Response.Write("<script>alert('Please submit bill detail above or you can upload the same by using CSV template.');</script>")
                        Exit Sub
                    End If

                    If (panelTotalBill.Visible) Then
                        Dim totalAmt As Decimal

                        For Each r As GridDataItem In radGrid1.MasterTableView.Items '5th
                            totalAmt = totalAmt + r("BillAmount").Text
                        Next

                        If Not (radGrid1.MasterTableView.Items.Count = txtTotalBillField.Text) Then
                            lblError.Text = String.Format("{0} is not matching", lblTotalBillField.Text)
                            Return
                        End If

                        If Not (totalAmt = Amount.Text) Then
                            lblError.Text = String.Format("{0} is not matching", lblAmount.Text)
                            Return
                        End If
                    End If
                End If

                If (dr("IsUpload")) Then
                    If (ReimbursementID = 0) Then
                        If (Session("WebTableID").ToString = "805" Or Session("WebTableID").ToString = "11097" Or Session("WebTableID").ToString = "497") And (ReimbursementType_Code = 2 Or ReimbursementTypeID = 2) Then
                            Dim uploadingcheck As Boolean = uploadcheck()
                            If uploadingcheck = False Then
                                Exit Sub
                            End If
                        Else
                            ' If (IsNothing(Session("UploadedFile"))) Then
                            If (Session("UploadedFile")).ToString = "" Then
                                Response.Write("<script>alert('Please upload bills to submit claim.');</script>")
                                Exit Sub
                            End If
                        End If



                    End If
                End If

                ID = InsertIntoReimburseDetails(dict, ReimbursementID)

                If (dr("isMultipleClaimDetails")) Then

                    Dim sqlParameter(6) As SqlParameter

                    If (ID <> 0) Then
                        DeleteOtherMultipleDetails(ReimbursementID)
                    End If

                    For Each r As GridDataItem In radGrid1.MasterTableView.Items '9th

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
                        sqlParameter(2).ParameterName = "@BillNumber"
                        If (r("BillNumber").Text = "&nbsp;") Then
                            sqlParameter(2).Value = ""
                        Else
                            sqlParameter(2).Value = r("BillNumber").Text
                        End If

                        sqlParameter(3) = New SqlParameter
                        sqlParameter(3).DbType = DbType.DateTime
                        sqlParameter(3).ParameterName = "@BillDated"
                        sqlParameter(3).Value = DateTime.ParseExact(r("BillDated").Text, "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture)

                        sqlParameter(4) = New SqlParameter
                        sqlParameter(4).DbType = DbType.String
                        sqlParameter(4).ParameterName = "@BillAmount"
                        If (r("BillAmount").Text = "&nbsp;") Then
                            sqlParameter(4).Value = ""
                        Else
                            sqlParameter(4).Value = r("BillAmount").Text
                        End If

                        sqlParameter(5) = New SqlParameter
                        sqlParameter(5).DbType = DbType.String
                        sqlParameter(5).ParameterName = "@BillDetail"
                        If (r("BillDetail").Text = "&nbsp;") Then
                            sqlParameter(5).Value = ""
                        Else
                            sqlParameter(5).Value = r("BillDetail").Text
                        End If

                        sqlParameter(6) = New SqlParameter
                        sqlParameter(6).DbType = DbType.Binary
                        sqlParameter(6).ParameterName = "@fileData"
                        Dim buffer As Byte() = New Byte() {}
                        sqlParameter(6).Value = buffer

                        If (ID <> 0) Then
                            Dim claimId As Integer = InsertIntoReimburseMultipleDetails(sqlParameter)
                        End If
                    Next
                End If

                'Upload bill
                Dim count As Integer = 0
                If (dr("IsUpload")) Then
                    Dim dicUploadFile As New Dictionary(Of Integer, UploadedFile)

                    If (Session("WebTableID").ToString = "805" Or Session("WebTableID").ToString = "11097" Or Session("WebTableID").ToString = "497") And (ReimbursementType_Code = 2 Or ReimbursementTypeID = 2) Then
                        Dim sqlParameter(9) As SqlParameter


                        'If Not IsNothing(Session("UploadedFile")) Or Not IsNothing(Session("UploadedFile2")) Then
                        If Not (Session("UploadedFile")).ToString = "" Or Not (Session("UploadedFile2")).ToString = "" Then
                            If (ID <> 0) Then
                                DeleteReimbursementProofUpload(ReimbursementID)
                            End If
                            Dim dicUploadFile2 As New Dictionary(Of Integer, UploadedFile)
                            'dicUploadFile = Session("UploadedFile")
                            'dicUploadFile2 = Session("UploadedFile2")
                            If Not String.IsNullOrEmpty(Session("UploadedFile").ToString) Then
                                dicUploadFile = Session("UploadedFile")
                            End If
                            If Not String.IsNullOrEmpty(Session("UploadedFile2").ToString) Then
                                dicUploadFile2 = Session("UploadedFile2")
                            End If



                            sqlParameter(0) = New SqlParameter
                            sqlParameter(0).DbType = DbType.String
                            sqlParameter(0).ParameterName = "@Emp_code"
                            sqlParameter(0).Value = Session("Emp_code").ToString()
                            Dim radextionstion As String = ""
                            If Not IsNothing(dicUploadFile) Then
                                If dicUploadFile.Count > 0 Then

                                    For Each rad As UploadedFile In dicUploadFile.Values


                                        radextionstion = rad.GetExtension

                                        Dim fStream As FileStream = rad.InputStream

                                        Dim contents As Byte() = New Byte(fStream.Length - 1) {}

                                        fStream.Read(contents, 0, CInt(fStream.Length))

                                        fStream.Close()
                                        sqlParameter(1) = New SqlParameter
                                        sqlParameter(1).DbType = DbType.Binary
                                        sqlParameter(1).ParameterName = "@fileData"
                                        sqlParameter(1).Value = DirectCast(contents, Byte())
                                    Next
                                Else
                                    sqlParameter(1) = New SqlParameter
                                    sqlParameter(1).DbType = DbType.Binary
                                    sqlParameter(1).ParameterName = "@fileData"
                                    sqlParameter(1).Value = DBNull.Value
                                End If
                            Else
                                sqlParameter(1) = New SqlParameter
                                sqlParameter(1).DbType = DbType.Binary
                                sqlParameter(1).ParameterName = "@fileData"
                                sqlParameter(1).Value = DBNull.Value
                            End If







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
                            sqlParameter(4).Value = "0"

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
                            sqlParameter(7).Value = radextionstion
                            Dim radextionstion2 As String = ""
                            If Not IsNothing(dicUploadFile2) Then
                                If dicUploadFile2.Count > 0 Then
                                    For Each rad2 As UploadedFile In dicUploadFile2.Values
                                        radextionstion2 = rad2.GetExtension
                                        Dim fStream2 As FileStream = rad2.InputStream

                                        Dim contents2 As Byte() = New Byte(fStream2.Length - 1) {}

                                        fStream2.Read(contents2, 0, CInt(fStream2.Length))

                                        fStream2.Close()

                                        sqlParameter(8) = New SqlParameter
                                        sqlParameter(8).DbType = DbType.Binary
                                        sqlParameter(8).ParameterName = "@broadbandfileData"
                                        sqlParameter(8).Value = DirectCast(contents2, Byte())
                                    Next
                                Else
                                    sqlParameter(8) = New SqlParameter
                                    sqlParameter(8).DbType = DbType.Binary
                                    sqlParameter(8).ParameterName = "@broadbandfileData"
                                    sqlParameter(8).Value = DBNull.Value
                                End If
                            Else
                                sqlParameter(8) = New SqlParameter
                                sqlParameter(8).DbType = DbType.Binary
                                sqlParameter(8).ParameterName = "@broadbandfileData"
                                sqlParameter(8).Value = DBNull.Value
                            End If


                            sqlParameter(9) = New SqlParameter
                            sqlParameter(9).DbType = DbType.String
                            sqlParameter(9).ParameterName = "@Ext2"
                            sqlParameter(9).Value = radextionstion2


                            If (ID <> 0) Then
                                Dim uploadId As Integer = InsertIntoReimbursementProofUpload(sqlParameter)
                            End If

                        End If
                    Else



                        Dim sqlParameter(7) As SqlParameter


                        ' If Not IsNothing(Session("UploadedFile")) Then
                        If Not (Session("UploadedFile")).ToString = "" Then
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
                                sqlParameter(4).Value = "0"

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
                End If


            Next

            If IsNothing(Request.QueryString("ID")) Then
                Dim sql_Query As String = "select Top 1 ReimburseDetailsID,IsNull(PSS.Print_Name,RTM.Name) as 'Field_Name',convert(nvarchar,EntryDate,106) as 'EntryDate',case when ClaimAmount=0 And RD.ExtraField3 = 0 then RD.ExtraField5 when ClaimAmount=0 then RD.ExtraField2 else ClaimAmount End as 'BillAmount',RD.ExtraField1,RD.ExtraField2,RD.ExtraField4,RD.ExtraField6 from reimbursementdetails" & Session("WebTableID") & " RD inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RTM.ReimburseType_Code=RD.ReimburseType_Code Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RTM.Name where emp_Code = '" & Session("Emp_code") & "' And RTM.Name = '" & type & "' And ReimburseDetailsID=" & ID & "  order by ReimburseDetailsID desc"
                Dim dsReimburseSummary As DataTable = OldNewConn.GetDataTable2(sql_Query)

                Dim Str As String = "Select FirstName+' '+ LastName as EmployeeName, Email from EmployeesMaster" & Session("WebTableID") & " where Emp_Code='" & Session("Emp_code").ToString & "'"
                Dim dtEmployeeEmail As DataTable = OldNewConn.GetDataTable2(Str)

                If (Session("WebtableID") = 510) Then

                    sql_Query = "select Top 1 ReimburseDetailsID,IsNull(PSS.Print_Name,RTM.Name) as 'Field_Name',convert(nvarchar,EntryDate,106) as 'EntryDate',RD.ExtraField1,RD.ExtraField2,RD.ExtraField4,RD.ExtraField6 from reimbursementdetails" & Session("WebTableID") & " RD inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RTM.ReimburseType_Code=RD.ReimburseType_Code Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RTM.Name where emp_Code = '" & Session("Emp_code") & "' And RTM.Name = '" & type & "' And ReimburseDetailsID<>" & ID & "  order by ReimburseDetailsID desc"
                    Dim dsPreviousReimburseSummary As DataTable = OldNewConn.GetDataTable2(sql_Query)

                    If (type.ToString.ToUpper.Contains("RIM_VEHICLE")) Then
                        If (dsPreviousReimburseSummary.Rows.Count > 0) Then
                            If (dsPreviousReimburseSummary.Rows(0)("ExtraField1").ToString.ToLower <> dsReimburseSummary.Rows(0)("ExtraField1").ToString.ToLower Or dsPreviousReimburseSummary.Rows(0)("ExtraField2").ToString.ToLower <> dsReimburseSummary.Rows(0)("ExtraField2").ToString.ToLower Or dsPreviousReimburseSummary.Rows(0)("ExtraField4").ToString.ToLower <> dsReimburseSummary.Rows(0)("ExtraField4").ToString.ToLower Or dsPreviousReimburseSummary.Rows(0)("ExtraField6").ToString.ToLower <> dsReimburseSummary.Rows(0)("ExtraField6").ToString.ToLower) Then
                                Dim Mailsubject As String = "Change Notification - " & dsReimburseSummary.Rows(0)("Field_Name").ToString & ""
                                Dim MailBody As String = "Dear Sir/Madam,<br /><br />This is just to notify that following changes are made by below employee on ESS Reimbursement platform with regard to " & dsReimburseSummary.Rows(0)("Field_Name").ToString & ".<br /><br />Employee ID:" & Session("Emp_code") & "<br />Employee Name:" & StrConv(dtEmployeeEmail.Rows(0)("EmployeeName").ToString, VbStrConv.ProperCase) & "<br /><br />#ClaimDetails#<br /><br />With Best Wishes <br>DKM Online"

                                Dim sb As System.Text.StringBuilder = New StringBuilder()
                                sb.Append("<TABLE border=1 style=""font-family: Segoe UI,Verdana,Arial,Georgia;font-size:10pt"" width=60%>")
                                sb.Append("<tr><th colspan=""2"">As per last claim</th><th style=""border-style: hidden""></th><th colspan=""2"">As per latest claim</th></tr>")
                                sb.Append("<tr><td>Claim Date</td><td>" & dsPreviousReimburseSummary.Rows(0)("EntryDate").ToString & "</td><td style=""border-style: hidden""></td><td>Claim Date</td><td>" & dsReimburseSummary.Rows(0)("EntryDate").ToString & "</td></tr>")
                                sb.Append("<tr><td>Car Registration Number</td><td>" & dsPreviousReimburseSummary.Rows(0)("ExtraField1").ToString & "</td><td style=""border-style: hidden""></td><td>Car Registration Number</td><td>" & dsReimburseSummary.Rows(0)("ExtraField1").ToString & "</td></tr>")
                                sb.Append("<tr><td>Car user name</td><td>" & dsPreviousReimburseSummary.Rows(0)("ExtraField2").ToString & "</td><td style=""border-style: hidden""></td><td>Car user name</td><td>" & dsReimburseSummary.Rows(0)("ExtraField2").ToString & "</td></tr>")
                                sb.Append("<tr><td>Car Engine</td><td>" & dsPreviousReimburseSummary.Rows(0)("ExtraField4").ToString & "</td><td style=""border-style: hidden""></td><td>Car Engine</td><td>" & dsReimburseSummary.Rows(0)("ExtraField4").ToString & "</td></tr>")
                                sb.Append("<tr><td>Car Cubic capacity</td><td>" & dsPreviousReimburseSummary.Rows(0)("ExtraField6").ToString & "</td><td style=""border-style: hidden""></td><td>Car Cubic capacity</td><td>" & dsReimburseSummary.Rows(0)("ExtraField6").ToString & "</td></tr>")
                                sb.Append("</TABLE><br/>")

                                MailBody = Replace(MailBody, "#ClaimDetails#", sb.ToString)

                                Dim flag As Boolean = SendEmail(Session("Emp_code").ToString, "sumit.kumar@dkmonline.com;SumitKumar.Shukla@dkmonline.com;Kamal.Chadha@sunlife.com", Mailsubject, MailBody, "", "", "from5", "subject5", "password5")

                                If (flag = True) Then
                                    Enter_EmailLog(Session("Emp_code").ToString, Session("WebTableID"), 1, "DKMWebApplication", Mailsubject, "To : " & dtEmployeeEmail.Rows(0)("EMail").ToString(), MailBody, "", Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type)
                                End If
                            End If
                        End If
                    ElseIf (type.ToString.ToLower.Contains("driver")) Then
                        If (dsPreviousReimburseSummary.Rows.Count > 0) Then
                            If (dsPreviousReimburseSummary.Rows(0)("ExtraField1").ToString.ToLower <> dsReimburseSummary.Rows(0)("ExtraField1").ToString.ToLower Or dsPreviousReimburseSummary.Rows(0)("ExtraField2").ToString.ToLower <> dsReimburseSummary.Rows(0)("ExtraField2").ToString.ToLower) Then
                                Dim Mailsubject As String = "Change Notification - " & dsReimburseSummary.Rows(0)("Field_Name").ToString & ""
                                Dim MailBody As String = "Dear Sir/Madam,<br /><br />This is just to notify that following changes are made by below employee on ESS Reimbursement platform with regard to " & dsReimburseSummary.Rows(0)("Field_Name").ToString & ".<br /><br />Employee ID:" & Session("Emp_code") & "<br />Employee Name:" & StrConv(dtEmployeeEmail.Rows(0)("EmployeeName").ToString, VbStrConv.ProperCase) & "<br /><br />#ClaimDetails#<br /><br />With Best Wishes <br>DKM Online"

                                Dim sb As System.Text.StringBuilder = New StringBuilder()
                                sb.Append("<TABLE border=1 style=""font-family: Segoe UI,Verdana,Arial,Georgia;font-size:10pt"" width=60%>")
                                sb.Append("<tr><th colspan=""2"">As per last claim</th><th style=""border-style: hidden""></th><th colspan=""2"">As per latest claim</th></tr>")
                                sb.Append("<tr><td>Claim Date</td><td>" & dsPreviousReimburseSummary.Rows(0)("EntryDate").ToString & "</td><td style=""border-style: hidden""></td><td>Claim Date</td><td>" & dsReimburseSummary.Rows(0)("EntryDate").ToString & "</td></tr>")
                                sb.Append("<tr><td>Driver Name</td><td>" & dsPreviousReimburseSummary.Rows(0)("ExtraField1").ToString & "</td><td style=""border-style: hidden""></td><td>Driver Name</td><td>" & dsReimburseSummary.Rows(0)("ExtraField1").ToString & "</td></tr>")
                                sb.Append("<tr><td>Driver License Number</td><td>" & dsPreviousReimburseSummary.Rows(0)("ExtraField2").ToString & "</td><td style=""border-style: hidden""></td><td>Driver License Number</td><td>" & dsReimburseSummary.Rows(0)("ExtraField2").ToString & "</td></tr>")
                                sb.Append("</TABLE><br/>")

                                MailBody = Replace(MailBody, "#ClaimDetails#", sb.ToString)

                                Dim flag As Boolean = SendEmail(Session("Emp_code").ToString, "sumit.kumar@dkmonline.com;SumitKumar.Shukla@dkmonline.com;Kamal.Chadha@sunlife.com", Mailsubject, MailBody, "", "", "from5", "subject5", "password5")

                                If (flag = True) Then
                                    Enter_EmailLog(Session("Emp_code").ToString, Session("WebTableID"), 1, "DKMWebApplication", Mailsubject, "To : " & dtEmployeeEmail.Rows(0)("EMail").ToString(), MailBody, "", Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type)
                                End If
                            End If
                        End If
                    End If
                End If


                Dim dtMailDetail As DataTable = OldNewConn.GetDataTable2("select Field_Detail as MailSubject, Print_Name as MailBody from PayslipSetup" & Session("WebTableID") & " where Name='ReimburseMail'")
                If (dtMailDetail.Rows.Count > 0) Then
                    Dim sb As System.Text.StringBuilder = New StringBuilder()

                    Dim Mailsubject As String = dtMailDetail.Rows(0)("MailSubject").ToString()
                    Dim MailBody As String = dtMailDetail.Rows(0)("MailBody").ToString()

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

                    Dim dtReimbursetable As DataTable = OldNewConn.GetDataTable2("select * from OtherMultipleReimburseClaimDetails" & Session("WebTableID") & " where emp_Code = '" & Session("Emp_code") & "' And ReimburseDetailsID=" & ID & "  order by MultipleClaimDetailsID desc")

                    If (dtReimbursetable.Rows.Count > 0) Then
                        sb.Append("<TABLE border=1 cellPadding=0 width=100% cellSpacing=0 style=" & Chr(34) & "font-family: Segoe UI,Verdana,Arial,Georgia;font-size:10pt" & Chr(34) & " width=60%>")
                        sb.Append("<tr><th>Bill Number</th><th>Bill Date</th><th>Bill Amount</th><th>Bill Detail</th></tr>")
                        For j As Integer = 0 To dtReimbursetable.Rows.Count - 1
                            sb.Append("<tr>")
                            sb.Append("<td>" & dtReimbursetable.Rows(j)("BillNumber").ToString & "</td>")
                            sb.Append("<td>" & CDate(dtReimbursetable.Rows(j)("BillDated")).Date.ToString("dd/MM/yyyy") & "</td>")
                            sb.Append("<td>" & CDec(dtReimbursetable.Rows(j)("BillAmount")).ToString("####.00") & "</td>")
                            sb.Append("<td>" & dtReimbursetable.Rows(j)("BillDetail").ToString & "</td>")
                            sb.Append("</tr>")
                        Next
                        sb.Append("</TABLE><br/>")
                    Else
                        sb.Append("")
                    End If

                    If (dtEmployeeEmail.Rows.Count > 0) Then

                        MailBody = Replace(MailBody, "#EmployeeName#", StrConv(dtEmployeeEmail.Rows(0)("EmployeeName").ToString, VbStrConv.ProperCase))
                        MailBody = Replace(MailBody, "#CurrentStatus#", sb.ToString)

                        If (dsReimburseSummary.Rows.Count > 0) Then
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
                lnkMultiple.Enabled = False
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
                Dim sql_Query As String = "select Top 1 ReimburseDetailsID,IsNull(PSS.Print_Name,RTM.Name) as 'Field_Name',convert(nvarchar,EntryDate,106) as 'EntryDate',case when ClaimAmount=0 And RD.ExtraField3 = 0 then RD.ExtraField5 when ClaimAmount=0 then RD.ExtraField2 else ClaimAmount End as 'BillAmount',RD.ExtraField1,RD.ExtraField2,RD.ExtraField4,RD.ExtraField6 from reimbursementdetails" & Session("WebTableID") & " RD inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RTM.ReimburseType_Code=RD.ReimburseType_Code Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RTM.Name where emp_Code = '" & Session("Emp_code") & "' And RTM.Name = '" & type & "' And ReimburseDetailsID=" & ReimbursementID & "  order by ReimburseDetailsID desc"
                Dim dsReimburseSummary As DataTable = OldNewConn.GetDataTable2(sql_Query)

                Dim Str As String = "Select FirstName+' '+ LastName as EmployeeName, Email from EmployeesMaster" & Session("WebTableID") & " where Emp_Code='" & Session("Emp_code").ToString & "'"
                Dim dtEmployeeEmail As DataTable = OldNewConn.GetDataTable2(Str)

                If (Session("WebtableID") = 510) Then
                    sql_Query = "select Top 1 ReimburseDetailsID,convert(nvarchar,EntryDate,106) as 'EntryDate',RD.ExtraField1,RD.ExtraField2,RD.ExtraField4,RD.ExtraField6 from reimbursementdetails" & Session("WebTableID") & " RD inner join ReimburseMentTypeMaster" & Session("WebTableID") & " RTM on RTM.ReimburseType_Code=RD.ReimburseType_Code Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name =RTM.Name where emp_Code = '" & Session("Emp_code") & "' And RTM.Name = '" & type & "' And ReimburseDetailsID<>" & ReimbursementID & "  order by ReimburseDetailsID desc"
                    Dim dsPreviousReimburseSummary As DataTable = OldNewConn.GetDataTable2(sql_Query)

                    If (type.ToString.ToUpper.Contains("RIM_VEHICLE")) Then
                        If (dsPreviousReimburseSummary.Rows.Count > 0) Then
                            If (dsPreviousReimburseSummary.Rows(0)("ExtraField1").ToString.ToLower <> dsReimburseSummary.Rows(0)("ExtraField1").ToString.ToLower Or dsPreviousReimburseSummary.Rows(0)("ExtraField2").ToString.ToLower <> dsReimburseSummary.Rows(0)("ExtraField2").ToString.ToLower Or dsPreviousReimburseSummary.Rows(0)("ExtraField4").ToString.ToLower <> dsReimburseSummary.Rows(0)("ExtraField4").ToString.ToLower Or dsPreviousReimburseSummary.Rows(0)("ExtraField6").ToString.ToLower <> dsReimburseSummary.Rows(0)("ExtraField6").ToString.ToLower) Then
                                Dim Mailsubject As String = "Change Notification - " & dsReimburseSummary.Rows(0)("Field_Name").ToString & ""
                                Dim MailBody As String = "Dear Sir/Madam,<br /><br />This is just to notify that following changes are made by below employee on ESS Reimbursement platform with regard to " & dsReimburseSummary.Rows(0)("Field_Name").ToString & ".<br /><br />Employee ID:" & Session("Emp_code") & "<br />Employee Name:" & StrConv(dtEmployeeEmail.Rows(0)("EmployeeName").ToString, VbStrConv.ProperCase) & "<br /><br />#ClaimDetails#<br /><br />With Best Wishes <br>DKM Online"

                                Dim sb As System.Text.StringBuilder = New StringBuilder()
                                sb.Append("<TABLE border=1 style=""font-family: Segoe UI,Verdana,Arial,Georgia;font-size:10pt"" width=60%>")
                                sb.Append("<tr><th colspan=""2"">As per last claim</th><th style=""border-style: hidden""></th><th colspan=""2"">As per latest claim</th></tr>")
                                sb.Append("<tr><td>Claim Date</td><td>" & dsPreviousReimburseSummary.Rows(0)("EntryDate").ToString & "</td><td style=""border-style: hidden""></td><td>Claim Date</td><td>" & dsReimburseSummary.Rows(0)("EntryDate").ToString & "</td></tr>")
                                sb.Append("<tr><td>Car Registration Number</td><td>" & dsPreviousReimburseSummary.Rows(0)("ExtraField1").ToString & "</td><td style=""border-style: hidden""></td><td>Car Registration Number</td><td>" & dsReimburseSummary.Rows(0)("ExtraField1").ToString & "</td></tr>")
                                sb.Append("<tr><td>Car user name</td><td>" & dsPreviousReimburseSummary.Rows(0)("ExtraField2").ToString & "</td><td style=""border-style: hidden""></td><td>Car user name</td><td>" & dsReimburseSummary.Rows(0)("ExtraField2").ToString & "</td></tr>")
                                sb.Append("<tr><td>Car Engine</td><td>" & dsPreviousReimburseSummary.Rows(0)("ExtraField4").ToString & "</td><td style=""border-style: hidden""></td><td>Car Engine</td><td>" & dsReimburseSummary.Rows(0)("ExtraField4").ToString & "</td></tr>")
                                sb.Append("<tr><td>Car Cubic capacity</td><td>" & dsPreviousReimburseSummary.Rows(0)("ExtraField6").ToString & "</td><td style=""border-style: hidden""></td><td>Car Cubic capacity</td><td>" & dsReimburseSummary.Rows(0)("ExtraField6").ToString & "</td></tr>")
                                sb.Append("</TABLE><br/>")

                                MailBody = Replace(MailBody, "#ClaimDetails#", sb.ToString)

                                Dim flag As Boolean = SendEmail(Session("Emp_code").ToString, "sumit.kumar@dkmonline.com;SumitKumar.Shukla@dkmonline.com;Kamal.Chadha@sunlife.com", Mailsubject, MailBody, "", "", "from5", "subject5", "password5")

                                If (flag = True) Then
                                    Enter_EmailLog(Session("Emp_code").ToString, Session("WebTableID"), 1, "DKMWebApplication", Mailsubject, "To : " & dtEmployeeEmail.Rows(0)("EMail").ToString(), MailBody, "", Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type)
                                End If
                            End If
                        End If
                    ElseIf (type.ToString.ToLower.Contains("driver")) Then
                        If (dsPreviousReimburseSummary.Rows.Count > 0) Then
                            If (dsPreviousReimburseSummary.Rows(0)("ExtraField1").ToString.ToLower <> dsReimburseSummary.Rows(0)("ExtraField1").ToString.ToLower Or dsPreviousReimburseSummary.Rows(0)("ExtraField2").ToString.ToLower <> dsReimburseSummary.Rows(0)("ExtraField2").ToString.ToLower) Then
                                Dim Mailsubject As String = "Change Notification - " & dsReimburseSummary.Rows(0)("Field_Name").ToString & ""
                                Dim MailBody As String = "Dear Sir/Madam,<br /><br />This is just to notify that following changes are made by below employee on ESS Reimbursement platform with regard to " & dsReimburseSummary.Rows(0)("Field_Name").ToString & ".<br /><br />Employee ID:" & Session("Emp_code") & "<br />Employee Name:" & StrConv(dtEmployeeEmail.Rows(0)("EmployeeName").ToString, VbStrConv.ProperCase) & "<br /><br />#ClaimDetails#<br /><br />With Best Wishes <br>DKM Online"

                                Dim sb As System.Text.StringBuilder = New StringBuilder()
                                sb.Append("<TABLE border=1 style=""font-family: Segoe UI,Verdana,Arial,Georgia;font-size:10pt"" width=60%>")
                                sb.Append("<tr><th colspan=""2"">As per last claim</th><th style=""border-style: hidden""></th><th colspan=""2"">As per latest claim</th></tr>")
                                sb.Append("<tr><td>Claim Date</td><td>" & dsPreviousReimburseSummary.Rows(0)("EntryDate").ToString & "</td><td style=""border-style: hidden""></td><td>Claim Date</td><td>" & dsReimburseSummary.Rows(0)("EntryDate").ToString & "</td></tr>")
                                sb.Append("<tr><td>Driver Name</td><td>" & dsPreviousReimburseSummary.Rows(0)("ExtraField1").ToString & "</td><td style=""border-style: hidden""></td><td>Driver Name</td><td>" & dsReimburseSummary.Rows(0)("ExtraField1").ToString & "</td></tr>")
                                sb.Append("<tr><td>Driver License Number</td><td>" & dsPreviousReimburseSummary.Rows(0)("ExtraField2").ToString & "</td><td style=""border-style: hidden""></td><td>Driver License Number</td><td>" & dsReimburseSummary.Rows(0)("ExtraField2").ToString & "</td></tr>")
                                sb.Append("</TABLE><br/>")

                                MailBody = Replace(MailBody, "#ClaimDetails#", sb.ToString)

                                Dim flag As Boolean = SendEmail(Session("Emp_code").ToString, "sumit.kumar@dkmonline.com;SumitKumar.Shukla@dkmonline.com;Kamal.Chadha@sunlife.com", Mailsubject, MailBody, "", "", "from5", "subject5", "password5")

                                If (flag = True) Then
                                    Enter_EmailLog(Session("Emp_code").ToString, Session("WebTableID"), 1, "DKMWebApplication", Mailsubject, "To : " & dtEmployeeEmail.Rows(0)("EMail").ToString(), MailBody, "", Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type)
                                End If
                            End If
                        End If
                    End If
                End If

                Dim dtMailDetail As DataTable = OldNewConn.GetDataTable2("select Field_Detail as MailSubject, Print_Name as MailBody from PayslipSetup" & Session("WebTableID") & " where Name='ReimburseMail'")
                If (dtMailDetail.Rows.Count > 0) Then
                    Dim sb As System.Text.StringBuilder = New StringBuilder()

                    Dim Mailsubject As String = dtMailDetail.Rows(0)("MailSubject").ToString()
                    Dim MailBody As String = dtMailDetail.Rows(0)("MailBody").ToString()

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

                    Dim dtReimbursetable As DataTable = OldNewConn.GetDataTable2("select * from OtherMultipleReimburseClaimDetails" & Session("WebTableID") & " where emp_Code = '" & Session("Emp_code") & "' And ReimburseDetailsID=" & ReimbursementID & "  order by MultipleClaimDetailsID desc")

                    If (dtReimbursetable.Rows.Count > 0) Then
                        sb.Append("<TABLE border=1 cellPadding=0 width=100% cellSpacing=0 style=" & Chr(34) & "font-family: Segoe UI,Verdana,Arial,Georgia;font-size:10pt" & Chr(34) & " width=60%>")
                        sb.Append("<tr><th>Bill Number</th><th>Bill Date</th><th>Bill Amount</th><th>Bill Detail</th></tr>")

                        For j As Integer = 0 To dtReimbursetable.Rows.Count - 1
                            sb.Append("<tr>")
                            sb.Append("<td>" & dtReimbursetable.Rows(j)("BillNumber").ToString & "</td>")
                            sb.Append("<td>" & CDate(dtReimbursetable.Rows(j)("BillDated")).Date.ToString("dd/MM/yyyy") & "</td>")
                            sb.Append("<td>" & CDec(dtReimbursetable.Rows(j)("BillAmount")).ToString("####.00") & "</td>")
                            sb.Append("<td>" & dtReimbursetable.Rows(j)("BillDetail").ToString & "</td>")
                            sb.Append("</tr>")
                        Next
                        sb.Append("</TABLE><br/>")
                    Else
                        sb.Append("")
                    End If

                    If (dtEmployeeEmail.Rows.Count > 0) Then

                        MailBody = Replace(MailBody, "#EmployeeName#", StrConv(dtEmployeeEmail.Rows(0)("EmployeeName").ToString, VbStrConv.ProperCase))
                        MailBody = Replace(MailBody, "#CurrentStatus#", sb.ToString)

                        If (dsReimburseSummary.Rows.Count > 0) Then
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
                lnkMultiple.Enabled = True
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

            If IsNothing(Request.QueryString("ID")) Then
                IsEntrySaved = True
                radGrid1.Enabled = False
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

    Public Function GetPersonalDetailsofReimbursementMultiple(MultipleClaimDetailsID As Integer) As DataTable
        Try
            Dim extraSql As String = ""
            Dim extraSql2 As String = ""

            If MultipleClaimDetailsID = 0 Then
                commendText = "select ROW_NUMBER() OVER (ORDER BY Emp_Code) AS 'Sl.No'," & extraSql & " BillNumber ," & extraSql2 & " BillDated , Round(BillAmount ,2) as BillAmount , BillDetail ,MultipleClaimDetailsID from OtherMultipleReimburseClaimDetails" & HttpContext.Current.Session("WebTableID") & " where MultipleClaimDetailsID =0 ORDER BY BillDated"
            Else
                commendText = "select ROW_NUMBER() OVER (ORDER BY Emp_Code) AS 'Sl.No'," & extraSql & " BillNumber ," & extraSql2 & " BillDated , Round(BillAmount ,2) as BillAmount , BillDetail ,MultipleClaimDetailsID from OtherMultipleReimburseClaimDetails" & HttpContext.Current.Session("WebTableID") & " where ReimburseDetailsID = " & MultipleClaimDetailsID & " ORDER BY BillDated"
            End If

            Return OldNewConn.GetDataTable2(commendText)

        Catch ex As Exception
            Return New DataTable
        End Try
    End Function

    Public Function DeleteOtherMultipleDetails(ReimbursementID As Integer) As Integer
        Try
            commendText = String.Format("delete  from  OtherMultipleReimburseClaimDetails{0}  where Emp_Code='" & Session("Emp_Code") & "' and ReimburseDetailsId='" & ReimbursementID & "'", HttpContext.Current.Session("WebTableID"))
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

    Public Function InsertIntoReimburseMultipleDetails(ByVal SqlParameter As SqlParameter()) As Integer
        commendText = "Insert into OtherMultipleReimburseClaimDetails" & Session("WebTableID") & " (ReimburseDetailsId,Emp_Code,BillNumber,BillDated,BillAmount,BillDetail,fileData) values (@ReimburseDetailsId,@Emp_Code,@BillNumber,@BillDated,@BillAmount,@BillDetail,@fileData);Select Scope_Identity();"

        Return OldNewConn.ExecuteScalar(CommandType.Text, commendText, SqlParameter)

    End Function

    Public Function InsertIntoReimbursementProofUpload(ByVal SqlParameter As SqlParameter()) As Integer
        If (Session("WebTableID").ToString = "805" Or Session("WebTableID").ToString = "11097" Or Session("WebTableID").ToString = "497") And (ReimbursementType_Code = 2 Or ReimbursementTypeID = 2) Then
            commendText = "insert into ReimbursementProofUpload" & Session("WebTableID") & " (Emp_Code,fileData,Reimbursetype_Code,ReimburseDetailsId,IsApproved,Remarks,SentDate,Ext,broadbandfileData,Ext2) values (@Emp_Code,@fileData,@Reimbursetype_Code,@ReimburseDetailsId,@IsApproved,@Remarks,@SentDate,@Ext,@broadbandfileData,@Ext2);Select Scope_Identity();"

        Else
            commendText = "insert into ReimbursementProofUpload" & Session("WebTableID") & " (Emp_Code,fileData,Reimbursetype_Code,ReimburseDetailsId,IsApproved,Remarks,SentDate,Ext) values (@Emp_Code,@fileData,@Reimbursetype_Code,@ReimburseDetailsId,@IsApproved,@Remarks,@SentDate,@Ext);Select Scope_Identity();"
        End If
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
                commendText = String.Format("Update reimbursementdetails" & Session("WebtableID") & " set {0} where  ReimburseDetailsID=" & ReimbursementID & "", strKeys)
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

    Protected Sub btnUploadBill2_Click(sender As Object, e As EventArgs)
        Session("UploadedFile2") = ""
        Session("file1name2") = ""
        lit1.Text = ""
        Dim dicUploadFile2 As New Dictionary(Of Integer, UploadedFile)
        Dim sb As StringBuilder = New StringBuilder()

        If (RadAsyncUpload2.UploadedFiles.Count = 0) Then
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please Select jpeg/jpg/png/pdf File.');", True)
            Exit Sub
        End If

        If (Session("WebTableID").ToString = "805" Or Session("WebTableID").ToString = "11097" Or Session("WebTableID").ToString = "497") And (ReimbursementType_Code = 2 Or ReimbursementTypeID = 2) Then



            If ExtraField4.SelectedItem.Text = "Yes" And ExtraField6.SelectedItem.Text = "Yes" And ExtraField9.SelectedItem.Text = "Yes" Then
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You can not upload personal mobile phone bill and personal broadband bill as company has already provided you benefit.');", True)
                Exit Sub
            ElseIf ExtraField4.SelectedItem.Text = "No" And ExtraField6.SelectedItem.Text = "No" And ExtraField9.SelectedItem.Text = "Yes" Then
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You can not upload personal broadband bill.You can upload only personal mobile phone bill as company has already provided you benefit.');", True)
                Exit Sub
            ElseIf ExtraField4.SelectedItem.Text = "Yes" And ExtraField6.SelectedItem.Text = "No" And ExtraField9.SelectedItem.Text = "Yes" Then
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You can not upload personal broadband bill and personal mobile phone bill as company has already provided you benefit.');", True)
                Exit Sub
            ElseIf ExtraField4.SelectedItem.Text = "No" And ExtraField6.SelectedItem.Text = "Yes" And ExtraField9.SelectedItem.Text = "Yes" Then
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You can not upload personal broadband bill and personal mobile phone bill as company has already provided you benefit.');", True)
                Exit Sub
            End If

        End If



        If (RadAsyncUpload2.UploadedFiles.Count > 0) Then
            Dim i As Int32 = 0
            For Each rad As UploadedFile In RadAsyncUpload2.UploadedFiles
                i += 1
                dicUploadFile2.Add(i, rad)
                sb.Append("(" & i & ")  ")
                sb.Append(rad.GetName)
                Session("file1name2") = rad.GetName
                sb.Append("   ")
            Next

            'If Not String.IsNullOrEmpty(sb.ToString()) Then
            '    lit1.Text = Session("file1name").ToString
            'End If

            If Not IsNothing(Session("file1name")) Then
                If Not String.IsNullOrEmpty(Session("file1name").ToString) Then
                    If String.IsNullOrEmpty(lit1.Text) Then
                        lit1.Text = Session("file1name").ToString
                    Else
                        lit1.Text = lit1.Text & "<br/>" & Session("file1name").ToString
                    End If
                End If

            End If
            If Not IsNothing(Session("file1name2")) Then
                If Not String.IsNullOrEmpty(Session("file1name2").ToString) Then
                    If String.IsNullOrEmpty(lit1.Text) Then
                        lit1.Text = Session("file1name2").ToString
                    Else
                        lit1.Text = lit1.Text & "<br/>" & Session("file1name2").ToString
                    End If
                End If

            End If


            'If String.IsNullOrEmpty(lit1.Text) Then
            '    lit1.Text = sb.ToString()
            'Else
            '    lit1.Text = lit1.Text & "<br/>" & sb.ToString()
            'End If
            Session("UploadedFile2") = dicUploadFile2
        End If
        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Bills uploaded, please proceed to submit claim by clicking on ""Submit Claim""');", True)

    End Sub

    Protected Sub ExtraField11_SelectedIndexChanged(sender As Object, e As EventArgs)
        If (Session("WebTableID").ToString = "805" Or Session("WebTableID").ToString = "11097" Or Session("WebTableID").ToString = "497") And (ReimbursementType_Code = 2 Or ReimbursementTypeID = 2) Then

            If ExtraField11.SelectedItem.Text = "other" Then
                ExtraField4.Enabled = False
                ExtraField6.Enabled = False
                ExtraField9.Enabled = False
                ExtraField10.Enabled = False
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Other than the employee’s name on the bill is not admissible');", True)
                Exit Sub
            Else
                ExtraField4.Enabled = True
                ExtraField6.Enabled = True
                ExtraField9.Enabled = True
                ExtraField10.Enabled = True


            End If
        End If
    End Sub

    Public Function uploadcheck() As Boolean
        If (Session("WebTableID").ToString = "805" Or Session("WebTableID").ToString = "11097" Or Session("WebTableID").ToString = "497") And (ReimbursementType_Code = 2 Or ReimbursementTypeID = 2) Then
            'If IsNothing(Session("UploadedFile")) And IsNothing(Session("UploadedFile2")) Then
            If Session("UploadedFile").ToString = "" And Session("UploadedFile2").ToString = "" Then
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You are not entitle to submit the claim as you have not uploaded any bill.');", True)
                Return False
            End If

            If ExtraField11.SelectedItem.Text = "Please Select" Then
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please Select Bill Name with');", True)
                Return False
            ElseIf ExtraField11.SelectedItem.Text = "other" Then
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You can not upload bill with other name');", True)
                Return False
            End If
            If ExtraField4.SelectedItem.Text = "Yes" And ExtraField6.SelectedItem.Text = "No" And ExtraField9.SelectedItem.Text = "No" Then
                'If Not String.IsNullOrEmpty(lit1.Text) Then
                'If Not lit1.Text = "" Then
                If Not IsNothing(Session("UploadedFile")) Then
                    If Session("UploadedFile").ToString <> "" Then
                        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You can not upload personal mobile phone bill. You can upload only personal broadband bill as company has already provided you benefit.');", True)
                        Session("UploadedFile") = ""

                        lit1.Text = ""
                        Return False
                    End If
                End If
                If IsNothing(Session("UploadedFile2")) Then
                    If String.IsNullOrEmpty(Session("UploadedFile2").ToString) Then
                        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please upload personal broadband bill');", True)
                        Session("UploadedFile") = ""
                        Session("UploadedFile2") = ""
                        lit1.Text = ""
                        Return False
                    End If
                End If


            ElseIf ExtraField4.SelectedItem.Text = "No" And ExtraField6.SelectedItem.Text = "Yes" And ExtraField9.SelectedItem.Text = "No" Then
                'If Not IsNothing(Session("UploadedFile")) Then
                If Not (Session("UploadedFile")).ToString = "" Then
                    ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You can not upload personal mobile phone bill.You can upload only personal broadband bill as company has already provided you benefit.');", True)
                    Session("UploadedFile") = ""

                    lit1.Text = ""
                    Return False
                End If
                If IsNothing(Session("UploadedFile2")) Then
                    If String.IsNullOrEmpty(Session("UploadedFile2").ToString) Then
                        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please upload personal broadband bill');", True)
                        Session("UploadedFile") = ""
                        Session("UploadedFile2") = ""
                        lit1.Text = ""
                        Return False
                    End If
                End If
            ElseIf ExtraField4.SelectedItem.Text = "Yes" And ExtraField6.SelectedItem.Text = "Yes" And ExtraField9.SelectedItem.Text = "Yes" Then
                If Session("UploadedFile").ToString <> "" Or Session("UploadedFile2").ToString <> "" Then
                    ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You can not upload personal mobile phone bill and personal broadband bill as company has already provided you benefit.');", True)
                    Session("UploadedFile") = ""
                    Session("UploadedFile2") = ""
                    lit1.Text = ""
                    Return False
                End If
            ElseIf ExtraField4.SelectedItem.Text = "No" And ExtraField6.SelectedItem.Text = "No" And ExtraField9.SelectedItem.Text = "Yes" Then

                If Not IsNothing(Session("UploadedFile2")) Then
                    If Session("UploadedFile2").ToString <> "" Then
                        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You can not upload personal broadband bill.You can upload only personal mobile phone bill as company has already provided you benefit.');", True)

                        Session("UploadedFile2") = ""
                        lit1.Text = ""
                        Return False
                    End If
                End If
                If IsNothing(Session("UploadedFile")) Then
                    If String.IsNullOrEmpty(Session("UploadedFile").ToString) Then
                        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please upload personal mobile phone bill');", True)
                        Session("UploadedFile") = ""
                        Session("UploadedFile2") = ""
                        lit1.Text = ""
                        Return False
                    End If
                End If

            ElseIf ExtraField4.SelectedItem.Text = "Yes" And ExtraField6.SelectedItem.Text = "No" And ExtraField9.SelectedItem.Text = "Yes" Then
                If Session("UploadedFile").ToString <> "" Or Session("UploadedFile2").ToString <> "" Then
                    ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You can not upload personal broadband bill and personal mobile phone bill as company has already provided you benefit.');", True)
                    Session("UploadedFile") = ""
                    Session("UploadedFile2") = ""
                    lit1.Text = ""
                    Return False
                End If


            ElseIf ExtraField4.SelectedItem.Text = "No" And ExtraField6.SelectedItem.Text = "Yes" And ExtraField9.SelectedItem.Text = "Yes" Then
                If Session("UploadedFile").ToString <> "" Or Session("UploadedFile2").ToString <> "" Then
                    ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You can not upload personal broadband bill and personal mobile phone bill as company has already provided you benefit.');", True)
                    Session("UploadedFile") = ""
                    Session("UploadedFile2") = ""
                    lit1.Text = ""
                    Return False
                End If

            End If
            'If (RadAsyncUpload1.UploadedFiles.Count > 0) Then
            '    ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('You can not upload personal mobile phone bill');", True)
            '    Exit Sub
            'End If
            Return True
        End If
    End Function
End Class