<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<NzbDrone.Core.Entities.Series>>" %>

<%@ Import Namespace="Telerik.Web.Mvc.UI" %>

<asp:Content ID="Content3" ContentPlaceHolderID="JavascriptContent" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {

            $("#Mediabox").bind("click", MediaBoxClick);
            $("#Mediabox").dialog({
                autoOpen: false,
                modal: false,
                resizable: false,
                height: 100,
                width: 300,
                hide: "fade",
                show: "fade",
                beforeClose: function (event, ui) { return MinimizeMediabox(); }
            });

            setTimeout('MediaDetect();', 1000);
        });
        var Discovered = false;

        function MinimizeMediabox() {
            var mb = $("#Mediabox");
            mb.toggle('slow');
            return false;
        }

        function MediaDetect() {
            $.ajax({
                url: 'Series/MediaDetect',
                success: MediaDetectCallback
            });

        }
        function MediaDetectCallback(data) {
            Discovered = data.Discovered;
            if (!Discovered)
                setTimeout('MediaDetect();', 10000);
            else
                LightUpMedia(data);
        }

        function LightUpMedia(data) {
            $.ajax({
                url: 'Series/LightUpMedia',
                success: LightUpMediaSuccess
            });
        }
        function LightUpMediaSuccess(data) {
            var mb = $("#Mediabox")
            mb.html(data.HTML);
            var x = $(document).width() - mb.width();
            var y = 0;  

            mb.dialog('option', 'title', data.FriendlyName);
            mb.dialog('option', 'position', [x, y]);
            mb.dialog('open');
        }

        function MediaBoxClick(args) {
            var cn = args.target.className;
            $.ajax({
                url: 'Series/ControlMedia',
                data: "Action=" + cn
            });
        }


    </script>
    <div id="Mediabox" class="ui-widget-content ui-corner-all"></div>
    <style type="text/css">
    .Mediabox 
    {
    	background:black;
    }
    .Play 
    {
    	cursor:pointer;
        padding:5px;

    }
    .Pause 
    {
    	cursor:pointer;
        padding:5px;
    }
    .Stop 
    {
    	cursor:pointer;
        padding:5px;
    }
    </style>
    
</asp:Content>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Series
</asp:Content>
<asp:Content ID="Menue" ContentPlaceHolderID="ActionMenue" runat="server">
    <%
        Html.Telerik().Menu().Name("telerikGrid").Items(items => { items.Add().Text("View Unmapped Folders").Action("Unmapped", "Series"); })
                                                .Items(items => items.Add().Text("Sync With Disk").Action("Sync", "Series"))
                                                .Render();
    %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%


        Html.Telerik().Grid(Model)
       .Name("Grid")
       .Columns(columns =>
       {
           columns.Bound(o => o.SeriesId).Width(100);
           columns.Template(c =>
                                   {
    %>
    <%:Html.ActionLink(c.Title, "Details", new {seriesId =c.SeriesId}) %>
    <%
        }).Title("Title");
           columns.Bound(o => o.Status);
           columns.Bound(o => o.Path);
       })
       .Sortable(sort => sort.OrderBy(order => order.Add(o => o.Title).Ascending()).Enabled(false))
       .Render();
    %>
</asp:Content>
