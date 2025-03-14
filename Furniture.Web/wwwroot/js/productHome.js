$(document).ready(function () {
    $("#button-search").click(function (event) {
        event.preventDefault(); 

        var searchText = $("#searchInput").val().trim();
        if (searchText) {
            var searchUrl = "/Product/SearchProducts?SearchText=" + encodeURIComponent(searchText);
            window.location.href = searchUrl;
        }
        
    });
    $(".pagination-link").click(function (event) {
        event.preventDefault();

        var searchText = $("#searchInput").val().trim(); 
        var pageIndex = $(this).data("page"); 

        var url;
        if (searchText) {
            url = "/Product/SearchProducts?SearchText=" + encodeURIComponent(searchText) + "&pageIndex=" + pageIndex;
        } else {
            url = "/Product/ProductHome?pageIndex=" + pageIndex;
        }

        window.location.href = url;
    });
});