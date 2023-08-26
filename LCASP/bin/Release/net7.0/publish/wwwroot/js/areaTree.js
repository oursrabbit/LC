var areaList = [[]];
areaList[0] = ["装备战略规划", "发展战略与规划计划", "国防与装备经济性", "国防工业体系级能力", "装备领域军民融合", "供应链"];
areaList[1] = ["装备建设发展", "陆上装备", "海上装备", "空中装备", "航天装备", "核领域装备", "导弹装备", "网络信息体系装备", "网络空间装备", "后勤装备和通用装备", "武警装备", "军用动力装备", "安全保密装备"];
areaList[2] = ["装备前沿技术", "人工智能", "无人系统", "大数据", "生物交叉", "先进计算", "先进材料", "先进制造", "高超声速", "定向能", "深海极地"];
areaList[3] = ["装备现代治理", "装备采办", "政策法规", "试验鉴定", "军备控制", "装备技术合作", "装备保障", "重大工程管理", "合同监管"];
areaList[4] = ["装备部署运用", "蓝军组织机构与指挥决策", "作战构想与战法运用", "装备体系与作战能力", "标准规范与方法手段"];
areaList[5] = ["军情要点、时政要闻"];
areaList[6] = ["纪录片"];
areaList[7] = ["宣传片"];
areaList[8] = ["国际军贸"];
areaList[9] = ["阅兵"];
areaList[10] = ["人物"];
areaList[11] = ["演习"];

function createAreaList(parent, placeholder) {
    var html = `
<div class="input-group">
    <input class="dropdown-toggle" type="text" data-bs-toggle="dropdown" data-bs-auto-close="false" aria-expanded="true" placeholder="${placeholder}..." id="area_input"/>
    <div class="dropdown-menu" id="area_list">
        <div class="container">`;
    areaList.forEach(function (area, index) {
        html += `<ul class="list-group list-group-horizontal">`;
        area.forEach(function (areadirection, adindex) {
            html += `
            <li class="list-group-item">
                <input class="form-check-input me-1" type="checkbox" value="" id="area_${index}_${adindex}" />
                <label class="form-check-label" for="area_${index}_${adindex}">${areadirection}</label >
            </li>`;
        });
        html += `</ul>`;
    });
    html += `
        </div>
        <div class="container" style="margin-top: 20px">
            <button class="btn btn-primary" onclick="areaConfirm()">确认</button>
            <button class="btn btn-danger" onclick="areaCancel()">取消</button>
        </div>
    </div>
</div>`;
    parent.html(html);
}

function areaConfirm() {
    var text = $("#area_input").val();
    areaList.forEach(function (area, index) {
        area.forEach(function (areadirection, adindex) {
            // add to text
            if ($(`#area_${index}_${adindex}`).prop("checked") == true) {
                if (!text.includes(areadirection))
                    text = areadirection + " " + text;
            }
            // remove from text
            else
            {
                if (text.includes(areadirection)) {
                    text = text.replace(areadirection, "");
                    text = text.replace("  ", " ");
                }
            }
        });
    });
    $("#area_input").val(text);
    $("#area_input").dropdown('toggle');
}

function areaCancel() {
    $("#area_input").dropdown('toggle');
}


