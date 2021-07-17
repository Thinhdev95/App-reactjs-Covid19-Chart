<!--#include virtual ="/library/library1.asp"-->
<html>
<%

QYear_F 	= Int(Request("QYear_F"))
QMont_F 	= Int(Request("QMont_F"))
QDay_F  	= Int(Request("QDay_F"))
QYear_T 	= Int(Request("QYear_T"))
QMont_T 	= Int(Request("QMont_T"))
QDay_T  	= Int(Request("QDay_T"))

Date_T = DateSerial(QYear_T, QMont_T, QDay_T)
Date_F = DateSerial(QYear_F, QMont_F, QDay_F)

If Date_F > Date_T then 
	Date_F = DateSerial(QYear_T, QMont_T, QDay_T)
	Date_T = DateSerial(QYear_F, QMont_F, QDay_F+1)
Else
	Date_F = DateSerial(QYear_F, QMont_F, QDay_F)
	Date_T = DateSerial(QYear_T, QMont_T, QDay_T+1)
End if

fCode		= Trim(Request("fCode"))
fCompany 	= Trim(Request("fCompany"))
fSubParent 	= Trim(Request("fSubParent"))
fLocationID	= Int(Request("fLocationID"))
fBranchCode	= Int(Request("fBranchCode"))
fDistrict	= Trim(Request("fDistrict"))
fLocalType	= NumberCheck(Request("fLocalType")) 
fStatusProcess	= NumberCheck(Request("fStatusProcess"))
fKindSale 		= Request("fKindSale")
fLocalTypeSale	= NumberCheck(Request("fLocalTypeSale")) 
fType		= NumberCheck(Request("fType")) 
fCusType		= NumberCheck(Request("fCusType")) 
fStatus		= NumberCheck(Request("fStatus")) 
fAction		= NumberCheck(Request("fAction")) 
fStaff		= Trim(Request("fStaff"))
fStaffrp		= Trim(Request("fStaffrp"))
fContract	= Trim(Request("fContract"))
fKind 		= NumberCheck(Request("fKind"))
 
viewSale	= NumberCheck(Request("ViewSale")) 
fAssignDate	= Trim(Request("fAssignDate"))
fSource     = NumberCheck(Request("fSource"))
fDateType   = NumberCheck(Request("fDateType"))
fProcessStatus = NumberCheck(Request("fProcessStatus"))
fReport =  NumberCheck(Request("fReport"))
fTypeTotal=NumberCheck(Request("fTypeTotal"))

ReasonNotOk = NumberCheck(Request("ReasonNotOk"))
sReasonNotOk =""

if ReasonNotOk>0 then
	 if ReasonNotOk = 99 then
	 sReasonNotOk = " AND LR3.[VALUE] IN (1,2,3,4,5) "
	 else
	 sReasonNotOk = " AND LR3.[VALUE]="& ReasonNotOk
	 end if
end if
strDate = ""
strDateT = ""
strDateDetail = ""

If fDateType = 1 Then 'Ngay phan cong
    strDate = " AND AssignDate>="& FieldCheck(Date_F) & " and AssignDate < " & FieldCheck(Date_T) & " "
    strDateT = " AND A.AssignDate>="& FieldCheck(Date_F) & " and A.AssignDate < " & FieldCheck(Date_T) & " "
ElseIf fDateType = 2 Then ' Ngay cham soc --khac
    strDateDetail = " AND Date>="& FieldCheck(Date_F) & " and Date < " & FieldCheck(Date_T) & " "
    strDateT = " AND R.Date>="& FieldCheck(Date_F) & " and R.Date < " & FieldCheck(Date_T) & " "
ElseIf fDateType = 5 Then ' Ngay tiep nhan
    strDate = " AND CareProposalDate>="& FieldCheck(Date_F) & " and CareProposalDate < " & FieldCheck(Date_T) & " "
    strDateT = " AND CareProposalDate>="& FieldCheck(Date_F) & " and CareProposalDate < " & FieldCheck(Date_T) & " "
Else    'Ngay do du lieu
    strDate = " AND Date>="& FieldCheck(Date_F) & " and Date < " & FieldCheck(Date_T) & " "
    strDateT = " AND A.Date>="& FieldCheck(Date_F) & " and A.Date < " & FieldCheck(Date_T) & " "
End If
'---------------------------------------------------------------------------------
sWHERE = ""
sTypeCustomer = ""
sCodeWhere=""
If fCompany <> "" Then sWHERE = sWHERE & " AND Lo.Parent=" & FieldCheck(fCompany) 
If fSubParent <> "" Then sWHERE = sWHERE & " AND Lo.SubParent=" & FieldCheck(fSubParent) 

If fLocationID > 0 Then 
	sWhereLocation = " AND LocationID = " & fLocationID
    fLocationTemp = fLocationID
Else
	sLocationRule = get_LocationRule
	sWhereLocation = sWhereLocation & " AND LocationID IN " & sLocationRule	
    If fCompany = "FTN" Then 
        fLocationTemp = 4
    Else
        fLocationTemp = 8
    End If
End if

if fBranchCode>0 Then sWHERE = sWHERE & " AND  O.BranchCode =" & fBranchCode
If fDistrict <> "0" Then sWHERE = sWHERE & " AND Location_District =" & FieldCheck(fDistrict) 
If fLocalType <> 0 Then sWHERE = sWHERE & " AND O.LocalType = " & fLocalType
If fLocalTypeSale <> 0 Then sWHERE = sWHERE & " AND ISNULL(O.BillingLocalType,O.LocalType) = " & fLocalTypeSale
If fType <> 0 and fType <> 8 Then sWHERE = sWHERE & " AND A.Type = " & fType & " AND ISNULL(CareProposalID,0) = 0 "
If fType = 8 Then sWHERE = sWHERE & " AND ( A.Type = " & fType & " OR CareProposalID > 0 ) " 
If fStatus < 99 Then sWHERE = sWHERE & " AND A.Status = " & fStatus
If fAction <>99 Then sWHERE = sWHERE & " AND R.Action = " & fAction

If fType = 2 or fType = 9 Then 
    sTypeCustomer = ",O.TypeCustomer"
    If fCusType = 1 Then 
        sCusType = " ,CASE WHEN LT.Fee_Monthly = SO.DisAmount  OR (SO.Description LIKE '%VIP%' OR SO.Description LIKE N'%Tri ân%' OR SO.Description LIKE  N'%Nhân viên%')  AND LT.ID IS NOT NULL THEN N'Đặc Biệt' END TypeCustomer"
        wCusType = " AND LT.Fee_Monthly = SO.DisAmount  OR ( SO.Description LIKE '%VIP%' OR SO.Description LIKE N'%Tri ân%'  OR SO.Description LIKE N'%Nhân viên%'  ) AND LT.ID IS NOT NULL"   
    End If             
    If fCusType = 2 Then 
        sCusType = " ,CASE WHEN LT.Fee_Monthly <> SO.DisAmount  AND (SO.Description NOT LIKE '%VIP%' OR SO.Description NOT LIKE N'%Tri ân%' OR SO.Description NOT LIKE  N'%Nhân viên%')  OR LT.ID IS NULL THEN N'Thường' END TypeCustomer"
        wCusType = " AND LT.Fee_Monthly <> SO.DisAmount  AND (SO.Description NOT LIKE '%VIP%' OR SO.Description NOT LIKE N'%Tri ân%' OR SO.Description NOT LIKE  N'%Nhân viên%')  OR LT.ID IS NULL  "
    End If
    If fCusType = 0 then sCusType = ",CASE WHEN LT.Fee_Monthly = SO.DisAmount  OR (SO.Description LIKE '%VIP%' OR SO.Description LIKE N'%Tri ân%' OR SO.Description LIKE  N'%Nhân viên%')  AND LT.ID IS NOT NULL THEN N'Đặc Biệt' ELSE N'Thường' END TypeCustomer"
    
