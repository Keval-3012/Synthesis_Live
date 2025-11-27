var currentDate = '';
var currentTime = '';
var obj = new ecwEncoder();

var languagePrefrence = ''; /*sessionStorage.getItem('languagePrefrence');*/

var nextButtonText;
var submitButtonText;
var downloadCertificateText;
var TakeTestAgainText;
var supposedCurrentTime = 0;

function setTraning(slanguagePrefrence, sfirstName, slastName, suserid, susername, slastSlideName) {
       
    sessionStorage.setItem('firstName', sfirstName);
    sessionStorage.setItem('lastName', slastName);
    sessionStorage.setItem('languagePrefrence', slanguagePrefrence);
    sessionStorage.setItem('userid', suserid);
    sessionStorage.setItem('username', susername);
    sessionStorage.setItem('lastSlideName', slastSlideName);

    firstName = sessionStorage.getItem('firstName');
    lastName = sessionStorage.getItem('lastName');
    additionalLastName = sessionStorage.getItem('lastName');
    languagePrefrence = sessionStorage.getItem('languagePrefrence');
    userid = sessionStorage.getItem('userid');
    username = sessionStorage.getItem('username');
    if (languagePrefrence == "2") {
        $('.Segment1').attr("src", "../Traning/Spanish/Segment1.mp4");
        $('.Segment111').attr("src", "../Traning/Spanish/Segment111.mp4");
        $('.Segment112').attr("src", "../Traning/Spanish/Segment112.mp4");
        $('.Segment2').attr("src", "../Traning/Spanish/Segment2.mp4");
        $('.Segment3').attr("src", "../Traning/Spanish/Segment3.mp4");
        $('.Segment4').attr("src", "../Traning/Spanish/Segment4.mp4");
        $('.Segment5').attr("src", "../Traning/Spanish/Segment5.mp4");
        $('.Segment6').attr("src", "../Traning/Spanish/Segment6.mp4");
        $('.Segment7').attr("src", "../Traning/Spanish/Segment7.mp4");
        $('.Segment8').attr("src", "../Traning/Spanish/Segment8.mp4");
        $('.Segment81').attr("src", "../Traning/Spanish/Segment81.mp4");
        $('.Segment9').attr("src", "../Traning/Spanish/Segment9.mp4");
        $('.Segment91').attr("src", "../Traning/Spanish/Segment91.mp4");
        $('.Segment10').attr("src", "../Traning/Spanish/Segment10.mp4");
        $('.Segment11').attr("src", "../Traning/Spanish/Segment11.mp4");
        $('.Segment12').attr("src", "../Traning/Spanish/Segment12.mp4");
        $('.Segment13').attr("src", "../Traning/Spanish/Segment13.mp4");
        $('.Segment14').attr("src", "../Traning/Spanish/Segment14.mp4");
        $('.trainingImportantNoticeContainer-spanish').show();
        $('.trainingImportantNoticeContainer').hide();
        $('.trainingImportantNoticeContainer-spanish').removeClass("hidden");
        $('.trainingImportantNoticeContainer').addClass("hidden");

        $(".nextButtonText").text('Proxima');
        $(".submitButtonText").text('Enviar');
        $(".downloadCertificateText").text('Baja el Certificado');
        $(".TakeTestAgainText").text("Take Test Again");
    }
    else {
        $('.Segment1').attr("src", "../Traning/Segment1.mp4");
        $('.Segment111').attr("src", "../Traning/Segment111.mp4");
        $('.Segment112').attr("src", "../Traning/Segment112.mp4");
        $('.Segment2').attr("src", "../Traning/Segment2.mp4");
        $('.Segment3').attr("src", "../Traning/Segment3.mp4");
        $('.Segment4').attr("src", "../Traning/Segment4.mp4");
        $('.Segment5').attr("src", "../Traning/Segment5.mp4");
        $('.Segment6').attr("src", "../Traning/Segment6.mp4");
        $('.Segment7').attr("src", "../Traning/Segment7.mp4");
        $('.Segment8').attr("src", "../Traning/Segment8.mp4");
        $('.Segment81').attr("src", "../Traning/Segment81.mp4");
        $('.Segment9').attr("src", "../Traning/Segment9.mp4");
        $('.Segment91').attr("src", "../Traning/Segment91.mp4");
        $('.Segment10').attr("src", "../Traning/Segment10.mp4");
        $('.Segment11').attr("src", "../Traning/Segment11.mp4");
        $('.Segment12').attr("src", "../Traning/Segment12.mp4");
        $('.Segment13').attr("src", "../Traning/Segment13.mp4");
        $('.Segment14').attr("src", "../Traning/Segment14.mp4");
        $('.trainingImportantNoticeContainer-spanish').hide();
        $('.trainingImportantNoticeContainer-spanish').addClass("hidden");
        $('.trainingImportantNoticeContainer').show();
        $('.trainingImportantNoticeContainer').removeClass("hidden");

        $(".nextButtonText").text('Next');
        $(".submitButtonText").text('Submit');
        $(".downloadCertificateText").text('Download Certificate');
        $(".TakeTestAgainText").text('Take Test again');
    }
    $('.q1DataSp').hide();
    $('.q1DataAnswerSp').hide();
    $('.q2DataSp').hide();
    $('.q2DataAnswerSp').hide();
    $('.q3DataSp').hide();
    $('.q3DataAnswerSp').hide();
    $('.q4DataSp').hide();
    $('.q4DataAnswerSp').hide();
    $('.q5DataSp').hide();
    $('.q5DataAnswerSp').hide();
    $('.q6DataSp').hide();
    $('.q6DataAnswerSp').hide();
    $('.q7DataSp').hide();
    $('.q7DataAnswerSp').hide();
    $('.q8DataSp').hide();
    $('.q8DataAnswerSp').hide();
    $('.q9DataSp').hide();
    $('.q9DataAnswerSp').hide();
    $('.q10DataSp').hide();
    $('.q10DataAnswerSp').hide();
    $('.q11DataSp').hide();
    $('.q11DataAnswerSp').hide();
    $('.q12DataSp').hide();
    $('.q12DataAnswerSp').hide();
    $('.q13DataSp').hide();
    $('.q13DataAnswerSp').hide();
    $('.q14DataSp').hide();


    $('.certificate').hide();
    $('.btnCompleteTraning').hide();
    $('.v1').hide();
    $('.v111').hide();
    $('.v112').hide();
    $('.v2').hide();
    $('.v3').hide();
    $('.v4').hide();
    $('.v5').hide();
    $('.v6').hide();
    $('.v7').hide();
    $('.v8').hide();
    $('.v81').hide();
    $('.v9').hide();
    $('.v91').hide();
    $('.v10').hide();
    $('.v11').hide();
    $('.v12').hide();
    $('.v13').hide();
    $('.v14').hide();

    $('.q1Data').hide();
    $('.q2Data').hide();
    $('.q3Data').hide();
    $('.q4Data').hide();
    $('.q5Data').hide();
    $('.q6Data').hide();
    $('.q7Data').hide();
    $('.q8Data').hide();
    $('.q9Data').hide();
    $('.q10Data').hide();
    $('.q11Data').hide();
    $('.q12Data').hide();
    $('.q13Data').hide();
    $('.q14Data').hide();



    $('.q1DataAnswer').hide();
    $('.q2DataAnswer').hide();
    $('.q3DataAnswer').hide();
    $('.q4DataAnswer').hide();
    $('.q5DataAnswer').hide();
    $('.q6DataAnswer').hide();
    $('.q7DataAnswer').hide();
    $('.q8DataAnswer').hide();
    $('.q9DataAnswer').hide();
    $('.q10DataAnswer').hide();
    $('.q11DataAnswer').hide();
    $('.q12DataAnswer').hide();
    $('.q13DataAnswer').hide();
    $('.q14DataAnswer').hide();


    $('.video-div-1').hide();
    $('.video-div-1-1').hide();
    $('.video-div-1-2').hide();
    $('.video-div-2').hide();
    $('.video-div-3').hide();
    $('.video-div-4').hide();
    $('.video-div-5').hide();
    $('.video-div-6').hide();
    $('.video-div-7').hide();
    $('.video-div-8').hide();
    $('.video-div-8-1').hide();
    $('.video-div-9').hide();
    $('.video-div-9-1').hide();
    $('.video-div-10').hide();
    $('.video-div-11').hide();
    $('.video-div-12').hide();
    $('.video-div-13').hide();
    $('.video-div-14').hide();


    document.getElementById('v1').addEventListener('ended', endofVideo1, false);
    document.getElementById('v1').addEventListener('seeking', stopSeeking, false);

    document.getElementById('v111').addEventListener('ended', endofVideo111, false);
    document.getElementById('v111').addEventListener('seeking', stopSeeking, false);

    document.getElementById('v112').addEventListener('ended', endofVideo112, false);
    document.getElementById('v112').addEventListener('seeking', stopSeeking, false);

    document.getElementById('v2').addEventListener('ended', endofVideo2, false);
    document.getElementById('v2').addEventListener('seeking', stopSeeking, false);

    document.getElementById('v3').addEventListener('ended', endofVideo3, false);
    document.getElementById('v3').addEventListener('seeking', stopSeeking, false);

    document.getElementById('v4').addEventListener('ended', endofVideo4, false);
    document.getElementById('v4').addEventListener('seeking', stopSeeking, false);

    document.getElementById('v5').addEventListener('ended', endofVideo5, false);
    document.getElementById('v5').addEventListener('seeking', stopSeeking, false);

    document.getElementById('v6').addEventListener('ended', endofVideo6, false);
    document.getElementById('v6').addEventListener('seeking', stopSeeking, false);

    document.getElementById('v7').addEventListener('ended', endofVideo7, false);
    document.getElementById('v7').addEventListener('seeking', stopSeeking, false);

    document.getElementById('v8').addEventListener('ended', endofVideo8, false);
    document.getElementById('v8').addEventListener('seeking', stopSeeking, false);


    document.getElementById('v81').addEventListener('ended', endofVideo81, false);
    document.getElementById('v81').addEventListener('seeking', stopSeeking, false);


    document.getElementById('v9').addEventListener('ended', endofVideo9, false);
    document.getElementById('v9').addEventListener('seeking', stopSeeking, false);

    document.getElementById('v91').addEventListener('ended', endofVideo91, false);
    document.getElementById('v91').addEventListener('seeking', stopSeeking, false);

    document.getElementById('v10').addEventListener('ended', endofVideo10, false);
    document.getElementById('v10').addEventListener('seeking', stopSeeking, false);

    document.getElementById('v11').addEventListener('ended', endofVideo11, false);
    document.getElementById('v11').addEventListener('seeking', stopSeeking, false);

    document.getElementById('v12').addEventListener('ended', endofVideo12, false);
    document.getElementById('v12').addEventListener('seeking', stopSeeking, false);

    document.getElementById('v13').addEventListener('ended', endofVideo13, false);
    document.getElementById('v13').addEventListener('seeking', stopSeeking, false);

    document.getElementById('v14').addEventListener('ended', endofVideo14, false);
    document.getElementById('v14').addEventListener('seeking', stopSeeking, false);

    $(".intialVideoButton").attr('disabled', 'disabled');

}

