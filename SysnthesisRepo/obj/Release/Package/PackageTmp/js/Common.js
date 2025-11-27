

var ModuleName = "";
var PageName = "";
var LblAction = "";
var LblMessage = "";
var LinkImg = "";
$(Document).on("click", "a", function () {
    var details = this.getAttribute("detail");
    //var Logout = this.getAttribute("data-original-title");
    var onClickText = this.text.trim();

    if (details != null && details != "" && details != undefined) {
        onClickText = details + " " + onClickText;
    }
    
    else {
        onClickText = this.text.trim();
    }

    var LinkImg = this.innerHTML;


    if ((onClickText != null && onClickText != "" && onClickText != undefined) || (LinkImg != null && LinkImg != "" && LinkImg != undefined)) {
        GetActivityLogDetails("CLICK", onClickText, LinkImg);
        $.ajax({
            url: '/UserActivityLog/AddActivityLoad',
            data: { onClickText: onClickText, ActionName: ActionName, ControllerName: ControllerName, ModuleName: ModuleName, PageName: PageName, LblAction: LblAction, Message: LblMessage },
            async: false,
            success: function (response) {
                return true;
            },
            error: function () {

            }
        });
    }
    return true;
});
$(Document).on("click", "button", function () {
    var onClickText = this.innerHTML.trim();
    var details = this.getAttribute("detail");
    if (details != null && details != "" && details != undefined) {
        onClickText = details + " " + this.innerHTML.trim();
    }
    if (onClickText.includes("</svg>") && details != null && details != undefined && details != "") {
        onClickText = details;
    }

    if (onClickText != null && onClickText != "" && onClickText != undefined) {
        GetActivityLogDetails("BUTTON", onClickText);
        $.ajax({
            url: '/UserActivityLog/AddActivityLoad',
            data: { onClickText: onClickText, ActionName: ActionName, ControllerName: ControllerName, ModuleName: ModuleName, PageName: PageName, LblAction: LblAction, Message: LblMessage },
            async: false,
            success: function (response) {
                return true;
            },
            error: function () {

            }
        });
    }
    return true;
});
$(Document).on("click", "input:button", function () {
    var onClickText = this.value.trim();

    if (onClickText != null && onClickText != "" && onClickText != undefined) {
        GetActivityLogDetails("BUTTON", onClickText);
        $.ajax({
            url: '/UserActivityLog/AddActivityLoad',
            data: { onClickText: onClickText, ActionName: ActionName, ControllerName: ControllerName, ModuleName: ModuleName, PageName: PageName, LblAction: LblAction, Message: LblMessage },
            async: false,
            success: function (response) {
                return true;
            },
            error: function () {

            }
        });
    }
    return true;
});
$(Document).on("change", "input[type='checkbox']", function () {
    var details = this.getAttribute("detail");
    if (details != null && details != "" && details != undefined) {
        var onClickText = details + " " + $(this).parent().find('label').text();
    }
    else {
        var onClickText = $(this).parent().find('label').text().trim();
    }
    //alert(onClickText);
    var Value = this.checked;
    if (onClickText != null && onClickText != "" && onClickText != undefined) {
        if (Value) {
            GetActivityLogDetails("ONCHECKCHECKBOX", onClickText);
        }
        else {
            GetActivityLogDetails("ONUNCHECKCHECKBOX", onClickText);
        }
        if (ModuleName != "" && PageName != "") {

            $.ajax({
                url: '/UserActivityLog/AddActivityLoad',
                data: { onClickText: onClickText, ActionName: ActionName, ControllerName: ControllerName, ModuleName: ModuleName, PageName: PageName, LblAction: LblAction, Message: LblMessage },
                async: false,
                success: function (response) {
                    return true;
                },
                error: function () {

                }
            });
        }
    }
    return true;
});

$(Document).on('dp.change', function (e) {
    var onClickText = e.date.format('MM-DD-YYYY').trim();

    var details = e.target.attributes.detail ? e.target.attributes.detail.value : null;
    
    if (details != null && details != "" && details != undefined) {
        onClickText = details + " " + e.date.format('MM-DD-YYYY').trim();
    }

    if (onClickText != null && onClickText != "" && onClickText != undefined) {
        GetActivityLogDetails("Date", onClickText);
        $.ajax({
            url: '/UserActivityLog/AddActivityLoad',
            data: { onClickText: onClickText, ActionName: ActionName, ControllerName: ControllerName, ModuleName: ModuleName, PageName: PageName, LblAction: LblAction, Message: LblMessage },
            async: false,
            success: function (response) {
                return true;
            },
            error: function () {

            }
        });
    }
    return true;
});

$('select').on('change', function () {
    var selectedValue = $(this).val();
    var details = this.getAttribute("detail");
    if (details != null && details != undefined && details != "") {
        var selectedText = details + " " + $(this).find(':selected').text();
    }
    else {
        var selectedText = $(this).find(':selected').text().trim();
    }
    if (selectedText != null && selectedText != "" && selectedText != undefined) {
        GetActivityLogDetails("Dropdown", selectedText);
        $.ajax({
            url: '/UserActivityLog/AddActivityLoad',
            data: { onClickText: selectedText, ActionName: ActionName, ControllerName: ControllerName, ModuleName: ModuleName, PageName: PageName, LblAction: LblAction, Message: LblMessage },
            async: false,
            success: function (response) {
                return true;
            },
            error: function () {

            }
        });
    }
    return true;
});

$('input[type="radio"]').on('change', function () {
    var selectedValue = $(this).val();
    var details = this.getAttribute("detail");
    if (details != null && details != undefined && details != "") {
        var selectedText = details;
    }
    else {
        var selectedText = $('label[for="' + $(this).attr('id') + '"]').text().trim();
    }
    if (selectedText != null && selectedText != "" && selectedText != undefined) {
        GetActivityLogDetails("Radio", selectedText);
        $.ajax({
            url: '/UserActivityLog/AddActivityLoad',
            data: { onClickText: selectedText, ActionName: ActionName, ControllerName: ControllerName, ModuleName: ModuleName, PageName: PageName, LblAction: LblAction, Message: LblMessage },
            async: false,
            success: function (response) {
                return true;
            },
            error: function () {

            }
        });
    }
    return true;
});
//$("a").on("click", function () {
//    var onClickText = this.text;
//    var LinkImg = this.innerHTML;
//    if ((onClickText != null && onClickText != "" && onClickText != undefined) || (LinkImg != null && LinkImg != "" && LinkImg != undefined)) {
//        GetActivityLogDetails("CLICK", onClickText, LinkImg);
//        $.ajax({
//            url: '/ActivityLog/AddActivityLoad',
//            data: { onClickText: onClickText, ActionName: ActionName, ControllerName: ControllerName, ModuleName: ModuleName, PageName: PageName, LblAction: LblAction, Message: LblMessage },
//            async: false,
//            success: function (response) {
//                return true;
//            },
//            error: function () {

