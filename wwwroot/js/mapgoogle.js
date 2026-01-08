let map;
let destinationMarker = null;
let parkingMarkers = {};
let pendingTarget = null;
let maxWalkTime = 10;
let autocomplete;

const USER_LOCATION = {
  lat: 38.722282,
  lon: -9.135389,
};

// =======================
// HARİTA
// =======================
function initMap() {
  map = new google.maps.Map(document.getElementById("map"), {
    center: { lat: 38.729062, lng: -9.145312 },
    zoom: 14,
  });

  // GOOGLE SEARCH
  const input = document.getElementById("placeSearch");
  autocomplete = new google.maps.places.Autocomplete(input, {
    fields: ["geometry", "name"],
  });

  autocomplete.addListener("place_changed", () => {
    const place = autocomplete.getPlace();
    if (!place.geometry) return;

    const loc = place.geometry.location;

    pendingTarget = {
      lat: loc.lat(),
      lon: loc.lng(),
    };

    clearRecommendations();

    if (destinationMarker) destinationMarker.setMap(null);

    destinationMarker = new google.maps.Marker({
      position: loc,
      map,
      icon: "http://maps.google.com/mapfiles/ms/icons/red-dot.png",
    });

    map.panTo(loc);
    map.setZoom(15);

    document.getElementById("status-label").innerText = place.name;
    document.getElementById("searchBtn").classList.remove("d-none");
  });

  // MAP CLICK
  map.addListener("click", (e) => {
    pendingTarget = {
      lat: e.latLng.lat(),
      lon: e.latLng.lng(),
    };

    clearRecommendations();

    if (destinationMarker) destinationMarker.setMap(null);

    destinationMarker = new google.maps.Marker({
      position: e.latLng,
      map,
      icon: "http://maps.google.com/mapfiles/ms/icons/red-dot.png",
    });

    document.getElementById("status-label").innerText = "Seçilen nokta";
    document.getElementById("searchBtn").classList.remove("d-none");
  });
}

// =======================
// SLIDER
// =======================
document.getElementById("walkSlider").addEventListener("input", (e) => {
  maxWalkTime = e.target.value;
  document.getElementById("walkValue").innerText = `${maxWalkTime} dk`;
});

// =======================
// API
// =======================
document.getElementById("searchBtn").addEventListener("click", searchParking);

async function searchParking() {
  if (!pendingTarget) return;

  const res = await fetch("http://localhost:8000/recommend", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      target_lat: pendingTarget.lat,
      target_lon: pendingTarget.lon,
      user_lat: USER_LOCATION.lat,
      user_lon: USER_LOCATION.lon,
      max_walk_time: parseInt(maxWalkTime),
    }),
  });

  const data = await res.json();
  renderRecommendations(data);
}

// =======================
// SOL PANEL
// =======================
function renderRecommendations(data) {
  const panel = document.getElementById("recommendationList");
  panel.innerHTML = "";

  data.all_parkings.forEach((p, index) => {
    const best = p.park_id === data.recommended_parking.park_id;

    panel.innerHTML += `
      <div class="card mb-2 ${best ? "border-success" : ""}"
           onclick="focusParking('${p.park_id}')"
           style="cursor:pointer">
        <div class="card-body p-2">
          <b>${index + 1}. ${p.park_id}</b><br>
          <small>
            Doluluk: ${Math.round(p.occupancy_ratio * 100)}%<br>
            Yürüme: ${p.walk_min} dk
          </small>
        </div>
      </div>
    `;
  });

  renderRecommendationMarkers(data);
}

// =======================
// MARKER
// =======================
function renderRecommendationMarkers(data) {
  Object.values(parkingMarkers).forEach((m) => m.setMap(null));
  parkingMarkers = {};

  data.all_parkings.forEach((p) => {
    let color = "blue";
    if (p.park_id === data.recommended_parking.park_id) color = "yellow";

    const marker = new google.maps.Marker({
      position: { lat: p.latitude, lng: p.longitude },
      map,
      icon: `http://maps.google.com/mapfiles/ms/icons/${color}-dot.png`,
    });

    parkingMarkers[p.park_id] = marker;
  });
}

// =======================
// PANEL TIKLAMA
// =======================
function focusParking(parkId) {
  const marker = parkingMarkers[parkId];
  if (!marker) return;

  const pos = marker.getPosition();

  pendingTarget = {
    lat: pos.lat(),
    lon: pos.lng(),
  };

  map.panTo(pos);
  map.setZoom(16);

  document.getElementById("searchBtn").classList.remove("d-none");

  Object.values(parkingMarkers).forEach((m) =>
    m.setIcon("http://maps.google.com/mapfiles/ms/icons/blue-dot.png")
  );

  marker.setIcon("http://maps.google.com/mapfiles/ms/icons/green-dot.png");
}

// =======================
// TEMİZLE
// =======================
function clearRecommendations() {
  document.getElementById("recommendationList").innerHTML = `
    <div class="text-muted text-center mt-5">
      Yeni bir nokta seç
    </div>
  `;

  Object.values(parkingMarkers).forEach((m) => m.setMap(null));
  parkingMarkers = {};
}

window.onload = initMap;
