(function () {

    var postsController = function ($scope, $stateParams, $state, $sce, postsService, interestsService) {


        var vm = this;
        vm.Posts = [];
        vm.PageState = 1;
        vm.userId = _userId;

        vm.getText = function (t,func) {

            var value= decodeURIComponent(t);

            var link = "https://www.youtube.com/watch?v=";
            var data = value;
            if (data.indexOf(link) >=0)
            {
                var splited = value.split(link);

                var alias=splited[1].substring(0,11);

                var image = '<iframe width="520" height="300" src="https://www.youtube.com/embed/'+alias+'" frameborder="0" allowfullscreen></iframe>';
                    Text2 = splited[0] + image + splited[1].replace(alias, "");

                    value= Text2;
                }
            

            function linkify(inputText) {
                return inputText;
                var replacedText, replacePattern1, replacePattern2, replacePattern3;

                //URLs starting with http://, https://, or ftp://
                replacePattern1 = /(\b(https?|ftp):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/gim;
                replacedText = inputText.replace(replacePattern1, '<a href="$1" target="_blank">$1</a>');

                //URLs starting with "www." (without // before it, or it'd re-link the ones done above).
                replacePattern2 = /(^|[^\/])(www\.[\S]+(\b|$))/gim;
                replacedText = replacedText.replace(replacePattern2, '$1<a href="http://$2" target="_blank">$2</a>');

                //Change email addresses to mailto:: links.
                replacePattern3 = /(([a-zA-Z0-9\-\_\.])+@[a-zA-Z\_]+?(\.[a-zA-Z]{2,6})+)/gim;
                replacedText = replacedText.replace(replacePattern3, '<a href="mailto:$1">$1</a>');

                return replacedText;
            }

            return $sce.trustAsHtml(linkify(value));
        }

        vm.defaultInterests = [{ Id: -3, Name: "friends", Posts: 0 }, { Id: -2, Name: "nearby", Posts: 0 }, { Id: -1, Name: "anonymous", Posts: 0 }];

        if ($state.current.name == "Posts")
            vm.PageState = 2;
        if ($state.current.name == "Post")
            vm.PageState = 3;
        
        if ($state.current.name == "Profile.Posts.All")
            vm.PageState = 4;
        if ($state.current.name == "Profile.Posts.Anonimus")
            vm.PageState = 5;
        if ($state.current.name == "Profile.Posts.Saved")
            vm.PageState = 6;

        //#region Add Post region

        vm.postModel = {
            Text: "",
            InterestNames: [],
            IsAnonimus: false,
            UserId: _userId,
            CurrentInterest:"",
        }

        vm.addPost = function () {

            var file = document.getElementById("file").files[0];

            vm.postModel.InterestNames = $('#tag_it').val().split(',');

            if ($('#tag_it').val().length == 0)
                return;

            //vm.getText(vm.postModel.Text, function (d) {

            //    debugger
            //});
            //return
            vm.postModel.Text = encodeURIComponent(vm.postModel.Text.replace(/\r\n|\r|\n/g, "<br />"));
            //  vm.postModel.Text=vm.getText(vm.postModel.Text);
            vm.postModel.Anonoimus = vm.postModel.IsAnonimus;
            postsService.addPost(vm.postModel, file).then(function (d) {
                var fr = [d];
                vm.Posts = fr.concat(vm.Posts);

                removeTempImage();

                location.hash = "#/Home";

                $("#create_post_textarea").height(15);

            });


            for (var i = 0; i < vm.postModel.InterestNames.length; i++)
                $('#tag_it').tagit("removeTagByLabel", vm.postModel.InterestNames[i]);

            vm.postModel = {
                Text: "",
                InterestNames: [],
                IsAnonimus: false,
                UserId: _userId,
                CurrentInterest: "",
            }

        }


        //#endregion

        //#region posts

        vm.getPosts = function (start, end) {

            if (vm.PageState == 1) {
                postsService.getPosts(start, end).then(function (d) {

                    for (var i = 0; i < d.length; i++) {
                        vm.Posts.push(d[i]);
                    }

                });
            } else if (vm.PageState == 2) {

                postsService.getPostsByInterest($stateParams.id, start, end).then(function (d) {

                    for (var i = 0; i < d.length; i++) {
                        vm.Posts.push(d[i]);
                    }


                    //Set Interest As Seen

                    var ind = _.findIndex(angular.element("body").scope().user.userModel.Interests, { Id: Number($stateParams.id) });

                    angular.element("body").scope().user.userModel.Interests[ind].Posts = 0;

                });
            }
            else if (vm.PageState == 3) {

                postsService.getPostById($stateParams.id).then(function (d) {

                    vm.Posts = [d];

                });
            }
            else if (vm.PageState == 4) {

                postsService.getPostsByUserId($stateParams.id, start, end).then(function (d) {

                    for (var i = 0; i < d.length; i++) {
                        vm.Posts.push(d[i]);
                    }

                });
            }
            else if (vm.PageState == 5) {

                postsService.getAnonimusPostsByUserId($stateParams.id, start, end).then(function (d) {

                    for (var i = 0; i < d.length; i++) {
                        vm.Posts.push(d[i]);
                    }

                });
            }
            else if (vm.PageState == 6) {

                postsService.getSavedPostsByUserId($stateParams.id, start, end).then(function (d) {

                    for (var i = 0; i < d.length; i++) {
                        vm.Posts.push(d[i]);
                    }

                });
            }

        }

        vm.getPosts(0, 10);

        vm.deletePost = function (post) {

            postsService.deletePost(post.Id);

            post.Delete = true;

        }

        vm.hidePost = function (post) {

            postsService.hidePost(post.Id);

            post.Delete = true;

        }

        vm.reportPost = function (post) {

            postsService.reportPost(post.Id);
            post.Reported = true;
            notify("post reported");

        }

        //#endregion

        //#region Rate

        vm.rate = function (rate, post) {

            postsService.ratePost(post.Id, rate).then(function (d) {

                post.Rate = d;

            });

        }

        vm.getRate = function (rt, i) {

            if (rt == undefined)
                return 0;

            return rt[i] == undefined ? 0 : rt[i];
        }

        //#endregion

        //#region Comments

        vm.addingComment = false;
        vm.addComment = function (post) {

            var file = document.getElementById("cooment_attachement_" + post.Id).files[0];

            if (post.Comment.length == 0 || vm.addingComment)
                return;

            vm.addingComment = true;

            post.CommentsCount++;

            post.Comment = encodeURIComponent(post.Comment.replace(/\r\n|\r|\n/g, "<br />"))

            postsService.addComment({
                Text: post.Comment,
                PostId: post.Id,
                UserId: _userId
            }, file).then(function (d) {
                vm.addingComment = false;

                var b = [d].concat(post.PostComments);

                post.PostComments = b;
                post.Comment = "";

                $(".comment_textarea").height(15);
            });

        }
        vm.deleteComment = function (postComments,index,id) {

            postsService.deleteComment(id);

            postComments.splice(index, 1);
        }
        //#endregion

        //#region Save

        vm.savePost = function (post) {

            postsService.savePost(post.Id);

            if (post.Saved == 1) {
                post.Saved = 0;
                notify("post removed from saved list !");
            }
            else {
                post.Saved = 1;
                notify("post succesfully saved !");
            }

        }

        //#endregion

        //#region Share 

        vm.sharePost = function (post) {

            postsService.sharePost(post.Id).then(function (d) {
                var fr = [d];
                vm.Posts = fr.concat(vm.Posts);
                notify("you shared post");
            });
        }

        //#endregion

        //#region Filter

        vm.Interest = {};
        if (vm.PageState == 2) {

           // if ($stateParams.id < 0) {

           //     vm.Interest = _.findWhere(vm.defaultInterests, { Id: Number($stateParams.id) });


            //} else {

                interestsService.getFilterInterest($stateParams.id).then(function (d) {

                    vm.Interest = d;

                });
           // }
        }

        vm.addToMyInterest = function (interest, user) {

            interestsService.addToMyInterest(interest.Id);

            if (interest.IsMain) {

                var index = _.findIndex(user.Interests, { Id: interest.Id });

                user.Interests.splice(index, 1);

            } else {
                user.Interests.push({
                    Id: interest.Id,
                    Name: interest.Name,
                    Posts:0
                });
            }

            interest.IsMain = !interest.IsMain;
        }

        //#endregion

        //#region Overlay

        vm.selectedPost=null;

        vm.openImage = function (pt) {
            vm.selectedPost = pt;
        }

        vm.closePopup = function () {

            if ($(".opened_post:hover").length == 0)
                vm.selectedPost = null;
        }

        vm.selectedComment = null;

        vm.openComment = function (co) {
            vm.selectedComment = co;
        }

        vm.closeComment = function () {

            if ($(".popup_post:hover").length == 0)
                vm.selectedComment = null;
        }
        //#endregion

        $('#tag_it').tagit({
          //  availableTags: sampleTags,
            // This will make Tag-it submit a single form value, as a comma-delimited field.
            singleField: true,
            singleFieldNode: $('#tag_it'),
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

        vm.changeInterest = function () {

            debugger
        }

        $("#post_attach").on("click", function () {
            $("#file").click();
        });


        vm.getlogo = function (data) {
            return data.replace("~", "");
        }

        vm.getAttachementName = function (att) {

            var name = att.split("/")[3];
            var dt = name.split("_")[0];
            return name.replace(dt + "_", "");
        }
        vm.openCommentUpload = function (id) {

            $("#cooment_attachement_" + id).trigger("click");

        }

        vm.postMenu = function (pt) {
            
            var allMenu = $(".drop_down_menu")
            var currMenu = $(".drop_down_menu_" + pt.Id);

            if (currMenu.hasClass("active")) {
                allMenu.removeClass("active");
            }
            else {
                allMenu.removeClass("active");
                currMenu.addClass("active");
            }

        }

        $('body').click(function () {
            if ($(".post_buttons_wrp:hover").length == 0)
                $(".drop_down_menu").removeClass("active");
        });

        //single post info

        vm.moreComments = function (post) {
            var postId = post.Id;
            var postCommentsCount = post.CommentsCount;
            var commentsCount = post.PostComments.length;

            postsService.getPostComments(postId, commentsCount , 10).then(function (d) {

                var newComms = post.PostComments.concat(d);

                post.PostComments = newComms;

            });
        }

        vm.getNewComment = function () {

            setTimeout(function () {

                if (vm.PageState == 3) {
                    var postId = vm.Posts[0].Id;
                    var lastId = vm.Posts[0].PostComments[0].Id;

                    postsService.getNewComment(postId, lastId).then(function (d) {

                        var newComms = d.concat(vm.Posts[0].PostComments);

                        vm.Posts[0].PostComments = newComms;

                        vm.getNewComment();

                    });
                } else {
                    vm.getNewComment();
                }

            }, 5000);
           
        }

        vm.getNewComment();

        //end single post

        //upload single file fro post

        vm.uploadForShow = function () {
            debugger
            var file = document.getElementById("file").files[0];

            postsService.uploadTemp(file).then(function (d) {

                var ul = $(".tagit");

                var extArray = ["jpg", "JPG", "gif", "GIF", "png", "PNG", "svg", "SVG" ];

                var ext = d.split(".")[d.split(".").length - 1];

                var isImage = extArray.indexOf(ext) > -1;

                var file = "";

                if (isImage) {
                    file = "<div class='post_temp_image'><img src='" + vm.getlogo(d) + "'><span>X<span></div>";
                } else {
                    file = "<div class='post_temp_attch'>X<span>X<span></div>"
                }

                var il = '<li class="post_temp_file">' + file + '</li>';


                ul.append(li);

            });
        }

        $(window).scroll(function () {


            var scrollTop = $("body").scrollTop();

            var height = $("body").height();
            
            if ((scrollTop + 350) > height) {
                
                
                vm.getPosts(vm.Posts.length, 10);

                console.log((vm.Posts.length), height)

                $scope.$apply()
            }

        });

        vm.emotions = null;
        vm.emotionPostId = 0;
        vm.seeEmotions = function (post) {
            vm.filterRates = 0;
            vm.emotionPostId = post.Id;
            postsService.getPostRatesByPostId(vm.emotionPostId, 0).then(function (d) {
                post.Emotions = d;

                $(".emmotion_authors_wrp_" + post.Id).css("display", "block");

                setTimeout(function () { $("#emotion_scroll").niceScroll(); }, 100);
                
            });
        }
        vm.seeMoreEmotions = function (post) {
            postsService.getPostRatesByPostId(post.Id, posr.Emotions.length).then(function (d) {
                vm.emotions = vm.emotions.concat(d);
                setTimeout(function () { $("#emotion_scroll").niceScroll(); }, 100);
            });
        }
        vm.closeEemotions = function (id) {
            $(".emmotion_authors_wrp_" + id).css("display", "none");
        }

        vm.filterRates = 0;

        $("body").on("click", function () {

            if ($(".emmotions_authors_hide:hover").length==0)
            $(".emmotions_authors_hide").css("display", "none");
        })
    };

    angular.module('app')
        .controller('postsController', ['$scope','$stateParams', '$state','$sce','postsService','interestsService', postsController]);

})();

function bb() {
    
    var getlogo = function (data) {
        return data.replace("~", "");
    }

    var file = document.getElementById("file").files[0];

    oData = new FormData();

    oData.append("file", file);

    var oReq = new XMLHttpRequest();
    oReq.open("POST", "/Posts/UploadTemp", true);
    oReq.onload = function (oEvent) {
        if (oReq.status == 200) {

            $(".post_temp_file").remove();

            var img = eval(oReq.response);

            var ul = $(".post_uploads");

            var extArray = ["jpg", "JPG", "gif", "GIF", "png", "PNG", "svg", "SVG"];

            var ext = img.substring(img.length - 3, img.length);

            var isImage = extArray.indexOf(ext) > -1;

            var file = "";
            
            if (isImage) {
                file = "<div class='post_temp_image'><img src='" + getlogo(img) + "'><span  onclick='removeTempImage()'>X<span></div>";
            } else {
                file = "<div class='post_temp_attch'>" + ext + "<span onclick='removeTempImage()'>X<span></div>"
            }

            var il = '<div class="post_temp_file">' + file + '</div>';


            ul.append(il);


        }
    };

    oReq.send(oData);



}

var removeTempImage = function () {

    $(".post_temp_file").remove();

    $("#file").val(null);
}

var openCommentUpload = function (d) {

    var id = $(d).attr('ng-data-id');

    $("#cooment_attachement_" + id).trigger("click");

}