function endofvideoSetting(currentVideo, currentVideoDiv, nextVideo, nextVideoDiv, quetsionDiv, spanishQuestionDiv) {
    var currentVideoclassName = currentVideo;
    if (currentVideoclassName != null && currentVideoclassName != "") {
        var vid = document.getElementById(currentVideoclassName.substring(1, currentVideoclassName.length));
        vid.pause();
    }
    $(currentVideo).hide();
    $(currentVideoDiv).hide();
    $(currentVideo).addClass("hidden");
    $(currentVideoDiv).addClass("hidden");
    $(nextVideo).show();
    $(nextVideo).removeClass("hidden");
    var nextVideoclassName = nextVideo;
    if (nextVideoclassName != null && nextVideoclassName != "") {
        var nextvid = document.getElementById(nextVideoclassName.substring(1, nextVideoclassName.length));
        nextvid.play();
        console.log(4);
    }
    $(nextVideoDiv).show();
    $(nextVideoDiv).removeClass("hidden");
    if (quetsionDiv == ".q1Data" || quetsionDiv == ".q1DataSp" || quetsionDiv == ".q2Data" || quetsionDiv == ".q2DataSp" ||
        quetsionDiv == ".q3Data" || quetsionDiv == ".q3DataSp" || quetsionDiv == ".q4Data" || quetsionDiv == ".q4DataSp" ||
        quetsionDiv == ".q5Data" || quetsionDiv == ".q5DataSp" || quetsionDiv == ".q6Data" || quetsionDiv == ".q6DataSp" ||
        quetsionDiv == ".q7Data" || quetsionDiv == ".q7DataSp" || quetsionDiv == ".q8Data" || quetsionDiv == ".q8DataSp" ||
        quetsionDiv == ".q9Data" || quetsionDiv == ".q9DataSp" || quetsionDiv == ".q10Data" || quetsionDiv == ".q10DataSp" ||
        quetsionDiv == ".q11Data" || quetsionDiv == ".q11DataSp" || quetsionDiv == ".q12Data" || quetsionDiv == ".q12DataSp" ||
        quetsionDiv == ".q13Data" || quetsionDiv == ".q13DataSp") {
        $(quetsionDiv + ' .btn').attr('disabled', 'disabled');
        $(quetsionDiv + 'Sp .btn').attr('disabled', 'disabled');
    }
    if ($.isEmptyObject(nextVideoDiv)) {
        if (languagePrefrence == "2")
            updateNextSlide(spanishQuestionDiv);
        else
            updateNextSlide(quetsionDiv);
    }
    else {
        updateNextSlide(nextVideoDiv);
    }
    if (languagePrefrence == "2") {
        $(spanishQuestionDiv).show();
        $(spanishQuestionDiv).removeClass("hidden");
    }
    else {
        $(quetsionDiv).show();
        $(quetsionDiv).removeClass("hidden");
    }
}

