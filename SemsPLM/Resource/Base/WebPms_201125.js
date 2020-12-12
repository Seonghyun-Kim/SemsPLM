var gTempPicker;
var gDeaultWorkDay = 1;
var gClickDuplication = false;
var gPmsModifyHistoryMaster = {};
var gPmsModifyHistoryHoliday = {};

$(document).on('focus', '.datePicker', function () {
    const projId = $(this).attr('data-proj-id');
    const working = $(this).attr('data-proj-workingday');
    const holiday = gPmsModifyHistoryHoliday[projId];

    gTempPicker = $(".datePicker").datepicker({
        dateFormat: 'yy-mm-dd',
        dayNames: ['일', '월', '화', '수', '목', '금', '토'],
        dayNamesMin: ['일', '월', '화', '수', '목', '금', '토'],
        dayNamesShort: ['일', '월', '화', '수', '목', '금', '토'],
        monthNames: ['1월', '2월', '3월', '4월', '5월', '6월', '7월', '8월', '9월', '10월', '11월', '12월'],
        monthNamesShort: ['1월', '2월', '3월', '4월', '5월', '6월', '7월', '8월', '9월', '10월', '11월', '12월'],
        yearSuffix: '년',
        showMonthAfterYear: true,
        changeYear: true,
        changeMonth: true,
        beforeShowDay: function (date) {
            var day = date.getDay();
            if (working == 6) {
                return [(day != 0 && holiday.indexOf('#' + moment(date).format('YYYY-MM-DD') + '#') < 0)];
            } else if (working == 5) {
                return [(day != 0 && day != 6 && holiday.indexOf('#' + moment(date).format('YYYY-MM-DD') + '#') < 0)];
            }
        },
        onSelect: function (dateText, inst) {
            $('.ui-datepicker').remove();
            $('#loading').css('display', 'block');
            setTimeout(function () {
                const args = gPmsModifyHistoryMaster[projId].event.args;
                const row = args.row;
                const records = row.records;
                const dataField = args.dataField;

                if (dataField == 'EstStartDt') {
                    const parentRowKey = gPmsModifyHistoryMaster[projId].obj.jqxTreeGrid('getKey', row);
                    if (projId == parentRowKey) {
                        gPmsModifyHistoryMaster[projId].data[0]['EstStartDt'] = dateText;
                    }
                    if (records != undefined && records.length > 0) {
                        for (var i = 0; i < records.length; i++) {
                            var rowKey = gPmsModifyHistoryMaster[projId].obj.jqxTreeGrid('getKey', records[i]);
                            var tmpRowData = gPmsModifyHistoryMaster[projId].obj.jqxTreeGrid('getRow', rowKey);
                            if ((new Date(tmpRowData.EstStartDt).getTime()) < (new Date(dateText).getTime())) {
                                gPmsModifyHistoryMaster[projId].obj.jqxTreeGrid('setCellValue', rowKey, 'EstStartDt', dateText);
                            }
                        }
                    }
                } else if (dataField == 'EstEndDt') {
                    gPmsModifyHistoryMaster[projId].obj.jqxTreeGrid('setCellValue', args.key, 'EstDuration', fWeekendCalc(row.EstStartDt, dateText, working, holiday));
                }
                gPmsModifyHistoryMaster[projId].obj.jqxTreeGrid('updateBoundData');
                fDependencyControl(projId);

                 /*
                   const maxEstEndDt = moment(fMaxDate(fPmsLoadProperties(gPmsModifyHistoryMaster[projId].obj.jqxTreeGrid('getRows'), 'EstEndDt'))).format('YYYY-MM-DD');
                   gPmsModifyHistoryMaster[projId].data[0]['EstEndDt'] = maxEstEndDt;
                   gPmsModifyHistoryMaster[projId].data[0]['EstDuration'] = fWeekendCalc(gPmsModifyHistoryMaster[projId].data[0]['EstStartDt'], maxEstEndDt, working, holiday);
                   gPmsModifyHistoryMaster[projId].obj.jqxTreeGrid('updateBoundData');
                 */

                $('#loading').css('display', 'none');
            }, 100);
           
            
        }
    });
});