Imports System.Web.UI.WebControls
Imports Telerik.Web.UI
Imports DkmOnline.Lib
Imports DkmOnline.Common
Imports System.Data.SqlClient
Imports System.IO


Public Class ReimburseTypeAdmin
    Inherits System.Web.UI.Page

    Dim commendText As String
    Dim constantValuesnew As New ConstantValues

    Dim OldNewConn As OldNewConnection = New OldNewConnection()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsNothing(Session("Emp_Code")) Then
            Response.Redirect("../Logout.aspx")
        End If

        OldNewConn.CheckDatabaseOldNew(Session("WebTableID"))

        DatabaseFacade.Instance.Enter_SiteLog(Session("Emp_Code"), Session("LoginType"), Session("LoginAccessType"), Session("Comp_Code"), 0, Request.Url.AbsolutePath, Request.Url.AbsolutePath, Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type, Session("WebTableID"), Session.SessionID, Session("MenuID"))

        If Not (IsPostBack) Then
            If Not IsNothing(Session("Emp_Code")) And Not IsNothing(Session("WebTableID")) And Not IsNothing(Session("MenuID")) Then
            Else
                Response.Redirect("../Logout.aspx")
            End If
        End If
    End Sub

    Protected Sub RadGrid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles RadGrid1.NeedDataSource
        Dim str As String

        Dim dt1 As New DataTable

        str = "Select  ReimburseType_Code,Name,IsInactive from  ReimburseMentTypeMaster" & Session("WebTableID") & ""

        dt1 = OldNewConn.GetDataTable2(str)

        radGrid1.DataSource = dt1
    End Sub

    Protected Sub radGrid1_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles RadGrid1.ItemDataBound

        Dim griditem As GridDataItem = TryCast(e.Item, GridDataItem)

        If Not IsNothing(griditem) Then
            Try
                Dim image As ImageButton = DirectCast(griditem("Edit").Controls(0), ImageButton)

                If Not IsNothing(image) Then
                    Dim ReimburseType_Code As String = griditem.GetDataKeyValue("ReimburseType_Code")
                    Dim ID As String = griditem.GetDataKeyValue("ReimburseType_Code") & ",119"
                    ID = EncryDecrypt.Encrypt(ID, "EncryptString01")
                    image.Attributes.Add("onclick", String.Format("return openRadWindow1('{0}');", "ReimburseTypeDetail.aspx?ID=" & ID & ""))
                End If
            Catch ex As Exception

            End Try
        End If

        Dim footeritem As GridFooterItem = TryCast(e.Item, GridFooterItem)

        If Not IsNothing(footeritem) Then
            Try
                Dim btnInsert As Button = TryCast(footeritem.FindControl("btnInsert"), Button)
                If Not IsNothing(btnInsert) Then
                    btnInsert.Attributes.Add("onclick", String.Format("return openRadWindow1('{0}');", "ReimburseTypeDetail.aspx"))
                End If
            Catch ex As Exception
            End Try
        End If
    End Sub

    Protected Sub radGrid1_DeleteCommand(sender As Object, e As GridCommandEventArgs) Handles RadGrid1.DeleteCommand
        Dim griditem As GridDataItem = TryCast(e.Item, GridDataItem)

        If Not IsNothing(griditem) Then

            Dim image As ImageButton = DirectCast(griditem("Delete").Controls(0), ImageButton)
            If Not IsNothing(image) Then
                Dim ReimburseType_Code As String = griditem.GetDataKeyValue("ReimburseType_Code")
                commendText = String.Format("delete  from  ReimburseMentTypeMaster{0}  where  ReimburseType_Code='" & ReimburseType_Code & "'", HttpContext.Current.Session("WebTableID"))
                OldNewConn.ExecuteNonQuery(CommandType.Text, commendText, Nothing)
            End If
        End If
    End Sub
End Class