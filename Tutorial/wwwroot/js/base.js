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