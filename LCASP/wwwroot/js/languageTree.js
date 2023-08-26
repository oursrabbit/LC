var languageList = ["中文", "英文", "俄文", "日文"];

function createLanguageList(parent, placeholder) {
    var html = `
<div class="input-group">
    <input class="dropdown-toggle" type="text" data-bs-toggle="dropdown" data-bs-auto-close="false" aria-expanded="true" placeholder="${placeholder}..." id="lang_input"/>
    <div class="dropdown-menu" id="lang_list">
        <div class="container">
            <ul class="list-group">`;
    languageList.forEach(function (lang, index) {
        html += `
                <li class="list-group-item">
                    <input class="form-check-input me-1" type="checkbox" value="" id="lang_${index}"/>
                    <label class="form-check-label stretched-link" for="lang_${index}">${lang}</label>
                </li>`;
    });
       html += `
            </ul >
        </div>
        <div class="container" style="margin-top: 20px">
            <button class="btn btn-primary" onclick="languageConfirm()">确认</button>
            <button class="btn btn-danger" onclick="languageCancel()">取消</button>
        </div>
    </div>
</div>`;
    parent.html(html);
}

function languageConfirm() {
    var text = $("#lang_input").val();

    languageList.forEach(function (lang, index) {
        if ($(`#lang_${index}`).prop("checked") == true) {
            if (!text.includes(lang))
                text = lang + " " + text;
        }
        else {
            if (text.includes(lang)) {
                text = text.replace(lang, "");
                text = text.replace("  ", " ");
            }
        }
    });

    $("#lang_input").val(text)
    $("#lang_input").dropdown('toggle');
}

function languageCancel() {
    $("#lang_input").dropdown('toggle');
}


