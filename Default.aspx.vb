Imports Telerik.Web.UI
Imports System.Data.Sql
Imports System.Data.SqlClient
Imports System.IO
Imports System.Data

Partial Class _Default
    Inherits Page

    Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("Centlec08_v2").ConnectionString)
    Private Property cmd As SqlCommand
    Dim rdr As SqlDataReader


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        
    End Sub

    Protected Sub RadButton1_Click(sender As Object, e As EventArgs) Handles RadButton1.Click
        Dim meterNumber As String
        Dim startDt, endDt As Date

        meterNumber = meterSearch.Text
        startDt = calStart.SelectedDate
        endDt = calEnd.SelectedDate


        If meterNumber = "" Then
            calcDataNoMeterNumber(startDt, endDt)
        Else
            calcDataWithMeterNumber(meterNumber, startDt, endDt)
        End If
    End Sub

    Private Sub calcDataNoMeterNumber(startDt As Date, endDt As Date)
        Dim totalUnits, totalAmount, count As Decimal
        Dim query As String

        conn.Open()

        'get total amount
        query = "select sum(TotalAmount) as TotalAmount FROM [Centlec08_v2].[sde].[TBL_METERPURCHASEPATTERN] where PurchaseDate between @startDt and @endDt"
        Using cmd As New SqlCommand
            With cmd
                .Connection = conn
                .CommandType = Data.CommandType.Text
                .CommandText = query
                .Parameters.AddWithValue("@startDt", startDt)
                .Parameters.AddWithValue("@endDt", endDt.AddDays(1))
            End With
            rdr = cmd.ExecuteReader
        End Using

        If rdr.HasRows Then
            Do While rdr.Read
                totalAmount = rdr.Item("TotalAmount")
            Loop
        End If
        rdr.Close()

        'get total unit
        query = "select sum(Units_Electricity) as Units_Electricity FROM [Centlec08_v2].[sde].[TBL_METERPURCHASEPATTERN] where PurchaseDate between @startDt and @endDt"
        Using cmd As New SqlCommand
            With cmd
                .Connection = conn
                .CommandType = Data.CommandType.Text
                .CommandText = query
                .Parameters.AddWithValue("@startDt", startDt)
                .Parameters.AddWithValue("@endDt", endDt.AddDays(1))
            End With
            rdr = cmd.ExecuteReader
        End Using

        If rdr.HasRows Then
            Do While rdr.Read
                totalUnits = rdr.Item("Units_Electricity")
            Loop
        End If
        rdr.Close()

        'get meter counts
        Dim meterCount As Integer
        query = "select count(MeterNumber) as meterCount FROM [Centlec08_v2].[sde].[TBL_METERPURCHASEPATTERN] where PurchaseDate between  @startDt and @endDt "
        Using cmd As New SqlCommand
            With cmd
                .Connection = conn
                .CommandType = Data.CommandType.Text
                .CommandText = query
                .Parameters.AddWithValue("@startDt", startDt)
                .Parameters.AddWithValue("@endDt", endDt.AddDays(1))
            End With
            rdr = cmd.ExecuteReader
        End Using

        If rdr.HasRows Then
            Do While rdr.Read
                meterCount = rdr.Item("meterCount")
            Loop
        End If
        rdr.Close()

        'calc averages + data
        Dim numDays, avgUnits, avgCost, deferedAmount As Decimal
        numDays = endDt.Subtract(startDt).Days
        'get average units + cost per day
        avgUnits = totalUnits / numDays
        avgCost = totalAmount / numDays
        'calc total days for the end month
        'daysInMonth = System.DateTime.DaysInMonth(Year(endDt), Month(endDt))
        'calc days left from last purchase
        'daysLeft = daysInMonth - lastpurchasedt.Day
        'calc units left from last purchase
        ' daysleftUnits = lastpurchasedAmount - (daysLeft * avgUnits)
        'calc cost carried over to new month
        deferedAmount = 1 * (avgCost / avgUnits)

        conn.Close()

        'create data table
        Dim Table1 As DataTable
        Table1 = New DataTable("Results")
        ' Define columns
        Table1.Columns.Add("Total Prepaid Meters", GetType(System.String))
        Table1.Columns.Add("Average Cost p/day", GetType(System.String))
        Table1.Columns.Add("Average Units p/day", GetType(System.String))
        Table1.Columns.Add("Deferred Amount", GetType(System.String))
        ' Add a row of data
        Table1.Rows.Add(FormatNumber(meterCount, 0, , , TriState.True), FormatCurrency(Math.Round(avgCost, 2), , , TriState.True, TriState.True), FormatNumber(Math.Round(avgUnits, 2), 2, , , TriState.True), "R" & Math.Round(deferedAmount, 2))


        RadGrid1.DataSource = Table1
        RadGrid1.Rebind()
        lblError.Visible = False

    End Sub

    Private Sub calcDataWithMeterNumber(meterNumber As String, startDt As Date, endDt As Date)
        Dim totalUnits, totalAmount, count As Decimal
        Dim query As String

        conn.Open()

        count = 0
        totalAmount = 0
        totalUnits = 0

        query = "select * FROM [Centlec08_v2].[sde].[TBL_METERPURCHASEPATTERN] where PurchaseDate between @startDt and @endDt and MeterNumber = @number order by PurchaseDate"
        Using cmd As New SqlCommand
            With cmd
                .Connection = conn
                .CommandType = Data.CommandType.Text
                .CommandText = query
                .Parameters.AddWithValue("@startDt", startDt)
                .Parameters.AddWithValue("@endDt", endDt.AddDays(1))
                .Parameters.AddWithValue("@number", meterNumber)
            End With
            rdr = cmd.ExecuteReader
        End Using

        Dim lastpurchasedt As Date
        Dim lastpurchasedAmount As Decimal

        If rdr.HasRows Then
            Do While rdr.Read
                count = count + 1
                totalAmount = totalAmount + rdr.Item("TotalAmount")
                totalUnits = totalUnits + rdr.Item("Units_Electricity")
                'get last record
                If Day(rdr.Item("PurchaseDate")) = Day(lastpurchasedt) Then
                    'lastpurchasedt = rdr.Item("PurchaseDate")
                    lastpurchasedAmount = lastpurchasedAmount + rdr.Item("Units_Electricity")
                Else
                    lastpurchasedt = rdr.Item("PurchaseDate")
                    lastpurchasedAmount = rdr.Item("Units_Electricity")
                End If

            Loop
        End If
        rdr.Close()


        'get meter counts
        Dim meterCount As Integer
        query = "select count(MeterNumber) as meterCount FROM [Centlec08_v2].[sde].[TBL_METERPURCHASEPATTERN] where PurchaseDate between  @startDt and @endDt and MeterNumber = @number "
        Using cmd As New SqlCommand
            With cmd
                .Connection = conn
                .CommandType = Data.CommandType.Text
                .CommandText = query
                .Parameters.AddWithValue("@startDt", startDt)
                .Parameters.AddWithValue("@endDt", endDt.AddDays(1))
                .Parameters.AddWithValue("@number", meterNumber)
            End With
            rdr = cmd.ExecuteReader
        End Using

        If rdr.HasRows Then
            Do While rdr.Read
                meterCount = rdr.Item("meterCount")
            Loop
        End If
        rdr.Close()

        'calc averages + data
        Dim numDays, avgUnits, daysInMonth, avgCost, daysLeft, daysleftUnits, deferedAmount As Decimal
        numDays = endDt.Subtract(startDt).Days
        'get average units + cost per day
        avgUnits = totalUnits / numDays
        avgCost = totalAmount / numDays
        'calc total days for the end month
        daysInMonth = System.DateTime.DaysInMonth(Year(endDt), Month(endDt))
        'calc days left from last purchase
        daysLeft = daysInMonth - lastpurchasedt.Day
        'calc units left from last purchase
        daysleftUnits = lastpurchasedAmount - (daysLeft * avgUnits)
        'calc cost carried over to new month
        deferedAmount = daysleftUnits * (avgCost / avgUnits)

        conn.Close()

        'create data table
        Dim Table1 As DataTable
        Table1 = New DataTable("Results")
        ' Define columns
        Table1.Columns.Add("Meter Number", GetType(System.String))
        Table1.Columns.Add("Amount Purchases", GetType(System.String))
        Table1.Columns.Add("Average Cost p/day", GetType(System.String))
        Table1.Columns.Add("Average Units p/day", GetType(System.String))
        Table1.Columns.Add("Last Purchase Date", GetType(System.DateTime))
        Table1.Columns.Add("Last Unit Amount", GetType(System.String))
        Table1.Columns.Add("Deferred Amount", GetType(System.String))
        ' Add a row of data
        Table1.Rows.Add(meterNumber, meterCount, "R" & Math.Round(avgCost, 2), Math.Round(avgUnits, 2), lastpurchasedt, Math.Round(lastpurchasedAmount, 2), "R" & Math.Round(deferedAmount, 2))


        RadGrid1.DataSource = Table1
        RadGrid1.Rebind()
        lblError.Visible = False
    End Sub

  
End Class