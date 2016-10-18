(function () {

    var app = angular.module('app', ['ui.router', 'smartArea', 'monospaced.elastic', 'ngSanitize']);

    app.config(function ($stateProvider, $urlRouterProvider, $locationProvider) {
        //
        // For any unmatched url, redirect to /state1
        $urlRouterProvider.otherwise("/Home");
        //
        // Now set up the states
        $stateProvider
          .state('Home', {
              url: "/Home",
              templateUrl: "App/Views/Home/HomeView.html"
          })
            .state('Profile', {
                url: "/Profile",
                defaultChild: 'Profile.Posts',
                templateUrl: "App/Views/Profile/ProfileView.html"
            })
           .state('Profile.Activity', {
               url: "/Activity/:id",
               templateUrl: "App/Views/Profile/Partials/UserTimeLinePartialView.html"
           })
            .state('Profile.Photos', {
                url: "/Photos/:id",
                templateUrl: "App/Views/Profile/Partials/PhotosPartialView.html"
            })
             .state('Profile.Files', {
                 url: "/Files/:id",
                 templateUrl: "App/Views/Profile/Partials/FilesPartialView.html"
             })
            .state('Profile.Rooms', {
                url: "/Rooms/:id",
                templateUrl: "App/Views/Profile/Partials/RoomsPartialView.html"
            })
            .state('Profile.Friends', {
                url: "/Friends/:id",
                templateUrl: "App/Views/Profile/Partials/FriendsPartialView.html"
            })
            .state('Profile.Posts', {
                url: "/Posts",
                templateUrl: "App/Views/Profile/Partials/PostsContainer.html"
            })

            .state('Profile.Posts.All', {
                url: "/All/:id",
                templateUrl: "App/Views/Profile/Partials/PostsPartialView.html"
            })
            .state('Profile.Posts.Anonimus', {
                url: "/Anonimus/:id",
                templateUrl: "App/Views/Profile/Partials/PostsPartialView.html"
            })
            .state('Profile.Posts.Saved', {
                url: "/Saved/:id",
                templateUrl: "App/Views/Profile/Partials/PostsPartialView.html"
            })
            .state('Profile.Personal', {
                url: "/Personal/:id",
                templateUrl: "App/Views/Profile/Partials/PersonalPartialView.html"
            })

            .state('Posts', {
                url: "/Posts/:id",
                templateUrl: "App/Views/Home/HomeView.html"
            })
            .state('Post', {
                url: "/Post/:id",
                templateUrl: "App/Views/Home/HomeView.html"
            })

            .state('Rooms', {
                url: "/Rooms",
                templateUrl: "App/Views/Rooms/RoomsView.html"
            })
            .state('Rooms.Opened', {
                url: "/Opened",
                templateUrl: "App/Views/Rooms/Partials/OpenedRoomPartialView.html"
            })
            .state('Rooms.Opened.Messages', {
                url: "/Messages/:id",
                templateUrl: "App/Views/Rooms/Partials/RoomMessagesPartialView.html"
            })
            .state('Rooms.Opened.People', {
                url: "/People/:id",
                templateUrl: "App/Views/Rooms/Partials/RoomPeoplePartialView.html"
            })
            .state('Rooms.Opened.Photos', {
                url: "/Photos/:id",
                templateUrl: "App/Views/Rooms/Partials/RoomPhotosPartialView.html"
            })
            .state('Rooms.Opened.Files', {
                url: "/Files/:id",
                templateUrl: "App/Views/Rooms/Partials/RoomFilesPartialView.html"
            })
            .state('Rooms.New', {
                url: "/New",
                templateUrl: "App/Views/Rooms/Partials/NewRoomPartialView.html"
            })

          

          .state('state2.list', {
              url: "/list",
              templateUrl: "partials/state2.list.html",
              controller: function ($scope) {
                  $scope.things = ["A", "Set", "Of", "Things"];
              }
          });

        //$locationProvider.html5Mode({
        //    enabled: true,
        //    requireBase: false
        //});
    });

    app.directive('ngEnter', function () {
        return function (scope, element, attrs) {
            element.bind("keydown keypress", function (event) {
                if (event.which === 13) {
                    scope.$apply(function () {
                        scope.$eval(attrs.ngEnter);
                    });

                    event.preventDefault();
                }
            });
        };
    });

})();