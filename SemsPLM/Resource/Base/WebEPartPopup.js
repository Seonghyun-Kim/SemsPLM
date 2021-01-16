//대체시
function OpenSearchEBomTreeDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title, _Name, _Id, key, _Action, _SelectRow) {
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
    var posX = (winWidth / 2) - (1500 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (800 / 2) + $(window).scrollTop();

    //var EBomStructureParam = {};
    //EBomStructureParam.Name = _Name;

    var rowKey = null;

    $(popLayer).jqxWindow({
        width: 1500, maxWidth: 1500, height: 800, minHeight: 800, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
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
                    { name: 'BPolicy' },

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

                    { name: 'ObjOem_Lib_Nm', type: 'text' },
                    { name: 'ObjDivision', type: 'text' },

                    { name: 'ObjDivisionNm', type: 'text' },

                    { name: 'ObjSerial', type: 'text' },
                    { name: 'ObjSel_Revision', type: 'text' },
                    { name: 'ObjStandard', type: 'text' },
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
                height: 570,
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

                    { name: 'ObjOem_Lib_Nm', dataField: 'ObjOem_Lib_Nm', type: 'text', text: 'OEM', width: '5%',  cellsalign: 'center', align: 'center' },
                    { name: 'ObjCar_Lib_Nm', dataField: 'ObjCar_Lib_Nm', type: 'text', text: '차종', width: '5%',  cellsalign: 'center', align: 'center' },
                    { name: 'ObjDivisionNm', dataField: 'ObjDivisionNm', type: 'text', text: '구분', width: '5%', cellsalign: 'center', align: 'center' },
                    { name: 'ObjName', dataField: 'ObjName', align: 'center', cellsalign: 'center', type: 'text', text: '품번', width: '11%' },
                    { name: 'ObjTitle', dataField: 'ObjTitle', type: 'text', text: '품명', width: '11%', cellsalign: 'center', align: 'center' },
                    { name: 'ObjITEM_NoNm', dataField: 'ObjITEM_NoNm', type: 'text', text: 'ITEM_NO', width: '8%',  cellsalign: 'center', align: 'center' },
                    { name: 'ObjMaterial_Nm', dataField: 'ObjMaterial_Nm', type: 'text', text: '재질', width: '8%', cellsalign: 'center', align: 'center' },

                    { name: 'ObjStandard', dataField: 'ObjStandard', type: 'text', text: '규격', width: '7%', cellsalign: 'center', align: 'center' },
                    { name: 'ObjSel_Revision', dataField: 'ObjSel_Revision', type: 'text', text: '고객리비전', width: '7%', cellsalign: 'center', align: 'center' },
                    { name: 'ObjRevision', dataField: 'ObjRevision', type: 'text', text: '리비전', width: '4%', cellsalign: 'center', align: 'center' },
                    { name: 'Ord', dataField: 'Ord', type: 'number', align: 'center', cellsalign: 'center', text: '순서', width: '5%' },
                    { name: 'Count', dataField: 'Count', align: 'center', cellsalign: 'center', type: 'number', text: '수량', width: '5%' },
                    {
                        name: 'ObjThumbnail', dataField: 'ObjThumbnail', align: 'center', type: 'text', text: '이미지', width: '10%',
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
                EBomStructureParam.Title = $("#SearchEBomStructureTitle").val();
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
        loading$.css('display', 'none');
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

//비교시 검색
function OpenSearchEBomOIDDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title, _Action) {
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
    var posX = (winWidth / 2) - (1500 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (800 / 2) + $(window).scrollTop();

    var rowKey = null;

    $(popLayer).jqxWindow({
        width: 1500, maxWidth: 1500, height: 800, minHeight: 800, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
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

                    { name: 'Title' },

                    { name: 'Oem_Lib_OID' },
                    { name: 'Oem_Lib_Nm' },
                    { name: 'Sel_Revision' },
                    { name: 'Division' },
                    { name: 'DivisionNm' },
                    { name: 'Standard' },

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
                        text: 'NO', width: "3%", cellsalign: 'center', columntype: 'number', align: 'center',
                        cellsrenderer: function (row, column, value) {
                            return "<div style='width:100%;height:100%;text-align:center;vertical-align:middle;line-height:1.9;'>" + (value + 1) + "</div>";
                        }
                    },
                    { text: 'OEM', datafield: 'Oem_Lib_Nm', width: "5%", align: 'center', cellsalign: 'center', },
                    { text: '차종', datafield: 'Car_Lib_Nm', width: "5%", align: 'center', cellsalign: 'center', },
                    { text: '구분', datafield: 'DivisionNm', width: "7%", align: 'center', cellsalign: 'center', },
                    { text: '품번', datafield: 'Name', width: "12%", align: 'center', cellsalign: 'center', },
                    { text: '품명', datafield: 'Title', width: "12%", align: 'center', cellsalign: 'center', },
                    { text: 'ITEM_NO', datafield: 'ITEM_NoNm', width: "11%", align: 'center', cellsalign: 'center', },
                    { text: '재질', datafield: 'Material_Nm', width: "8%", align: 'center', cellsalign: 'center', },
                    { text: '규격', datafield: 'Standard', width: "6%", align: 'center', cellsalign: 'center', },
                    { text: '고객리비전', datafield: 'Sel_Revision', width: "6%", align: 'center', cellsalign: 'center', },
                    { text: '리비전', datafield: 'Revision', width: "5%", align: 'center', cellsalign: 'center', },
                    { text: '작성일', datafield: 'CreateDt', width: "8%", align: 'center', cellsalign: 'center', cellsFormat: 'yyyy-MM-dd', },
                    { text: '작성자', datafield: 'CreateUsNm', width: "6%", align: 'center', cellsalign: 'center', },
                    {
                        text: '상태', datafield: 'BPolicy', width: "6%", align: 'center', cellsalign: 'center',
                        cellsrenderer: function (row, column, value) {
                            return "<div style='width:100%;height:100%;text-align:center;vertical-align:middle;line-height:1.9;'>" + value.StatusNm + "</div>";
                        }
                    },
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
        loading$.css('display', 'none');
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
    var posX = (winWidth / 2) - (1450 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (900 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 1450, maxWidth: 1450, height: 900, minHeight: 900, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {

            $('#dlgCreateEPartBtn').on('click', function () {
                var param = {};
                param.Name = $('#dlgCreateEPartName').val();
                param.Title = $('#dlgCreateEPartTitle').val();
                param.Standard = $('#dlgCreateEPart_Standard').val();
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

                if (param.Title == null || param.Title == "") {
                    alert('품명을 확인해주세요.');
                    return;
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
                    //_Param = param.Name;
                    if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                        _CallBackFunction(param.Name);
                    }
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
        loading$.css('display', 'none');
        $(popLayer).jqxWindow('setTitle', _Title);
        $(popLayer).jqxWindow("show");
    });

    $(popLayer).on('close', function (event) {
        if (_Wrap === undefined || _Wrap === null) {
            $(popLayer).jqxWindow('modalDestory');
        }
    });
   
}


// EBOM 추가시
function OpenSearchEBomTreeADialog(_CallBackFunction, _Wrap, _Param, _Url, _Title, _Name, _Id, key, _Action, _SelectRow) {
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
    var posX = (winWidth / 2) - (1500 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (800 / 2) + $(window).scrollTop();

    var EBomStructureParam = {};
    EBomStructureParam.Name = _Name;

    var rowKey = null;

    $(popLayer).jqxWindow({
        width: 1500, maxWidth: 1500, height: 800, minHeight: 800, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {

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
                    { name: 'BPolicy'},

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

                    { name: 'ObjOem_Lib_Nm', type: 'text' },
                    { name: 'ObjDivision', type: 'text' },

                    { name: 'ObjDivisionNm', type: 'text' },
                    
                    { name: 'ObjSerial', type: 'text' },
                    { name: 'ObjSel_Revision', type: 'text' },
                    { name: 'ObjStandard', type: 'text' },
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
            const dlgSetSearchEBomStructureGrid$ = $('#dlgSetSearchEBomStructureGrid');
            dlgSetSearchEBomStructureGrid$.jqxTreeGrid({
                width: "100%",
                theme: "kdnc",
                sortable: true,
                showToolbar: true,
                height: 730,
                toolbarHeight: 44,
                editable: false,
                source: dataAdapter,
                ready: function () {

                },
                columns: [
                    { name: 'Level', dataField: 'Level', type: 'number', width: '10%', editable: false },
                    { name: 'OID', dataField: 'OID', type: 'number', width: '10%', editable: false, hidden: true },
                    { name: 'FromOID', dataField: 'FromOID', type: 'number', width: '10%', editable: false, hidden: true },
                    { name: 'ToOID', dataField: 'ToOID', type: 'number', width: '10%', editable: false, hidden: true },

                    { name: 'ObjOem_Lib_Nm', dataField: 'ObjOem_Lib_Nm', type: 'text', text: 'OEM', width: '5%', editable: false, cellsalign: 'center', align: 'center' },
                    { name: 'ObjCar_Lib_Nm', dataField: 'ObjCar_Lib_Nm', type: 'text', text: '차종', width: '5%', editable: false, cellsalign: 'center', align: 'center' },
                    { name: 'ObjDivisionNm', dataField: 'ObjDivisionNm', type: 'text', text: '구분', width: '5%', editable: false, cellsalign: 'center', align: 'center' },
                    { name: 'ObjName', id: 'ObjName', dataField: 'ObjName', align: 'center', cellsalign: 'center', type: 'text', text: '품번', width: '13%' },
                    { name: 'ObjTitle', dataField: 'ObjTitle', type: 'text', text: '품명', width: '13%', editable: false, cellsalign: 'center', align: 'center' },
                    { name: 'ObjITEM_NoNm', dataField: 'ObjITEM_NoNm', type: 'text', text: 'ITEM_NO', width: '10%', editable: false, cellsalign: 'center', align: 'center' },
                    { name: 'ObjMaterial_Nm', dataField: 'ObjMaterial_Nm', type: 'text', text: '재질', width: '10%', editable: false, cellsalign: 'center', align: 'center' },

                    { name: 'ObjStandard', dataField: 'ObjStandard', type: 'text', text: '규격', width: '7%', editable: false, cellsalign: 'center', align: 'center' },
                    { name: 'ObjSel_Revision', dataField: 'ObjSel_Revision', type: 'text', text: '고객리비전', width: '6%', editable: false, cellsalign: 'center', align: 'center' },
                    { name: 'ObjRevision', dataField: 'ObjRevision', type: 'text', text: '리비전', width: '4%', editable: false, cellsalign: 'center', align: 'center' },

                    { name: 'Ord', dataField: 'Ord', type: 'number', align: 'center', cellsalign: 'center', text: '순서', width: '6%' },
                    { name: 'Count', dataField: 'Count', align: 'center', cellsalign: 'center', type: 'number', text: '수량', width: '6%' },
                    
                    //{
                    //    name: 'ObjThumbnail', dataField: 'ObjThumbnail', align: 'center', type: 'text', text: '이미지', width: '12%',
                    //    cellsrenderer: function (row, column, value) {
                    //        if (value.length > 1) {
                    //            return "<div class='ebomImg'><img src='/images/Thumbnail/" + value + "'></div>";
                    //        } else {
                    //            return ""
                    //        }
                    //    }
                    //}

                ],

                rendertoolbar: function (toolBar) {
                    toolBar.empty();
                    var container = $("<div class='lGridComponent' ></div>");
                    var AddButton = $("<button class='custom-button'><i class='fas fa-stream'></i> 추가</button>").jqxButton();
                    var CreateButton = $("<button class='custom-button'><i class='fas fa-plus'></i> 생성</button>").jqxButton();
                    container.append(AddButton);
                    container.append(CreateButton);
                    toolBar.append(container);

                    dlgSetSearchEBomStructureGrid$.on('rowSelect', function (event) {
                        const previousRowKey = rowKey;
                        var args = event.args;
                        if (previousRowKey != null && rowKey == previousRowKey) {
                            dlgSetSearchEBomStructureGrid$.jqxTreeGrid('uncheckRow', previousRowKey);
                        }
                        rowKey = args.key;
                    });

                    AddButton.click(function (event) {
                        dlgSetSearchEBomStructureGrid$.jqxTreeGrid('expandRow', rowKey);
                        const SelectData = dlgSetSearchEBomStructureGrid$.jqxTreeGrid('getRow', rowKey);
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

                    var AddName;
                    CreateButton.click(function (event) {
                        OpenInfoEPartCreateDialog(
                            function (res) {
                                RequestData('/EBom/SelectEBomAddChild', { Name: res }, function (res) {
                                    PrintJqxTreeGrid(EBomStructureSource, dlgSetSearchEBomStructureGrid$, res);
                                    dlgSetSearchEBomStructureGrid$.jqxTreeGrid('expandAll');
                                })
                            }, null, AddName, '/EBom/dlgCreateEPart', 'EPART 등록');
                    });

                    //검색 
                },
            });

            dlgSetSearchEBomStructureGrid$.on('rowSelect', function (event) {
                const previousRowKey = rowKey;
                var args = event.args;

                if (args.row.Level != 0) {
                    dlgSetSearchEBomStructureGrid$.jqxTreeGrid('clearSelection');
                }
                rowKey = args.key;
            });

            var SetSearchData = _Id.jqxTreeGrid('getRow', key);

            RequestData('/EBom/SelectEBomAddChild', { Name: SetSearchData.ObjName }, function (res) {
                PrintJqxTreeGrid(EBomStructureSource, dlgSetSearchEBomStructureGrid$, res);
                dlgSetSearchEBomStructureGrid$.jqxTreeGrid('expandAll');
            });
        }
    });

    $(popContent).load(_Url, _Param, function () {
        loading$.css('display', 'none');
        $(popLayer).jqxWindow('setTitle', _Title);
        $(popLayer).jqxWindow("show");
    });

    $(popLayer).on('close', function (event) {
        if (_Wrap === undefined || _Wrap === null) {
            $(popLayer).jqxWindow('modalDestory');
        }
    });
}


function OpenSearchEPartAssyDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
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

    var rowKey = null;

    var winHeight = $(window).height();
    var winWidth = $(window).width();
    var posX = (winWidth / 2) - (1500 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (800 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 1500, maxWidth: 1500, height: 800, minHeight: 800, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {

            var dlgEPartAssySource =
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
            var dlgEPartdataAdapter = new $.jqx.dataAdapter(dlgEPartAssySource);

            digSearchEPartAssyGrid$.jqxGrid('render');
            digSearchEPartAssyGrid$.jqxGrid({
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
                    { name: 'ITEM_NoNm', dataField: 'ITEM_NoNm', type: 'text', align: 'center', cellsalign: 'center', text: 'ITEM_NO', width: '14%' },
                    { name: 'Material_Nm', dataField: 'Material_Nm', type: 'text', align: 'center', cellsalign: 'center', text: '재질', width: '12%' },
                    { name: 'Block_NoNm', dataField: 'Block_NoNm', type: 'text', align: 'center', cellsalign: 'center', text: 'Block_No', width: '14%' },
                    { name: 'CreateDt', dataField: 'CreateDt', type: 'text', align: 'center', cellsalign: 'center', text: '작성일', cellsFormat: 'yyyy-MM-dd', width: '12%' },
                    {
                        name: 'Thumbnail', dataField: 'Thumbnail', align: 'center', type: 'text', text: '이미지', width: '12%',
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
                    var container = $("<div class='lGridComponent' ></div>");
                    var AddButton = $("<button class='custom-button'><i class='fas fa-plus'></i> 추가</button>").jqxButton();
                    container.append(AddButton);
                    toolBar.append(container);

                    AddButton.click(function (event) {
                        const SelectData = digSearchEPartAssyGrid$.jqxGrid('getrowdata', rowKey);
                        if (SelectData == null || SelectData == undefined) {
                            alert('품목을 선택하여 주세요');
                            return;
                        }

                        var param = {};
                        param.RootOID = _Param.RootOID;
                        param.FromOID = _Param.RootOID;
                        param.ToOID = SelectData.OID;
                        RequestData('/Pms/InsProjectEPartRelation', param, function (res) {
                            _CallBackFunction();
                            $(popLayer).jqxWindow('modalDestory');
                        });
                    });
                },
            });

            digSearchEPartAssyGrid$.on('rowselect', function (event) {
                var args = event.args;
                rowKey = args.rowindex;
            });

            $('#dlgSearchEPartbtn').on('click', function () {
                //var SearchEPartCreateDt = $('#dlgSearchEPartCreateDt').val();
                //var SearchEPartCreateDtArray = SearchEPartCreateDt.split('-');
                var EBomStructureParam = {};
                EBomStructureParam.Car_Lib_OID = $("#dlgSearchEPartCar").val();
                EBomStructureParam.Name = $('#dlgSearchEPartName').val();
                EBomStructureParam.ITEM_No = $('#dlgSearchEPartItemNo').val();
                EBomStructureParam.Division = EPartAssyDivision;
                //EBomStructureParam.StartCreateDt = SearchEPartCreateDtArray[0];
                //EBomStructureParam.EndCreateDt = SearchEPartCreateDtArray[1] + " 23:59:59";
                EBomStructureParam.EPartType = EPartTypeAssy;

                RequestData('/Ebom/SelEPart', EBomStructureParam, function (res) {
                    PrintJqxGrid(dlgEPartAssySource, digSearchEPartAssyGrid$, res);
                });
            });
        }
    });

    $(popContent).load(_Url, _Param, function () {
        loading$.css('display', 'none');
        $(popLayer).jqxWindow('setTitle', _Title);
        $(popLayer).jqxWindow("show");
    });

    $(popLayer).on('close', function (event) {
        if (_Wrap === undefined || _Wrap === null) {
            $(popLayer).jqxWindow('modalDestory');
        }
    });
}


function OpenSearchTopEPartDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
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

    var rowKey = null;

    var winHeight = $(window).height();
    var winWidth = $(window).width();
    var posX = (winWidth / 2) - (1500 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (800 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 1500, maxWidth: 1500, height: 800, minHeight: 800, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {

        }
    });

    $(popContent).load(_Url, _Param, function () {
        loading$.css('display', 'none');
        $(popLayer).jqxWindow('setTitle', _Title);
        $(popLayer).jqxWindow("show");
    });

    $(popLayer).on('close', function (event) {
        if (_Wrap === undefined || _Wrap === null) {
            $(popLayer).jqxWindow('modalDestory');
        }
    });
}