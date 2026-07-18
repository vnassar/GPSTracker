let map;
let marker;

function initMap(elementId) {
map = L.map(elementId).setView([0, 0], 2);

L.tileLayer("https://tile.openstreetmap.org/{z}/{x}/{y}.png", {
attribution: "© OpenStreetMap contributors"
}).addTo(map);

marker = L.marker([0, 0]).addTo(map);
}

function updateMarker(latitude, longitude) {
if (!map || !marker) return;

marker.setLatLng([latitude, longitude]);
map.setView([latitude, longitude], map.getZoom() < 13 ? 13 : map.getZoom());
}

window.initMap = initMap;
window.updateMarker = updateMarker;

window.initMap = initMap;
window.updateMarker = updateMarker;
window.centerMap = centerMap;