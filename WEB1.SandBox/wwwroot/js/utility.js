
//chartJS
const chartInstances = {};

window.renderChart = function (chartId, chartType, labels, values, colors, showLegend) {
    const canvas = document.getElementById(chartId);
    if (!canvas) return;

    const ctx = canvas.getContext('2d');

    if (chartInstances[chartId]) {
        chartInstances[chartId].data.labels = labels;
        chartInstances[chartId].data.datasets[0].data = values;
        chartInstances[chartId].data.datasets[0].backgroundColor = colors || Chart.defaults.backgroundColor;
        chartInstances[chartId].options.plugins.legend.display = showLegend !== false;
        chartInstances[chartId].update();
    } else {
        chartInstances[chartId] = new Chart(ctx, {
            type: chartType,
            data: {
                labels: labels,
                datasets: [{
                    data: values,
                    backgroundColor: colors || Chart.defaults.backgroundColor
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: showLegend !== false
                    }
                }
            }
        });
    }
};

window.destroyChart = function (chartId) {
    if (chartInstances[chartId]) {
        chartInstances[chartId].destroy();
        delete chartInstances[chartId];
    }
};

// this code is in the ~/wwwroot/ThemeChanger.js file

var themeChanger = {
    changeCss: function (cssFileUrl) {
        var oldLink = document.getElementById("TelerikThemeLink"); // we have this id on the <link> that references the theme

        if (cssFileUrl === oldLink.getAttribute("href")) {
            return;
        }

        var newLink = document.createElement("link");
        newLink.setAttribute("id", "TelerikThemeLink");
        newLink.setAttribute("rel", "stylesheet");
        newLink.setAttribute("type", "text/css");
        newLink.setAttribute("href", cssFileUrl);
        newLink.onload = () => {
            oldLink.parentElement.removeChild(oldLink);
        };

        document.getElementsByTagName("head")[0].appendChild(newLink);
    }
}

function spinnerJS(id) {
    $("#" + id).css("z-index", "7");
}

function spinnerJSEnd(id) {
    $("#" + id).css("z-index", "1060");
}
function AlertsToastr(Tipo, Mensaje, titulo) {

    toastr[Tipo](Mensaje, titulo);
};

async function downloadFileFromStream2(fileName, contentStreamReference) {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    triggerFileDownload(fileName, url);
    URL.revokeObjectURL(url);
}

function triggerFileDownload(fileName, url) {
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
}
window.addEventListener('dragstart', function (e) {
    event.dataTransfer.effectAllowed = "move";
}, false);
window.addEventListener('dragover', function (event) {
    event.preventDefault();
    event.stopPropagation();
}, false);
$(document).on("dragover", "#inputShared", function (e) {
    var timeout = window.dropZoneTimeout;
    if (!timeout) {
        $('.container-inputShared').addClass('image-dropping');
    } else {
        clearTimeout(timeout);
    }
    window.dropZoneTimeout = setTimeout(function () {
        window.dropZoneTimeout = null;
        $('.container-inputShared').removeClass('image-dropping');
    }, 100);
});

function clearToastr() {
    window.toastr.clear();
}

function openModal(id, lockClose) {
    var el = document.getElementById(id);
    if (!el) return;
    var modal = bootstrap.Modal.getOrCreateInstance(el, {
        backdrop: lockClose ? 'static' : true,
        keyboard: !lockClose
    });
    modal.show();
}

function closeModal(id) {
    var el = document.getElementById(id);
    if (!el) return;
    var modal = bootstrap.Modal.getInstance(el);
    if (modal) {
        modal.hide();
    }
}

