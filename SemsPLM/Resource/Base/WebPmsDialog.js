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
    const loading$ = $('#loading');
    loading$.css('display', 'block');
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
    var posX = (winWidth / 2) - (950 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (820 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 950, maxWidth: 950, height: 820, minHeight: 820, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {

            $('#btnDlgCreateProject').on('click', function () {
                const param = {};
                param.Name = $('#txtDlgProjName').val();
                param.ProjectType = $('#dlgProjType').jqxComboBox('getSelectedItem').value;
                param.Oem_Lib_OID = $('#dlgOem').val();
                param.Car_Lib_OID = $('#dlgCar').val();
                param.ITEM_No = $('#dlgItem').val();
                if ($('#dlgItemMiddle').val() == undefined || $('#dlgItemMiddle').val() == "") {
                    param.ITEM_Middle = null;
                } else {
                    param.ITEM_Middle = $('#dlgItemMiddle').val();
                }
                param.ProjectGrade = $('#dlgProjGrade').val();
                param.Customer_OID = $('#dlgCust').val();
                param.ProductNm = $('#txtDlgProductNm').val();
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

            $('#btnDlgModifyProject').on('click', function () {
                const param = {};
                param.OID = _Param.ProjectOID;
                param.ProjectGrade = $('#dlgProjGrade').val();
                param.Description = $('#txtDlgContent').val();
                param.Customer_OID = $('#dlgCust').val();

                RequestData('/Pms/UdtProject', param, function (response) {
                    if (response.isError) {
                        alert(response.resultMessage);
                        return;
                    }
                    alert("수정 되었습니다.");
                    if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                        _CallBackFunction(_Param);
                    }
                    $(popLayer).jqxWindow('modalDestory');
                });

            });
        }
    });

    $(popContent).load(_Url, _Param, function () {
        loading$.css('display', 'none');
        if (_Url.indexOf('ModifyProject') > -1) {
            if ($(popLayer).find('#btnDlgModifyProject').length < 1) {
                $(popLayer).jqxWindow('modalDestory');
                alert('권한이 없습니다.');
                return;
            }
        }
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
    const loading$ = $('#loading');
    loading$.css('display', 'block');
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
        loading$.css('display', 'none');
    });

    $(popLayer).on('close', function (event) {
        if (_Wrap === undefined || _Wrap === null) {
            $(popLayer).jqxWindow('modalDestory');
        }
    });
}

// Pms Create Project content
function OpenCreateProjectContentDialog(_CallBackFunction, _Wrap, _Url, _Param) {
    const loading$ = $('#loading');
    loading$.css('display', 'block');
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
        loading$.css('display', 'none');
    });

    $(popLayer).on('close', function (event) {
        $(popLayer).jqxWindow('modalDestory');
    });
}

// Pms BaseLine Detail
function OpenBaseLineDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
    const loading$ = $('#loading');
    loading$.css('display', 'block');
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
        loading$.css('display', 'none');
    });

    $(popLayer).on('close', function (event) {
        $(popLayer).jqxWindow('modalDestory');
    });
}

// Pms BaseLine Detail
function OpenAddModifyMemberDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
    const loading$ = $('#loading');
    loading$.css('display', 'block');
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
                    var checkIdxes = procResourceInfo$.jqxGrid('selectedrowindexes');
                    if (checkIdxes.length < 1) {
                        alert('선택해주세요.');
                        return;
                    }

                    var checkDataes = [];
                    for (var i = 0, size = checkIdxes.length; i < size; i++) {
                        checkDataes.push(procResourceInfo$.jqxGrid('getrowdata', checkIdxes[i]));
                    }

                    if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                        _CallBackFunction(checkDataes);
                    }
                }
                $(popLayer).jqxWindow('modalDestory');
            });

        }
    });

    $(popContent).load(_Url, _Param, function () {
        $(popLayer).jqxWindow('setTitle', _Title);
        $(popLayer).jqxWindow("show");
        loading$.css('display', 'none');
    });

    $(popLayer).on('close', function (event) {
        $(popLayer).jqxWindow('modalDestory');
    });
}

