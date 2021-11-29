const { ipcRenderer } = require("electron");


// Code for the sidebar.
$(document).ready(function () {
    $('#sidebarCollapse').on('click', function () {
        // open or close navbar and save to GlobalControllers
        $('#sidebar').toggleClass('active');
        ipcRenderer.send("SidebarToggle", "owo");

        // close dropdowns
        $('.collapse.in').toggleClass('in');

        // adjust aria-expanded attributes used for open/close arrows
        $('a[aria-expanded=true]').attr('aria-expanded', 'false');
    });
});

document.onreadystatechange = (event) => {
    if (document.readyState == "complete") {
        document.getElementById('min-button').addEventListener("click", event => {
            ipcRenderer.send("MinimizeWindow", "owo");
            $('#sidebar').toggleClass('active');
        });
        document.getElementById('max-button').addEventListener("click", event => {
            ipcRenderer.send("MaximizeWindow", "owo");
        });
        document.getElementById('restore-button').addEventListener("click", event => {
            ipcRenderer.send("RestoreWindow", "owo");
        });
        document.getElementById('close-button').addEventListener("click", event => {
            ipcRenderer.send("CloseWindow", "owo");
        });
        ipcRenderer.on('maximized', (event, arg) => {
            document.body.classList.add('maximized');
        });
        ipcRenderer.on('minimized', (event, arg) => {
            document.body.classList.remove('maximized');
        });
    }
};

/*
// For the custom titlebar:
function handleWindowControls() {
    // Make minimise/maximise/restore/close buttons work when they are clicked
    document.getElementById('min-button').addEventListener("click", event => {
        ipcRenderer.send("MinimizeWindow", "owo");
    });
    document.getElementById('max-button').addEventListener("click", event => {
        ipcRenderer.send("MaximizeWindow", "owo");
    });
    document.getElementById('restore-button').addEventListener("click", event => {
        ipcRenderer.send("RestoreWindow", "owo");
    });
    document.getElementById('close-button').addEventListener("click", event => {
        ipcRenderer.send("CloseWindow", "owo");
    });
    ipcRenderer.on('maximized', (event, arg) => {
        document.body.classList.add('maximized');
    });
    ipcRenderer.on('minimized', (event, arg) => {
        document.body.classList.remove('maximized');
    });
}*/