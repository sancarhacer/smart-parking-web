let map;
let userMarker;
let parkingMarkers = [];
let targetMarker;

// ---------------- HARİTA ----------------
map = L.map("map").setView([38.4237, 27.1428], 13); // İzmir default

L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
  attribution: "© OpenStreetMap",
}).addTo(map);

// ---------------- KULLANICI KONUMU ----------------
navigator.geolocation.getCurrentPosition(
  (pos) => {
    // const lat = pos.coords.latitude;
    // const lon = pos.coords.longitude;
    const lat = 38.7247866;
    const lon = -9.151042999;

    window.userLocation = { lat, lon };

    userMarker = L.marker([lat, lon])
      .addTo(map)
      .bindPopup("Konumunuz")
      .openPopup();

    map.setView([lat, lon], 14);
  },
  () => alert("Konum alınamadı")
);

// ---------------- ARAMA (NOMINATIM) ----------------
document.getElementById("searchBox").addEventListener("keydown", async (e) => {
  if (e.key !== "Enter") return;

  const query = e.target.value;
  const res = await fetch(
    `https://nominatim.openstreetmap.org/search?format=json&q=${query}`
  );
  const data = await res.json();

  if (!data.length) {
    alert("Konum bulunamadı");
    return;
  }

  const lat = parseFloat(data[0].lat);
  const lon = parseFloat(data[0].lon);

  map.setView([lat, lon], 15);
  getRecommendations(lat, lon);
});

// ---------------- HARİTAYA TIKLAMA ----------------
map.on("click", (e) => {
  getRecommendations(e.latlng.lat, e.latlng.lng);
});

// ---------------- FASTAPI ÇAĞRISI ----------------
async function getRecommendations(targetLat, targetLon) {
  if (!window.userLocation) {
    alert("Kullanıcı konumu alınamadı");
    return;
  }

  clearMarkers();

  const body = {
    target_lat: targetLat,
    target_lon: targetLon,
    user_lat: window.userLocation.lat,
    user_lon: window.userLocation.lon,
    max_walk_time: 15,
  };

  const res = await fetch("http://localhost:8000/recommend", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  });

  const data = await res.json();
  showResults(data.all_parkings, targetLat, targetLon);
}

// ---------------- SONUÇLARI GÖSTER ----------------
function showResults(parkings, targetLat, targetLon) {
  const list = document.getElementById("parkingList");
  list.innerHTML = "";

  targetMarker = L.circleMarker([targetLat, targetLon], {
    radius: 8,
    color: "blue",
  })
    .addTo(map)
    .bindPopup("Hedef Nokta")
    .openPopup();

  parkings.forEach((p) => {
    const li = document.createElement("li");
    li.className = "list-group-item";
    li.innerHTML = `
            <b>${p.park_id}</b><br/>
            Doluluk: ${p.occupancy_ratio}<br/>
            Yürüme: ${p.walk_min} dk<br/>
            Sürüş: ${p.duration_min} dk
        `;
    list.appendChild(li);

    const marker = L.marker([p.latitude, p.longitude])
      .addTo(map)
      .bindPopup(`Otopark: ${p.park_id}`);

    parkingMarkers.push(marker);
  });
}

// ---------------- TEMİZLE ----------------
function clearMarkers() {
  parkingMarkers.forEach((m) => map.removeLayer(m));
  parkingMarkers = [];

  if (targetMarker) {
    map.removeLayer(targetMarker);
    targetMarker = null;
  }
}