function confirmar(title, text, icon, theme) {
    var fg = "";
    var bg = "";
    if (theme == "dark") {
        fg = "#f1f3f7";
        bg = "#333";
    }
    if (theme == "light") {

        fg = "#333";
        bg = "#f1f3f7";
    }
    return new Promise(resolve => {
        Swal.fire({
            title,
            text,
            icon,
            color: fg,
            background: bg,
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Si',
            cancelButtonText: 'No'
        }).then((result) => {

            resolve(result.isConfirmed);

        })
    });
}
function alerta(title, text, icon, theme) {

    console.log(theme);
    var fg = "";
    var bg = "";
    if (theme == "dark") {
        fg = "#f1f3f7";
        bg = "#333";
    }
    if (theme == "light") {

        fg = "#333";
        bg = "#f1f3f7";
    }
    Swal.fire({
        title,
        text,
        icon,
        color: fg,
        background: bg
    })
}
var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl)
})
function StorageEvent(dotnetHelper) {
    window.addEventListener('storage', function (event) {
        if (event.storageArea === localStorage) {
            // It's local storage
            dotnetHelper.invokeMethodAsync("VerificarLogueo");
        }
    }, false);
}
//function timerInactivo(dotnetHelper) {
//    var timer;
//    document.onmousemove = resetTimer;
//    document.onmousedown = resetTimer;
//    document.onmouseenter = resetTimer;
//    document.onkeypress = resetTimer;
//    function resetTimer() {
//        clearTimeout(timer);
//        timer = setTimeout(logout, 1800000); //30 MIN
//    }

//    function logout() {
//        dotnetHelper.invokeMethodAsync("Logout");
//    }
//}

function playSound() {
    var audio = new Audio('/source/sounds/cash.mp3');
    audio.play();
}
window.getScrollValues = (elementId) => {
    const element = document.getElementById(elementId);
    console.log(element);
    return {
        scrollTop: element.scrollTop,
        scrollHeight: element.scrollHeight,
        clientHeight: element.clientHeight
    };
};

var endScrollTimer = false;
function ScrollToElement(id) {
    endScrollTimer = false;
    var contenedor = document.getElementById("ContentCalendarEvent");  // Make sure you define the ID you're looking for
    var elements = Array.from(contenedor.children).filter(function (element) {
        return element.id.includes(id);  // Check if the ID contains the given substring
    });

    // If you want to scroll the first matched element into view
    if (elements.length > 0) {
        if (elements[0]) {
            elements[0].scrollIntoView({ behavior: 'smooth', block: 'start' });


        } else {
            setTimeout(() => scrollToElement(id), 100); // Reintentar después de un pequeño retraso
        }
    }
    /*    setTimeout(() => endScrollTimer = true, 1000); */

}

function AdicionarArribaCalendario(dotnetHelper) {

    var contenedor = document.getElementById("ContentCalendarEvent");

    // Guardamos la posición de desplazamiento y la altura original
    var scrollPos = contenedor.scrollTop;
    var scrollHeight = contenedor.scrollHeight;  // La altura total antes de la inserción

    // Realizar la inserción de los nuevos elementos al principio de la lista
    dotnetHelper.invokeMethodAsync("AdiccionarMesArriba");

    // Ajustar la posición del scroll después de la inserción
    setTimeout(function () {
        // Calcular la nueva altura del contenedor
        var newScrollHeight = contenedor.scrollHeight;

        // Calcular la diferencia en la altura
        var diff = newScrollHeight - scrollHeight;

        // Ajustar el scrollTop para que la posición relativa se mantenga
        contenedor.scrollTop = scrollPos + diff;
    }, 0);


}

var previousScrollTop = 0;  // Para almacenar la última posición de desplazamiento
var scrollTimer = null;     // Para detectar la detención del desplazamiento
var scrollDelay = 20;      // Tiempo en ms para considerar que el scroll ha terminado

function detectElementInView(dotnetHelper) {
    clearTimeout(scrollTimer);
    var items = document.querySelectorAll('.observed');  // Todos los elementos dentro del contenedor
    var contenedor = document.getElementById("ContentCalendarEvent");
    let closestElement = null;

    // Iteramos sobre todos los elementos
    items.forEach(item => {
        const rect = item.getBoundingClientRect();
        const containerRect = contenedor.getBoundingClientRect();

        // Verificamos si el borde superior del elemento está en la parte visible del contenedor
        if (rect.top + 10 >= containerRect.top && rect.top <= containerRect.top + contenedor.clientHeight) {
            // Guardamos el primer elemento visible
            if (!closestElement || rect.top < closestElement.getBoundingClientRect().top) {
                closestElement = item;
            }
        }
    });
    scrollTimer = setTimeout(function () {
        if (closestElement) {
            dotnetHelper.invokeMethodAsync("UpdateCurrentDate", closestElement.id);
        }
    }, scrollDelay);

}
