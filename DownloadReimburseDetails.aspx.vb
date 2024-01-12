Imports System.Data.SqlClient
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports System.IO
Imports DkmOnline.Lib
Imports DataAccessLayer
Imports DkmOnline.Common
Imports ClosedXML.Excel

Public Class DownloadReimburseDetails
    Inherits System.Web.UI.Page


    Dim databasename As String = String.Empty
    Dim methodobj As pdfMethodClass = New pdfMethodClass
    Dim OldNewConn As OldNewConnection = New OldNewConnection()
    Dim dtOption As New DataTable
    Dim dtActReim As New DataTable
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If IsNothing(Session("Emp_Code")) Then
            Response.Redirect("../Logout.aspx")
        End If

        DatabaseFacade.Instance.Enter_SiteLog(Session("Emp_Code"), Session("LoginType"), Session("LoginAccessType"), Session("Comp_Code"), 0, Request.Url.AbsolutePath, Request.Url.AbsolutePath, Request.ServerVariables("REMOTE_ADDR"), Request.Browser.Type, Session("WebTableID"), Session.SessionID, Session("MenuID"))
        If Session("DKMWebSite") = "DKM1968" Then
            databasename = "Webdatabasedkmonline"
        Else
            databasename = methodobj.GetDatabaseName()
        End If
        OldNewConn.CheckDatabaseOldNew(Session("WebTableID"))

        dtOption = OldNewConn.GetDataTable2("Select Print_Name from PayslipSetup" & Session("WebTableID") & " where Name ='ReimbursementOption'")
        dtActReim = OldNewConn.GetDataTable2("Select Print_Name from PayslipSetup" & Session("WebTableID") & " where Name ='ActivateReimbursementDeclaration'")
        Session("SelectedMonthName") = DateTime.Now.ToString("MMMM")

        Dim sql_Query As String = String.Format("Select (E.FirstName +' ' +E.LastName) as Name,E.Email  from Employeesmaster{0} E  where E.Emp_Code='" & Session("Emp_Code") & "'", Session("WebTableID"))

        Dim dtPayslipSetup As DataTable = OldNewConn.GetDataTable2(sql_Query)

        If (dtPayslipSetup.Rows.Count > 0) Then
            Session("Emp_Name") = dtPayslipSetup.Rows(0)("Name")
            Session("Email") = dtPayslipSetup.Rows(0)("Email")
        End If

        Dim dtPayslipSetup1 As DataTable = DatabaseFacade.Instance.GetGivenTable("Select WebMastercode,Comp_Name from DkmCompanyMaster where Webtableid=" & Session("WebTableID"))

        If (dtPayslipSetup1.Rows.Count > 0) Then
            Session("Comp_Code") = EncryDecrypt.DecryptNew(dtPayslipSetup1.Rows(0)("WebMastercode"), "Klqoqwqj")
            Session("Comp_Name") = EncryDecrypt.DecryptNew(dtPayslipSetup1.Rows(0)("Comp_Name"), "OP2kiwqj")
        End If

        Dim dtPayslipSetup5 As DataTable = DatabaseFacade.Instance.GetGivenTable("select showlogo from companymanagement where webtableid=" & Session("WebTableID"))

        If (dtPayslipSetup5.Rows.Count > 0) Then
            Session("ShowLogo") = dtPayslipSetup5.Rows(0)("ShowLogo")
        End If

        If Not IsNothing(Request.QueryString("Year")) Then
            Dim date1 As New Date(Request.QueryString("Year"), Request.QueryString("Month"), 1)
            Session("SelectedMonthName") = date1.ToString("MMMM")
            Reimbursement_BillDetails(Request.QueryString("Month"), Request.QueryString("Year"))
        ElseIf Not IsNothing(Request.QueryString("ClaimReport")) Then
        Else
            Reimbursement_BillDetails(DateTime.Now.Month, DateTime.Now.Year)
        End If
        If (dtOption.Rows.Count > 0) Then
            If (dtOption.Rows(0)("Print_Name").ToString.ToLower = "withoutbudget") Then
                If dtActReim.Rows.Count > 0 Then
                    If dtActReim.Rows(0)("print_Name").ToString.ToUpper = "Y" Or dtActReim.Rows(0)("print_Name").ToString.ToUpper = "YES" Then
                        Session("Reimbursementdeclaration") = "Yes"

                    End If
                End If
            End If
        End If
         If Not (IsPostBack) Then

            Dim dt As DataTable = OldNewConn.GetDataTable2("select distinct right(table_name,4) as finyear from INFORMATION_SCHEMA.columns where table_name like 'Reimbursementdeclaration%' and table_name like '%" & Session("WebTableID") & "%'")

            If dt.Rows.Count > 0 Then
                ddlyear.Items.Clear()
                ddlyear.DataSource = dt
                ddlyear.DataTextField = "finyear"
                ddlyear.DataValueField = "finyear"
                ddlyear.DataBind()
                ddlyear.Items.Insert(0, New System.Web.UI.WebControls.ListItem("Please Select", "0"))
            End If
            If IsNothing(Session("Reimbursementdeclaration")) Then
                declarediv.Style.Add("display", "none")
            Else
                declarediv.Style.Add("display", "block")
            End If

        End If


    End Sub

    Private Sub Reimbursement_BillDetails(Bill_Month As Integer, Bill_Year As Integer)

        Dim extraFileName As String = Session("Emp_Code").ToString.Replace("\", "_").Replace("/", "_")
        extraFileName = extraFileName & "wl" & (Now.TimeOfDay.Hours * 42) & "-" & CInt(Now.TimeOfDay.Minutes + 4) & "-" & CInt(Now.TimeOfDay.Seconds + 4) & "-" & Today.Date.Day & "-" & Format(Today.Date.Date, "ddmmyy")

        If Not (Directory.Exists(Server.MapPath("/OutPutFiles/" & Session("Comp_Code")))) Then
            Directory.CreateDirectory(Server.MapPath("/OutPutFiles/" & Session("Comp_Code")))
        End If

        Dim FileName As String = Request.PhysicalApplicationPath & "\OutPutFiles\" & Session("Comp_Code") & "\N42" & Session("WebTableID") & "12ReimClaimForm_" & extraFileName & ".pdf"

        Dim doc As New iTextSharp.text.Document

        Dim writer As PdfWriter = PdfWriter.GetInstance(doc, New FileStream(FileName, FileMode.CreateNew))
        doc.NewPage()
        Dim table As PdfPTable

        Try
            Dim columnsCount As Integer = 13
            Dim Gray As New iTextSharp.text.BaseColor(Drawing.Color.Gray)
            Dim largerFont As iTextSharp.text.Font
            largerFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 11, iTextSharp.text.Font.NORMAL, BaseColor.WHITE)
            Dim largerFontBlack As iTextSharp.text.Font
            largerFontBlack = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 20, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)

            Dim largerFont_Small As iTextSharp.text.Font
            largerFont_Small = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.WHITE)
            Dim largerFont_Small_Black As iTextSharp.text.Font
            largerFont_Small_Black = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
            Dim largerFont_Small_White As iTextSharp.text.Font
            largerFont_Small_White = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.WHITE)

            Dim normalItalicFont As iTextSharp.text.Font
            normalItalicFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.ITALIC, BaseColor.BLACK)

            Dim normalFontRedColor As iTextSharp.text.Font
            normalFontRedColor = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.Font.NORMAL, BaseColor.RED)

            Dim normalItalicFontGreenColor As iTextSharp.text.Font
            normalItalicFontGreenColor = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 20, iTextSharp.text.Font.NORMAL, BaseColor.GREEN)

            Dim largeFont As iTextSharp.text.Font
            largeFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.DARK_GRAY)

            Dim normalFont As iTextSharp.text.Font
            normalFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)

            Dim smallFont As iTextSharp.text.Font
            smallFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)

            Dim normalFontBold As iTextSharp.text.Font
            normalFontBold = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK)

            Dim normalFontBoldblack As iTextSharp.text.Font
            normalFontBoldblack = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 11, iTextSharp.text.Font.BOLD, BaseColor.BLACK)

            Dim normalboldunserline As iTextSharp.text.Font
            normalboldunserline = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.UNDERLINE, BaseColor.BLACK)

            table = New PdfPTable(columnsCount)
            Dim header As PdfPCell
            Dim blankheader As PdfPCell
            blankheader = New PdfPCell(New Phrase(" ", normalFont))
            blankheader.HorizontalAlignment = Element.ALIGN_LEFT
            blankheader.Border = 0
            blankheader.Colspan = columnsCount

            table.WidthPercentage = 90

            Dim intialquery As String

            Dim Imagefile As String

            Dim image3 As iTextSharp.text.Image

            doc.Open()

            Dim LogoWidth2 As Double
            Dim LogoHeight As Double
            Dim LogoX As Double
            Dim LogoY As Double

            LogoWidth2 = 100
            LogoHeight = 60

            table.AddCell(blankheader)
            table.AddCell(blankheader)


            Dim dsReimburseType As New DataSet
            Dim dtReimburseType As DataTable
            Dim sql_Query As String = ""
            Dim PageNumber As Integer = 0
            Dim dsReimburseDetails As New DataTable
            Dim actualTotalColumns As Integer = 4


            sql_Query = "select RTM.*,ISNULL(PSS.Print_Name,RTM.Name) as 'Print_Name' from ReimburseMentTypeMaster" & Session("WebTableID") & " RTM Left join PayslipSetup" & Session("WebTableID") & " PSS on RTM.Name=PSS.Name where reimburseType_Code in (select distinct(ReimburseType_Code) from reimbursementdetails" & Session("WebTableID") & "   where emp_Code = '" & Session("Emp_Code") & "' and Month(EntryDate) = " & Bill_Month & " and Year(EntryDate) = " & Bill_Year & ")  Order by Print_Name"

            dtReimburseType = OldNewConn.GetDataTable2(sql_Query)

            For Each dr In dtReimburseType.Rows

                Dim isMultipleClaimDetails As String = dr.Item("isMultipleClaimDetails")
                Dim isPersonsDetailRequired As String = dr.Item("isPersonsDetailRequired")
                Dim isUploadRequired As String = dr.Item("IsUpload")
                Dim Disclaimer As String = dr.Item("Disclaimer")
                Dim ReimbursementName As String = dr.Item("Name")

                Dim NextValue As Integer
                If Not (PageNumber = 0) Then
                    doc.NewPage()
                    table = New PdfPTable(columnsCount)
                End If

                header = New PdfPCell(New Phrase("Company Name : " & Session("Comp_Name"), normalFontBold))
                header.Colspan = 10
                header.Border = 0
                table.AddCell(header)

                header = New PdfPCell(New Phrase("Date : " & Format(Today, "dd-MMM-yyyy"), normalFontBold))
                header.Colspan = columnsCount - 3
                header.HorizontalAlignment = Element.ALIGN_RIGHT
                header.Border = 0
                table.AddCell(header)

                table.AddCell(blankheader)
                table.AddCell(blankheader)
                table.AddCell(blankheader)

                If (Session("WebtableID") = 977) Then
                    header = New PdfPCell(New Phrase(StrConv("Reimbursement claim form along with Form 12BB (on next pages) for the month of " & Session("SelectedMonthName") & " - " & Bill_Year, VbStrConv.Uppercase), normalFontBold))
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    header.Border = 0
                    header.Colspan = columnsCount
                    table.AddCell(header)
                Else
                    header = New PdfPCell(New Phrase(StrConv("Reimbursement claim for the month of " & Session("SelectedMonthName") & " - " & Bill_Year, VbStrConv.Uppercase), normalFontBold))
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    header.Border = 0
                    header.Colspan = columnsCount
                    table.AddCell(header)
                End If


                table.AddCell(blankheader)

                header = New PdfPCell(New Phrase(StrConv(dr.Item("Print_Name"), VbStrConv.ProperCase), normalFontBoldblack))
                header.Border = 0
                header.HorizontalAlignment = Element.ALIGN_CENTER
                header.Colspan = columnsCount
                table.AddCell(header)
                table.AddCell(blankheader)

                header = New PdfPCell(New Phrase("Code : ", normalFontBold))
                header.Colspan = 2
                table.AddCell(header)

                header = New PdfPCell(New Phrase(StrConv(Session("Emp_Code"), VbStrConv.Uppercase), normalFontBold))
                header.Colspan = 4
                table.AddCell(header)

                header = New PdfPCell(New Phrase("Name : ", normalFontBold))
                header.Colspan = 2
                table.AddCell(header)

                header = New PdfPCell(New Phrase(StrConv(Session("Emp_Name"), VbStrConv.ProperCase).ToString, normalFontBold))
                header.Colspan = 5
                table.AddCell(header)

                table.AddCell(blankheader)
                table.AddCell(blankheader)

                Dim FromDate As Date
                Dim ToDate As Date

                If (Bill_Month <= 3) Then
                    FromDate = New Date(Bill_Year - 1, 4, 1)
                    ToDate = New Date(Bill_Year, 3, 31)
                Else
                    FromDate = New Date(Bill_Year, 4, 1)
                    ToDate = New Date(Bill_Year + 1, 3, 31)
                End If

                sql_Query = "select * from reimbursementdetails" & Session("WebTableID") & "  where emp_Code = '" & Session("Emp_Code") & "' and Month(EntryDate) = " & Bill_Month & " and Year(EntryDate) = " & Bill_Year & " and reimburseType_Code = " & dr.Item("reimburseType_Code") & " order by ReimburseDetailsID Asc"

                dsReimburseDetails = OldNewConn.GetDataTable2(sql_Query)

                Dim Sno As Integer = 0
                Dim r As DataRow
                Dim totalBillAmount As Double = 0
                Dim Total_BillField As Double = 0

                Dim currentCol As Integer = 0
                Dim currentClaim As Integer = 0


                For Each r In dsReimburseDetails.Rows
                    Dim ReimburseDetailsID As Integer = 0
                    ReimburseDetailsID = r.Item("ReimburseDetailsID")
                    ViewState("ReimburseDetailsID") = r.Item("ReimburseDetailsID")
                    currentCol = currentCol + 1
                    currentClaim = currentClaim + 1

                    If isPersonsDetailRequired = True Then
                        header = New PdfPCell(New Phrase(currentClaim & ". Claim Dated : " & Format(r.Item("EntryDate"), "dd-MMM-yyyy"), normalFontBold))
                        header.Colspan = columnsCount
                        header.Border = 0
                        table.AddCell(header)
                    Else
                        header = New PdfPCell(New Phrase(currentClaim & ". Claim Dated : " & Format(r.Item("TransactionDate"), "dd-MMM-yyyy"), normalFontBold))
                        header.Colspan = columnsCount
                        header.Border = 0
                        table.AddCell(header)
                    End If

                    table.AddCell(blankheader)

                    If (isMultipleClaimDetails = False And isPersonsDetailRequired = False) Then
                        header = New PdfPCell(New Phrase(Format(dr.Item("ClaimAmountField")), normalFontBold))
                        header.Colspan = 10
                        table.AddCell(header)
                        header = New PdfPCell(New Phrase(Format(r.Item("ClaimAmount"), "####0.00"), normalFontBold))
                        header.Colspan = 3
                        header.HorizontalAlignment = Element.ALIGN_RIGHT
                        table.AddCell(header)
                    End If

                    sql_Query = "select top 2 * from LTAChecking" & Session("WebTableID") & " where emp_code = '" & Session("Emp_Code").ToString() & "' order by createddate"

                    Dim reimTrans As New DataTable

                    reimTrans = OldNewConn.GetDataTable2(sql_Query)

                    If isPersonsDetailRequired = True Then
                        If (reimTrans.Rows.Count > 0) Then
                            If (reimTrans.Rows(1)("FirstBlockYear").ToString() <> "") Then
                                header = New PdfPCell(New Phrase("Number of tax free LTA claims in current Block Year (January " & reimTrans.Rows(1)("FirstBlockYear") & " – December " & reimTrans.Rows(1)("LastBlockYear") & ") :", normalFontBold))
                                header.Colspan = 12
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase(reimTrans.Rows(1)("NoOfClaims"), normalFont))
                                header.Colspan = 1
                                header.HorizontalAlignment = Element.ALIGN_RIGHT
                                table.AddCell(header)
                            End If

                            If (reimTrans.Rows(0)("FirstBlockYear").ToString() <> "") Then
                                header = New PdfPCell(New Phrase("Number of tax free LTA claims in last Block Year (January " & reimTrans.Rows(0)("FirstBlockYear") & " – December " & reimTrans.Rows(0)("LastBlockYear") & ") :", normalFontBold))
                                header.Colspan = 12
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase(reimTrans.Rows(0)("NoOfClaims"), normalFont))
                                header.Colspan = 1
                                header.HorizontalAlignment = Element.ALIGN_RIGHT
                                table.AddCell(header)
                            End If
                        End If

                        If (dr.Item("TransactionDateTitle") <> "") Then
                            header = New PdfPCell(New Phrase(dr.Item("TransactionDateTitle"), normalFontBold))
                            header.Colspan = 3
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(Format(r.Item("TransactionDate"), "dd-MMM-yyyy"), normalFont))
                            header.Colspan = 4
                            table.AddCell(header)
                        End If

                        If (dr.Item("ExtraDateFieldDescription") <> "") Then
                            header = New PdfPCell(New Phrase(dr.Item("ExtraDateFieldDescription"), normalFontBold))
                            header.Colspan = 3
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(Format(r.Item("ExtraDateField"), "dd-MMM-yyyy"), normalFont))
                            header.Colspan = 4
                            table.AddCell(header)
                        End If

                        If (dr.Item("ExtraField7") <> "") Then
                            header = New PdfPCell(New Phrase(dr.Item("ExtraField7"), normalFontBold))
                            header.Colspan = 3
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(Format(r.Item("ExtraField7"), "dd-MMM-yyyy"), normalFont))
                            header.Colspan = 4
                            table.AddCell(header)
                        End If

                        If (dr.Item("ExtraField8") <> "") Then
                            header = New PdfPCell(New Phrase(dr.Item("ExtraField8"), normalFontBold))
                            header.Colspan = 3
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(Format(r.Item("ExtraField8"), "dd-MMM-yyyy"), normalFont))
                            header.Colspan = 4
                            table.AddCell(header)
                        End If
                    End If

                    If currentCol = 1 Then

                        If (dr.Item("ExtraField1") <> "") Then
                            header = New PdfPCell(New Phrase(dr.Item("ExtraField1"), normalFontBold))
                            header.Colspan = 3
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(r.Item("ExtraField1"), normalFont))
                            header.Colspan = 4
                            table.AddCell(header)
                        End If

                        If (dr.Item("ExtraField2") <> "") Then
                            header = New PdfPCell(New Phrase(dr.Item("ExtraField2"), normalFontBold))
                            header.Colspan = 3
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(r.Item("ExtraField2"), normalFont))
                            header.Colspan = 3
                            table.AddCell(header)
                        End If

                        If (dr.Item("ExtraField3") <> "") Then
                            header = New PdfPCell(New Phrase(dr.Item("ExtraField3"), normalFontBold))
                            header.Colspan = 3
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(r.Item("ExtraField3"), normalFont))
                            header.Colspan = 4
                            table.AddCell(header)
                        End If

                        If (dr.Item("ExtraField5") <> "") Then
                            header = New PdfPCell(New Phrase(dr.Item("ExtraField5"), normalFontBold))
                            header.Colspan = 3
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(r.Item("ExtraField5"), normalFont))
                            header.Colspan = 3
                            table.AddCell(header)
                        End If

                        If (dr.Item("ExtraField4") <> "") And (dr.Item("ExtraField4Description") <> "") Then
                            header = New PdfPCell(New Phrase(dr.Item("ExtraField4Description"), normalFontBold))
                            header.Colspan = 3
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(r.Item("ExtraField4"), normalFont))
                            header.Colspan = 4
                            table.AddCell(header)
                        ElseIf (dr.Item("ExtraField4") <> "") Then
                            header = New PdfPCell(New Phrase(dr.Item("ExtraField4"), normalFontBold))
                            header.Colspan = 3
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(r.Item("ExtraField4"), normalFont))
                            header.Colspan = 4
                            table.AddCell(header)
                        End If

                        If (dr.Item("ExtraField6") <> "") And (dr.Item("ExtraField6Description") <> "") Then
                            header = New PdfPCell(New Phrase(dr.Item("ExtraField6Description"), normalFontBold))
                            header.Colspan = 3
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(r.Item("ExtraField6"), normalFont))
                            header.Colspan = 3
                            table.AddCell(header)
                        ElseIf (dr.Item("ExtraField6") <> "") Then
                            header = New PdfPCell(New Phrase(dr.Item("ExtraField6"), normalFontBold))
                            header.Colspan = 3
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(r.Item("ExtraField6"), normalFont))
                            header.Colspan = 3
                            table.AddCell(header)
                        End If

                        table.AddCell(blankheader)
                    End If

                    If isMultipleClaimDetails = True Then
                        sql_Query = "select BillNumber,BillDated,BillAmount,BillDetail from OtherMultipleReimburseClaimDetails" & Session("WebTableID") & "  where ReimburseDetailsID = " & ReimburseDetailsID & " order by ReimburseDetailsId"

                        dsReimburseDetails = OldNewConn.GetDataTable2(sql_Query)


                        Dim row As DataRow
                        Dim SerialNo As Integer = 0

                        For Each row In dsReimburseDetails.Rows
                            SerialNo = SerialNo + 1
                            Dim ColAdjustment As Integer = 0
                            If SerialNo = 1 Then
                                header = New PdfPCell(New Phrase("Bill detail SNo.", normalFont))
                                header.Colspan = 2
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("Bill number", normalFont))
                                header.Colspan = 2
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("Bill dated", normalFont))
                                header.Colspan = 2
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("Bill amount", normalFont))
                                header.Colspan = 2
                                header.HorizontalAlignment = Element.ALIGN_RIGHT
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("Bill detail", normalFont))
                                header.Colspan = 5
                                table.AddCell(header)
                            End If

                            header = New PdfPCell(New Phrase(SerialNo, normalFont))
                            header.Colspan = 2
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(row.Item("Billnumber"), normalFont))
                            header.Colspan = 2
                            table.AddCell(header)

                            If IsDBNull(row.Item("BillDated")) = False Then
                                header = New PdfPCell(New Phrase(Format(row.Item("Billdated"), "dd-MMM-yyyy"), normalFont))
                            Else
                                header = New PdfPCell(New Phrase("Bill dated", normalFont))
                            End If
                            header.Colspan = 2
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(Format(row.Item("Billamount"), "####0.00"), normalFont))
                            header.Colspan = 2
                            header.HorizontalAlignment = Element.ALIGN_RIGHT
                            table.AddCell(header)

                            totalBillAmount = totalBillAmount + CInt(Format(row.Item("Billamount"), "####0.00"))

                            header = New PdfPCell(New Phrase(row.Item("Billdetail"), normalFont))
                            header.Colspan = 5
                            table.AddCell(header)
                        Next
                        header = New PdfPCell(New Phrase("", normalFont))
                        header.Colspan = columnsCount
                        table.AddCell(header)

                        header = New PdfPCell(New Phrase("Total", normalFontBold))
                        header.Colspan = 2
                        table.AddCell(header)

                        header = New PdfPCell(New Phrase("", normalFont))
                        header.Colspan = 2
                        table.AddCell(header)

                        header = New PdfPCell(New Phrase("", normalFont))
                        header.Colspan = 2
                        table.AddCell(header)

                        header = New PdfPCell(New Phrase(Format(totalBillAmount, "####0.00"), normalFontBold))
                        header.Colspan = 2
                        header.HorizontalAlignment = Element.ALIGN_RIGHT
                        table.AddCell(header)

                        header = New PdfPCell(New Phrase("", normalFont))
                        header.Colspan = 5
                        table.AddCell(header)
                        table.AddCell(blankheader)
                    End If

                    If isPersonsDetailRequired = True And dr.Item("Name").ToString.ToLower <> "deemed lta" Then

                        If (dr.Item("TotalBillField") <> "") Then
                            header = New PdfPCell(New Phrase(dr.Item("TotalBillField"), normalFontBold))
                            header.Colspan = 9
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(Format(r.Item("TotalBillField"), "####0"), normalFont))
                            header.Colspan = 4
                            table.AddCell(header)
                        End If

                        dsReimburseDetails = New DataTable

                        sql_Query = "select PersonsName,Dependence,RelationshipWithEmployee from OtherReimbursementPersonsDetail" & Session("WebTableID") & "  where ReimburseDetailsID = " & ReimburseDetailsID & " order by ReimbursementPersonsDetailID"


                        dsReimburseDetails = OldNewConn.GetDataTable2(sql_Query)


                        Dim SerialNo As Integer = 0

                        Dim row As DataRow
                        For Each row In dsReimburseDetails.Rows
                            SerialNo = SerialNo + 1
                            If SerialNo = 1 Then
                                header = New PdfPCell(New Phrase("S No.", normalFont))
                                header.Colspan = 2
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("Name of the Member Travelled", normalFont))
                                header.Colspan = 4
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("Relationship with employee", normalFont))
                                header.Colspan = 4
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("Dependence (Y/N)", normalFont))
                                header.Colspan = 3
                                table.AddCell(header)
                            End If

                            header = New PdfPCell(New Phrase(SerialNo, normalFont))
                            header.Colspan = 2
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(row.Item("PersonsName"), normalFont))
                            header.Colspan = 4
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(row.Item("RelationshipWithEmployee"), normalFont))
                            header.Colspan = 4
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(row.Item("Dependence"), normalFont))
                            header.Colspan = 3
                            table.AddCell(header)
                        Next
                        header = New PdfPCell(New Phrase("", normalFont))
                        header.Colspan = columnsCount
                        table.AddCell(header)
                    End If

                    table.AddCell(blankheader)

                    If isPersonsDetailRequired = True And dr.Item("Name").ToString.ToLower <> "deemed lta" Then

                        header = New PdfPCell(New Phrase("Detail of Travel", normalFontBold))
                        header.Colspan = columnsCount
                        header.HorizontalAlignment = Element.ALIGN_CENTER
                        table.AddCell(header)

                        dsReimburseDetails = New DataTable

                        sql_Query = "select PersonsName,Dependence,RelationshipWithEmployee,[From],[To],JourneyDate,ModeOfJourney,TypeOfJourney,ClaimAmount from OtherReimbursementPersonsDetail" & Session("WebTableID") & "  where ReimburseDetailsID = " & ReimburseDetailsID & " order by ReimbursementPersonsDetailID"

                        dsReimburseDetails = OldNewConn.GetDataTable2(sql_Query)

                        Dim SerialNo As Integer = 0

                        Dim row As DataRow
                        For Each row In dsReimburseDetails.Rows
                            SerialNo = SerialNo + 1
                            If SerialNo = 1 Then

                                header = New PdfPCell(New Phrase("Name of the Member Travelled", normalFont))
                                header.Colspan = 2
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("Mode of journey", normalFont))
                                header.Colspan = 2
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("Type of journey", normalFont))
                                header.Colspan = 2
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("Journey From", normalFont))
                                header.Colspan = 2
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("Journey To", normalFont))
                                header.Colspan = 2
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("Date of journey", normalFont))
                                header.Colspan = 2
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("Journey Fare (in INR)", normalFont))
                                header.Colspan = 1
                                header.HorizontalAlignment = Element.ALIGN_RIGHT
                                table.AddCell(header)
                            End If

                            header = New PdfPCell(New Phrase(row.Item("PersonsName"), normalFont))
                            header.Colspan = 2
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(row.Item("ModeOfJourney"), normalFont))
                            header.Colspan = 2
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(row.Item("TypeOfJourney"), normalFont))
                            header.Colspan = 2
                            table.AddCell(header)


                            If IsDBNull(row.Item("From")) = False Then
                                header = New PdfPCell(New Phrase(row.Item("From"), normalFont))
                            Else
                                header = New PdfPCell(New Phrase("", normalFont))
                            End If
                            header.Colspan = 2
                            table.AddCell(header)

                            If IsDBNull(row.Item("To")) = False Then
                                header = New PdfPCell(New Phrase(row.Item("To"), normalFont))
                            Else
                                header = New PdfPCell(New Phrase("", normalFont))
                            End If
                            header.Colspan = 2
                            table.AddCell(header)

                            If IsDBNull(row.Item("JourneyDate")) = False Then
                                header = New PdfPCell(New Phrase(Format(row.Item("JourneyDate"), "dd-MMM-yyyy"), normalFont))
                            Else
                                header = New PdfPCell(New Phrase("JourneyDate", normalFont))
                            End If
                            header.Colspan = 2
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(Format(row.Item("ClaimAmount"), "####0"), normalFont))
                            header.HorizontalAlignment = Element.ALIGN_RIGHT
                            header.Colspan = 1
                            table.AddCell(header)
                        Next
                        header = New PdfPCell(New Phrase("", normalFont))
                        header.Colspan = columnsCount
                        table.AddCell(header)

                        header = New PdfPCell(New Phrase(Format(dr.Item("ClaimAmountField")), normalFontBold))
                        header.Colspan = 10
                        table.AddCell(header)
                        header = New PdfPCell(New Phrase(Format(r.Item("ClaimAmount"), "####0.00"), normalFontBold))
                        header.Colspan = 3
                        header.HorizontalAlignment = Element.ALIGN_RIGHT
                        table.AddCell(header)

                        Total_BillField += CDec(r.Item("ClaimAmount"))

                    End If

                    If isPersonsDetailRequired = True And dr.Item("Name").ToString.ToLower = "deemed lta" Then

                        If (dr.Item("TotalBillField") <> "") Then
                            header = New PdfPCell(New Phrase(dr.Item("TotalBillField"), normalFontBold))
                            header.Colspan = 9
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(Format(r.Item("TotalBillField"), "####0"), normalFont))
                            header.Colspan = 4
                            table.AddCell(header)
                        End If

                        dsReimburseDetails = New DataTable

                        sql_Query = "select PersonsName,Dependence,RelationshipWithEmployee from OtherReimbursementPersonsDetailDeemedLTA" & Session("WebTableID") & "  where ReimburseDetailsID = " & ReimburseDetailsID & " order by ReimbursementPersonsDetailID"

                        dsReimburseDetails = OldNewConn.GetDataTable2(sql_Query)


                        Dim SerialNo As Integer = 0

                        Dim row As DataRow
                        For Each row In dsReimburseDetails.Rows
                            SerialNo = SerialNo + 1
                            If SerialNo = 1 Then
                                header = New PdfPCell(New Phrase("S No.", normalFont))
                                header.Colspan = 2
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("Name Of Member", normalFont))
                                header.Colspan = 4
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("Relationship with employee", normalFont))
                                header.Colspan = 4
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("Dependence (Y/N)", normalFont))
                                header.Colspan = 3
                                table.AddCell(header)
                            End If

                            header = New PdfPCell(New Phrase(SerialNo, normalFont))
                            header.Colspan = 2
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(row.Item("PersonsName"), normalFont))
                            header.Colspan = 4
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(row.Item("RelationshipWithEmployee"), normalFont))
                            header.Colspan = 4
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(row.Item("Dependence"), normalFont))
                            header.Colspan = 3
                            table.AddCell(header)
                        Next
                        header = New PdfPCell(New Phrase("", normalFont))
                        header.Colspan = columnsCount
                        table.AddCell(header)
                    End If

                    table.AddCell(blankheader)

                    If isPersonsDetailRequired = True And dr.Item("Name").ToString.ToLower = "deemed lta" Then

                        header = New PdfPCell(New Phrase("Detail of deemed LTA", normalFontBold))
                        header.Colspan = columnsCount
                        header.HorizontalAlignment = Element.ALIGN_CENTER
                        table.AddCell(header)

                        dsReimburseDetails = New DataTable

                        sql_Query = "select PersonsName,BillNumber,BillDated,GSTNumber,GSTRate,BillAmount,Paymentmode,BillIssueTo from OtherReimbursementPersonsDetailDeemedLTA" & Session("WebTableID") & "  where ReimburseDetailsID = " & ReimburseDetailsID & " order by ReimbursementPersonsDetailID"

                        dsReimburseDetails = OldNewConn.GetDataTable2(sql_Query)

                        Dim SerialNo As Integer = 0

                        Dim row As DataRow
                        For Each row In dsReimburseDetails.Rows
                            SerialNo = SerialNo + 1
                            If SerialNo = 1 Then

                                header = New PdfPCell(New Phrase("Name Of Member", normalFont))
                                header.Colspan = 2
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("Bill Number", normalFont))
                                header.Colspan = 2
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("Bill Date", normalFont))
                                header.Colspan = 2
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("GSTNumber", normalFont))
                                header.Colspan = 2
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("GSTRate", normalFont))
                                header.Colspan = 1
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("BillAmount", normalFont))
                                header.Colspan = 1
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("Paymentmode", normalFont))
                                header.Colspan = 1
                                table.AddCell(header)

                                header = New PdfPCell(New Phrase("BillIssueTo", normalFont))
                                header.Colspan = 2
                                table.AddCell(header)

                            End If

                            header = New PdfPCell(New Phrase(row.Item("PersonsName"), normalFont))
                            header.Colspan = 2
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(row.Item("BillNumber"), normalFont))
                            header.Colspan = 2
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(Format(row.Item("BillDated"), "dd-MMM-yyyy"), normalFont))
                            header.Colspan = 2
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(row.Item("GSTNumber"), normalFont))
                            header.Colspan = 2
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(Format(row.Item("GSTRate"), "####0"), normalFont))
                            header.Colspan = 1
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(Format(row.Item("BillAmount"), "####0"), normalFont))
                            header.HorizontalAlignment = Element.ALIGN_RIGHT
                            header.Colspan = 1
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(row.Item("Paymentmode"), normalFont))
                            header.Colspan = 1
                            table.AddCell(header)

                            header = New PdfPCell(New Phrase(row.Item("BillIssueTo"), normalFont))
                            header.Colspan = 2
                            table.AddCell(header)
                        Next
                        header = New PdfPCell(New Phrase("", normalFont))
                        header.Colspan = columnsCount
                        table.AddCell(header)

                        header = New PdfPCell(New Phrase(Format(dr.Item("ClaimAmountField")), normalFontBold))
                        header.Colspan = 10
                        table.AddCell(header)
                        header = New PdfPCell(New Phrase(Format(r.Item("ClaimAmount"), "####0.00"), normalFontBold))
                        header.Colspan = 3
                        header.HorizontalAlignment = Element.ALIGN_RIGHT
                        table.AddCell(header)

                        Total_BillField += CDec(r.Item("ClaimAmount"))

                    End If

                    currentCol = 0
                    totalBillAmount = 0
                Next

                table.AddCell(blankheader)

                If isPersonsDetailRequired = True Then
                    header = New PdfPCell(New Phrase("Hereby I do apply for Tax-free Leave Travel Assistance for the expenses incurred by me, amounting INR " & Format(Total_BillField, "####0.00") & " and annexing required documents for this purpose, detail of which is given above.", normalFont))
                    header.Colspan = 13
                    header.Border = 0
                    table.AddCell(header)
                End If

                table.AddCell(blankheader)

                header = New PdfPCell(New Phrase(Disclaimer, normalItalicFont))
                header.Border = 0
                header.Colspan = columnsCount
                table.AddCell(header)

                table.AddCell(blankheader)
                table.AddCell(blankheader)
                table.AddCell(blankheader)

                If isUploadRequired = True Then
                    If (Session("WebtableID") = 977) Then
                        header = New PdfPCell(New Phrase("Date : ", normalFont))
                        header.Colspan = 5
                        header.Border = 0
                        table.AddCell(header)

                        header = New PdfPCell(New Phrase("Signature :", normalFont))
                        header.Colspan = columnsCount - 5
                        header.Border = 0
                        header.HorizontalAlignment = Element.ALIGN_CENTER
                        table.AddCell(header)
                    End If
                Else
                    header = New PdfPCell(New Phrase("Date : ", normalFont))
                    header.Colspan = 5
                    header.Border = 0
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("Signature :", normalFont))
                    header.Colspan = columnsCount - 5
                    header.Border = 0
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)
                End If

                doc.Add(table)
                writer.PageEmpty = False
                table = Nothing
                PageNumber = PageNumber + 1

                If isPersonsDetailRequired = True Then

                    sql_Query = "Select F.FirstName + ' ' +f.lastname as FirstName, F.FatherName ,F.PANNo, D.Name as Designation,L.Name as Location from Employeesmaster" & Session("webtableID") & " F inner join DSGmaster" & Session("webtableID") & " d on F.Dsg_Code = d.Dsg_Code inner join LocMaster" & Session("webtableID") & " L on L.Loc_Code=f.Loc_Code where F.emp_code ='" & Session("Emp_Code") & "'"
                    Dim dtemployeemaster As DataTable

                    dtemployeemaster = OldNewConn.GetDataTable2(sql_Query)

                    doc.NewPage()

                    table = New PdfPTable(columnsCount)

                    header = New PdfPCell(New Phrase(Session("Comp_Name").ToString(), normalboldunserline))
                    header.Colspan = columnsCount
                    header.Border = 0
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)

                    table.AddCell(blankheader)

                    header = New PdfPCell(New Phrase("FORM NO.12BB", normalFontBold))
                    header.Colspan = columnsCount
                    header.Border = 0
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("(See rule 26C)", normalFontBold))
                    header.Colspan = columnsCount
                    header.Border = 0
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("Statement showing particulars of claims by an employee for deduction of tax under section 192 ", normalFontBold))
                    header.Colspan = columnsCount
                    header.Border = 0
                    header.HorizontalAlignment = Element.ALIGN_LEFT
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("1. Name and address of the employee: " & StrConv(dtemployeemaster.Rows(0)("FirstName"), VbStrConv.ProperCase), normalFont))
                    header.Colspan = columnsCount
                    header.Border = 0
                    header.HorizontalAlignment = Element.ALIGN_LEFT
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("2. Permanent Account Number of the employee: " & dtemployeemaster.Rows(0)("PANNo").ToString(), normalFont))
                    header.Colspan = columnsCount
                    header.Border = 0
                    header.HorizontalAlignment = Element.ALIGN_LEFT
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("3. Financial year: " & FromDate.Year & "-" & ToDate.Year, normalFont))
                    header.Colspan = columnsCount
                    header.Border = 0
                    header.HorizontalAlignment = Element.ALIGN_LEFT
                    table.AddCell(header)

                    table.AddCell(blankheader)

                    header = New PdfPCell(New Phrase("Details of claims and evidences thereof ", normalFontBold))
                    header.Colspan = 13
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("Sl. No.", normalFont))
                    header.Colspan = 1
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("Nature of claim", normalFont))
                    header.Colspan = 7
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("Amount(Rs.)", normalFont))
                    header.Colspan = 2
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("Evidence / particulars", normalFont))
                    header.Colspan = 3
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)


                    header = New PdfPCell(New Phrase("(1)", normalFont))
                    header.Colspan = 1
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("(2)", normalFont))
                    header.Colspan = 7
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("(3)", normalFont))
                    header.Colspan = 2
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("(4)", normalFont))
                    header.Colspan = 3
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)


                    header = New PdfPCell(New Phrase("1.", normalFont))
                    header.Colspan = 1
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)

                    Dim phrase2 As New Phrase()
                    phrase2.Add(New Chunk("House Rent Allowance: ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("(i) Rent paid to the landlord ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("(ii) Name of the landlord ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("(iii) Address of the landlord  ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("(iv) Permanent Account Number of the landlord ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("Note: Permanent Account Number shall be furnished if the aggregate rent paid during the previous year exceeds one lakh rupees.", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(Chunk.NEWLINE)
                    header = New PdfPCell(phrase2)
                    header.Colspan = 7
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("", normalFont))
                    header.Colspan = 2
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("", normalFont))
                    header.Colspan = 3
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)


                    header = New PdfPCell(New Phrase("2.", normalFont))
                    header.Colspan = 1
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)

                    phrase2 = New Phrase()
                    phrase2.Add(New Chunk("Leave travel concessions or assistance", normalFont))
                    header = New PdfPCell(phrase2)
                    header.Colspan = 7
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase(Format(Total_BillField, "####0.00"), normalFontBold))
                    header.Colspan = 2
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("", normalFont))
                    header.Colspan = 3
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)


                    header = New PdfPCell(New Phrase("3.", normalFont))
                    header.Colspan = 1
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)

                    phrase2 = New Phrase()
                    phrase2.Add(New Chunk("Deduction of interest on borrowing:  ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("(i) Interest payable/paid to the lender  ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("(ii)Name of the lender ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("(iii) Address of the lender  ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("(iv) Permanent Account Number of the lender ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("(a) Financial Institutions (if available) ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("(b) Employer (if available) ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("(c) Others  ", normalFont))
                    header = New PdfPCell(phrase2)
                    header.Colspan = 7
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("", normalFont))
                    header.Colspan = 2
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("", normalFont))
                    header.Colspan = 3
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)



                    header = New PdfPCell(New Phrase("4.", normalFont))
                    header.Colspan = 1
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)

                    phrase2 = New Phrase()
                    phrase2.Add(New Chunk("Deduction under Chapter VI-A ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("     (A) Section 80C,80CCC and 80CCD", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("         (i) Section 80C ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("             (a) …………….. ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("             (b) …………….. ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("             (c) …………….. ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("             (d) …………….. ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("             (e) …………….. ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("             (f) …………….. ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("             (g) …………….. ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("         (ii) Section 80CCC ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("         (iii) Section 80CCC ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("     (B) Other sections (e.g. 80E, 80G, 80TTA, etc.) under Chapter VI-A. ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("         (i) section………………. ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("         (ii) section………………. ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("         (iii) section………………. ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("         (iv) section……………….", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(New Chunk("         (v) section………………. ", normalFont))
                    phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(Chunk.NEWLINE)
                    header = New PdfPCell(phrase2)
                    header.Colspan = 7
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("", normalFont))
                    header.Colspan = 2
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("", normalFont))
                    header.Colspan = 3
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)


                    'table.AddCell(blankheader)
                    'table.AddCell(blankheader)
                    'table.AddCell(blankheader)
                    'table.AddCell(blankheader)

                    header = New PdfPCell(New Phrase("", normalFont))
                    header.Colspan = 1
                    header.HorizontalAlignment = Element.ALIGN_MIDDLE
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("Verification", normalFont))
                    header.Colspan = 12
                    header.HorizontalAlignment = Element.ALIGN_CENTER
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("", normalFont))
                    header.Colspan = 1
                    header.HorizontalAlignment = Element.ALIGN_MIDDLE
                    table.AddCell(header)

                    phrase2 = New Phrase()
                    phrase2.Add(New Chunk("I, " & StrConv(dtemployeemaster.Rows(0)("FirstName"), VbStrConv.ProperCase) & ",son/daughter of " & StrConv(dtemployeemaster.Rows(0)("FatherName"), VbStrConv.ProperCase) & " do hereby certify that the information given above is complete and correct.", normalFont))
                    'phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(Chunk.NEWLINE)
                    header = New PdfPCell(phrase2)
                    header.Colspan = 12
                    header.HorizontalAlignment = Element.ALIGN_MIDDLE
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("", normalFont))
                    header.Colspan = 1
                    header.HorizontalAlignment = Element.ALIGN_MIDDLE
                    table.AddCell(header)

                    phrase2 = New Phrase()
                    phrase2.Add(New Chunk("Place : " & StrConv(dtemployeemaster.Rows(0)("Location"), VbStrConv.ProperCase) & "", normalFont))
                    'phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(Chunk.NEWLINE)
                    header = New PdfPCell(phrase2)
                    header.Colspan = 12
                    header.HorizontalAlignment = Element.ALIGN_MIDDLE
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("", normalFont))
                    header.Colspan = 1
                    header.HorizontalAlignment = Element.ALIGN_MIDDLE
                    table.AddCell(header)

                    phrase2 = New Phrase()
                    phrase2.Add(New Chunk("Date : " & Format(DateTime.Now.Date, "dd-MMM-yyyy"), normalFont))
                    'phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(Chunk.NEWLINE)
                    header = New PdfPCell(phrase2)
                    header.Colspan = 6
                    header.HorizontalAlignment = Element.ALIGN_MIDDLE
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("(Signature of the employee) :", normalFont))
                    header.Colspan = 6
                    header.HorizontalAlignment = Element.ALIGN_MIDDLE
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("", normalFont))
                    header.Colspan = 1
                    header.HorizontalAlignment = Element.ALIGN_MIDDLE
                    table.AddCell(header)

                    phrase2 = New Phrase()
                    phrase2.Add(New Chunk("Designation : " & StrConv(dtemployeemaster.Rows(0)("Designation"), VbStrConv.ProperCase) & "", normalFont))
                    'phrase2.Add(Chunk.NEWLINE)
                    phrase2.Add(Chunk.NEWLINE)
                    header = New PdfPCell(phrase2)
                    header.Colspan = 6
                    header.HorizontalAlignment = Element.ALIGN_MIDDLE
                    table.AddCell(header)

                    header = New PdfPCell(New Phrase("Full Name : " & StrConv(dtemployeemaster.Rows(0)("FirstName"), VbStrConv.ProperCase) & "", normalFont))
                    header.Colspan = 6
                    header.HorizontalAlignment = Element.ALIGN_MIDDLE
                    table.AddCell(header)
                    doc.Add(table)
                End If
            Next

            doc.Add(New Phrase(" "))
            doc.Close()
            Session("ClaimFormName") = "~/OutputFiles/" & Session("Comp_Code") & "/N42" & Session("WebTableID") & "12ReimClaimForm_" & extraFileName & ".pdf"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & extraFileName & FileName)
            Response.ContentType = "text/pdf"
            Response.WriteFile(FileName)
            Response.Flush()
            Response.End()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Reimbursement_ClaimReport(Fromdate As Date, Todate As Date)
        Try

            Dim ds As New DataSet

            Dim year As Integer
            If (DateTime.Now.Month <= 3) Then
                year = DateTime.Now.Year - 1
            Else
                year = DateTime.Now.Year
            End If

            If (dtOption.Rows.Count > 0) Then
                If (dtOption.Rows(0)("Print_Name").ToString.ToLower = "withoutbudget") Then
                    Dim dtLTA As DataTable = OldNewConn.GetDataTable2("Select RANK () OVER( ORDER BY R.ReimburseDetailsID) AS ReimburseDetailsID,R.ReimburseDetailsID as [Claim ID],M.ReimbursementPersonsDetailId as [Sub claim ID],M.Emp_Code as [Employee Code],EM.FirstName as [Employee Name],IsNull(PSS.Print_Name,R1.Name) as [Reimburse Type],M.PersonsName as [Name of Traveller], M.RelationshipWithEmployee as [Relationship with Employee],M.Dependence,M.ModeOfJourney as [Mode of Journey],M.[From] as [Travel From],M.[To] as [Travel To],CAST(M.ClaimAmount AS INT)  as [Claim Amount],Convert(nvarchar,M.JourneyDate,103) as [Journey Date],Convert(nvarchar,R.TransactionDate,103) as [Transaction Date],R.TransactionDetail as [Transaction Detail],CAST(R.ClaimAmount AS INT) as [Total amount of the Journey (both sides)],Convert(nvarchar,R.EntryDate,103) as [Entry Date] from dbo.OtherReimbursementPersonsDetail" & Session("WebTableID") & " M inner join dbo.reimbursementdetails" & Session("WebTableID") & " R on M.Reimbursedetailsid=R.Reimbursedetailsid inner join ReimburseMentTypeMaster" & Session("WebTableID") & " r1 on r1.reimbursetype_Code=R.reimbursetype_Code Left join payslipsetup" & Session("WebTableiD") & " PSS on PSS.Name = R1.Name left join EmployeesMaster" & Session("WebTableID") & " EM on EM.Emp_Code=R.Emp_Code where EntryDate between '" & Format(Fromdate.Date, "MM/dd/yyyy") & "' and '" & Format(Todate.Date, "MM/dd/yyyy") & "'")
                    Dim copyDtLTA As DataTable = dtLTA.Copy
                    copyDtLTA.TableName = "LTA Reimbursement Claim"
                    ds.Tables.Add(copyDtLTA)

                    Dim dtLTACheck As DataTable = OldNewConn.GetDataTable2("Select row_number () OVER( ORDER BY CheckingID) AS Sno,LC.Emp_code,EM.FirstName as [Employee Name],FirstBlockYear,LastBlockYear,NoOfClaims,LC.CreatedDate from LTAChecking" & Session("WebTableID") & " LC left join EmployeesMaster" & Session("WebTableID") & " EM on EM.Emp_Code=LC.Emp_Code")
                    Dim copyDtLTACheck As DataTable = dtLTACheck.Copy
                    copyDtLTACheck.TableName = "LTA exemption"
                    ds.Tables.Add(copyDtLTACheck)
                ElseIf (dtOption.Rows(0)("Print_Name").ToString.ToLower = "withfbp" Or dtOption.Rows(0)("Print_Name").ToString.ToLower = "with&withoutbudget") Then
                    Dim dtReimburseMastertable As DataTable = OldNewConn.GetDataTable2("select Name from Sys.tables where Name='ReimburseMaster" & Session("WebTableID") & "'")
                    If (dtReimburseMastertable.Rows.Count > 0) Then
                        If Session("WebTableID").ToString = "510" Then

                            Dim dtLTA As DataTable = OldNewConn.GetDataTable2("Select RANK () OVER( ORDER BY R.ReimburseDetailsID) AS ReimburseDetailsID,R.ReimburseDetailsID as [Claim ID],M.ReimbursementPersonsDetailId as [Sub claim ID],M.Emp_Code as [Employee Code],EM.FirstName as [Employee Name],IsNull(PSS.Print_Name,R1.Name) as [Reimburse Type],M.PersonsName as [Name of Traveller], M.RelationshipWithEmployee as [Relationship with Employee],M.Dependence,M.ModeOfJourney as [Mode of Journey],M.[From] as [Travel From],M.[To] as [Travel To],CAST(M.ClaimAmount AS INT)  as [Claim Amount],Convert(nvarchar,M.JourneyDate,103) as [Journey Date],Convert(nvarchar,R.TransactionDate,103) as [Transaction Date],R.TransactionDetail as [Transaction Detail],CAST(R.ClaimAmount AS INT) as [Total amount of the Journey (both sides)],Convert(nvarchar,R.EntryDate,103) as [Entry Date] from dbo.OtherReimbursementPersonsDetail" & Session("WebTableID") & " M inner join dbo.reimbursementdetails" & Session("WebTableID") & " R on M.Reimbursedetailsid=R.Reimbursedetailsid inner join ReimburseMentTypeMaster" & Session("WebTableID") & " r1 on r1.reimbursetype_Code=R.reimbursetype_Code Left join payslipsetup" & Session("WebTableiD") & " PSS on PSS.Name = R1.Name left join EmployeesMaster" & Session("WebTableID") & " EM on EM.Emp_Code=R.Emp_Code  where EntryDate between '" & Format(Fromdate.Date, "MM/dd/yyyy") & "' and '" & Format(Todate.Date, "MM/dd/yyyy") & "'")
                            Dim copyDtLTA As DataTable = dtLTA.Copy
                            copyDtLTA.TableName = "LTA Reimbursement Claim"
                            ds.Tables.Add(copyDtLTA)

                            Dim dtLTACheck As DataTable = OldNewConn.GetDataTable2("Select row_number () OVER( ORDER BY CheckingID) AS Sno,LC.Emp_code,EM.FirstName as [Employee Name],FirstBlockYear,LastBlockYear,NoOfClaims,LC.CreatedDate from LTAChecking" & Session("WebTableID") & " LC left join EmployeesMaster" & Session("WebTableID") & " EM on EM.Emp_Code=LC.Emp_Code")
                            Dim copyDtLTACheck As DataTable = dtLTACheck.Copy
                            copyDtLTACheck.TableName = "LTA exemption"
                            ds.Tables.Add(copyDtLTACheck)

                        Else
                            Dim dt As DataTable = OldNewConn.GetDataTable2("select * from ReimburseMaster" & Session("WebTableID") & " where field_name like '%LTA%' and Budget_Wet between '04/01/" & year & "' and '03/31/" & year + 1 & "'")
                            If (dt.Rows.Count > 0) Then
                                Dim dtLTA As DataTable = OldNewConn.GetDataTable2("Select RANK () OVER( ORDER BY R.ReimburseDetailsID) AS ReimburseDetailsID,R.ReimburseDetailsID as [Claim ID],M.ReimbursementPersonsDetailId as [Sub claim ID],M.Emp_Code as [Employee Code],EM.FirstName as [Employee Name],IsNull(PSS.Print_Name,R1.Name) as [Reimburse Type],M.PersonsName as [Name of Traveller], M.RelationshipWithEmployee as [Relationship with Employee],M.Dependence,M.ModeOfJourney as [Mode of Journey],M.[From] as [Travel From],M.[To] as [Travel To],CAST(M.ClaimAmount AS INT)  as [Claim Amount],Convert(nvarchar,M.JourneyDate,103) as [Journey Date],Convert(nvarchar,R.TransactionDate,103) as [Transaction Date],R.TransactionDetail as [Transaction Detail],CAST(R.ClaimAmount AS INT) as [Total amount of the Journey (both sides)],Convert(nvarchar,R.EntryDate,103) as [Entry Date] from dbo.OtherReimbursementPersonsDetail" & Session("WebTableID") & " M inner join dbo.reimbursementdetails" & Session("WebTableID") & " R on M.Reimbursedetailsid=R.Reimbursedetailsid inner join ReimburseMentTypeMaster" & Session("WebTableID") & " r1 on r1.reimbursetype_Code=R.reimbursetype_Code Left join payslipsetup" & Session("WebTableiD") & " PSS on PSS.Name = R1.Name left join EmployeesMaster" & Session("WebTableID") & " EM on EM.Emp_Code=R.Emp_Code  where EntryDate between '" & Format(Fromdate.Date, "MM/dd/yyyy") & "' and '" & Format(Todate.Date, "MM/dd/yyyy") & "'")
                                Dim copyDtLTA As DataTable = dtLTA.Copy
                                copyDtLTA.TableName = "LTA Reimbursement Claim"
                                ds.Tables.Add(copyDtLTA)

                                Dim dtLTACheck As DataTable = OldNewConn.GetDataTable2("Select row_number () OVER( ORDER BY CheckingID) AS Sno,LC.Emp_code,EM.FirstName as [Employee Name],FirstBlockYear,LastBlockYear,NoOfClaims,LC.CreatedDate from LTAChecking" & Session("WebTableID") & " LC left join EmployeesMaster" & Session("WebTableID") & " EM on EM.Emp_Code=LC.Emp_Code")
                                Dim copyDtLTACheck As DataTable = dtLTACheck.Copy
                                copyDtLTACheck.TableName = "LTA exemption"
                                ds.Tables.Add(copyDtLTACheck)
                            End If
                        End If

                    End If

                End If
            Else
                Dim dtReimburseMastertable As DataTable = OldNewConn.GetDataTable2("select Name from Sys.tables where Name='ReimburseMaster" & Session("WebTableID") & "'")
                If (dtReimburseMastertable.Rows.Count > 0) Then
                    Dim dt As DataTable = OldNewConn.GetDataTable2("select * from ReimburseMaster" & Session("WebTableID") & " where field_name like '%LTA%' and Budget_Wet between '04/01/" & year & "' and '03/31/" & year + 1 & "'")
                    If (dt.Rows.Count > 0) Then
                        Dim dtLTA As DataTable = OldNewConn.GetDataTable2("Select RANK () OVER( ORDER BY R.ReimburseDetailsID) AS ReimburseDetailsID,R.ReimburseDetailsID as [Claim ID],M.ReimbursementPersonsDetailId as [Sub claim ID],M.Emp_Code as [Employee Code],EM.FirstName as [Employee Name],IsNull(PSS.Print_Name,R1.Name) as [Reimburse Type],M.PersonsName as [Name of Traveller], M.RelationshipWithEmployee as [Relationship with Employee],M.Dependence,M.ModeOfJourney as [Mode of Journey],M.[From] as [Travel From],M.[To] as [Travel To],CAST(M.ClaimAmount AS INT)  as [Claim Amount],Convert(nvarchar,M.JourneyDate,103) as [Journey Date],Convert(nvarchar,R.TransactionDate,103) as [Transaction Date],R.TransactionDetail as [Transaction Detail],CAST(R.ClaimAmount AS INT) as [Total amount of the Journey (both sides)],Convert(nvarchar,R.EntryDate,103) as [Entry Date] from dbo.OtherReimbursementPersonsDetail" & Session("WebTableID") & " M inner join dbo.reimbursementdetails" & Session("WebTableID") & " R on M.Reimbursedetailsid=R.Reimbursedetailsid inner join ReimburseMentTypeMaster" & Session("WebTableID") & " r1 on r1.reimbursetype_Code=R.reimbursetype_Code Left join payslipsetup" & Session("WebTableiD") & " PSS on PSS.Name = R1.Name left join EmployeesMaster" & Session("WebTableID") & " EM on EM.Emp_Code=R.Emp_Code  where EntryDate between '" & Format(Fromdate.Date, "MM/dd/yyyy") & "' and '" & Format(Todate.Date, "MM/dd/yyyy") & "'")
                        Dim copyDtLTA As DataTable = dtLTA.Copy
                        copyDtLTA.TableName = "LTA Reimbursement Claim"
                        ds.Tables.Add(copyDtLTA)

                        Dim dtLTACheck As DataTable = OldNewConn.GetDataTable2("Select row_number () OVER( ORDER BY CheckingID) AS Sno,LC.Emp_code,EM.FirstName as [Employee Name],FirstBlockYear,LastBlockYear,NoOfClaims,LC.CreatedDate from LTAChecking" & Session("WebTableID") & " LC left join EmployeesMaster" & Session("WebTableID") & " EM on EM.Emp_Code=LC.Emp_Code")
                        Dim copyDtLTACheck As DataTable = dtLTACheck.Copy
                        copyDtLTACheck.TableName = "LTA exemption"
                        ds.Tables.Add(copyDtLTACheck)
                    End If
                End If

            End If

            Dim dttaxableMenu As DataTable = OldNewConn.GetDataTable2("Select name, print_name from payslipsetup" & Session("WebTableID") & " where name in ('ShowtaxableMenu') ")

            If (dttaxableMenu.Rows.Count > 0) Then
                If (dttaxableMenu.Rows(0)("Print_Name").ToString.ToLower = "y") Then
                    Dim dttaxableLTA As DataTable = OldNewConn.GetDataTable2("Select RANK () OVER( ORDER BY R.ReimburseDetailsID) AS ReimburseDetailsID,R.ReimburseDetailsID as [Claim ID],R.Emp_Code as [Employee Code],IsNull(PSS.Print_Name,R1.Name) as [Reimburse Type] ,CAST(R.ClaimAmount AS money)  as [Claim Amount],Convert(nvarchar,R.TransactionDate,103) as [Transaction Date], R.TransactionDetail as [Transaction Detail],CAST(R.ClaimAmount AS money) as [Total amount of the Journey (both sides)],Convert(nvarchar,R.EntryDate,103) as [Entry Date] from dbo.reimbursementdetails" & Session("WebTableID") & " R inner join ReimburseMentTypeMaster" & Session("WebTableID") & " R1 on R1.reimbursetype_Code=R.reimbursetype_Code Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name = R1.Name where EntryDate between '" & Format(Fromdate.Date, "MM/dd/yyyy") & "' and '" & Format(Todate.Date, "MM/dd/yyyy") & "' And R1.Name='Taxable LTA'")
                    Dim copyDttaxableLTA As DataTable = dttaxableLTA.Copy
                    copyDttaxableLTA.TableName = "Taxable LTA Reimbursement Claim"
                    ds.Tables.Add(copyDttaxableLTA)
                End If
            End If

            Dim cmdtext As String = "Select RANK () OVER( ORDER BY R.ReimburseDetailsID) AS ReimburseDetailsID,R.ReimburseDetailsID as [Claim ID],M.MultipleClaimDetailsID as [Sub claim ID],R.Emp_Code as [Employee Code],EM.FirstName as [Employee Name],IsNull(PSS.Print_Name,R1.Name) as [Reimburse Type],M.BillNumber as [Bill Number],Convert(nvarchar,M.BillDated,103) as [Bill Dated],CAST(M.BillAmount AS money)  as [Bill Amount],M.BillDetail as [Password/ Any other info],Convert(nvarchar,R.TransactionDate,103) as [Transaction Date],R.TransactionDetail as [Transaction Detail],CAST(R.ClaimAmount AS money) as [Total Claim Amount] ,Convert(nvarchar,R.EntryDate,103) as [Entry Date] from dbo.reimbursementdetails" & Session("WebTableID") & " R inner join dbo.OtherMultipleReimburseClaimDetails" & Session("WebTableID") & " M on M.Reimbursedetailsid= R.Reimbursedetailsid inner join ReimburseMentTypeMaster" & Session("WebTableID") & " r1 on r1.reimbursetype_Code=R.reimbursetype_Code Left join payslipsetup" & Session("WebTableiD") & " PSS on PSS.Name = R1.Name left join EmployeesMaster" & Session("WebTableID") & " EM on EM.Emp_Code=R.Emp_Code where EntryDate between '" & Format(Fromdate.Date, "MM/dd/yyyy") & "' and '" & Format(Todate.Date, "MM/dd/yyyy") & "' union all "
            cmdtext += "Select RANK () OVER( ORDER BY R.ReimburseDetailsID) AS ReimburseDetailsID,R.ReimburseDetailsID as [Claim ID],'' as [Sub claim ID],R.Emp_Code as [Employee Code],EM.FirstName as [Employee Name],IsNull(PSS.Print_Name,R1.Name) as [Reimburse Type],'' as [Bill Number],'' as [Bill Dated],''  as [Bill Amount],'' as [Password/ Any other info],Convert(nvarchar,R.TransactionDate,103) as [Transaction Date],R.TransactionDetail as [Transaction Detail],CAST(R.ClaimAmount AS money) as [Total Claim Amount] ,Convert(nvarchar,R.EntryDate,103) as [Entry Date] from dbo.reimbursementdetails" & Session("WebTableID") & " R left join ReimburseMentTypeMaster" & Session("WebTableID") & " r1 on r1.reimbursetype_Code=R.reimbursetype_Code Left join payslipsetup" & Session("WebTableID") & " PSS on PSS.Name = R1.Name left join EmployeesMaster" & Session("WebTableID") & " EM on EM.Emp_Code=R.Emp_Code where EntryDate between '" & Format(Fromdate.Date, "MM/dd/yyyy") & "' and '" & Format(Todate.Date, "MM/dd/yyyy") & "' and r1.isMultipleClaimDetails=0 And r1.isPersonsDetailRequired=0"

            Dim dtother As DataTable = OldNewConn.GetDataTable2(cmdtext)
            Dim copyDtother As DataTable = dtother.Copy
            copyDtother.TableName = "Other Reimbursement Claim"
            ds.Tables.Add(copyDtother)

            ExcelHelper.ToExcel(ds, "Report", Page.Response)

        Catch ex As Exception
            Dim message As String = ex.Message.ToString()
            Dim sb As New System.Text.StringBuilder()
            sb.Append("<script type = 'text/javascript'>")
            sb.Append("window.onload=function(){")
            sb.Append("alert('")
            sb.Append(message)
            sb.Append("')};")
            sb.Append("</script>")
            ClientScript.RegisterClientScriptBlock(Me.GetType(), "Redirect", sb.ToString())
        End Try
    End Sub

    Public Function getAmountFromOtherSetup(ByVal Field_Name As String, ByVal connString As String) As Integer
        Dim Logo As Double = 0.0
        Field_Name = "'" + Field_Name + "'"
        Using connObj As New SqlClient.SqlConnection(connString)
            Using cmdObj As New SqlClient.SqlCommand("Select IsNull(Amount,0) as Amount from OtherSetup" & Session("WebTableID") & " where Field_Name=" & Field_Name & "", connObj)
                connObj.Open()
                Using readerObj As SqlClient.SqlDataReader = cmdObj.ExecuteReader
                    'This will loop through all returned records 
                    While readerObj.Read
                        Logo = readerObj("Amount").ToString
                    End While
                End Using
                connObj.Close()
            End Using
        End Using
        Return Logo
    End Function

    Public Shared Function getPayDate(month As Integer, year As Integer) As Date
        Dim lastdayinmonth As Integer = DateTime.DaysInMonth(year, month)

        Return New DateTime(year, month, lastdayinmonth)
    End Function

    Protected Sub btndownload_Click(sender As Object, e As EventArgs) Handles btndownload.Click

        Dim FromDate As New DateTime()
        Dim ToDate As New DateTime()

        FromDate = DateTime.Now.Year & "-" & DateTime.Now.Month.ToString("00") & "-01"
        ToDate = DateTime.Now.Year & "-" & DateTime.Now.Month.ToString("00") & "-" & DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month.ToString("00"))

        If Not IsNothing(radFromDate.SelectedDate) And Not IsNothing(radToDate.SelectedDate) Then
            FromDate = radFromDate.SelectedDate.Value
            ToDate = radToDate.SelectedDate.Value
        End If

        Reimbursement_ClaimReport(FromDate, ToDate)
    End Sub




    Protected Sub btndownloadreimdeclaraton_Click(sender As Object, e As EventArgs)
        If ddlyear.SelectedIndex = 0 Then
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('please select year');", True)
            Return
        End If
        Dim comm As String = "Select rd.Emp_Code,em.FirstName,ISNULL(PSS.Print_Name,RTM.Name) as [Reimburse Type] , rd.DeclarationAmount,rd.DeclareDate,rd.DeclareBy,rd.ModifiedBy,rd.ModifiedDate from " & databasename & ".dbo.Reimbursementdeclaration" & Session("WebTableID").ToString & "_" & ddlyear.SelectedItem.Text & " rd join " & databasename & ".dbo.EmployeesMaster" & Session("WebTableID").ToString & " em on rd.Emp_Code=em.Emp_Code join " & databasename & ".dbo.ReimburseMentTypeMaster" & Session("WebTableID").ToString & " RTM on rd.ReimburseType_Code=RTM.ReimburseType_Code Left join " & databasename & ".dbo.payslipsetup" & Session("WebTableID").ToString & " PSS on PSS.Name =RTM.Name left join " & databasename & ".dbo.EmployeesMaster" & Session("WebTableID").ToString & " em2 on rd.DeclareBy=em2.Emp_Code order by rd.DeclareDate"
        Dim declaredt As DataTable = sqlHelper.utility.SqlHelper.ExecuteTable(ConfigurationManager.ConnectionStrings("DkmOnlineConnectionString").ToString, CommandType.Text, comm, Nothing)

        comm = "Select rd.ID, rd.Emp_Code,em.FirstName,ISNULL(PSS.Print_Name,RTM.Name) as [Reimburse Type] , rd.DeclarationAmount,rd.DeclareDate,rd.DeclareBy,rd.ModifiedBy,rd.ModifiedDate from " & databasename & ".dbo.ReimbursementdeclarationLog" & Session("WebTableID").ToString & "_" & ddlyear.SelectedItem.Text & " rd join " & databasename & ".dbo.EmployeesMaster" & Session("WebTableID").ToString & " em on rd.Emp_Code=em.Emp_Code join " & databasename & ".dbo.ReimburseMentTypeMaster" & Session("WebTableID").ToString & " RTM on rd.ReimburseType_Code=RTM.ReimburseType_Code Left join " & databasename & ".dbo.payslipsetup" & Session("WebTableID").ToString & " PSS on PSS.Name =RTM.Name left join " & databasename & ".dbo.EmployeesMaster" & Session("WebTableID").ToString & " em2 on rd.DeclareBy=em2.Emp_Code order by rd.ID"
        Dim declarelogdt As DataTable = sqlHelper.utility.SqlHelper.ExecuteTable(ConfigurationManager.ConnectionStrings("DkmOnlineConnectionString").ToString, CommandType.Text, comm, Nothing)

        Dim ds As DataSet = Nothing
        ds = New DataSet()
        ds.Tables.Add(declaredt)
        ds.Tables(0).TableName = "Declare Report"

        ds.Tables.Add(declarelogdt)
        ds.Tables(1).TableName = "Log Report"

        Using wb As New XLWorkbook()
            wb.Worksheets.Add(ds)

            Response.Clear()
            Response.Buffer = True
            Response.Charset = ""
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            Response.AddHeader("content-disposition", "attachment;filename=Reimbursement_Declaration_Reportt.xlsx")
            Using MyMemoryStream As New MemoryStream()
                wb.SaveAs(MyMemoryStream)
                MyMemoryStream.WriteTo(Response.OutputStream)
                Response.Flush()
                Response.End()
            End Using
        End Using
    End Sub
End Class