(function () {

    var postsService = function ($http) {

        var addPost = function (post, file) {

            var fd = new FormData();

            var url = '/Posts/AddPosts';

                fd.append('file', file);

            fd.append("data", JSON.stringify(post));

            var data = $http.post(url, fd, {
                withCredentials: false,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function (d) {
                return d.data;
            });

            return data;

        }

        var uploadTemp = function (file) {

            var fd = new FormData();

            var url = '/Posts/UploadTemp';

            fd.append('file', file);

            var data = $http.post(url, fd, {
                withCredentials: false,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function (d) {
                return d.data;
            });

            return data;

        }


        var getPosts =  function (start,end) {

            var data = $http.get("/Posts/GetPosts?userId=" + _userId + "&start=" + start + "&end=" + end).then(function (d) {
                return d.data;
            });

            return data;

        }

        var getPostsByInterest = function (interestId, start, end) {

            var data = $http.get("/Posts/GetPostsByInterestId?interestId=" + interestId + "&userId=" + _userId + "&start=" + start + "&end=" + end).then(function (d) {
                return d.data;
            });

            return data;

        }

        var getPostsByUserId = function (userId, start, end) {

            var data = $http.get("/Posts/GetPostsByUserId?postUserId=" + userId + "&userId=" + _userId + "&start=" + start + "&end=" + end).then(function (d) {
                return d.data;
            });

            return data;

        }

        var getAnonimusPostsByUserId = function (userId, start, end) {

            var data = $http.get("/Posts/GetAnonimusPostsByUserId?postUserId=" + userId + "&userId=" + _userId + "&start=" + start + "&end=" + end).then(function (d) {
                return d.data;
            });

            return data;

        }

        var getSavedPostsByUserId = function (userId, start, end) {

            var data = $http.get("/Posts/GetSavedPostsByUserId?postUserId=" + userId + "&userId=" + _userId + "&start=" + start + "&end=" + end).then(function (d) {
                return d.data;
            });

            return data;

        }

        var ratePost = function (postId, rate) {

            var data = $http.get("/Posts/Rate?userId=" + _userId + "&postId=" + postId + "&rate=" + rate).then(function (d) {
                return d.data;
            });

            return data;

        }

        var addComment = function (comment, file) {

            var fd = new FormData();

            var url = '/Posts/AddComment';

            fd.append('file', file);

            fd.append("data", JSON.stringify(comment));

            var data = $http.post(url, fd, {
                withCredentials: false,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function (d) {
                return d.data;
            });

            return data;

        }

        var savePost = function (postId) {

            var data = $http.get("/Posts/Save?userId=" + _userId + "&postId=" + postId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var sharePost = function (postId) {

            var data = $http.get("/Posts/Share?userId=" + _userId + "&postId=" + postId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var deletePost = function (postId) {

            var data = $http.get("/Posts/DeletePost?userId=" + _userId + "&id=" + postId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var deleteComment = function (commentId) {

            var data = $http.get("/Posts/DeleteComment?userId=" + _userId + "&id=" + commentId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var hidePost = function (postId) {

            var data = $http.get("/Posts/Hide?userId=" + _userId + "&postId=" + postId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var reportPost = function (postId) {

            var data = $http.get("/Posts/Report?userId=" + _userId + "&postId=" + postId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var getPostById = function (postId) {

            var data = $http.get("/Posts/GetPostById?userId=" + _userId + "&postId=" + postId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var getPostComments = function (postId, start,end) {

            var data = $http.get("/Posts/GetPostComments?userId=" + _userId + "&postId=" + postId+"&start="+start+"&end="+end).then(function (d) {
                return d.data;
            });

            return data;

        }

        var getNewComment = function (postId, lastId) {

            var data = $http.get("/Posts/GetNewComment?userId=" + _userId + "&postId=" + postId + "&lastId=" + lastId).then(function (d) {
                return d.data;
            });

            return data;

        }

        var getPostRatesByPostId = function (postId, start) {

            var data = $http.get("/Posts/GetPostRatesByPostId?userId=" + _userId + "&postId=" + postId + "&start=" + start).then(function (d) {
                return d.data;
            });

            return data;

        }
       
        return {

            addPost: addPost,

            getPosts: getPosts,

            ratePost: ratePost,

            addComment: addComment,

            savePost: savePost,

            sharePost: sharePost,

            getPostsByInterest: getPostsByInterest,

            deletePost: deletePost,

            deleteComment: deleteComment,

            hidePost: hidePost,

            getPostById: getPostById,
            getPostComments:getPostComments,

            reportPost: reportPost,
            getPostsByUserId: getPostsByUserId,
            getAnonimusPostsByUserId: getAnonimusPostsByUserId,
            getSavedPostsByUserId: getSavedPostsByUserId,
            getNewComment: getNewComment,

            uploadTemp: uploadTemp,
            getPostRatesByPostId:getPostRatesByPostId
        };
    };

    angular.module('app')
        .factory('postsService', postsService);
})();