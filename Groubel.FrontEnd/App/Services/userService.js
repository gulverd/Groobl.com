(function () {

    var userService = function ($http) {

        var getUser = function (id) {

            var data = $http.get("/User/GetUserData?id="+id+"&curId="+_userId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var ping = function (id) {

            var data = $http.get("/User/Ping?id=" + id)

        }

        var isOnline = function (id) {

            var data = $http.get("/User/IsOnline?id=" + id).then(function (d) {
                return d.data;
            });

            return data;

        }

        var getUserByName = function (name) {

            var data = $http.get("/User/GetUserByName?name=" + name+"&userId="+_userId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var getUserFiles = function (userId, isImage) {

            var data = $http.get("/User/GetUserFiles?isImage=" + isImage + "&userId=" + userId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var updateUser = function (us) {

            var data = $http.post("/User/UpdateUser", { user: us });
        }

        var addImage = function (file) {

            var fd = new FormData();

            var url = '/User/AddImage';

            fd.append('file', file);

            fd.append("id", _userId);

            var data = $http.post(url, fd, {
                withCredentials: false,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function (d) {
                return d.data;
            });

            return data;
        }

        return {

            getUser: getUser,
            isOnline: isOnline,
            ping: ping,
            getUserByName: getUserByName,
            getUserFiles: getUserFiles,
            updateUser: updateUser,
            addImage
        };
    };

    angular.module('app')
        .factory('userService', userService);
})();