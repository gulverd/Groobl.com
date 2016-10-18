(function () {

    var userController = function ($scope, $state, userService, friendsService, notificationService) {

        var vm = this;

        vm.userModel = {}

        vm.defaultInterests = [{ Id: -3, Name: "friends", Posts: 0 }, { Id: -2, Name: "nearby", Posts: 0 }, { Id: -1, Name: "anonymous", Posts: 0 }];

        userService.getUser(_userId).then(function (d) {

            vm.userModel = d;
          //  vm.userModel.Interests = vm.defaultInterests.concat(vm.userModel.Interests);
        })

        vm.openInterest = function (id) {
            $state.go('Posts', { id: id });
        }

        
            $('#tag_it').tagit({
                availableTags: sampleTags,
                // This will make Tag-it submit a single form value, as a comma-delimited field.
                singleField: true,
                singleFieldNode: $('#tag_it'),
                placeholderText: "#add your interest"
            });
            setTimeout(function () { 
                $(".ui-autocomplete-input").on("keydown", function (e) {

                    if (e.key == "#")
                        return false;
                    console.log(e)
             //   $(".ui-autocomplete-input").val($(".ui-autocomplete-input").val().replace("#", ""))
            });
            }, 300);
        
        //region interests
           
            vm.toogleInterests = function () {

                var div = $("#main_interest_list");

                if (div.hasClass("active"))
                    div.removeClass("active");
                else
                    div.addClass("active");
            }

        //endregion

            vm.addFriend = function (user) {
               
                if (user.IsFriend) {
                    friendsService.removeFriend(user.Id);
                    notify("friend removed!");
                } else {
                    friendsService.addFriend(user.Id);
                    notify("friend added!");
                }

                user.IsFriend = !user.IsFriend;
                user.RequestSent = false;
                user.RequestRecived = false;

            }

            vm.removeFriend = function (user) {
                friendsService.removeFriend(user.Id);
                notify("friend removed!");
                user.IsFriend = false;
                user.RequestSent = false;
                user.RequestRecived=false;
            }

            vm.sendRequest = function (userId, us) {
                notificationService.addNotification(userId);
                us.RequestSent = true;
                notify("friend request sent!");
            }
    };

    angular.module('app')
        .controller('userController', ['$scope', '$state', 'userService','friendsService','notificationService', userController]);

})();

var tm;
function notify(st) {

    clearTimeout(tm);

    var div = $("#not");

    div.addClass("active").text(st);

    tm = setTimeout(function () {

        div.removeClass("active").text("");

    }, 5000);

}