End If

strProcessStatus = ""
If fProcessStatus > 0 and fProcessStatus <> 99 Then strProcessStatus = " AND ISNULL(R.ProcessStatus, 1) = " & fProcessStatus

If fStaff <>"0" Then 
	If fStaff = "A"  Then 
		sWHERE = sWHERE & " AND A.StaffCare IS NULL "	
		sWHEREStaffCare = " AND A.StaffCare IS NULL "	
	Else
		if fStaff <> "" then
			sWHERE = sWHERE & " AND A.StaffCare = " & FieldCheck(fStaff)
			sWHEREStaffCare = " AND A.StaffCare = " & FieldCheck(fStaff)
		end if
	End if
End if
sCareProposalID =" CareProposalID "
ssCareProposalID =" WHEN A.Type=8 or CareProposalID > 0 THEN N'KH yêu cầu'"
strRecareCus2016=""
if fReport = 1 then 
strCareProposalDateNew = "CASE " &_
		" WHEN  DATEPART(DW,CareProposalDate) = 1 THEN FORMAT(CAST(DATEADD(DAY,1,CareProposalDate) AS DATE),'yyyy/MM/dd 08:00:00')  " &_ 
		" WHEN  DATEPART(DW,CareProposalDate) = 7 THEN FORMAT(CAST(DATEADD(DAY,2,CareProposalDate) AS DATE),'yyyy/MM/dd 08:00:00')  " &_ 
        " WHEN  DATEPART(DW,CareProposalDate) = 6 AND  DATEPART(HOUR, CareProposalDate) >=17 " &_
        "             AND DATEPART(HOUR,CareProposalDate) < 24 THEN FORMAT(CAST(DATEADD(DAY,3,CareProposalDate) AS DATE),'yyyy/MM/dd 08:00:00') " &_ 
        " WHEN DATEPART(HOUR, CareProposalDate) >= 17 AND DATEPART(HOUR, CareProposalDate) <24 THEN FORMAT(CAST(DATEADD(DAY,1,CareProposalDate) AS DATE),'yyyy/MM/dd 08:00:00') " &_
        " WHEN  DATEPART(HOUR, CareProposalDate) >= 11 AND DATEPART(HOUR, CareProposalDate) < 14 THEN FORMAT(CAST(DATEADD(DAY,0,CareProposalDate) AS DATE),'yyyy/MM/dd 14:00:00')" &_
        " WHEN  DATEPART(HOUR, CareProposalDate) <= 7 THEN FORMAT(CAST(DATEADD(DAY,0,CareProposalDate) AS DATE),'yyyy/MM/dd 08:00:00') " &_
        "  ELSE CareProposalDate  " &_
		"	END AS CareProposalDateNew, "
End if
strRecare ="SELECT *, "&strCareProposalDateNew&"  MONTH([Date]) [Month], YEAR([Date]) [Year] FROM   Internet..RecareCus(NOLOCK) A  "
If fContract <>"" Then 
	sWHERE = sWHERE & " AND O.Contract = " & FieldCheck(fContract)
	strContract = " WHERE Contract = " & FieldCheck(fContract)
	if QYear_F <2017 or QYear_T <2017  then 
	strRecareCus2016 = strRecareCus2016 & "  SELECT *,  MONTH([Date]) [Month], YEAR([Date]) [Year] FROM " &_             
						"	(  SELECT ID	,ObjID	,StaffCare	,Type	,Resource	,Status	,AssignBy	,AssignDate	,Date	,ImportBy	,Flag	,Auto	,LocationID	,RecareNumber	,TransferDate	,ProcessStatus	,BillID	,RecareDate	,FlagIPTV	   FROM   Internet..RecareCus (NOLOCK) UNION ALL " &_ 
         "SELECT ID	,ObjID	,StaffCare	,Type	,Resource	,Status	,AssignBy	,AssignDate	,Date	,ImportBy	,Flag	,Auto	,LocationID	,RecareNumber	,TransferDate	,ProcessStatus	,BillID	,RecareDate	,FlagIPTV	   FROM   [172.20.16.10].[PowerHistory].[dbo].[RecareCus2016] ) AS a "
	strRecare=""
	sCareProposalID=" 0 as CareProposalID "
	ssCareProposalID=""
	end if
End if

If fKind >= 0 and fKind <> 99   and fKind <> 6  and fKind <> 7  Then sWHERE = sWHERE & " AND L.FNNew = " & fKind
if fKind = 6 then sWHERE = sWHERE & " AND c.ObjId IS NOT NULL "
if fKind = 7 then sWHERE = sWHERE & " AND (A.FlagIPTV = 1  AND L.FNnew <> 5)  "
If fKindSale <> "" Then
If NumberCheck(fKindSale) >= 0 and NumberCheck(fKindSale) <> 99 Then sWHERE = sWHERE & " AND LB.FN = " & NumberCheck(fKindSale)
End if
if ViewSale = 1 Then sWHERE = sWHERE & " AND Total>0 "
if ViewSale = 2 Then sWHERE = sWHERE & " AND Debt>0 "
if ViewSale = 3 Then sWHERE = sWHERE & " AND R.Revenue>0 "
if fAssignDate <> "" Then sWHERE = sWHERE & " AND CONVERT(datetime, CONVERT(varchar,AssignDate,110),110) = " & FieldCheck(fAssignDate)

strFlagCareNumber = ""
FlagCareNumber = NumberCheck(Request("FlagCareNumber")) 
If Request.QueryString("FlagCareNumber") = 1 Then strFlagCareNumber = " AND StaffCare IS NOT NULL "
If Request.QueryString("FlagCareNumber") = 2 Then strFlagCareNumber = " AND StaffCare IS NULL "
If Request.QueryString("FlagCareNumber") = 3 Then strFlagCareNumber = " AND RecareNumber > 0  AND Status > 1 AND Status <> 12 AND Status <> 11"
If Request.QueryString("FlagCareNumber") = 4 Then strFlagCareNumber = " AND StaffCare IS NOT NULL AND Status = 0 "

strSource = ""
If fSource <> 0 Then strSource = " AND Resource = " & fSource 

sJOIN = ""
if fDistrict <>"0" then sJOIN = sJOIN & " LEFT JOIN (SELECT ObjID, Location_District FROM ObjDSL D(nolock))AS D ON A.ObjID = D.ObjID " 
'-----------------------------------------
sWHERENEW = ""

If fCompany <> "" Then sWHERENEW = sWHERENEW & " AND Lo.Parent=" & FieldCheck(fCompany) 
If fSubParent <> "" Then sWHERENEW = sWHERENEW & " AND Lo.SubParent=" & FieldCheck(fSubParent) 

If fLocationID > 0 Then 
	sWHERENEW = sWHERENEW & " AND B.LocationID = " & fLocationID
    fLocationTemp = fLocationID
Else
	sLocationRule = get_LocationRule
	sWHERENEW = sWHERENEW & " AND B.LocationID IN " & sLocationRule	
    If fCompany = "FTN" Then 
        fLocationTemp = 4
    Else
        fLocationTemp = 8
    End If
End if

if fBranchCode>0 Then sWHERENEW = sWHERENEW & " AND B.BranchCode =" & fBranchCode
If fDistrict <> "0" Then sWHERENEW = sWHERENEW & " AND Location_District =" & FieldCheck(fDistrict) 
If fLocalType <> 0 Then sWHERENEW = sWHERENEW & " AND b.LocalType = " & fLocalType
If fLocalTypeSale <> 0 Then sWHERENEW = sWHERENEW & " AND ISNULL(b.BillingLocalType,b.LocalType) = " & fLocalTypeSale
If fType <> 0 Then sWHERENEW = sWHERENEW & " AND A.Type = " & fType
If fStatus < 99 Then sWHERENEW = sWHERENEW & " AND A.Status = " & fStatus
If fAction <>99 Then sWHERENEW = sWHERENEW & " AND Action = " & fAction

