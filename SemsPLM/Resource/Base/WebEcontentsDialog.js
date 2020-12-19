function OpenProblemsLibraryCreateDialog(_CallBackFunction, _Wrap, _Param, _Url, _Title) {
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
    var posX = (winWidth / 2) - (1450 / 2) + $(window).scrollLeft();
    var posY = (winHeight / 2) - (900 / 2) + $(window).scrollTop();

    $(popLayer).jqxWindow({
        width: 1450, maxWidth: 1450, height: 900, minHeight: 900, resizable: false, isModal: true, autoOpen: false, modalOpacity: 0.5, showCloseButton: true, position: { x: posX, y: posY },
        initContent: function () {


            $('#CreateProblemsLibraryBtn').on('click', function () {
                var param = {};
                var Issues_Thumbnail = CreateProblemLibIssues;
                var Cause_Thumbnail = CreateProblemLibCause;
                var Countermeasures_Thumbnail = CreateProblemLibCountermeasures;
                param.Oem_Lib_OID = $('#CreateProblemsLibraryOEM').val(); //차종
                param.Car_Lib_OID = $('#CreateProblemsLibraryCAR').val(); //차종
                //param.Product = $('#CreateProblemProduct').val(); //제품 
                //param.Part = $('#CreateProblemPart').val(); //부품
                //param.Occurrence = $('#CreateProblemOccurrence').val(); //발생처
                //param.Stage_Occurrence = $('#CreateProblemStage_Occurrence').val();
                //param.Failure_Type = $('#CreateProblemFailure_Type').val(); //고장유형
                //param.Division = $('#CreateProblemDivision').val(); //구분
                param.Issues = $('#CreateProblemIssues').val();
                param.Issues_Thumbnail = Issues_Thumbnail;
                param.Cause = $('#CreateProblemCause').val();
                param.Cause_Thumbnail = Cause_Thumbnail;
                param.Countermeasures = $('#CreateProblemCountermeasures').val();
                param.Countermeasures_Thumbnail = Countermeasures_Thumbnail;

                //param.Name = $('#CreateProblemName').val(); //번호
                param.Description = $('#CreateProblemDescription').val(); //기타

                $.post('/Econtents/InsProblemsLibrary', param, function (response) {
                    if (response.isError) {
                        alert(response.resultMessage);
                        return;
                    }
                    $(popLayer).jqxWindow('modalDestory');
                }).fail(function (err) {
                    alert(err.responseText);
                });

            });

            $('#CreateDocument_canclebtn').on('click', function () {
                $(popLayer).jqxWindow('modalDestory');
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