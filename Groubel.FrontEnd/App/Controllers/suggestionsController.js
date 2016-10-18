(function () {

    var suggestionsController = function ($scope, $state, suggestionsService) {

        var vm = this;

        vm.tab = 1;
        vm.setTab = function (st) {
            vm.tab = st;
        }


        vm.getlogo = function (data) {
            return data.replace("~", "");
        }

        vm.suggestions = {};

        suggestionsService.getSuggestions().then(function (d) {
            vm.suggestions = d;
        });

        vm.userSteps = 0;
        vm.userNav = function (step) {

            if (vm.userSteps == 0 && step == -1)
                return;

            vm.userSteps = vm.userSteps + step;
            suggestionsService.getSuggestedUsers(vm.userSteps*4).then(function (d) {
                vm.suggestions["1"] = d;
            });
        }

        vm.roomSteps = 0;
        vm.roomNav = function (step) {

            if (vm.roomSteps == 0 && step == -1)
                return;

            vm.roomSteps = vm.roomSteps + step;
            suggestionsService.getSuggestedRooms(vm.roomSteps * 2).then(function (d) {
                vm.suggestions["2"] = d;
            });
        }

        vm.interestSteps = 0;
        vm.interestNav = function (step) {

            if (vm.interestSteps == 0 && step == -1)
                return;

            vm.interestSteps = vm.interestSteps + step;
            suggestionsService.getSuggestedInterests(vm.interestSteps * 3).then(function (d) {
                vm.suggestions["3"] = d;
            });
        }

        
    };

    angular.module('app')
       .controller('suggestionsController', ['$scope', '$state', 'suggestionsService', suggestionsController]);
})();