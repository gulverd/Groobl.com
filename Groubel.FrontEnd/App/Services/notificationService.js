(function () {

    var notificationService = function ($http) {

        var getUserNotificationsCount = function () {

            var data = $http.get("/Notification/GetUserNotificationsCount?userId=" + _userId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var addNotification = function (friendId) {

            var data = $http.get("/Notification/AddNotification?userId=" + _userId +"&friendId="+friendId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var getUserNotifications = function (lastId, nextId) {

            var data = $http.get("/Notification/GetUserNotifications?userId=" + _userId + "&lastId=" + lastId + "&nextId=" + nextId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var getUserActivity = function (userId, lastId) {

            var data = $http.get("/Notification/GetUserActivity?userId=" + userId + "&lastId=" + lastId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var setApproved = function (id) {
            
            var data = $http.get("/Notification/SetApproved?id=" + id ).then(function (d) {
                return d.data;
            });

            return data;

        }

        return {

            getUserNotificationsCount: getUserNotificationsCount,
            getUserNotifications: getUserNotifications,
            getUserActivity: getUserActivity,
            setApproved: setApproved,
            addNotification:addNotification
        };

        
    };

    angular.module('app')
        .factory('notificationService', notificationService);
})();