If fStaff <>"0" Then 
	If fStaff = "A" Then 
		sWHERENEW = sWHERENEW & " AND A.StaffCare IS NULL "	
	Else
		sWHERENEW = sWHERENEW & " AND A.StaffCare = " & FieldCheck(fStaff)
	End if
End if

If fContract <>"" Then 
	sWHERENEW = sWHERENEW & " AND B.Contract = " & FieldCheck(fContract)
	strContract = " WHERE Contract = " & FieldCheck(fContract)
End if


If fKind >= 0 and fKind <> 99 Then sWHERENEW = sWHERENEW & " AND L.FNNew = " & fKind
if ViewSale = 1 Then sWHERENEW = sWHERENEW & " AND Total>0 "
if ViewSale = 2 Then sWHERENEW = sWHERENEW & " AND (total-paidtotal)>0 "
if ViewSale = 3 Then sWHERENEW = sWHERENEW & " AND Revenue>0 "
if fAssignDate <> "" Then sWHERENEW = sWHERENEW & " AND CONVERT(datetime, CONVERT(varchar,AssignDate,110),110) = " & FieldCheck(fAssignDate)
'-----------------------------------------

' If (fStaff <>"0" AND fStaff<>"A") Then 
	' QueryString = "SELECT ISNULL(SUM(SL),0)AS sCount, ISNULL(SUM(Total),0)AS Total, ISNULL(SUM(Revenue),0)AS Revenue, ISNULL(SUM(Debt),0)AS Debt " & _
			' "FROM  " & _
			' "( " & _
			' "	SELECT ISNULL(Total,0)AS Total, ISNULL(Revenue,0)AS Revenue, ISNULL((total-paidtotal),0)AS Debt,A.StaffCare, 1 AS SL  " & _
			' "	FROM (SELECT * FROM Recarecus a(nolock) WHERE 1 =1 " & strDate & " and staffcare = "& FieldCheck(fStaff)&")AS A  " & _
			' "	INNER JOIN (SELECT id, Status,Localtype,locationid, BranchCode, Promotion, Contract FROM Object b(nolock) "& strContract &")as O on a.objid = O.id " & _
			' "	LEFT JOIN ( SELECT b.objid, fee, vat, paidtotal, total,billdate,lastpaiddate,billnumber FROM billing b(nolock) WHERE (datediff(m,fromdate,todate)>=2 OR ( "&_
		    ' " Content like '%Tra truoc ADSL 2T%'  "&_
            ' " OR Content like '%Tra truoc FTTH 2T%' "&_
            ' " OR Content like '%2T Tra truoc ADSL%' "&_
            ' " OR Content like '%2T Tra truoc FTTH%' "&_
		    ' " )) and paidaction = 0 and branchcode<90 and Servicetype in(21,22,23) AND BillDate>="& FieldCheck(Date_F)&" and BillDate<"& FieldCheck(Date_T)&" )AS C ON a.objid = c.objid  " & _
			' "	INNER  JOIN localtypeNew l(nolocK) on O.localtype = l.id " & _
			' "	INNER JOIN location lo(nolock) on lo.id = O.locationid " & sJOIN & _
			' " 	LEFT JOIN (SELECT * FROM RecareDetail R(nolock) WHERE Flag=1 " & strDateDetail & ")AS R ON A.ID = R.RecareID " & _
			' "	WHERE 1 = 1 " & strDateT & sWHERE & _
			' ")AS Main"																											    

	' if logonuser = "Oanhvk" Or logonuser = "Oanh254" Or logonuser = "Nhavh" Or Logonuser="Lamkbd" Then Response.write QueryString & "<br>" 

	' Set RsSum = IMSObj.ExecuteSQL(QueryString)
	' if Not RsSum.EOF Then
		    ' Total = RsSum("Total")
		    ' Revenue = RsSum("Revenue")
		    ' sCount = RsSum("sCount")
		    ' Debt = RsSum("Debt")		
	' End if
	' Set RsSum=Nothing
' End if

'Response.Write(fKindSale&"<br/>")
'Response.Write(sWHERE)
'Reponse.End
%>
<!--#include file ="CheckRule.asp"-->
<%
If fPower=-1 AND fRuleOut=0 Then
        Response.Write("<script>alert('Vui long lien he bo phan cap quyen !');history.back();</script>")
        Response.End
End If
If fPower=-1 AND fRuleOut=1 AND fContract="" Then
        Response.Write("<script>alert('Vui long nhap so hop dong !');history.back();</script>")
        Response.End
End If

If fPower=0 AND fContract="" AND (LCASE(fStaff)<>LCASE(Logonuser) and fReport <> 1 or( fReport = 1 and LCASE(fStaffrp)<>LCASE(Logonuser) ) ) Then
        Response.Write("<script>alert('Vui long nhap so hop dong hoac chon ten nhan vien !');history.back();</script>")
        Response.End
End If

If fCode<>"KXD" and fCode<>"" Then
	sCodeWhere=" AND lo.Code='"&fCode&"' AND A.StaffCare IS NOT NULL "
Elseif fCode="KXD" then
	sCodeWhere=" AND A.StaffCare IS NULL "
	' sCodeWhere=" AND A.StaffCare IS NULL"
End If

IF fTypeTotal=1 THEN 
	sCodeWhere= sCodeWhere &" AND W.Fee>0 AND W.FeeITV=0 "
ELSEIF fTypeTotal=2 THEN
	sCodeWhere= sCodeWhere &" AND W.Fee=0 AND W.FeeITV>0 "
ELSEIF fTypeTotal =3 THEN
	sCodeWhere= sCodeWhere &" AND W.Fee>0 AND W.FeeITV>0 "
ELSEIF fTypeTotal =4 THEN'doanh thu - cong no: chua xuat hoa don
	sCodeWhere= sCodeWhere &" AND W.Fee=0 AND W.FeeITV=0 "
ELSEIF fTypeTotal =5 THEN'doanh thu- cong no:da xuat hd
	sCodeWhere= sCodeWhere &" AND W.Fee>0 AND W.FeeITV>0 "	
ELSEIF fTypeTotal = 99 THEN
	sCodeWhere= sCodeWhere &" AND ( W.Fee>0 OR W.FeeITV>0 OR W.FeeIPTinh > 0 ) "
ELSEIF fTypeTotal = 101 THEN
	sCodeWhere= sCodeWhere &" AND W.FeeIPTinh > 0 "
END IF


fTypeDetail		= Request.QueryString("fTypeDetail")
fCode1 = Request.QueryString("fCode1")
fBranchName1 = Request.QueryString("fBranchName1")


'FieldCheck() 'asdas' NumberCheck()'
If fCode1 <> "" THEN sCodeWhere= sCodeWhere &" AND lo.Code = '"&fCode1&"' "

IF fTypeDetail = 1 THEN sCodeWhere= sCodeWhere &" AND (a.Status=14 OR a.Status=15 OR a.Status=16 OR a.Status=24) "
IF fTypeDetail = 2 THEN sCodeWhere= sCodeWhere &" AND ((a.Status=14 AND (W.Fee > 0 OR W.FeeITV > 0)) "&_
 " OR (a.Status=15 AND (W.Fee > 0 OR W.FeeITV > 0)) "&_
 " OR (a.Status=16 AND (W.Fee > 0 OR W.FeeITV > 0)) "&_
 " OR (a.Status=24 AND (W.Fee > 0 OR W.FeeITV > 0))) " 'tong thanh toan :da xuat hd 
IF fTypeDetail = 3 THEN sCodeWhere= sCodeWhere &" AND ((a.Status=14 AND( W.Fee = 0 AND W.FeeITV = 0)) "&_
 " OR (a.Status=15 AND( W.Fee = 0 AND W.FeeITV = 0)) "&_ 
 " OR (a.Status=16 AND( W.Fee = 0 AND W.FeeITV = 0)) "&_
 " OR (a.Status=24 AND( W.Fee = 0 AND W.FeeITV = 0)))  "  'tong thanh toan :chua xuat hd


