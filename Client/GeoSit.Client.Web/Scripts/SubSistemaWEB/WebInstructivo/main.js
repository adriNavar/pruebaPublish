$(document).ready(initWebInstructivo);
$(window).resize(ajustarmodalWebInstructivo);
$('#modal-window-webinstructivo').on('shown.bs.modal', function (e) {
    ajustarScrollBarsWebInstructivo();
    hideLoading();
});
function initWebInstructivo() {
    ///////////////////// Scrollbars ////////////////////////
    $(".webinstructivo-content").niceScroll(getNiceScrollConfig());
    $('#scroll-content-webinstructivo .panel-body').resize(ajustarScrollBarsWebInstructivo);
    $('.webinstructivo-content .panel-heading').click(function () {
        setTimeout(function () {
            $(".webinstructivo-content").getNiceScroll().resize();
        }, 10);
    });
    ////////////////////////////////////////////////////////

    myTable = $('#Grilla_WebInstructivo').dataTable({
        "scrollY": "100%",
        "scrollCollapse": true,
        "paging": false,
        "searching": false,
        "bInfo": false,
        "aaSorting": [[1, 'asc']],
        "language": { "url": BASE_URL + "Scripts/dataTables.spanish.txt" },
        "columnDefs": [
            { "visible": false, "targets": 1 }
        ],
        "drawCallback": function () {
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


    ajustarmodalWebInstructivo();
    ///////////////////// Tooltips /////////////////////////
    $('#modal-window-webinstructivo .tooltips').tooltip({ container: 'body' });
    ////////////////////////////////////////////////////////
    $("#modal-window-webinstructivo").modal('show');

    //$('#Grilla_WebInstructivo').dataTable().fnFakeRowspan(1);
};
function ajustarmodalWebInstructivo() {
    var viewportHeight = $(window).height(),
        headerFooter = 190,
        altura = viewportHeight - headerFooter; //value corresponding to the modal heading + footer
    $(".webinstructivo-body", "#scroll-content-webinstructivo").css({ "height": altura, "overflow": "hidden" });

    var NewHeight = $(document).height() - 260;
    var oSettings = myTable.fnSettings();
    oSettings.oScroll.sY = (altura - 100) + "px";
    myTable.fnDraw();
    ajustarScrollBarsWebInstructivo();
}
function ajustarScrollBarsWebInstructivo() {
    $('.webinstructivo-content').collapse('show');

    temp = $(".webinstructivo-body").height();

    var outerHeight = 20;

    $('#accordion-webinstructivo .collapse').each(function () {
        outerHeight += $(this).outerHeight();
    });
    $('#accordion-webinstructivo .panel-heading').each(function () {
        outerHeight += $(this).outerHeight();
    });
    temp = Math.min(outerHeight, temp);
    $('.webinstructivo-content').css({ "max-height": temp + 'px' })
    $('#accordion-webinstructivo').css({ "max-height": temp + 1 + 'px' })
    $(".webinstructivo-content").getNiceScroll().resize();
    $(".webinstructivo-content").getNiceScroll().show();

    //--GridFix_ temporal(Mantis: 5782)
    $('.dataTables_scrollHeadInner').css('width', '100%');
    $('table[role="grid"]').css('width', '100%');
    //--
}


//@ sourceURL=WebInstructivo.js