function enableButton(quetsionDiv) {
    $(quetsionDiv + ' .btn').removeAttr('disabled');
}
function endofVideo1(e, isButtonClick) {
    $('.v1button').removeAttr('disabled');
}
function endofVideo111(e, isButtonClick) {
    $('.v111button').removeAttr('disabled');
}
function endofVideo112(e, isButtonClick) {
    $('.v112button').removeAttr('disabled');
}
function endofVideo2() {
    $('.v2button').removeAttr('disabled');
}
function endofVideo3() {
    $('.v3button').removeAttr('disabled');
}
function endofVideo4() {
    $('.v4button').removeAttr('disabled');
}
function endofVideo5() {
    $('.v5button').removeAttr('disabled');
}
function endofVideo6() {
    $('.v6button').removeAttr('disabled');
}
function endofVideo7() {
    $('.v7button').removeAttr('disabled');
}
function endofVideo8() {
    $('.v8button').removeAttr('disabled');
}
function endofVideo81() {
    $('.v81button').removeAttr('disabled');
}
function endofVideo9() {
    $('.v9button').removeAttr('disabled');
}
function endofVideo91() {
    $('.v91button').removeAttr('disabled');
}
function endofVideo10() {
    $('.v10button').removeAttr('disabled');
}
function endofVideo11() {
    $('.v11button').removeAttr('disabled');
}
function endofVideo12() {
    $('.v12button').removeAttr('disabled');
}
function endofVideo13() {
    $('.v13button').removeAttr('disabled');
}
function endofVideo14() {
    $('.v14button').removeAttr('disabled');
}

