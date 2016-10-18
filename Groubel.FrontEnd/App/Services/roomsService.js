(function () {

    var roomsService = function ($http) {

        var addRoom = function (room, file) {

            var url = '/Rooms/AddRoom';

            var data = $http.post(url, { chat: room }

            ).then(function (d) {
                return d.data;
            });

            return data;

        }

        var deleteChat = function (id) {
            
            var data = $http.get("/Rooms/DeleteChat?userId=" + _userId + "&id=" + id).then(function (d) {
                return d.data;
            });

            return data;

        }

        var getUserRooms = function (userId, isRoomid) {

            var data = $http.get("/Rooms/GetUserRooms?userId=" + userId + "&mainUserId=" + _userId + "&isRoom=" + isRoomid).then(function (d) {
                return d.data;
            });

            return data;

        }

        var searchRooms = function (name, isRoomid) {

            var data = $http.get("/Rooms/SearchRooms?name=" + name + "&mainUserId=" + _userId + "&isRoom=" + isRoomid).then(function (d) {
                return d.data;
            });

            return data;

        }

        var getRoomById = function (id, userId) {

            var data = $http.get("/Rooms/GetRoomById?id=" + id + "&userId=" + userId).then(function (d) {
                return d.data;
            });

            return data;

        }

        //#region Comments

        var addComment = function (comment, file) {

            var fd = new FormData();

            var url = '/Rooms/AddComment';

            fd.append('file', file);

            fd.append("userId", _userId);
            fd.append("roomId", comment.RoomId);
            fd.append("text", comment.Text);

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

        var deleteComment = function (id) {

            var data = $http.get("/Rooms/DeleteComment?id=" + id + "&userId=" + _userId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var getLastComments = function (chatId, lastCommentId) {

            var data = $http.get("/Rooms/GetLastComments?chatId=" + chatId + "&lastCommentId=" + lastCommentId+"&userId="+_userId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var GetLatestCommentsStatus = function (chats) {

            var data = $http.post("/Rooms/GetLatestCommentsStatus", { chats: chats  }).then(function (d) {
                return d.data;
            });

            return data;

        }

        //#endregion

        //#region Members

        var addUser = function (chatId, userId) {

            var data = $http.get("/Rooms/AddUser?chatId=" + chatId + "&userId=" + userId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var deleteUser = function (chatId, userId) {

            var data = $http.get("/Rooms/DeleteUser?chatId=" + chatId + "&userId=" + userId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var activateUser = function (chatId, userId) {

            var data = $http.get("/Rooms/ActivateUser?chatId=" + chatId + "&userId=" + userId + "&adminUserId="+_userId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var hideUser = function (chatId, userId) {

            var data = $http.get("/Rooms/HideUser?chatId=" + chatId + "&userId=" + userId + "&adminUserId=" + _userId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var muteUser = function (chatId, userId) {

            var data = $http.get("/Rooms/MuteUser?chatId=" + chatId + "&userId=" + userId + "&adminUserId=" + _userId).then(function (d) {
                return d.data;
            });

            return data;

        }

        //#endregion


        //#region Attachement

        var getAttachements = function (roomId, isImage) {

            var data = $http.get("/Rooms/GetAttachements?roomId=" + roomId + "&isImage=" + isImage).then(function (d) {
                return d.data;
            });

            return data;

        }

        //#endregion

        var requestAddToRoom = function (adminId, roomId) {

            var data = $http.get("/Rooms/RequestAddToRoom?userId=" + _userId + "&adminId=" + adminId+"&roomId="+roomId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var closeChat = function (id) {

            var data = $http.get("/Rooms/CloseChat?id=" + id).then(function (d) {
                return d.data;
            });

            return data;

        }

        var openChat = function (secondUserId) {

            var data = $http.get("/Rooms/OpenChat?userId=" + _userId + "&secondUserId=" + secondUserId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var getUserSuggestions = function (count) {

            var data = $http.get("/Rooms/GetUserSuggestions?userId=" + _userId + "&count=" + count).then(function (d) {
                return d.data;
            });

            return data;

        }

        

        return {

            addRoom: addRoom,
            deleteChat: deleteChat,
            getUserRooms: getUserRooms,
            searchRooms: searchRooms,
            getRoomById: getRoomById,
            addComment: addComment,
            deleteComment: deleteComment,
            getLastComments: getLastComments,
            GetLatestCommentsStatus: GetLatestCommentsStatus,
            getAttachements: getAttachements,
            activateUser: activateUser,
            hideUser: hideUser,
            muteUser: muteUser,
            requestAddToRoom: requestAddToRoom,
            addUser: addUser,

            closeChat: closeChat,
            openChat: openChat,

            getUserSuggestions: getUserSuggestions
        };
    };

    angular.module('app')
        .factory('roomsService', roomsService);
})();