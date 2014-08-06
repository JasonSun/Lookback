/*
    操作高德地图的相关函数
*/

// 初始化地图对象
function initMap() {
    var mapObj = new AMap.Map("map-container", {
        view: new AMap.View2D({
            zoom: 3
        })
    });

    return mapObj;
}

// 设置地图上默认鼠标样式
function setMapMouseStyle(mapObj) {
    
    mapObj.setDefaultCursor("url(/Images/openhand.cur), pointer");

    // 通过地图的dragstart、dragging、dragend事件切换鼠标拖拽地图过程中的不同样式
    AMap.event.addListener(mapObj, 'dragstart', function (e) {
        mapObj.setDefaultCursor("url(/Images/closedhand.cur), pointer");
    });
    AMap.event.addListener(mapObj, 'dragging', function (e) {
        mapObj.setDefaultCursor("url(/Images/closedhand.cur), pointer");
    });
    AMap.event.addListener(mapObj, 'dragend', function (e) {
        mapObj.setDefaultCursor("url(/Images/openhand.cur), pointer");
    });
}

// 添加地图标记
function addMarker(lng, lat, mapObj) {
    var marker = new AMap.Marker({
        position: new AMap.LngLat(lng, lat),
        offset: new AMap.Pixel(-10, -34),
        icon: "/Images/marker.png"
    });

    return marker;
}

// 添加地图的信息窗口
function addInfoWindow(mapObj, sid, title, text, thumbnail, date) {
    var window = new AMap.InfoWindow({
        isCustom: true,
        content: customInfoWindow(mapObj, sid, title, text, thumbnail, date),
        offset: new AMap.Pixel(16, -45)
    });

    return window;
}

// 自定义地图的信息窗口
function customInfoWindow(mapObj, sid, title, text, thumbnail, date) {
    var info = document.createElement("div");
    info.className = "info";

    // 自定义信息窗口的高度
    //info.style.width = "400px";

    // 自定义信息窗口的顶部标题栏
    var top = document.createElement("div");
    top.className = "info-top";

    var titleNode = document.createElement("div");
    titleNode.innerHTML = "<b>" + title + "</b>";
    var closeX = document.createElement("img");
    closeX.src = "/Images/close.gif";
    closeX.addEventListener("click", function () {
        closeInfoWindow(mapObj)
    });
    
    top.appendChild(titleNode);
    top.appendChild(closeX);
    info.appendChild(top);

    // 自定义信息窗口的中部内容栏
    var middle = document.createElement("div");
    middle.className = "info-middle clearfix";
    middle.style.backgroundColor = "white";
    var content;
    if (thumbnail == '') {
        content = "感受：" + text +
            "<br><br>记录时间：" + date;
    } else {
        content = "<div class='col-sm-4'>" +
            "<img width=70px; height=70px; src=" + thumbnail + ">" + "</img>" +
            "</div>" +
            "<div class='col-sm-8'>" +
            "感受：" + text +
            "<br><br>记录时间：" + date +
            "<a class='more-pics' style='margin-left: 1em;' data-id=" + sid + ">详情</a>"
            "</div>";
    }
    
    middle.innerHTML = content;
    info.appendChild(middle);

    // 自定义底部内容栏
    var bottom = document.createElement("div");
    bottom.className = "info-bottom";
    bottom.style.position = "relative";
    bottom.style.top = "0px";
    bottom.style.margin = "0 auto";
    var sharp = document.createElement("img");
    sharp.src = "/Images/sharp.png";
    bottom.appendChild(sharp);
    info.appendChild(bottom);

    return info;
}

function closeInfoWindow(mapObj) {
    mapObj.clearInfoWindow();
}