// 통합프로젝트 관리 템플릿 불러오기 김창수
function OpenLoadTemplateDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title, Car_Lib_OID) {
    const loading$ = $('#loading');
    loading$.css('display', 'block');
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
    var posX = (winWidth / 2) - (1400 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (850 / 2) + $(window).scrollTop();


    $(popLayer).jqxWindow({
        width: 1400, maxWidth: 1400, height: 850, minHeight: 850, resizable: false, zIndex: 99996, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {
            var param = {};
            var Data;
            var mkey; //메인idx

            var ScheduleTemplateParentsource =
            {
                dataType: "json",
                dataFields: [
                    { name: 'OID', type: 'number' },
                    { name: 'Name', type: 'string' },
                    { name: 'KorNm', type: 'string' },
                    { name: 'Ord', type: 'number' },
                    { name: 'isMove', type: 'string' },
                    { name: 'Cdata', type: 'array' },
                    { name: 'isChange', type: 'string' },
                    { name: 'isParentMove', type: 'string' },
                ],
            };
            var initialized = false;
            var ScheduleTemplateParentGrd$ = $('#CustomerScheduleTemplate_ParentGrid');
            ScheduleTemplateParentGrd$.jqxGrid(
                {
                    width: "30%",
                    height: 800,
                    theme: "kdnc",
                    selectionmode: 'checkbox',
                    sortable: true,
                    ready: function () {
                        initialized = true;

                    },
                    columns: [
                        { text: 'OID', datafield: 'OID', type: 'number', align: 'center', hidden: true },
                        { text: 'isChange', datafield: 'isChange', type: 'string', align: 'center', hidden: true },
                        { text: 'isMove', datafield: 'isMove', type: 'string', align: 'center', hidden: true },
                        { text: 'isParentMove', datafield: 'isParentMove', type: 'string', align: 'center', hidden: true },
                        {

                            text: 'NO', width: "10%", cellsalign: 'center', columntype: 'number', align: 'center',
                            cellsrenderer: function (row, column, value) {
                                return "<div style='width:100%;height:100%;text-align:center;vertical-align:middle;line-height:1.9;'>" + (value + 1) + "</div>";
                            }
                        },
                        {
                            text: '템플릿명', datafield: 'Name', type: 'string', width: '83%',
                            cellsrenderer: function (row, column, value) {
                                var item = ScheduleTemplateParentGrd$.jqxGrid('getrowdata', row);
                                if (item.isChange == "Y") {
                                    return "<div class='modifyTag' style='text-align:center;vertical-align:middle;'></div><p style='line-height:32px;text-indent:4px;'>" + value + "</p>";
                                } else {
                                    return "<div style='width:100%;height:100%;text-indent:4px;vertical-align:middle;line-height:32px;'>" + value + "</div>";
                                }
                            }
                        },
                    ],
                    editable: true,
                    editmode: 'dblclick',
                    showToolbar: true,
                    toolbarHeight: 45,

                    renderToolbar: function (statusbar) {
                        statusbar.empty();
                        var container$ = $('<div class="lGridComponent"></div>');
                        var LoadButton = $("<button class='custom-button'><i class='fas fa-plus'></i> 불러오기</button>").jqxButton();
                        container$.append(LoadButton);
                        statusbar.append(container$);

                        LoadButton.on('click', function () {
                            var Check = ScheduleTemplateParentGrd$.jqxGrid('getselectedrowindexes');
                            if (Check.length <= 0) {
                                alert("템플릿을 선택해 주세요");
                                return;
                            }

                            if (Check.length > 1) {
                                alert("템플릿을 하나만 선택해 주세요");
                                return;
                            }

                            var LoadData = ScheduleTemplateChildGrd$.jqxGrid('getrows');
                            for (var i = 0; i < LoadData.length; i++) {
                                _Param.jqxGrid('addrow', null, {
                                    "Car_Lib_OID": Car_Lib_OID,
                                    "Name": LoadData[i].Name,
                                    "Ord": null,
                                    "StartDt": null
                                }, 'last');
                            }

                            $(popLayer).jqxWindow('modalDestory');
                        });
                    }
                });
            getScheduleTemplateList(ScheduleTemplateParentsource, ScheduleTemplateParentGrd$, param);

            var ScheduleTemplateChildsource =
            {
                dataType: "json",
                dataFields: [
                    { name: 'OID', type: 'number' },
                    { name: 'Name', type: 'string' },
                    { name: 'KorNm', type: 'string' },
                    { name: 'Ord', type: 'number' },
                    { name: 'IsUse', type: 'string' },
                    { name: 'isChange', type: 'string' },
                ],
            };

            var initialized = false;
            var ScheduleTemplateChildGrd$ = $('#CustomerScheduleTemplate_ChildGrid');
            ScheduleTemplateChildGrd$.jqxGrid('refreshdata');
            ScheduleTemplateChildGrd$.jqxGrid(
                {
                    width: "68%",
                    height: 800,
                    theme: "kdnc",
                    //selectionmode: 'checkbox',
                    sortable: true,
                    ready: function () {
                        initialized = true;

                    },
                    columns: [
                        { text: 'OID', datafield: 'OID', type: 'number', cellsalign: 'center', hidden: true },
                        {

                            text: 'NO', width: "10%", cellsalign: 'center', columntype: 'number', align: 'center',
                            cellsrenderer: function (row, column, value) {
                                return "<div style='width:100%;height:100%;text-align:center;vertical-align:middle;line-height:1.9;'>" + (value + 1) + "</div>";
                            }
                        },
                        {
                            text: '고객일정명', width: '90%', datafield: 'Name', type: 'string', cellsrenderer: function (row, column, value) {
                                var item = ScheduleTemplateChildGrd$.jqxGrid('getrowdata', row);
                                if (item.isChange == "Y") {
                                    return "<div class='modifyTag' style='text-align:center;vertical-align:middle;'></div><p style='line-height:32px;text-indent:4px;'>" + value + "</p>";
                                } else {
                                    return "<div style='width:100%;height:100%;text-indent:4px;vertical-align:middle;line-height:32px;'>" + value + "</div>";
                                }
                            },
                        },

                    ],
                    editable: true,
                    editmode: 'dblclick',
                    showToolbar: true,
                    toolbarHeight: 45,
                    renderToolbar: function (statusbar) {
                        statusbar.empty();
                        var container$ = $('<div class="lGridComponent"></div>');
                        statusbar.append(container$);
                    }

                });
            ScheduleTemplateParentGrd$.on('rowselect', function (event) {
                var args = event.args;
                mkey = args.rowindex;
                var item = ScheduleTemplateParentGrd$.jqxGrid('getrowdata', args.rowindex);
                ScheduleTemplateChildGrd$.jqxGrid('clearselection');
                PrintJqxGrid(ScheduleTemplateChildsource, ScheduleTemplateChildGrd$, item.Cdata);
            });
            ScheduleTemplateParentGrd$.on('cellclick', function (event) {
                var args = event.args;
                mkey = args.rowindex;
                var item = ScheduleTemplateParentGrd$.jqxGrid('getrowdata', args.rowindex);
                ScheduleTemplateChildGrd$.jqxGrid('clearselection');
                PrintJqxGrid(ScheduleTemplateChildsource, ScheduleTemplateChildGrd$, item.Cdata);
            });
        }
    });

    function getScheduleTemplateList(_Source, _Grid$, _param) {
        RequestData("/Pms/SelCustomerScheduleTemplate", _param, function (res) {
            Data = res;
            PrintJqxGrid(_Source, _Grid$, Data);
        });
    }

    $(popContent).load(_Url, _Param, function () {
        $(popLayer).jqxWindow('setTitle', _Title);
        $(popLayer).jqxWindow("show");
        loading$.css('display', 'none');
    });

    $(popLayer).on('close', function (event) {
        $(popLayer).jqxWindow('modalDestory');
    });
}


// Pms GateSign Detail
function OpenGateSignOffDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
    const loading$ = $('#loading');
    loading$.css('display', 'block');

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
    var posX = (winWidth / 2) - (1250 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (850 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 1250, maxWidth: 1250, height: 850, minHeight: 850, resizable: false, zIndex: 99996, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {
        }
    });

    $(popContent).load(_Url, _Param, function () {
        $(popLayer).jqxWindow('setTitle', _Title);
        $(popLayer).jqxWindow("show");
        loading$.css('display', 'none');
    });

    $(popLayer).on('close', function (event) {
        $(popLayer).jqxWindow('modalDestory');
    });
}


