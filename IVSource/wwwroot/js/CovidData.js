function GetCovidData(countryIso) {
    //debugger;
    //var ImageList = $(".map");//.find(".map");//.find('.flag');
    var InputData = {
        CountryCode: countryIso,
        CityName: "",
        AirportCode: ""
    }
    try {
        $.ajax({
            type: "POST",
            url: "https://faio.itq.in/ITQCovidAPI/api/CovidData",
            datatype: "json",
            data: InputData,
            success: function (data) {
                if (data != null) {
                    var table = "";
                    var table1 = "";
                    var table3 = "";
                    var inc = 0;
                    for (var i = 0; i < data.Result.length; i++) {
                        var AirportDetail = data.Result[i].airport;
                        var TravelAdvisories = data.Result[i].travel_advisories;
                        var CovidInfo = data.Result[i].covid_info.entry_exit_info[0];
                        table += "<div class=\"col-lg-12\">";
                        //Start Airport Detail
                        table += "<div class=\"panel panel-default\" style=\"margin-top: 22px;\">";
                        table += "<div style=\"padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #6BBA70;color: white;text-align: center;font-size: 20px;\">";
                        table += "Airport Detail";
                        table += "</div>";
                        table += "<div class=\"panel-body center\">";
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
                                table += "<td>" + AirportDetail[keys[iAD]].replace("/", "/ ") + "</td>";
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
                        table += "<div style=\"padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #6BBA70;color: white;text-align: center;    font-size: 20px;\">";
                        table += "Travel Advisories Details";
                        table += "</div>";
                        table += "<div class=\"panel-body center\">";

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
                        table += "<div style=\"padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #6BBA70;color: white;text-align: center;    font-size: 20px;\">";
                        table += "Covid Info";
                        table += "</div>";
                        table += "<div class=\"panel-body center\">";

                        //Binding Data in Div
                        var keysCO = Object.keys(CovidInfo);
                        table += "<table>";
                        for (var jCO = 0; jCO < keysCO.length; jCO++) {

                            table += "<tr>";
                            table += "<td style=\"background-color: #6BBA70;color: white;\">" + keysCO[jCO].toUpperCase().replace("_", " ") + "</td>";
                            if (keysCO[jCO].toUpperCase() == "SOURCE".toUpperCase()) {
                                table += "<td><a href=\"" + CovidInfo[keysCO[jCO]] + "\">" + CovidInfo[keysCO[jCO]] + "</a></td>";
                            }
                            else if (keysCO[jCO].toUpperCase() == "LAST_UPDATED".toUpperCase()) {
                                if (CovidInfo[keysCO[jCO]]!=null) {
                                    table += "<td>" + CovidInfo[keysCO[jCO]].substring(0, 10).replace("No summary available - please", "Please") + "</td>";
                                }
                                else {
                                    table += "<td></td>";
                                }
                                
                            }
                            else {
                                if (CovidInfo[keysCO[jCO]]!=null) {
                                    table += "<td>" + CovidInfo[keysCO[jCO]].replace("No summary available - please", "Please") + "</td>";
                                }
                                else {
                                    table += "<td></td>";
                                }
                            }
                            table += "</tr>";

                        }
                        table += "</table>";
                        /*
                        //Header
                        table += "<table id=\"table\" class=\"table\">";
                        table += "<tbody id=\"table_body_AD\">";
                        table += "<tr>";
                        var keysCO = Object.keys(CovidInfo);
                        for (var jCO = 0; jCO < keysCO.length; jCO++) {
                            if (keysCO[jCO].toUpperCase() == "LOCATION_TYPE".toUpperCase() || keysCO[jCO].toUpperCase() == "LOCATION_CODE".toUpperCase()
                                || keysCO[jCO].toUpperCase() == "SOURCE".toUpperCase()) {
        
                            }
                            else {
                                table += "<th>" + keysCO[jCO].toUpperCase() + "</th>";
                            }
                        }
        
                        table += "</tr>";
                        table += "</thead>";
                        //End
        
                        //Body
                        table += "<tbody id=\"table_body_AD\">";
        
                        //for (var iCO = 0; iCO < CovidInfo.length; iCO++) {
                        var sourceLink = "";
                        table += "<tr>";
                        for (var jCO = 0; jCO < keysCO.length; jCO++) {
                            
                            if (keysCO[jCO].toUpperCase() == "LOCATION_TYPE".toUpperCase() || keysCO[jCO].toUpperCase() == "LOCATION_CODE".toUpperCase()) {
        
                            }
                            else {
                                if (keysCO[jCO].toUpperCase() == "SOURCE".toUpperCase()) {
                                    sourceLink=CovidInfo[keysCO[jCO]];
                                }
                                else if (keysCO[jCO].toUpperCase() == "LAST_UPDATED".toUpperCase()) {
                                    table += "<td>" + CovidInfo[keysCO[jCO]].substring(0, 10) + "</td>";
                                }
                                else
                                    table += "<td>" + CovidInfo[keysCO[jCO]] + "</td>";
                            }
                        }
                        table += "</tr>";
                        table += "<tr><td colspan=\"" + keysCO.length + "\"><a href=\"" + sourceLink + "\">" + sourceLink + "</a></td><tr>";
                        //}
                        table += "</tbody>";
        
        
                        table += "</table>";
                        */
                        table += "</div>";
                        table += "</div>";
                        //End Covid Info

                        //Start Covid Stat
                        if (data.Result[i].covid_stats != null) {
                            if (data.Result[i].covid_stats.country != null && i == 0) {
                                var CovidStat_Country = data.Result[i].covid_stats.country.daily_data
                                var population = data.Result[i].covid_stats.country.population;
                                var location_code = data.Result[i].covid_stats.country.location_code;
                                var location_name = data.Result[i].covid_stats.country.location_name;
                                var location_type = data.Result[i].covid_stats.country.location_type;

                                //Start Covid Stat Country
                                table += "<div class=\"panel panel-default\" style=\"margin-top: 22px;\">";
                                table += "<div style=\"padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #6BBA70;color: white;text-align: center;font-size: 28px;\">";
                                table += "Covid Statistics Country Wise";
                                table += "</div>";
                                table += "<div class=\"panel-body center\">";
                                table += "<table id=\"table\" class=\"table\">";
                                //Heading
                                table += "<thead id=\"table_head_AD\">";
                                table += "<tr>";

                                // table += "<th>Location Name</th>";
                                //table += "<th>Location Code</th>";
                                //table += "<th>Location Type</th>";
                                //table += "<th>Population</th>";
                                table += "<th>Report Date</th>";
                                table += "<th>New Cases</th>";
                                table += "<th>New Death</th>";
                                table += "</thead>";
                                //End
                                table += "<tbody id=\"table_body_AD\">";
                                for (var j = 0; j < CovidStat_Country.report_date.length; j++) {
                                    table += "<tr>";
                                    //  table += "<td>" + location_name + "</td>";
                                    //table += "<td>" + location_code + "</td>";
                                    //table += "<td>" + location_type + "</td>";
                                    //table += "<td>" + population + "</td>";
                                    table += "<td>" + CovidStat_Country.report_date[j] + "</td>";
                                    table += "<td>" + CovidStat_Country.new_cases[j] + "</td>";
                                    table += "<td>" + CovidStat_Country.new_deaths[j] + "</td>";
                                    table += "</tr>";
                                }
                                table += "</tbody>";
                                table += "</table>";
                                //Row


                                //End
                                table += "</div>";
                                table += "</div>";
                            }

                            //End  Covid Stat Country


                            //Risk Rating Country
                            if (data.Result[i].covid_stats.country != null) {
                                table += "<div class=\"panel panel-default\" style=\"margin-top: 22px;\">";
                                table += "<div style=\"padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #6BBA70;color: white;text-align: center;font-size: 28px;\">";
                                table += "Risk Rating Country Wise";
                                table += "</div>";
                                table += "<div class=\"panel-body center\">";
                                table += "<table id=\"table\" class=\"table\">";
                                //Heading
                                table += "<thead id=\"table_head_AD\">";
                                table += "<tr>";

                                table += "<th>Current to Last Ratio</th>";
                                table += "<th>Current to Peak Ratio</th>";
                                table += "<th>Risk Index</th>";
                                table += "<th>Risk Level</th>";
                                table += "</thead>";
                                //End
                                table += "<tbody id=\"table_body_AD\">";
                                table += "<tr>";
                                table += "<td>" + data.Result[i].covid_stats.country.risk_rating.current_to_last_ratio + "</td>";
                                table += "<td>" + data.Result[i].covid_stats.country.risk_rating.current_to_peak_ratio + "</td>";
                                table += "<td>" + data.Result[i].covid_stats.country.risk_rating.risk_index + "</td>";
                                table += "<td>" + data.Result[i].covid_stats.country.risk_rating.risk_level + "</td>";

                                table += "</tbody>";
                                table += "</table>";
                                //Row


                                //End
                                table += "</div>";
                                table += "</div>";
                            }
                            //End  Covid Risk Rating Country

                            //Start Covid Stat state
                            if (data.Result[i].covid_stats.state_province != null) {
                                var CovidStat_State = data.Result[i].covid_stats.state_province.daily_data
                                var population_State = data.Result[i].covid_stats.state_province.population;
                                var location_code_State = data.Result[i].covid_stats.state_province.location_code;
                                var location_name_State = data.Result[i].covid_stats.state_province.location_name;
                                var location_type_State = data.Result[i].covid_stats.state_province.location_type;
                                table += "<div class=\"panel panel-default\" style=\"margin-top: 22px;\">";
                                table += "<div style=\"padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #6BBA70;color: white;text-align: center;font-size: 28px;\">";
                                table += "Covid Statistics State Wise";
                                table += "</div>";
                                table += "<div class=\"panel-body center\">";
                                table += "<table id=\"table\" class=\"table\">";
                                //Heading
                                table += "<thead id=\"table_head_AD\">";
                                table += "<tr>";

                                table += "<th>Location Name</th>";
                                table += "<th>Location Code</th>";
                                table += "<th>Location Type</th>";
                                table += "<th>Population</th>";
                                table += "<th>Report Date</th>";
                                table += "<th>New Cases</th>";
                                table += "<th>New Death</th>";
                                table += "</thead>";
                                //End
                                table += "<tbody id=\"table_body_AD\">";
                                for (var j = 0; j < CovidStat_State.report_date.length; j++) {
                                    table += "<tr>";
                                    table += "<td>" + location_name_State + "</td>";
                                    table += "<td>" + location_code_State + "</td>";
                                    table += "<td>" + location_type_State + "</td>";
                                    table += "<td>" + population_State + "</td>";

                                    table += "<td>" + CovidStat_State.report_date[j] + "</td>";
                                    table += "<td>" + CovidStat_State.new_cases[j] + "</td>";
                                    table += "<td>" + CovidStat_State.new_deaths[j] + "</td>";
                                    table += "</tr>";
                                }
                                table += "</tbody>";
                                table += "</table>";
                                //Row


                                //End
                                table += "</div>";
                                table += "</div>";
                            }
                            //End  Covid Stat state



                            //Risk Rating State Wise
                            if (data.Result[i].covid_stats.state_province != null) {
                                table += "<div class=\"panel panel-default\" style=\"margin-top: 22px;\">";
                                table += "<div style=\"padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #6BBA70;color: white;text-align: center;font-size: 28px;\">";
                                table += "Risk Rating State Wise";
                                table += "</div>";
                                table += "<div class=\"panel-body center\">";
                                table += "<table id=\"table\" class=\"table\">";
                                //Heading
                                table += "<thead id=\"table_head_AD\">";
                                table += "<tr>";

                                table += "<th>Current to Last Ratio</th>";
                                table += "<th>Current to Peak Ratio</th>";
                                table += "<th>Risk Index</th>";
                                table += "<th>Risk Level</th>";
                                table += "</thead>";
                                //End
                                table += "<tbody id=\"table_body_AD\">";
                                table += "<tr>";
                                table += "<td>" + data.Result[i].covid_stats.state_province.risk_rating.current_to_last_ratio + "</td>";
                                table += "<td>" + data.Result[i].covid_stats.state_province.risk_rating.current_to_peak_ratio + "</td>";
                                table += "<td>" + data.Result[i].covid_stats.state_province.risk_rating.risk_index + "</td>";
                                table += "<td>" + data.Result[i].covid_stats.state_province.risk_rating.risk_level + "</td>";

                                table += "</tbody>";
                                table += "</table>";
                                //Row


                                //End
                                table += "</div>";
                                table += "</div>";
                            }
                            //End  Covid Risk Rating State Wise



                        }

                        //End




                        table += "</div>";

                    }
                    $('#CovidData').empty();
                    $('#CovidData').append(table);
                    load(this, 10);
                }
                else {
                    $('#CovidData').empty();
                    $('#CovidData').append("<div style=\"padding-top: 12px;padding-bottom: 12px;background-color: green;color: white;font-size: 25px;text-align: center;\">No covid information available</div>");
                    load(this, 10);
                }
            },
            error: function(XMLHttpRequest, textStatus, errorThrown){
                alert(errorThrown);
            }
        });
    }
    catch (err) {
        alert(err);
    }
}



