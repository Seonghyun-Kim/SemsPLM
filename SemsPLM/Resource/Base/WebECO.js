function OpenSearchEPartDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title, _Type, _Mod) {
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
    var posX = (winWidth / 2) - (1400 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (800 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 1400, maxWidth: 1400, height: 800, minHeight: 800, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {
            $('#parentOID').val(_Param.RootOID);
            if (_Mod == "Create") {
                for (var i = 0; i < _Param.ECOListData.length; i++) {
                    partOIDList.push(_Param.ECOListData[i].OID);
                }
            }

            var EPartSource = {
                datatype: 'json',
                datafields: [
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
            }

            const SearchEPartGrid$ = $('#dlgSearchEPart_grid');
            SearchEPartGrid$.jqxGrid({
                theme: "kdnc",
                width: "100%",
                height: 610,
                rowsheight: 28,
                selectionmode: 'checkbox',
                columnsheight: 30,
                sortable: false,
                pageable: false,
                columns: [
                    { text: 'OEM', datafield: 'Oem_Lib_Nm', width: "5%", align: 'center', cellsalign: 'center', },
                    { text: '차종', datafield: 'Car_Lib_Nm', width: "5%", align: 'center', cellsalign: 'center', },
                    { text: '구분', datafield: 'DivisionNm', width: "7%", align: 'center', cellsalign: 'center', },
                    { text: '품번', datafield: 'Name', width: "12%", align: 'center', cellsalign: 'center', },
                    { text: '품명', datafield: 'Title', width: "12%", align: 'center', cellsalign: 'center', },
                    { text: 'ITEM_NO', datafield: 'ITEM_NoNm', width: "11%", align: 'center', cellsalign: 'center', },
                    { text: '재질', datafield: 'Material_Nm', width: "8%", align: 'center', cellsalign: 'center', },
                    { text: '규격', datafield: 'Standard', width: "6%", align: 'center', cellsalign: 'center', },
                    { text: '고객리비전', datafield: 'Sel_Revision', width: "7%", align: 'center', cellsalign: 'center', },
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
                showtoolbar: true,
                toolbarheight: 45,
                renderToolbar: function (statusbar) {
                    statusbar.empty();
                    var container$ = $('<div class="lGridComponent"></div>');
                    var AddBtn$ = $('<button class="custom-button"><i class="fas fa-plus-square"></i> 추가</button>').jqxButton();

                    container$.append(AddBtn$);
                    statusbar.append(container$);
                    //행 추가
                    AddBtn$.on('click', function (e) {
                        var rows = SearchEPartGrid$.jqxGrid('selectedrowindexes');
                        var selectedRecords = new Array();
                        for (var m = 0; m < rows.length; m++) {
                            var row = SearchEPartGrid$.jqxGrid('getrowdata', rows[m]);
                            var param = {};
                            param.RootOID = _Param.RootOID;
                            param.ToOID = row.OID;
                            param.Type = _Param.Type;
                            selectedRecords[selectedRecords.length] = param;
                            row.Type = _Param.Type;
                            row.ToOID = row.OID;
                            if (_Mod == "Create") {
                                _Param.ECOListData.push(row);
                            }
                        }
                        if (selectedRecords.length == 0) {
                            alert('품목을 선택해주세요');
                            return false;
                        }
                        if (_Mod == "Create") {
                            if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                                _CallBackFunction(_Param);
                            }
                            $(popLayer).jqxWindow('modalDestory');

                        } else if (_Mod == "Edit") {
                            RequestData('/ChangeOrder/InsECOContents', { _param: selectedRecords }, function (response) {
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
                        }
                    });
                }

            });
            SearchEPartGrid$.on('rowselect', function (event) {
                var rowObj = event.args;
                var disChk = [];

                if (partOIDList != null && partOIDList != undefined) {
                    partOIDList.filter(function (item) {
                        if (typeof rowObj.rowindex == 'number') {
                            if (item == rowObj.row.OID) {
                                disChk.push(rowObj.rowindex);
                            }
                        } else if (typeof rowObj.rowindex == 'object') {
                            SearchEPartGrid$.jqxGrid('getrows').filter(function (innerItem) {
                                if (item == innerItem.OID) {
                                    disChk.push(innerItem.uid);
                                }
                            });
                        }
                    });
                }
                if (disChk != null && disChk != undefined && disChk.length > 0) {
                    for (var index = 0; index < disChk.length; index++) {
                        SearchEPartGrid$.jqxGrid('unselectrow', disChk[index]);
                    }
                }
            });
            if (_Type == 'Epart') {

                $('#dlgSearchEPartbtn').on('click', function () {
                    var param = {};
                    param.Name = EPartSearch.val();
                    param.Division = SearchDivision;
                    param.Car_Lib_OID = dlgSearchEPartCarLib$.val();
                    param.ITEM_No = dlgSearchEPartItemLib$.val();
                    param.ITEM_Middle = dlgSearchEPartItemMiddle$.val();


                    param.IsLatest = 1;
                    param.IsReleasedLatest = 0;
                    RequestData('/EBom/SelEPart', param, function (res) {
                        PrintJqxGrid(EPartSource, SearchEPartGrid$, res);
                    });
                });
            } else if (_Type == 'EpartChild') {

                $('#dlgSearchEPart_searchForm').attr('hidden', true);
                var param = {};
                param.RootOID = _Param.RootOID;
                param.FromOID = _Param.FromOID;
                param.IsLatest = 1;
                param.IsReleasedLatest = 0;
                RequestData('/EBom/SelRootChildList', param, function (res) {
                    PrintJqxGrid(EPartSource, SearchEPartGrid$, res);
                });
            }
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

function OpenSearchECRDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title, _Grid, _Source, _Mod) {
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
    var posX = (winWidth / 2) - (1400 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (750 / 2) + $(window).scrollTop();

    var rowKey = null;

    $(popLayer).jqxWindow({
        width: 1400, maxWidth: 1400, height: 750, minHeight: 750, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {
            var ECRRelationList = [];
            for (var i = 0; i < parent.ECRRelationData.length; i++) {
                ECRRelationList.push(parent.ECRRelationData[i]);
            }
           
            var dlgSearchECRSource =
            {
                dataType: "json",
                dataFields: [
                    { name: 'OID' },
                    { name: 'Name' },
                    { name: 'Title' },
                    { name: 'BPolicyOID' },
                    { name: 'BPolicy' },
                    { name: 'CreateUsNm' },
                    { name: 'CreateDt', type: 'date' },
                    { name: 'DesignChangeDt', type: 'date' },
                    { name: 'CreateUs' },
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
            var dlgSearchECRdataAdapter = new $.jqx.dataAdapter(dlgSearchECRSource);
            const dlgSearchECRgrid$ = $('#dlgSearchECRgrid');
            dlgSearchECRgrid$.jqxGrid('render');
            dlgSearchECRgrid$.jqxGrid({
                width: "100%",
                theme: "kdnc",
                height: 510,
                sortable: true,
                showToolbar: true,
                toolbarHeight: 44,
                editable: false,
                selectionmode: 'checkbox',
                source: dlgSearchECRdataAdapter,
                ready: function () {

                },
                columns: [
                    {
                        text: 'NO', width: "5.5%", cellsalign: 'center', columntype: 'number', align: 'center',
                        cellsrenderer: function (row, column, value) {
                            return "<div style='width:100%;height:100%;text-align:center;vertical-align:middle;line-height:1.9;'>" + (value + 1) + "</div>";
                        }
                    },

                    { dataField: 'OID', type: 'text', align: 'center', cellsalign: 'center', text: 'OID', hidden: 'true' },

                    { dataField: 'Name', type: 'text', align: 'center', cellsalign: 'center', text: 'ECR_NO', width: '16%' },
                    { dataField: 'Title', type: 'text', align: 'center', cellsalign: 'center', text: '제목', width: '40%' },
                    { dataField: 'DesignChangeDt', type: 'text', align: 'center', cellsalign: 'center', text: '변경요청일자', cellsFormat: 'yyyy-MM-dd', width: '15%' },
                    { dataField: 'CreateDt', type: 'text', align: 'center', cellsalign: 'center', text: '작성일', cellsFormat: 'yyyy-MM-dd', width: '15%' },
                    { dataField: 'BPolicyOID', type: 'text', align: 'center', cellsalign: 'center', text: '상태', width: '6.4%' },

                ],
                rendertoolbar: function (toolBar) {
                    var container = $("<div class='lGridComponent' ></div>");
                    var AddButton = $("<button class='custom-button'><i class='fas fa-plus'></i> 추가</button>").jqxButton();
                    container.append(AddButton);
                    toolBar.append(container);

                    AddButton.click(function (event) {
                        var selectedRecords = new Array();
                        var SelectData = dlgSearchECRgrid$.jqxGrid('selectedrowindexes');
                        for (var i = 0; i < SelectData.length; i++) {
                            var row = dlgSearchECRgrid$.jqxGrid('getrowdata', SelectData[i]);
                            var param = {};
                            param.RootOID = _Param.RootOID;
                            param.ToOID = row.OID;
                            param.Type = _Param.Type;
                            selectedRecords[selectedRecords.length] = param;
                            row.Type = _Param.Type;
                            row.ToOID = row.OID;
                           
                            if (_Mod == "Create") {
                                parent.ECRRelationData.push(row);
                            }
                        }
                        if (selectedRecords.length == 0) {
                            alert('항목을 선택해주세요');
                            return false;
                        }
                        if (_Mod == "Create") {
                            if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                                _CallBackFunction(_Param);
                            }
                            PrintJqxGrid(_Source, _Grid, parent.ECRRelationData);
                            $(popLayer).jqxWindow('modalDestory');
                        } else if (_Mod == "Edit") {
                            RequestData('/ChangeOrder/InsECOContents', { _param: selectedRecords }, function (response) {
                                if (response.isError) {
                                    alert(response.resultMessage);
                                    return;
                                }
                                if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                                    _CallBackFunction(_Param);
                                }
                                $(popLayer).jqxWindow('modalDestory');
                            });
                        }



                        $(popLayer).jqxWindow('modalDestory');

                    });
                },
            });

            dlgSearchECRgrid$.on('rowselect', function (event) {
                var rowObj = event.args;
                var disChk = [];
                var rowsCount = dlgSearchECRgrid$.jqxGrid('getrows').length;
                if (ECRRelationList != null && ECRRelationList != undefined) {
                    ECRRelationList.filter(function (item) {
                        if (typeof rowObj.rowindex == 'number') {
                            if (item.OID == rowObj.row.OID) {
                                disChk.push(rowObj.rowindex);
                            }
                        } else if (typeof rowObj.rowindex == 'object') {
                            dlgSearchECRgrid$.jqxGrid('getrows').filter(function (innerItem) {
                                if (item.OID == innerItem.OID) {
                                    disChk.push(innerItem.uid);
                                }
                            });
                        }
                    });
                }

                if (event.args.rowindex.length === rowsCount) {
                    for (var index = 0; index < disChk.length; index++) {
                        var rowdata = dlgSearchECRgrid$.jqxGrid('getrowdatabyid', disChk[index]);
                        dlgSearchECRgrid$.jqxGrid('unselectrow', rowdata.boundindex);
                    }
                } else {
                    for (var index = 0; index < disChk.length; index++) {
                        dlgSearchECRgrid$.jqxGrid('unselectrow', disChk[index]);
                    }
                }
            });

            RequestData('/ChangeRequest/SelChangeRequest', null, function (res) {
                PrintJqxGrid(dlgSearchECRSource, dlgSearchECRgrid$, res);
            });
            $('#dlgSearchECR_SearchBtn').on('click', function () {
                var DesignChangeDt = $('#dlgSearchECR_DesignChangeDt').val();
                var DesignChangeDtArray = DesignChangeDt.split('-');
                var param = {};
                param.Name = $('#dlgSearchECR_Name').val();

                param.Title = $('#dlgSearchECR_Title').val();
                param.StartDesignChangeDt = DesignChangeDtArray[0];
                param.EndDesignChangeDt = DesignChangeDtArray[1] + " 23:59:59";;

                RequestData('/ChangeRequest/SelChangeRequest', param, function (res) {
                    PrintJqxGrid(dlgSearchECRSource, dlgSearchECRgrid$, res);
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


function fMoveGridRecusive(_rows, _target, _motion) {
    for (var i = 0; i < _rows.length; i++) {
        if (_rows[i].OID == _target) {
            if (_motion == 'UP') {
                if (i - 1 < 0) {
                    return;
                }

                const changeRow = _rows[i - 1];
                _rows[i - 1] = _rows[i];
                _rows[i] = changeRow;
                return;
            } else if (_motion == 'DOWN') {
                if (i + 1 >= _rows.length) {
                    return;
                }

                const changeRow = _rows[i + 1];
                _rows[i + 1] = _rows[i];
                _rows[i] = changeRow;
                return;
            } 
        }
    }
}

function OpenAssessManagerDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
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
        width: 800, maxWidth: 800, height: 650, minHeight: 650, resizable: false, zIndex: 99996, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
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
                if (_Param.OID == null) {
                    $('#managerOID_' + _Param.ToOID).val(returnList[0].OID);
                    $('#managerNm_' + _Param.ToOID).val(returnList[0].Name);
                } else {
                    $('#managerOID_' + _Param.ToOID + "_" + _Param.OID).val(returnList[0].OID);
                    $('#managerNm_' + _Param.ToOID + "_" + _Param.OID).val(returnList[0].Name);
                }
                if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                    _CallBackFunction();
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



function OpenEBomStructureDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
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
    var posY = (winHeight / 2) - (650 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 1200, maxWidth: 1200, height: 650, minHeight: 650, resizable: false, zIndex: 99996, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
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