// Pms Issue
function OpenIssueDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
    const loading$ = $('#loading');
    loading$.css('display', 'block');
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
    var posX = (winWidth / 2) - (1200 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (800 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 1200, maxWidth: 1200, height: 800, minHeight: 800, resizable: false, zIndex: 99996, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {
            divId = $(popLayer);
            $('#dlgCreateIssue').on('click', function (event) {
                var param = {};
                param.FromOID = _Param.FromOID;
                param.RootOID = _Param.RootOID;
                param.Name = $('#dlgIssueNm').val();
                param.Type = _Param.Type;
                param.Importance = $('#importance_Good').jqxRadioButton('checked') ? 3 : $('#importance_Aver').jqxRadioButton('checked') ? 2 : 1;
                param.EstFinDt = $('#issueEstimatedFinDate').val();
                param.Description = $('#dlgDescription').val();
                param.Manager_OID = $('#dlgMangerOID').val();
                var chkIssueType = [];
                for (var i = 0; i < $('input[name="IssueType"]').length; i++) {
                    if ($('input[name="IssueType"]')[i].value == "true") {
                        chkIssueType.push($('div[name="IssueType"]').eq(i).data('value'));
                    }
                }
                param.IssueType = chkIssueType.join(',');
                RequestData('/Pms/InsIssue',param, function (response) {
                    if (response.isError) {
                        alert(response.resultMessage);
                        return;
                    }
                    alert("저장되었습니다.");
                    if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                        _CallBackFunction(_Param);
                    }
                    $(popLayer).jqxWindow('modalDestory');
                });
            });


            $('#dlgCloseBtn').on('click', function (event) {
                $(popLayer).jqxWindow('modalDestory');
            });

            $('#dlgSaveContents').on('click', function (event) {
                var param = {};
                param.TYPE = _Param.Type;
                param.OID = _Param.OID;
                param.Contents = $('#dlgContents').val();
                var Files = fileUpload.Files();

                var removeFiles = fileUpload.RemoveFiles();
                if (!WebUtils.isEmpty(removeFiles)) {
                    param.delFiles = [];
                    param.delFiles = removeFiles;
                }

                SendDataWithFile('/Pms/UdtIssue', param,Files, function (response) {
                    if (response.isError) {
                        alert(response.resultMessage);
                        return;
                    }
                    alert("저장되었습니다.");
                    if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                        _CallBackFunction(_Param);
                    }
                });
            });

            $('#dlgSaveIssue').on('click', function (event) {
                var param = {};
                param.TYPE = _Param.Type;
                param.OID = _Param.OID;
                param.Name = $('#dlgIssueNm').val();
                param.Importance = $('#importance_Good').jqxRadioButton('checked') ? 3 : $('#importance_Aver').jqxRadioButton('checked') ? 2 : 1;
                param.EstFinDt = $('#issueEstimatedFinDate').val();
                param.Description = $('#dlgDescription').val();
                param.Manager_OID = $('#dlgMangerOID').val();
                param.BPolicyNm = $('#dlgStatus').val();
                var chkIssueType = [];
                for (var i = 0; i < $('input[name="IssueType"]').length; i++) {
                    if ($('input[name="IssueType"]')[i].value == "true") {
                        chkIssueType.push($('div[name="IssueType"]').eq(i).data('value'));
                    }
                }
                param.IssueType = chkIssueType.join(',');

                var Files = fileUpload.Files();

                var removeFiles = fileUpload.RemoveFiles();
                if (!WebUtils.isEmpty(removeFiles)) {
                    param.delFiles = [];
                    param.delFiles = removeFiles;
                }

                SendDataWithFile('/Pms/UdtIssue', param, Files, function (response) {
                    if (response.isError) {
                        alert(response.resultMessage);
                        return;
                    }
                    alert("저장되었습니다.");
                    if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                        _CallBackFunction(_Param);
                    }
                    $(popLayer).jqxWindow('modalDestory');
                });
            });

        }
    });

    $(popContent).load(_Url, _Param, function () {
        $(popLayer).jqxWindow('setTitle', _Title);
        $(popLayer).jqxWindow("show");
        loading$.css('display', 'none');
    });

    $(popLayer).on('close', function (event) {
        $(popLayer).jqxWindow('modalDestory');
    });
}

