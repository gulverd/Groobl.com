(function () {

    var notificationController = function ($scope, $state, userService, roomsService, notificationService, friendsService) {

        var vm = this;

        vm.users = [];
        vm.rooms = [];

        vm.searchModel = "";
        vm.searchTimeOut = 0;
        vm.searcUser = false;
        vm.searcRoom = false;
        vm.search = function () {

            clearTimeout(vm.searchTimeOut);

            vm.searchTimeOut = setTimeout(function () {

                if (vm.searchModel == "") {
                    vm.users = [];
                    vm.rooms = [];
                    return;
                }
                    
                vm.searcUser = false;
                vm.searcRoom = false;

                userService.getUserByName(vm.searchModel).then(function (d) {
                    vm.users = d;
                    if (d.length == 0)
                        vm.searcUser = true;

                    setTimeout(nav, 100);
                });

                roomsService.searchRooms(vm.searchModel, true).then(function (d) {
                    vm.rooms = d;
                    if (d.length == 0)
                        vm.searcRoom = true;

                    setTimeout(nav, 100);
                });

            });


        }

        vm.goToUser = function () {

            $("#search_open_users_wrapper .col-md-12.selected a").trigger("click");
            vm.searchModel = "";
        }

        vm.getlogo = function (data) {

            if (data == undefined)
                return "";

            return data.replace("~", "");
        }

        $("body").click(function () {
            vm.users = [];
            vm.rooms = [];
            vm.searchModel = "";
            vm.opened = false;
            vm.searcUser = false;
            vm.searcRoom = false;
        });

        vm.NotificationTypeEnum=
        {
            PostAddedOnInterest:1,
            CmmentedOnYourPost:2,
            LikedYourPost:3,
            SharedYourPost:4,

            AddedYouInRoom:5,
            CommentedInRoom:6,
            ChangedYouStatusInRoom:7,
            RequestedAddToRoom:10,

            SendMessageInChat:8,
            AddEdYouAsFriend: 9,

            RequestedAddFriend:11
        }

        vm.notificationCount = {};

        notificationService.getUserNotificationsCount().then(function (d) {
            vm.notificationCount = d;
        });

        vm.getNotificationText = function () {

            var list = vm.notificationCount["Item2"];

            if (list.length == 0)
                return "No Any Notificarions";

            var type = list[0].Type;

            if (type == vm.NotificationTypeEnum.PostAddedOnInterest) {

                return "posted"
            }

        }

        vm.notifications = [];
        vm.opened = false;
        vm.openNotifications = function () {

            if (vm.opened) {
                vm.opened = false;
                return;
            }

            notificationService.getUserNotifications(0, 0).then(function (d) {
                vm.notifications = d;
                vm.opened = true;

                setTimeout(function () {
                    $("#not_list>div").niceScroll();
                }, 100);
            });
        }

        function poolingForData() {

            notificationService.getUserNotificationsCount().then(function (d) {

                var div = $("#last_not");

                if (d.Item2.Id != vm.notificationCount.Item2.Id) {

                    div.css("opacity", 1).css("top", 12).animate({ opacity: 0,top:16 }, 300, function () {
                        vm.notificationCount = d;

                        div.css("top", 4).animate({ opacity: 1, top: 12 }, 300, function () {
                            setTimeout(function () {
                                poolingForData();
                            }, 2000);
                        });

                    });

                } else {

                    vm.notificationCount = d;
                    setTimeout(function () {
                        poolingForData();
                    }, 2000);
                }
 
                
            });


            //var lastId = 0;
            //if (vm.notifications.length > 0) 
            //    lastId = vm.notifications[0].Id;

            //notificationService.getUserNotifications(lastId, 0).then(function (d) {
            //    vm.notifications = d;
            //    vm.opened = true;

            //    setTimeout(function () {
            //        $("#not_list>div").niceScroll();
            //    }, 100);

            //    setTimeout(function () {
            //        $("#not_list>div").niceScroll();
            //    }, 100);

            //});
           
        }

        vm.submitUser = function (nt) {

            roomsService.addUser(nt.AliasId, nt.SenderUserId);
            nt.ApproovedStatus = true;

            notify("you have added " + nt.SenderUser.FirstName+ " "+ nt.SenderUser.LastName+" to room");
        }

        vm.notSubmitUser = function (nt) {

            notificationService.setApproved(nt.Id);
            nt.ApproovedStatus = true;

            notify("you rejected room request")
        }

        vm.addFriend = function (nt) {

            friendsService.addFriend(nt.SenderUserId);
            notificationService.setApproved(nt.Id);
            nt.ApproovedStatus = true;

            notify("you added friend");
        }

        poolingForData();

        $("#not_list>div").scroll(function(info){
      
            var top = $("#not_list>div").scrollTop();

            console.log(top, $("#not_list>div").height())

        });

        $scope.safeApply = function (fn) {
            var phase = this.$root.$$phase;
            if (phase == '$apply' || phase == '$digest') {
                if (fn && (typeof (fn) === 'function')) {
                    fn();
                }
            } else {
                this.$apply(fn);
            }
        };

        $("body").click(function () {
            
            vm.opened = false;
            $scope.safeApply()
        });

        function nav() {

            var li = $('#search_open_users_wrapper .col-md-12 ');
            var liSelected;
            $(window).keydown(function (e) {
                if (e.which === 40) {
                    if (liSelected) {
                        liSelected.removeClass('selected');
                        next = liSelected.next();
                        if (next.length > 0) {
                            liSelected = next.addClass('selected');
                        } else {
                            liSelected = li.eq(0).addClass('selected');
                        }
                    } else {
                        liSelected = li.eq(0).addClass('selected');
                    }
                } else if (e.which === 38) {
                    if (liSelected) {
                        liSelected.removeClass('selected');
                        next = liSelected.prev();
                        if (next.length > 0) {
                            liSelected = next.addClass('selected');
                        } else {
                            liSelected = li.last().addClass('selected');
                        }
                    } else {
                        liSelected = li.last().addClass('selected');
                    }
                }
            });
        }

    };

    angular.module('app')
        .controller('notificationController', ['$scope', '$state', 'userService', 'roomsService', 'notificationService', 'friendsService', notificationController]);

})();