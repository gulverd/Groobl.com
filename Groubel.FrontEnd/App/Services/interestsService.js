(function () {

    var interestsService = function ($http) {

        var getFilterInterest = function (id) {

            var data = $http.get("/Interests/GetFilterInterest?userId=" + _userId + "&id=" + id).then(function (d) {
                return d.data;
            });

            return data;

        }

        var addToMyInterest = function (id) {

            var data = $http.get("/Interests/AddToMyInterest?userId=" + _userId + "&id=" + id);

        }

        var getInterestsByName = function (name) {

            var data = $http.get("/Interests/GetInterestsByName?name=" + name).then(function (d) {
                return d.data;
            });

            return data;

        }

        return {

            getFilterInterest: getFilterInterest,
            addToMyInterest: addToMyInterest,
            getInterestsByName: getInterestsByName
        };
    };

    angular.module('app')
        .factory('interestsService', interestsService);
})();