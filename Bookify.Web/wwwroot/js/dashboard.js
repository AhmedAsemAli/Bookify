var chart;

$(document).ready(function () {
    
    $('[data-kt-daterangepicker="true"]').on('apply.daterangepicker', function (ev, picker) {
        chart.destroy();
        var startDate = picker.startDate.format('MM/DD/YYYY');
        var endDate = picker.endDate.format('MM/DD/YYYY');
        drawRentalsChart(startDate, endDate);
    });
});

drawRentalsChart();
drawSubscribersChart();

function drawRentalsChart(startDate = null, endDate = null) {
    var element = document.getElementById('RentalsPerDay');

    if (!element) {
        return;
    }

    var height = parseInt(KTUtil.css(element, 'height'));
    var labelColor = KTUtil.getCssVariableValue('--kt-gray-500');
    var borderColor = KTUtil.getCssVariableValue('--kt-gray-200');
    var baseColor = KTUtil.getCssVariableValue('--kt-info');
    var lightColor = KTUtil.getCssVariableValue('--kt-info-light');

    $.get({
        url: `/Dashboard/GetRentalsPerDay?startDate=${startDate}&endDate=${endDate}`,
        success: function (data) {

            var numericValues = data.map(i => parseInt(i.value));
            var maxValue = Math.max(...numericValues);

            var options = {
                series: [{
                    name: 'Books',
                    data: numericValues
                }],
                chart: {
                    fontFamily: 'inherit',
                    type: 'area',
                    height: height,
                    toolbar: {
                        show: false
                    }
                },
                plotOptions: {},
                legend: {
                    show: false
                },
                dataLabels: {
                    enabled: false
                },
                fill: {
                    type: 'solid',
                    opacity: 1
                },
                stroke: {
                    curve: 'smooth',
                    show: true,
                    width: 3,
                    colors: [baseColor]
                },
                xaxis: {
                    categories: data.map(i => i.label),
                    axisBorder: {
                        show: false,
                    },
                    axisTicks: {
                        show: false
                    },
                    labels: {
                        style: {
                            colors: labelColor,
                            fontSize: '12px'
                        }
                    },
                    crosshairs: {
                        position: 'front',
                        stroke: {
                            color: baseColor,
                            width: 1,
                            dashArray: 3
                        }
                    },
                    tooltip: {
                        enabled: true,
                        formatter: undefined,
                        offsetY: 0,
                        style: {
                            fontSize: '12px'
                        }
                    }
                },
                yaxis: {
                    tickAmount: maxValue > 0 ? maxValue : 1,
                    min: 0,
                    labels: {
                        style: {
                            colors: labelColor,
                            fontSize: '12px'
                        }
                    }
                },
                states: {
                    normal: {
                        filter: { type: 'none', value: 0 }
                    },
                    hover: {
                        filter: { type: 'none', value: 0 }
                    },
                    active: {
                        allowMultipleDataPointsSelection: false,
                        filter: { type: 'none', value: 0 }
                    }
                },
                tooltip: {
                    style: {
                        fontSize: '12px'
                    }
                },
                colors: [lightColor],
                grid: {
                    borderColor: borderColor,
                    strokeDashArray: 4,
                    yaxis: {
                        lines: {
                            show: true
                        }
                    }
                },
                markers: {
                    strokeColor: baseColor,
                    strokeWidth: 3
                }
            };

            chart = new ApexCharts(element, options);
            chart.render();
        }
    });
}

function drawSubscribersChart() {
    $.get({
        url: '/Dashboard/GetSubscribersPerCity',
        success: function (figures) {
            var ctx = document.getElementById('SubscribersPerCity');

            var primaryColor = KTUtil.getCssVariableValue('--kt-primary');
            var dangerColor = KTUtil.getCssVariableValue('--kt-danger');
            var successColor = KTUtil.getCssVariableValue('--kt-success');
            var warningColor = KTUtil.getCssVariableValue('--kt-warning');
            var infoColor = KTUtil.getCssVariableValue('--kt-info');
            var fontFamily = KTUtil.getCssVariableValue('--bs-font-sans-serif');

            const data = {
                labels: figures.map(f => f.label),
                datasets: [{
                    data: figures.map(f => parseInt(f.value)),
                    backgroundColor: [
                        infoColor,
                        successColor,
                        warningColor,
                        primaryColor,
                        dangerColor,
                        '#5F91B6',
                        '#D3F6FC',
                        '#C8B0D2'
                    ],
                    borderRadius: 8
                }]
            };

            const config = {
                type: 'doughnut',
                data: data,
                options: {
                    plugins: {
                        title: {
                            display: false,
                        }
                    },
                    responsive: true,
                },
                defaults: {
                    global: {
                        defaultFont: fontFamily
                    }
                }
            };

            new Chart(ctx, config);
        }
    });
}