//Issue Manager Select Dialog
function OpenIssueManagerDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
    const loading$ = $('#loading');
    loading$.css('display', 'block');
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
            var dlgMemberGrd$ = $("#dlgMemberInfo");
            $('#dlgSelMember').on('click', function () {
                if (_CallBackFunction !== null && typeof _CallBackFunction == 'function') {
                    var checkIdx = dlgMemberGrd$.jqxGrid('getselectedrowindex');
                    if (checkIdx == null || checkIdx == -1) {
                        alert('선택해주세요.');
                        return;
                    }
                    var rowData = dlgMemberGrd$.jqxGrid('getrowdata', checkIdx);
                    $('#dlgMangerOID').val(rowData.ToOID);
                    $('#dlgManager').val(rowData.PersonNm);

                    if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                        _CallBackFunction(rowData);
                    }
                }
                $(popLayer).jqxWindow('modalDestory');
            });

        }
    });

    $(popContent).load(_Url, _Param, function () {
        $(popLayer).jqxWindow('setTitle', _Title);
        $(popLayer).jqxWindow("show");
        loading$.css('display', 'none');
    });

    $(popLayer).on('close', function (event) {
        $(popLayer).jqxWindow('modalDestory');
    });
}

//Pms GateView 회의록
function OpenAddModifyGateViewDetailDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
    const loading$ = $('#loading');
    loading$.css('display', 'block');
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
    var posX = (winWidth / 2) - (1800 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (650 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 1800, maxWidth: 1800, height: 650, minHeight: 650, resizable: false, zIndex: 99996, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {

        }
    });

    $(popContent).load(_Url, _Param, function () {
        $(popLayer).jqxWindow('setTitle', _Title);
        $(popLayer).jqxWindow("show");
        loading$.css('display', 'none');
    });

    $(popLayer).on('close', function (event) {
        $(popLayer).jqxWindow('modalDestory');
    });
}


