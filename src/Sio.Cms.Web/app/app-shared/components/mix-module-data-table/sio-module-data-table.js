modules.component('sioModuleDataTable', {
    templateUrl: '/app/app-shared/components/sio-module-data-table/sio-module-data-table.html',
    controller: ['$rootScope', '$scope', '$location', 'ngAppSettings', function ($rootScope, $scope, $location, ngAppSettings) {
        var ctrl = this;
        ctrl.colWidth = 3;
        ctrl.init = function () {
            if(ctrl.data.items.length)
            {
                ctrl.min = ctrl.data.items[0].priority;
                if(!ctrl.min){
                    ctrl.min=0;
                }
            }
            ctrl.colWidth = parseInt(9 / ctrl.columns.length);
            ctrl.lastColWidth = (9 % ctrl.columns.length) > 0 ? 2 : 1;
        };
        ctrl.translate = $rootScope.translate;
        ctrl.selected = null;
        ctrl.updateOrders = function (index, items) {
            items.splice(index, 1);
            for (var i = 0; i < items.length; i++) {
                items[i].priority = ctrl.min + i;
            }
            ctrl.onUpdateInfos({ items: items });
        };
        
        ctrl.dragoverCallback = function (index, item, external, type) {
            //console.log('drop ', index, item, external, type);
        }
        ctrl.insertCallback = function (index, item, external, type) {
            //console.log('insert ', index, item, external, type);
        }
        ctrl.delete = function (id) {
            ctrl.onDelete({ id: id,moduleId: ctrl.moduleId });
        };
        ctrl.goTo = function (id) {
            $location.path(ctrl.editUrl + '/' + id);
        }
        ctrl.toggleChildNavs = function(nav){
            nav.showChildNavs = nav.childNavs.length>0 && !nav.showChildNavs;
        }
        ctrl.view = function (moduleId, contentId) {
            var obj = {
                moduleId: moduleId,
                id: contentId
            };
            $rootScope.preview('module-data', obj, null, 'modal-lg');
        }
    }],
    bindings: {
        moduleId: '=',
        data: '=',
        childName: '=',
        canDrag: '=',
        editUrl: '=',
        columns: '=',
        onDelete: '&',
        onUpdateInfos: '&',
        onUpdateChildInfos: '&',
    }
});
