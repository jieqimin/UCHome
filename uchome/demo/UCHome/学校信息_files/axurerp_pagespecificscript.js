
var PageName = '学校信息';
var PageId = '08c6bb58527c4727829b7b32e6999af4'
var PageUrl = '学校信息.html'
document.title = '学校信息';
var PageNotes = 
{
"pageName":"学校信息",
"showNotesNames":"False"}
var $OnLoadVariable = '';

var $CSUM;

var hasQuery = false;
var query = window.location.hash.substring(1);
if (query.length > 0) hasQuery = true;
var vars = query.split("&");
for (var i = 0; i < vars.length; i++) {
    var pair = vars[i].split("=");
    if (pair[0].length > 0) eval("$" + pair[0] + " = decodeURIComponent(pair[1]);");
} 

if (hasQuery && $CSUM != 1) {
alert('Prototype Warning: The variable values were too long to pass to this page.\nIf you are using IE, using Firefox will support more data.');
}

function GetQuerystring() {
    return '#OnLoadVariable=' + encodeURIComponent($OnLoadVariable) + '&CSUM=1';
}

function PopulateVariables(value) {
    var d = new Date();
  value = value.replace(/\[\[OnLoadVariable\]\]/g, $OnLoadVariable);
  value = value.replace(/\[\[PageName\]\]/g, PageName);
  value = value.replace(/\[\[GenDay\]\]/g, '1');
  value = value.replace(/\[\[GenMonth\]\]/g, '6');
  value = value.replace(/\[\[GenMonthName\]\]/g, '六月');
  value = value.replace(/\[\[GenDayOfWeek\]\]/g, '星期一');
  value = value.replace(/\[\[GenYear\]\]/g, '2015');
  value = value.replace(/\[\[Day\]\]/g, d.getDate());
  value = value.replace(/\[\[Month\]\]/g, d.getMonth() + 1);
  value = value.replace(/\[\[MonthName\]\]/g, GetMonthString(d.getMonth()));
  value = value.replace(/\[\[DayOfWeek\]\]/g, GetDayString(d.getDay()));
  value = value.replace(/\[\[Year\]\]/g, d.getFullYear());
  return value;
}

function OnLoad(e) {

}

var u122 = document.getElementById('u122');

var u32 = document.getElementById('u32');
gv_vAlignTable['u32'] = 'top';
var u156 = document.getElementById('u156');

var u45 = document.getElementById('u45');
gv_vAlignTable['u45'] = 'top';
var u79 = document.getElementById('u79');
gv_vAlignTable['u79'] = 'top';
var u161 = document.getElementById('u161');
gv_vAlignTable['u161'] = 'center';
var u4 = document.getElementById('u4');
gv_vAlignTable['u4'] = 'top';
var u17 = document.getElementById('u17');

var u135 = document.getElementById('u135');
gv_vAlignTable['u135'] = 'top';
var u42 = document.getElementById('u42');
gv_vAlignTable['u42'] = 'top';
var u159 = document.getElementById('u159');
gv_vAlignTable['u159'] = 'center';
var u102 = document.getElementById('u102');
gv_vAlignTable['u102'] = 'top';
var u55 = document.getElementById('u55');

var u14 = document.getElementById('u14');
gv_vAlignTable['u14'] = 'top';
var u5 = document.getElementById('u5');

var u105 = document.getElementById('u105');
gv_vAlignTable['u105'] = 'center';
var u27 = document.getElementById('u27');
gv_vAlignTable['u27'] = 'top';
var u138 = document.getElementById('u138');
gv_vAlignTable['u138'] = 'top';
var u52 = document.getElementById('u52');

var u65 = document.getElementById('u65');

var u110 = document.getElementById('u110');

var u2 = document.getElementById('u2');

var u24 = document.getElementById('u24');
gv_vAlignTable['u24'] = 'center';
var u58 = document.getElementById('u58');
gv_vAlignTable['u58'] = 'center';
var u150 = document.getElementById('u150');
gv_vAlignTable['u150'] = 'top';
var u108 = document.getElementById('u108');
gv_vAlignTable['u108'] = 'top';
var u37 = document.getElementById('u37');
gv_vAlignTable['u37'] = 'center';
var u30 = document.getElementById('u30');

var u62 = document.getElementById('u62');

var u21 = document.getElementById('u21');

var u75 = document.getElementById('u75');
gv_vAlignTable['u75'] = 'center';
var u133 = document.getElementById('u133');
gv_vAlignTable['u133'] = 'center';
var u88 = document.getElementById('u88');
gv_vAlignTable['u88'] = 'top';
var u7 = document.getElementById('u7');
gv_vAlignTable['u7'] = 'top';
var u47 = document.getElementById('u47');
gv_vAlignTable['u47'] = 'top';
var u72 = document.getElementById('u72');
gv_vAlignTable['u72'] = 'center';
var u112 = document.getElementById('u112');
gv_vAlignTable['u112'] = 'top';
var u154 = document.getElementById('u154');