IF fTypeDetail = 4 THEN sCodeWhere= sCodeWhere &" AND a.Status=15 "
IF fTypeDetail = 5 THEN sCodeWhere= sCodeWhere &" AND a.Status=15 AND (W.Fee > 0 OR W.FeeITV > 0) " 'thanh toan tai nha :da xuat hd 
IF fTypeDetail = 6 THEN sCodeWhere= sCodeWhere &" AND a.Status=15 AND W.Fee = 0 AND W.FeeITV = 0 "  'thanh toan tai nha :chua xuat hd


IF fTypeDetail = 7 THEN sCodeWhere= sCodeWhere &" AND a.Status=16 "
IF fTypeDetail = 8 THEN sCodeWhere= sCodeWhere &" AND a.Status=16 AND (W.Fee > 0 OR W.FeeITV > 0) " 'thanh toan tai quay :da xuat hd 
IF fTypeDetail = 9 THEN sCodeWhere= sCodeWhere &" AND a.Status=16 AND W.Fee = 0 AND W.FeeITV = 0 "  'thanh toan tai quay :chua xuat hd

IF fTypeDetail = 10 THEN sCodeWhere= sCodeWhere &" AND a.Status=14 "
IF fTypeDetail = 11 THEN sCodeWhere= sCodeWhere &" AND a.Status=14 AND (W.Fee > 0 OR W.FeeITV > 0) " 'thanh toan chuyen khoan :da xuat hd 
IF fTypeDetail = 12 THEN sCodeWhere= sCodeWhere &" AND a.Status=14 AND W.Fee = 0 AND W.FeeITV = 0 "  'thanh toan chuyen khoan :chua xuat hd

IF fTypeDetail = 13 THEN sCodeWhere= sCodeWhere &" AND a.Status=24 "
IF fTypeDetail = 14 THEN sCodeWhere= sCodeWhere &" AND a.Status=24 AND (W.Fee > 0 OR W.FeeITV > 0) " 'thanh toan online :da xuat hd 
IF fTypeDetail = 15 THEN sCodeWhere= sCodeWhere &" AND a.Status=24 AND W.Fee = 0 AND W.FeeITV = 0 "  'thanh toan online :chua xuat hd


If fStatusProcess <> 0 Then
	If fStatusProcess=1 Then	sStatusProcess = " and ISNULL(R.ProcessStatus, 0) IN (0) AND R.Date IS NULL "
	If fStatusProcess=2 Then	sStatusProcess = " and ISNULL(R.ProcessStatus, 0) IN (0)  AND (ISNULL(DATEDIFF(minute,A.CareProposalDateNew,GETDATE()),0)) <=50 AND R.Date IS NULL " 
	If fStatusProcess=3 Then	sStatusProcess = " and ISNULL(R.ProcessStatus, 0) IN (0)  AND (ISNULL(DATEDIFF(minute,A.CareProposalDateNew,GETDATE()),0)) >50 AND (ISNULL(DATEDIFF(minute,A.CareProposalDateNew,GETDATE()),0)) <=60 AND R.Date IS NULL " 
	if fStatusProcess=4 then 	sStatusProcess = " and  ISNULL(R.ProcessStatus, 0) IN (0)  AND  (ISNULL(DATEDIFF(minute,A.CareProposalDateNew,GETDATE()),0)) > 60 AND R.Date IS NULL "
	If fStatusProcess=5 then 	sStatusProcess =  " and ISNULL(R.ProcessStatus, 0) IN (1) "
	If fStatusProcess=6 Then	sStatusProcess = " and ISNULL(R.ProcessStatus, 0) IN (1)   AND  (ISNULL(DATEDIFF(minute,A.CareProposalDateNew,R.Date),0)) <= 60" 
	If fStatusProcess=7 Then	sStatusProcess = " and ISNULL(R.ProcessStatus, 0) IN (1)  AND  (ISNULL(DATEDIFF(minute,A.CareProposalDateNew,R.Date),0)) > 60 AND (ISNULL(DATEDIFF(minute,A.CareProposalDateNew,R.Date),0)) <= 90" 
	If fStatusProcess=8 Then	sStatusProcess = " and ISNULL(R.ProcessStatus, 0) IN (1)  AND  (ISNULL(DATEDIFF(minute,A.CareProposalDateNew,R.Date),0)) > 90 AND (ISNULL(DATEDIFF(minute,A.CareProposalDateNew,R.Date),0)) <= 120" 
	if fStatusProcess=9 then 	sStatusProcess = " and  ISNULL(R.ProcessStatus, 0) IN (1)  AND  (ISNULL(DATEDIFF(minute,A.CareProposalDateNew,R.Date),0)) > 120"
	
	If fStatusProcess=10 then   sStatusProcess = " and ISNULL(R.ProcessStatus, 0) IN (2)  "
	If fStatusProcess=11 Then	sStatusProcess = " and ISNULL(R.ProcessStatus, 0) IN (2)   AND  (ISNULL(DATEDIFF(minute,A.CareProposalDateNew,R.Date),0)) <= 60"
	If fStatusProcess=12 Then	sStatusProcess = " and ISNULL(R.ProcessStatus, 0) IN (2)   AND  (ISNULL(DATEDIFF(minute,A.CareProposalDateNew,R.Date),0)) > 60 AND (ISNULL(DATEDIFF(minute,A.CareProposalDateNew,R.Date),0)) <= 90"
	
	If fStatusProcess=13 Then	sStatusProcess = " and ISNULL(R.ProcessStatus, 0) IN (2)    AND  (ISNULL(DATEDIFF(minute,A.CareProposalDateNew,R.Date),0)) > 90 AND (ISNULL(DATEDIFF(minute,A.CareProposalDateNew,R.Date),0)) <= 120"
	If fStatusProcess=14 Then	sStatusProcess =" and ISNULL(R.ProcessStatus, 0) IN (2)  AND  (ISNULL(DATEDIFF(minute,A.CareProposalDateNew,R.Date),0)) > 120"
	If fStatusProcess=999 Then	sStatusProcess =" and ISNULL(R.ProcessStatus, 0) IN (0,1,2) "
	If fStatusProcess=15 Then	sStatusProcess =" and  (RD3.Date IS NOT NULL) OR (RD3.Date IS  NULL AND R.Date IS NOT NULL) "
	If fStatusProcess=16 Then	sStatusProcess =" and (RD3.Date IS NOT NULL AND ISNULL(DATEDIFF(minute,  A.CareProposalDateNew,RD3.Date), 0) < 50) OR (RD3.Date IS  NULL AND R.Date IS NOT NULL)   "
	If fStatusProcess=17 Then	sStatusProcess =" and  RD3.Date IS NOT NULL AND ISNULL(DATEDIFF(minute, A.CareProposalDateNew,RD3.Date), 0) >= 50  AND ISNULL(DATEDIFF(minute, A.CareProposalDateNew,RD3.Date), 0)  <= 60 "
	If fStatusProcess=18 Then	sStatusProcess =" and RD3.Date IS NOT NULL AND ISNULL(DATEDIFF(minute, A.CareProposalDateNew,RD3.Date), 0) > 60  "
	If fStatusProcess=19 Then	sStatusProcess =" and R.STATUS IN (18) and ((RD3.Date IS NOT NULL) OR (RD3.Date IS  NULL AND R.Date IS NOT NULL) ) " 
End If

