$(document).ready(function () {

    //hide & show msg at necessary time
    $('#SuccessMsg').hide();
    $('#NoRecordMsg').hide();

    //check it (Clear button Function)
    $('#ClearRecord').click(function () {

        //location.reload(true);
        $('#SuccessMsg').hide();
        $('#showData').empty();
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
                url: '/StudentList/InsertExcelData',
                type: "POST",
                contentType: false, // Not to set any content header  
                processData: false, // Not to process data  
                data: fileData,
                success: function (result) {
                    alert(result);
                },
                error: function (err) {
                    alert(err.statusText);
                }
            });
        } else {
            alert("FormData is not supported.");
        }
    });

    //load excel to view data's
    $("body").on("click", "#SubmitFile", function () {

        //Reference the FileUpload element.
        var fileUpload = $("#FileUpload")[0];

        //Validate whether File is valid Excel file.
        var regex = /^([a-zA-Z0-9\s_\\.\-:])+(.xls|.xlsx)$/;
        if (regex.test(fileUpload.value.toLowerCase())) {
            if (typeof (FileReader) != "undefined") {
                var reader = new FileReader();

                //For Browsers other than IE.
                if (reader.readAsBinaryString) {
                    reader.onload = function (e) {
                        ProcessExcel(e.target.result);
                    };
                    reader.readAsBinaryString(fileUpload.files[0]);
                } else {
                    //For IE Browser.
                    reader.onload = function (e) {
                        var data = "";
                        var bytes = new Uint8Array(e.target.result);
                        for (var i = 0; i < bytes.byteLength; i++) {
                            data += String.fromCharCode(bytes[i]);
                        }
                        ProcessExcel(data);
                    };
                    reader.readAsArrayBuffer(fileUpload.files[0]);
                }
            } else {
                alert("This browser does not support HTML5.");
            }
        } else {
            alert("Please upload a valid Excel file.");
        }
    });

    //function to process the binary of excel data
    function ProcessExcel(data) {
        //Read the Excel File data.
        var workbook = XLSX.read(data, {
            type: 'binary'
        });

        //Fetch the name of First Sheet.
        var firstSheet = workbook.SheetNames[0];

        //Read all rows from First Sheet into an JSON array.
        var excelRows = XLSX.utils.sheet_to_row_object_array(workbook.Sheets[firstSheet]);

        //Create a HTML Table element.
        var table = $("<table />");
        table[0].border = "1";

        //Add the header row.
        var row = $(table[0].insertRow(-1));

        //Add the header cells.
        var headerCell = $("<th />");
        headerCell.html("Name");
        row.append(headerCell);

        var headerCell = $("<th />");
        headerCell.html("Marks");
        row.append(headerCell);

        var headerCell = $("<th  />");
        headerCell.html("Result");
        row.append(headerCell);

        //Add the data rows from Excel file.
        for (var i = 0; i < excelRows.length; i++) {
            //Add the data row.
            var row = $(table[0].insertRow(-1));

            //Add the data cells.
            var cell = $("<td />");

            cell = $("<td />");
            cell.html(excelRows[i].Name);
            row.append(cell);

            cell = $("<td />");
            cell.html(excelRows[i].Marks);
            row.append(cell);

            cell = $("<td />");
            cell.html(excelRows[i].Result);
            row.append(cell);
        }

        var dvExcel = $("#dvExcel");
        dvExcel.html("");
        dvExcel.append(table);

    };


});
