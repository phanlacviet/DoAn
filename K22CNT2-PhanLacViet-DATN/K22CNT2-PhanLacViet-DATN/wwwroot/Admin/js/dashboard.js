document.addEventListener("DOMContentLoaded", function () {
    const ctx = document.getElementById('viewChart').getContext('2d');

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: chartLabels, 
            datasets: [{
                label: 'Lượt xem',
                data: chartData,
                fill: true,
                backgroundColor: 'rgba(78, 115, 223, 0.05)',
                borderColor: 'rgba(78, 115, 223, 1)',
                pointRadius: 3,
                pointBackgroundColor: 'rgba(78, 115, 223, 1)',
                pointBorderColor: 'rgba(78, 115, 223, 1)',
                tension: 0.3
            }]
        },
        options: {
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) { return value.toLocaleString(); }
                    }
                }
            },
            plugins: {
                legend: { display: false }
            }
        }
    });
});