function updateNextSlide(currentSlidename) {
    
    $.ajax({
        url: "/HR/HREmployeeTraining/UpdateLastSlideTrainig",
        type: "GET",
        data: { "LastSlideName": currentSlidename },
        dataType: "json",
        contentType: "application/json;",
        success: function (result) {
            console.log(currentSlidename);
        }
    });
}

function stopSeeking() {
    var video = document.getElementById('v1');
    var delta = video.currentTime - supposedCurrentTime;
    if (Math.abs(delta) > 0.01) {
        console.log("Seeking is disabled");
        video.currentTime = supposedCurrentTime;
    }
    var video11 = document.getElementById('v11');
    var delta11 = video11.currentTime - supposedCurrentTime;
    if (Math.abs(delta11) > 0.01) {
        console.log("Seeking is disabled");
        video11.currentTime = supposedCurrentTime;
    }

    var video12 = document.getElementById('v12');
    var delta12 = video12.currentTime - supposedCurrentTime;
    if (Math.abs(delta12) > 0.01) {
        console.log("Seeking is disabled");
        video12.currentTime = supposedCurrentTime;
    }

    var video2 = document.getElementById('v2');
    var delta2 = video2.currentTime - supposedCurrentTime;
    if (Math.abs(delta2) > 0.01) {
        console.log("Seeking is disabled");
        video2.currentTime = supposedCurrentTime;
    }
    var video3 = document.getElementById('v3');
    var delta3 = video3.currentTime - supposedCurrentTime;
    if (Math.abs(delta3) > 0.01) {
        console.log("Seeking is disabled");
        video3.currentTime = supposedCurrentTime;
    }
    var video4 = document.getElementById('v4');
    var delta4 = video4.currentTime - supposedCurrentTime;
    if (Math.abs(delta4) > 0.01) {
        console.log("Seeking is disabled");
        video4.currentTime = supposedCurrentTime;
    }
    var video5 = document.getElementById('v5');
    var delta5 = video5.currentTime - supposedCurrentTime;
    if (Math.abs(delta5) > 0.01) {
        console.log("Seeking is disabled");
        video5.currentTime = supposedCurrentTime;
    }
    var video6 = document.getElementById('v6');
    var delta6 = video6.currentTime - supposedCurrentTime;
    if (Math.abs(delta6) > 0.01) {
        console.log("Seeking is disabled");
        video6.currentTime = supposedCurrentTime;
    }
    var video7 = document.getElementById('v7');
    var delta7 = video7.currentTime - supposedCurrentTime;
    if (Math.abs(delta7) > 0.01) {
        console.log("Seeking is disabled");
        video7.currentTime = supposedCurrentTime;
    }

    var video8 = document.getElementById('v8');
    var delta8 = video8.currentTime - supposedCurrentTime;
    if (Math.abs(delta8) > 0.01) {
        console.log("Seeking is disabled");
        video8.currentTime = supposedCurrentTime;
    }

    var video9 = document.getElementById('v9');
    var delta9 = video9.currentTime - supposedCurrentTime;
    if (Math.abs(delta9) > 0.01) {
        console.log("Seeking is disabled");
        video9.currentTime = supposedCurrentTime;
    }

    var video10 = document.getElementById('v10');
    var delta10 = video10.currentTime - supposedCurrentTime;
    if (Math.abs(delta10) > 0.01) {
        console.log("Seeking is disabled");
        video10.currentTime = supposedCurrentTime;
    }

    var video13 = document.getElementById('v13');
    var delta13 = video13.currentTime - supposedCurrentTime;
    if (Math.abs(delta13) > 0.01) {
        console.log("Seeking is disabled");
        video13.currentTime = supposedCurrentTime;
    }

    var video14 = document.getElementById('v14');
    var delta14 = video14.currentTime - supposedCurrentTime;
    if (Math.abs(delta14) > 0.01) {
        console.log("Seeking is disabled");
        video14.currentTime = supposedCurrentTime;
    }

}

