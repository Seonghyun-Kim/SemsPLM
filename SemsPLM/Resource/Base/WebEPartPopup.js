function OpenSearchEBomTreeDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title, _Name, _Id, key, _Action, _SelectRow) {
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
    var posX = (winWidth / 2) - (1400 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (750 / 2) + $(window).scrollTop();

    //var EBomStructureParam = {};
    //EBomStructureParam.Name = _Name;

    var rowKey = null;

    

    $(popLayer).jqxWindow({
        width: 1400, maxWidth: 1400, height: 750, minHeight: 750, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {

            if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                _CallBackFunction();
            }

            var EBomStructureSource =
            {
                dataType: "json",
                dataFields: [
                    { name: 'OID', type: 'number' },
                    { name: 'FromOID', type: 'number' },
                    { name: 'ToOID', type: 'number' },
                    { name: 'Children', type: 'array' },
                    { name: 'Ord', type: 'number' },
                    { name: 'Count', type: 'number' },

                    { name: 'ObjRevision', type: 'text' },
                    { name: 'ObjTdmxOID', type: 'text' },
                    { name: 'ObjIsLatest', type: 'number' },
                    { name: 'ObjEPartType', type: 'text' },
                    { name: 'Level', type: 'number' },
                    { name: 'ObjName', type: 'text' },
                    { name: 'ObjThumbnail', type: 'text' },
                    { name: 'ObjCar_Lib_OID', type: 'number' },
                    { name: 'ObjCar_Lib_Nm', type: 'text' },
                    { name: 'ObjTitle', type: 'text' },
                    { name: 'ObjITEM_No', type: 'text' },
                    { name: 'ObjITEM_NoNm', type: 'text' },
                    { name: 'ObjMaterial_OID', type: 'text' },
                    { name: 'ObjMaterial_Nm', type: 'text' },
                    { name: 'ObjITEM_Middle', type: 'text' },              
                    { name: 'ObjITEM_MiddleNm', type: 'text' },
                    { name: 'ObjProduction_Place', type: 'text' },
                    { name: 'ObjProduction_PlaceNm', type: 'text' },
                    { name: 'ObjBlock_No', type: 'text' },
                    { name: 'ObjBlock_NoNm', type: 'text' },
                    { name: 'ObjSerial', type: 'text' },
                    { name: 'ObjSel_Revision', type: 'text' },

                ],
                hierarchy:
                {
                    root: 'Children',
                },
                id: 'OID',
                addRow: function (rowID, rowData, position, parentID, commit) {
                    newRowID = rowID;
                    commit(true);
                },
                updateRow: function (rowID, rowData, commit) {
                    commit(true);
                }
            };
            var dataAdapter = new $.jqx.dataAdapter(EBomStructureSource);
            const digSearchEBomStructureGrid$ = $('#digSearchEBomStructureGrid');
            digSearchEBomStructureGrid$.jqxTreeGrid('render');
            digSearchEBomStructureGrid$.jqxTreeGrid({
                width: "100%",
                theme: "kdnc",
                height: 510,
                sortable: true,
                showToolbar: true,
                toolbarHeight: 44,
                editable: false,
                source: dataAdapter,
                ready: function () {

                },
                columns: [
                    { name: 'Level', dataField: 'Level', type: 'number', width: '9%' },

                    { name: 'OID', dataField: 'OID', type: 'text', align: 'center', cellsalign: 'center', text: 'OID', width: '9%', hidden:'true' },
                    { name: 'FromOID', dataField: 'FromOID', type: 'text', align: 'center', cellsalign: 'center', text: 'FromOID', width: '9%', hidden: 'true' },
                    { name: 'ToOID', dataField: 'ToOID', type: 'text', align: 'center', cellsalign: 'center', text: 'ToOID', width: '9%', hidden: 'true' },

                    { name: 'ObjCar_Lib_Nm', dataField: 'ObjCar_Lib_Nm', align: 'center', cellsalign: 'center', type: 'text', text: '차종', width: '14%' },
                    { name: 'ObjName', id: 'ObjName', dataField: 'ObjName', align: 'center', cellsalign: 'center', type: 'text', text: '품번', width: '14%' },
                    { name: 'ObjITEM_NoNm', dataField: 'ObjITEM_NoNm', align: 'center', cellsalign: 'center', type: 'text', text: 'ITEM_NO', width: '14%' },
                    { name: 'ObjMaterial_Nm', dataField: 'ObjMaterial_Nm', align: 'center', cellsalign: 'center', type: 'text', text: '재질', width: '11%' },
                    { name: 'ObjBlock_NoNm', dataField: 'ObjBlock_NoNm', align: 'center', cellsalign: 'center', type: 'text', text: 'Block_No', width: '14%' },
                    { name: 'Ord', dataField: 'Ord', type: 'number', align: 'center', cellsalign: 'center', text: '순서', width: '6%' },
                    { name: 'Count', dataField: 'Count', align: 'center', cellsalign: 'center', type: 'number', text: '수량', width: '6%' },
                    {
                        name: 'ObjThumbnail', dataField: 'ObjThumbnail', align: 'center', type: 'text', text: '이미지', width: '12%',
                        cellsrenderer: function (row, column, value) {
                            if (value.length > 1) {
                                return "<div class='ebomImg'><img src='~/images/Thumbnail/" + value + "'></div>";
                            } else {
                                return ""
                            }
                        }
                    }

                ],
                rendertoolbar: function (toolBar) {
                    var container = $("<div class='lGridComponent' ></div>");
                    var AddButton = $("<button class='custom-button'><i class='fas fa-stream'></i> 추가</button>").jqxButton();
                    var CreateButton = $("<button class='custom-button'><i class='fas fa-plus'></i> 생성</button>").jqxButton();
                    container.append(AddButton);
                    container.append(CreateButton);
                    toolBar.append(container);
                    
                    AddButton.click(function (event) {
                        digSearchEBomStructureGrid$.jqxTreeGrid('expandRow', rowKey);
                        const SelectData = digSearchEBomStructureGrid$.jqxTreeGrid('getRow', rowKey);
                        if (SelectData == null) {
                            alert("값을 선택해주세요");
                            return;
                        }
                        var parentData = _Id.jqxTreeGrid('getRow', key);

                        
                        if (_Action == "A") {
                            SelectData.Action = "A";
                            SelectData.FromOID = parentData.ToOID;

                            SelectData.Ord = 1;
                            SelectData.Count = 1;

                            ffEPartArray([SelectData], Number(parentData.Level) + 1);
                            
                            parentData.Children.push(SelectData);
                            
                        } else if (_Action == "RU") {
                            SelectData.Action = "RU";
                            SelectData.OldOID = parentData.OID;
                            SelectData.FromOID = parentData.FromOID;

                            SelectData.Ord = parentData.Ord;
                            SelectData.Count = 1;

                            
                            ffEPartArray([SelectData], Number(parentData.parent.Level) + 1);

                            //_Id.jqxTreeGrid('updateRow', key, SelectData);

                            //parentData = _Id.jqxTreeGrid('getRow', key);
                            //parentData.Children.push(SelectData.Children);
                            var BfKey;
                            for (var i = 0; i < parentData.parent.Children.length; i++) {
                                if (parentData.parent.Children[i].uid == key) {
                                    BfKey = i
                                    break;
                                }
                            }
                            parentData.parent.Children.push(SelectData);

                            const DataSource = _Id.jqxTreeGrid("source").loadedData;
                            const MoveCount = Number(parentData.parent.Children.length) - Number(BfKey + 1);

                            var RuAdd = parentData.parent.Children.length - 1;

                            fEPartMoveRecusive(DataSource, parentData.parent.Children[RuAdd], 'RU', MoveCount);
                            _Id.jqxTreeGrid('updateBoundData');

                            _Id.jqxTreeGrid('deleteRow', key);
                        }
                        
                        //_Id.jqxTreeGrid("source").loadedData[0].Children.push(SelectData);
                        _Id.jqxTreeGrid('updateBoundData');
                        _Id.jqxTreeGrid('expandAll');
                    
                        $(popLayer).jqxWindow('modalDestory');
                    });

                    CreateButton.click(function (event) {
                        OpenInfoEPartCreateDialog(null, null, null, '/EBom/dlgCreateEPart', 'EPART 등록');
                    });
                },
            });

            digSearchEBomStructureGrid$.on('rowSelect', function (event) {
                const previousRowKey = rowKey;
                var args = event.args;

                if (args.row.Level != 0) {
                    digSearchEBomStructureGrid$.jqxTreeGrid('clearSelection');
                }
                rowKey = args.key;
            });

            $('#SearchEBomStructurebtn').on('click', function () {
                
                var SearchEPartCreateDt = $('#SearchEBomStructureCreateDt').val();
                var SearchEPartCreateDtArray = SearchEPartCreateDt.split('-');

                var EBomStructureParam = {};
                EBomStructureParam.Car_Lib_OID = $("#SearchEBomStructureCar").val();
                //EBomStructureParam. = dlgSearchEPartPms
                EBomStructureParam.Name = $('#SearchEBomStructureName').val();
                EBomStructureParam.ITEM_No = $('#SearchEBomStructureItemNo').val();
                EBomStructureParam.Division = EPartDivision;
                EBomStructureParam.StartCreateDt = SearchEPartCreateDtArray[0];
                EBomStructureParam.EndCreateDt = SearchEPartCreateDtArray[1]+" 23:59:59";
;
                RequestData('/EBom/SelectEBomAddChild', EBomStructureParam, function (res) {
                    PrintJqxTreeGrid(EBomStructureSource, digSearchEBomStructureGrid$, res);
                    digSearchEBomStructureGrid$.jqxTreeGrid('expandAll');
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

function EPartNewTabClick(url, Oid, Name) {
    const linkUrl = url + "?OID=" + Oid;
    if ($("div[tabUrl='" + linkUrl + "']").length > 0) {
        var tabDiv = $("div[tabUrl='" + linkUrl + "']")[0].parentNode;
        $('#tabContent').jqxTabs('select', $(tabDiv).index());
        return;
    }

    const loading$ = $('#loading');
    const tabContent$ = $('#tabContent');
    loading$.css('display', 'block');
    var linkName = Name;
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
        }
        , complete: function () {
            loading$.css('display', 'none');
        }
    });
}

function OpenSearchEBomOIDDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title, _Action) {
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
    var posX = (winWidth / 2) - (1400 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (750 / 2) + $(window).scrollTop();

    var rowKey = null;

    $(popLayer).jqxWindow({
        width: 1400, maxWidth: 1400, height: 750, minHeight: 750, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {

            if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                _CallBackFunction();
            }

            var dlgEPartSource =
            {
                dataType: "json",
                dataFields: [
                    { name: 'OID' },
                    { name: 'Name' },
                    { name: 'BPolicyOID' },
                    { name: 'BPolicy' },
                    { name: 'CreateUsNm' },
                    { name: 'CreateDt', type: 'date' },
                    { name: 'Car_Lib_OID' },
                    { name: 'Car_Lib_Nm' },
                    { name: 'ITEM_No' },
                    { name: 'ITEM_NoNm' },
                    { name: 'Block_No' },
                    { name: 'Block_NoNm' },
                    { name: 'Material_OID' },
                    { name: 'Material_Nm' },
                    { name: 'EPartType' },
                    { name: 'Sel_Eo' },
                    { name: 'Sel_Eo_Dt', type: 'date' },
                    { name: 'Thumbnail' },
                    { name: 'Revision' },
                ],
                id: 'OID',
                addRow: function (rowID, rowData, position, parentID, commit) {
                    newRowID = rowID;
                    commit(true);
                },
                updateRow: function (rowID, rowData, commit) {
                    commit(true);
                }
            };
            var dlgEPartdataAdapter = new $.jqx.dataAdapter(dlgEPartSource);
            const digSearchEPartGrid$ = $('#digSearchEPartGrid');
            digSearchEPartGrid$.jqxGrid('render');
            digSearchEPartGrid$.jqxGrid({
                width: "100%",
                theme: "kdnc",
                height: 510,
                sortable: true,
                showToolbar: true,
                toolbarHeight: 44,
                editable: false,
                source: dlgEPartdataAdapter,
                ready: function () {

                },
                columns: [
                    {
                        text: 'NO', width: "6%", cellsalign: 'center', columntype: 'number', align: 'center',
                        cellsrenderer: function (row, column, value) {
                            return "<div style='width:100%;height:100%;text-align:center;vertical-align:middle;line-height:1.9;'>" + (value + 1) + "</div>";
                        }
                    },

                    { name: 'OID', dataField: 'OID', type: 'text', align: 'center', cellsalign: 'center', text: 'OID', width: '9%', hidden: 'true' },

                    { name: 'Car_Lib_Nm', dataField: 'Car_Lib_Nm', type: 'text', align: 'center', cellsalign: 'center', text: '차종', width: '14%' },
                    { name: 'Name', id: 'ObjName', dataField: 'Name', type: 'text', align: 'center', cellsalign: 'center', text: '품번', width: '16%' },
                    { name: 'ITEM_NoNm', dataField: 'ITEM_NoNm', type: 'text', align: 'center', cellsalign: 'center',text: 'ITEM_NO', width: '14%' },
                    { name: 'Material_Nm', dataField: 'Material_Nm', type: 'text', align: 'center', cellsalign: 'center', text: '재질', width: '12%' },
                    { name: 'Block_NoNm', dataField: 'Block_NoNm', type: 'text', align: 'center', cellsalign: 'center', text: 'Block_No', width: '14%' },
                    { name: 'CreateDt', dataField: 'CreateDt', type: 'text', align: 'center', cellsalign: 'center', text: '작성일', cellsFormat: 'yyyy-MM-dd',  width: '12%' },
                    {
                        name: 'Thumbnail', dataField: 'Thumbnail', align: 'center', type: 'text', text: '이미지', width: '12%',
                        cellsrenderer: function (row, column, value) {
                            if (value.length > 1) {
                                return "<div class='ebomImg'><img src='~/images/Thumbnail/" + value + "'></div>";
                            } else {
                                return ""
                            }
                        }
                    }

                ],
                rendertoolbar: function (toolBar) {
                    var container = $("<div class='lGridComponent' ></div>");
                    var AddButton = $("<button class='custom-button'><i class='fas fa-plus'></i> 추가</button>").jqxButton();
                    container.append(AddButton);
                    toolBar.append(container);
                    
                    AddButton.click(function (event) {
                        const SelectData = digSearchEPartGrid$.jqxGrid('getrowdata', rowKey);
                        if (SelectData == null || SelectData == undefined) {
                            alert('품목을 선택하여 주세요');
                            return;
                        }
                        if (_Action == "L") {                         
                            $('#LOID').val(SelectData.OID);
                            $('#LName').val(SelectData.Name);
                            $(popLayer).jqxWindow('modalDestory');
                        } else if (_Action == "R") {
                            $('#ROID').val(SelectData.OID);
                            $('#RName').val(SelectData.Name);
                            $(popLayer).jqxWindow('modalDestory');
                        }
                    });
                },
            });

            digSearchEPartGrid$.on('rowselect', function (event) {
                const previousRowKey = rowKey;
                var args = event.args;
                rowKey = args.rowindex;
            });

            $('#dlgSearchEPartbtn').on('click', function () {
                var SearchEPartCreateDt = $('#dlgSearchEPartCreateDt').val();
                var SearchEPartCreateDtArray = SearchEPartCreateDt.split('-');
                var EBomStructureParam = {};
                EBomStructureParam.Car_Lib_OID = $("#dlgSearchEPartCar").val();
                //EBomStructureParam. = dlgSearchEPartPms
                EBomStructureParam.Name = $('#dlgSearchEPartName').val();
                EBomStructureParam.ITEM_No = $('#dlgSearchEPartItemNo').val();
                EBomStructureParam.Division = EPartDivision;
                EBomStructureParam.StartCreateDt = SearchEPartCreateDtArray[0];
                EBomStructureParam.EndCreateDt = SearchEPartCreateDtArray[1] + " 23:59:59";

                //dlgSearchEPartTypeAssy
                //dlgSearchEPartTypeSAssy
                //dlgSearchEPartTypeDetail

                RequestData('/Ebom/SelEPart', EBomStructureParam, function (res) {
                    PrintJqxGrid(dlgEPartSource, digSearchEPartGrid$, res);
                    //digSearchEPartGrid$.jqxGrid('expandAll');
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

function OpenInfoEPartCreateDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
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
    var posX = (winWidth / 2) - (1450 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (900 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 1450, maxWidth: 1450, height: 900, minHeight: 900, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {

            $('#dlgCreateEPartBtn').on('click', function () {
                var param = {};
                param.Name = $('#dlgCreateEPartName').val();
                param.Description = $('#dlgCreateEPart_Description').val();
                param.Thumbnail = EPartThumbnailData;
                if (EPartDivision == "ASSEMBLY") {            //조립도일때
                    param.Division = EPartDivision;
                    param.ITEM_No = $('#dlgCreateEPart_ITEM_NO1').val();
                    param.ITEM_Middle = itemMiddle$.val();
                    param.EPartType = epartType$.val();
                    param.Production_Place = placeList$.val();
                    param.Block_No = $('#dlgCreateEPart_BlockNo1').val();
                    param.Oem_Lib_OID = $('#dlgCreateEPart_Oem1').val();
                    param.Car_Lib_OID = $('#dlgCreateEPart_Car1').val();
                    param.Serial = serial;
                    param.Sel_Revision = srev;


                } else if (EPartDivision == "SINGLE") {       //단품도일떄
                    param.Division = EPartDivision;
                    param.Material_OID = psize$.val();
                    param.ITEM_No = $('#dlgCreateEPart_ITEM_NO2').val();
                    param.Block_No = $('#dlgCreateEPart_BlockNo2').val();
                    param.Serial = serial;
                    param.Oem_Lib_OID = $('#dlgCreateEPart_Oem2').val();
                    param.Car_Lib_OID = $('#dlgCreateEPart_Car2').val();
                    param.Sel_Revision = srev;

                } else if (EPartDivision == "STANDARD") {     //스탠다드일떄
                    param.Division = EPartDivision;
                    param.Material_OID = psize$.val();
                    param.Block_No = $('#dlgCreateEPart_BlockNo3').val();
                    param.Serial = serial;

                }

                if (param.Name == null || param.Name == "") {
                    alert('품번을 확인해주세요.');
                    return;
                } else if (param.Name.length != 12) {
                    alert('품번을 확인해주세요.');
                    return;
                } else if (param.Division == null || param.Division == "") {
                    alert('구분을 선택해주세요.');
                    return;
                }

                RequestData('/EBom/InsEPart', param, function (response) {
                    if (response.isError) {
                        alert(response.resultMessage);
                        return;
                    }
                    _Param = response;
                    alert("저장되었습니다.");
                    $(popLayer).jqxWindow('modalDestory');
                });
            });


            $('#dlgCreateEPartCancelBtn').on('click', function () {
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



function OpenSearchEBomTreeADialog(_CallBackFunction, _Wrap, _Param, _Url, _Title, _Name, _Id, key, _Action, _SelectRow) {
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
    var posX = (winWidth / 2) - (1400 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (750 / 2) + $(window).scrollTop();

    var EBomStructureParam = {};
    EBomStructureParam.Name = _Name;

    var rowKey = null;

    $(popLayer).jqxWindow({
        width: 1400, maxWidth: 1400, height: 750, minHeight: 750, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {
            if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                _CallBackFunction();
            }

            var EBomStructureSource =
            {
                dataType: "json",
                dataFields: [
                    { name: 'OID', type: 'number' },
                    { name: 'FromOID', type: 'number' },
                    { name: 'ToOID', type: 'number' },
                    { name: 'Children', type: 'array' },
                    { name: 'Ord', type: 'number' },
                    { name: 'Count', type: 'number' },

                    { name: 'ObjRevision', type: 'text' },
                    { name: 'ObjTdmxOID', type: 'text' },
                    { name: 'ObjIsLatest', type: 'number' },
                    { name: 'ObjEPartType', type: 'text' },
                    { name: 'Level', type: 'number' },
                    { name: 'ObjName', type: 'text' },
                    { name: 'ObjThumbnail', type: 'text' },
                    { name: 'ObjCar_Lib_OID', type: 'number' },
                    { name: 'ObjCar_Lib_Nm', type: 'text' },
                    { name: 'ObjTitle', type: 'text' },
                    { name: 'ObjITEM_No', type: 'text' },
                    { name: 'ObjITEM_NoNm', type: 'text' },
                    { name: 'ObjMaterial_OID', type: 'text' },
                    { name: 'ObjMaterial_Nm', type: 'text' },
                    { name: 'ObjITEM_Middle', type: 'text' },
                    { name: 'ObjITEM_MiddleNm', type: 'text' },
                    { name: 'ObjProduction_Place', type: 'text' },
                    { name: 'ObjProduction_PlaceNm', type: 'text' },
                    { name: 'ObjBlock_No', type: 'text' },
                    { name: 'ObjBlock_NoNm', type: 'text' },
                    { name: 'ObjSerial', type: 'text' },
                    { name: 'ObjSel_Revision', type: 'text' },

                ],
                hierarchy:
                {
                    root: 'Children',
                },
                id: 'OID',
                addRow: function (rowID, rowData, position, parentID, commit) {
                    newRowID = rowID;
                    commit(true);
                },
                updateRow: function (rowID, rowData, commit) {
                    commit(true);
                }
            };
            var dataAdapter = new $.jqx.dataAdapter(EBomStructureSource);
            const digSetSearchEBomStructureGrid$ = $('#digSetSearchEBomStructureGrid');
            digSetSearchEBomStructureGrid$.jqxTreeGrid({
                width: "100%",
                theme: "kdnc",
                sortable: true,
                showToolbar: true,
                toolbarHeight: 44,
                editable: false,
                source: dataAdapter,
                ready: function () {

                },
                columns: [
                    { name: 'Level', dataField: 'Level', type: 'number', width: '9%' },

                    { name: 'OID', dataField: 'OID', type: 'text', align: 'center', cellsalign: 'center', text: 'OID', width: '9%', hidden: 'true' },
                    { name: 'FromOID', dataField: 'FromOID', type: 'text', align: 'center', cellsalign: 'center', text: 'FromOID', width: '9%', hidden: 'true' },
                    { name: 'ToOID', dataField: 'ToOID', type: 'text', align: 'center', cellsalign: 'center', text: 'ToOID', width: '9%', hidden: 'true' },

                    { name: 'ObjCar_Lib_Nm', dataField: 'ObjCar_Lib_Nm', align: 'center', cellsalign: 'center', type: 'text', text: '차종', width: '14%' },
                    { name: 'ObjName', id: 'ObjName', dataField: 'ObjName', align: 'center', cellsalign: 'center', type: 'text', text: '품번', width: '14%' },
                    { name: 'ObjITEM_NoNm', dataField: 'ObjITEM_NoNm', align: 'center', cellsalign: 'center', type: 'text', text: 'ITEM_NO', width: '14%' },
                    { name: 'ObjMaterial_Nm', dataField: 'ObjMaterial_Nm', align: 'center', cellsalign: 'center', type: 'text', text: '재질', width: '11%' },
                    { name: 'ObjBlock_NoNm', dataField: 'ObjBlock_NoNm', align: 'center', cellsalign: 'center', type: 'text', text: 'Block_No', width: '14%' },
                    { name: 'Ord', dataField: 'Ord', type: 'number', align: 'center', cellsalign: 'center', text: '순서', width: '6%' },
                    { name: 'Count', dataField: 'Count', align: 'center', cellsalign: 'center', type: 'number', text: '수량', width: '6%' },
                    {
                        name: 'ObjThumbnail', dataField: 'ObjThumbnail', align: 'center', type: 'text', text: '이미지', width: '12%',
                        cellsrenderer: function (row, column, value) {
                            if (value.length > 1) {
                                return "<div class='ebomImg'><img src='/images/Thumbnail/" + value + "'></div>";
                            } else {
                                return ""
                            }
                        }
                    }

                ],

                rendertoolbar: function (toolBar) {
                    toolBar.empty();
                    var container = $("<div class='lGridComponent' ></div>");
                    var AddButton = $("<button class='custom-button'><i class='fas fa-stream'></i> 추가</button>").jqxButton();
                    var CreateButton = $("<button class='custom-button'><i class='fas fa-plus'></i> 생성</button>").jqxButton();
                    container.append(AddButton);
                    container.append(CreateButton);
                    toolBar.append(container);

                    digSetSearchEBomStructureGrid$.on('rowSelect', function (event) {
                        const previousRowKey = rowKey;
                        var args = event.args;
                        if (previousRowKey != null && rowKey == previousRowKey) {
                            digSetSearchEBomStructureGrid$.jqxTreeGrid('uncheckRow', previousRowKey);
                        }
                        rowKey = args.key;
                    });

                    AddButton.click(function (event) {
                        digSetSearchEBomStructureGrid$.jqxTreeGrid('expandRow', rowKey);
                        const SelectData = digSetSearchEBomStructureGrid$.jqxTreeGrid('getRow', rowKey);
                        var parentData = _Id.jqxTreeGrid('getRow', key);
                        SelectData.Action = "A";
                        SelectData.FromOID = parentData.FromOID;
                        SelectData.Ord = 1;
                        SelectData.Count = 1;
                        ffEPartArray([SelectData], Number(parentData.Level));
                        if (parentData.Action == "A") {
                            parentData.Action = "AD";
                        }

                        _Id.jqxTreeGrid('deleteRow', key);

                        parentData.parent.Children.push(SelectData);
                        _Id.jqxTreeGrid('updateBoundData');

                        _Id.jqxTreeGrid('expandAll');

                        $(popLayer).jqxWindow('modalDestory');
                    });

                    var A;
                    CreateButton.click(function (event) {
                        OpenInfoEPartCreateDialog(null, null, A, '/EBom/dlgCreateEPart', 'EPART 등록');
                    });

                    //검색 
                },
            });

            digSetSearchEBomStructureGrid$.on('rowSelect', function (event) {
                const previousRowKey = rowKey;
                var args = event.args;

                if (args.row.Level != 0) {
                    digSetSearchEBomStructureGrid$.jqxTreeGrid('clearSelection');
                }
                rowKey = args.key;
            });

            var SetSearchData = _Id.jqxTreeGrid('getRow', key);

            RequestData('/EBom/SelectEBomAddChild', { Name: SetSearchData.ObjName }, function (res) {
                PrintJqxTreeGrid(EBomStructureSource, digSetSearchEBomStructureGrid$, res);
                digSetSearchEBomStructureGrid$.jqxTreeGrid('expandAll');
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