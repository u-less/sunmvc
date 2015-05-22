//周泽辉
//2015.2.9
(function ($) {
    var methods = {
        init: function (options) {
            var config = { pageSize: 10, recordCount: 0, currentPage: 0, callback: function (page) {} };
            var conf = $.extend({}, config, options);
            BuildPage($(this), conf.recordCount, conf.pageSize, conf.currentPage, conf.callback);
        }
    };
    function BuildPage(container, recordCount, pageSize, currentPage, callback) {
        var maxShowPages = 8, pageCls = "page_wraper", currentPageCls = "page_current", prePageCls = "page_pre", nextPageCls = "page_next";
        var pages = Math.ceil(recordCount * 1.0 / pageSize);
        var prePage = currentPage - 1;
        var nextPage = currentPage + 1;
        var result = '';
        if (pages > 1) {
            result = result.concat('<div class="{0}">'.Format(pageCls));
            if (currentPage > 1) {
                result = result.concat('<a class="{0}" title="上一页" data-page="{1}">上一页<b></b></a>'.Format(prePageCls, currentPage - 1));
                result = result.concat('<a title="第1页" data-page="1">1</a>');
            }
            else {
                result = result.concat('<span class="prev-disabled">上一页<b></b></span>');
                result = result.concat('<a title="第1页" class="{0}">1</a>'.Format(currentPageCls));
            }
            if (pages <= maxShowPages + 1)//当总页数小于展示页数
            {
                for (var i = 2; i < pages; i++) {
                    if (currentPage != i)
                        result = result.concat('<a title="第{0}页" data-page="{0}">{0}</a>'.Format(i));
                    else
                        result = result.concat('<a title="第{0}页" class="{1}" data-page="{0}">{0}</a>'.Format(i, currentPageCls));
                }
            }
            else {
                if (currentPage > maxShowPages - 2 && currentPage < pages - maxShowPages + 3) {
                    var avg = (maxShowPages - 2) / 2.0;
                    var prec = Math.ceil(avg);//前面数量
                    var nextc = Math.floor(avg);//后面数量
                    var start = currentPage - prec;
                    var end = currentPage + nextc;
                    result = result.concat('<a title="前面分页" data-page="{0}">…</a>'.Format(start - 1));
                    for (var i = start; i <= end; i++) {
                        if (currentPage != i)
                            result = result.concat('<a title="第{0}页" data-page="{0}">{0}</a>'.Format(i));
                        else
                            result = result.concat('<a title="第{0}页" class="{1}" data-page="{0}">{0}</a>'.Format(i, currentPageCls));
                    }
                    result = result.concat('<a  title="后面分页" data-page="{0}">…</a>'.Format(end + 1));
                }
                else {
                    if (currentPage < maxShowPages - 1) {
                        for (var i = 2; i < maxShowPages; i++) {
                            if (currentPage != i)
                                result = result.concat('<a title="第{0}页" data-page="{0}">{0}</a>'.Format(i));
                            else
                                result = result.concat('<a title="第{0}页" class="{1}" data-page="{0}">{0}</a>'.Format(i, currentPageCls));
                        }
                        result = result.concat('<a  title="后面分页" data-page="{0}">…</a>'.Format(maxShowPages));
                    }
                    else {
                        var start = pages - (maxShowPages - 2);
                        result = result.concat('<a title="前面分页" data-page="{0}">…</a>'.Format(start - 1));
                        for (var i = start; i <= pages - 1; i++) {
                            if (currentPage != i)
                                result = result.concat('<a title="第{0}页" data-page="{0}">{0}</a>'.Format(i));
                            else
                                result = result.concat('<a title="第{0}页" class="{1}" data-page="{0}">{0}</a>'.Format(i, currentPageCls));
                        }
                    }
                }
            }
            if (currentPage < pages) {
                result = result.concat('<a title="第{0}页" data-page="{0}">{0}</a>'.Format(pages));
                result = result.concat('<a class="{0}" title="下一页" data-page="{1}">下一页<b></b></a>'.Format(nextPageCls, currentPage + 1));
            }
            else {
                if (pages != 1)
                    result = result.concat('<a title="第{1}页" class="{0}" data-page="{1}">{1}</a>'.Format(currentPageCls, currentPage));
                result = result.concat('<span class="next-disabled">下一页<b></b></span>');
            }
            result = result.concat("</div>");
        }
        container.html(result);
        $("a", container).click(function () {
            var page = $(this).attr("data-page");
            if (page != undefined && parseInt(page) != 0) {
                $("a", container).unbind();
                callback(parseInt(page));
                BuildPage(container, recordCount, pageSize, parseInt(page), callback);
            }
        });
    }
    $.fn.sunpage = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            throw new Error("调用方法不存在");
        };
    };
})(jQuery);