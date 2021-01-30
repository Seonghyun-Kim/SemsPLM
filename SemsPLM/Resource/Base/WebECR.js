function OpenSearchECODialog(_CallBackFunction, _Wrap, _Param, _Url, _Title, _Grid, _Source, _Mod, _RootOID) {
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

            var dlgSearchECOSource =
            {
                dataType: "json",
                dataFields: [
                    { name: 'OID' },
                    { name: 'Name' },
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
            var dlgSearchECOdataAdapter = new $.jqx.dataAdapter(dlgSearchECOSource);
            const dlgSearchECOgrid$ = $('#dlgSearchECOgrid');
            dlgSearchECOgrid$.jqxGrid('render');
            dlgSearchECOgrid$.jqxGrid({
                width: "100%",
                theme: "kdnc",
                height: 510,
                sortable: true,
                showToolbar: true,
                toolbarHeight: 44,
                editable: false,
                selectionmode: 'checkbox',
                source: dlgSearchECOdataAdapter,
                ready: function () {

                },
                columns: [
                    {
                        text: 'NO', width: "6%", cellsalign: 'center', columntype: 'number', align: 'center',
                        cellsrenderer: function (row, column, value) {
                            return "<div style='width:100%;height:100%;text-align:center;vertical-align:middle;line-height:1.9;'>" + (value + 1) + "</div>";
                        }
                    },

                    { name: 'OID', dataField: 'OID', type: 'text', align: 'center', cellsalign: 'center', text: 'OID', hidden: 'true' },

                    { name: 'Name', dataField: 'Name', type: 'text', align: 'center', cellsalign: 'center', text: 'ECO', width: '56%' },
                    { name: 'DesignChangeDt', dataField: 'DesignChangeDt', type: 'text', align: 'center', cellsalign: 'center', text: '설계변경일자', cellsFormat: 'yyyy-MM-dd', width: '15%' },
                    { name: 'CreateDt', dataField: 'CreateDt', type: 'text', align: 'center', cellsalign: 'center', text: '작성일', cellsFormat: 'yyyy-MM-dd', width: '15%' },
                    {
                        text: '상태', datafield: 'BPolicy', width: "6%", align: 'center', cellsalign: 'center',
                        cellsrenderer: function (row, column, value) {
                            return "<div style='width:100%;height:100%;text-align:center;vertical-align:middle;line-height:1.9;'>" + value.StatusNm + "</div>";
                        }
                    },
                    //{ name: 'BPolicyOID', dataField: 'BPolicyOID', type: 'text', align: 'center', cellsalign: 'center', text: '상태', width: '5.9%' },

                ],
                rendertoolbar: function (toolBar) {
                    var container = $("<div class='lGridComponent' ></div>");
                    var AddButton = $("<button class='custom-button'><i class='fas fa-plus'></i> 추가</button>").jqxButton();
                    container.append(AddButton);
                    toolBar.append(container);

                    AddButton.click(function (event) {
                        var SelectAllRow = [];

                        var SelectData = dlgSearchECOgrid$.jqxGrid('selectedrowindexes');
                        for (var i = 0; i < SelectData.length; i++) {
                            var row = dlgSearchECOgrid$.jqxGrid('getrowdata', SelectData[i]);
                            _Param.push(row);
                            SelectAllRow.push(row);
                        }

                        if (_Mod == "Edit") {
                            RequestData('/ChangeRequest/InsECRRelationContents', { RootOID: _RootOID, _param: SelectAllRow }, function (response) {
                                if (response.isError) {
                                    alert(response.resultMessage);
                                    return;
                                }
                                alert("저장되었습니다.");
                                if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                                    _CallBackFunction();
                                }
                            });
                        } else {
                            PrintJqxGrid(_Source, _Grid, _Param);
                        }

                        $(popLayer).jqxWindow('modalDestory');
                    });
                },
            });

            dlgSearchECOgrid$.on('rowselect', function (event) {
                console.log(_Param);
                var rowObj = event.args;
                var disChk = [];

                var rowsCount = dlgSearchECOgrid$.jqxGrid('getrows').length;
                

                if (_Param != null && _Param != undefined) {
                    _Param.filter(function (item) {
                        if (typeof rowObj.rowindex == 'number') {
                            if (item.OID == rowObj.row.OID) {
                                disChk.push(rowObj.rowindex);
                            }
                        } else if (typeof rowObj.rowindex == 'object') {
                            dlgSearchECOgrid$.jqxGrid('getrows').filter(function (innerItem) {
                                if (item.OID == innerItem.OID) {
                                    disChk.push(innerItem.uid);
                                }
                            });
                        }
                    });
                }

                if (event.args.rowindex.length === rowsCount) {
                    for (var index = 0; index < disChk.length; index++) {
                        var rowdata = dlgSearchECOgrid$.jqxGrid('getrowdatabyid', disChk[index]);
                        dlgSearchECOgrid$.jqxGrid('unselectrow', rowdata.boundindex);
                    }
                } else {
                    for (var index = 0; index < disChk.length; index++) {
                        dlgSearchECOgrid$.jqxGrid('unselectrow', disChk[index]);
                    }
                }
            });
            RequestData('/ChangeOrder/SelChangeOrder', null, function (res) {
                PrintJqxGrid(dlgSearchECOSource, dlgSearchECOgrid$, res);
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