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

Public Class ReimburseTypeDetail
    Inherits System.Web.UI.Page

    Dim dicreimType As Dictionary(Of Integer, Byte())
    Dim dtreimType As DataTable
    Public ReimburseType_Code As Integer
    Dim commendText As String
    Dim constantValuesnew As New ConstantValues

    Dim OldNewConn As OldNewConnection = New OldNewConnection()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsNothing(Session("Emp_Code")) Then
            Response.Redirect("../Logout.aspx")
        End If

        DatabaseFacade.Instance.Enter_SiteLog(Session("Emp_Code"), Session("LoginType"), Session("LoginAccessType"), Session("Comp_Code"), 0, Request.Url.AbsolutePath, Request.Url.AbsolutePath, Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type, Session("WebTableID"), Session.SessionID, Session("MenuID"))

        OldNewConn.CheckDatabaseOldNew(Session("WebTableID"))

        If Not IsNothing(Session("Emp_Code")) And Not IsNothing(Session("WebTableID")) And Not IsNothing(Session("MenuID")) Then
            If Not (IsPostBack) Then
                ReturnType()
                bindControls()
            End If
        Else
            Response.Redirect("../Logout.aspx")
        End If
    End Sub

    Protected Sub btnInsert_Click(sender As Object, e As EventArgs)
        Try

            Dim sqlParameter(26) As SqlParameter

            sqlParameter(0) = New SqlParameter
            sqlParameter(0).DbType = DbType.String
            sqlParameter(0).ParameterName = "@Name"
            sqlParameter(0).Value = ddlReimType.SelectedItem.ToString()

            sqlParameter(1) = New SqlParameter
            sqlParameter(1).DbType = DbType.String
            sqlParameter(1).ParameterName = "@TransactionDateTitle"
            sqlParameter(1).Value = txttranDatetitle.Text

            sqlParameter(2) = New SqlParameter
            sqlParameter(2).DbType = DbType.String
            sqlParameter(2).ParameterName = "@TransactionTitle"
            sqlParameter(2).Value = txttransTitle.Text

            sqlParameter(3) = New SqlParameter
            sqlParameter(3).DbType = DbType.String
            sqlParameter(3).ParameterName = "@NeedExtraDateField"
            sqlParameter(3).Value = chkneedextdateField.Checked

            sqlParameter(4) = New SqlParameter
            sqlParameter(4).DbType = DbType.String
            sqlParameter(4).ParameterName = "@ExtraDateFieldDescription"
            sqlParameter(4).Value = txtExtdateFieldDesc.Text

            sqlParameter(5) = New SqlParameter
            sqlParameter(5).DbType = DbType.String
            sqlParameter(5).ParameterName = "@ExtraField1"
            sqlParameter(5).Value = txtExtrafield1.Text

            sqlParameter(6) = New SqlParameter
            sqlParameter(6).DbType = DbType.String
            sqlParameter(6).ParameterName = "@ExtraField2"
            sqlParameter(6).Value = txtExtrafield2.Text

            sqlParameter(7) = New SqlParameter
            sqlParameter(7).DbType = DbType.String
            sqlParameter(7).ParameterName = "@ExtraField3"
            sqlParameter(7).Value = txtExtrafield3.Text

            sqlParameter(8) = New SqlParameter
            sqlParameter(8).DbType = DbType.String
            sqlParameter(8).ParameterName = "@ExtraField5"
            sqlParameter(8).Value = txtExtrafield5.Text

            sqlParameter(9) = New SqlParameter
            sqlParameter(9).DbType = DbType.String
            sqlParameter(9).ParameterName = "@ExtraField4"
            sqlParameter(9).Value = txtExtrafield4.Text

            sqlParameter(10) = New SqlParameter
            sqlParameter(10).DbType = DbType.String
            sqlParameter(10).ParameterName = "@ExtraField4Description"
            sqlParameter(10).Value = txtExtrafield4desc.Text

            sqlParameter(11) = New SqlParameter
            sqlParameter(11).DbType = DbType.String
            sqlParameter(11).ParameterName = "@ExtraField6"
            sqlParameter(11).Value = txtExtrafield6.Text

            sqlParameter(12) = New SqlParameter
            sqlParameter(12).DbType = DbType.String
            sqlParameter(12).ParameterName = "@ExtraField6Description"
            sqlParameter(12).Value = txtExtrafield6desc.Text

            sqlParameter(13) = New SqlParameter
            sqlParameter(13).DbType = DbType.String
            sqlParameter(13).ParameterName = "@BillMonth"
            sqlParameter(13).Value = chkbillMonth.Checked

            sqlParameter(14) = New SqlParameter
            sqlParameter(14).DbType = DbType.DateTime
            sqlParameter(14).ParameterName = "@BillValidStartDate"
            sqlParameter(14).Value = radDatebillvalidstartDate.SelectedDate

            sqlParameter(15) = New SqlParameter
            sqlParameter(15).DbType = DbType.DateTime
            sqlParameter(15).ParameterName = "@BillValidEndDate"
            sqlParameter(15).Value = radDatebillvalidendDate.SelectedDate

            sqlParameter(16) = New SqlParameter
            sqlParameter(16).DbType = DbType.String
            sqlParameter(16).ParameterName = "@isMultipleClaimDetails"
            sqlParameter(16).Value = chkmulticlaimreq.Checked

            sqlParameter(17) = New SqlParameter
            sqlParameter(17).DbType = DbType.String
            sqlParameter(17).ParameterName = "@isPersonsDetailRequired"
            sqlParameter(17).Value = chkpersondetailreq.Checked

            sqlParameter(18) = New SqlParameter
            sqlParameter(18).DbType = DbType.String
            sqlParameter(18).ParameterName = "@ClaimAmountField"
            sqlParameter(18).Value = txtClaimField.Text

            sqlParameter(19) = New SqlParameter
            sqlParameter(19).DbType = DbType.String
            sqlParameter(19).ParameterName = "@TotalBillField"
            sqlParameter(19).Value = txtTotalBillFiled.Text

            sqlParameter(20) = New SqlParameter
            sqlParameter(20).DbType = DbType.String
            sqlParameter(20).ParameterName = "@IsUpload"
            sqlParameter(20).Value = chkUpload.Checked

            sqlParameter(21) = New SqlParameter
            sqlParameter(21).DbType = DbType.String
            sqlParameter(21).ParameterName = "@IsInactive"
            sqlParameter(21).Value = chkActive.Checked

            sqlParameter(22) = New SqlParameter
            sqlParameter(22).DbType = DbType.String
            sqlParameter(22).ParameterName = "@ReimbursePolicy"
            sqlParameter(22).Value = ""

            sqlParameter(23) = New SqlParameter
            sqlParameter(23).DbType = DbType.String
            sqlParameter(23).ParameterName = "@Disclaimer"
            sqlParameter(23).Value = txtDisclaimer.Text

            sqlParameter(24) = New SqlParameter
            sqlParameter(24).DbType = DbType.String
            sqlParameter(24).ParameterName = "@ExtraField7"
            sqlParameter(24).Value = txtExtrafield7.Text

            sqlParameter(25) = New SqlParameter
            sqlParameter(25).DbType = DbType.String
            sqlParameter(25).ParameterName = "@ExtraField8"
            sqlParameter(25).Value = txtExtrafield8.Text

            sqlParameter(26) = New SqlParameter
            sqlParameter(26).DbType = DbType.String
            sqlParameter(26).ParameterName = "@ReimburseApplyNote"
            sqlParameter(26).Value = txtReimburseApplyNote.Text

            Dim reimID As Integer = InsertIntoReimburseTypeDetails(sqlParameter)

            If (reimID > 0) Then
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "close", "InsertClose();", True)
            End If
        Catch ex As Exception
            Response.Write("<script>alert('Error');</script>")
        End Try
    End Sub

    Protected Sub btnUpdate_Click(sender As Object, e As EventArgs)
        Dim sqlParameter(26) As SqlParameter

        sqlParameter(0) = New SqlParameter
        sqlParameter(0).DbType = DbType.String
        sqlParameter(0).ParameterName = "@Name"
        sqlParameter(0).Value = ddlReimType.SelectedItem.ToString()

        sqlParameter(1) = New SqlParameter
        sqlParameter(1).DbType = DbType.String
        sqlParameter(1).ParameterName = "@TransactionDateTitle"
        sqlParameter(1).Value = txttranDatetitle.Text

        sqlParameter(2) = New SqlParameter
        sqlParameter(2).DbType = DbType.String
        sqlParameter(2).ParameterName = "@TransactionTitle"
        sqlParameter(2).Value = txttransTitle.Text

        sqlParameter(3) = New SqlParameter
        sqlParameter(3).DbType = DbType.String
        sqlParameter(3).ParameterName = "@NeedExtraDateField"
        sqlParameter(3).Value = chkneedextdateField.Checked

        sqlParameter(4) = New SqlParameter
        sqlParameter(4).DbType = DbType.String
        sqlParameter(4).ParameterName = "@ExtraDateFieldDescription"
        sqlParameter(4).Value = txtExtdateFieldDesc.Text

        sqlParameter(5) = New SqlParameter
        sqlParameter(5).DbType = DbType.String
        sqlParameter(5).ParameterName = "@ExtraField1"
        sqlParameter(5).Value = txtExtrafield1.Text

        sqlParameter(6) = New SqlParameter
        sqlParameter(6).DbType = DbType.String
        sqlParameter(6).ParameterName = "@ExtraField2"
        sqlParameter(6).Value = txtExtrafield2.Text

        sqlParameter(7) = New SqlParameter
        sqlParameter(7).DbType = DbType.String
        sqlParameter(7).ParameterName = "@ExtraField3"
        sqlParameter(7).Value = txtExtrafield3.Text

        sqlParameter(8) = New SqlParameter
        sqlParameter(8).DbType = DbType.String
        sqlParameter(8).ParameterName = "@ExtraField5"
        sqlParameter(8).Value = txtExtrafield5.Text

        sqlParameter(9) = New SqlParameter
        sqlParameter(9).DbType = DbType.String
        sqlParameter(9).ParameterName = "@ExtraField4"
        sqlParameter(9).Value = txtExtrafield4.Text

        sqlParameter(10) = New SqlParameter
        sqlParameter(10).DbType = DbType.String
        sqlParameter(10).ParameterName = "@ExtraField4Description"
        sqlParameter(10).Value = txtExtrafield4desc.Text

        sqlParameter(11) = New SqlParameter
        sqlParameter(11).DbType = DbType.String
        sqlParameter(11).ParameterName = "@ExtraField6"
        sqlParameter(11).Value = txtExtrafield6.Text

        sqlParameter(12) = New SqlParameter
        sqlParameter(12).DbType = DbType.String
        sqlParameter(12).ParameterName = "@ExtraField6Description"
        sqlParameter(12).Value = txtExtrafield6desc.Text

        sqlParameter(13) = New SqlParameter
        sqlParameter(13).DbType = DbType.String
        sqlParameter(13).ParameterName = "@BillMonth"
        sqlParameter(13).Value = chkbillMonth.Checked

        sqlParameter(14) = New SqlParameter
        sqlParameter(14).DbType = DbType.DateTime
        sqlParameter(14).ParameterName = "@BillValidStartDate"
        sqlParameter(14).Value = radDatebillvalidstartDate.SelectedDate

        sqlParameter(15) = New SqlParameter
        sqlParameter(15).DbType = DbType.DateTime
        sqlParameter(15).ParameterName = "@BillValidEndDate"
        sqlParameter(15).Value = radDatebillvalidendDate.SelectedDate

        sqlParameter(16) = New SqlParameter
        sqlParameter(16).DbType = DbType.String
        sqlParameter(16).ParameterName = "@isMultipleClaimDetails"
        sqlParameter(16).Value = chkmulticlaimreq.Checked

        sqlParameter(17) = New SqlParameter
        sqlParameter(17).DbType = DbType.String
        sqlParameter(17).ParameterName = "@isPersonsDetailRequired"
        sqlParameter(17).Value = chkpersondetailreq.Checked

        sqlParameter(18) = New SqlParameter
        sqlParameter(18).DbType = DbType.String
        sqlParameter(18).ParameterName = "@ClaimAmountField"
        sqlParameter(18).Value = txtClaimField.Text

        sqlParameter(19) = New SqlParameter
        sqlParameter(19).DbType = DbType.String
        sqlParameter(19).ParameterName = "@TotalBillField"
        sqlParameter(19).Value = txtTotalBillFiled.Text

        sqlParameter(20) = New SqlParameter
        sqlParameter(20).DbType = DbType.String
        sqlParameter(20).ParameterName = "@IsUpload"
        sqlParameter(20).Value = chkUpload.Checked

        sqlParameter(21) = New SqlParameter
        sqlParameter(21).DbType = DbType.String
        sqlParameter(21).ParameterName = "@IsInactive"
        sqlParameter(21).Value = chkActive.Checked

        sqlParameter(22) = New SqlParameter
        sqlParameter(22).DbType = DbType.String
        sqlParameter(22).ParameterName = "@ReimbursePolicy"
        sqlParameter(22).Value = ""

        sqlParameter(23) = New SqlParameter
        sqlParameter(23).DbType = DbType.String
        sqlParameter(23).ParameterName = "@Disclaimer"
        sqlParameter(23).Value = txtDisclaimer.Text

        sqlParameter(24) = New SqlParameter
        sqlParameter(24).DbType = DbType.String
        sqlParameter(24).ParameterName = "@ExtraField7"
        sqlParameter(24).Value = txtExtrafield7.Text

        sqlParameter(25) = New SqlParameter
        sqlParameter(25).DbType = DbType.String
        sqlParameter(25).ParameterName = "@ExtraField8"
        sqlParameter(25).Value = txtExtrafield8.Text

        sqlParameter(26) = New SqlParameter
        sqlParameter(26).DbType = DbType.String
        sqlParameter(26).ParameterName = "@ReimburseApplyNote"
        sqlParameter(26).Value = txtReimburseApplyNote.Text

        Dim reimID As Integer = UpdateIntoReimburseTypeDetails(sqlParameter)

        If (reimID > 0) Then

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "close", "UpdateClose();", True)
        End If

    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        ddlReimType.SelectedValue = 0
        txttranDatetitle.Text = ""
        txttransTitle.Text = ""
        chkneedextdateField.Checked = False
        txtExtdateFieldDesc.Text = ""
        txtExtrafield1.Text = ""
        txtExtrafield2.Text = ""
        txtExtrafield3.Text = ""
        txtExtrafield5.Text = ""
        txtExtrafield4.Text = ""
        txtExtrafield4desc.Text = ""
        txtExtrafield6.Text = ""
        txtExtrafield7.Text = ""
        txtExtrafield8.Text = ""
        txtExtrafield6desc.Text = ""
        chkbillMonth.Checked = False
        radDatebillvalidstartDate.SelectedDate = Nothing
        radDatebillvalidendDate.SelectedDate = Nothing
        chkmulticlaimreq.Checked = False
        chkpersondetailreq.Checked = False
        txtClaimField.Text = ""
        txtTotalBillFiled.Text = ""
        txtDisclaimer.Text = ""
        chkUpload.Checked = False
        chkActive.Checked = False
        txtReimburseApplyNote.Text = ""
    End Sub

    ''' <summary>
    ''' Functions
    ''' </summary>
    ''' <remarks></remarks>

    Private Sub bindControls()
        If Not IsNothing(Request.QueryString("ID")) Then
            btnInsert.Visible = False
            btnUpdate.Visible = True

            Dim s As String = Request.QueryString("ID").Replace(" ", "+")

            If s.Length Mod 4 > 0 Then
                s = s.PadRight(s.Length + 4 - s.Length Mod 4, "="c)
            End If

            ReimburseType_Code = EncryDecrypt.Decrypt(s, "EncryptString01")

            ReimburseType_Code = ReimburseType_Code.ToString.Substring(0, ReimburseType_Code.ToString.Length - 3)

            Session("ReimburseType_Code") = ReimburseType_Code.ToString()

            dtreimType = OldNewConn.GetDataTable2("select * from ReimburseMentTypeMaster" & Session("WebTableID") & " where ReimburseType_Code='" & ReimburseType_Code & "'")

            ddlReimType.SelectedValue = ddlReimType.Items.FindByText(dtreimType.Rows(0)("Name").ToString()).Value
            txttranDatetitle.Text = dtreimType.Rows(0)("TransactionDateTitle").ToString()
            txttransTitle.Text = dtreimType.Rows(0)("TransactionTitle").ToString()

            If (dtreimType.Rows(0)("NeedExtraDateField").ToString() = "True") Then
                chkneedextdateField.Checked = True
            Else
                chkneedextdateField.Checked = False
            End If

            txtExtdateFieldDesc.Text = dtreimType.Rows(0)("ExtraDateFieldDescription").ToString()
            txtExtrafield1.Text = dtreimType.Rows(0)("ExtraField1").ToString()
            txtExtrafield2.Text = dtreimType.Rows(0)("ExtraField2").ToString()
            txtExtrafield3.Text = dtreimType.Rows(0)("ExtraField3").ToString()
            txtExtrafield5.Text = dtreimType.Rows(0)("ExtraField5").ToString()
            txtExtrafield4.Text = dtreimType.Rows(0)("ExtraField4").ToString()
            txtExtrafield4desc.Text = dtreimType.Rows(0)("ExtraField4Description").ToString()
            txtExtrafield6.Text = dtreimType.Rows(0)("ExtraField6").ToString()
            txtExtrafield6desc.Text = dtreimType.Rows(0)("ExtraField6Description").ToString()
            txtExtrafield7.Text = dtreimType.Rows(0)("ExtraField7").ToString()
            txtExtrafield8.Text = dtreimType.Rows(0)("ExtraField8").ToString()

            If (dtreimType.Rows(0)("BillMonth").ToString() = "True") Then
                chkbillMonth.Checked = True
            Else
                chkbillMonth.Checked = False
            End If

            radDatebillvalidstartDate.SelectedDate = Convert.ToDateTime(dtreimType.Rows(0)("BillValidStartDate").ToString())
            radDatebillvalidendDate.SelectedDate = Convert.ToDateTime(dtreimType.Rows(0)("BillValidEndDate").ToString())

            If (dtreimType.Rows(0)("isMultipleClaimDetails").ToString() = "True") Then
                chkmulticlaimreq.Checked = True
            Else
                chkmulticlaimreq.Checked = False
            End If

            If (dtreimType.Rows(0)("isPersonsDetailRequired").ToString() = "True") Then
                chkpersondetailreq.Checked = True
            Else
                chkpersondetailreq.Checked = False
            End If

            txtClaimField.Text = dtreimType.Rows(0)("ClaimAmountField").ToString()
            txtTotalBillFiled.Text = dtreimType.Rows(0)("TotalBillField").ToString()
            txtDisclaimer.Text = dtreimType.Rows(0)("Disclaimer").ToString()
            txtReimburseApplyNote.Text = dtreimType.Rows(0)("ReimburseApplyNote").ToString()


            If (dtreimType.Rows(0)("IsUpload").ToString() = "True") Then
                chkUpload.Checked = True
            Else
                chkUpload.Checked = False
            End If

            If (dtreimType.Rows(0)("IsInactive").ToString() = "True") Then
                chkActive.Checked = True
            Else
                chkActive.Checked = False
            End If
        Else
            btnInsert.Visible = True
            btnUpdate.Visible = False
        End If
    End Sub

    Private Sub ReturnType()

        Dim dttype As DataTable
        Dim str As String

        If Not IsNothing(Request.QueryString("ID")) Then
            str = "select Name,ReimburseType_Code as 'PolicyID' from ReimburseMentTypeMaster" & Session("WebTableID") & ""
        Else
            str = "select Field_Name as 'Name',PolicyID from ReimbursePolicy" & Session("WebTableID") & ""
        End If

        dttype = OldNewConn.GetDataTable2(str)

        ddlReimType.DataSource = dttype
        ddlReimType.DataTextField = "Name"
        ddlReimType.DataValueField = "PolicyID"
        ddlReimType.DataBind()
    End Sub

    Public Function InsertIntoReimburseTypeDetails(ByVal SqlParameter As SqlParameter()) As Integer
        commendText = "Insert into ReimburseMentTypeMaster" & Session("WebTableID") & " (Name,TransactionDateTitle,TransactionTitle,NeedExtraDateField,ExtraDateFieldDescription,ExtraField1,ExtraField2,ExtraField3,ExtraField5,ExtraField4,ExtraField4Description,ExtraField6,ExtraField6Description, BillMonth ,BillValidStartDate,BillValidEndDate,isMultipleClaimDetails,isPersonsDetailRequired,ClaimAmountField,TotalBillField,IsUpload,IsInactive,ReimbursePolicy,Disclaimer,ExtraField7,ExtraField8,ReimburseApplyNote) values (@Name,@TransactionDateTitle,@TransactionTitle,@NeedExtraDateField,@ExtraDateFieldDescription,@ExtraField1,@ExtraField2,@ExtraField3,@ExtraField5,@ExtraField4,@ExtraField4Description,@ExtraField6,@ExtraField6Description,@BillMonth,@BillValidStartDate,@BillValidEndDate,@isMultipleClaimDetails,@isPersonsDetailRequired,@ClaimAmountField,@TotalBillField,@IsUpload,@IsInactive,@ReimbursePolicy,@Disclaimer,@ExtraField7,@ExtraField8,@ReimburseApplyNote);Select Scope_Identity();"
        Return OldNewConn.ExecuteScalar(CommandType.Text, commendText, SqlParameter)
    End Function

    Public Function UpdateIntoReimburseTypeDetails(ByVal SqlParameter As SqlParameter()) As Integer
        commendText = "update ReimburseMentTypeMaster" & Session("WebTableID") & " set Name=@Name,TransactionDateTitle=@TransactionDateTitle,TransactionTitle=@TransactionTitle,NeedExtraDateField=@NeedExtraDateField,ExtraDateFieldDescription=@ExtraDateFieldDescription,ExtraField1=@ExtraField1,ExtraField2=@ExtraField2,ExtraField3=@ExtraField3,ExtraField5=@ExtraField5,ExtraField4=@ExtraField4,ExtraField4Description=@ExtraField4Description,ExtraField6=@ExtraField6,ExtraField6Description=@ExtraField6Description,BillMonth=@BillMonth,BillValidStartDate=@BillValidStartDate,BillValidEndDate=@BillValidEndDate,isMultipleClaimDetails=@isMultipleClaimDetails,isPersonsDetailRequired=@isPersonsDetailRequired,ClaimAmountField=@ClaimAmountField,TotalBillField=@TotalBillField,IsUpload=@IsUpload,IsInactive=@IsInactive,ReimbursePolicy=@ReimbursePolicy,Disclaimer=@Disclaimer,ExtraField7=@ExtraField7,ExtraField8=@ExtraField8,ReimburseApplyNote=@ReimburseApplyNote where ReimburseType_Code='" & Session("ReimburseType_Code") & "'"
        Return OldNewConn.ExecuteNonQuery(CommandType.Text, commendText, SqlParameter)
    End Function
End Class