var keywordList = ["关键词1", "关键词2", "关键词3"];

function createKeywordList(parent, placeholder) {
    var html = `
<div class="input-group">
    <input class="dropdown-toggle" type="text" data-bs-toggle="dropdown" data-bs-auto-close="false" aria-expanded="true" placeholder="${placeholder}..." id="kw_input"/>
    <div class="dropdown-menu" id="kw_list">
        <div class="container">
            <ul class="list-group">`;
    keywordList.forEach(function (kw, index) {
        html += `
                <li class="list-group-item">
                    <input class="form-check-input me-1" type="checkbox" value="" id="kw_${index}"/>
                    <label class="form-check-label stretched-link" for="kw_${index}">${kw}</label>
                </li>`;
    });
    html += `
            </ul >
        </div>
        <div class="container" style="margin-top: 20px">
            <button class="btn btn-primary" onclick="kwConfirm()">确认</button>
            <button class="btn btn-danger" onclick="kwCancel()">取消</button>
        </div>
    </div>
</div>`;
    parent.html(html);
}

function kwConfirm() {
    var text = $("#kw_input").val();
    keywordList.forEach(function (kw, index) {
        if ($(`#kw_${index}`).prop("checked") == true) {
            if (!text.includes(kw))
                text = kw + " " + text;
        }
        else {
            if (text.includes(kw)) {
                text = text.replace(kw, "");
                text = text.replace("  ", " ");
            }
        }
    });
    $("#kw_input").val(text);
    $("#kw_input").dropdown('toggle');
}

function kwCancel() {
    $("#kw_input").dropdown('toggle');
}


