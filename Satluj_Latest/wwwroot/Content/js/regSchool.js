$(document).on('blur', '#emailaddress', function () {
    validateEmail();
});
function validateEmail() {
    var status = 1;
    var Email = $('#emailaddress').val();
    if (Email.length > 0) {
        var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        if (regex.test(Email)) {
            $.ajax({

                method: "GET",
                url: "/Account/IsEmailExist?Email=" + Email,
                type: 'json'
            })
                .done(function (result) {
                    if (result.status) {
                        $('#EmailAvail').addClass('isHidden');
                        $('#EmailExist').removeClass('isHidden');
                        $('#EmailNotValid').addClass('isHidden');
                        //$('#BtnSubmit').attr("disabled", true);
                        //$('#submit-btn-dummy-reg').show();
                        disableSubmitButton();
                        $('html,body').animate({
                            scrollTop: $('#emailaddress').offset().top
                        }, 'slow');
                        disableSubmitButton();
                        status = 0;
                        $('#hfEmailValid').val(0);
                        //$('#submit-btn-dummy-reg-fake').show();
                    }
                    else {
                        $('#EmailExist').addClass('isHidden');
                        $('#EmailAvail').removeClass('isHidden');
                        $('#EmailNotValid').addClass('isHidden');
                        enableSubmitButton();
                        $('#hfEmailValid').val(1);
                        //$('#BtnSubmit').removeAttr("disabled");
                        //$('#submit-btn-dummy-reg').show();
                        //$('#submit-btn-dummy-reg-fake').hide();
                    }
                });
        }
        else {
            $('#EmailNotValid').removeClass('isHidden');
            $('#EmailNotValid').text("Email required");
            $('#EmailAvail').addClass('isHidden');
            $('#EmailExist').addClass('isHidden');
            disableSubmitButton();
            $('#hfEmailValid').val(0);
            //$('#BtnSubmit').attr("disabled", true);
            //$('#submit-btn-dummy-reg').show();
            $('html,body').animate({
                scrollTop: $('#emailaddress').offset().top
            }, 'slow');
            status = 0;

            //$('#submit-btn-dummy-reg-fake').show();
        }
    }
    else {
        $('#EmailNotValid').removeClass('isHidden');
        $('#EmailNotValid').text("Email required");
        disableSubmitButton();
        status = 0;
        $('#hfEmailValid').val(0);
    }

    return status;
}

$(document).on('blur', '#schoolName', function () {
    validateSchoolName();
});
function validateSchoolName() {
    var status = 1;
    if ($('#schoolName').val().length > 0) {
        $('.nameMsg').addClass('isHidden');

        enableSubmitButton();
    }
    else {
        $('.nameMsg').removeClass('isHidden');
        disableSubmitButton();
        status = 0;
    }
    return status;
}

$(document).on('blur', '#address', function () {
    validateAddress();
});
function validateAddress() {
    var status = 1;
    if ($('#address').val().length > 0) {
        $('.addressMsg').addClass('isHidden');
        enableSubmitButton();
    }
    else {
        $('.addressMsg').removeClass('isHidden');
        disableSubmitButton();
        status = 0;
    }
    return status;
}

$(document).on('blur', '#password', function () {
    validatePassword();
});
function validatePassword() {
    var status = 1;
    if ($('#password').val().length > 0) {
        $('.passwordMsg').addClass('isHidden');
        enableSubmitButton();
    }
    else {
        $('.passwordMsg').removeClass('isHidden');
        disableSubmitButton();
        status = 0;
    }
    return status;
}

$(document).on('blur', '#contactNumber', function () {
    validatePhone();
});
function validatePhone() {
    var status = 1;
    if ($('#contactNumber').val().length > 0) {
        $('.phonedMsg').addClass('isHidden');
        enableSubmitButton();
    }
    else {
        $('.phonedMsg').removeClass('isHidden');
        disableSubmitButton();
        status = 0;
    }
    return status;
}

$(document).on('change', '#state', function () {
    validateState();
});
function validateState() {
    var status = 1;
    if ($('#state').val() != "") {
        $('.stateMsg').addClass('isHidden');
        enableSubmitButton();
    }
    else {
        $('.stateMsg').removeClass('isHidden');
        disableSubmitButton();
        status = 0;
    }
    return status;
}

$(document).on('blur', '#city', function () {
    validateCity();
});
function validateCity() {
    var status = 1;
    if ($('#city').val() != "") {
        $('.cityMsg').addClass('isHidden');
        enableSubmitButton();
    }
    else {
        $('.cityMsg').removeClass('isHidden');
        disableSubmitButton();
        status = 0;
    }
    return status;
}

function checkAllFields() {

    var status = 0;

    //firstName
    status = validateSchoolName();
    if (status == 0) {
        $('html,body').animate({
            scrollTop: $('#schoolName').offset().top
        },
        'slow');
        // $('body').pleaseWait('stop');
        return;
    }
    //lastName
    status = validateAddress();
    if (status == 0) {
        $('html,body').animate({
            scrollTop: $('#address').offset().top
        },
        'slow');
        // $('body').pleaseWait('stop');
        return;
    }
    //Email
    status = validateEmail();


    if (status == 0) {

        $('html,body').animate({
            scrollTop: $('#emailaddress').offset().top
        },
        'slow');

        //$('body').pleaseWait('stop');
        return;

    }

    //Password
    status = validatePassword();
    if (status == 0) {

        $('html,body').animate({
            scrollTop: $('#password').offset().top
        },
        'slow');
        //$('body').pleaseWait('stop');
        return;
    }

    //phone
    status = validatePhone();
    if (status == 0) {
        $('html, body').animate({
            scrollTop: $('#contactNumber').offset().top
        },
        'slow');
        //$('body').pleaseWait('stop');
        return;
    }

    //State
    status = validateState();
    if (status == 0) {

        $('html,body').animate({
            scrollTop: $('#state').offset().top
        },
        'slow');
        $('body').pleaseWait('stop');
        return;
    }

    //City
    status = validateCity();
    if (status == 0) {

        $('html,body').animate({
            scrollTop: $('#city').offset().top
        },
        'slow');
        $('body').pleaseWait('stop');
        return;
    }

    var validEmail = $('#hfEmailValid').val();
    if (status == 1 && validEmail == 1) {
        // $('body').pleaseWait();
        SubmitAllData();

    }

}

function enableSubmitButton() {
    $('#btn-dummy-Signup').removeAttr("disabled");
}
function disableSubmitButton() {

    $('#btn-dummy-Signup').attr("disabled", true);
}


//-------------------------------------------------
//Archana For Chart
/**
 * Define data
 */

//--------------------THE END--------------------------