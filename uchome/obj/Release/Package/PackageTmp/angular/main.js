'use strict';
(function (win) {
    require.config({
        baseUrl: '/uchome',
        // 依赖相对路径
        paths: {
            'angular': 'plugs/AngularJs/angular.min',
            'angularmodule': 'angular/angularmodule',
            'formvalid':'angular/uchome/demo/formvalid'
        },
        // 引入没有使用requirejs模块写法的类库
        shim: {
            'angularmodule': {
                // angular-route依赖angular
                deps: ['angular','formvalid']
            }
        }
    });
    alert(0);
    // 自动导入router.js模块，由于后缀名可以省略，故写作'router',
    // 并将模块返回的结果赋予到router中。
    require(['angularmodule'], function () {
        // 手动启动angularjs，特别说明此处的bootstrap不是那个ui框架，
        // 而是angularjs的一个手动启动框架的函数
        angular.bootstrap(document, ['appCenter']);
    });
    alert(1);
})(window);