var u31 = document.getElementById('u31');
gv_vAlignTable['u31'] = 'center';
var u146 = document.getElementById('u146');
gv_vAlignTable['u146'] = 'top';
var u44 = document.getElementById('u44');
gv_vAlignTable['u44'] = 'center';
var u78 = document.getElementById('u78');
gv_vAlignTable['u78'] = 'center';
var u57 = document.getElementById('u57');

var u23 = document.getElementById('u23');

var u8 = document.getElementById('u8');
gv_vAlignTable['u8'] = 'top';
var u16 = document.getElementById('u16');
gv_vAlignTable['u16'] = 'center';
var u125 = document.getElementById('u125');
gv_vAlignTable['u125'] = 'center';
var u151 = document.getElementById('u151');
gv_vAlignTable['u151'] = 'top';
var u6 = document.getElementById('u6');
gv_vAlignTable['u6'] = 'center';
var u41 = document.getElementById('u41');
gv_vAlignTable['u41'] = 'center';
var u149 = document.getElementById('u149');
gv_vAlignTable['u149'] = 'center';
var u119 = document.getElementById('u119');
gv_vAlignTable['u119'] = 'center';
var u54 = document.getElementById('u54');
gv_vAlignTable['u54'] = 'top';
var u131 = document.getElementById('u131');
gv_vAlignTable['u131'] = 'top';
var u26 = document.getElementById('u26');
gv_vAlignTable['u26'] = 'center';
var u128 = document.getElementById('u128');

var u94 = document.getElementById('u94');
gv_vAlignTable['u94'] = 'top';
var u99 = document.getElementById('u99');
gv_vAlignTable['u99'] = 'center';
var u10 = document.getElementById('u10');

var u51 = document.getElementById('u51');
gv_vAlignTable['u51'] = 'top';
var u77 = document.getElementById('u77');

var u144 = document.getElementById('u144');

var u82 = document.getElementById('u82');
gv_vAlignTable['u82'] = 'top';
var u36 = document.getElementById('u36');

var u101 = document.getElementById('u101');
gv_vAlignTable['u101'] = 'center';
var u95 = document.getElementById('u95');
gv_vAlignTable['u95'] = 'top';
var u61 = document.getElementById('u61');
gv_vAlignTable['u61'] = 'top';
var u143 = document.getElementById('u143');
gv_vAlignTable['u143'] = 'top';
var u74 = document.getElementById('u74');

var u114 = document.getElementById('u114');

var u20 = document.getElementById('u20');
gv_vAlignTable['u20'] = 'center';
var u33 = document.getElementById('u33');

var u157 = document.getElementById('u157');
gv_vAlignTable['u157'] = 'center';
var u104 = document.getElementById('u104');

var u92 = document.getElementById('u92');

var u46 = document.getElementById('u46');
gv_vAlignTable['u46'] = 'top';
var u34 = document.getElementById('u34');
gv_vAlignTable['u34'] = 'center';
var u71 = document.getElementById('u71');

var u98 = document.getElementById('u98');

var u127 = document.getElementById('u127');
gv_vAlignTable['u127'] = 'center';
var u43 = document.getElementById('u43');

var u160 = document.getElementById('u160');

var u56 = document.getElementById('u56');
gv_vAlignTable['u56'] = 'center';
var u142 = document.getElementById('u142');
gv_vAlignTable['u142'] = 'top';
var u106 = document.getElementById('u106');

var u53 = document.getElementById('u53');
gv_vAlignTable['u53'] = 'center';
var u40 = document.getElementById('u40');

var u139 = document.getElementById('u139');
gv_vAlignTable['u139'] = 'top';
var u87 = document.getElementById('u87');
gv_vAlignTable['u87'] = 'center';
var u38 = document.getElementById('u38');

var u109 = document.getElementById('u109');
gv_vAlignTable['u109'] = 'top';
var u121 = document.getElementById('u121');
gv_vAlignTable['u121'] = 'top';
var u67 = document.getElementById('u67');
gv_vAlignTable['u67'] = 'top';
var u116 = document.getElementById('u116');
gv_vAlignTable['u116'] = 'top';
var u155 = document.getElementById('u155');
gv_vAlignTable['u155'] = 'center';
var u9 = document.getElementById('u9');
gv_vAlignTable['u9'] = 'top';
var u84 = document.getElementById('u84');
gv_vAlignTable['u84'] = 'center';
var u50 = document.getElementById('u50');
gv_vAlignTable['u50'] = 'center';
var u97 = document.getElementById('u97');
gv_vAlignTable['u97'] = 'center';
var u63 = document.getElementById('u63');
gv_vAlignTable['u63'] = 'center';
var u124 = document.getElementById('u124');