function hideQuestion(question, answer, currentvideo) {
    $('.' + currentvideo).hide();
    $('.' + question).hide();
    $('.' + currentvideo).addClass("hidden");
    $('.' + question).addClass("hidden");
    $('.' + answer).show();
    $('.' + answer).removeClass("hidden");
    updateNextSlide('.' + answer);
}

function hideQuestionAnswer(answer, question, nextvideo, visibleNextVideoDiv) {
    $('.' + answer).hide();
    $('.' + question).hide();
    $('.' + visibleNextVideoDiv).show();
    $('.' + answer).addClass("hidden");
    $('.' + question).addClass("hidden");
    $('.' + visibleNextVideoDiv).removeClass("hidden");
    updateNextSlide('.' + visibleNextVideoDiv);
    $('.' + nextvideo).show();
    $('.' + nextvideo).removeClass("hidden");
    $('.' + nextvideo).get(0).play();
    console.log(2);
}

function startTraning() {
    $('.ChangeLanguage').attr("hidden", true);
    $('.v1').show();
    $('.video-div-1').show();
    $('.v1').removeClass("hidden");
    $('.video-div-1').removeClass("hidden");
    console.log(1);
    var playPromise = $('.v1').get(0).play();
    if (playPromise !== null) {
        playPromise.catch(() => { media.play(); });
    }
    updateNextSlide('.video-div-1');
    $('.trainingImportantNoticeContainer').hide();
    $('.trainingImportantNoticeContainer-spanish').hide();
    $('.trainingImportantNoticeContainer').addClass("hidden");

}