// Pms 신뢰성 시험 의뢰서
function OpenReliabilityDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
    const loading$ = $('#loading');
    loading$.css('display', 'block');
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
    var posX = (winWidth / 2) - (1800 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (850 / 2) + $(window).scrollTop();


    $(popLayer).jqxWindow({
        width: 1800, maxWidth: 1800, height: 850, minHeight: 850, resizable: false, zIndex: 99996, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {
            $('#dlgSaveBtn').on('click', function () {
                var param = {};
                param.RootOID = _Param.RootOID;
                param.FromOID = _Param.FromOID;

                param.Description = $('#Description').val();

                param.RequiredSchedule = $('#RequiredSchedule').val();
                param.DevStep = DevStep$.val();
                param.TestStandard = $('#TestStandard').val();
                param.RegNum = $('#RegNum').val();
                //param.PartNo = $('#PartNoPartNo'); //
                //param.CarType = $('#CarTypeCarType'); //SYSTEM
                param.TestMethodDt = TestMethodDt$.val(); //달력
                param.NewVer = $('#NewVer').val();
                param.HWVer = $('#HWVer').val();
                param.SWVer = $('#SWVer').val();
                param.CANVer = $('#CANVer').val();
                param.TestApplyVer = $('#TestApplyVer').val();
                param.TestCarType = $('#TestCarType').val();
                param.TestPurpose = $('#TestPurpose').val();
                param.TestContents = $('#TestContents').val();
                param.SampleQuantity = $('#SampleQuantity').val();
                param.TestStandardContents = $('#TestStandardContents').val();
                param.Requirements = $('#Requirements').val();

                RequestData('/Pms/InsReliability', { _param: param, _ItemListParam: TestItemListGrid$.jqxGrid('getrows') }, function (response) {
                    if (response.isError) {
                        alert(response.resultMessage);
                        return;
                    }
                    alert("저장되었습니다.");
                    if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                        _CallBackFunction();
                    }
                    $(popLayer).jqxWindow('modalDestory');
                });
            });
        }
    });

    $(popContent).load(_Url, _Param, function () {
        $(popLayer).jqxWindow('setTitle', _Title);
        $(popLayer).jqxWindow("show");
        loading$.css('display', 'none');
    });

    $(popLayer).on('close', function (event) {
        $(popLayer).jqxWindow('modalDestory');
    });
}


