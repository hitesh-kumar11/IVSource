jQuery(".accordion").click(function (e) {
    function CityId() {
        var cityId = $(this).val();
        alert(cityId);
        $.ajax({
            type: "Post",
            url: "/IvsVisaCategories_User/VisaNoteFees",
            datatype: "text",
            data: {
                "city": cityId,
            },
            success: function (data) {
                alert("Success");
            }
        });
    }
});

