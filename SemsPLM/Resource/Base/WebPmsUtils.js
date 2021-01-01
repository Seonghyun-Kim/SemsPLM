function fPmsLoadProperties(_rows, _property) {
    const arrProperties = [];
    const arrRows = fPmsArrayTasks(_rows);
    for (var i = 0; i < arrRows.length; i++) {
        arrProperties.push(arrRows[i][_property]);
    }
    return arrProperties;
}

function fPmsArrayDataRecusive(_rows, _dataFields, _removeDataFields) {
    var returnList = [];
    fArrayRecusive(_rows, returnList, []);
    const gettingData = [];
    for (var key in returnList) {
        var dataObj = {};
        for (var innerKey in _dataFields) {
            if (_removeDataFields.indexOf(_dataFields[innerKey]['name']) > -1) {
                continue;
            }
            dataObj[_dataFields[innerKey]['name']] = returnList[key][_dataFields[innerKey]['name']];
        }
        gettingData.push(dataObj);
    }
    return gettingData;
}

function fPmsArrayTasks(_rows) {
    var returnList = [];
    if (_rows != null && _rows.length > 0) {
        fArrayRecusive(_rows, returnList, ['PROJECT', 'PROJECT_TEMP']);
    }
    return returnList;
}

function fArrayRecusive(_row, _arrResult, _removeTypes) {
    for (var i = 0; i < _row.length; i++) {
        if (_.indexOf(_removeTypes, _row[i].ObjType) < 0) {
            _arrResult.push(_row[i]);
        }
        if (_row[i].Children) {
            fArrayRecusive(_row[i].Children, _arrResult, _removeTypes);
        }
    }
}

function fPmsParentArray(_rows) {
    var returnList = [];
    if (_rows != null && _rows.length > 0) {
        fArrayParentRecusive(_rows, returnList);
    }
    return returnList;
}

function fArrayParentRecusive(_row, _arrResult) {
    for (var i = 0; i < _row.length; i++) {
        _arrResult.push(_row[i]);
        if (_row[i].parent) {
            fArrayParentRecusive([_row[i].parent], _arrResult);
        }
    }
}

function fMasterLink(_arrTasks) {
    var returnList = [];
    var arryTasks = _arrTasks;
    for (var index = 0; index < arryTasks.length; index++) {
        if (arryTasks[index].Dependency != null && arryTasks[index].Dependency.length > 0) {
            var splitDepId = arryTasks[index].Dependency.split(':');
            var fromData = arryTasks.filter(function (item) {
                return item.Id == splitDepId[0]
            });
            var returnObj = {};
            returnObj.from = fromData[0];
            returnObj.to = arryTasks[index];
            returnList.push(returnObj);
        }
    }
    return returnList;
}