if fReport = 1 then 
	if fStaffrp  <> "" then
		sstaffcare = " AND  StaffCare = " & FieldCheck(fStaffrp)
	end if
	 strDate = " AND CareProposalDate >="& FieldCheck(Date_F) & " and CareProposalDate < " & FieldCheck(Date_T) & " "
	swhererp = " AND StaffCare is  not null AND(CareProposalID > 0 or TYPE = 8) "&sstaffcare
	strDateT =""
	strDateOn = "  AND A.CareProposalDate < R.Date "
	if fStatusProcess >= 15 and fStatusProcess < 20 then
	strDateLJ= " LEFT JOIN (SELECT MIN(Date) Date,RecareID  " &_
		" FROM( SELECT ID,CareProposalDate,  " &_
              "CASE " &_
		" WHEN  DATEPART(DW,CareProposalDate) = 1 THEN FORMAT(CAST(DATEADD(DAY,1,CareProposalDate) AS DATE),'yyyy/MM/dd 08:00:00') " &_  
		" WHEN  DATEPART(DW,CareProposalDate) = 7 THEN FORMAT(CAST(DATEADD(DAY,2,CareProposalDate) AS DATE),'yyyy/MM/dd 08:00:00')   " &_
        " WHEN  DATEPART(DW,CareProposalDate) = 6 AND  DATEPART(HOUR, CareProposalDate) >=17  " &_
        "             AND DATEPART(HOUR,CareProposalDate) < 24 THEN FORMAT(CAST(DATEADD(DAY,3,CareProposalDate) AS DATE),'yyyy/MM/dd 08:00:00')  " &_
        " WHEN DATEPART(HOUR, CareProposalDate) >= 17 AND DATEPART(HOUR, CareProposalDate) <24 THEN FORMAT(CAST(DATEADD(DAY,1,CareProposalDate) AS  DATE),'yyyy/MM/dd 08:00:00')" &_
        " WHEN  DATEPART(HOUR, CareProposalDate) >= 11 AND DATEPART(HOUR, CareProposalDate) < 14 THEN FORMAT(CAST(DATEADD(DAY,0,CareProposalDate) AS DATE),'yyyy/MM/dd 14:00:00') " &_
        " WHEN  DATEPART(HOUR, CareProposalDate) <= 7 THEN FORMAT(CAST(DATEADD(DAY,0,CareProposalDate) AS DATE),'yyyy/MM/dd 08:00:00') " &_
        "  ELSE CareProposalDate  " &_
		"	END AS CareProposalDateNew " &_
		" FROM RecareCus (nolock)WHERE 1=1 	AND StaffCare IS NOT NULL  AND(CareProposalID > 0 or TYPE = 8) AND CareProposalDate >= " &FieldCheck(Date_F) & " AND CareProposalDate < " & FieldCheck(Date_T) & strLocation & strStaffCare &_
		")R " &_
		"INNER JOIN ( SELECT Date, RecareID " &_
		"FROM   RecareDetail(NOLOCK))rd3 " &_
        "ON  R.ID = rd3.RecareID  " &_
		"AND R.CareProposalDate < RD3.Date " &_
		"GROUP BY RecareID " &_
		")RD3 ON RD3.RecareID = A.ID " 
	End if	
	
      
End if


