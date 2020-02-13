var tableIdConstant;
var thIdsConstant;
var currentPage;
var sortColumn = "";
var sortDirection = "ASC";
var ajaxURL;
function ajaxTable(tableId, url) {
    tableIdConstant = tableId;
    ajaxURL = url;
    currentPage = 1;
    var table = document.getElementById(tableId).innerHTML;
    var tableHeaderMatch = Array.from(table.matchAll('(?<= (<th id=")).*(?=(">(.*)<\/th>))'));
    var tableHeaderIds = [];
    let i = 0;
    tableHeaderMatch.forEach(function () {
        tableHeaderIds[i] = tableHeaderMatch[i][0];
        i++;
    });
    thIdsConstant = tableHeaderIds;
    addElements();
    loadData();
}
function addElements() {
    let i = 0;
    thIdsConstant.forEach(function () {
        console.log("id: " + thIdsConstant[i]);
        document.getElementById(thIdsConstant[i]).innerHTML = "<div class='row' style='margin-left:2px'>" + document.getElementById(thIdsConstant[i]).innerHTML + "  <button class='float-right' onclick='sortAsc(" + thIdsConstant[i] + ")' style='margin-left:2px; background: none; border: none'>&#8593</button><button class='float-right' onclick='sortDesc(" + thIdsConstant[i] + ")' style='margin-left:2px; background: none; border: none'>&#8595</button></div></th>";
        i++;
    });
    document.getElementById(tableIdConstant).innerHTML += "<tbody id='body'></tbody>";
    document.getElementById(tableIdConstant).parentElement.innerHTML = "<label for='pageSize'>Results per page:</label>  <select id='pageSize' onchange='pageSize()'><option value='10'>10</option><option value='20'>20</option><option value='50'>50</option><option value='100'>100</option></select>" + document.getElementById(tableIdConstant).parentElement.innerHTML;
    document.getElementById(tableIdConstant).parentElement.innerHTML = "<div id='searchBox' class='float-right' style='margin-bottom: 25px'>Search: <input type='text' id='tableSearch'/><input type='submit' onclick='loadData()' /></div>" + document.getElementById(tableIdConstant).parentElement.innerHTML;
    document.getElementById(tableIdConstant).parentElement.innerHTML += "<div id='paging' class='float-right' style='margin-bottom: 5px'></div>";
}
function loadData() {
    var xhr = new XMLHttpRequest();
    xhr.open('POST', ajaxURL);
    xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
    xhr.onload = function () {
        if (xhr.status === 200) {
            document.getElementById('body').innerHTML = "";
            var data = JSON.parse(xhr.responseText);
            var internalData = data.data;
            resultsSize = internalData.length;
            totalRecords = data.recordsTotal;
            let i = 0;
            internalData.forEach(function () {
                let newRow = document.getElementById('body').insertRow(i);
                let j = 0;

                thIdsConstant.forEach(function () {
                    let newCell = newRow.insertCell(j);
                    let cellText = document.createTextNode(internalData[i][thIdsConstant[j]]);
                    newCell.appendChild(cellText);
                    j++;
                });
                i++;
            });
            drawPaging(totalRecords, resultsSize);
        }
        else {
            console.log("failed");
        }
    };
    var size = document.getElementById('pageSize');
    if (document.getElementsByClassName('page active').item(0) == null || (currentPage > Math.ceil(totalRecords / resultsSize))) {
        currentPage = 1;
    }
    var start = (currentPage - 1) * document.getElementById('pageSize').options[document.getElementById('pageSize').selectedIndex].value;
    xhr.send("start=" + start + "&length=" + size.options[size.selectedIndex].value + "&search=" + document.getElementById('tableSearch').value + "&sortColumn=" + sortColumn + "&sortDirection=" + sortDirection);
    return false;
};
function drawPaging(totalRecords, resultsSize) {
    document.getElementById('paging').innerHTML = "";
    document.getElementById('paging').innerHTML += "<div class='pagination'>";
    document.getElementById('paging').innerHTML += "<a href='#' onclick='previousPage()' style='padding: 2px; border:1px solid grey'>&laquo;</a>";
    if (totalRecords > document.getElementById('pageSize').options[document.getElementById('pageSize').selectedIndex].value) {
        for (let k = 0; k < Math.ceil(totalRecords / resultsSize); k++) {
            if (k === (currentPage - 1)) {
                document.getElementById('paging').innerHTML += "<a href='#' class='page active' id='page " + (k + 1) + "' onclick='changePage(" + (k + 1) + ")' style='padding: 2px; border:1px solid grey'>" + (k + 1) + "</a>";
            }
            else {
                document.getElementById('paging').innerHTML += "<a href='#' class='page' id='page " + (k + 1) + "' onclick='changePage(" + (k + 1) + ")' style='padding: 2px; border:1px solid grey'>" + (k + 1) + "</a>";
            }
        }
    }
    else {
        document.getElementById('paging').innerHTML += "<a href='#' class='page active' id='page 1' style='padding: 2px; border:1px solid grey'>1</a>";
    }
    document.getElementById('paging').innerHTML += "<a href='#' onclick='nextPage()' style='padding: 2px; border:1px solid grey'>&raquo;</a>";
    document.getElementById('paging').innerHTML += "</div>";
    return false;
};
function changePage(index) {
    if (document.getElementsByClassName('page active').item(0) != null) {
        document.getElementsByClassName('page active').item(0).classList.remove('active');
    }
    currentPage = index;
    var selectedPage = document.getElementById('page ' + index);
    selectedPage.classList.add('active');
    loadData();
    return false;
};
function pageSize() {
    changePage(1);
    loadData();
    return false;
};
function nextPage() {
    if (currentPage < (Math.ceil(totalRecords / resultsSize))) {
        changePage(currentPage + 1);
    }
    return false;
};
function previousPage() {
    if (currentPage > 1) {
        changePage(currentPage - 1);
    }
    return false;
};
function sortAsc(id) {
    sortDirection = "ASC";
    sortColumn = id.id;
    loadData();
    return false;
}
function sortDesc(id) {
    sortDirection = "DESC";
    sortColumn = id.id;
    loadData();
    return false;
}