function showAlert(message, type, closeDelay) {

    if ($("#alerts-container").length == 0) {
        $("body")
            .append($('<div id="alerts-container" style="z-index: 3000; position: fixed; width: 30%; left: 50%; top: 20%; transform: translate(-50%, -50%);">'));
    }

    type = type || "info";

    var alert = $('<div class="alert alert-' + type + ' fade in">')
        .append(
            $('<button type="button" style="margin-left:15px;" class="close" data-dismiss="alert">')
                .append("&times;")
        )
        .append(message);

    $("#alerts-container").prepend(alert);
    if (closeDelay)
        window.setTimeout(function () { alert.alert("close"); }, closeDelay);
}

function showCertificate(question, btnCompleteTraning) {
    $('.' + question).hide();
    $('.' + question).removeClass("hidden");
    $('.certificate').show();
    $('.certificate').removeClass("hidden");
    $('.' + btnCompleteTraning).show();
    $('.' + btnCompleteTraning).removeClass("hidden");

}

$(".cssResetTest").unbind().click(function () {
    var q = JSON.stringify({
        EmployeeId: 0
    });
    console.log(q);
    var url = "/HR/HREmployeeTraining/ResetTraining";

    $.ajax({
        url: url,
        type: "POST",
        data: '{data: ' + q + '}',
        dataType: "json",
        contentType: "application/json;",
        success: function (result) {
            if (result != null && result != "") {
                if (result == "200") {
                    location.reload();
                    showAlert("Test reset successfully.", "info", 5000);
                }
                else {
                    showAlert("Test not reset successfully.", "info", 5000);
                }
            }
            else {
                showAlert("Test not reset successfully.", "info", 5000);
            }
        }
    });
});

$(".csscompleteTraning").click(function () {

    DownloadCertificate();
});

