function OpenPageDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
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
            if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                _CallBackFunction();
            }
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

function OpenApprovalPersonDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
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

            $('#btRegPerson').on('click', function () {
                var memList$ = $("#selectedUserList").find('li');
                var returnList = [];
                for (var index = 0; index < memList$.length; index++) {
                    returnList.push({
                        'OID': memList$.eq(index).attr('OID'),
                        'Name': memList$.eq(index).attr('Name'),
                        'Depart': memList$.eq(index).attr('Depart'),
                        'DepartOID': memList$.eq(index).attr('DepartOID')
                    });   
                }
                if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                    _CallBackFunction(returnList);
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
        if (_Wrap === undefined || _Wrap === null) {
            $(popLayer).jqxWindow('modalDestory');
        }
    });
}

function OpenRoleDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
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
    var posX = (winWidth / 2) - (400 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (350 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 400, maxWidth: 400, height: 350, minHeight: 350, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {

            $('#btnInsRole').on('click', function () {
                const param = {};
                param.Name = $('#txtRoleName').val();

                $.post('/Manage/InsRole', param, function (response) {
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

//Library Logic Dialog _Type = Create, Edit 에대한 액션을 받아온다
function OpenLibraryManipDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title,_Type) {
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
    var posX = (winWidth / 2) - (700 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (560 / 2) + $(window).scrollTop();
    var IsUse ='Y';
    $(popLayer).jqxWindow({
        width: 700, maxWidth: 700, height: 560, minHeight: 560, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {
            if (_Type == "Edit") {
                $('#dlgLibraryManip_AddLib').css("display", "none");
                var param = {};
                param.OID = _Param;
                RequestData("/Manage/SelLibraryObject", param, function (res) {
                    $('#dlgLibraryManip_Name').val(res.Name);
                    $('#dlgLibraryManip_KorNm').val(res.KorNm);
                    $('#dlgLibraryManip_Ord').val(res.Ord);
                    $('#dlgLibraryManip_Description').val(res.Description);
                    if (res.IsUse == "Y") {
                        $('#dlgLibraryManip_IsYes').jqxRadioButton({ width: "40%", height: 25, checked: true });
                        $('#dlgLibraryManip_IsNo').jqxRadioButton({ width: "40%", height: 25, checked: false });
                    } else {
                        $('#dlgLibraryManip_IsYes').jqxRadioButton({ width: "40%", height: 25, checked: false });
                        $('#dlgLibraryManip_IsNo').jqxRadioButton({ width: "40%", height: 25, checked: true });
                    }
                });

            } else if (_Type == "Create") {
                $('#dlgLibraryManip_EditLib').css("display", "none");
                $('#dlgLibraryManip_IsYes').jqxRadioButton({ width: "40%", height: 25, checked: true });
                $('#dlgLibraryManip_IsNo').jqxRadioButton({ width: "40%", height: 25, checked: false });
            }
          

            $("#dlgLibraryManip_IsYes").on('change', function (event) {
                checked = event.args.checked;
                if (checked) {
                    IsUse = "Y";
                }
            });

            $("#dlgLibraryManip_IsNo").on('change', function (event) {
                checked = event.args.checked;
                if (checked) {
                    IsUse = "N";
                }
            });


            $('#dlgLibraryManip_AddLib').on('click', function () {
                const param = {};
                param.Name = $('#dlgLibraryManip_Name').val();
                param.KorNm = $('#dlgLibraryManip_KorNm').val();
                param.Description = $('#dlgLibraryManip_Description').val();
                param.CreateUs = 1;
                param.Ord = $('#dlgLibraryManip_Ord').val();
                
                param.IsUse = IsUse;
                if (_Param == null || _Param == undefined) {
                } else {
                    param.FromOID = _Param;
                }

                if (param.Name == null || param.Name.length < 1) {
                    alert('코드를 입력해주세요.');
                    return;
                }

                if (param.KorNm == null || param.KorNm.length < 1) {
                    alert('이름을 입력해주세요.');
                    return;
                }
                if (param.IsUse == null || param.IsUse.length < 1) {
                    alert('사용여부를 선택해주세요.');
                    return;
                }

                $.post('/Manage/InsertLibrary', param, function (response) {
                    if (response.isError) {
                        alert(response.resultMessage);
                        return;
                    }
                    alert("저장되었습니다.");
                    if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                        _CallBackFunction(response);
                    }
                    $(popLayer).jqxWindow('modalDestory');
                }).fail(function (err) {
                    alert(err.responseText);
                });
            });

            $('#dlgLibraryManip_EditLib').on('click', function () {
                const param = {};
                param.OID = _Param;
                param.Name = $('#dlgLibraryManip_Name').val();
                param.KorNm = $('#dlgLibraryManip_KorNm').val();
                param.Description = $('#dlgLibraryManip_Description').val();
                param.CreateUs = 1;
                param.Ord = $('#dlgLibraryManip_Ord').val();

                param.IsUse = IsUse;
                if (_Param == null || _Param == undefined) {
                } else {
                    param.FromOID = _Param;
                }

                if (param.Name == null || param.Name.length < 1) {
                    alert('코드를 입력해주세요.');
                    return;
                }

                if (param.KorNm == null || param.KorNm.length < 1) {
                    alert('이름을 입력해주세요.');
                    return;
                }
                if (param.IsUse == null || param.IsUse.length < 1) {
                    alert('사용여부를 선택해주세요.');
                    return;
                }

                $.post('/Manage/updateLibrary', param, function (response) {
                    if (response.isError) {
                        alert(response.resultMessage);
                        return;
                    }
                    alert("수정되었습니다.");
                    if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                        _CallBackFunction(param);
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

//Library Logic Dialog _Type = Create, Edit 에대한 액션을 받아온다
function OpenCodeLibraryManipDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title, _Type) {
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
    var posX = (winWidth / 2) - (700 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (560 / 2) + $(window).scrollTop();
    var IsUse = 'Y';
    $(popLayer).jqxWindow({
        width: 700, maxWidth: 700, height: 560, minHeight: 560, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {
            if (_Type == "Edit") {
                $('#dlgCodeLibraryManip_AddLib').css("display", "none");
                var param = {};
                param.OID = _Param;
                RequestData("/Manage/SelCodeLibraryObject", param, function (res) {
                    $('#dlgCodeLibraryManip_Code1').val(res.Code1);
                    $('#dlgCodeLibraryManip_Code2').val(res.Code2);
                    $('#dlgCodeLibraryManip_KorNm').val(res.KorNm);
                    $('#dlgCodeLibraryManip_Ord').val(res.Ord);
                    $('#dlgCodeLibraryManip_Description').val(res.Description);
                    if (res.IsUse == "Y") {
                        $('#dlgCodeLibraryManip_IsYes').jqxRadioButton({ width: "40%", height: 25, checked: true });
                        $('#dlgCodeLibraryManip_IsNo').jqxRadioButton({ width: "40%", height: 25, checked: false });
                    } else {
                        $('#dlgCodeLibraryManip_IsYes').jqxRadioButton({ width: "40%", height: 25, checked: false });
                        $('#dlgCodeLibraryManip_IsNo').jqxRadioButton({ width: "40%", height: 25, checked: true });
                    }
                });

            } else if (_Type == "Create") {
                $('#dlgCodeLibraryManip_EditLib').css("display", "none");
                $('#dlgCodeLibraryManip_IsYes').jqxRadioButton({ width: "40%", height: 25, checked: true });
                $('#dlgCodeLibraryManip_IsNo').jqxRadioButton({ width: "40%", height: 25, checked: false });
            }


            $("#dlgCodeLibraryManip_IsYes").on('change', function (event) {
                checked = event.args.checked;
                if (checked) {
                    IsUse = "Y";
                }
            });

            $("#dlgCodeLibraryManip_IsNo").on('change', function (event) {
                checked = event.args.checked;
                if (checked) {
                    IsUse = "N";
                }
            });


            $('#dlgCodeLibraryManip_AddLib').on('click', function () {
                const param = {};
                param.Code1 = $('#dlgCodeLibraryManip_Code1').val();
                param.Code2 = $('#dlgCodeLibraryManip_Code2').val();
                param.KorNm = $('#dlgCodeLibraryManip_KorNm').val();
                param.Description = $('#dlgCodeLibraryManip_Description').val();
                param.CreateUs = 1;
                param.Ord = $('#dlgCodeLibraryManip_Ord').val();

                param.IsUse = IsUse;
                if (_Param == null || _Param == undefined) {
                } else {
                    param.FromOID = _Param;
                }

                if (param.Code1 == null || param.Code1.length < 1) {
                    alert('코드를 입력해주세요.');
                    return;
                }

                if (param.KorNm == null || param.KorNm.length < 1) {
                    alert('이름을 입력해주세요.');
                    return;
                }
                if (param.IsUse == null || param.IsUse.length < 1) {
                    alert('사용여부를 선택해주세요.');
                    return;
                }

                $.post('/Manage/InsertCodeLibrary', param, function (response) {
                    if (response.isError) {
                        alert(response.resultMessage);
                        return;
                    }
                    alert("저장되었습니다.");
                    if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                        _CallBackFunction(response);
                    }
                    $(popLayer).jqxWindow('modalDestory');
                }).fail(function (err) {
                    alert(err.responseText);
                });
            });

            $('#dlgCodeLibraryManip_EditLib').on('click', function () {
                const param = {};
                param.OID = _Param;
                param.Code1 = $('#dlgCodeLibraryManip_Code1').val();
                param.Code2 = $('#dlgCodeLibraryManip_Code2').val();
                param.KorNm = $('#dlgCodeLibraryManip_KorNm').val();
                param.Description = $('#dlgCodeLibraryManip_Description').val();
                param.CreateUs = 1;
                param.Ord = $('#dlgCodeLibraryManip_Ord').val();

                param.IsUse = IsUse;
                if (_Param == null || _Param == undefined) {
                } else {
                    param.FromOID = _Param;
                }

                if (param.Code1 == null || param.Code1.length < 1) {
                    alert('코드를 입력해주세요.');
                    return;
                }

                if (param.KorNm == null || param.KorNm.length < 1) {
                    alert('이름을 입력해주세요.');
                    return;
                }
                if (param.IsUse == null || param.IsUse.length < 1) {
                    alert('사용여부를 선택해주세요.');
                    return;
                }

                $.post('/Manage/updateCodeLibrary', param, function (response) {
                    if (response.isError) {
                        alert(response.resultMessage);
                        return;
                    }
                    alert("수정되었습니다.");
                    if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                        _CallBackFunction(param);
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


function OpenDepartmentDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
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
    var posX = (winWidth / 2) - (400 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (450 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 400, maxWidth: 400, height: 450, minHeight: 450, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {

            var txtDlgDepartment$ = $('#txtDlgDepartment');
            var txtDlgDescription$ = $('#txtDlgDescription');

            $('#btnDlgSave').on('click', function () {
                var param = {};
                var callUrl = '';
                var callMsg = '';
                if (!WebUtils.isBlank(_Param.parentId)) {
                    param.parentId = _Param.parentId;
                    callUrl = '/Manage/InsDepartment';
                    callMsg = '등록되었습니다.';
                } else {
                    param.id = _Param.id;
                    callUrl = '/Manage/UtpDepartment';
                    callMsg = '수정되었습니다.';
                }

                param.label = txtDlgDepartment$.val()
                param.value = txtDlgDescription$.val();
                                
                if (WebUtils.isBlank(txtDlgDepartment$.val())) {
                    alert('부서명을 등록해주세요.');
                    return;
                }

                $.post(callUrl, param, function (response) {
                    if (response.isError) {
                        alert(response.resultMessage);
                        return;
                    }
                    alert(callMsg);
                    if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                        param.id = response;
                        _CallBackFunction(param);
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

function OpenPersonDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
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
    var posY = (winHeight / 2) - (680 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 800, maxWidth: 800, height: 680, minHeight: 680, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {

            var cbDlgUsTitle$ = $('#cbDlgUsTitle');
            cbDlgUsTitle$.jqxComboBox({ width: '100%', height: 33 });

            var cbDlgUsPosition$ = $('#cbDlgUsPosition');
            cbDlgUsPosition$.jqxComboBox({ width: '100%', height: 33 });

            var dtDlgUsJoinDt$ = $('#dtDlgUsJoinDt');
            var dtHidUsJoinDt$ = $('#hidPersonJoinDt');
            dtDlgUsJoinDt$.jqxDateTimeInput(DateFormat);
            dtDlgUsJoinDt$.jqxDateTimeInput({ value: (dtHidUsJoinDt$.val() != "" ? WebUtils.ToDate(dtHidUsJoinDt$.val()) : new Date()) });
            dtDlgUsJoinDt$.jqxDateTimeInput({ width: '100%', height: 33 });

            var chkHidUseF$ = $('#hidPersonIsUse');
            var chkDlgUseFl_y$ = $('#chkDlgUseFl_y');
            console.log(chkHidUseF$.val());
            chkDlgUseFl_y$.jqxRadioButton({ width: '45%', height: 25, checked: (chkHidUseF$.val() != '' ? chkHidUseF$.val() == 1 ? true : false : true) });
            var chkDlgUseFl_n$ = $('#chkDlgUseFl_n');
            chkDlgUseFl_n$.jqxRadioButton({ width: '45%', height: 25, checked: (chkHidUseF$.val() != '' ? chkHidUseF$.val() == 0 ? true : false : false) });

            $('#btnDlgSave').on('click', function () { 
                var param = {};
                var callUrl = '';
                var callMsg = '';

                if (WebUtils.isBlank($('#hidPersonOID').val())) {
                    callUrl = '/Manage/InsPerson';
                    callMsg = '등록되었습니다.';
                    param.Password = $('#txtDlgLoginPw').val();
                } else {
                    callUrl = '/Manage/UtpPerson';
                    callMsg = '수정되었습니다.';
                    param.OID = $('#hidPersonOID').val();
                }

                param.Name = $('#txtDlgUsNm').val();
                param.ID = $('#txtDlgLoginID').val();
                param.DepartmentOID = $('#hidDepartmentId').val();
                param.DepartmentNm = $('#cbDlgUsDepartment').val();
                param.Email = $('#txtDlgUsEmail').val();
                param.IsUse = chkDlgUseFl_y$.jqxRadioButton('checked') ? 1 : 0;
                param.EnterDt = dtDlgUsJoinDt$.jqxDateTimeInput('val');
                param.Thumbnail = $('#hidPersonImage').val();
                param.ImgSign = $('#hidPersonSign').val();

                if (!WebUtils.isBlank(param.Email)) {
                    if (!WebUtils.emailCheck(param.Email)) {
                        return;
                    }
                }
              
                $.post(callUrl, param, function (response) {
                    if (response.isError) {
                        alert(response.resultMessage);
                        return;
                    }
                    alert(callMsg);
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

function OpenApprovalDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
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
    var posX = (winWidth / 2) - (805 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (795 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 805, maxWidth: 805, height: 795, minHeight: 795, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {

            $('#btnApprov').on('click', function () {
                RequestData('/Common/InsApproval', foDataApprov(), function () {
                    alert('상신되었습니다.');
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
    });

    $(popLayer).on('close', function (event) {
        if (_Wrap === undefined || _Wrap === null) {
            $(popLayer).jqxWindow('modalDestory');
        }
    });
}


function OpenApprovalContentDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
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
    var posX = (winWidth / 2) - (960 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (795 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 960, maxWidth: 960, height: 795, minHeight: 795, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {
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
