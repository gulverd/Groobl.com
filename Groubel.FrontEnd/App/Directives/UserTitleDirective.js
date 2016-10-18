(function () {

    var userTitleDirective = function ($http) {
        return {
            restrict: 'A',
            replace: false,
          //  scope: {},
        //    require: '?ngModel',
     //       template: '<div class="upload_image" >upload/drag/move image here <input id="room_file" type="file" /> <div style="background:url({{room.getlogo(room.roomModel.Image)}})"></div> </div>',
            link: function (scope, element, attrs, ngModel) {
                
                var id = attrs.id;
  
                var st = attrs.up;

                element.on('mouseenter', function (e) {
                    e.preventDefault();
                    e.stopPropagation();

                    var data = $http.get("/User/GetUserByIdForPopup?id=" + id + "&curId=" + _userId).then(function (d) {
                        drowHtml(d.data);
                    });

                });

                element.on('mouseleave', function (e) {
                    e.preventDefault();
                    e.stopPropagation();

                    setTimeout(function () {

                        if ($(".bd_"+id+":hover").length > 0)
                            return;
                        
                       element.find(".user_pop_body").remove();
                    },500);
                    
                });

                var drowHtml = function (data) {
                    
                    if (element.find(".user_pop_body").length > 0)
                        return;

                    $(".user_pop_body").remove();

                    var shareFriends = data.ShareFriends;

                    var count = shareFriends.length;

                    var htmlFriends = "";

                    if (data.Id != _userId) {
                        if (count > 0) {
                            if (shareFriends[0] != undefined)
                                htmlFriends = shareFriends[0];

                            if (shareFriends[1] != undefined)
                                htmlFriends += ", " + shareFriends[1];

                            if (count - 2 > 0)
                                htmlFriends += " and " + (count - 2) + " other mutual friends";
                            else
                                htmlFriends = " mutual friends";
                        } else {
                            htmlFriends = "no shared friends";
                        }
                    }

                    var cls = "";

                    if (st == "true")
                        cls="new_pop"

                    var html = '<div class="user_post_hover_wrapper user_pop_body bd_' + id + ' ' + cls + '">' +
								'<div class="col-xs-12 user_post_hover_top_wrapper">' +
									'<img src="' + getlogo(data.Image) + '" class="user_post_hover_image">' +
								'</div>		' +
								'<div class="col-xs-12 user_post_hover_bottom_wrapper">' +
									'<div class="col-xs-12 user_post_hover_bottom_username_wrapper">' +
										'<a ui-sref="Profile.Posts.All({id:data.Id})">' + data.FirstName + ' ' + data.LastName + '</a>' +
									'</div>' +
									'<div class="col-xs-12 user_post_hover_bottom_mutual_wrapper">' +
										 htmlFriends +
									'</div>' +
									'<div class="col-xs-6 user_post_hover_bottom_texts_wrapper">' +
										'<div>' +
										'<i class="fa fa-circle-o" aria-hidden="true"></i> ' + data.LastLoginString +
										'</div>' +
										//'<div>'+
										////'<i class="fa fa-map-marker" aria-hidden="true"></i> Tbilisi - Varketili'+
										//'</div>'+
									'</div>' +

                    '<div class="col-xs-6 user_post_hover_bottom_right_side_wrapper">';
										

                      if (data.Id != _userId) {
                        if (data.IsFriend) {
                            html += '<button type="button" class="btn btn-info user_post_hover_button" title="remove from friends">Friends <i class="fa fa-users" aria-hidden="true"></i></button>';
                        } else {

                            if (data.RequestSent) {
                                html += '<button type="button" class="btn btn-default" id="exp_friends_wrp_not_friend_btn">Reqsuest sent <i class="fa fa-users" aria-hidden="true"></i></button>';
                            } else if (data.RequestRecived) {
                                html += '<button type="button" class="btn btn-default" id="exp_friends_wrp_not_friend_btn">Reqsuest received <i class="fa fa-users" aria-hidden="true"></i></button>';
                            } else {
                                html += '<button type="button" class="btn btn-default" id="exp_friends_wrp_not_friend_btn">Add friend <i class="fa fa-users" aria-hidden="true"></i></button>';
                            }

                        }
                    }




								html+=	'</div>'+
								'</div>'+
							'</div>';
                        
                        
                    //    <span class="user_pop_body bd_'+id+' '+cls+'" style="display:block !important">' +
                    //                    '<span class="po_user_image"><img src="' + getlogo(data.Image) + '"/></span>' +
                    //                    '<a ui-sref="Profile.Posts.All({id:data.Id})" class="po_user_name">' + data.FirstName + ' ' + data.LastName + '</a>' +

                    //                    '<div class="pop_fr">' + htmlFriends + '</div>' +

                    //                   '<div class="pop_fr_status">';

                    //if (data.Id != _userId) {
                    //    if (data.IsFriend) {
                    //        html += '<div class="add_to_friend active" title="remove from friends"></div>';
                    //    } else {

                    //        if (data.RequestSent) {
                    //            html += '<div class="add_to_friend sent" title="request sent"></div>';
                    //        } else if (data.RequestRecived) {
                    //            html += '<div class="add_to_friend sent" title="request recived, add to friends"></div>';
                    //        } else {
                    //            html += '<div class="add_to_friend" title="send request"></div>';
                    //        }

                    //    }
                    //}

                    //html += '</div>' +
                    //    '</span>';

                    element.append(html);

                }

                var getlogo = function (data) {
                    return data.replace("~", "");
                }

            }

        }
    };

    angular.module('app').directive('user', ['$http', userTitleDirective])

})();