Query = "SELECT ISNULL(No.OBJID,0) NoObjID,modem.Description modemdes,b.LocationID NoLocationID,b.BranchName NoBranchName, ISNULL(Amount,0) Advance, A.ID, lo.parent,lo.name+'-'+ cast(O.BranchCode AS varchar(5)) AS Branchcode, Case when LL.Name <> 'HN' AND LL.Name <> 'SG' Then LL.Name ELSE LL.Name + '-' + SUBSTRING(Br.Description,3,2) END BranchSale, IB.Name AS Sale, St.Description AS ObjStatus,O.Contract, A.ObjID, LocaltypeNew AS Localtype, LB.Description LocaltypeSale, " & _
	" S.Description PromotionRecare, PromotionObject, A.Date ,BT, KM,IPTV.RealPrepaid BT_IPTV,IPTV.Discount KM_IPTV,IPTV.Refund KT_IPTV, KMBT KMTB,IPTinh.Promotion_IP,IPTinh.BT_IP,IPTinh.KM_IP,  " & _
	" Total,TotalITV,TotalIPTinh, Debt,A.StaffCare, A.AssignDate,A.FlagIPTV,billnumber,billdate, D.Location,  " & _
	" D.Location_PHone,R.Note, R.Date AS RecareDate,LR1.Description StatusDetail,LR2.Description  ActionDetail,LR3.Description  ReasonNotOK,  " & _
	" case when R.ProcessStatus = 1 Then N'Đang xử lý' when R.ProcessStatus = 2 then N'Xử lý hoàn tất' else N'Chưa xử lý' end ProcessStatus,A.RecareNumber,"&sCareProposalID&",  " & _
	" ISNULL(OV.FullNameVN,FullName) FullName,ISNULL(W.Revenue,0)AS Revenue, CASE WHEN A.Resource=1 Then 'SYSTEM' ELSE 'EXCEL' END Resource,  " & _
	" CASE WHEN A.Type=1 THEN N'Hết tiền BT' WHEN A.Type=2 THEN N'Hết tiền KM' WHEN A.Type=3 THEN N'Hết tiền IPTV' WHEN A.Type=4 THEN N'Đáo hạn tháng T+'  WHEN A.Type=5 THEN N'Hết tiền KM Combo'  WHEN A.Type = 9 THEN N'Hết hạn KM IPTV'   WHEN A.Type = 12 THEN N'Hết tiền IP Tĩnh' "&ssCareProposalID&"   END TypeRecare, ISNULL(Vip,0) Vip, ISNULL(TongLLNow,0) TongLLNow, DATEDIFF(Month,O.FirstAccess,GETDATE()) AS SoTSD,W.ToDate, " & _
	" CASE WHEN RPC.[ObjID] IS NOT NULL AND RPC.Renewed = 1 THEN N'Đã gia hạn' "&_
	" WHEN RPC.[ObjID] IS NOT NULL AND RPC.Renewed <> 1 THEN N'Chưa gia hạn' ELSE '' END AS 'RenewalName', a.CareProposalDate, A.Type,A.FlagKMIPTV,(SELECT AddFeeZoning FROM PowerInfo.dbo.UpgradeBandwidthProcessing (NOLOCK) UBP WHERE O.Obj = UBP.ObjID) AddFeeZoning "&sTypeCustomer&" " & _
	" FROM ("&strRecare& strRecareCus2016 &"  WHERE 1 = 1 "&strDate&swhererp& strFlagCareNumber & strSource & sWhereLocation &")AS A  " & _	
		"INNER JOIN (SELECT O.id Obj,Localtype,BranchCode,Promotion,Contract,SalesID,Advance,Status StatusObj,FirstAccess,BillingLocalType,FullName,LocationID,LT.Description LocaltypeNew,SO.Description PromotionObject "&sCusType&" FROM (SELECT *  FROM Internet..Object O ( NOLOCK ) ) O LEFT JOIN  (SELECT ID, ISNULL(DescriptionVN,Description) Description , DisAmount FROM SubsProm SO(NOLOCK)) SO  ON  SO.ID = O.Promotion LEFT JOIN LocalType LT(NOLOCK)  ON  LT.ID = O.LocalType where 1=1 "&wCusType &sWhereLocation&") AS O ON A.objid = O.Obj " &_
        "LEFT JOIN InternetVN..ObjectVN OV  (NOLOCK) ON O.Obj = OV.ObjID " &_
		"LEFT JOIN (SELECT IDRecarecus,MAX(BillDate) BillDate, MAX(BillNumber) BillNumber,SUM(Amount) Amount,SUM(Vip) Vip,SUM(Advance) Advance,SUM(BT) BT,SUM(KM) KM,SUM(KMBT) KMBT,SUM(Total) Total,SUM(TotalITV) TotalITV, SUM(Revenue) Revenue,SUM(Debt) Debt,SUM(TongLLNow) TongLLNow,MAX(ToDate) ToDate,SUM(Fee) Fee,SUM(FeeITV) FeeITV,SUM(FeeIPTinh) FeeIPTinh,SUM(TotalIPTinh) TotalIPTinh FROM DataWareHouse..DW_RecareCUS a(nolock) WHERE 1 = 1 AND DateRecareCus >= "& FieldCheck(Date_F)&" AND DateRecareCus < "& FieldCheck(Date_T)&" GROUP BY IDRecarecus) W ON A.ID = W.IDRecarecus " &_			
		"LEFT JOIN (SELECT DISTINCT [ObjID],RealPrepaid,Discount,Refund,MONTH([Date]) [Month],YEAR([Date]) [Year] FROM DatawareHouse.dbo.[DW_RecareIPTV](NOLOCK) WHERE 1=1 "&sWhereLocation&") IPTV ON IPTV.[ObjID]=A.[ObjID] AND IPTV.[Month]=A.[Month] AND IPTV.[Year]=A.[Year] "&_ 
        "LEFT JOIN (select * from Internet..RecareDetail (NOLOCK) WHERE Flag = 1 "&strDateDetail&") R ON A.ID = R.RecareID "&strDateOn&_
        "LEFT JOIN (SELECT ID, ISNULL(DescriptionVN,Description) Description FROM Status(NOLOCK))St ON St.ID = O.StatusObj "&_         
        "LEFT JOIN(SELECT ID, Name, BranchCode, LocationID FROM IBBMembers(NOLOCK))IB ON IB.ID = O.SalesID "&_     
        "LEFT JOIN(SELECT ID, Description FROM Branchcode(NOLOCK))Br ON IB.BranchCode = Br.ID "&_
        "LEFT JOIN (SELECT ID, Name,Code FROM Location(NOLOCK))LL ON LL.ID = IB.LocationID " &_        
		"LEFT  JOIN (SELECT ID, Description, Kind, FNNew FROM localtypeNew (nolocK))L on O.localtype = l.id " & _
		"LEFT JOIN  localtype(NOLOCK) LB ON Isnull(O.BillingLocalType,O.LocalType) = LB.ID " & _		
		"LEFT JOIN (SELECT Id, Parent, Name, SubParent,Code FROM location (nolock))lo on lo.id = A.locationid " & _	
		"LEFT JOIN (SELECT ISNULL(DescriptionVN,Description) Description, Value FROM ListResult (NOLOCK) where kind = 1 and type = 1)AS LR1 ON LR1.Value = R.Status " & _
		"LEFT JOIN (SELECT ISNULL(DescriptionVN,Description) Description, Value FROM ListResult (NOLOCK) where kind = 1 and type = 2)AS LR2 ON LR2.Value = R.Action " & _
		"LEFT JOIN (SELECT ISNULL(DescriptionVN,Description) Description, Value FROM ListResult (NOLOCK) where kind = 1 and type = 5)AS LR3 ON LR3.Value = R.Reason " & _	
		"LEFT JOIN (SELECT ID, ISNULL(DescriptionVN,Description) Description from SubsProm (nolock))S ON R.PromotionID = S.ID "	& _
		"LEFT JOIN (SELECT ObjID, Location_District, Location, Location_Phone FROM ObjDSL D(nolock))AS D ON A.ObjID = D.ObjID " & _ 
        "LEFT JOIN  (SELECT a.ObjID,b.Description FROM(SELECT * FROM PowerInside.dbo.EquipmentReplace er (NOLOCK) WHERE  er.UpdateStatusDate is null or Datediff(DAY,er.UpdateStatusDate, GETDATE()) <= 5 ) a "&_
        "INNER JOIN ( SELECT * FROM Internet.dbo.ListResult (NOLOCK) WHERE Kind = 39 AND Type = 1 AND [Enabled] = 1) b ON b.Value = a.TYPE ) modem ON modem.objid = a.objid "&_
		"LEFT JOIN (SELECT DISTINCT ObjID,MONTH([Date]) [Month],YEAR([Date]) [Year],Renewed FROM PowerInfo.dbo.Renewal_PromotionCombo(NOLOCK)) AS RPC ON A.ObjID = RPC.ObjID AND MONTH(A.[Date])=RPC.[Month] AND YEAR(A.[Date])=RPC.[Year] " & _ 
		"/* LEFT JOIN PowerInfo..UpgradeBandwidthProcessing (NOLOCK) UBP ON O.Obj = UBP.ObjID */ " &_
        "LEFT JOIN (SELECT IP.ObjID,PRO.Name Promotion_IP,PRE1.FeeReal BT_IP,PRE2.FeePromotion KM_IP                                                                                     "&_
        "FROM PowerInside..IPCustomer(NOLOCK) IP LEFT JOIN PowerInside..IPPromotion(NOLOCK) PRO ON IP.IPPromotionID = PRO.ID                                                             "&_
        "LEFT JOIN (SELECT SUM(Type*Amount) FeeReal,IPCustomerID FROM PowerInside..IPPrepaid(NOLOCK) WHERE PrepaidType = 1 GROUP BY IPCustomerID) PRE1 ON IP.ID = PRE1.IPCustomerID      "&_
        "LEFT JOIN (SELECT SUM(Type*Amount) FeePromotion,IPCustomerID FROM PowerInside..IPPrepaid(NOLOCK) WHERE PrepaidType = 2 GROUP BY IPCustomerID) PRE2 ON IP.ID = PRE2.IPCustomerID "&_
        "WHERE IP.IPStatus= 1 GROUP BY IP.ObjID, PRO.Name,PRE1.FeeReal,PRE2.FeePromotion ) IPTinh  ON IPTinh.ObjID=A.ObjID                                                                 "&_
        "LEFT JOIN (SELECT DISTINCT ObjID, Type FROM PowerSMS..NotificationLogView (NOLOCK) WHERE CreateDate >= CAST( DATEADD(DAY, -3, GETDATE())  AS DATE)  AND CreateDate <  CAST( (DATEADD(DAY, 1, GETDATE())) AS DATE) AND Type=63 and Source =6) No ON No.ObjID = A.ObjID " &_
		"INNER JOIN(SELECT BranchCode,LocationID,SubCompanyName,b.BranchName FROM PowerData..Branch " &_
        "(NOLOCK) AS b WHERE 1 = 1 "&sWhereLocation&" "
IF (fBranchName1 <> "") THEN Query= Query &" AND b.BranchName = '" &fBranchName1&"') "
IF (fBranchName1 = "") THEN Query= Query & " )"
		
		Query = Query &" AS b ON o.BranchCode = b.BranchCode AND o.LocationID=b.LocationID  "&strDateLJ&_		
		" WHERE 1 = 1 " & strProcessStatus & sWHERE &sReasonNotOk &strDateT&sCodeWhere&sStatusProcess &_
		" ORDER BY lo.Parent,lo.NAME,O.Branchcode,A.Date,O.Obj "

if logonuser = "Oanhvk" Or logonuser = "Oanh254" Or logonuser = "Phuoctd" Or logonuser = "Nhanlh6" Or Logonuser="Lamkbd"  Or Logonuser="Duongnnb"  Or Logonuser="Daitq5"  Or Logonuser="Danhlt2" Or Logonuser="Duynk19" Then Response.write(Query) 

Set Rs = IMSObj.ExecuteSQL(Query , 0, 1)
RecordCount = 0
If Not Rs.EOF Then
	RecordCount = Rs.RecordCount
	Rs.PageSize = 100
	If Request("vPage")="" Then vPage = 1 Else vPage = Int(Request("vPage"))
	Rs.AbsolutePage = vPage
