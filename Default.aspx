<%@ Page Title="Reports" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <telerik:RadSkinManager ID="QsfSkinManager" runat="server" Skin="BlackMetroTouch" />

    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server">
</telerik:RadAjaxLoadingPanel>
<telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
    <div class="jumbotron">
        <h2 style="text-align:center">Metering Reports</h2>
        <hr  />
        <asp:Label ID="lblError" runat="server" Visible="false" ForeColor="Red"  Text=""></asp:Label>
        <div class="row">
            <div class="col-md-4" >
                <asp:Label ID="Label1" runat="server" Text="Start Date:"></asp:Label>
                <telerik:RadCalendar ID="calStart" runat="server" CultureInfo="en-ZA" EnableMultiSelect="False" EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" SelectedDate=""></telerik:RadCalendar>
            </div>
            <div class="col-md-4">
            </div>
            <div class="col-md-4">
                <asp:Label ID="Label2" runat="server" Text="End Date:"></asp:Label>
                <telerik:RadCalendar ID="calEnd" runat="server" CultureInfo="en-ZA" EnableMultiSelect="False" EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" SelectedDate=""></telerik:RadCalendar>
            </div>
        </div>
        <div class="row">
            <div class="col-md-6">
                <asp:Label ID="lblMeterNumber" runat="server" Text="Meter Number:"></asp:Label> <br />
                <asp:Label ID="lbltest" runat="server" Text="(Leave Blank to search on ALL meter numbers)"></asp:Label> 
                <telerik:RadTextBox ID="meterSearch" runat="server"></telerik:RadTextBox>
                <telerik:RadButton ID="RadButton1" runat="server" Text="Report"></telerik:RadButton>
            </div>
            <div class="col-md-6">
                
            </div>
        </div>
    </div>  

    <div class="row">
        <div class="col-md-12">
            <telerik:RadGrid ID="RadGrid1" runat="server"></telerik:RadGrid>
        </div>
    </div>
</telerik:RadAjaxPanel>
</asp:Content>
