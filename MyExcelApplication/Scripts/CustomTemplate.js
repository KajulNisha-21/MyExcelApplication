$(document).ready(function () {

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
    $("body").on("click", "#Searchrecord", function () {

        //Send the JSON array to Controller using AJAX.
        $.ajax({
            type: "POST",
            url: "/StudentList/SearchStudents",
            data: '{students: "' + $("#SearchFromFile").val() + '" }',
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                //debugger
                if (response != null && response != 0) {
                    alert(" record(s) Searched successfully.");
                    $('#SuccessMsg').show();
                    var Array = response;

                    // EXTRACT VALUE FOR HTML HEADER.
                    // ('RollNo', 'Name', 'Marks' and 'Result')
                    var col = [];
                    for (var i = 0; i < Array.length; i++) {
                        for (var key in Array[i]) {
                            if (col.indexOf(key) === -1) {
                                col.push(key);
                            }
                        }
                    }

                    // CREATE DYNAMIC TABLE.
                    var table = document.createElement("table");

                    // CREATE HTML TABLE HEADER ROW USING THE EXTRACTED HEADERS ABOVE.

                    var tr = table.insertRow(-1);                   // TABLE ROW.

                    for (var i = 0; i < col.length; i++) {
                        var th = document.createElement("th");      // TABLE HEADER.
                        th.innerHTML = col[i];
                        tr.appendChild(th);
                    }

                    // ADD JSON DATA TO THE TABLE AS ROWS.
                    for (var i = 0; i < Array.length; i++) {

                        tr = table.insertRow(-1);

                        for (var j = 0; j < col.length; j++) {
                            var tabCell = tr.insertCell(-1);
                            tabCell.innerHTML = Array[i][col[j]];
                        }
                    }

                    // FINALLY ADD THE NEWLY CREATED TABLE WITH JSON DATA TO A CONTAINER.
                    var divContainer = document.getElementById("showData");
                    divContainer.innerHTML = "";
                    divContainer.appendChild(table);

                }
                else if (response == 0) {
                    alert("No records found");
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
});
