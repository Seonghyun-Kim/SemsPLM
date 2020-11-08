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

                    { name: 'Level', type: 'number' },
                    { name: 'ObjName', type: 'text' },
                    { name: 'ObjTitle', type: 'text' },
                    { name: 'ObjRep_Part_No', type: 'text' },
                    { name: 'ObjRep_Part_No2', type: 'text' },
                    { name: 'ObjEo_No', type: 'text' },
                    { name: 'ObjEPartType', type: 'text' },
                    { name: 'ObjThumbnail', type: 'text' },
                    { name: 'ObjOem_Lib_OID', type: 'number' },
                    { name: 'ObjCar_Lib_OID', type: 'number' },
                    { name: 'ObjPms_OID', type: 'number' },
                    { name: 'ObjOem_Lib_NM', type: 'text' },
                    { name: 'ObjCar_Lib_NM', type: 'text' },
                    { name: 'ObjPms_NM', type: 'text' },


                    { name: 'Name', type: 'text' },
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
                    { name: 'Level', dataField: 'Level', type: 'number', width: '11%' },

                    { name: 'OID', dataField: 'OID', type: 'text', text: 'OID', width: '9%' },
                    { name: 'FromOID', dataField: 'FromOID', type: 'text', text: 'FromOID', width: '9%' },
                    { name: 'ToOID', dataField: 'ToOID', type: 'text', text: 'ToOID', width: '9%' },

                    { name: 'ObjCar_Lib_NM', dataField: 'ObjCar_Lib_NM', type: 'text', text: '차종', width: '9%' },
                    { name: 'ObjName', id: 'ObjName', dataField: 'ObjName', type: 'text', text: '품번', width: '11%' },
                    { name: 'ObjRep_Part_No', dataField: 'ObjRep_Part_No', type: 'text', text: 'S/ON 품번', width: '11%' },
                    { name: 'ObjRep_Part_No2', dataField: 'ObjRep_Part_No2', type: 'text', text: '대체 품번', width: '11%' },
                    { name: 'ObjTitle', dataField: 'ObjTitle', type: 'text', text: '품명', width: '11%' },
                    { name: 'Ord', dataField: 'Ord', type: 'number', text: '순서', width: '5%' },
                    { name: 'Count', dataField: 'Count', type: 'number', text: '수량', width: '5%' },
                    { name: 'ObjThumbnail', dataField: 'ObjThumbnail', type: 'text', text: '이미지', width: '9%' },
                    { name: 'ObjEo_No', dataField: 'ObjEo_No', type: 'text', text: 'EONO', width: '9%' },
                    { name: 'ObjEPartType', dataField: 'ObjEPartType', type: 'text', text: '타입', width: '5%' },

                ],
                rendertoolbar: function (toolBar) {
                    var container = $("<div class='lGridComponent' ></div>");
                    var AddButton = $("<button class='custom-button'><i class='fas fa-plus'></i> 추가</button>").jqxButton();
                    container.append(AddButton);
                    toolBar.append(container);
                    
                    console.log(_Id);
                    console.log(key);
                    console.log(_SelectRow);

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

            $('#dlgSearchEPartbtn').on('click', function () {
                var EBomStructureParam = {};
                EBomStructureParam.Oem_Lib_OID = $("#dlgSearchEPartOEM").jqxComboBox('val');
                EBomStructureParam.Car_Lib_OID = $("#dlgSearchEPartCar").jqxComboBox('val');
                //EBomStructureParam. = dlgSearchEPartPms
                EBomStructureParam.Name = $('#dlgSearchEPartName').val();
                EBomStructureParam.Title = $('#dlgSearchEPartTitle').val();
                EBomStructureParam.Eo_No = $('#dlgSearchEPartEoNo').val();
                EBomStructureParam.Sel_Eo = $('#dlgSearchEPartSelEo').val();
                //EBomStructureParam.EPartType = dlgSearchEPartType
                EBomStructureParam.CreateDt = $('#dlgSearchEPartCreateDt').val();

                //dlgSearchEPartTypeAssy
                //dlgSearchEPartTypeSAssy
                //dlgSearchEPartTypeDetail


                //console.log("검색");
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
            console.log(res);
        }
        , complete: function () {
            loading$.css('display', 'none');
        }
    });
}

function OpenSearchEBomOIDDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
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

    $(popLayer).jqxWindow({
        width: 1400, maxWidth: 1400, height: 750, minHeight: 750, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
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