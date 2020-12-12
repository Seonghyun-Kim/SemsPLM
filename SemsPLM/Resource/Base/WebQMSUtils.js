function ContentEditMode(v) {
    var AttrType = $(v).attr("AttrType");
    if (AttrType === "TEXT" || AttrType === "TEXTAREA") {
        $(v).removeAttr('readonly');
    } else if (AttrType === "COMBO") {
        $(v).jqxComboBox({ disabled: false });
    } else if (AttrType === "DATE") {
        $(v).jqxDateTimeInput({ disabled: false });
    } else if (AttrType === "CHECKBOX") {
        $(v).jqxCheckBox({ disabled: false });
    } else if (AttrType === "BUTTON") {
        $(v).jqxButton({ disabled: false });
    } else if (AttrType === "SWITCH") {
        $(v).jqxSwitchButton({ disabled: false });
    }           
}

function ContentReadMode(v) {
    var AttrType = $(v).attr("AttrType");
    if (AttrType === "TEXT" || AttrType === "TEXTAREA") {
        $(v).attr('readonly', 'readonly');
    } else if (AttrType === "COMBO") {
        $(v).jqxComboBox({ disabled: true });
    } else if (AttrType === "DATE") {
        $(v).jqxDateTimeInput({ disabled: true });
    } else if (AttrType === "CHECKBOX") {
        $(v).jqxCheckBox({ disabled: true });
    } else if (AttrType === "BUTTON") {
        $(v).jqxButton({ disabled: true });
    } else if (AttrType === "SWITCH") {
        $(v).jqxSwitchButton({ disabled: true });
    }     
}

