$(document).ready(initWebLinks);
$(window).resize(ajustarmodalWebLinks);
$('#modal-window-weblinks').on('shown.bs.modal', function (e) {
    ajustarScrollBarsWebLinks();
    hideLoading();
});
function initWebLinks() {
    ///////////////////// Scrollbars ////////////////////////
    $(".weblinks-content").niceScroll(getNiceScrollConfig());
    $('#scroll-content-weblinks .panel-body').resize(ajustarScrollBarsWebLinks);
    $('.weblinks-content .panel-heading').click(function () {
        setTimeout(function () {
            $(".weblinks-content").getNiceScroll().resize();
        }, 10);
    });
    ////////////////////////////////////////////////////////

    myTable = $('#Grilla_WebLinks').dataTable({
        "scrollY": "100%",
        "width": "100%",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "bInfo": false,
        "aaSorting": [[1, 'asc']],
        "language": { "url": BASE_URL + "Scripts/dataTables.spanish.txt" },
        "columnDefs": [
            { "visible": false, "targets": 1 }
        ],
        "drawCallback": function (settings) {
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var last = null;
            api.column(1, { page: 'current' }).data().each(function (group, i) {
                if (last !== group) {
                    $(rows).eq(i).before(
                        '<tr class="group"><td colspan="5">' + group + '</td></tr>'
                    );

                    last = group;
                }
            });
        }
    });

    ajustarmodalWebLinks();
    ///////////////////// Tooltips /////////////////////////
    $('#modal-window-weblinks .tooltips').tooltip({ container: 'body' });
    ////////////////////////////////////////////////////////
    $("#modal-window-weblinks").modal('show');
};
function ajustarmodalWebLinks() {
    var viewportHeight = $(window).height(),
        headerFooter = 190,
        altura = viewportHeight - headerFooter; //value corresponding to the modal heading + footer
    $(".weblinks-body", "#scroll-content-weblinks").css({ "height": altura, "overflow": "hidden" });

    var NewHeight = $(document).height() - 260;
    var oSettings = myTable.fnSettings();
    oSettings.oScroll.sY = (altura - 100) + "px";
    myTable.fnDraw();

    ajustarScrollBarsWebLinks();
}
function ajustarScrollBarsWebLinks() {
    $('.weblinks-content').collapse('show');
    temp = $(".weblinks-body").height();
    var outerHeight = 20;
    $('#accordion-weblinks .collapse').each(function () {
        outerHeight += $(this).outerHeight();
    });
    $('#accordion-weblinks .panel-heading').each(function () {
        outerHeight += $(this).outerHeight();
    });
    temp = Math.min(outerHeight, temp);
    $('.weblinks-content').css({ "max-height": temp + 'px' })
    $('#accordion-weblinks').css({ "max-height": temp + 1 + 'px' })
    $(".weblinks-content").getNiceScroll().resize();
    $(".weblinks-content").getNiceScroll().show();

    //--GridFix_ temporal(Mantis: 5781)
    $('.dataTables_scrollHeadInner').css('width', '100%');
    $('table[role="grid"]').css('width', '100%');
    //--
}



//@ sourceURL=WebLinks.js