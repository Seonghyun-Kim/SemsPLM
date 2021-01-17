function ContentEditMode(v) {
    var AttrType = $(v).attr("AttrType");
    if (AttrType === "TEXT" || AttrType === "TEXTAREA") {
        $(v).removeAttr('readonly');
    } else if (AttrType === "COMBO") {
        $(v).jqxComboBox({ disabled: false });
    } else if (AttrType === "DATE") {
        $(v).jqxDateTimeInput({ disabled: false });
    } else if (AttrType === "CHECKBOX") {
        $(v).jqxCheckBox({ disabled: false });
    } else if (AttrType === "BUTTON") {
        $(v).jqxButton({ disabled: false });
        $(v).css("display", "block");
    } else if (AttrType === "SWITCH") {
        $(v).jqxSwitchButton({ disabled: false });
    }
}

function ContentReadMode(v) {
    var AttrType = $(v).attr("AttrType");
    if (AttrType === "TEXT" || AttrType === "TEXTAREA") {
        $(v).attr('readonly', 'readonly');
    } else if (AttrType === "COMBO") {
        $(v).jqxComboBox({ disabled: true });
    } else if (AttrType === "DATE") {
        $(v).jqxDateTimeInput({ disabled: true });
    } else if (AttrType === "CHECKBOX") {
        $(v).jqxCheckBox({ disabled: true });
    } else if (AttrType === "BUTTON") {
        $(v).css("display", "none");
        $(v).jqxButton({ disabled: true });
    } else if (AttrType === "SWITCH") {
        $(v).jqxSwitchButton({ disabled: true });
    }
}

function addDays(date, days) {
    var result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
}