//            }
//        });
//    }
//});
//$("button").on("click", function () {
//    var onClickText = this.innerHTML;

//    if (onClickText != null && onClickText != "" && onClickText != undefined) {
//        GetActivityLogDetails("BUTTON", onClickText);
//        $.ajax({
//            url: '/ActivityLog/AddActivityLoad',
//            data: { onClickText: onClickText, ActionName: ActionName, ControllerName: ControllerName, ModuleName: ModuleName, PageName: PageName, LblAction: LblAction, Message: LblMessage },
//            async: false,
//            success: function (response) {
//                return true;
//            },
//            error: function () {

//            }
//        });
//    }
//});

//$('input:button').click(function () {
//    var onClickText = this.value;

//    if (onClickText != null && onClickText != "" && onClickText != undefined) {
//        GetActivityLogDetails("BUTTON", onClickText);
//        $.ajax({
//            url: '/ActivityLog/AddActivityLoad',
//            data: { onClickText: onClickText, ActionName: ActionName, ControllerName: ControllerName, ModuleName: ModuleName, PageName: PageName, LblAction: LblAction, Message: LblMessage },
//            async: false,
//            success: function (response) {
//                return true;
//            },
//            error: function () {

//            }
//        });
//    }
//});

//$("input[type='checkbox']").on("change", function () {
//    
//    var onClickText = $(this).parent().find('label').text();
//    //alert(onClickText);
//    var Value = this.checked;
//    if (onClickText != null && onClickText != "" && onClickText != undefined) {
//        if (Value) {
//            GetActivityLogDetails("ONCHECKCHECKBOX", onClickText);
//        }
//        else {
//            GetActivityLogDetails("ONUNCHECKCHECKBOX", onClickText);
//        }
//        if (ModuleName != "" && PageName != "") {

//            $.ajax({
//                url: '/ActivityLog/AddActivityLoad',
//                data: { onClickText: onClickText, ActionName: ActionName, ControllerName: ControllerName, ModuleName: ModuleName, PageName: PageName, LblAction: LblAction, Message: LblMessage },
//                async: false,
//                success: function (response) {
//                    return true;
//                },
//                error: function () {

//                }
//            });
//        }
//    }
//});

