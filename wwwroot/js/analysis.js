const select = document.getElementById("parkingSelect");
const ctx = document.getElementById("chart");

let chart;

select.addEventListener("change", loadData);
loadData();

async function loadData() {
  const parkId = select.value;

  const res = await fetch(`/Analysis/Get24HourPrediction?parkId=${parkId}`);

  if (!res.ok) {
    const text = await res.text();
    console.error("Backend error:", text);
    alert("Sunucu hatası, console'a bak");
    return;
  }

  const data = await res.json();
  renderChart(data);
  renderStats(data);
}

function renderChart(data) {
  if (chart) chart.destroy();

  chart = new Chart(ctx, {
    type: "line",
    data: {
      labels: data.map((d) => d.time),
      datasets: [
        {
          data: data.map((d) => d.ratio),
          borderColor: "#0D47A1",
          backgroundColor: "rgba(13,71,161,0.2)",
          fill: true,
          tension: 0.4,
        },
      ],
    },
    options: {
      scales: {
        y: {
          min: 0,
          max: 100,
          ticks: { callback: (v) => `%${v}` },
        },
      },
    },
  });
}

function renderStats(data) {
  const peak = data.reduce((a, b) => (a.ratio > b.ratio ? a : b));
  const empty = data.reduce((a, b) => (a.ratio < b.ratio ? a : b));
  const avg = (data.reduce((s, d) => s + d.ratio, 0) / data.length).toFixed(0);

  document.getElementById("peak").innerText = peak.time;
  document.getElementById("empty").innerText = empty.time;
  document.getElementById("avg").innerText = avg;
}
