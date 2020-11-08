
 
function ffEPartArray(_rows, _level) {
    if (_rows != null && _rows.length > 0) {
        ffEPartArrayRecusive(_rows, _level);
    }
}

function ffEPartArrayRecusive(_row, _level) {
    for (var i = 0; i < _row.length; i++) {
        _row[i].Level = _level;
        if (_row[i].OID == null) {
            _row[i].OID = new Date().getTime();
        }
        if (_row[i].Children) {
            ffEPartArrayRecusive(_row[i].Children, (_level+1));
        }
    }
}

function fEPartArray(_rows) {
    var returnList = [];
    if (_rows != null && _rows.length > 0) {
        fEPartArrayRecusive(_rows, returnList);
    }
    return returnList;
}

function fEPartArrayRecusive(_row, _arrResult) {
    for (var i = 0; i < _row.length; i++) {
        _arrResult.push(_row[i]);
        if (_row[i].Children) {
            fEPartArrayRecusive(_row[i].Children, _arrResult);
        }
    }
}

function fEPartMoveRecusive(_rows, _target, _motion, _movecount) {

    fEPartMoveChildrenRecusive(_rows, _target, _motion, _movecount);
}

function fEPartMoveChildrenRecusive(_rows, _target, _motion, _movecount) {
    for (var i = 0; i < _rows.length; i++) {
        if (_rows[i] == _target) {
            if (_motion == 'RU') {
                if (i - _movecount < 0) {
                    return;
                }

                const changeRow = _rows[i - _movecount];
                _rows[i - _movecount] = _rows[i];
                _rows[i] = changeRow;
                return;
            }
        }
        if (_rows[i].Children) {
            fEPartMoveRecusive(_rows[i].Children, _target, _motion, _movecount);
        }
    }
}


