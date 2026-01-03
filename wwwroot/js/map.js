let map;
let targetLat, targetLng;
let userLat, userLng;

function initMap() {
  map = new google.maps.Map(document.getElementById("map"), {
    center: { lat: 38.4237, lng: 27.1428 },
    zoom: 14,
  });

  map.addListener("click", (e) => {
    targetLat = e.latLng.lat();
    targetLng = e.latLng.lng();

    new google.maps.Marker({
      position: e.latLng,
      map,
      label: "🎯",
    });
  });

  navigator.geolocation.getCurrentPosition((p) => {
    userLat = p.coords.latitude;
    userLng = p.coords.longitude;
  });
}

async function recommend() {
  const res = await fetch("/Recommendation/Recommend", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      target_lat: targetLat,
      target_lon: targetLng,
      user_lat: userLat,
      user_lon: userLng,
      max_walk_time: 15,
    }),
  });

  const data = await res.json();
  const p = data.recommended_parking;

  new google.maps.Marker({
    position: { lat: p.latitude, lng: p.longitude },
    map,
    label: "⭐",
    title: `Doluluk: %${p.occupancy_ratio * 100}`,
  });
}

window.onload = initMap;
