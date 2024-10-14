var tableColor = "#686968"; // "#686968"; //"#0089cf"; //#6BBA70

function GetCovidData(url, type, user, pass, countryIso) {
    var InputData = {
        CountryCode: countryIso,
        CityName: "",
        AirportCode: ""
    }

    $.ajax({
        type: type, 
        url: url, 
        datatype: "json",
        contentType: "application/json",
        "headers": {
            "UserName": user, 
            "Password": pass, 
            "Content-Type": "application/json"
        },
        data: JSON.stringify(InputData),
        success: function (data) {
            if (data != null) {
                console.log(data);
                var table = "";
                for (var i = 0; i < data.Result.length; i++) {
                    var setFirstActive = " style=\"display:block;\"";
                    var AirportDetail = data.Result[i].airport;
                    var TravelAdvisories = data.Result[i].travel_advisories;
                    var CovidInfo = data.Result[i].covid_info.entry_exit_info[0];
                    table += "<div class=\"col-lg-12\">";
                    //Start Airport Detail
                    table += "<div class=\"panel panel-default\" style=\"margin-top: 22px;\">";
                    table += "<div class=\"accordion\" style=\"padding-top: 8px;padding-bottom: 8px;text-align: left;background-color: " + tableColor + ";color: white;font-size: 20px;\">";
                    table += "<span class=\"headLeft\">Airport Detail </span> <span class=\"headRight\"><i class=\"fa fa-sort-desc\" aria-hidden=\"true\"></i></span>";
                    table += "</div>";
                    table += "<div class=\"panel-body center panelAcc\" " + setFirstActive +">";
                    table += "<table id=\"table\" class=\"table\">";
                    //Heading
                    table += "<thead id=\"table_head_AD\">";
                    table += "<tr>";
                    var keys = Object.keys(AirportDetail);
                    for (var jAD = 0; jAD < keys.length; jAD++) {
                        if (keys[jAD].toUpperCase() == "LATITUDE".toUpperCase() || keys[jAD].toUpperCase() == "LONGITUDE".toUpperCase()) {

                        }
                        else {
                            table += "<th>" + keys[jAD].toUpperCase().replace("_", " ") + "</th>";
                        }
                    }
                    table += "</thead>";
                    //End
                    table += "<tbody id=\"table_body_AD\">";
                    table += "<tr>";
                    for (var iAD = 0; iAD < keys.length; iAD++) {
                        if (keys[iAD].toUpperCase() == "LATITUDE".toUpperCase() || keys[iAD].toUpperCase() == "LONGITUDE".toUpperCase()) {

                        }
                        else {
                            table += "<td>" + AirportDetail[keys[iAD]] + "</td>";
                        }
                    }
                    table += "</tr>";
                    table += "</tbody>";
                    table += "</table>";
                    //Row


                    //End
                    table += "</div>";
                    table += "</div>";
                    //End Start Airport Detail

                    //Start Travel Advisories
                    table += "<div class=\"panel panel-default\" style=\"margin-top: 22px;\">";
                    table += "<div class=\"accordion\" style=\"padding-top: 8px;padding-bottom: 8px;text-align: left;background-color: " + tableColor + ";color: white;  font-size: 20px;\">";
                    table += "<span class=\"headLeft\">Travel Advisories Details </span> <span class=\"headRight\"><i class=\"fa fa-sort-desc\" aria-hidden=\"true\"></i></span>";
                    table += "</div>";
                    table += "<div class=\"panel-body center panelAcc\">";

                    //Header
                    table += "<table id=\"table\" class=\"table\">";
                    table += "<tbody id=\"table_body_AD\">";
                    table += "<tr>";
                    var keysTA = Object.keys(TravelAdvisories[0]);
                    for (var jTA = 0; jTA < keysTA.length; jTA++) {
                        if (keysTA[jTA].toUpperCase() != "URL".toUpperCase()) {
                            table += "<th>" + keysTA[jTA].toUpperCase().replace("_", " ") + "</th>";
                        }
                    }

                    table += "</tr>";
                    table += "</thead>";
                    //End

                    //Body
                    table += "<tbody id=\"table_body_AD\">";

                    for (var iTA = 0; iTA < TravelAdvisories.length; iTA++) {
                        var link = "";
                        table += "<tr>";
                        for (var jTA = 0; jTA < keysTA.length; jTA++) {
                            if (keysTA[jTA].toUpperCase() == "URL".toUpperCase()) {
                                link = TravelAdvisories[iTA][keysTA[jTA]];
                            }
                            else if (keysTA[jTA].toUpperCase() == "LAST_UPDATED".toUpperCase()) {
                                table += "<td>" + TravelAdvisories[iTA][keysTA[jTA]].substring(0, 10) + "</td>";
                            }
                            else
                                table += "<td>" + TravelAdvisories[iTA][keysTA[jTA]] + "</td>";
                        }
                        table += "</tr>";
                        table += "<tr><td colspan=\"1\">URL : </td> <td colspan=\"4\"><a href=\"" + link + "\">" + link + "</a></td><tr>";
                    }
                    table += "</tbody>";
                    table += "</table>";
                    //End


                    table += "</div>";
                    table += "</div>";
                    //End Travel Advisories


                    //Start Covid Info
                    table += "<div class=\"panel panel-default\" style=\"margin-top: 22px;\">";
                    table += "<div class=\"accordion\" style=\"padding-top: 8px;padding-bottom: 8px;text-align: left;background-color: " + tableColor + ";color: white; font-size: 20px;\">";
                    table += "<span class=\"headLeft\">Covid Info </span> <span class=\"headRight\"><i class=\"fa fa-sort-desc\" aria-hidden=\"true\"></i></span>";
                    table += "</div>";
                    table += "<div class=\"panel-body center panelAcc\">";

                    //Binding Data in Div
                    var keysCO = Object.keys(CovidInfo);
                    table += "<table>";
                    for (var jCO = 0; jCO < keysCO.length; jCO++) {

                        table += "<tr>";
                        table += "<td style=\"background-color: " + tableColor + ";color: white;\">" + keysCO[jCO].toUpperCase().replace("_", " ") + "</td>";
                        if (keysCO[jCO].toUpperCase() == "SOURCE".toUpperCase()) {
                            table += "<td><a href=\"" + CovidInfo[keysCO[jCO]] + "\">" + CovidInfo[keysCO[jCO]] + "</a></td>";
                        }
                        else if (keysCO[jCO].toUpperCase() == "LAST_UPDATED".toUpperCase()) {
                            table += "<td>" + CovidInfo[keysCO[jCO]].substring(0, 10) + "</td>";
                        }
                        else {
                            table += "<td>" + CovidInfo[keysCO[jCO]] + "</td>";
                        }
                        table += "</tr>";

                    }
                    table += "</table>";
                    table += "</div>";
                    table += "</div>";
                    //End Covid Info
                    table += "</div>";

                }
                $('#CovidData').empty();
                $('#CovidData').append(table);

                ApplyAccordion();
            }
            else {
                $('#CovidData').empty();
                $('#CovidData').append("<div style=\"padding-top: 8px;padding-bottom: 8px;background-color: green;color: white;font-size: 25px;text-align: center;\">No covid information available</div>");
            }
        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });
}

function ApplyAccordion() {
    var acc = document.getElementsByClassName("accordion");
    var i;

    for (i = 0; i < acc.length; i++) {
        acc[i].addEventListener("click", function () {
            this.classList.toggle("active");
            var panel = this.nextElementSibling;
            if (panel.style.display === "block") {
                panel.style.display = "none";
            } else {
                panel.style.display = "block";
            }
        });
    }
}