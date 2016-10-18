(function () {

    var openedRoomController = function ($scope, $stateParams, $state, roomsService, interestsService, friendsService) {

        var vm = this;
        vm.PageState = 1;
        vm.userId = _userId;
        vm.openedRoom = {};

        $scope.$on("$stateChangeSuccess", function updatePage() {
            $scope.page = $state.params.slug;
            initPage()
        });

        var initPage = function () {

            vm.roomId = $stateParams.id;

            roomsService.getRoomById(vm.roomId, _userId).then(function (d) {
                vm.openedRoom = d;

                if ($state.current.name == "Rooms.Opened.People")
                    vm.getUsers();

                setTimeout(function () {
                    $("#room_comment_lists_wrapper").scrollTop(100000);
                }, 100);

            });

            vm.roomCommentModel = {
                UserId: _userId,
                Text: "",
                RoomId: vm.roomId,
                Image: "",
                Attachement: ""
            }

            vm.attacheFile = function () {
                $("#comment_file").trigger("click");
            }

            vm.sendComment = function () {

                if (vm.roomCommentModel.Text == "")
                    return;

                var file = document.getElementById("comment_file").files[0];

                roomsService.addComment(vm.roomCommentModel, file).then(function (d) {

                    vm.openedRoom.Comments.push(d);

                    
                    document.getElementById("comment_file").value = "";

                    setTimeout(function () {
                       $("#room_comment_lists_wrapper").scrollTop(100000);
                    }, 100);
                })

                vm.roomCommentModel.Text = "";
            }

            vm.sendEmo = function (id) {
                
                vm.roomCommentModel.Text = "__emo__|" + id;

                vm.sendComment();

            }

            vm.getStateOfComment = function (text) {

                return (text.indexOf("__emo__|") > -1)
            }

            vm.getValueOfComment = function (text) {

                return text.split("__emo__|")[1];
            }

            //#region People

            vm.groupedMembers = {};
            vm.getUsers = function () {

                var names = {};

                vm.openedRoom.Members.forEach(function (obj) {
                    if (!obj.FirstName || !obj.FirstName.length) return;

                    var firstLetter = obj.FirstName[0];

                    if (!names[firstLetter]) {
                        names[firstLetter] = [];
                    }
                    names[firstLetter].push(obj);
                });

                console.log(names)
                vm.groupedMembers = names;
            }

            vm.changeStatus = function (us, status) {
                us.Status = status;

                if(status==1)
                    roomsService.activateUser(vm.roomId, us.Id);
                else if (status == 2)
                    roomsService.hideUser(vm.roomId, us.Id)
                else if (status == 3)
                    roomsService.muteUser(vm.roomId, us.Id)

                if (status == 1)
                    notify("you have activated user");
                else if (status == 2)
                    notify("you have hided user");
                else if (status == 3)
                    notify("you have muted user");

            }

            vm.getMemberStatus = function (id) {
                var item = _.findWhere(vm.openedRoom.Members, { Id: id });

                if (item == undefined)
                    return 1;

                return item.Status;
            }
            
            //#endregion

            //#region Files

            vm.files = [];
            vm.getFiles = function (isImage) {

                roomsService.getAttachements(vm.roomId, isImage).then(function (d) {
                    vm.files = d;
                });
            }

            if ($state.current.name == "Rooms.Opened.Photos")
                vm.getFiles(true);
            if ($state.current.name == "Rooms.Opened.Files")
                vm.getFiles(false);
            //#endregion

            vm.sendAddRequest = function () {
                roomsService.requestAddToRoom(vm.openedRoom.UserId, vm.openedRoom.Id);
                vm.openedRoom.RequestSent=true;
            }

        }

        vm.getlogo = function (data) {
            return data.replace("~", "");
        }

        vm.getAttachementName = function (att) {

            var name = att.split("/")[3];
            var dt = name.split("_")[0];
            return name.replace(dt + "_", "");
        }

        //Last comments

        vm.getLastComments = function () {

            setTimeout(function () {

                var id = vm.openedRoom.Id;
                
                var lastId = 0;

                if (vm.openedRoom.Comments.length>0)
                       lastId = vm.openedRoom.Comments[vm.openedRoom.Comments.length - 1].Id;

                roomsService.getLastComments(id, lastId).then(function (d) {

                    var b = vm.openedRoom.Comments.concat(d);

                    vm.openedRoom.Comments = b;

                    vm.getLastComments();

                });

            }, 3000);

        }

        vm.getLastComments();
    }

    angular.module('app')
        .controller('openedRoomController', ['$scope', '$stateParams', '$state', 'roomsService', 'interestsService', 'friendsService', openedRoomController]);

})();

var attacheFile = function () {
    $("#comment_file").trigger("click");
}