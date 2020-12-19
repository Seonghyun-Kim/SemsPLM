(function ($) {
    var defaults = {
        OID: null,
        module: null,
        submodule: null,
        targetoid: null,
        modulestatus: 83,
        width: "100%",
        height: "340px",
        //url: "/Common/CommonFileUpload",
        uploadMultiple: true,
        parallelUploads: 20,
        maxFiles: 20,
        maxFilesize: 2048,
        thumbnailWidth: 80,
        thumbnailHeight: 80,
        method: 'post',
    }

    function FileUpload(element, options) {
        var wrap = null;
        if (typeof element === "undefined") {
            wrap = document.body;
        } else {
            wrap = element;
        }

        this.options = $.extend({}, defaults, options);
        this.init(wrap);
    }


    FileUpload.prototype = {
        DropFileContent: null,
        FileGrid: null,
        init: function (wrap) {
            var fragment = document.createDocumentFragment();
            var wrapDropzone = document.createElement("div");
            wrapDropzone.style.height = "100%";
            wrapDropzone.style.width = this.options.width;

            var drop = document.createElement("div");
            drop.className = "dropzone";
            drop.style.height = "130px";
            drop.style.display = "none";

            var FileList = document.createElement("div");
            FileList.style.height = "200px";
            FileList.style.marginTop = "10px";

            wrapDropzone.appendChild(drop);
            wrapDropzone.appendChild(FileList);

            fragment.appendChild(wrapDropzone);

            var CellNoWidth = 40;
            var CellCreateUsNm = 80;
            var CellCreateDt = 100;
            var CellButton = 65;
            var CellFileOrdNm = $(wrap).width() - (CellNoWidth + CellCreateUsNm + CellCreateDt + CellButton + 20);
            var OID = this.options.OID;
            var SelectionMode = "checkbox";

            this.DropFileContent = new Dropzone(drop, {
                url: 'upload',
                addRemoveLinks: true,
                autoProcessQueue: false,
                parallelChunkUploads: true,
                uploadMultiple: this.options.uploadMultiple,
                parallelUploads: this.options.parallelUploads,
                maxFiles: this.options.maxFiles,
                maxFilesize: this.options.maxFilesize,
                method: 'this.options',
                init: function () {
                    fileDropFrm = this;
                }
            });

            this.DropFileContent.on("addedfile", function (file) {
                if (file.size === 0) {
                    alert("사이즈가 0인 파일은 업로드 할 수 없습니다.");
                    this.removeFile(file);
                }
            });

            var fileSource =
            {
                datatype: "json",
                datafields: [
                    { name: 'FileOID' },
                    { name: 'OrgNm' },
                    { name: 'Type' },
                    { name: 'OID' },
                    { name: 'FileSize' },
                    { name: 'CreateUsNm' },
                    { name: 'CreateDt', type: "date" },
                    { name: 'IsDel', type: "bool" }
                ],
                id: 'FileOID',
                url: "/Common/GetFileList",
                data: {
                    OID: OID
                },
                type: "POST"
            };

            var GridCellFileBtn = function (row, columnfield, value, defaulthtml, columnproperties, rowData) {
                var DownloadBtn$ = "<button style='width:26px;' title='다운로드' onclick='javascript:WebUtils.CommonFileDownload(" + rowData.FileOID + ");'><i class='fas fa-file-download'></i></button>";
                var ViewerBtn$ = "<button style='width:26px;' title='뷰어'><i class='fas fa-book-reader'></i></button>";
                return "<div style='display:flex;justify-content:space-around;align-items:center;width:100%;height:100%;text-align:center;vertical-align:middle;'>" + DownloadBtn$ + ViewerBtn$ + "</div>";
            }

            this.FileGrid = $(FileList).jqxGrid({
                theme: "kdnc",
                width: "99.8%",
                height: "190px",
                rowsheight: 35,
                columnsheight: 37,
                source: fileSource,
                pageable: false,
                sortable: false,
                scrollbarsize: 17,
                selectionmode: SelectionMode,
                showtoolbar: true,
                toolbarHeight: 45,
                rendertoolbar: function (toolBar) {
                    var container = $("<div class='lGridComponent' ></div>");
                    var btnFileDel$ = $("<button class='custom-button'><span class='ico-del'>삭제</span></button>").jqxButton();
                    $(btnFileDel$).on("click", function () {
                        var selection = $(FileList).jqxGrid('getselectedrowindexes');

                        if (WebUtils.isEmpty(selection)) {
                            alert("선택된 항목이 없습니다");
                            return;
                        } else {
                            if (!confirm("저장시 삭제가 완료됩니다. 삭제하시겠습니까?")) {
                                return;
                            }

                            selection.forEach(function (v) {
                                var rowData = $(FileList).jqxGrid('getrowdata', v);
                                rowData.IsDel = true;
                                $(FileList).jqxGrid('updaterow', rowData.uid, rowData);
                                $(FileList).jqxGrid("hiderow", rowData.visibleindex);
                            });
                        }
                    });
                    container.append(btnFileDel$);
                    toolBar.append(container);
                },
                columns: [
                    {
                        text: '제목', datafield: 'OrgNm', resizable: true, align: 'center', cellsalign: 'left', width: '76.2%', cellsrenderer: function (row, columnfield, value, defaulthtml, columnproperties) {
                            return "<div style='display:flex;align-items:center;width:100%;height:100%;' title='" + value + "'><div style='width:calc(100% - 8px);margin-left:8px;'>" + value + "</div></div>";
                        }
                    },
                    { text: '작성자', datafield: 'CreateUsNm', width: CellCreateUsNm, align: 'center', cellsalign: 'center', width:'6%', },
                    { text: '작성일', datafield: 'CreateDt', width: CellCreateDt, align: 'center', cellsalign: 'center', cellsformat: 'yyyy-MM-dd', width: '10%', },
                    { text: '', datafield: '', width: CellButton, align: 'center', cellsalign: 'center', cellsrenderer: GridCellFileBtn, width: '6%', },
                ]
            });

            wrap[0].appendChild(fragment);
            this.FileGrid.jqxGrid({ showtoolbar: false });

        },

        Files: function () {
            var DropDiv = this.DropFileContent;
            if (DropDiv.files.length <= 0) {
                //if (!isNotCheck) {
                //    alert("업로드할 파일이 없습니다.");
                //}
                //alert("업로드할 파일이 없습니다.");
                return null;
            }
            return DropDiv.files;
        },

        ClearFile: function () {
            var DropDiv = this.DropFileContent;
            if (DropDiv.files.length > 0) {
                DropDiv.files.forEach(function (v, i) {
                    this.removeFile(v);
                });
            }
        },

        ReadOnlyMode: function () {
            var DropDiv = this.DropFileContent;
            DropDiv.element.style.display = "none";

            $(this.FileGrid).jqxGrid({ showtoolbar: false });
        },

        EditMode: function () {
            var DropDiv = this.DropFileContent;
            DropDiv.element.style.display = "block";

            $(this.FileGrid).jqxGrid({ showtoolbar: true });
        },

        Reload: function () {
            ClearFile();
            $(this.FileGrid).jqxGrid("updatebounddate");
        },

        destroy: function () {
            var _dropzoneObject = this.DropFileContent;
            _dropzoneObject.emit("destroy");
        },

        RemoveFiles: function (a) {
            var rows = $(this.FileGrid).jqxGrid('getrows');

            var retValue = rows.filter(function (v) {
                if (v.IsDel) { return v; }
            });

            return retValue;
        }
    };

    $.fn.FileUpload = function (data) {
        return new FileUpload(this, data);
    };
}(jQuery));

