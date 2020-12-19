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
    });

    $(popLayer).on('close', function (event) {
        $(popLayer).jqxWindow('modalDestory');
    });
}

// 통합프로젝트 관리 템플릿 불러오기 김창수
function OpenLoadTemplateDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title, Car_Lib_OID) {
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
    });

    $(popLayer).on('close', function (event) {
        $(popLayer).jqxWindow('modalDestory');
    });
}


// Pms GateSign Detail
function OpenGateSignOffDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
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


// Pms Issue
function OpenIssueDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
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
                param.ProjectNm = _Param._param.ProjectNm;
                param.Type = _Param._param.Type;
                param.TaskNm = $('#dlgTaskNm').val();
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
                param.TaskNm = $('#dlgTaskNm').val();
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
    });

    $(popLayer).on('close', function (event) {
        $(popLayer).jqxWindow('modalDestory');
    });
}

//Issue Manager Select Dialog
function OpenIssueManagerDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
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
    });

    $(popLayer).on('close', function (event) {
        $(popLayer).jqxWindow('modalDestory');
    });
}