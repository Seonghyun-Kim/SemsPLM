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
    var retData = null;
    var param = _Args;
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

function PrintJqxGrid(_Source, _GridObject, _JsonResult) {
    _Source.localdata = _JsonResult;

    var Adapter = new $.jqx.dataAdapter(_Source);
    Adapter.dataBind();

    _GridObject.jqxGrid({ source: Adapter });
    Adapter = null;
}

function PrintJqxTreeGrid(_Source, _GridObject, _JsonResult) {
    _Source.localdata = _JsonResult;

    var Adapter = new $.jqx.dataAdapter(_Source);
    Adapter.dataBind();

    _GridObject.jqxTreeGrid({ source: Adapter });
    Adapter = null;
}

function PrintJqxKanban(_Source, _GridObject, _JsonResult) {
    _Source.localdata = _JsonResult;

    var Adapter = new $.jqx.dataAdapter(_Source);
    Adapter.dataBind();

    _GridObject.jqxKanban({ source: Adapter });
    Adapter = null;
}

function PrintJqxCombo(_Source, _GridObject, _JsonResult) {
    _Source.localdata = _JsonResult;

    var Adapter = new $.jqx.dataAdapter(_Source);
    Adapter.dataBind();

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