function DownloadCertificate() {
    
    //$(".csscompleteTraning").prop('disabled', true);
    var currentDate = '';
    var currentTime = '';

    var d = new Date();
    utc = d.getTime() + (d.getTimezoneOffset() * 60000);
    nd = new Date(utc + (3600000 * '-4'));
    var month = nd.getMonth() + 1; // Adjusting month index to start from 1

    // Pad single-digit months with leading zero
    month = (month < 10 ? '0' : '') + month;

    var day = nd.getDate();
    // Pad single-digit days with leading zero
    day = (day < 10 ? '0' : '') + day;

    var year = nd.getFullYear();

    currentDate = month + '/' + day + '/' + year;

    var hours = nd.getHours();
    // Pad single-digit hours with leading zero
    hours = (hours < 10 ? '0' : '') + hours;

    var minutes = nd.getMinutes();
    // Pad single-digit minutes with leading zero
    minutes = (minutes < 10 ? '0' : '') + minutes;

    currentTime = hours + ':' + minutes;

    $('#curentDate').text(currentDate);
    $('#curentTime').text(currentTime);
    $('.certificateFirstName').text(sessionStorage.getItem('firstName'));
    $('.certificateLastName').text(sessionStorage.getItem('lastName'));

    var q = {
        Data: (languagePrefrence != "2" ? $('.certificate').html() : $('.certificate').html()), TrainingDate: currentDate, TrainingTime: currentTime
    };

    var url = "/HR/HREmployeeTraining/CompleteTraining";

    $.ajax({
        type: "POST",
        url: url,
        data: '{data: ' + JSON.stringify(q) + '}',
        dataType: "json",
        contentType: "application/json;",
        success: function (result) {
            
            if (result != null && result != "") {
                var link = document.createElement('a');
                link.href = "/UserFiles/HR_File/Certificates/" + result;
                var date = new Date();
                link.download = "Training-" + userid + "-" + username + "-" + date.getDay() + '-' + date.getMonth() + '-' + date.getYear() + ".pdf";
                link.click();
                console.log(1);
                sessionStorage.setItem("IsTraningCompleted", true);
            }
            else {
                //showAlert("Training not completed", "info", 5000);
                console.log(2);
            }
        }
    });
}

function checkTrainingStartedOrNot(lsname) {
    
    if (!$.isEmptyObject(lsname)) {
        $('.ChangeLanguage').attr("hidden", true);
        $('.trainingImportantNoticeContainer-spanish').hide();
        $('.trainingImportantNoticeContainer').hide();
        $('.trainingImportantNoticeContainer').addClass("hidden");
        $(lsname).show();
        $(lsname).removeClass("hidden");
        if ($(lsname).find("video").show().get(0) != null) {
            console.log($(lsname).find("video").show().get(0));
            $(lsname).find("video").show().get(0).play();
            console.log(3);
        }
    }
};

function calcTime(city, offset) {

    // create Date object for current location
    d = new Date();

    // convert to msec
    // add local time zone offset
    // get UTC time in msec
    utc = d.getTime() + (d.getTimezoneOffset() * 60000);

    // create new Date object for different city
    // using supplied offset
    nd = new Date(utc + (3600000 * offset));

    var month = nd.getMonth();
    if (nd.getMonth() == 12)
        month = 1;
    else
        month = nd.getMonth() + 1;

    currentDate = month + '/' + nd.getDate() + '/' + nd.getFullYear();
    currentTime = (nd.getHours() < 10 ? '0' : '') + nd.getHours() + ':' + (nd.getMinutes() < 10 ? '0' : '') + nd.getMinutes(); //+ ':' + today.getSeconds();
    // return time as a string
    console.log("The local time in " + city + " is " + nd.toLocaleString());
    //The local time in New_York is 10/10/2019, 1:07:57 AM

}

