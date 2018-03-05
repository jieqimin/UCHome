"use strict"
tempApp
.constant("Modules_Config", [
  //{
  //    name: "ngTable",
  //    module: true,
  //    files: [
  //      "Scripts/ng-table/dist/ng-table.min.css",
  //      "Scripts/ng-table/dist/ng-table.min.js"
  //    ]
  //}
])
.config(["$ocLazyLoadProvider", "Modules_Config", routeFn]);
function routeFn($ocLazyLoadProvider, Modules_Config) {
    $ocLazyLoadProvider.config({
        debug: false,
        events: false,
        modules: Modules_Config
    });
};