// Pms Gate CheckList
function OpenGateCheckListDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
    const loading$ = $('#loading');
    loading$.css('display', 'block');
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
    var posX = (winWidth / 2) - (1500 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (660 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 1500, maxWidth: 1500, height: 650, minHeight: 650, resizable: false, zIndex: 99996, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {
        }
    });

    $(popContent).load(_Url, _Param, function () {
        $(popLayer).jqxWindow('setTitle', _Title);
        $(popLayer).jqxWindow("show");
        loading$.css('display', 'none');
    });

    $(popLayer).on('close', function (event) {
        $(popLayer).jqxWindow('modalDestory');
    });
}
// pms Document Dialog
function OpenPmsDocumentDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
    const loading$ = $('#loading');
    loading$.css('display', 'block');
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
    var posX = (winWidth / 2) - (1200 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (800 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 1200, height: 800, minHeight: 800, maxWidth: 1200, resizable: false, zIndex: 99996, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {
            $('#dlgCreateDocument_createbtn').on('click', function () {
                const param = {};

                param.Title = $('#dlgCreateDocument_Title').val();                 //제목
                param.DocType = $('#dlgCreateDocument_DocClassOID').val();  //문서분류
                param.Description = $('#dlgCreateDocument_Description').val();     //설명
                param.Files = documentFile.Files();
                param.RootOID = _Param.RootOID;
                param.FromOID = _Param.FromOID;
                if (param.RootOID == param.FromOID) {

                } else {
                    param.TaskOID = param.FromOID;
                }
                var removeFiles = documentFile.RemoveFiles();
                if (!WebUtils.isEmpty(removeFiles)) {
                    param.delFiles = [];
                    param.delFiles = removeFiles;
                }
                if (param.Title == null || param.Title.length < 1) {
                    alert('제목을 입력해주세요.');
                    return;
                }
                SendDataWithFile('/Pms/InsertPmsDocument', param, null, function (response) {
                    if (response.isError) {
                        alert(response.resultMessage);
                        return;
                    }
                    alert("저장되었습니다.");
                    if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                        _CallBackFunction();
                    }
                    $(popLayer).jqxWindow('modalDestory');
                });
            });
            
            $('#dlgInfoDocument_canclebtn, #dlgCreateDocument_canclebtn').on('click', function () {
                $(popLayer).jqxWindow('modalDestory');
            });
            $('#dlgInfoDocument_savebtn').on('click', function () {
                if (confirm('수정하시겠습니까?')) {
                    const param = {};
                    param.OID = _Param.OID;
                    param.Title = $('#dlgInfoDocument_Title').val();                 //제목
                    param.Description = $('#dlgInfoDocument_Description').val();     //설명
                    param.Files = documentFile.Files();
                    param.Type = _Param.Type;
                    var removeFiles = documentFile.RemoveFiles();
                    if (!WebUtils.isEmpty(removeFiles)) {
                        param.delFiles = [];
                        param.delFiles = removeFiles;
                    }
                    if (param.Title == null || param.Title.length < 1) {
                        alert('제목을 입력해주세요.');
                        return;
                    }
                    SendDataWithFile('/Document/UdtDocument', param, null, function (response) {
                        if (response.isError) {
                            alert(response.resultMessage);
                            return;
                        }
                        alert("수정되었습니다.");
                        if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                            _CallBackFunction();
                        }
                        $(popLayer).jqxWindow('modalDestory');
                    });
                }
            });
        }
    });
    $(popContent).load(_Url, _Param, function () {
        $(popLayer).jqxWindow('setTitle', _Title);
        $(popLayer).jqxWindow("show");
        loading$.css('display', 'none');
    });

    $(popLayer).on('close', function (event) {
        $(popLayer).jqxWindow('modalDestory');
    });
}