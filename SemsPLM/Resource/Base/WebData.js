var DateFormat = {
    culture: 'ko-KR',
    formatString: 'yyyy-MM-dd',
    width: 140,
    clearString: 'Clear'
};

var renderWord = function (row, columnfield, value, defaulthtml, columnproperties) {
    return "<span class='renderText' title='" + value + "'>" + value + "</span>";
};

function RequestData(_Url, _Args, _CallBackFunc) {
    const loading = $('#loading');
    var retData = null;
    var param = _Args;
    loading.css('display', 'block');
    $.ajax({
        contentType: "application/json; charset=utf-8",
        type: "post",
        dataType: 'json',
        url: _Url,
        data: JSON.stringify(param),
        success: function (res) {
            if (res === null) {
                return;
            }
            if (res.isError) {
                alert(res.resultMessage);
                console.log(res.resultDescription);
                return;
            }
            retData = res;
            _CallBackFunc(retData);
        }, error: function (res) {
            alert(res.responseText);
            console.log(res);
        }, complete: function(){
            loading.css('display', 'none');
        }
    });
}

function RequestHtml(url, args, callBackFunc) {
    var param = args;
    $.ajax({
        contentType: "application/json; charset=utf-8",
        type: "post",
        dataType: 'html',
        url: url,
        data: JSON.stringify(param),
        success: function (res) {
            if (res === null) {
                return;
            }
            if (res.isError) {
                alert(res.resultMessage);
                console.log(res.resultDescription);
                return;
            }

            callBackFunc(res);
        }, error: function (res) {
            alert(res.responseText);
            console.log(res);
        }
    });
}

var SendDataWithFile = function (requestUrl, jsonData, fileList, callBackFunc) {
    var formData = objectToFormData(jsonData, null, null);

    if (!WebUtils.isEmpty(fileList)) {
        fileList.forEach(function (v, i) {
            formData.append("Files", v);
        });
    }

    $.ajax({
        contentType: false,
        processData: false,
        type: "post",
        url: requestUrl,
        data: formData,
        success: function (res) {
            if (res === null) {
                return;
            }
            if (res.isError) {
                alert(res.resultMessage);
                console.log(res.resultDescription);
                return;
            }

            retData = res;
            if (!(callBackFunc === undefined || callBackFunc === null)) {
                callBackFunc(retData);
            }
        }, error: function (res) {
            alert(res.responseText);
            console.log(res);
        }
    });
}


// 2020.10.20 김성현
// Json 형식 데이터를 FormData 로 변경해주는 기능
function objectToFormData(obj, rootName, ignoreList) {
    var ff = document.createElement('form');
    ff.enctype = 'multipart/form-data';
    ff.method = 'post';

    var formData = new FormData(ff);

    function appendFormData(data, root) {
        if (!ignore(root)) {
            root = root || '';
            if (data instanceof File) {
                formData.append(root, data);
            } else if (Array.isArray(data)) {
                for (var i = 0; i < data.length; i++) {
                    appendFormData(data[i], root + '[' + i + ']');
                }
            } else if (typeof data === 'object' && data) {
                for (var key in data) {
                    if (data.hasOwnProperty(key)) {
                        if (root === '') {
                            appendFormData(data[key], key);
                        } else {
                            appendFormData(data[key], root + '.' + key);
                        }
                    }
                }
            } else {
                if (data !== null && typeof data !== 'undefined') {
                    formData.append(root, data);
                }
            }
        }
    }

    function ignore(root) {
        return Array.isArray(ignoreList)
            && ignoreList.some(function (x) { return x === root; });
    }

    appendFormData(obj, rootName);

    return formData;
}

function PrintJqxGrid(_Source, _GridObject, _JsonResult) {
    _Source.localdata = _JsonResult;

    var Adapter = new $.jqx.dataAdapter(_Source);
    //Adapter.dataBind();

    _GridObject.jqxGrid({ source: Adapter });
    Adapter = null;
}

function PrintJqxTreeGrid(_Source, _GridObject, _JsonResult) {
    _Source.localdata = _JsonResult;

    var Adapter = new $.jqx.dataAdapter(_Source);
    //Adapter.dataBind();

    _GridObject.jqxTreeGrid({ source: Adapter });
    Adapter = null;
}

function PrintJqxKanban(_Source, _GridObject, _JsonResult) {
    _Source.localdata = _JsonResult;

    var Adapter = new $.jqx.dataAdapter(_Source);
    //Adapter.dataBind();

    _GridObject.jqxKanban({ source: Adapter });
    Adapter = null;
}

function PrintJqxCombo(_Source, _GridObject, _JsonResult) {
    _Source.localdata = _JsonResult;

    var Adapter = new $.jqx.dataAdapter(_Source);
    //Adapter.dataBind();

    _GridObject.jqxComboBox({ source: Adapter });
    Adapter = null;
}

function fStatusAction(_status, _current, _oid) {
    for (var index = 0; index < _status.length; index++) {
        if (_status[index].getAttribute('data-Status') == _current) {
            continue;
        }

        _status[index].addEventListener('click', function (event) {
           
        });

        _status[index].addEventListener('mouseover', function (event) {
            this.style.color = '#1E5D88';
            this.style.fontWeight = "bold";
            this.style.cursor = "pointer";
            this.style.backgroundImage = "url('/images/status_over.png')";
        });
        _status[index].addEventListener('mouseout', function (event) {
            this.style.color = 'black';
            this.style.fontWeight = "normal";
            this.style.backgroundImage = "url('/images/status_w.png')";
        });
    }
}