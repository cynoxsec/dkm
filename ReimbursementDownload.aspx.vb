Imports DkmOnline.Lib
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports DkmOnline.Common

Public Class ReimbursementDownload
    Inherits System.Web.UI.Page

    Dim ClaimId As Integer = 0
    Dim ReimbursementType_Code As Integer = 0

    Dim OldNewConn As OldNewConnection = New OldNewConnection()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If IsNothing(Session("Emp_Code")) Then
            Response.Redirect("../Logout.aspx")
        End If

        DatabaseFacade.Instance.Enter_SiteLog(Session("Emp_Code"), Session("LoginType"), Session("LoginAccessType"), Session("Comp_Code"), 0, Request.Url.AbsolutePath, Request.Url.AbsolutePath, Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type, Session("WebTableID"), Session.SessionID, Session("MenuID"))

        OldNewConn.CheckDatabaseOldNew(Session("WebTableID"))

        If Not IsNothing(Request.QueryString("PersonsDetailID")) Then
            ClaimId = Request.QueryString("PersonsDetailID")
        End If

        If Not IsNothing(Request.QueryString("Type_Code")) Then
            ReimbursementType_Code = Request.QueryString("Type_Code")
        End If

        Dim str As String = ""

        Dim extension As String = ".pdf"

        Dim dt As DataTable

        str = "Select fileData from dbo.OtherMultipleReimburseClaimDetails" & Session("WebTableID") & " where MultipleClaimDetailsID=" & ClaimId & ""

        dt = OldNewConn.GetDataTable2(str)

        If (dt.Rows.Count > 0) Then

            Dim g As Guid = Guid.NewGuid()

            Dim ToSaveFileTo As String = Server.MapPath("~\\OutputFiles\\" & g.ToString & "" & extension & "")

            Dim sr As New StreamWriter(ToSaveFileTo)
            sr.Write("hello")
            sr.Close()

            If Not IsDBNull(dt.Rows(0)("fileData")) Then
                Dim fileData As Byte() = DirectCast(dt.Rows(0)("fileData"), Byte())
                Using fs As New System.IO.FileStream(ToSaveFileTo, System.IO.FileMode.Create, System.IO.FileAccess.Write)

                    Using bw As New System.IO.BinaryWriter(fs)
                        bw.Write(fileData)
                        bw.Close()
                    End Using
                End Using
                download_File(ToSaveFileTo, extension)
            End If
        End If

    End Sub

    Private Sub download_File(fileName As String, Optional ContentType As String = "") '"application/pdf")

        Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & "" & ContentType & "")
        Response.ContentType = ContentType
        Response.WriteFile(fileName)
        Response.Flush()
        Response.End()

    End Sub
End Class