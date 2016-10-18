(function () {

    var fileUpload = function ($http) {



        return {
            restrict: 'E',
            replace: true,
          //  scope: {},
            require: '?ngModel',
            template: '<div class="upload_image" >upload/drag/move image here <input id="room_file" type="file" style="display:none"/> <div style="background:url({{room.getlogo(room.roomModel.Image)}});width:100%;height:100%"></div> </div>',
            link: function (scope, element, attrs, ngModel) {

                element.on('dragover', function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                });
                element.on('dragenter', function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    element.addClass("active")
                });
                element.on('dragleave', function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    element.removeClass("active")
                });
                element.on('drop', function (e) {
                    e.preventDefault();
                    e.stopPropagation();

                    if (e.originalEvent.dataTransfer) {
                        if (e.originalEvent.dataTransfer.files.length > 0) {
                            upload(e.originalEvent.dataTransfer.files);
                        }
                    }
                    return false;

                });

                element.on('click', function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    
                    $("#room_file").trigger("click");
                });

                var upload = function (files) {
                    var data = new FormData();
                    angular.forEach(files, function (value) {
                        data.append("file", value);
                    });

                   // data.append("objectId", ngModel.$viewValue);

                    $http({
                        method: 'POST',
                        url: attrs.to,
                        data: data,
                        withCredentials: true,
                        headers: { 'Content-Type': undefined },
                        transformRequest: angular.identity
                    }).success(function (d) {
                        scope.room.roomModel.Image = d;
                        console.log("Uploaded");
                    }).error(function (d) {
                        console.log("Error");
                    });
                };
            }

        }
    };

    angular.module('app').directive('upload', ['$http', fileUpload])

})();

angular.module('app').directive('customOnChange', function () {
  return {
    restrict: 'A',
    link: function (scope, element, attrs) {
      var onChangeFunc = scope.$eval(attrs.customOnChange);
      element.bind('change', onChangeFunc);
    }
  };
});

angular.module('app').directive('animateOnChange', function ($animate, $timeout) {
    return function (scope, elem, attr) {
        scope.$watch(attr.animateOnChange, function (nv, ov) {
            if (nv.Id != ov.Id) {
                debugger
                var c = "changed";//nv > ov ? 'change-up' : 'change';
                $animate.addClass(elem, c).then(function () {
                    $timeout(function () { $animate.removeClass(elem, c) });
                });
            }
        })
    }
})