var u130 = document.getElementById('u130');
gv_vAlignTable['u130'] = 'top';
var u76 = document.getElementById('u76');
gv_vAlignTable['u76'] = 'top';
var u134 = document.getElementById('u134');
gv_vAlignTable['u134'] = 'top';
var u81 = document.getElementById('u81');
gv_vAlignTable['u81'] = 'center';
var u118 = document.getElementById('u118');

var u3 = document.getElementById('u3');
gv_vAlignTable['u3'] = 'center';
var u60 = document.getElementById('u60');
gv_vAlignTable['u60'] = 'center';
var u64 = document.getElementById('u64');
gv_vAlignTable['u64'] = 'top';
var u73 = document.getElementById('u73');
gv_vAlignTable['u73'] = 'top';
var u113 = document.getElementById('u113');
gv_vAlignTable['u113'] = 'top';
var u147 = document.getElementById('u147');
gv_vAlignTable['u147'] = 'top';
var u91 = document.getElementById('u91');
gv_vAlignTable['u91'] = 'top';
var u19 = document.getElementById('u19');

var u70 = document.getElementById('u70');
gv_vAlignTable['u70'] = 'top';
var u140 = document.getElementById('u140');

var u117 = document.getElementById('u117');
gv_vAlignTable['u117'] = 'top';
var u141 = document.getElementById('u141');
gv_vAlignTable['u141'] = 'center';
var u89 = document.getElementById('u89');

var u68 = document.getElementById('u68');

var u136 = document.getElementById('u136');

var u29 = document.getElementById('u29');
gv_vAlignTable['u29'] = 'center';
var u152 = document.getElementById('u152');

var u132 = document.getElementById('u132');

var u103 = document.getElementById('u103');
gv_vAlignTable['u103'] = 'top';
var u129 = document.getElementById('u129');
gv_vAlignTable['u129'] = 'center';
var u86 = document.getElementById('u86');

var u11 = document.getElementById('u11');
gv_vAlignTable['u11'] = 'center';
var u39 = document.getElementById('u39');
gv_vAlignTable['u39'] = 'center';
var u111 = document.getElementById('u111');
gv_vAlignTable['u111'] = 'center';
var u66 = document.getElementById('u66');
gv_vAlignTable['u66'] = 'center';
var u0 = document.getElementById('u0');

var u145 = document.getElementById('u145');
gv_vAlignTable['u145'] = 'center';
var u153 = document.getElementById('u153');
gv_vAlignTable['u153'] = 'center';
var u25 = document.getElementById('u25');

var u83 = document.getElementById('u83');

var u96 = document.getElementById('u96');

var u15 = document.getElementById('u15');

var u49 = document.getElementById('u49');

var u120 = document.getElementById('u120');
gv_vAlignTable['u120'] = 'top';
var u80 = document.getElementById('u80');

var u1 = document.getElementById('u1');
gv_vAlignTable['u1'] = 'center';
var u85 = document.getElementById('u85');
gv_vAlignTable['u85'] = 'top';
var u148 = document.getElementById('u148');

var u93 = document.getElementById('u93');
gv_vAlignTable['u93'] = 'center';
var u12 = document.getElementById('u12');

var u115 = document.getElementById('u115');
gv_vAlignTable['u115'] = 'center';
var u59 = document.getElementById('u59');

var u137 = document.getElementById('u137');
gv_vAlignTable['u137'] = 'center';
var u90 = document.getElementById('u90');
gv_vAlignTable['u90'] = 'center';
var u18 = document.getElementById('u18');
gv_vAlignTable['u18'] = 'center';
var u158 = document.getElementById('u158');

var u100 = document.getElementById('u100');

var u123 = document.getElementById('u123');
gv_vAlignTable['u123'] = 'center';
var u48 = document.getElementById('u48');
gv_vAlignTable['u48'] = 'top';
var u22 = document.getElementById('u22');
gv_vAlignTable['u22'] = 'center';
var u13 = document.getElementById('u13');
gv_vAlignTable['u13'] = 'center';
var u107 = document.getElementById('u107');
gv_vAlignTable['u107'] = 'center';
var u35 = document.getElementById('u35');
gv_vAlignTable['u35'] = 'top';
var u69 = document.getElementById('u69');
gv_vAlignTable['u69'] = 'center';
var u28 = document.getElementById('u28');

var u126 = document.getElementById('u126');

if (window.OnLoad) OnLoad();
