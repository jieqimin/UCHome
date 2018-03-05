function returngrade(schstage, curyear, createyear) {
    var stagecn;
    switch (schstage) {
        case 1:
            stagecn = "学前";
            break;
        case 2:
            stagecn = "小学";
            break;
        case 3:
            stagecn = "初中";
            break;
        case 4:
            stagecn = "高中";
            break;
        default:
            stagecn = "其它";
            break;
    }
    switch (curyear - createyear) {
        case 0:
            return stagecn + "一年级";
        case 1:
            return stagecn + "二年级";
        case 2:
            return stagecn + "三年级";
        case 3:
            return stagecn + "四年级";
        case 4:
            return stagecn + "五年级";
        case 5:
            return stagecn + "六年级";
        default:
            return "其它年级";
    }
}