var GridStatus = function (ModuleFl, BPolicyNm, EstEndDt, ChargeUserNm) {
    var retJson = {};
    var BackgroundColor = "white";
    var ChargeUserText = ModuleFl ? ChargeUserNm : "N/A";
    var milli = EstEndDt.replace(/\/Date\((-?\d+)\)\//, '$1');
    var EstDate = new Date(parseInt(milli));
    var CompareEstDate = addDays(EstDate, 1);
    var nowDate = new Date();

    if (!ModuleFl) {
        BackgroundColor = "lightgreen";
    } else if (BPolicyNm === "Completed") {
        BackgroundColor = "lightgreen";
        ChargeUserText = "(완료) " + ChargeUserText;
    } else if (BPolicyNm === "Started" && CompareEstDate >= nowDate) {
        BackgroundColor = "Yellow";
        ChargeUserText = "(실행전) " + ChargeUserText;
    } else if (BPolicyNm === "Prepare" && CompareEstDate >= nowDate) {
        BackgroundColor = "white";
    } else if (BPolicyNm === "Confirm" && CompareEstDate >= nowDate) {
        BackgroundColor = "Yellow";
    } else if (BPolicyNm === "Review" && CompareEstDate >= nowDate) {
        BackgroundColor = "Yellow";
        ChargeUserText = "(결재중) " + ChargeUserText;
    } else {
        BackgroundColor = "red";
        ChargeUserText = "(실행전) " + ChargeUserText;
    }

    retJson.BackgroundColor = BackgroundColor;
    retJson.ChargeUserText = ChargeUserText;
    var sYear = EstDate.getFullYear();
    var sMonth = (EstDate.getMonth() + 1) <= 9 ? "0" + (EstDate.getMonth() + 1).toString() : (EstDate.getMonth() + 1).toString();
    var sDate = EstDate.getDate() <= 9 ? "0" + EstDate.getDate().toString() : EstDate.getDate().toString();
    retJson.EstDt = sYear + "-" + sMonth + "-" + sDate;
    return retJson;
}

function OpenQuickResponseDetail(gridid, uid) {
    var rowData = $(gridid).jqxGrid("getrowdata", uid);
    var linkName = "[신속대응상세] " + rowData.Title;
    var linkUrl = '/Qms/InfoQuickResponseDetail?OID=' + rowData.OID;

    TabPageLoad(linkUrl, linkName);
}


function OpenQuickResonse(gridid, uid) {
    var rowData = $(gridid).jqxGrid("getrowdata", uid);
    var linkName = "[신속대응] " + rowData.Title;
    var linkUrl = '/Qms/InfoQuickResponse?OID=' + rowData.OID;

    TabPageLoad(linkUrl, linkName);
}

function OpenBlockade(gridid, uid) {
    var rowData = $(gridid).jqxGrid("getrowdata", uid);
    var linkName = "[" + rowData.Title + "] 봉쇄조치";
    var linkUrl = '/Qms/InfoBlockade?OID=' + rowData.ModuleBlockadeOID;

    TabPageLoad(linkUrl, linkName);
}

function OpenOccurrenceCause(gridid, uid) {
    var rowData = $(gridid).jqxGrid("getrowdata", uid);
    var linkName = "[" + rowData.Title + "] 원인분석";
    var linkUrl = '/Qms/InfoOccurrenceCause?OID=' + rowData.ModuleOccurrenceCauseOID;

    TabPageLoad(linkUrl, linkName);
}

function OpenImproveCountermeasure(gridid, uid) {
    var rowData = $(gridid).jqxGrid("getrowdata", uid);
    var linkName = "[" + rowData.Title + "] 개선대책";
    var linkUrl = '/Qms/InfoImproveCountermeasure?OID=' + rowData.ModuleImproveCountermeasureOID;

    TabPageLoad(linkUrl, linkName);
}

function OpenErrorProof(gridid, uid) {
    var rowData = $(gridid).jqxGrid("getrowdata", uid);
    if (!rowData.ModuleErrorProofFl) { return; }

    var linkName = "[" + rowData.Title + "] Error Prrof";
    var linkUrl = '/Qms/InfoErrorProof?OID=' + rowData.ModuleErrorProofOID;

    TabPageLoad(linkUrl, linkName);
}


function OpenLpaUnfit(gridid, uid) {
    var rowData = $(gridid).jqxGrid("getrowdata", uid);
    if (!rowData.ModuleLpaFl) { return; }

    var linkName = "[" + rowData.Title + "] LPA 감사";
    var linkUrl = '/Qms/InfoLpaUnfit?OID=' + rowData.ModuleLpaOID;

    TabPageLoad(linkUrl, linkName);
}

/**
    * 유효성 상세화면
    */
function OpenQuickValidation(gridid, uid) {
    var rowData = $(gridid).jqxGrid("getrowdata", uid);
    if (!rowData.ModuleCheckFl) { return; }

    var linkName = "[" + rowData.Title + "] 유효성 검증";
    var linkUrl = '/Qms/InfoQuickValidation?OID=' + rowData.ModuleCheckOID;

    TabPageLoad(linkUrl, linkName);
}

/**
    * 표준화 상세화면
    */
function OpenStandardFollowUp(gridid, uid) {
    var rowData = $(gridid).jqxGrid("getrowdata", uid);
    if (!rowData.ModuleStandardFl) { return; }

    var linkName = "[" + rowData.Title + "] 표준화";
    var linkUrl = '/Qms/InfoStandardFollowUp?OID=' + rowData.ModuleStandardOID;

    TabPageLoad(linkUrl, linkName);
}

/**
    * 작업자교육 상세화면
    */
function OpenWorkerEducation(gridid, uid) {
    var rowData = $(gridid).jqxGrid("getrowdata", uid);
    if (!rowData.ModuleWorkerEduFl) { return; }

    var linkName = "[" + rowData.Title + "] 작업자 교육";
    var linkUrl = '/Qms/InfoWorkerEducation?OID=' + rowData.ModuleWorkerEduOID;

    TabPageLoad(linkUrl, linkName);
}

function OpenDlgQuickResponse(QuickResponseOID) {
    var popTitle = "";
    if (QuickResponseOID === null) {
        popTitle = "신규 신속대응 등록";
    } else {
        popTitle = "선택 신속대응 수정";
    }

    var dlgDivWrap = document.createElement("div");

    dlgDivWrap.id = "dlgEditQuickResponse";
    var dlgDivTitle = document.createElement("div");
    var dlgDivView = document.createElement("div");

    dlgDivWrap.appendChild(dlgDivTitle);
    dlgDivWrap.appendChild(dlgDivView);

    var param = {};
    param.OID = QuickResponseOID;

    $(dlgDivView).load("/Qms/EditQuickResponse", param, function () {
        var winHeight = $(window).height();
        var winWidth = $(window).width();

        var posX = (winWidth / 2) - (1100 / 2) + $(window).scrollLeft();
        var posY = (winHeight / 2) - (900 / 2) + $(window).scrollTop();

        $(dlgDivWrap).jqxWindow({
            width: 1100, height: 900, minWidth: 1100, minHeight: 900, draggable: true, resizable: true, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
            initContent: function () {

            }
        });
        $(dlgDivWrap).jqxWindow('setTitle', popTitle);
        $(dlgDivWrap).jqxWindow("show");
    });

    $(dlgDivWrap).on('close', function (event) {
        //_DlgAttributeItem();
        $(dlgDivWrap).jqxWindow('modalDestory');
        //GetAttributeItemList();
    });
}

function OpenDlgQuickResponsePlan(QuickResponseOID) {
    var popTitle = "신속대응 일정수립";

    var dlgDivWrap = document.createElement("div");

    dlgDivWrap.id = "dlgEditQuickResponsePlan";
    var dlgDivTitle = document.createElement("div");
    var dlgDivView = document.createElement("div");

    dlgDivWrap.appendChild(dlgDivTitle);
    dlgDivWrap.appendChild(dlgDivView);

    var param = {};
    param.OID = QuickResponseOID;

    $(dlgDivView).load("/Qms/EditQuickResponsePlan", param, function () {
        var winHeight = $(window).height();
        var winWidth = $(window).width();

        var posX = (winWidth / 2) - (1100 / 2) + $(window).scrollLeft();
        var posY = (winHeight / 2) - (900 / 2) + $(window).scrollTop();

        $(dlgDivWrap).jqxWindow({
            width: 1100, height: 900, minWidth: 1100, minHeight: 900, draggable: true, resizable: true, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
            initContent: function () {

            }
        });
        $(dlgDivWrap).jqxWindow('setTitle', popTitle);
        $(dlgDivWrap).jqxWindow("show");
    });

    $(dlgDivWrap).on('close', function (event) {
        //_DlgAttributeItem();
        $(dlgDivWrap).jqxWindow('modalDestory');
        //GetAttributeItemList();
    });
}

function CloseDlgQuickResponse() {
    $("#dlgEditQuickResponse").jqxWindow("close");
}