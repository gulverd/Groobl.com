(function () {

    var mainController = function ($scope, dataService) {

        var vm = this;

       

        var emptyCategory = {
            id: 0,
            name: "",
            parent: {name: "Has No Parent", id:0}
        }
        vm.treeList = [];
        vm.selectedCategory = {}

        angular.copy(emptyCategory, vm.selectedCategory);

        dataService.getTree().then(function (d) {
            vm.treeList = d;
        });

        vm.openMenu = function (item) {

            $(".dropdown_menu_p").height(0);

            var div = $(".dropdown_menu_" + item.id);

            if (div.height() == 0)
                div.height(49);
        }


        vm.delete = function (data) {
            dataService.removeChild(data.id);
            data.name=""
        };

        vm._parentItemTemp = {};
        vm.addChildCategory = function (data) {

            angular.copy(emptyCategory, vm.selectedCategory);
            vm.selectedCategory.parent = data;
            vm._parentItemTemp = data;
        }
        vm.clear = function () {
            angular.copy(emptyCategory, vm.selectedCategory);
            vm._parentItemTemp = {}
        }

        vm.add = function (data) {

            dataService.addChild({ name: vm.selectedCategory.name, parentId: vm.selectedCategory.parent.id })
                .then(function (item) {

                    if (item.parentId == 0)
                        vm.treeList.push(item);
                    else
                        vm._parentItemTemp.children.push(item);

                    vm.clear();

                });
        };

            $('#tag_it').tagit({
                availableTags: sampleTags,
                // This will make Tag-it submit a single form value, as a comma-delimited field.
                singleField: true,
                singleFieldNode: $('#tag_it'),
                placeholderText: "#add your interest"
            });
        
    };

    angular.module('app')
        .controller('homeController', ['$scope', 'dataService', mainController]);

})();