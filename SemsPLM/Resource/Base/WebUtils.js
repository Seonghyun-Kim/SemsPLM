﻿var WebUtils = function () { };

WebUtils.IsRightClick = function (event) {
    var rightclick;
    if (!event) event = window.event;
    if (event.which) rightclick = event.which === 3 ? true : false;
    else if (event.button) rightclick = event.button === 2 ? true : false;
    return rightclick;
}

/** * 공백이나 널인지 확인 * * @return : boolean */
WebUtils.isBlank = function (_str) {
    if (_str == undefined) {
        return true;
    }
    var str = _str.trim();
    for (var i = 0; i < str.length; i++) {
        if ((str.charAt(i) != "\t") && (str.charAt(i) != "\n") && (str.charAt(i) != "\r")) {
            return false;
        }
    }
    return true;
}

/** * 이메일 형식 체크 * * @param 데이터 */
WebUtils.emailCheck = function(email) {
    var exptext = /^[A-Za-z0-9_\.\-]+@[A-Za-z0-9\-]+\.[A-Za-z0-9\-]+/;
    if (exptext.test(email) == false) {
        // 이메일 형식이 알파벳+숫자@알파벳+숫자.알파벳+숫자 형식이 아닐경우
        alert("이메일형식이 올바르지 않습니다.");
        return false;
    }
    return true;
}

/** * 특수문자 여부 체크 * * @param 데이터 */
WebUtils.checkSpecial = function(str) {
    var special_pattern = /[`~!@#$%^&*|\\\'\";:\/?]/gi;
    if (special_pattern.test(str) == true) {
        return 0;
    } else {
        return -1;
    }
}

/** * 전화번호 포맷으로 변환 * * @param 데이터 */
WebUtils.formatPhone = function(phoneNum) {
    if (isPhone(phoneNum)) {
        var rtnNum;
        var regExp = /(02)([0-9]{3,4})([0-9]{4})$/;
        var myArray;
        if (regExp.test(phoneNum)) {
            myArray = regExp.exec(phoneNum);
            rtnNum = myArray[1] + '-' + myArray[2] + '-' + myArray[3];
            return rtnNum;
        } else {
            regExp = /(0[3-9]{1}[0-9]{1})([0-9]{3,4})([0-9]{4})$/;
            if (regExp.test(phoneNum)) {
                myArray = regExp.exec(phoneNum); rtnNum = myArray[1] + '-' + myArray[2] + '-' + myArray[3];
                return rtnNum;
            } else {
                return phoneNum;
            }
        }
    } else {
        return phoneNum;
    }
}

/** * 전화번호 형식 체크 * * @param 데이터 */
WebUtils.isPhone = function(phoneNum) { 
    var regExp = /(02)([0-9]{3,4})([0-9]{4})$/;
    var myArray;
    if (regExp.test(phoneNum)) {
        myArray = regExp.exec(phoneNum);
        return true;
    } else {
        regExp = /(0[3-9]{1}[0-9]{1})([0-9]{3,4})([0-9]{4})$/;
        if (regExp.test(phoneNum)) {
            myArray = regExp.exec(phoneNum);
            return true;
        } else {
            return false;
        }
    }
}

/** * 핸드폰번호 포맷으로 변환 * * @param 데이터 */
WebUtils.formatMobile = function(phoneNum) {
    if (isMobile(phoneNum)) {
        var rtnNum;
        var regExp = /(01[016789])([1-9]{1}[0-9]{2,3})([0-9]{4})$/;
        var myArray;
        if (regExp.test(phoneNum)) {
            myArray = regExp.exec(phoneNum);
            rtnNum = myArray[1] + '-' + myArray[2] + '-' + myArray[3];
            return rtnNum;
        } else {
            return phoneNum;
        }
    } else {
        return phoneNum;
    }
}

/** * 핸드폰번호 형식 체크 * * @param 데이터 */
WebUtils.isMobile = function(phoneNum) {
    var regExp = /(01[016789])([1-9]{1}[0-9]{2,3})([0-9]{4})$/;
    var myArray;
    if (regExp.test(phoneNum)) {
        myArray = regExp.exec(phoneNum);
        return true;
    } else {
        return false;
    }
}

/** * 바코드 포맷으로 변환 * * @param 데이터 */
WebUtils.formatBarcode = function (code, appendKeywork) {
    var pattern_eng = /[a-zA-Z]/;	// 문자 
    var pattern_spc = /[~!@#$%^&*()_+|<>?:{}]/; // 특수문자
    var returnData = '';
    for (var i = 0; i < code.length; i++) {
        if (pattern_eng.test(code.charAt(i))) {
            returnData += appendKeywork + code.charAt(i);
        } else {
            returnData += code.charAt(i);
        }
    }
    return returnData;
}

WebUtils.GetDate = function (num, type, tag) {
    if (tag === undefined || tag === null) {
        tag = "-";
    }

    var d = new Date(); //오늘날짜 데이터 세팅

    var retDate = new Date();

    if (type === "y") {
        retDate.setYear(d.getFullYear() + num);
    }
    else if (type === "m") {
        retDate.setMonth(d.getMonth() + num);
    }
    else if (type === "d") {
        retDate.setDate(d.getDate() + num);
    }

    var yy = retDate.getFullYear();
    var mm = (retDate.getMonth() + 1).toString().length === 1 ? "0" + (retDate.getMonth() + 1) : (retDate.getMonth() + 1);
    var dd = (retDate.getDate()).toString().length === 1 ? "0" + (retDate.getDate()) : (retDate.getDate());

    return yy + tag + mm + tag + dd;
}

//yyyy-MM-dd 날짜 문자열을 Date형으로 반환
WebUtils.ToDate = function (date_str) {
    var yyyyMMdd = String(date_str);
    var sYear = yyyyMMdd.substring(0, 4);
    var sMonth = yyyyMMdd.substring(5, 7);
    var sDate = yyyyMMdd.substring(8, 10);

    //alert("sYear :"+sYear +"   sMonth :"+sMonth + "   sDate :"+sDate);
    return new Date(Number(sYear), Number(sMonth) - 1, Number(sDate));
}