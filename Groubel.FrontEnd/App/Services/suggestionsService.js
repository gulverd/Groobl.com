(function () {

    var suggestionsService = function ($http) {

        var getSuggestions = function () {

            var data = $http.get("/Suggestion/GetSuggestions?userId=" + _userId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var getSuggestedUsers = function (ind) {

            var data = $http.get("/Suggestion/GetSuggestedUsers?userId=" + _userId+"&ind="+ind).then(function (d) {
                return d.data;
            });

            return data;

        }

        var getSuggestedRooms = function (ind) {

            var data = $http.get("/Suggestion/GetSuggestedRooms?userId=" + _userId + "&ind=" + ind).then(function (d) {
                return d.data;
            });

            return data;

        }

        var getSuggestedInterests = function (ind) {

            var data = $http.get("/Suggestion/GetSuggestedInterests?userId=" + _userId + "&ind=" + ind).then(function (d) {
                return d.data;
            });

            return data;

        }

        return {

            getSuggestions: getSuggestions,
            getSuggestedUsers: getSuggestedUsers,
            getSuggestedRooms: getSuggestedRooms,
            getSuggestedInterests: getSuggestedInterests
        };
    };

    angular.module('app')
        .factory('suggestionsService', suggestionsService);
})();