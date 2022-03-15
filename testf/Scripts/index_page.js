var tasks = ko.observableArray();
var currentIdTask = 0;
var count = 0;
if (localStorage.getItem('token') == null) {
    alert("You are not logged in!");
    window.location.replace("/Login/Login");
}
function afterLoading() {
    $.ajax({
        type: "GET",
        contentType: "application/x-www-form-urlencoded",
        url: "https://localhost:44370/api/tasks/",
        headers: { "Authorization": "Bearer " + localStorage.getItem('token') },
        success: function (data) {
            getTask(data);
        }
    });
}
function getTask(data) {
    for (var i = 0; i < data.length; i++) {
        var task = {
            dateOfTask: ko.observable(parseDate(data[i].dateOfTask)),
            description: ko.observable(data[i].description),
            directory: ko.observable(data[i].directory),
            header: ko.observable(data[i].header),
            id: ko.observable(data[i].id),
            isDone: ko.observable(data[i].isDone),
            isFavorite: ko.observable(data[i].isFavorite),
            isOverdue: ko.observable(data[i].isOverdue)
        };
        tasks.push(task);
    }
    ko.applyBindings(tasks);
}
function parseDate(date) {
    if (date != null) {
        var d = new Date(date);
        return d.toLocaleString();
    }
    return null;

}
function selectTask(data) {
    currentIdTask = ko.toJS(data).id;
    checkSelectedTasks();
    console.log(ko.toJS(data).id);
}
function checkSelectedTasks() {
    checkboxes = document.getElementsByName('checkedTask');
    count = 0;
    for (var i = 0, n = checkboxes.length; i < n; i++) {
        if (checkboxes[i].checked == true) {
            count++;
        }
    }
    if (count == 0) {
        currentIdTask = 0;
    }
    if (count > 1) {
        currentIdTask = 0;
        alert("You cannot select two or more tasks!");
        clearCheckboxes();
    }
}
function clearCheckboxes() {
    for (var i = 0, n = checkboxes.length; i < n; i++) {
        if (checkboxes[i].checked == true) {
            checkboxes[i].checked = false;
        }
    }
    count = 0;
}
function deleteTask() {
    if (currentIdTask != 0) {
        $.ajax({
            type: "DELETE",
            contentType: "application/x-www-form-urlencoded",
            url: "https://localhost:44370/api/tasks/" + currentIdTask,
            headers: { "Authorization": "Bearer " + localStorage.getItem('token') },
            success: function (data) {
                alert("Selected task was deleted!");
                window.location.href = '/Home/Index/';
            }
        });
    }
    else {
        alert("Please, choose a task!");
    }
}
function editTask() {
    if (currentIdTask != 0) {
        window.location.href = "/Home/EditTask/?task=" + currentIdTask;
    }
    else {
        alert("Please, choose a task!");
    }
}