function ecwEncoder() {
    {
        var base64 = {};
        base64.PADCHAR = '=';
        base64.ALPHA = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';
        base64.makeDOMException = function () {
            // sadly in FF,Safari,Chrome you can't make a DOMException
            var e, tmp;
            try {
                return new DOMException(DOMException.INVALID_CHARACTER_ERR);
            } catch (tmp) {
                var ex = new Error("DOM Exception 5");
                ex.code = ex.number = 5;
                ex.name = ex.description = "INVALID_CHARACTER_ERR";
                ex.toString = function () { return 'Error: ' + ex.name + ': ' + ex.message; };
                return ex;
            }
        }
        base64.getbyte64 = function (s, i) {
            // This is oddly fast, except on Chrome/V8.
            // Minimal or no improvement in performance by using a
            // object with properties mapping chars to value (eg. 'A': 0)
            var idx = base64.ALPHA.indexOf(s.charAt(i));
            if (idx === -1) {
                throw base64.makeDOMException();
            }
            return idx;
        }
        base64.decode = function (s) {
            // convert to string
            s = '' + s;
            var getbyte64 = base64.getbyte64;
            var pads, i, b10;
            var imax = s.length
            if (imax === 0) {
                return s;
            }
            if (imax % 4 !== 0) {
                throw base64.makeDOMException();
            }
            pads = 0
            if (s.charAt(imax - 1) === base64.PADCHAR) {
                pads = 1;
                if (s.charAt(imax - 2) === base64.PADCHAR) {
                    pads = 2;
                }
                // either way, we want to ignore this last block
                imax -= 4;
            }
            var x = [];
            for (i = 0; i < imax; i += 4) {
                b10 = (getbyte64(s, i) << 18) | (getbyte64(s, i + 1) << 12) | (getbyte64(s, i + 2) << 6) | getbyte64(s, i + 3);
                x.push(String.fromCharCode(b10 >> 16, (b10 >> 8) & 0xff, b10 & 0xff));
            }
            switch (pads) {
                case 1:
                    b10 = (getbyte64(s, i) << 18) | (getbyte64(s, i + 1) << 12) | (getbyte64(s, i + 2) << 6);
                    x.push(String.fromCharCode(b10 >> 16, (b10 >> 8) & 0xff));
                    break;
                case 2:
                    b10 = (getbyte64(s, i) << 18) | (getbyte64(s, i + 1) << 12);
                    x.push(String.fromCharCode(b10 >> 16));
                    break;
            }
            return x.join('');
        }
        base64.getbyte = function (s, i) {
            var x = s.charCodeAt(i);
            if (x > 255) {
                throw base64.makeDOMException();
            }
            return x;
        }
        base64.encode = function (s) {
            if (arguments.length !== 1) {
                throw new SyntaxError("Not enough arguments");
            }
            var padchar = base64.PADCHAR;
            var alpha = base64.ALPHA;
            var getbyte = base64.getbyte;
            var i, b10;
            var x = [];
            // convert to string
            s = '' + s;
            var imax = s.length - s.length % 3;
            if (s.length === 0) {
                return s;
            }
            for (i = 0; i < imax; i += 3) {
                b10 = (getbyte(s, i) << 16) | (getbyte(s, i + 1) << 8) | getbyte(s, i + 2);
                x.push(alpha.charAt(b10 >> 18));
                x.push(alpha.charAt((b10 >> 12) & 0x3F));
                x.push(alpha.charAt((b10 >> 6) & 0x3f));
                x.push(alpha.charAt(b10 & 0x3f));
            }
            switch (s.length - imax) {
                case 1:
                    b10 = getbyte(s, i) << 16;
                    x.push(alpha.charAt(b10 >> 18) + alpha.charAt((b10 >> 12) & 0x3F) + padchar + padchar);
                    break;
                case 2:
                    b10 = (getbyte(s, i) << 16) | (getbyte(s, i + 1) << 8);
                    x.push(alpha.charAt(b10 >> 18) + alpha.charAt((b10 >> 12) & 0x3F) + alpha.charAt((b10 >> 6) & 0x3f) + padchar);
                    break;
            }
            return x.join('');
        }

        return {
            encode: base64.encode,
            decode: base64.decode
        }
    }
}

