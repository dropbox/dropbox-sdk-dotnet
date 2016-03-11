//$(function () {
//    var d = new Date()
//    var dataList = [];

//    for (var i = 6; i >= 0; i--) {
//        var map = {};
//        map["y"] = (d.getMonth() + 1) + "/" + (d.getDate() - i) + "/" + d.getFullYear();
//        map["a"] = 100;
//        dataList.push(map);
//    }

//    var dataInput = JSON.parse(JSON.stringify(dataList));

//    Morris.Bar({
//        element: 'morris-bar-chart',
//        data: dataInput,
//        xkey: 'y',
//        ykeys: ['a'],
//        labels: ['Number Users'],
//        hideHover: 'auto',
//        resize: true
//    });

//});
