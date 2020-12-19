function OpenAddDocClassification(_CallBackFunction, _Wrap, _Param, _Url, _Title, _Mod, _Data) {
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
    var posY = (winHeight / 2) - (500 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 700, maxWidth: 700, height: 500, minHeight: 500, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {
            var FromOIDNm$ = $('#FromOIDNm');
            var Classification$ = $('#Classification');
            var Code$ = $('#Code');
            var Name$ = $('#Name');
            var Description$ = $('#Description');
            
            var DocClassCreate$ = $('#DocClassCreate');
            var DocClassEdit$ = $('#DocClassEdit');
            
            
            if (_Mod == "New") {
                DocClassEdit$.prop('hidden', true);
            
                DocClassCreate$.on('click', function () {
                    var param = {};
                    param.Code = Code$.val();
                    param.Name = Name$.val();
                    param.Description = Description$.val();
                    param.IsUse = IsUseVal;

                    if (param.IsUse == null) {
                        alert("사용여부를 선택해주세요");
                        return;
                    }
                    if (param.Name == null) {
                        alert("분류를 입력해주세요");
                        return;
                    }
            
                    RequestData("/Manage/InsDocumentClassification", param, function (res) {
                        if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                            _CallBackFunction();
                        }
            
                        $(popLayer).jqxWindow('modalDestory');
                    });
                });
            }
            else if (_Mod == "ChildAdd")
            {
                DocClassEdit$.prop('hidden', true);
                FromOIDNm$.val(_Data.Name);
                
                DocClassCreate$.on('click', function () {
                    var param = {};
                    param.FromOID = _Data.OID;
                    param.Code = Code$.val();
                    param.Name = Name$.val();
                    param.Description = Description$.val();
                    param.IsUse = IsUseVal;

                    if (param.IsUse == null) {
                        alert("사용여부를 선택해주세요");
                        return;
                    }
                    if (param.Name == null) {
                        alert("분류를 입력해주세요");
                        return;
                    }

                    RequestData("/Manage/InsDocumentClassification", param, function (res) {
                        if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                            _CallBackFunction();
                        }

                        $(popLayer).jqxWindow('modalDestory');
                    });
                });
            }
            else if (_Mod == "Mod")
            {
                Code$.prop('disabled', 'disabled');
                DocClassCreate$.prop('hidden', true);
                if (_Data.parent != null) {
                    FromOIDNm$.val(_Data.parent.Name);
                }
                Code$.val(_Data.Code);
                Name$.val(_Data.Name);
                Description$.val(_Data.Description);
                if (_Data.IsUse == "Y") {
                    IsUseY$.jqxRadioButton({ width: "45%", height: 25, checked: true });
                    IsUseVal = "Y";
                } else {
                    IsUseN$.jqxRadioButton({ width: "45%", height: 25, checked: true });
                    IsUseVal = "N";
                }

                DocClassEdit$.on('click', function () {
                    var param = {};
                    param.OID = _Data.OID;
                    param.Name = Name$.val();
                    param.Description = Description$.val();
                    param.IsUse = IsUseVal;

                    RequestData("/Manage/UdtDocumentClassification", param, function (res) {
                        if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                            _CallBackFunction();
                        }
                        alert("수정 되었습니다.");
                        $(popLayer).jqxWindow('modalDestory');
                    });
                });
            
                
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

function OpenNewAddDocClassification(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
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
    var posX = (winWidth / 2) - (750 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (800 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 750, maxWidth: 750, height: 800, minHeight: 800, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {
            var param = {};
            var DocClassRowKey = null;
            var dlgDocClassSearchSource =
            {
                dataType: "json",
                dataFields: [
                    { name: 'OID', type: 'number' },
                    { name: 'Name', type: 'string' },

                    { name: 'FromOID', type: 'number' },

                    { name: 'Classification', type: 'string' },
                    { name: 'Code', type: 'string' },
                    { name: 'ViewUrl', type: 'string' },
                    { name: 'IsUse', type: 'string' },
                    { name: 'CreateUsNm', type: 'string' },
                    { name: 'CreateUs', type: 'string' },
                    { name: 'CreateDt', type: 'date' },
                    { name: 'Description', type: 'string' },
                    { name: 'IsRequired', type: 'string' },
                ],
                hierarchy:
                {
                    keyDataField: { name: 'OID' },
                    parentDataField: { name: 'FromOID' }
                },
                id: 'OID',
                addRow: function (rowID, rowData, IsUse, parentID, commit) {
                    commit(true);
                },
            };

            const dlgDocClassSearchGrid$ = $('#dlgDocClassSearchGrid');
            dlgDocClassSearchGrid$.jqxTreeGrid(
                {
                    width: "100%",
                    height: 590,
                    theme: "kdnc",
                    sortable: true,
                    columns: [
                        {
                            text: '분류', datafield: 'Name', width: "100%", align: 'center', cellsalign: 'left',
                            cellsRenderer: function (row, dataField, cellValue, rowData, cellText) {
                                if (rowData.IsRequired == "Y") {
                                    return cellValue + ' <i class="fas fa-lock"></i>';
                                } else {
                                    return cellValue;
                                }
                            }
                        }
                    ],
                    editable: false,
                    showToolbar: true,
                    toolbarHeight: 45,
                    renderToolbar: function (toolbar) {
                        toolbar.empty();
                        var container = $("<div class='lGridComponent'></div>");
                        var btnAdd = $("<button><i class='fas fa-plus'></i> 선택</button>").jqxButton();
                        container.append(btnAdd);
                        toolbar.append(container);

                        btnAdd.on('click', function () {
                            var Insparam = {};
                            Insparam.RootOID = _Param.RootOID;
                            Insparam.FromOID = _Param.FromOID;
                            Insparam.ToOID = DocClassRowKey;

                            if (Insparam.ToOID == null || Insparam.ToOID == "") {
                                alert("문서가 선택이 되지 않았습니다");
                                return;
                            }

                            RequestData("/Pms/InsProjectDocumentClassification", Insparam, function (res) {
                                _CallBackFunction();
                                $(popLayer).jqxWindow('modalDestory');
                            });
                        });
                    }
                });

            dlgDocClassSearchGrid$.on('rowSelect', function (event) {
                if (DocClassRowKey == event.args.key) {
                    dlgDocClassSearchGrid$.jqxTreeGrid('unselectRow', null);
                    DocClassRowKey = null;
                    return;
                }
                else {
                    if (event.args.boundIndex == 0 && event.args.key == 1034) {
                        dlgDocClassSearchGrid$.jqxTreeGrid('unselectRow', null);
                    }
                    DocClassRowKey = event.args.key;
                }
            });

            $('#dlgDocClassSearchBtn').on('click', function () {
                param = {};
                param.Name = $('#dlgDocClassName').val();
                param.OID = 1034;
                param.FromOID = 1034;

                RequestData("/Manage/SelProjectDocumentClassification", param, function (res) {
                    PrintJqxTreeGrid(dlgDocClassSearchSource, dlgDocClassSearchGrid$, res);
                    dlgDocClassSearchGrid$.jqxTreeGrid('expandAll');
                });
            });

            param.OID = 1034;
            param.FromOID = 1034;
            RequestData("/Manage/SelProjectDocumentClassification", param, function (res) {
                PrintJqxTreeGrid(dlgDocClassSearchSource, dlgDocClassSearchGrid$, res);
                dlgDocClassSearchGrid$.jqxTreeGrid('expandAll');
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