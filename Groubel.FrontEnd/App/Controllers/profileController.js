function upIm() {
    $("#pr_img").trigger("click")
}

(function () {

    var profileController = function ($scope, $state, $http,$stateParams, userService) {

        var vm = this;

        vm.selected = false;

        $scope.$on("$stateChangeSuccess", function updatePage() {
            setTimeout(function () {
            if (location.hash.indexOf("Posts/Anonimus") > -1 || location.hash.indexOf("Posts/Saved") > -1 || location.hash.indexOf("Posts/All") > -1)
                vm.selected = true;
            else
                vm.selected = false;

           
            }, 500);
        });
        

        vm.email = "";
        vm.userModel = {}

        $scope.$on("$stateChangeSuccess", function updatePage() {
            $scope.page = $state.params.slug;
             userService.getUser($stateParams.id).then(function (d) {
            vm.userModel = d;
            vm.userModel.DateOfBirthString = new Date(vm.userModel.DateOfBirthString);
            vm.email = d.Email;
        })
        });

       

        vm.openInterest = function (id) {
            $state.go('Posts', { id: id });
        }


        vm.getlogo = function (data) {

            if (data == undefined)
                return "";

            return data.replace("~", "");
        }

        

        //region interests
           
            vm.toogleInterests = function () {

                var div = $("#main_interest_list");

                if (div.hasClass("active"))
                    div.removeClass("active");
                else
                    div.addClass("active");
            }

        //endregion

        //eddit region

            vm.img = "";
            vm.changeUpload = function () {
                
                var img = document.getElementById("pr_img").files[0];

                userService.addImage(img).then(function (d) {

                    vm.userModel.Image = d;

                });

            }

            vm.save = function () {
                
                
                var checkEmail = function (callbeck) {

                    $http.get("/Account/CheckEmail?email=" + vm.userModel.Email).then(function (d) {

                        d = d.data
                        callbeck(d);
                    });
                }

                var sendData = function () {

                    if (vm.userModel.Password != vm.userModel.RePassword && vm.userModel.Password.length>0) {
                        notify("please enter same passwords ")
                    } else {

                        var k = angular.copy(vm.userModel);

                        k.DateOfBirth = k.DateOfBirthString
                        userService.updateUser(k);
                        notify("saved");
                    }
                }

                var update = function (d) {

                    if (d == null) {
                        sendData();
                    } else {
                        if (d) {
                            notify("try different email address");
                        } else {
                            sendData();
                        }
                    }
                }

                if (vm.userModel.Email == vm.email) {
                    update(null);
                } else {
                    checkEmail(update);
                }

                
            }

        //endregion
    };

    
    angular.module('app')
        .controller('profileController', ['$scope', '$state', '$http','$stateParams', 'userService', profileController]);

    //Photos
    var photosController = function ($scope, $state, $stateParams, userService) {

        var vm=this;
        vm.photos=[];
        vm.filter = 0;

        userService.getUserFiles($stateParams.id, true).then(function (d) {
            vm.photos = d;
            setTimeout(function () { jQuery(".user_image_item ").html5lightbox(); },500);
        });

        vm.getlogo = function (data) {

            if (data == undefined)
                return "";

            return data.replace("~", "");
        }

    }

    angular.module('app')
        .controller('photosController', ['$scope', '$state', '$stateParams', 'userService', photosController]);

    //Files
    var filesController = function ($scope, $state, $stateParams, userService) {

        var vm = this;
        vm.photos = [];
        vm.filter = 0;

        userService.getUserFiles($stateParams.id, false).then(function (d) {
            vm.photos = d;
        });

        vm.getlogo = function (data) {

            if (data == undefined)
                return "";

            return data.replace("~", "");
        }

        vm.getAttachementName = function (att) {

            if (att == undefined)
                return "";

            var name = att.split("/")[3];
            var dt = name.split("_")[0];
            return name.replace(dt + "_", "");
        }
    }

    angular.module('app')
        .controller('filesController', ['$scope', '$state', '$stateParams', 'userService', filesController]);

    //Files
    var userActivityController = function ($scope, $state, $stateParams, notificationService) {

        var vm = this;
        vm.data = [];

        notificationService.getUserActivity($stateParams.id, 0).then(function (d) {
            vm.data = d;
        });

        vm.NotificationTypeEnum =
{
    PostAddedOnInterest: 1,
    CmmentedOnYourPost: 2,
    LikedYourPost: 3,
    SharedYourPost: 4,

    AddedYouInRoom: 5,
    CommentedInRoom: 6,
    ChangedYouStatusInRoom: 7,
    RequestedAddToRoom: 10,

    SendMessageInChat: 8,
    AddEdYouAsFriend: 9,
    RequestedAddFriend: 11
}


    }

    angular.module('app')
        .controller('userActivityController', ['$scope', '$state', '$stateParams', 'notificationService', userActivityController]);


    var userRoomsController = function ($scope, $state, $stateParams, roomsService) {

        var vm = this;
        vm.data = [];
        vm.filter = 0;
        vm.userId = _userId;
        vm.currentUserrId = $stateParams.id;

        roomsService.getUserRooms(vm.currentUserrId, true).then(function (d) {
            vm.data = d;
        });

        vm.getlogo = function (data) {

            if (data == undefined)
                return "";

            return data.replace("~", "");
        }

        vm.getRoomState = function (rm) {
            
            if (vm.filter == 0)
                return true;

            if (vm.filter == 1 && rm.UserId == Number(vm.currentUserrId))
                return true;

            if (vm.filter == 2 && rm.UserId == Number(vm.userId))
                return true;

            return false;
        }
    }

    angular.module('app')
        .controller('userRoomsController', ['$scope', '$state', '$stateParams', 'roomsService', userRoomsController]);

    var userFriendsController = function ($scope, $state, $stateParams, friendsService) {

        var vm = this;
        vm.data = [];
        vm.userId = _userId;
        vm.currentUserrId = $stateParams.id;

        friendsService.getFriends(vm.currentUserrId, true).then(function (d) {
            vm.data = d;
        });

        vm.removeFriend = function (index) {
            friendsService.removeFriend(vm.data[index].Id);
            vm.data.splice(index, 1);
        }

        vm.getlogo = function (data) {

            if (data == undefined)
                return "";

            return data.replace("~", "");
        }

    }

    angular.module('app')
        .controller('userFriendsController', ['$scope', '$state', '$stateParams', 'friendsService', userFriendsController]);

})();