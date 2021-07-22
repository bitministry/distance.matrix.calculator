ComparisonReport = (function () {

    var comparisonReport = {
        colors: ["blue", "purple", "orange", "brown"], 
        data : null 
    };

    //var comparisonReport =  function(json, id)
    //{
    //    this.data = json;
    //    this.id = id; 
    //    this.colors = ["blue", "purple", "orange", "brown"];
    //    this.JQuetyTmplBind();
    //}

    comparisonReport.InitFromJsonStr = function (jsonFunc)
    {
        this.data = jsonFunc();
        this.JQuetyTmplBind();
    }

    comparisonReport.InitFromJsonStr = function (jsonStr)
    {
        this.data = JSON.parse(jsonStr);
        this.JQuetyTmplBind();
    }

    comparisonReport.InitFromTextarea = function () {
        var jsonStr = $("#textareaJson").val();
        this.InitFromJsonStr(jsonStr);
    }

    comparisonReport.JQuetyTmplBind = function () {
        var myTemplate = $("#tmplCustomerAddressInReport").html();
        var xhtml = $.tmpl(myTemplate, this.data);
        $('#tableReport tbody').html(xhtml);
        return xhtml;
    }

    comparisonReport.ToggleDetails = function ( trId, button) {            

        $("#trDetails_" + trId).toggleClass('hidden');
        $("#trDetailsMap_" + trId).toggleClass('hidden');
        var btn = $(button);

        if (btn.html() == '<span class="glyphicon glyphicon-minus"></span>')
            btn.html('<span class="glyphicon glyphicon-plus"></span>');
        else {
            btn.html('<span class="glyphicon glyphicon-minus"></span>');

            var mapCanvasObject = $("#mapCanvas_" + trId)[0];
            if ( mapCanvasObject.innerHTML.length > 0 ) return;

            var iconStartPath = "../../Content/";
            var map = new google.maps.Map(mapCanvasObject );
            var bounds = new google.maps.LatLngBounds();
            var customerAddress = Enumerable.From( this.data )
                .Where("$.CustomerAddressId == "+ trId ).FirstOrDefault();

            var centrePoint = new google.maps.LatLng(customerAddress.CustomerAddressLat, customerAddress.CustomerAddressLng); 
            new google.maps.Marker({
                map: map,
                animation: google.maps.Animation.BOUNCE,
                icon:  iconStartPath+ "Markers/dots/red-dot.png",
                position: centrePoint
            });

            bounds.extend(centrePoint);

            this.CreateMarkersFromSpans(map, bounds, customerAddress.SpansToEnterprise, iconStartPath + "markers/EnterpriseRootContact.png", "enterprise");

            for (var index = 0; index < customerAddress.SpansToCompetitors.length; ++index) {
                var competitor = customerAddress.SpansToCompetitors[index];
                $("#th" + competitor.CompetitorContactId).html(competitor.CompetitorName + " <img src='" + iconStartPath + competitor.Icon + "'>");
                this.CreateMarkersFromSpans(map, bounds, competitor.SpansTo, iconStartPath + competitor.Icon, competitor.CompetitorName);
            }

            map.fitBounds(bounds);
            map.setCenter(bounds.getCenter());
        }
    }

    comparisonReport.CreateMarkersFromSpans = function (map, bounds, spans, iconUrl, contactName) {
        for (var index = 0; index < spans.length; ++index) {
            var span = spans[index];
            var pos = new google.maps.LatLng(span.Lat, span.Lng);
            new google.maps.Marker({
                map: map,
                title: contactName + " - " + span.PostCode,
                icon: iconUrl,
                position: pos
            });
            bounds.extend(pos);
        }
    }
    comparisonReport.ArrayShiftByVal = function ( arr )
    {
        var newarr = arr.slice();
        newarr.shift();
        return newarr;
    }

    return comparisonReport; 
})();