function fComputeCriticalPath(_tasks, _links) {
    for (var i = 0; i < _tasks.length; i++) {
        var t = _tasks[i];
        t.earlyStart = -1;
        t.earlyFinish = -1;
        t.latestStart = -1;
        t.latestFinish = -1;
        t.criticalCost = -1;
        t.isCritical = false;
    }

    // tasks whose critical cost has been calculated
    var completed = [];
    // tasks whose critical cost needs to be calculated
    var remaining = _tasks.concat(); // put all tasks in remaining

    // Backflow algorithm
    // while there are tasks whose critical cost isn't calculated.
    while (remaining.length > 0) {
        var progress = false;

        // find a new task to calculate
        for (var i = 0; i < remaining.length; i++) {
            var task = remaining[i];
            var inferiorTasks = getInferiorTasks(task, _links);

            if (containsAll(completed, inferiorTasks)) {
                // all dependencies calculated, critical cost is max dependency critical cost, plus our cost
                var critical = 0;
                for (var j = 0; j < inferiorTasks.length; j++) {
                    var t = inferiorTasks[j];
                    if (t.criticalCost > critical) {
                        critical = t.criticalCost;
                    }
                }
                task.criticalCost = critical + task.EstDuration;
                // set task as calculated an remove
                completed.push(task);
                remaining.splice(i, 1);

                // note we are making progress
                progress = true;
            }
        }
        // If we haven't made any progress then a cycle must exist in
        // the graph and we wont be able to calculate the critical path
        if (!progress) {
            console.error("Cyclic dependency, algorithm stopped!");
            return false;
        }
    }

    // set earlyStart, earlyFinish, latestStart, latestFinish
    computeMaxCost(_tasks);
    var initialNodes = initials(_tasks);
    calculateEarly(initialNodes, _links);
    calculateCritical(_tasks);

    return _tasks;

    function containsAll(set, targets) {
        for (var i = 0; i < targets.length; i++) {
            if (set.indexOf(targets[i]) < 0)
                return false;
        }
        return true;
    }

    function getInferiors(_task, _links) {
        var ret = [];
        var task = _task;
        if (_links) {
            ret = _links.filter(function (link) {
                return link.from == task;
            });
        }
        return ret;
    }

    function getInferiorTasks(_task, _links) {
        var ret = [];
        var infs = getInferiors(_task, _links);
        for (var i = 0; i < infs.length; i++)
            ret.push(infs[i].to);
        return ret;
    }


    function computeMaxCost(_tasks) {
        var max = -1;
        for (var i = 0; i < _tasks.length; i++) {
            var t = _tasks[i];

            if (t.criticalCost > max)
                max = t.criticalCost;
        }
        //console.debug("Critical path length (cost): " + max);
        for (var i = 0; i < _tasks.length; i++) {
            var t = _tasks[i];
            t.latestStart = max - t.criticalCost;
            t.latestFinish = t.latestStart + t.EstDuration;
        }
    }

    function initials(_tasks) {
        var initials = [];
        for (var i = 0; i < _tasks.length; i++) {
            if (!_tasks[i].Dependency || _tasks[i].Dependency == "")
                initials.push(_tasks[i]);
        }
        return initials;
    }

    function calculateEarly(_initials, _links) {
        for (var i = 0; i < _initials.length; i++) {
            var initial = _initials[i];
            initial.earlyStart = 0;
            initial.earlyFinish = initial.EstDuration;
            setEarly(initial, _links);
        }
    }

    function setEarly(_initial, _links) {
        var completionTime = _initial.earlyFinish;
        var inferiorTasks = getInferiorTasks(_initial, _links);
        for (var i = 0; i < inferiorTasks.length; i++) {
            var t = inferiorTasks[i];
            if (completionTime >= t.earlyStart) {
                t.earlyStart = completionTime;
                t.earlyFinish = completionTime + parseInt(t.EstDuration);
            }
            setEarly(t, _links);
        }
    }

    function calculateCritical(_tasks) {
        for (var i = 0; i < _tasks.length; i++) {
            var t = _tasks[i];
            t.isCritical = (t.earlyStart == t.latestStart)
        }
    }
}

function fMoveRecusive(_rows, _target, _motion) {
    fMoveChildrenRecusive(_rows, _target, _motion);
}

function fMoveChildrenRecusive(_rows, _target, _motion) {
    for (var i = 0; i < _rows.length; i++) {
        if (_rows[i].ToOID == _target) {
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
            } else if (_motion == 'LEFT') {
                const tmpTarget = _rows[i];
                const tmpMainParent = _rows[i].parent.parent;
                const tmpSubParent = _rows[i].parent;
                if (tmpMainParent == undefined || tmpSubParent == undefined) {
                    return;
                }
                if (tmpMainParent.Children.filter(function (_data) { return _data.ToOID == parseInt(_target) }).length > 0) {
                    return;
                }
                const tmpTargetIndex = tmpSubParent.Children.findIndex(function (item) { return item.RelOID == tmpTarget.RelOID });
                tmpSubParent.Children.splice(tmpTargetIndex, 1);
                tmpTarget.Level = tmpTarget.Level - 1;
                tmpTarget.FromOID = tmpMainParent.ToOID;
                tmpMainParent.Children.push(tmpTarget);
                return;
            } else if (_motion == 'RIGHT') {
                return;
            }
        }
        if (_rows[i].Children) {
            fMoveChildrenRecusive(_rows[i].Children, _target, _motion);
        }
    }
}

function fPmsModifyValueRecusive(_obj$, _datas, _value) {
    if (_datas != undefined && _datas.length > 0) {
        for (var i = 0; i < _datas.length; i++) {
            var rowKey = _obj$.jqxTreeGrid('getKey', _datas[i]);
            _obj$.jqxTreeGrid('setCellValue', rowKey, 'EstStartDt', _value);
            fPmsModifyValueRecusive(_obj$, _datas[i].records, _value);
        }
    }
}

