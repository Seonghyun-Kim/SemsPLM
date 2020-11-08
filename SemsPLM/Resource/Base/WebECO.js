function OpenSearchEPartDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title, _Type) {
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
    var posX = (winWidth / 2) - (1200 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (650 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 1200, maxWidth: 1200, height: 650, minHeight: 650, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {
            if (_CallBackFunction != null && typeof _CallBackFunction == 'function') {
                _CallBackFunction();
            }

            $('#parentOID').val(_Param.RootOID);
            var EPartSource = {
                datatype: 'json',
                datafields: [
                    { name: 'OID' },
                    { name: 'Name' },
                    { name: 'CreateUsNm' },
                    { name: 'CreateDt', type: 'date' },
                    { name: 'Description' },
                    { name: 'Title' },
                    { name: 'Car_Lib_OID' },
                    { name: 'Car_Lib_NM' },
                    { name: 'Eo_No' },
                    { name: 'Eo_No_ApplyDt', type: 'date' },
                    { name: 'Etc' },
                    { name: 'EPartType' },
                    { name: 'Sel_Eo' },
                    { name: 'Sel_Eo_Dt', type: 'date' },
                    { name: 'Spec' },
                    { name: 'EPartType' },
                    { name: 'Thumbnail' },
                    { name: 'Revision' },
                    { name: 'Surface' },
                    { name: 'BPolicy' },
                    { name: 'BPolicyOID', type: 'number' },
                ],
            }

            const SearchEPartGrid$ = $('#dlgSearchEPart_grid');
            SearchEPartGrid$.jqxGrid({
                theme: "kdnc",
                width: "100%",
                height: 520,
                rowsheight: 28,
                selectionmode: 'checkbox',
                columnsheight: 30,
                //source: ProjectSource,
                sortable: false,
                pageable: false,
                columns: [
                    { text: '차종', datafield: 'Car_Lib_NM', width: "15%", align: 'center', cellsalign: 'center', },

                    { text: '품번', datafield: 'Name', width: "25%", align: 'center', cellsalign: 'center', },
                    { text: '품명', datafield: 'Title', width: "25%", align: 'center', cellsalign: 'center', },
                    { text: '타입', datafield: 'EPartType', width: "9%", align: 'center', cellsalign: 'center', },
                    { text: 'C/N', datafield: 'Revision', width: "9%", align: 'center', cellsalign: 'center', },

                    {
                        text: '상태', datafield: 'BPolicy', width: "9%", align: 'center', cellsalign: 'center',
                        cellsrenderer: function (row, column, value) {
                            return "<div style='width:100%;height:100%;text-align:center;vertical-align:middle;line-height:1.9;'>" + value.StatusNm + "</div>";
                        },
                    },
                    { text: '작성자', datafield: 'CreatUsNm', width: "8%", align: 'center', cellsalign: 'center' },


                    //   { text: '내용', datafield: 'Description', width: "30%", align: 'center', cellsalign: 'center', },
                ],
                showtoolbar: true,
                toolbarheight: 45,
                renderToolbar: function (statusbar) {

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
                        }
                        if (selectedRecords.length == 0) {
                            alert('품목을 선택해주세요');
                            return false;
                        }
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
                    });
                }

            });
            SearchEPartGrid$.on('rowselect', function (event) {
                var rowObj = event.args;
                var disChk = [];
                console.log(partOIDList);
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
                    RequestData('/EBom/SelEPart', param, function (res) {
                        PrintJqxGrid(EPartSource, SearchEPartGrid$, res);
                    });
                });

            } else if (_Type == 'EpartChild') {

                $('#dlgSearchEPart_searchForm').attr('hidden', true);

                var param = {};
                param.RootOID = _Param.RootOID;
                param.FromOID = _Param.FromOID;
                RequestData('/EBom/SelRootChildList', param, function (res) {
                    PrintJqxGrid(EPartSource, SearchEPartGrid$, res);
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