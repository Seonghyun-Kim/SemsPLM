function PmsNewTabClick(ProjectOID, ProjectNm, url, Oid, Name) {
    const linkUrl = url + '?ProjectOID=' + ProjectOID + "&OID=" + Oid;
    if ($("div[tabUrl='" + linkUrl + "']").length > 0) {
        var tabDiv = $("div[tabUrl='" + linkUrl + "']")[0].parentNode;
        $('#tabContent').jqxTabs('select', $(tabDiv).index());
        return;
    }

    const loading$ = $('#loading');
    const tabContent$ = $('#tabContent');
    loading$.css('display', 'block');
    var linkName = '[' + ProjectNm + '] ' + Name;
    tabContent$.jqxTabs('addLast', linkName, "");
    var tabLength = tabContent$.jqxTabs('length');
    $.ajax({
        url: linkUrl,
        type: 'get',
        dataType: 'html',
        async: true,
        success: function (resHtml) {
            var content = tabContent$.jqxTabs('getContentAt', tabLength - 1);
            $(content).html("<div class='wrapPage' tabUrl='" + linkUrl + "'>" + resHtml + "</div>");
        }, error: function (res) {
            if (res.status === 404) {
                alert("현재 개발중인 화면입니다.");
                return false;
            }
            alert(res.responseText);
            console.log(res);
        }
        , complete: function () {
            loading$.css('display', 'none');
        }
    });
}

//Pms Create Dialog
function OpenPmsCreateDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
    var popLayer = document.createElement("div");
    popLayer.style.display = "none";

    var popTitle = document.createElement("div");
    var popContent = document.createElement("div");

    popLayer.appendChild(popTitle);
    popLayer.appendChild(popContent);

    if (_Wrap === undefined || _Wrap === null) {
        document.body.appendChild(popLayer);
    } else {
        _Wrap.appendChild(popLayer);
    }

    var winHeight = $(window).height();
    var winWidth = $(window).width();
    var posX = (winWidth / 2) - (800 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (650 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 800, maxWidth: 800, height: 650, minHeight: 650, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {

            $('#btnDlgCreateProject').on('click', function () {
                const param = {};
                param.Name = $('#txtDlgProjName').val();
                param.ProjectType = $('#dlgProjType').jqxComboBox('getSelectedItem').value;
                param.BaseDt = $('#txtDlgProjDate').jqxDateTimeInput('val');
                param.CalendarOID = $('#hidCalendarOID').val();
                param.Description = $('#txtDlgContent').val();
                param.ProductOID = 0;
                param.ProdecessorOID = 0;
                param.WorkingDay = $("#hidWorkingDay").val();
                param.ProdecessorOID = $("#hidProdecessorProject").val();

                if (param.ProjectType == null || param.ProjectType.length < 1) {
                    alert('구분을 입력해주세요.');
                    return;
                }

                if ((param.TemplateOID == null || param.TemplateOID.length < 1) && (param.WorkingDay == null || param.WorkingDay.length < 1)) {
                    alert('달력을 입력해주세요.');
                    return;
                }

                $.post('/Pms/InsProject', param, function (response) {
                    if (response.isError) {
                        alert(response.resultMessage);
                        return;
                    }
                    alert("저장되었습니다.");
                    if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                        _CallBackFunction();
                    }
                    $(popLayer).jqxWindow('modalDestory');
                }).fail(function (err) {
                    alert(err.responseText);
                });

            });
        }
    });

    $(popContent).load(_Url, _Param, function () {
        $(popLayer).jqxWindow('setTitle', _Title);
        $(popLayer).jqxWindow("show");
    });

    $(popLayer).on('close', function (event) {
        if (_Wrap === undefined || _Wrap === null) {
            $(popLayer).jqxWindow('modalDestory');
        }
    });
}

//Pms Temp Create Dialog
function OpenPmsTmpCreateDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
    var popLayer = document.createElement("div");
    popLayer.style.display = "none";

    var popTitle = document.createElement("div");
    var popContent = document.createElement("div");

    popLayer.appendChild(popTitle);
    popLayer.appendChild(popContent);

    if (_Wrap === undefined || _Wrap === null) {
        document.body.appendChild(popLayer);
    } else {
        _Wrap.appendChild(popLayer);
    }

    var winHeight = $(window).height();
    var winWidth = $(window).width();
    var posX = (winWidth / 2) - (800 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (650 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 800, maxWidth: 800, height: 650, minHeight: 650, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {

            $('#btnDlgTmpCreateProject').on('click', function () {
                const param = {};
                param.Name = $('#txtDlgTmpProjName').val();
                param.ProjectType = $('#hidIsTemplateNm').val();
                param.BaseDt = $('#txtDlgTmpProjDate').jqxDateTimeInput('val');
                param.CalendarOID = $('#hidTmpCalendarOID').val();
                param.Description = $('#txtDlgTmpContent').val();
                param.ProductOID = 0;
                param.ProdecessorOID = 0;
                param.WorkingDay = $("#hidTmpWorkingDay").val();
                param.Type = $('#hidIsTemplate').val();
                param.IsTemplate = param.Type;

                if ((param.TemplateOID == null || param.TemplateOID.length < 1) && (param.WorkingDay == null || param.WorkingDay.length < 1)) {
                    alert('달력을 입력해주세요.');
                    return;
                }

                $.post('/Pms/InsProject', param, function (response) {
                    if (response.isError) {
                        alert(response.resultMessage);
                        return;
                    }
                    alert("저장되었습니다.");
                    if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                        _CallBackFunction();
                    }
                    $(popLayer).jqxWindow('modalDestory');
                }).fail(function (err) {
                    alert(err.responseText);
                });

            });
        }
    });

    $(popContent).load(_Url, _Param, function () {
        $(popLayer).jqxWindow('setTitle', _Title);
        $(popLayer).jqxWindow("show");
    });

    $(popLayer).on('close', function (event) {
        if (_Wrap === undefined || _Wrap === null) {
            $(popLayer).jqxWindow('modalDestory');
        }
    });
}

