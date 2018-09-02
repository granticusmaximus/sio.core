﻿modules.component('mainSideBarDynamic', {
    templateUrl: '/app-shared/components/main-side-bar-dynamic/main-side-bar-dynamic.html',
    controller: ['$rootScope', '$scope', 'RoleServices', 'TranslatorService', function ($rootScope, $scope, roleServices, translatorService) {
        var ctrl = this;
        ctrl.init = function () {
            if (ctrl.roles) {
                ctrl.role = ctrl.roles[0];
            }    
        };
    }],
    bindings: {
        roles: '=',
        activedRole: '=',
        translate: '&'
    }
});