/**
 * Shows, hides or toggles OverlaySpinner
 */
var OverlaySpinner = OverlaySpinner || (function overlaySpinner() {
    'use-strict';

    /**
     * Toggle the overlay's status between hidden or visible on entire viewport. If 'containerId' parameter is set
     * as a valid selector, overlay will be set only on that element.
     * @param {String} containerId: CSS ID selector string
     */
    function toggleOverlay(containerId) {
        var container = document.querySelector('body > .overlay-spinner'),
            hideClass = 'hidden-overlay';
        if (containerId) {
            if ((typeof (containerId) === 'string')) {
                toggleOverlayDisplay(containerId);
            }
            else if (containerId instanceof Array) {
                var containerIds = containerId;
                containerIds.forEach(toggleOverlayDisplay);
            }
        }
        else if (container) {
            container.classList.toggle('fixed-overlay');
            container.classList.toggle(hideClass);
        }
        else {
            container = document.querySelector('.overlay-spinner');
            if (container) {
                var body = document.querySelector('body');
                body.insertBefore(container, body.firstChild);
                container.classList.toggle(hideClass);
            }
        }
    }
    /**
     * Toggle an overlay display
     * @param {String} containerId:     CSS ID selector for container where overlay should be shown/hidden
     * @param {Number} index:           
     */
    function toggleOverlayDisplay(containerId, index) {
        if (typeof (containerId) === 'string') {
            var hideClass = 'hidden-overlay',
                newContainer = document.getElementById(containerId);
            if (newContainer) {
                var overlayInNewContainer = newContainer.querySelector('.overlay-spinner'),
                    overlayMain = document.querySelector('body > .overlay-spinner');
                if (overlayInNewContainer) {
                    if (index && index > 0) {
                        //  Remove overlay because there is already a main overlay
                        overlayInNewContainer.parentElement.removeChild(overlayInNewContainer);
                    }
                    else {
                        var body = document.querySelector('body');
                        body.insertBefore(overlayInNewContainer, body.firstChild);
                        overlayInNewContainer.classList.toggle(hideClass);
                    }
                }
                else {
                    if (index && index > 0) {
                        //  Clone overlay if function is being executing in forEach context
                        var clonedOverlay = document.querySelector('.overlay-spinner').cloneNode(true);
                        newContainer.insertBefore(clonedOverlay, newContainer.firstChild);
                    }
                    else {
                        newContainer.insertBefore(overlayMain, newContainer.firstChild);
                        overlayMain.classList.toggle(hideClass);
                    }
                }
            }
        }
    }
    /*
     *  Hide overlay and removes any other which is on another node.
     */
    function hide() {
        var containers = Array.from(document.querySelectorAll('.overlay-spinner'));
        containers.forEach(function (overlay, index) {
            var body = document.querySelector('body'),
                mainOverlay = document.querySelector('body > .overlay-spinner:first-child');
            if (mainOverlay) {
                if (index && index > 0) {
                    overlay.parentElement.removeChild(overlay);
                }
                else {
                    mainOverlay.classList.add('hidden-overlay');
                    mainOverlay.classList.remove('fixed-overlay');
                }
            }
            else {
                body.insertBefore(overlay, body.firstChild);
                overlay.classList.add('hidden-overlay');
                overlay.classList.remove('fixed-overlay');
                if (overlay.parentElement.tagName !== 'BODY') {
                    overlay.parentElement.removeChild(container);
                }
            }
        });
    }
    /*
        Object to be exposed outside the module.
    */
    var API = {
        Show: function () {
            hide();
            toggleOverlay();
        },
        Toggle: toggleOverlay,
        Hide: hide
    };

    return API;
})();

OverlaySpinner.Toggle();