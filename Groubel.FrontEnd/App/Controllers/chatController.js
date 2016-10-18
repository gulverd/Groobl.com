(function () {

    var chatController = function ($scope, $stateParams, $state, roomsService, interestsService, friendsService) {

        $(".chat_area").niceScroll()

        var vm = this;
        vm.PageState = 1;
        vm.userId = _userId;

        vm.userRooms = [];
        vm.openedRoom = {};

        roomsService.getUserRooms(_userId, false).then(function (d) {
            
            vm.userRooms = d;

            console.log(vm.userRooms)

            if (d.length > 0)
                getRoomById(d[0].Id);

            if (d.length == 0)
                angular.element(".user_chat").scope().friend.opened = true;

        });

        var chatTimer = function () {

           
                setTimeout(function () {

                    var empty = vm.userRooms.length;

                    roomsService.getUserRooms(_userId, false).then(function (d) {

                        vm.userRooms = d;
                        //for (var i = 0; i < d.length; d++) {
                        //    var it = d[i];

                        //    var ind = _.findIndex(vm.userRooms, { Id: it.Id });
                        //    if (ind == -1) {

                        //        var b = [it];

                        //        vm.userRooms = b.concat(vm.userRooms);

                        //    }
                        //}

                        chatTimer();
                    });

                }, 3000);
          

        }

        chatTimer();

        vm.getLastComments = function () {
            var friend = angular.element(".userFr").scope().friend;

            if (!friend.opened) {

            setTimeout(function () {

                var id = vm.openedRoom.Id;

                if (id != undefined) {
                    var lastId = 0;

                    if (vm.openedRoom.Comments.length > 0)
                        lastId = vm.openedRoom.Comments[vm.openedRoom.Comments.length - 1].Id;

                    roomsService.getLastComments(id, lastId).then(function (d) {

                        var b = vm.openedRoom.Comments.concat(d);

                        vm.openedRoom.Comments = b;

                        setTimeout(function () { $(".chat_area").scrollTop(100000); }, 0);

                        vm.getLastComments();

                    });
                } else {
                    vm.getLastComments();
                }

            }, 3000);
            } else {
                setTimeout(function () { vm.getLastComments(); }, 4000);
            }

        }

        vm.getLastComments();


        vm.openRoomById = function (id) {
            getRoomById(id);
        }

        function getRoomById(id) {

            roomsService.getRoomById(id, _userId).then(function (d) {
                vm.openedRoom = d;
                
                setTimeout(function () {
                    $(".chat_area").scrollTop(100000);
                }, 100);

            });

        }


        vm.roomCommentModel = {
            UserId: _userId,
            Text: "",
            RoomId: vm.openedRoom.Id,
            Image: "",
            Attachement: ""
        }

        $("#input_chat_attachment_wraper").on('click', function () { $("#comment_file").trigger("click"); });

        vm.sendComment = function () {

            if (vm.roomCommentModel.Text == "")
                return;

            var file = document.getElementById("comment_file").files[0];
            vm.roomCommentModel.RoomId = vm.openedRoom.Id;

            roomsService.addComment(vm.roomCommentModel, file).then(function (d) {

                vm.openedRoom.Comments.push(d);

                
                document.getElementById("comment_file").value = "";

                setTimeout(function () {
                    $(".chat_area").scrollTop(100000);
                }, 100);
            });
            vm.roomCommentModel.Text = "";
        }

        vm.sendEmo = function (id) {

            vm.roomCommentModel.Text = "__emo__|" + id;

            vm.sendComment();

        }

        vm.getStateOfComment = function (text) {

            return (text.indexOf("__emo__|")>-1)
        }

        vm.getValueOfComment = function (text) {

            return text.split("__emo__|")[1];
        }

        vm.getlogo = function (data) {

            if (data == undefined)
                return "";

            return data.replace("~", "");
        }

        vm.getAttachementName = function (att) {

            var name = att.split("/")[3];
            var dt = name.split("_")[0];
            return name.replace(dt + "_", "");
        }

        vm.getFriendName = function (room, user,state) {

            var name = "";
            var crUserName = user.userModel.FirstName + " " + user.userModel.LastName;

            if (room.MemberName[0] != user.userModel.FirstName+ " " + user.userModel.LastName)
                name =room.MemberName[0];

            name = room.MemberName[1];

            if (state)
                return name;

            var spl = name.split(" ");

            if (spl.length > 0)
                return spl[0];

            return name;
        }

        vm.getFriendImage = function (room, user, friends) {

            var name = "";
            var crUserName = user.userModel.FirstName + " " + user.userModel.LastName;
            debugger
            if (room.MemberName[0] != user.userModel.FirstName + " " + user.userModel.LastName)
                name = room.MemberName[0];

            name = room.MemberName[1];

            if (state)
                return name;

        }

        vm.closeChat = function (id,fr) {

            roomsService.closeChat(id);

            var findIndex = _.findIndex(vm.userRooms, { Id: id });

            vm.userRooms.splice(findIndex, 1);

            if (vm.userRooms.length > 0)
                getRoomById(vm.userRooms[0].Id);
            else {
                vm.openedRoom = {};

                fr.opened = true;
            }
        }

        vm.openChat = function (secondUserId,fr) {
            fr.opened = false;
            roomsService.openChat(secondUserId).then(function (d) {
                
                //var state = false;

                //for (var i = 0; i < vm.userRooms.length; i++) {
                //    var curRm = vm.userRooms[i].MemberName;

                //    if (curRm[0] == d.MemberName[0] && curRm[1] == d.MemberName[1])
                //        state = true;
                //}

                //if (state)
                //    return;
                if (_.findIndex(vm.userRooms, { Id: d.Id })==-1) {
                    
                    var arr = [d];
                    vm.userRooms = arr.concat(vm.userRooms);
                }
                vm.openedRoom = d;

                setTimeout(function () {
                    $(".chat_area").scrollTop(100000);
                }, 50);
            });

        }

        vm.openOldChat = function (rm, index) {

            var b = [rm];

            vm.userRooms.splice(index, 1);

            vm.userRooms = b.concat(vm.userRooms);

            getRoomById(vm.userRooms[0].Id);

        }

        vm.getCoutForUser = function (userId) {

            var room = _.findWhere(vm.userRooms, { AnotherUserId: userId });
            if (room == null || room == undefined)
                return 0;

            return room.UnSeenCmments

        }

    };

    angular.module('app')
        .controller('chatController', ['$scope', '$stateParams', '$state', 'roomsService', 'interestsService', 'friendsService', chatController]);

})();

function comment_file() { $("#comment_file").trigger("click"); };