End If
%>
<head>
    <title></title>
    <script type="text/javascript">
        SetHeader('Báo cáo chi tiết xử lý');
    </script> 
    <script type="text/javascript" src="/Library/jquery.min.js"></script>
    <script type="text/javascript" src="/Library/jquery.isc.js"></script>
    <script type="text/javascript">   
        $(document).ready(function () {
            TabSort('<%=RecordCount%>');
        });
        function clickContract(fAssignDate, RecareID, ObjID, fCompany, fSubParent, fLocationID, fBranchCode, fDistrict, fReportType, QDay_F, QMont_F, QYear_F, QDay_T, QMont_T, QYear_T, fLocalType, fType, fStatus, fProcessDetail, fStaff, fContract, fAction, trNumber) {
            if (fAssignDate != '') {
                fURL = "Consultancy3.asp?RecareID=" + RecareID + "&ObjID=" + ObjID + "&fCompany=" + fCompany + "&fSubParent=" + fSubParent + "&fLocationID=" + fLocationID + "&fBranchCode=" + fBranchCode + "&fDistrict=" + fDistrict + "&fReportType=" + fReportType + "&QDay_F=" + QDay_F + "&QMont_F=" + QMont_F + "&QYear_F=" + QYear_F + "&QDay_T=" + QDay_T + "&QMont_T=" + QMont_T + "&QYear_T=" + QYear_T + "&fLocalType=" + fLocalType + "&fType=" + fType + "&fStatus=" + fStatus + "&fProcessDetail=" + fProcessDetail + "&fStaff=" + fStaff + "&fContract=" + fContract + "&fAction=" + fAction + "&fAssignDate=" + fAssignDate + "&trNumber=" + trNumber;
                window.open(fURL, "PopUpContract", "width=" + parseInt(screen.width) + ",height=" + parseInt(screen.height) + ",top=0" + ",left=0,toolbars=yes,scrollbars=yes,status=yes,resizable=yes");
            }
        }
        function LoadPackage(objid)
        {
            var w = 350;
            var h = 350;
            var left = Number((screen.width / 2) - (w / 2));
            var top = Number((screen.height / 2) - (h / 2));
            window.open("PackageIPTV.asp?fObjID=" + objid, "PopUpPackageIPTV", "width=" + w + ",height=" + h + ", top=" + top + ", left=" + left+",toolbars=yes,scrollbars=yes,status=yes,resizable=yes");
        }
    </script> 
    <script type="text/javascript" src="TableSorter.js"></script>
