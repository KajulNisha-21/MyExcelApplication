$(document).ready(function () {
    //get all student list function
    loadStudentlist();
    $("#myTab a").click(function (e) {
        e.preventDefault();
        $(this).tab('show');
    });

    //hide & show msg at necessary time
    $('#SuccessMsg').hide();
    $('#NoRecordMsg').hide();
    

    //check it (Clear button Function)
    $('#ClearRecord').click(function () {
        //location.reload(true);
        $('#SuccessMsg').hide();
        $('#StudentContainer').empty();
        $('#NoRecordMsg').hide();
        var ClearSearch = $('#SearchFromFile');
        ClearSearch.val("");
    });

    //search all the records in the field
    $("#Search-File").on("click", "#Searchrecord", function () {

        //Send the JSON array to Controller using AJAX.
        $.ajax({
            type: "POST",
            url: "/Home/SearchStudents",
            data: '{students: "' + $("#SearchFromFile").val() + '" }',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                //debugger
                if (response != null && response != 0) {
                    //alert(" record(s) Searched successfully.");
                    $('#SuccessMsg').show();
                    console.log(response);
                    $('#MyTemplate').tmpl(response).appendTo('#StudentContainer');

                }
                else if (response == 0) {
                    //alert("No records found");
                    $('#NoRecordMsg').show();
                }
                else {
                    alert("Something went wrong");
                }
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }

        });
    });

    //direct ajax to load student list without button click
    function loadStudentlist() {
        $.ajax({
            type: "GET",
            url: '/Home/DataList',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                //alert("Sucess");
                //debugger
                if (response != null) {
                    //$('#EmptyTable').hide();
                    $("#MyTemplate").tmpl(response).appendTo("#StudentListContainer");
                }
                else if (response == 0) {
                    alert("No records found");
                    //$('#EmptyTable').show();
                }
                else {
                    alert("Something went wrong");
                }
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }

        });
    };

    //upload excel to db
    $('#btnUpload').click(function () {

        // Checking whether FormData is available in browser
        if (window.FormData !== undefined) {
            var fileUpload = $("#FileUpload").get(0);
            var files = fileUpload.files;

            // Create FormData object
            var fileData = new FormData();

            // Looping over all files and add it to FormData object
            for (var i = 0; i < files.length; i++) {
                fileData.append(files[i].name, files[i]);
            }

            $.ajax({
                url: '/Home/InsertExcelData',
                type: "POST",
                contentType: false, // Not to set any content header
                processData: false, // Not to process data
                data: fileData,
                success: function (result) {
                    alert(result);
                    $("#FileUpload").val('');
                    $('#StudentListContainer').empty();
                    loadStudentlist();
                },
                error: function (err) {
                    alert(err.statusText);
                }
            });
        } else {
            alert("FormData is not supported.");
        }
    });

});

   