function GetActivityLogDetails(ActionType, Message, LinkImg = "") {
    if (ActionName == null || ActionName == undefined || ActionName == "") {
        ActionName = "index";
    }
    if (ControllerName != null && ControllerName != "" && ControllerName != undefined) {
        switch (ControllerName.toUpperCase()) {

            // Dashboard
            case "DASHBOARD":
                switch (ActionName.toUpperCase()) {
                    case "DAILY":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Dashboard";
                            PageName = "Daily Dashbord";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Dashboard";
                            PageName = "Daily Dashbord";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Dashboard";
                            PageName = "Daily Dashbord";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Dashboard";
                            PageName = "Daily Dashbord";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Dashboard";
                            PageName = "Daily Dashbord";
                            LblAction = "Date Change ";
                            LblMessage = Message + " Date Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Dashboard";
                            PageName = "Daily Dashbord";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Dashboard";
                            PageName = "Daily Dashbord";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    case "WEEKLY":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Dashboard";
                            PageName = "Weekly Dashbord";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Dashboard";
                            PageName = "Weekly Dashbord";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Dashboard";
                            PageName = "Weekly Dashbord";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Dashboard";
                            PageName = "Weekly Dashbord";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Dashboard";
                            PageName = "Weekly Dashbord";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Dashboard";
                            PageName = "Weekly Dashbord";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Dashboard";
                            PageName = "Weekly Dashbord";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    case "PERIODIC":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Dashboard";
                            PageName = "Periodic Dashbord";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Dashboard";
                            PageName = "Periodic Dashbord";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Dashboard";
                            PageName = "Periodic Dashbord";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Dashboard";
                            PageName = "Periodic Dashbord";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Dashboard";
                            PageName = "Periodic Dashbord";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Dashboard";
                            PageName = "Periodic Dashbord";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Dashboard";
                            PageName = "Periodic Dashbord";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on" + Message;
                        }
                        break;
                    case "YEARLY":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Dashboard";
                            PageName = "Yearly Dashbord";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Dashboard";
                            PageName = "Yearly Dashbord";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Dashboard";
                            PageName = "Yearly Dashbord";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Dashboard";
                            PageName = "Yearly Dashbord";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Dashboard";
                            PageName = "Yearly Dashbord";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Dashboard";
                            PageName = "Yearly Dashbord";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Dashboard";
                            PageName = "Yearly Dashbord";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on" + Message;
                        }
                        break;
                    default:
                }
                break;
            case "PRODUCTPRICE":
                switch (ActionName.toUpperCase()) {
                    case "PRODUCTPRICE":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Dashboard";
                            PageName = "Cost Price Comparison";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Dashboard";
                            PageName = "Cost Price Comparison";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Dashboard";
                            PageName = "Cost Price Comparison";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Dashboard";
                            PageName = "Cost Price Comparison";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Dashboard";
                            PageName = "Cost Price Comparison";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Dashboard";
                            PageName = "Cost Price Comparison";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Dashboard";
                            PageName = "Cost Price Comparison";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on" + Message;
                        }
                        break;
                    default:
                }
                break;
            case "SELLPRICE":
                switch (ActionName.toUpperCase()) {
                    case "SELLPRICE":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Dashboard";
                            PageName = "Sales Price Comparison";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Dashboard";
                            PageName = "Sales Price Comparison";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Dashboard";
                            PageName = "Sales Price Comparison";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Dashboard";
                            PageName = "Sales Price Comparison";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Dashboard";
                            PageName = "Sales Price Comparison";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Dashboard";
                            PageName = "Sales Price Comparison";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Dashboard";
                            PageName = "Sales Price Comparison";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on" + Message;
                        }
                        break;
                    default:
                }
                break;
            case "TOPSELLERPRICE":
                switch (ActionName.toUpperCase()) {
                    case "TOPSELLERPRICE":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Dashboard";
                            PageName = "Top seller Items";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Dashboard";
                            PageName = "Top seller Items";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Dashboard";
                            PageName = "Top seller Items";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Dashboard";
                            PageName = "Top seller Items";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Dashboard";
                            PageName = "Top seller Items";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Dashboard";
                            PageName = "Top seller Items";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Dashboard";
                            PageName = "Top seller Items";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on" + Message;
                        }
                        break;
                    default:
                }
                break;
            // Invoices
            case "INVOICES":
                switch (ActionName.toUpperCase()) {
                    case "INDEX":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Invoice";
                            PageName = "View Invoices";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Invoice";
                            PageName = "View Invoices";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Invoice";
                            PageName = "View Invoices";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Invoice";
                            PageName = "View Invoices";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                       
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Invoice";
                            PageName = "View Invoices";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        break;
                    case "EDIT":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Invoice";
                            PageName = "Edit Invoice";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Invoice";
                            PageName = "Edit Invoice";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Invoice";
                            PageName = "Edit Invoice";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Invoice";
                            PageName = "Edit Invoice";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Invoice";
                            PageName = "Edit Invoice";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Invoice";
                            PageName = "Edit Invoice";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Invoice";
                            PageName = "Edit Invoice";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    case "Details":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Invoice";
                            PageName = "Invoice";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Invoice";
                            PageName = "Invoice";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Invoice";
                            PageName = "Invoice";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Invoice";
                            PageName = "Invoice";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Invoice";
                            PageName = "Invoice";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Invoice";
                            PageName = "Invoice";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Invoice";
                            PageName = "Invoice";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    case "INDEXBETA":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Invoice";
                            PageName = "View Invoices";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Invoice";
                            PageName = "View Invoices";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Invoice";
                            PageName = "View Invoices";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Invoice";
                            PageName = "View Invoices";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Invoice";
                            PageName = "View Invoices";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Invoice";
                            PageName = "View Invoices";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Invoice";
                            PageName = "View Invoices";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    case "CREATE":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Invoice";
                            PageName = "Add Invoice";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Invoice";
                            PageName = "Add Invoice";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Invoice";
                            PageName = "Add Invoice";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Invoice";
                            PageName = "Add Invoice";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Invoice";
                            PageName = "Add Invoice";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Invoice";
                            PageName = "Add Invoice";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Invoice";
                            PageName = "Add Invoice";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    case "CREATESPLITINVOICE":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Invoice";
                            PageName = "Add Split Invoice";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Invoice";
                            PageName = "Add Split Invoice";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Invoice";
                            PageName = "Add Split Invoice";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Invoice";
                            PageName = "Add Split Invoice";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Invoice";
                            PageName = "Add Split Invoice";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Invoice";
                            PageName = "Add Split Invoice";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Invoice";
                            PageName = "Add Split Invoice";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    case "VIEWINVOICE":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Invoice";
                            PageName = "Invoice File Read";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Invoice";
                            PageName = "Invoice File Read";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Invoice";
                            PageName = "Invoice File Read";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Invoice";
                            PageName = "Invoice File Read";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Invoice";
                            PageName = "Invoice File Read";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Invoice";
                            PageName = "Invoice File Read";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Invoice";
                            PageName = "Invoice File Read";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;
            case "BULKUPLOADFILE":
                switch (ActionName.toUpperCase()) {
                    case "INDEX":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Invoice";
                            PageName = "Add Bulk Invoices";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Invoice";
                            PageName = "Add Bulk Invoices";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Invoice";
                            PageName = "Add Bulk Invoices";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Invoice";
                            PageName = "Add Bulk Invoices";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Invoice";
                            PageName = "Add Bulk Invoices";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Invoice";
                            PageName = "Add Bulk Invoices";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Invoice";
                            PageName = "Add Bulk Invoices";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;


            // Inventory
            case "PRODUCTMAPPINGS":
                switch (ActionName.toUpperCase()) {
                    case "IMPORTPRODUCTEXCEL":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Inventory";
                            PageName = "Item Library";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Inventory";
                            PageName = "Item Library";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Inventory";
                            PageName = "Item Library";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Inventory";
                            PageName = "Item Library";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Inventory";
                            PageName = "Item Library";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Inventory";
                            PageName = "Item Library";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Inventory";
                            PageName = "Item Library";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;
            case "INVOICEPREVIEW":
                switch (ActionName.toUpperCase()) {
                    case "INDEX":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Inventory";
                            PageName = "Vendor Product Lists";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Inventory";
                            PageName = "Vendor Product Lists";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Inventory";
                            PageName = "Vendor Product Lists";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Inventory";
                            PageName = "Vendor Product Lists";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Inventory";
                            PageName = "Vendor Product Lists";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Inventory";
                            PageName = "Vendor Product Lists";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Inventory";
                            PageName = "Vendor Product Lists";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;
            case "LINEITEM":
                switch (ActionName.toUpperCase()) {
                    case "INDEX":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Inventory";
                            PageName = "Line Items";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Inventory";
                            PageName = "Line Items";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Inventory";
                            PageName = "Line Items";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Inventory";
                            PageName = "Line Items";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Inventory";
                            PageName = "Line Items";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Inventory";
                            PageName = "Line Items";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Inventory";
                            PageName = "Line Items";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;
            case "ITEMMOVEMENT":
                switch (ActionName.toUpperCase()) {
                    case "INDEX":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Inventory";
                            PageName = "Items Movement Reports";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Inventory";
                            PageName = "Items Movement Reports";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Inventory";
                            PageName = "Items Movement Reports";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Inventory";
                            PageName = "Items Movement Reports";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Inventory";
                            PageName = "Items Movement Reports";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Inventory";
                            PageName = "Items Movement Reports";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Inventory";
                            PageName = "Items Movement Reports";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;

            // Expense
            case "EXPENSEACCOUNTS":
                switch (ActionName.toUpperCase()) {
                    case "EXPENSECHECKINDEXNEW":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Expense";
                            PageName = "Expenses";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Expense";
                            PageName = "Expenses";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Expense";
                            PageName = "Expenses";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Expense";
                            PageName = "Expenses";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Expense";
                            PageName = "Expenses";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Expense";
                            PageName = "Expenses";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Expense";
                            PageName = "Expenses";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;
            case "EXPENSEWEEKLYSETTING":
                switch (ActionName.toUpperCase()) {
                    case "HOMEEXPENSEINDEXNEW":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Expense";
                            PageName = "Home Some Expenses";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Expense";
                            PageName = "Home Some Expenses";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Expense";
                            PageName = "Home Some Expenses";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Expense";
                            PageName = "Home Some Expenses";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Expense";
                            PageName = "Home Some Expenses";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Expense";
                            PageName = "Home Some Expenses";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Expense";
                            PageName = "Home Some Expenses";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;
            case "CHECKLISTEXPENSE":
                switch (ActionName.toUpperCase()) {
                    case "CHECKLISTINDEXNEW":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Expense";
                            PageName = "Uncleared Checks";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Expense";
                            PageName = "Uncleared Checks";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Expense";
                            PageName = "Uncleared Checks";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Expense";
                            PageName = "Uncleared Checks";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Expense";
                            PageName = "Uncleared Checks";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Expense";
                            PageName = "Uncleared Checks";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Expense";
                            PageName = "Uncleared Checks";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;

            // Payroll
            case "PDFREAD":
                switch (ActionName.toUpperCase()) {
                    case "CREATE":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "HR";
                            PageName = "Payroll Files";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "HR";
                            PageName = "Payroll Files";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "HR";
                            PageName = "Payroll Files";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "HR";
                            PageName = "Payroll Files";
                            LblActionLblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "HR";
                            PageName = "Payroll Files";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "HR";
                            PageName = "Payroll Files";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "HR";
                            PageName = "Payroll Files";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;
            case "MANAGEIPADDRESS":
                switch (ActionName.toUpperCase()) {
                    case "USERTIMEHOURSE":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "HR";
                            PageName = "Employee Timecards";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "HR";
                            PageName = "Employee Timecards";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "HR";
                            PageName = "Employee Timecards";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "HR";
                            PageName = "Employee Timecards";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "HR";
                            PageName = "Employee Timecards";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "HR";
                            PageName = "Employee Timecards";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "HR";
                            PageName = "Employee Timecards";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;

            //Reports
            case "REPORT":
                switch (ActionName.toUpperCase()) {
                    case "SALESSUMMARYREPORT":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Report";
                            PageName = "Sales Summary Report";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Report";
                            PageName = "Sales Summary Report";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Report";
                            PageName = "Sales Summary Report";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Report";
                            PageName = "Sales Summary Report";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Report";
                            PageName = "Sales Summary Report";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Report";
                            PageName = "Sales Summary Report";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Report";
                            PageName = "Sales Summary Report";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    case "OPERATINGRATIOREPORT":
                        if (ActionType.toUpperCase() == "CLICK") {
                            var startdate = $('#txtstartdate').val();
                            var enddate = $('#txtenddate').val();
                            var DrpShift = $('#select2-ShiftId-container :selected').text();
                            if (LinkImg.includes("print")) {

                                if (startdate != "" || enddate != "" || DrpShift != "All Shift") {
                                    LblMessage = "Operating Ratios Report gets Printed for " + (startdate == "" ? "" : "From Date : ") + startdate + (startdate == "" ? "" : " - To Date : ") + enddate + (DrpShift == "" ? "" : " Shift : " + DrpShift);
                                }
                                else {
                                    LblMessage = "Operating Ratios Report gets Printed"
                                }
                                LblAction = "Print";
                            }
                            else if (LinkImg.includes("pdf")) {
                                if (startdate != "" || enddate != "" || DrpShift != "All Shift") {
                                    LblMessage = "Operating Ratios Report based on it PDF is Generated for " + (startdate == "" ? "" : "From Date : ") + startdate + (startdate == "" ? "" : " - To Date : ") + enddate + (DrpShift == "" ? "" : " Shift : " + DrpShift);
                                }
                                else {
                                    LblMessage = "Operating Ratios Report based on it PDF is Generated"
                                }
                                LblAction = "PDF";
                            }
                            else if (LinkImg.includes("excel")) {
                                if (startdate != "" || enddate != "" || DrpShift != "All Shift") {
                                    LblMessage = "Operating Ratios Report based on it excel is Generated for " + (startdate == "" ? "" : "From Date : ") + startdate + (startdate == "" ? "" : " - To Date : ") + enddate + (DrpShift == "" ? "" : " Shift : " + DrpShift);
                                }
                                else {
                                    LblMessage = "Operating Ratios Report based on it excel is Generated"
                                }
                                LblAction = "Export";
                            }
                            else {
                                LblMessage = "Click on " + Message;
                                LblAction = "Click";
                            }

                            ModuleName = "Report";
                            PageName = "Operating Ratios Report";
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Report";
                            PageName = "Operating Ratios Report";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Report";
                            PageName = "Operating Ratios Report";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            var startdate = $('#txtstartdate').val();
                            var enddate = $('#txtenddate').val();
                            var DrpShift = $('#select2-ShiftId-container :selected').text();
                            if (LinkImg.includes("print")) {

                                if (startdate != "" || enddate != "" || DrpShift != "All Shift") {
                                    LblMessage = "Operating Ratios Report gets Printed for " + (startdate == "" ? "" : "From Date : ") + startdate + (startdate == "" ? "" : " - To Date : ") + enddate + (DrpShift == "" ? "" : " Shift : " + DrpShift);
                                }
                                else {
                                    LblMessage = "Operating Ratios Report gets Printed"
                                }
                                LblAction = "Print";
                            }
                            else if (LinkImg.includes("pdf")) {
                                if (startdate != "" || enddate != "" || DrpShift != "All Shift") {
                                    LblMessage = "Operating Ratios Report based on it PDF is Generated for " + (startdate == "" ? "" : "From Date : ") + startdate + (startdate == "" ? "" : " - To Date : ") + enddate + + (DrpShift == "" ? "" : " Shift : " + DrpShift);
                                }
                                else {
                                    LblMessage = "Operating Ratios Report based on it PDF is Generated"
                                }
                                LblAction = "PDF";
                            }
                            else if (LinkImg.includes("excel")) {
                                if (startdate != "" || enddate != "" || DrpShift != "All Shift") {
                                    LblMessage = "Operating Ratios Report based on it excel is Generated for " + (startdate == "" ? "" : "From Date : ") + startdate + (startdate == "" ? "" : " - To Date : ") + enddate + + (DrpShift == "" ? "" : " Shift : " + DrpShift);
                                }
                                else {
                                    LblMessage = "Operating Ratios Report based on it excel is Generated"
                                }
                                LblAction = "Export";
                            }
                            else {
                                LblMessage = "Click on " + Message;
                                LblAction = "Click";
                            }

                            ModuleName = "Report";
                            PageName = "Operating Ratios Report";
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Report";
                            PageName = "Operating Ratios Report";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Report";
                            PageName = "Operating Ratios Report";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Report";
                            PageName = "Operating Ratios Report";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    case "PAYROLLANALYSISREPORT":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Report";
                            PageName = "Payroll Analysis Report";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Report";
                            PageName = "Payroll Analysis Report";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Report";
                            PageName = "Payroll Analysis Report";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Report";
                            PageName = "Payroll Analysis Report";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Report";
                            PageName = "Payroll Analysis Report";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Report";
                            PageName = "Payroll Analysis Report";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Report";
                            PageName = "Payroll Analysis Report";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;

                    case "DAILYPOSFEEDS": //For Registers
                        if (ActionType.toUpperCase() == "CLICK") {
                            var startdate = $('#txtstartdate').val();
                            var enddate = $('#txtenddate').val();
                            var DrpShift = $('#DrpLstShift :selected').text();
                            if (LinkImg.includes("print")) {

                                if (startdate != "" || enddate != "" || DrpShift != "All Shift") {
                                    LblMessage = "Daily POS Feeds gets Printed for " + (startdate == "" ? "" : "From Date : ") + startdate + (startdate == "" ? "" : " - To Date : ") + enddate + (DrpShift == "" ? "" : " Shift : " + DrpShift);
                                }
                                else {
                                    LblMessage = "Daily POS Feeds gets Printed"
                                }
                                LblAction = "Print";
                            }
                            else if (LinkImg.includes("pdf")) {
                                if (startdate != "" || enddate != "" || DrpShift != "All Shift") {
                                    LblMessage = "Daily POS Feeds based on it PDF is Generated for " + (startdate == "" ? "" : "From Date : ") + startdate + (startdate == "" ? "" : " - To Date : ") + enddate + (DrpShift == "" ? "" : " Shift : " + DrpShift);
                                }
                                else {
                                    LblMessage = "Daily POS Feeds based on it PDF is Generated"
                                }
                                LblAction = "PDF";
                            }
                            else if (LinkImg.includes("excel")) {
                                if (startdate != "" || enddate != "" || DrpShift != "All Shift") {
                                    LblMessage = "Daily POS Feeds based on it excel is Generated for " + (startdate == "" ? "" : "From Date : ") + startdate + (startdate == "" ? "" : " - To Date : ") + enddate + (DrpShift == "" ? "" : " Shift : " + DrpShift);
                                }
                                else {
                                    LblMessage = "Daily POS Feeds based on it excel is Generated"
                                }
                                LblAction = "Export";
                            }
                            else {
                                LblMessage = "Click on " + Message;
                                LblAction = "Click";
                            }
                            ModuleName = "Registers";
                            PageName = "Daily POS Feeds";
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Registers";
                            PageName = "Daily POS Feeds";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Registers";
                            PageName = "Daily POS Feeds";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Registers";
                            PageName = "Daily POS Feeds";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Registers";
                            PageName = "Daily POS Feeds";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Registers";
                            PageName = "Daily POS Feeds";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Registers";
                            PageName = "Daily POS Feeds";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    case "PAYROLLEXPENSE": //For Registers
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "HR";
                            PageName = "Payroll Expense";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "HR";
                            PageName = "Payroll Expense";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "HR";
                            PageName = "Payroll Expense";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "HR";
                            PageName = "Payroll Expense";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "HR";
                            PageName = "Payroll Expense";
                            LblAction = "Date Change";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "HR";
                            PageName = "Payroll Expense";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "HR";
                            PageName = "Payroll Expense";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;
            case "INVOICEREPORT":
                switch (ActionName.toUpperCase()) {
                    case "INDEX":
                        if (ActionType.toUpperCase() == "CLICK") {
                            var payment = $('input[name="radio"]:checked').val();
                            var startdate = $('#txtstartdate').val();
                            var enddate = $('#txtenddate').val();
                            var DrpLstdept = $('#DrpLstdept :selected').text();
                            var Drpstatus = $('#Drpstatus :selected').text();
                            var VendorName = $('#VendorName :selected').text();
                            var AmtMinimum = $('#AmtMinimum').val();
                            var AmtMaximum = $('#AmtMaximum').val();
                            if (LinkImg.includes("print")) {

                                if (startdate != "" || enddate != "" || payment != undefined || DrpLstdept != "Select Department" || Drpstatus != "Select Status" || VendorName != "Select Vendor Name" || AmtMinimum != 0 || AmtMaximum != 0) {
                                    LblMessage = "COGS/Bills Report gets Printed for " + (startdate == "" ? "" : "From Date : ") + startdate + (startdate == "" ? "" : " - To Date : ") + enddate + (payment == "" ? "" : " ,Payment Method : ") + payment + (DrpLstdept == "Select Department" ? "" : " , Departments : " + DrpLstdept) + (Drpstatus == "Select Status" ? "" : " ,Status : " + Drpstatus) + (VendorName == "Select Vendor Name" ? "" : " ,Vendor Name : " + VendorName) + (AmtMinimum == 0 ? "" : " ,Min Amount : ") + (AmtMinimum == 0 ? "" : (AmtMinimum)) + (AmtMaximum == 0 ? "" : " ,Max Amount : ") + (AmtMaximum == 0 ? "" : (AmtMaximum));
                                }
                                else {
                                    LblMessage = "COGS/Bills Report gets Printed"
                                }
                                LblAction = "Print";
                            }
                            else if (LinkImg.includes("pdf")) {
                                if (startdate != "" || enddate != "" || payment != undefined || DrpLstdept != "Select Department" || Drpstatus != "Select Status" || VendorName != "Select Vendor Name" || AmtMinimum != 0 || AmtMaximum != 0) {
                                    LblMessage = "COGS/Bills Report based on it PDF is Generated for " + (startdate == "" ? "" : "From Date : ") + startdate + (startdate == "" ? "" : " - To Date : ") + enddate + (payment == "" ? "" : " ,Payment Method : ") + payment + (DrpLstdept == "Select Department" ? "" : " , Departments : " + DrpLstdept) + (Drpstatus == "Select Status" ? "" : " ,Status : " + Drpstatus) + (VendorName == "Select Vendor Name" ? "" : " ,Vendor Name : " + VendorName) + (AmtMinimum == 0 ? "" : " ,Min Amount : ") + (AmtMinimum == 0 ? "" : (AmtMinimum)) + (AmtMaximum == 0 ? "" : " ,Max Amount : ") + (AmtMaximum == 0 ? "" : (AmtMaximum));
                                }
                                else {
                                    LblMessage = "COGS/Bills Report based on it PDF is Generated"
                                }
                                LblAction = "PDF";
                            }
                            else if (LinkImg.includes("excel")) {
                                if (startdate != "" || enddate != "" || payment != undefined || DrpLstdept != "Select Department" || Drpstatus != "Select Status" || VendorName != "Select Vendor Name" || AmtMinimum != 0 || AmtMaximum != 0) {
                                    LblMessage = "COGS/Bills Report based on it excel is Generated for " + (startdate == "" ? "" : "From Date : ") + startdate + (startdate == "" ? "" : " - To Date : ") + enddate + (payment == "" ? "" : " ,Payment Method : ") + payment + (DrpLstdept == "Select Department" ? "" : " , Departments : " + DrpLstdept) + (Drpstatus == "Select Status" ? "" : " ,Status : " + Drpstatus) + (VendorName == "Select Vendor Name" ? "" : " ,Vendor Name : " + VendorName) + (AmtMinimum == 0 ? "" : " ,Min Amount : ") + (AmtMinimum == 0 ? "" : (AmtMinimum)) + (AmtMaximum == 0 ? "" : " ,Max Amount : ") + (AmtMaximum == 0 ? "" : (AmtMaximum));
                                }
                                else {
                                    LblMessage = "COGS/Bills Report based on it excel is Generated"
                                }
                                LblAction = "Export";
                            }
                            else {
                                LblMessage = "Click on " + Message;
                                LblAction = "Click";
                            }
                            ModuleName = "Report";
                            PageName = "COGS/Bills Report";
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Report";
                            PageName = "COGS/Bills Report";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Report";
                            PageName = "COGS/Bills Report";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Report";
                            PageName = "COGS/Bills Report";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Report";
                            PageName = "COGS/Bills Report";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Report";
                            PageName = "COGS/Bills Report";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Report";
                            PageName = "COGS/Bills Report";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;
            case "EXPENSEREPORT":
                switch (ActionName.toUpperCase()) {
                    case "INDEX":
                        if (ActionType.toUpperCase() == "CLICK") {
                            var payment = $('input[name="radio"]:checked').val();
                            var startdate = $('#txtstartdate').val();
                            var enddate = $('#txtenddate').val();
                            var DrpLstdept = $('#DrpLstdept :selected').text();
                            var Drpstatus = $('#Drpstatus :selected').text();
                            var VendorName = $('#VendorName :selected').text();
                            var AmtMinimum = $('#AmtMinimum').val();
                            var AmtMaximum = $('#AmtMaximum').val();
                            if (LinkImg.includes("print")) {

                                if (startdate != "" || enddate != "" || payment != undefined || DrpLstdept != "Select Department" || Drpstatus != "Select Status" || VendorName != "Select Vendor Name" || AmtMinimum != 0 || AmtMaximum != 0) {
                                    LblMessage = "Expense Report gets Printed for " + (startdate == "" ? "" : "From Date : ") + startdate + (startdate == "" ? "" : " - To Date : ") + enddate + (payment == "" ? "" : " ,Payment Method : ") + payment + (DrpLstdept == "Select Department" ? "" : " , Departments : " + DrpLstdept) + (Drpstatus == "Select Status" ? "" : " ,Status : " + Drpstatus) + (VendorName == "Select Vendor Name" ? "" : " ,Vendor Name : " + VendorName) + (AmtMinimum == 0 ? "" : " ,Min Amount : ") + (AmtMinimum == 0 ? "" : (AmtMinimum)) + (AmtMaximum == 0 ? "" : " ,Max Amount : ") + (AmtMaximum == 0 ? "" : (AmtMaximum));
                                }
                                else {
                                    LblMessage = "Expense Report gets Printed"
                                }
                                LblAction = "Print";
                            }
                            else if (LinkImg.includes("pdf")) {
                                if (startdate != "" || enddate != "" || payment != undefined || DrpLstdept != "Select Department" || Drpstatus != "Select Status" || VendorName != "Select Vendor Name" || AmtMinimum != 0 || AmtMaximum != 0) {
                                    LblMessage = "Expense Report based on it PDF is Generated for " + (startdate == "" ? "" : "From Date : ") + startdate + (startdate == "" ? "" : " - To Date : ") + enddate + (payment == "" ? "" : " ,Payment Method : ") + payment + (DrpLstdept == "Select Department" ? "" : " , Departments : " + DrpLstdept) + (Drpstatus == "Select Status" ? "" : " ,Status : " + Drpstatus) + (VendorName == "Select Vendor Name" ? "" : " ,Vendor Name : " + VendorName) + (AmtMinimum == 0 ? "" : " ,Min Amount : ") + (AmtMinimum == 0 ? "" : (AmtMinimum)) + (AmtMaximum == 0 ? "" : " ,Max Amount : ") + (AmtMaximum == 0 ? "" : (AmtMaximum));
                                }
                                else {
                                    LblMessage = "Expense Report based on it PDF is Generated"
                                }
                                LblAction = "PDF";
                            }
                            else if (LinkImg.includes("excel")) {
                                if (startdate != "" || enddate != "" || payment != undefined || DrpLstdept != "Select Department" || Drpstatus != "Select Status" || VendorName != "Select Vendor Name" || AmtMinimum != 0 || AmtMaximum != 0) {
                                    LblMessage = "Expense Report based on it excel is Generated for " + (startdate == "" ? "" : "From Date : ") + startdate + (startdate == "" ? "" : " - To Date : ") + enddate + (payment == "" ? "" : " ,Payment Method : ") + payment + (DrpLstdept == "Select Department" ? "" : " , Departments : " + DrpLstdept) + (Drpstatus == "Select Status" ? "" : " ,Status : " + Drpstatus) + (VendorName == "Select Vendor Name" ? "" : " ,Vendor Name : " + VendorName) + (AmtMinimum == 0 ? "" : " ,Min Amount : ") + (AmtMinimum == 0 ? "" : (AmtMinimum)) + (AmtMaximum == 0 ? "" : " ,Max Amount : ") + (AmtMaximum == 0 ? "" : (AmtMaximum));
                                }
                                else {
                                    LblMessage = "Expense Report based on it excel is Generated"
                                }
                                LblAction = "Export";
                            }
                            else {
                                LblMessage = "Click on " + Message;
                                LblAction = "Click";
                            }

                            ModuleName = "Report";
                            PageName = "Expense Report";
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Report";
                            PageName = "Expense Report";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Report";
                            PageName = "Expense Report";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Report";
                            PageName = "Expense Report";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Report";
                            PageName = "Expense Report";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Report";
                            PageName = "Expense Report";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Report";
                            PageName = "Expense Report";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;

            //QuickBooks
            case "QBCONFIGURATION":
                switch (ActionName.toUpperCase()) {
                    case "QBSYNCONLINEDATA":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Quickboooks";
                            PageName = "QuickBook Sync Start/Stop";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Quickboooks";
                            PageName = "QuickBook Sync Start/Stop";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Quickboooks";
                            PageName = "QuickBook Sync Start/Stop";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Quickboooks";
                            PageName = "QuickBook Sync Start/Stop";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Quickboooks";
                            PageName = "QuickBook Sync Start/Stop";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Quickboooks";
                            PageName = "QuickBook Sync Start/Stop";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Quickboooks";
                            PageName = "QuickBook Sync Start/Stop";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    case "INDEX":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Quickboooks";
                            PageName = "Quickbooks Configuration";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Quickboooks";
                            PageName = "Quickbooks Configuration";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Quickboooks";
                            PageName = "Quickbooks Configuration";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Quickboooks";
                            PageName = "Quickbooks Configuration";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Quickboooks";
                            PageName = "Quickbooks Configuration";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Quickboooks";
                            PageName = "Quickbooks Configuration";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Quickboooks";
                            PageName = "Quickbooks Configuration";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;
            case "VENDORMASTERS":
                switch (ActionName.toUpperCase()) {
                    case "INDEX":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Quickbooks";
                            PageName = "Vendors";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Quickbooks";
                            PageName = "Vendors";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Quickbooks";
                            PageName = "Vendors";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Quickbooks";
                            PageName = "Vendors";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Quickboooks";
                            PageName = "Vendors";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Quickboooks";
                            PageName = "Vendors";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Quickboooks";
                            PageName = "Vendors";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    case "MERGEVENDOR":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Quickbooks";
                            PageName = "Merge Vendor";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Quickbooks";
                            PageName = "Merge Vendor";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Quickbooks";
                            PageName = "Merge Vendor";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Quickbooks";
                            PageName = "Merge Vendor";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Quickboooks";
                            PageName = "Merge Vendor";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Quickboooks";
                            PageName = "Merge Vendor";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Quickboooks";
                            PageName = "Merge Vendor";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;
            case "CHARTOFACCOUNTS":
                switch (ActionName.toUpperCase()) {
                    case "INDEX":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Quickbooks";
                            PageName = "Departments";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Quickbooks";
                            PageName = "Departments";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Quickbooks";
                            PageName = "Departments";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Quickbooks";
                            PageName = "Departments";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Quickboooks";
                            PageName = "Departments";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Quickboooks";
                            PageName = "Departments";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Quickboooks";
                            PageName = "Departments";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;


            // Registers
            case "TERMINAL":
                switch (ActionName.toUpperCase()) {
                    case "INDEX":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Registers";
                            PageName = "Shifts/Registers Close Out";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Registers";
                            PageName = "Shifts/Registers Close Out";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Registers";
                            PageName = "Shifts/Registers Close Out";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Registers";
                            PageName = "Shifts/Registers Close Out";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Registers";
                            PageName = "Shifts/Registers Close Out";
                            LblAction = "Date Change";
                            LblMessage = Message + " Date Filtered In " + PageName
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Registers";
                            PageName = "Shifts/Registers Close Out";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Registers";
                            PageName = "Shifts/Registers Close Out";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;
            case "JOURNALENTRIES":
                switch (ActionName.toUpperCase()) {
                    case "INDEX":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Registers";
                            PageName = "Journal Entries";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Registers";
                            PageName = "Journal Entries";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Registers";
                            PageName = "Journal Entries";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Registers";
                            PageName = "Journal Entries";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Registers";
                            PageName = "Journal Entries";
                            LblAction = "Date Change";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Registers";
                            PageName = "Journal Entries";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Registers";
                            PageName = "Journal Entries";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    case "DETAIL":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Registers";
                            PageName = "Journal Entries";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Registers";
                            PageName = "Journal Entries";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Registers";
                            PageName = "Journal Entries";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Registers";
                            PageName = "Journal Entries";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Registers";
                            PageName = "Journal Entries";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Registers";
                            PageName = "Journal Entries";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Registers";
                            PageName = "Journal Entries";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;
            case "DELETEHOURLYFILES":
                switch (ActionName.toUpperCase()) {
                    case "INDEX":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "Registers";
                            PageName = "Hourly POS feeds";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "Registers";
                            PageName = "Hourly POS feeds";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "Registers";
                            PageName = "Hourly POS feeds";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "Registers";
                            PageName = "Hourly POS feeds";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "Registers";
                            PageName = "Hourly POS feeds";
                            LblAction = "Date Change";
                            LblMessage = Message + " Date Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "Registers";
                            PageName = "Hourly POS feeds";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "Registers";
                            PageName = "Hourly POS feeds";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;

            // HR Module
            case "HREMPLOYEE":
                switch (ActionName.toUpperCase()) {
                    case "INDEX":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "HR";
                            PageName = "Employees List";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "HR";
                            PageName = "Employees List";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "HR";
                            PageName = "Employees List";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "HR";
                            PageName = "Employees List";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "HR";
                            PageName = "Employees List";
                            LblAction = "Date Change";
                            LblMessage = Message + " Date Filtered In " + PageName
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "HR";
                            PageName = "Employees List";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "HR";
                            PageName = "Employees List";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }                        
                        break;
                    default:
                }
                break;

            case "HRCONSENTMASTERS":
                switch (ActionName.toUpperCase()) {
                    case "INDEX":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "HR";
                            PageName = "Consent Status";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "HR";
                            PageName = "Consent Status";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "HR";
                            PageName = "Consent Status";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "HR";
                            PageName = "Consent Status";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "HR";
                            PageName = "Consent Status";
                            LblAction = "Date Change";
                            LblMessage = Message + " Date Filtered In " + PageName
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "HR";
                            PageName = "Consent Status";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "HR";
                            PageName = "Consent Status";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;

            case "HRSTOREMANAGERS":
                switch (ActionName.toUpperCase()) {
                    case "INDEX":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "HR";
                            PageName = "Mobile App User";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "HR";
                            PageName = "Mobile App User";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "HR";
                            PageName = "Mobile App User";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "HR";
                            PageName = "Mobile App User";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "HR";
                            PageName = "Mobile App User";
                            LblAction = "Date Change";
                            LblMessage = Message + " Date Filtered In " + PageName
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "HR";
                            PageName = "Mobile App User";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "HR";
                            PageName = "Mobile App User";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;

            case "HRDEPARTMENTMASTERS":
                switch (ActionName.toUpperCase()) {
                    case "INDEX":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "HR";
                            PageName = "Store Departments";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "HR";
                            PageName = "Store Departments";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "HR";
                            PageName = "Store Departments";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "HR";
                            PageName = "Store Departments";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "HR";
                            PageName = "Store Departments";
                            LblAction = "Date Change";
                            LblMessage = Message + " Date Filtered In " + PageName
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "HR";
                            PageName = "Store Departments";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "HR";
                            PageName = "Store Departments";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;

            // Documents
            case "DOCUMENTS":
                switch (ActionName.toUpperCase()) {
                    case "INDEX":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "My Documents";
                            PageName = "My Documents";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "My Documents";
                            PageName = "My Documents";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "My Documents";
                            PageName = "My Documents";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "My Documents";
                            PageName = "My Documents";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "My Documents";
                            PageName = "My Documents";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "My Documents";
                            PageName = "My Documents";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "My Documents";
                            PageName = "My Documents";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    case "EDIT":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "My Documents";
                            PageName = "My Documents";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "My Documents";
                            PageName = "My Documents";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "My Documents";
                            PageName = "My Documents";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "My Documents";
                            PageName = "My Documents";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "My Documents";
                            PageName = "My Documents";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "My Documents";
                            PageName = "My Documents";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "My Documents";
                            PageName = "My Documents";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;
            default:

            // CRM
            case "CUSTOMERINFORMATION":
                switch (ActionName.toUpperCase()) {
                    case "INDEX":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "CRM";
                            PageName = "Customers Information";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "CRM";
                            PageName = "Customers Information";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "CRM";
                            PageName = "Customers Information";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "CRM";
                            PageName = "Customers Information";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "CRM";
                            PageName = "Customers Information";
                            LblAction = "Date Change";
                            LblMessage = Message + " Date Filtered In " + PageName
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "CRM";
                            PageName = "Customers Information";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "CRM";
                            PageName = "Customers Information";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    case "CUSTOMERRECIPT":
                        if (ActionType.toUpperCase() == "CLICK") {
                            ModuleName = "CRM";
                            PageName = "Customers Receipts";
                            LblAction = "Click";
                            LblMessage = "Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "ONCHECKCHECKBOX") {
                            ModuleName = "CRM";
                            PageName = "Customers Receipts";
                            LblAction = "Check";
                            LblMessage = Message + " selected for Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "ONUNCHECKCHECKBOX") {
                            ModuleName = "CRM";
                            PageName = "Customers Receipts";
                            LblAction = "Uncheck";
                            LblMessage = Message + " Remove from Data Filter In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "BUTTON") {
                            ModuleName = "CRM";
                            PageName = "Customers Receipts";
                            LblAction = "Button Click";
                            LblMessage = "Button Click on " + Message;
                        }
                        if (ActionType.toUpperCase() == "DATE") {
                            ModuleName = "CRM";
                            PageName = "Customers Receipts";
                            LblAction = "Date Change";
                            LblMessage = "Date change to " + Message;
                        }
                        if (ActionType.toUpperCase() == "DROPDOWN") {
                            ModuleName = "CRM";
                            PageName = "Customers Receipts";
                            LblAction = "Dropdown Change";
                            LblMessage = Message + " Dropdown value Filtered In " + PageName;
                        }
                        if (ActionType.toUpperCase() == "RADIO") {
                            ModuleName = "CRM";
                            PageName = "Customers Receipts";
                            LblAction = "Radio button Change";
                            LblMessage = "Clicked on " + Message;
                        }
                        break;
                    default:
                }
                break;
        }
    }
}

function formatDate(date) {
    var mm = date.getMonth() + 1;
    var dd = date.getDate();
    var yyyy = date.getFullYear();

    return mm + '/' + dd + '/' + yyyy;
}