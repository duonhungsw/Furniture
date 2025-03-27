$(document).ready(function () {
    function filterProducts(pageIndex = 1) {
        var selectedBrands = [];
        var selectedTypes = [];
        var searchText = $("#searchInput").val().trim();
        var orderBy = $(".sort-by").val();

        $(".type-checkbox:checked").each(function () {
            selectedTypes.push($(this).val());
        });
        $(".brand-checkbox:checked").each(function () {
            selectedBrands.push($(this).val());
        });

        $.ajax({
            url: "/Product/FilterProducts",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                brands: selectedBrands,
                types: selectedTypes,
                searchText: searchText,
                pageIndex: pageIndex,
                orderBy: orderBy
            }),
            success: function (response) {
                $("#productList").html("");
                $(".pagination").html("");
                if (response.items.length > 0) {
                    response.items.forEach(product => {
                        $("#productList").append(`
                            <div class="col-12 col-md-4 col-lg-3 mb-5">
                                <div class="product-item" href="#">
                                    <a href="/Product/ProductDetail/${product.id}">
                                        <img src="${product.pictureUrl.split(',')[0]}" class="img-fluid product-thumbnail">
                                    </a>

                                    <h3 class="product-title">${product.name}</h3>
                                    <strong class="product-price">${product.price.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</strong>

                                    <span class="icon-cross">
                                       <img src="/images/cross.svg" class="img-fluid">
                                    </span>
                                </div>
                            </div>
                        `);
                    });

                    let paginationHtml = "";
                    for (let i = 1; i <= response.totalPages; i++) {
                        paginationHtml += `
                        <li class="page-item ${i === response.pageIndex ? "active" : ""}">
                            <a class="page-link pagination-link" data-page="${i}" href="#">${i}</a>
                        </li>
                    `;
                    }
                    $(".pagination").html(paginationHtml);
                }
                else {
                    $("#productList").append(`
                        <p class="text-center">No data found 😢</p>
                    `);
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching products:", error);
            }
        });
    }

    $(".sort-by").change(function () {
        filterProducts(1);
    });
    
    $("#button-search").click(function (event) {
        event.preventDefault();

        var searchText = $("#searchInput").val().trim();
        if (searchText) {
            var searchUrl = "/Product/SearchProducts?SearchText=" + encodeURIComponent(searchText);
            window.location.href = searchUrl;
        }
    });

    $(document).on('click', '.pagination-link', function (event) {
        event.preventDefault();
        var pageIndex = $(this).data("page");
        filterProducts(pageIndex); 
    });

    function checkSelectedFilters() {
        if ($('input[type="checkbox"]:checked').length > 0) {
            $("#reset-filter").show();
        } else {
            $("#reset-filter").hide();
        }
    }
    $(document).on('click', '#reset-filter', function () {
        $('input[type="checkbox"]').prop('checked', false); 
        $("#reset-filter").hide();
        filterProducts(1); 
    });

    $(".brand-checkbox, .type-checkbox").change(function () {
        checkSelectedFilters();
        filterProducts(1); 
    });
    checkSelectedFilters();
});