</head>
<body style="background-color: #D1D1D6; margin: 1px; padding: 1px">
    
    <table border="0" width="100%" cellspacing="1" cellpadding="1">
        <tr style="height: 25px">
            <td>
            	
                <strong>Tìm được:
                    <%=FormatNumber(Rs.RecordCount, 0, -1, -1)%></strong>
            </td>
            <td width="20%">
                <%
				If Rs.PageCount > 1 Then
					If vPage - 5 < 1 Then sCount = 1 Else sCount = vPage - 5
					If sCount > 1 Then Response_Write "... "
					For intCounter = sCount to vPage - 1
						Response_Write "<a href=JavaScript:PageSet(" & intCounter & ")>" & intCounter & "</a>"
						Response_Write " "
					Next
			
					Response_Write vPage & " "
					
					If sCount = 1 Then
						If vPage + 10 < Rs.PageCount Then eCount = vPage + 10 Else eCount = Rs.PageCount
					Else
						If vPage + 5 < Rs.PageCount Then eCount = vPage + 5 Else eCount = Rs.PageCount
					End If
			
					For intCounter = vPage + 1 to eCount
						Response_Write "<a href=JavaScript:PageSet(" & intCounter & ")>" & intCounter & "</a>"
						Response_Write " "
					Next
			
					If eCount <Rs.PageCount Then Response_Write "..."
				End If
                %>
            </td>
        </tr>
    </table>
    <table border="0" width="100%" cellspacing="1" cellpadding="3" id="myTable" class="tablesorter">
        <thead>
            <tr style="color: White; font-weight: bold; text-align: center; background-color: #A00000">
               <th width="20px">
                    STT
                </th>
                <th width="40px">
                    CN
                    Quản lý
                </th>
                <th width="80px">
                    Số HĐ
                </th>              
                <th width="80px">
                    Tình trạng hợp đồng</th>  
               <th width="80px">
                    Chương trình thay thiết bị</th>
                <th width="80px">
                    CN Bán</th>              
                <th width="80px">
                    Nhân viên Sale bán</th>
                <th width="80px">
                    Họ tên
                </th>
                <th width="70px">
                    Gói dịch vụ
                </th>
                <th width="70px">
                    Gói dịch vụ tính cước
                </th>
                <th width="100px">
                    KM hiện tại
                </th>
              <th width="100px">
                    CLKM IP tĩnh hiện tại
                </th>
                <th width="100px">
                    KM Recare
                </th>
                <th width="60px">
                    Tiền BT
                </th>
                <th width="60px">
                    Tiền KM
                </th>
                <th width="60px">
                  KT Hết tiền IPTV
                </th>
                <th width="60px">
                    Tiền BT IPTV
                </th>
                <th width="60px">
                    Tiền KM IPTV
                </th>
                <th width="60px">
                    Tiền KT IPTV
                </th>
                <th width="60px">
                    Tiền KMTB
                </th>
              <th width="70px">
                    Tiền BT IP tĩnh
                </th>
                <th width="70px">
                    Tiền KM IP tĩnh
                </th> 
                <th width="60px">
                    Lưu lượng sử dụng (Mb)</th>
                <th width="70px">
                    Ngày đổ DL
                </th>
                <th width="70px">
                    Ngày PC
                </th>
				<% if fReport = 1 then  %>
					<th width="70px">
                    Ngày tiếp nhận yêu cầu
                </th>
				<% end if%>
                <th width="30px">
                    Phụ trách
                </th>
                <th width="70px">
                    Ngày Recare
                </th>
                <th width="30px">
                    Số lần recare
                </th>
				<th width="30px">
                    Xem lịch sử đề nghị
                </th>
                <th width="30px">
                    Tiền đặt cọc</th>
                <th width="50px">
                    Doanh thu
                </th>
                <th width="50px">
                    DT NV nhập
                </th>
                <th width="50px">
                    Doanh thu ITV
                </th>
                 <th width="50px">
                    Doanh thu IP Tĩnh
                </th>
                <th width="50px">
                    Công nợ
                </th>
                <th width="30px">
                    Xuất hóa đơn
                </th>
                <th width="40px">
                    Kết quả
                </th>
				 <th width="40px">
                    Nguyên nhân
                </th>
                <th width="40px">
                    Tình trạng XL
                </th>
                <th width="40px">
                    TG kết thúc HDTT
                </th>
                <th width="40px">
                    Ghi chú
                </th>
                <th width="40px">
                    Nguồn
                </th>
                <th width="40px">
                    Loại Recare
                </th>
                <th width="40px">
                    Điện thoại
                </th>
				<th width="40px">
                   Số tháng SD
                </th>
				<th width="40px">
                   Gia hạn combo
                </th>
				<th width="40px">
                   Cước điều chỉnh
                </th>	

                <% if fType = 2 or  fType = 9 then  %>
				<th width="40px">
                   Loại KH
                </th>			
				<% end if%>

              				
            </tr>
        </thead>
        <tbody id="content">
            <%
            LNumber = Rs.PageSize * (vPage - 1) + 1
	        For intCounter = 1 to Rs.PageSize
	        If Rs.Eof Then Exit For
	        assignDate = Rs("AssignDate")	       
	        recareid = Rs("ID")
	        objid = Rs("ObjID")
	        staffcare = Rs("StaffCare")
	        vip = Rs("Vip")
            %>
            <tr style="background-color: #FFFFF4" id="tr<%=LNumber %>"  class='Detail'>
                <td>
                    <%=LNumber%>
                </td>
                <td>
                    <%=Rs("Branchcode") %>
                </td>
                <td>
                    <span id="spanContract" <%if Rs("AssignDate") <> "" THen %> onClick="clickContract('<%=assignDate %>', '<%=recareid %>','<%=objid%>','<%=fCompany%>','<%=fSubParent%>','<%=fLocationID%>','<%=fBranchCode%>','<%=fDistrict%>',4,'<%=QDay_F%>','<%=QMont_F%>','<%=QYear_F%>','<%=QDay_T%>','<%=QMont_T%>','<%=QYear_T%>','<%=fLocaltype%>','<%=fType%>','<%=fStatus%>','<%=fProcessDetail%>','<%=staffcare%>','<%=fContract%>','<%=fAction%>','tr<%=LNumber %>')"
                        style="color: Blue; cursor: hand" <%End If %>>
                        <%=Rs("Contract") %></span> 
                        <%if vip <> 0 Then%>
                            <img src="vipsmall.gif" alt="vip" />
                        <%End If %>
						<%if Rs("Type") = 9 OR NumberCheck(Rs("FlagKMIPTV"))=1  then %>
							<span style="color:Red;font-weight:bold"><a href='ViewOutOfPromotionMoney.asp?fObjID=<%=Rs("ObjID")%>&fContract=<%=Rs("Contract")%>&fDate=<%=Rs("Date")%>'>Hết tiền KM IPTV</a></span> 
					
						<%end if%>
                        <%If Numbercheck(Rs("NoObjID")) <> 0 then %>
                       <a style="color:red" href="http://inside.fpt.net/Functions/Contract/History/ReportNotification.asp?fContract=<%=Rs("Contract") %>&fLocationID=<%=Rs("NoLocationID") %>&fBranchName=<%=Rs("NoBranchName") %>&fFlag=1" target="_blank"> AutoCall</a>
                        <%End if %>
                </td>
                <td><%=Rs("ObjStatus") %></td>
                <td style="font-weight:bold;color:red"><%=Rs("modemdes") %></td>
                <td><%=Rs("BranchSale") %></td>
                <td><%=Rs("Sale") %></td>
                <td>
                    <%=Rs("FullName") %>
                </td>
                <td>
                    <%=Rs("LocalType") %>
                </td>
                <td>
                    <%=Rs("LocalTypeSale") %>
                </td>
                <td>
                    <%=Rs("PromotionObject") %>
                </td>
               <td>
                    <%=Rs("Promotion_IP") %>
                </td>
                <td <% If (Rs("PromotionObject")<>Rs("PromotionRecare")) Then%> style="color: Red"
                    <% End if%>
                    <%=Rs("PromotionRecare") %>
                </td>
                <td align="right">
                    <%=FormatNumber(NumberCheck(Rs("BT")), 0, -1, -1)%>
                </td>
                <td align="right">
                    <%=FormatNumber(NumberCheck(Rs("KM")), 0, -1, -1)%>
                </td>
                <td align="right">
					<%IF NumberCheck(Rs("FlagIPTV"))=1 OR NumberCheck(Rs("FlagKMIPTV"))=1 THEN%>
					<font id='fFlagIPTV' color='blue' style='cursor:pointer;text-decoration:underline;font-weight:bold' onClick='LoadPackage("<%=objid%>")'>Yes</font>
					<%end if%>
                    
                </td>
                <td align="right">
                    <%=FormatNumber(NumberCheck(Rs("BT_IPTV")), 0, -1, -1)%>
                </td>
                <td align="right">
                    <%=FormatNumber(NumberCheck(Rs("KM_IPTV")), 0, -1, -1)%>
                </td>
                <td align="right">
                    <%=FormatNumber(NumberCheck(Rs("KT_IPTV")), 0, -1, -1)%>
                </td>
                <td align="right">
                    <%=FormatNumber(NumberCheck(Rs("KMTB")), 0, -1, -1)%>
                    </td>
               <td align="right">
                  <%=Rs("BT_IP") %>
                    </td>
               <td align="right">
                  <%=Rs("KM_IP") %>
                  </td>
                <td align="right">
                    <%=FormatNumber(NumberCheck(Rs("TongLLNow")), 0, -1, -1) %> 
                </td>
                <td>
                    <%=IMSObj.FormatData(Rs("Date"), "dd/mm/yyyy ")%>
                </td>
                <td>
                    <%=IMSObj.FormatData(Rs("AssignDate"), "dd/mm/yyyy ")%>
                </td>
				<% if fReport = 1 then  %>
				<td>
                    <%=IMSObj.FormatData(Rs("CareProposalDate"), "dd/mm/yyyy hh:mm:ss")%>
                </td>
				<% end if%>
                <td>
                    <%=Rs("StaffCare") %>
                </td>

                <td>
                    <%=IMSObj.FormatData(Rs("RecareDate"), "dd/mm/yyyy hh:mm:ss")%>
                </td>
                <td align="right">
                    <%If Rs("RecareNumber")>0 Then%><a title="Chi tiet CS" href="Detail.asp?RecareID=<%=Rs("ID")%>"><%End if%><%=Rs("RecareNumber")%></a>
                </td>
				<td>
					<a href="#" onClick="window.open('/Functions/Contract/CareCustomer/Recare/ViewCareProposal.asp?fObjID=<%=Rs("ObjID")%>','DenghiCSKH','resizable,height=500,width=800'); return false;">Xem</a>
				</td>
                <td align="right">
                   <%=FormatNumber(Rs("Advance"), 0, -1, -1)%></td>
                <td align="right">
                    <%If Rs("Total")>0 Then%><a href="/Functions/InternetSubs/Accounting/BillHistory/View_9.Asp?vID=<%=Rs("ObjID")%>&QMont=<%=QMont_F%>&QYear=<%=QYear_F%>"><%End if%><%=FormatNumber(NumberCheck(Rs("Total")), 0, -1, -1)%></a>
                    </td>
                <td align="right" <% If (Rs("Total")<>Rs("Revenue")) Then%> style="color: Red" <% End if%>>
                    <%=FormatNumber(NumberCheck(Rs("Revenue")), 0, -1, -1)%>
                </td>
                <td align="right" >
                    <%=FormatNumber(NumberCheck(Rs("TotalITV")), 0, -1, -1)%>
                </td>
                <td align="right" >
                    <%=FormatNumber(NumberCheck(Rs("TotalIPTinh")), 0, -1, -1)%>
                </td>
                <td align="right">
                    <%=FormatNumber(NumberCheck(Rs("Debt")), 0, -1, -1)%>
                </td>
                <td>
                    <%=IMSObj.FormatData(Rs("BillDate"), "dd/mm/yyyy")%>
                </td>
                <td>
                    <%=Rs("StatusDetail")%>
                </td>
				 <td>
                    <%=Rs("ReasonNotOK")%>
                </td>
                <td>
                    <%=Rs("ProcessStatus")%>
                </td>
                <td>
                    <%=Rs("ToDate")%>
                </td>
                <td>
                    <%=Rs("Note")%>
                </td>
                <td>
                    <%=Rs("Resource")%>
                </td>
                <td>
                    <%=Rs("TypeRecare")%>
                    </td>
                <td>
                    <%=Rs("Location_Phone")%>
                </td>
                <td>
                    <%=Rs("SoTSD")%>
                </td>
				<td>
                    <%=Rs("RenewalName")%>
                </td>
				<td>
                    <%=Rs("AddFeeZoning")%>
                </td>
                <% if fType = 2 or fType = 9  then  %>
                <td>
                    <%=Rs("TypeCustomer")%>
                </td>
                <% end if %>
            </tr>
            <%
	        LNumber=LNumber+1								
	        Rs.MoveNext 
	        Next
            Rs.Close()
	        Set Rs = Nothing 
            %>
        </tbody>
    </table>
    <div id="divPackage" title="Chi tiết gói cước IPTV" style="display:none">      
    </div>
	<% strQueryString= Request.ServerVariables("QUERY_STRING") %>
	<input id="bExportExcel" type="button" style="width:100px" value="Xuất Excel" onClick="location.href = 'ViewProcessDetailNewExcel.asp?<%=strQueryString%>';" />
    <%
    IMSObj.dataConn.close()               
    Set IMSObj.dataConn = nothing
    %>
</body>
</html>