function fWeekendCalcDuration(fromDate, duration, workingDay, holiday) {
    var date1 = new Date(fromDate);
    var index = gDeaultWorkDay;
    var temp_date = date1;
    if (duration < 1) {
        duration = 1;
    }
    while (true) {
        if (index == duration) {
            break;
        } else {
            temp_date.setDate(temp_date.getDate() + 1);
            var tmp = temp_date.getDay();
            if (fPmsWokingDay(workingDay, tmp) || holiday.indexOf('#' + moment(temp_date).format('YYYY-MM-DD') + '#') > -1) {
            } else {
                index++;
            }
        }
    }
    return temp_date;
}

function fPmsWokingDay(_validaciton, _param) {
    var result = false;
    if (_validaciton == 6) {
        if (_param == 0) {
            result = true;
        }
    } else if (_validaciton == 5) {
        if (_param == 0 || _param == 6) {
            result = true;
        }
    }
    return result;
}

function fPmsModifyValueReverseRecusive(_obj$, _data, _value) {
    if (_data != undefined) {
        var rowKey = _data.ToOID;
        if ((new Date(_data.EstEndDt).getTime()) < (new Date(_value).getTime())) {
            _obj$.jqxTreeGrid('setCellValue', rowKey, 'EstEndDt', _value);
            _obj$.jqxTreeGrid('setCellValue', rowKey, 'EstDuration', fWeekendCalc(_data.EstStartDt, _value, _data.WorkingDay, gPmsModifyHistoryHoliday[_data.RootOID]));
        } else {
            const arrRows = fPmsArrayTasks(_data.Children);
            if (arrRows != null && arrRows.length > 0) {
                const maxEstEndDt = moment(fMaxDate(fPmsLoadProperties(arrRows, 'EstEndDt'))).format('YYYY-MM-DD');
                _obj$.jqxTreeGrid('setCellValue', rowKey, 'EstEndDt', maxEstEndDt);
                _obj$.jqxTreeGrid('setCellValue', rowKey, 'EstDuration', fWeekendCalc(_data.EstStartDt, maxEstEndDt, _data.WorkingDay, gPmsModifyHistoryHoliday[_data.RootOID]));
            }
        }
        if (_data.parent != null && _data.parent != undefined) {
            fPmsModifyValueReverseRecusive(_obj$, _data.parent, _value);
        }
    }
}

function fWeekendCalc(fromDate, toDate, workingDay, holiday) {
    var date1 = new Date(fromDate);
    var date2;
    if (typeof toDate == 'string') {
        date2 = moment(toDate).toDate();
    } else {
        date2 = new Date(toDate);
    }
    var count = gDeaultWorkDay;
    var temp_date = date1;
    while (true) {
        if (temp_date.getTime() >= date2.getTime()) {
            break;
        } else {
            temp_date.setDate(temp_date.getDate() + 1);
            var tmp = temp_date.getDay();
            if (fPmsWokingDay(workingDay, tmp) || holiday.indexOf('#' + moment(temp_date).format('YYYY-MM-DD') + '#') > -1) {
            } else {
                count++;
            }
        }
    }
    return count;
}

function fMaxDate(all_dates) {
    var max_dt = all_dates[0],
        max_dtObj = new Date(all_dates[0]);
    all_dates.forEach(function (dt, index) {
        if (new Date(dt) > max_dtObj) {
            max_dt = dt;
            max_dtObj = new Date(dt);
        }
    });
    return max_dt;
}

function fDependencyRescureControl(_projOid, _selData) {
    const obj$ = gPmsModifyHistoryMaster[_projOid].obj;
    const allDatas = fPmsArrayTasks(obj$.jqxTreeGrid('getRows'));

    const targetData = fListMatch(allDatas, function (x) { return fGettingDependData(_selData.Dependency).hasOwnProperty(x.Id); });
    if (targetData.length > 0) {
        for (var index = 0; index < targetData.length; index++) {
            obj$.jqxTreeGrid('setCellValue', _selData.ToOID, 'EstStartDt', fWeekendCalcDuration(targetData[index].EstEndDt, (1 + fGettingDependData(_selData.Dependency)[targetData[index].Id]), _selData.WorkingDay, gPmsModifyHistoryHoliday[_projOid]));
        }
    }
    fDependencyRescureDataControl(obj$, _projOid, _selData, allDatas);
}

