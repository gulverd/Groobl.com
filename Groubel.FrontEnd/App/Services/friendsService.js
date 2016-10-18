(function () {

    var friendsService = function ($http) {

        var addFriend = function (firstuserId) {

            var data = $http.get("/Friends/AddFriend?firstuserId=" + firstuserId + "&secondUserId=" + _userId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var removeFriend = function (firstuserId) {

            var data = $http.get("/Friends/RemoveFriend?firstuserId=" + firstuserId + "&secondUserId=" + _userId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var submitFriendship = function (firstuserId) {

            var data = $http.get("/Friends/SubmitFriendship?firstuserId=" + firstuserId + "&secondUserId=" + _userId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var getFriends = function (userId) {

            var data = $http.get("/Friends/GetFriends?userId=" + userId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var getFriendsByName = function (userId, name) {

            var data = $http.get("/Friends/GetFriendsByName?userId=" + userId + "&name=" + name).then(function (d) {
                return d.data;
            });

            return data;

        }

        return {

            addFriend: addFriend,
            removeFriend: removeFriend,
            submitFriendship: submitFriendship,
            getFriends: getFriends,
            getFriendsByName: getFriendsByName

        };
    };

    angular.module('app')
        .factory('friendsService', friendsService);
})();