(function () {

    var friendsController = function ($scope, $state, friendsService, roomsService) {

        var vm = this;

        vm.friends = []

        friendsService.getFriends(_userId).then(function (d) {
            vm.friends = d;
            $(".fr_list").niceScroll();
        });

        vm.data = [];
        roomsService.getUserRooms(_userId, true).then(function (d) {
            vm.data = d;
        });

        vm.searchModel = "";
        vm.searchTimeOut = 0;
        vm.search = function () {

            clearTimeout(vm.searchTimeOut);

            vm.searchTimeOut = setTimeout(function () {

                friendsService.getFriendsByName(_userId, vm.searchModel).then(function (d) {
                    vm.friends = d;
                });

            });
        }

        vm.opened = false;


        vm.getlogo = function (data) {
            return data.replace("~", "");
        }


    };

    angular.module('app')
        .controller('friendsController', ['$scope', '$state', 'friendsService','roomsService', friendsController]);

})();