function fDependencyRescureDataControl(obj$, projOid, selData, datas) {
    const targetData = fListMatch(datas, function (x) { return x.Dependency != null && fGettingDependData(x.Dependency).hasOwnProperty(selData.Id); });
    if (targetData.length > 0) {
        for (var index = 0; index < targetData.length; index++) {
            obj$.jqxTreeGrid('setCellValue', targetData[index].ToOID, 'EstStartDt', fWeekendCalcDuration(selData.EstEndDt, (1 + fGettingDependData(targetData[index].Dependency)[selData.Id]), selData.WorkingDay, gPmsModifyHistoryHoliday[projOid]));
            fDependencyRescureDataControl(obj$, projOid, targetData[index], datas);
        }
    }
}

function fDependencyRecureTargetDataControl(obj$, projOid, selData, datas) {
    const targetData = fListMatch(datas, function (x) { return fGettingDependData(selData.Dependency).hasOwnProperty(x.Id); });
    if (targetData.length > 0) {
        for (var index = 0; index < targetData.length; index++) {
            obj$.jqxTreeGrid('setCellValue', selData.ToOID, 'EstStartDt', fWeekendCalcDuration(targetData[index].EstEndDt, (1 + fGettingDependData(selData.Dependency)[targetData[index].Id]), selData.WorkingDay, gPmsModifyHistoryHoliday[projOid]));
            fDependencyRecureTargetDataControl(obj$, projOid, targetData[index], datas);
        }
    }
}

function fDependencyControl(_projOid) {
    const obj$ = gPmsModifyHistoryMaster[_projOid].obj;
    const allDatas = fPmsArrayTasks(obj$.jqxTreeGrid('getRows'));

    for (var index = 0; index < allDatas.length; index++) {
        fDependencyDataControl(obj$, _projOid, index, allDatas);
    }

    for (var index = allDatas.length - 1; index >= 0; --index) {
        fDependencyDataControl(obj$, _projOid, index, allDatas);
    }
}

function fDependencyDataControl(obj$, projOid, idx, datas) {
    if (datas[idx].Dependency != null && datas[idx].Dependency.length > 0) {
        const val = datas[idx].Dependency;
        if (val.length > 0) {
            const splitValue = val.split(':');
            var splitDuration = 0;
            if (splitValue.length > 1) {
                splitDuration = parseInt(splitValue[1]) - 1;
            }
            const targetData = fListMatch(datas, function (x) { return x.Id == splitValue[0] });
            if (targetData.length > 0) {
                obj$.jqxTreeGrid('setCellValue', datas[idx].ToOID, 'EstStartDt', fWeekendCalcDuration(targetData[0].EstEndDt, (2 + splitDuration), datas[idx].WorkingDay, gPmsModifyHistoryHoliday[projOid]));
            }
        }
    }
}

function fListMatch(_list, _matchFunction) {
    var results = [];
    for (var i = 0; i < _list.length; i++) {
        if (_matchFunction(_list[i])) {
            results.push(_list[i]);
        }
    }
    return results;
}


function fAddModMemberRecusive(_rows, _target, _members) {
    fAddModMemberChildrenRecusive(_rows, _target, _members);
}

function fAddModMemberChildrenRecusive(_rows, _target, _members) {
    for (var i = 0; i < _rows.length; i++) {
        if (_rows[i].ToOID == _target) {
            _rows[i].Members = [];
            for (var index = 0; index < _members.length; index++) {
                _rows[i].Members.push(_members[index]);
            }
            return;
        }
        if (_rows[i].Children) {
            fAddModMemberChildrenRecusive(_rows[i].Children, _target, _members);
        }
    }
}

function fTaskStart(_this, proj, idx) {
    RequestData('Common/PromoteObjectTask',
    { Type: _this.getAttribute('data-Type'), Status: _this.getAttribute('data-Status'), OID: idx, RootOID: proj }, function () {
        gPmsModifyHistoryMaster[proj].obj.jqxTreeGrid('pagerRenderer');
    });
}

function fGettingDependData(depends) {
    var returns = {};
    if (depends != null && depends != undefined) {
        var lDepends = [];
        if (depends.indexOf(',') > -1) {
            lDepends = depends.split(',');
        } else {
            lDepends.push(depends);
        }

        for (var index = 0, size = lDepends.length; index < size; index++) {
            if (lDepends[index].indexOf(':') > -1) {
                var splitVal = lDepends[index].split(':');
                returns[splitVal[0]] = parseInt(splitVal[1] != '' ? splitVal[1] : '2');
            } else {
                if (lDepends[index] != '') {
                    returns[lDepends[index]] = 2;
                }
            }
        }
    }
    return returns;
}