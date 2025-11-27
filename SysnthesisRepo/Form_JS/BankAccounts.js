function ComfirmDelete(ID) {
    var DivId = "#" + ID + "D";
    $(DivId).show();
}

function Delete(ID) {
    $.ajax({
        url: ROOTURL + 'BankAccounts/Delete',
        data: { Id: ID },
        async: false,
        success: function (response) {

            if (response === deletes) {
                MyCustomToster(response);
                $('.divIDClass').css('display', 'none');
                window.location.href = ROOTURL + "BankAccounts/Index";
            }
            else if (response != "") {
                alert(response);
                $(".divIDClass").hide();
            }

            return true;
        },
        error: function () {
            Loader(0);
        }
    });
}

$(document).ready(function () {
    var StoreID = StoreIDValue;
    if (StoreID == 0) {
        $('#popupStoreAlertValue').show();
    }
});

function closemodal() {
    $(".divIDClass").hide();
}

function BankAccouneDetailAdd() {

    $(".ItemIDError").text("");
    $(".AccessTokenError").text("");
    var ItemID = $('#ItemID').val();
    var AccessToken = $('#AccessToken').val();

    if (ItemID == "") {
        $(".ItemIDError").text(ItemRequred);
        return false;
    }
    if (AccessToken == "") {
        $(".AccessTokenError").text(AccessRequired);
        return false;
    }
    $.ajax({
        url: '/BankAccounts/AddBankAccountDetail',
        type: "post",
        cache: false,
        data: { ItemID: ItemID, AccessToken: AccessToken },
        success: function (states) {
            if (states == "Success") {
                window.location.href = ROOTURL + "BankAccounts/Index";
            }
            else if (states == "ErrorStore") {
                $('#popupStoreAlertValue').show();
            }
            else {
                toastr.options = {
                    "closeButton": true,
                    "debug": false,
                    "newestOnTop": false,
                    "progressBar": false,
                    "positionClass": "toast-top-right",
                    "preventDuplicates": false,
                    "onclick": null,
                    "showDuration": "3000",
                    "hideDuration": "1000",
                    "timeOut": "5000",
                    "extendedTimeOut": "10000",
                    "showEasing": "swing",
                    "hideEasing": "linear",
                    "showMethod": "fadeIn",
                    "hideMethod": "fadeOut"
                }

                toastr.error('Something went to wrong!');
            }
        }
    });
}