// Pms Create Project content
function OpenCreateProjectContentDialog(_CallBackFunction, _Wrap, _Url, _Param) {
    var popLayer = document.createElement("div");
    popLayer.style.display = "none";

    var popTitle = document.createElement("div");
    var popContent = document.createElement("div");

    popLayer.appendChild(popTitle);
    popLayer.appendChild(popContent);

    if (_Wrap === undefined || _Wrap === null) {
        document.body.appendChild(popLayer);
    } else {
        wrap.appendChild(popLayer);
    }

    var winHeight = $(window).height();
    var winWidth = $(window).width();
    var posX = (winWidth / 2) - (800 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (650 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 800, height: 650, minHeight: 650, maxWidth: 1500, resizable: false, zIndex: 99996, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {

            $('#btnDlgSelContent').on('click', function () {
                if (_CallBackFunction !== null && typeof _CallBackFunction == 'function') {
                    if (checkData == undefined || checkData == null) {
                        alert('선택해주세요.');
                        return;
                    }
                    if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                        _CallBackFunction(checkData);
                    }
                }
                $(popLayer).jqxWindow('modalDestory');
            });
        }
    });

    $(popContent).load(_Url, _Param, function () {
        $(popLayer).jqxWindow('setTitle', _Param.Title);
        $(popLayer).jqxWindow("show");
    });

    $(popLayer).on('close', function (event) {
        $(popLayer).jqxWindow('modalDestory');
    });
}

// Pms BaseLine Detail
function OpenBaseLineDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
    var popLayer = document.createElement("div");
    popLayer.style.display = "none";

    var popTitle = document.createElement("div");
    var popContent = document.createElement("div");

    popLayer.appendChild(popTitle);
    popLayer.appendChild(popContent);

    if (_Wrap === undefined || _Wrap === null) {
        document.body.appendChild(popLayer);
    } else {
        wrap.appendChild(popLayer);
    }

    var winHeight = $(window).height();
    var winWidth = $(window).width();
    var posX = (winWidth / 2) - (1600 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (650 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 1600, maxWidth: 1600, height: 650, minHeight: 650, resizable: false, zIndex: 99996, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {

        }
    });

    $(popContent).load(_Url, _Param, function () {
        $(popLayer).jqxWindow('setTitle', _Title);
        $(popLayer).jqxWindow("show");
    });

    $(popLayer).on('close', function (event) {
        $(popLayer).jqxWindow('modalDestory');
    });
}

// Pms BaseLine Detail
function OpenAddModifyMemberDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
    var popLayer = document.createElement("div");
    popLayer.style.display = "none";

    var popTitle = document.createElement("div");
    var popContent = document.createElement("div");

    popLayer.appendChild(popTitle);
    popLayer.appendChild(popContent);

    if (_Wrap === undefined || _Wrap === null) {
        document.body.appendChild(popLayer);
    } else {
        wrap.appendChild(popLayer);
    }

    var winHeight = $(window).height();
    var winWidth = $(window).width();
    var posX = (winWidth / 2) - (900 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (650 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 900, maxWidth: 900, height: 650, minHeight: 650, resizable: false, zIndex: 99996, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {
            var procResourceInfo$ = $("#procResourceInfo");
            $('#btnIntProcResource').on('click', function () {
                if (_CallBackFunction !== null && typeof _CallBackFunction == 'function') {
                    var checkIdx = procResourceInfo$.jqxGrid('selectedrowindex');
                    var checkData = procResourceInfo$.jqxGrid('getrowdata', checkIdx);
                    if (checkData == undefined || checkData == null) {
                        alert('선택해주세요.');
                        return;
                    }
                    if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                        _CallBackFunction(checkData);
                    }
                }
                $(popLayer).jqxWindow('modalDestory');
            });

        }
    });

    $(popContent).load(_Url, _Param, function () {
        $(popLayer).jqxWindow('setTitle', _Title);
        $(popLayer).jqxWindow("show");
    });

    $(popLayer).on('close', function (event) {
        $(popLayer).jqxWindow('modalDestory');
    });
}
