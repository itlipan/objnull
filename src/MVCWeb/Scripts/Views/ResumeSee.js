
$(function () {
    var doc = new jsPDF();
    $("#BtnPDF").click(function () {
        doc.fromHTML($('#MDValue').get(0), 15, 15, {
            'width': 170
        });
        doc.save('简历.pdf')
    });
});
