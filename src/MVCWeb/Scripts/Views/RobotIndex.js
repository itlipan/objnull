
//NewBeePage
function GitHubEventPage(index) {
    var pageSize = parseInt($("#ValPageSize").val());
    var postData = { pageSize: pageSize, pageNum: index }
    $.ajax({
        url: "/Robot/GitHubEventPage",
        type: "Post",
        data: postData,
        success: function (result) {
            $("#GitHubEventPage").html(result);
            var total = parseInt($("#TotalCount").val());
            if (total <= pageSize) {
                return;
            }
            $("#EventPager").pagination({
                items: total,
                itemsOnPage: pageSize,
                currentPage: $("#CurrentPage").val(),
                prevText: "<",
                nextText: ">",
                listStyle: "pagination pagination",
                onPageClick: function (pageNumber, event) {
                    GitHubEventPage(pageNumber);
                }
            });
        }
    });
}

$(function () {
    GitHubEventPage(1);
});
