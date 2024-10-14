function SendEmail() {
    var recipient = $('#p_email').val();
    var code1 = $('#p_txtCC').val();
    var code2 = $('#p_txtMessage').val();
    alert(code);
    alert("A");
    alert("B" + code2);
    alert("C");
    $.ajax({
        type: "Post",
        url: "/IvsVisaCategories/SendEmail1",
        dataType: "text",
        data: {
            "code": recipient,
            "code1": code1,
            "code2": code2
        },
        success: function (data) {
            alert("Success");
        }
    });
}
