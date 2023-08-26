const options = [
    {
        name: '装备战略规划',
        value: 1000,
        children: [
            {
                name: '发展战略与规划计划',
                value: 1010,
                children: []
            },
            {
                name: '国防与装备经济性',
                value: 1011,
                children: []
            },
            {
                name: '国防工业体系级能力',
                value: 1012,
                children: []
            },
            {
                name: '装备领域军民融合',
                value: 1013,
                children: []
            },
            {
                name: '供应链',
                value: 1014,
                children: []
            }
        ]
    },
    {
        name: '装备建设发展',
        value: 1100,
        children: [
            {
                name: '陆上装备',
                value: 1110,
                children: []
            },
            {
                name: '海上装备',
                value: 1111,
                children: []
            },
            {
                name: '空中装备',
                value: 1112,
                children: []
            },
            {
                name: '航天装备',
                value: 1113,
                children: []
            },
            {
                name: '核领域装备',
                value: 1114,
                children: []
            },
            {
                name: '导弹装备',
                value: 1115,
                children: []
            },
            {
                name: '网络信息体系装备',
                value: 1116,
                children: []
            },
            {
                name: '网络空间装备',
                value: 1117,
                children: []
            },
            {
                name: '后勤装备和通用装备',
                value: 1118,
                children: []
            },
            {
                name: '武警装备',
                value: 1119,
                children: []
            },
            {
                name: '军用动力装备',
                value: 1120,
                children: []
            },
            {
                name: '安全保密装备',
                value: 1121,
                children: []
            }
        ]
    },
    {
        name: '装备前沿技术',
        value: 1200,
        children: [
            {
                name: '人工智能',
                value: 1210,
                children: []
            },
            {
                name: '无人系统',
                value: 1211,
                children: []
            },
            {
                name: '大数据',
                value: 1212,
                children: []
            },
            {
                name: '生物交叉',
                value: 1213,
                children: []
            },
            {
                name: '先进计算',
                value: 1214,
                children: []
            },
            {
                name: '先进材料',
                value: 1215,
                children: []
            },
            {
                name: '先进制造',
                value: 1216,
                children: []
            },
            {
                name: '高超声速',
                value: 1217,
                children: []
            },
            {
                name: '定向能',
                value: 1218,
                children: []
            },
            {
                name: '深海极地',
                value: 1219,
                children: []
            }
        ]
    },
    {
        name: '装备现代治理',
        value: 1300,
        children: [
            {
                name: '装备采办',
                value: 1310,
                children: []
            },
            {
                name: '政策法规',
                value: 1311,
                children: []
            },
            {
                name: '试验鉴定',
                value: 1312,
                children: []
            },
            {
                name: '军备控制',
                value: 1313,
                children: []
            },
            {
                name: '装备技术合作',
                value: 1314,
                children: []
            },
            {
                name: '装备保障',
                value: 1315,
                children: []
            },
            {
                name: '重大工程管理',
                value: 1316,
                children: []
            },
            {
                name: '合同监管',
                value: 1317,
                children: []
            }
        ]
    },
    {
        name: '装备部署运用',
        value: 1400,
        children: [
            {
                name: '蓝军组织机构与指挥决策',
                value: 1410,
                children: []
            },
            {
                name: '作战构想与战法运用',
                value: 1411,
                children: []
            },
            {
                name: '装备体系与作战能力',
                value: 1412,
                children: []
            },
            {
                name: '标准规范与方法手段',
                value: 1413,
                children: []
            }
        ]
    },
    {
        name: '军情要点、时政要闻',
        value: 1500,
        children: []
    },
    {
        name: '纪录片',
        value: 1600,
        children: []
    },
    {
        name: '宣传片',
        value: 1700,
        children: []
    },
    {
        name: '国际军贸',
        value: 1800,
        children: []
    },
    {
        name: '阅兵',
        value: 1900,
        children: []
    },
    {
        name: '人物',
        value: 2000,
        children: []
    },
    {
        name: '演习',
        value: 2100,
        children: []
    }
]

const domElement = document.querySelector('.treeselect-area')

const treeselect = new Treeselect({
    parentHtmlContainer: domElement,
    value: [],
    options: options,
})

const inputhidden = document.querySelector('.treeselect-area-hidden');
inputhidden.value = treeselect.value;

treeselect.srcElement.addEventListener('input', (e) => {
    const inputhidden = document.querySelector('.treeselect-area-hidden');
    inputhidden.value = treeselect.value;
    console.log('Selected value:', e.detail)
})


const domElement_detail = document.querySelector('.detail_mediaarea_tree')

const treeselect_detail = new Treeselect({
    parentHtmlContainer: domElement_detail,
    value: [1900],
    options: options,
})

const inputhidden_detail = document.querySelector('.treeselect-area-hidden-detail');
inputhidden_detail.value = treeselect_detail.value;

treeselect_detail.srcElement.addEventListener('input', (e) => {
    const inputhidden_detail = document.querySelector('.treeselect-area-hidden-detail');
    inputhidden_detail.value = treeselect_detail.value;
    console.log('Selected value:', e.detail)
})
