(function () {

    var roomsController = function ($scope, $stateParams, $state, roomsService, interestsService, friendsService) {

        var vm = this;
        vm.PageState = 1;
        vm.userId = _userId;

        vm.userRooms = [];

        roomsService.getUserRooms(_userId, true).then(function (d) {
            
            vm.userRooms = d;

            setTimeout(function () { $("#rooms_list_container").niceScroll(); }, 200);
        });


        if ($state.current.name == "Rooms.New")
            vm.PageState = 2;
        

        if (vm.PageState == 2) {

            //#region Search

            vm.searchText = "";
            vm.searchData = {};
            vm.search = function () {
                roomsService.searchRooms(vm.searchText, true).then(function (d) {

                });
            }

            //#endregion

            //#region AddRegon

            vm.roomModel = {
                IsRoom: true,
                Name: "",
                Visibility: true,
                Anonimus: false,
                Image: "",
                IsArchive: false,
                MaxUsers: 40,
                UserId: vm.userId,

                MemberName:"",

                Interests: [],
                Members: [],
                Comments: []
            }

            vm.maxValue = function () {

                if (vm.roomModel.MaxUsers > 40)
                    vm.roomModel.MaxUsers = 40;

                if (vm.roomModel.MaxUsers < 0)
                    vm.roomModel.MaxUsers = 0;

                if (vm.roomModel.Members.length > Number(vm.roomModel.MaxUsers))
                    vm.roomModel.Members.splice(Number(vm.roomModel.MaxUsers), vm.roomModel.Members.length - 1);
            }

            vm.members = [];
            vm.removeMember = function (i) {
                vm.roomModel.Members.splice(i, 1);
            }
            vm.memberNameChange=function(){
                friendsService.getFriendsByName(vm.userId,vm.roomModel.MemberName).then(function (d) {
                    vm.members = d;
                });
            }

            vm.getMemberState = function (id) {
                var ind = _.findIndex(vm.roomModel.Members, { Id: id });

                return ind == -1;
            }

            vm.addToMembers = function (fr) {

                if (vm.roomModel.Members.length >= Number(vm.roomModel.MaxUsers))
                   return;
                
                vm.roomModel.Members.push(fr);
                vm.members = [];
                vm.userId, vm.roomModel.MemberName = "";
            }

            vm.getUsers = function () {

                roomsService.getUserSuggestions(vm.roomModel.MaxUsers).then(function (d) {
                    vm.roomModel.Members = d;
                });
            }

            $('#tag_it2').tagit({
                //  availableTags: sampleTags,
                // This will make Tag-it submit a single form value, as a comma-delimited field.
                singleField: true,
                singleFieldNode: $('#tag_it2'),
                placeholderText: "#add your interest",
                tagSource: function ajaxCall(request, response) {

                    $.ajax({
                        url: "/Interests/GetInterestsByName?name=" + request.term,
                        data: {
                            term: request.term
                        },
                        dataType: "json",
                        success: function (data) {

                            response($.map(data, function (item) {

                                return item;

                            }));
                        }
                        //error: function(error){
                        //alert("El valor no existe");
                        //}
                    });
                },
            });

            vm.create = function () {

                if (vm.roomModel.Name.length == 0)
                    return;

                var interests = $("#tag_it2").val().split(",");
                var intObjList = [];

                angular.forEach(interests, function (value, key) {
                    this.push({Id:0, Name:value});
                }, intObjList);

                vm.roomModel.Interests = intObjList;

                if (intObjList.length == 0 && vm.roomModel.Members.length==0)
                    return;

                roomsService.addRoom(vm.roomModel, null).then(function (d) {
                    var rm = [{
                        Id: d.Id,
                        Name: d.Name,
                        LastCommentDate: 0,
                        AreMember: true,
                        UnSeenCmments:0
                    }]
                    
                    vm.userRooms = rm.concat(vm.userRooms);

                    $state.go("Rooms.Opened.Messages", { id: d.Id });

                    notify("you have a created a room");
                });

                console.log(vm.roomModel)
            }

            //#endregion
        }


        vm.getlogo = function (data) {
            return data.replace("~", "");
        }

        vm.getAttachementName = function (att) {

            var name = att.split("/")[3];
            var dt = name.split("_")[0];
            return name.replace(dt + "_", "");
        }

        vm.users = [];
        vm.rooms = [];

        vm.searchModel = "";
        vm.searchTimeOut = 0;
        vm.search = function () {

            clearTimeout(vm.searchTimeOut);

            vm.searchTimeOut = setTimeout(function () {

                if (vm.searchModel == "")
                    return;

                friendsService.getFriendsByName(vm.userId, vm.searchModel).then(function (d) {
                    vm.users = d;
                });

                roomsService.searchRooms(vm.searchModel, true).then(function (d) {
                    vm.rooms = d;
                });

            });

        }

        $("body").click(function () {
            vm.users = [];
            vm.rooms = [];
            vm.searchModel = "";
        });

       

    };

    angular.module('app')
        .controller('roomsController', ['$scope', '$stateParams', '$state', 'roomsService', 'interestsService','friendsService', roomsController]);

})();