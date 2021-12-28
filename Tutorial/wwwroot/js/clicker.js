/* Bootstrap toggle */
$('#global-toggle').change(function () {
    ipcRenderer.send("ClickerToggle", $('#global-toggle').prop('checked'));
})