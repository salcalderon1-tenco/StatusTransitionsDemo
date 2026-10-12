var feedbackAlert = feedbackAlert || (function feedBackAlertClosure() {
    'use-strict';

    /**
     * Global variables
     */
    var global = {
        alert: document.getElementById('feedback-alert'),
        alertBody: document.getElementById('feedback-alert-body'),
        alertContainer: document.getElementById('feedback-alert-container'),
        alertIcon: document.getElementById('feedback-alert-icon'),
        alertTitle: document.getElementById('feedback-alert-title'),
        i18n: {
            error: 'Error',
            success: 'Éxito',
            info: 'Información',
            warning: 'Advertencia'
        },
        timer: null,
        timeout: null
    }
    /**
     * Module's settings
     */
    var settings = {
        allowedTranslations: [
            'error', 'success', 'info', 'warning'
        ],
        allowedPositions: [
            'absolute'
        ]
    }

    /**
     * Display a success styled alert
     * @param {String} message Message to display in alert's body
     * @param {String | Node} container Container or CSS selector for container where alert should be displayed
     */
    function show(response, container) {
        container = getContainer(container);
        setUpAlert(response, container);
        setTitle(global.i18n.success);
        global.alert.classList.add('alert-success');
        global.alertIcon.classList.add('fa-check');
        global.alert.classList.remove('d-none');

        checkTimer();
    }
    /**
     * Display a danger styled alert
     * @param {String} message Message to display in alert's body
     * @param {String | Node} container Container or CSS selector for container where alert should be displayed
     */
    function showAsError(response, container) {
        container = getContainer(container);
        setUpAlert(response, container);
        setTitle(global.i18n.error);
        global.alert.classList.add('alert-danger');
        global.alertIcon.classList.add('fa-ban');
        global.alert.classList.remove('d-none');

        checkTimer();
    }
    /**
     * Display an information styled alert
     * @param {String} message Message to display in alert's body
     * @param {String | Node} container Container or CSS selector for container where alert should be displayed
     */
    function showAsInfo(response, container) {
        container = getContainer(container);
        setUpAlert(response, container);
        setTitle(global.i18n.info);
        global.alert.classList.add('alert-info');
        global.alertIcon.classList.add('fa-info');
        global.alert.classList.remove('d-none');

        checkTimer();
    }
    /**
     * Display a alert with callback event, it is required to insert the 
     * callback button manually in the message property
     * @param {String} message Message to display in alert's body
     * @param {String | Node} container Container or CSS selector for container where alert should be displayed
     * @param {Function} callbackbtn The button that will have a callback event
     * @param {Function} callback Function that will execute after the callbackbtn is press
     */
    function showAsCallbackWarning(message, container, callbackbtn, callback) {
        container = getContainer(container);
        setUpAlert(message, container);
        setTitle(global.i18n.warning);
        global.alert.classList.add('alert-warning');
        global.alertIcon.classList.add('fa-exclamation-triangle');
        global.alert.classList.remove('d-none');
        callbackbtn = getContainer(callbackbtn);
        $(callbackbtn)
            .off('click')
            .on('click', callback);
    }
    /**
     * Display a warning styled alert
     * @param {String} message Message to display in alert's body
     * @param {String | Node} container Container or CSS selector for container where alert should be displayed
     */
    function showAsWarning(message, container) {
        container = getContainer(container);
        setUpAlert(message, container);
        setTitle(global.i18n.warning);
        global.alert.classList.add('alert-warning');
        global.alertIcon.classList.add('fa-exclamation-triangle');
        global.alert.classList.remove('d-none');

        checkTimer();
    }
    /**
     * Hides an alert
     */
    function hide() {
        global.alert.classList.add('d-none');
    }
    /**
     *  Get container in case parameter is a CSS selector
     * @param {String | Node} container Container or CSS selector for container where alert should be displayed
     */
    function getContainer(container) {
        var result = undefined;
        if (container) {
            if (typeof (container) === 'string') {
                result = document.querySelector(container);
                if (!result) {
                    result = document.querySelector(`#${container}`);
                }
            }
            else if (typeof (container) === 'object' && container instanceof Node) {
                result = container;
            }
        }

        return result;
    }
    /**
     * Setup alert
     * @param {String} message Message to display in alert's body
     * @param {String | Node} container Container or CSS selector for container where alert should be displayed
     */
    function setUpAlert(message, container) {
        check();
        resetAlert();
        setMessage(message);
        moveToContainer(container);
    }
    /**
     * Select message which will be displayed inside paragraph
     * @param {String} message Message to display in alert's body
     */
    function setMessage(message) {
        if (message) {
            if (typeof (message) === 'string' && message.length > 0) {
                global.alertBody.innerHTML = message;
            }
        }
    }
    /**
     * Set title's text
     * @param {String} text Text to set in title
     */
    function setTitle(text) {
        var i = global.alertTitle.firstElementChild;

        global.alertTitle.innerHTML = ` ${text}`;
        global.alertTitle.insertBefore(i, global.alertTitle.firstChild);
    }
    /**
     * Clear title's text
     */
    function clearTitle() {
        var i = global.alertTitle.firstElementChild;
        global.alertTitle.innerHTML = '';
        global.alertTitle.appendChild(i);
    }
    /**
     * Move alert into given container
     * @param {String | Node} container Container or CSS selector for container where alert should be displayed
     */
    function moveToContainer(container) {
        if (container) {
            container.insertBefore(global.alert, container.firstChild);
        }
    }
    /**
     * Clear alert's body and return it into its original position
     */
    function resetAlert() {
        clearTimeout(global.timeout);

        if (!document.querySelector('feedback-alert-container > feedback-alert')) {
            global.alertContainer.insertBefore(global.alert, global.alertContainer.firstChild);
        }
        global.alert.classList.remove('alert-success');
        global.alert.classList.remove('alert-info');
        global.alert.classList.remove('alert-warning');
        global.alert.classList.remove('alert-danger');
        global.alertIcon.classList.remove('fa-check');
        global.alertIcon.classList.remove('fa-info');
        global.alertIcon.classList.remove('fa-exclamation-triangle');
        global.alertIcon.classList.remove('fa-ban');
        global.alertBody.innerHTML = '';
        clearTitle();

        //  Hide alert in case resetAlert method was invoked through close button
        global.alert.classList.add('d-none');

        //  False to avoid remove alert from DOM
        return false;
    }
    /**
     * Check if DOM elements exists and append an event handler into alert on close
     */
    function check() {
        var isValidAlert = global.alert && global.alert instanceof HTMLDivElement,
            isValidAlertBody = global.alertBody && global.alertBody instanceof HTMLParagraphElement,
            isValidContainer = global.alertContainer && global.alertContainer instanceof HTMLDivElement,
            isValidIcon = global.alertIcon,
            isValidTitle = global.alertTitle && global.alertTitle instanceof HTMLHeadingElement;
        if (!isValidAlert) {
            throw 'Alert DIV not found.';
        }
        if (!isValidAlertBody) {
            throw 'Alert paragraph not found.';
        }
        if (!isValidContainer) {
            throw 'Alert container not found.';
        }
        if (!isValidIcon) {
            throw 'Alert icon not found.';
        }
        if (!isValidTitle) {
            throw 'Alert title not found.';
        }

        $(global.alert)
            .off('close.bs.alert')
            .on('close.bs.alert', resetAlert);
    }
    /**
    * Define a default timer for the feedback alert to close.
    * @param {number} seconds Duration in seconds of the Feedback Alert before it close automatic. If no number is provide the default timer will be 5 seconds
    */
    function setTimer(seconds = 5) {
        if (!Number.isInteger(seconds)) {
            global.timer = null;
            throw `The variable ${seconds} must be an int type with no floating points.`;
        }

        if (seconds < 0) {
            global.timer = null;
            throw `The number ${seconds} must be greater than 0`;
        }

        global.timer = seconds * 1000;
    }
    /**
     * Define a set timeout function to close a feedback alert after a specific time
     */
    function checkTimer() {
        if (!global.timer || global.timer === 0) {
            return;
        };

        global.timeout = setTimeout(() => {
            hide();
        }, global.timer);
    }
    /**
     * Configure a Feedback alert with custom properties
     * @param {string} position The position desired for the Feedbackalert
     * @param {number} timerInSeconds The timer for the Feedbackalert
     */
    function setup({ position = null, timerInSeconds = null }) {
        if (position && settings.allowedPositions.includes(position)) {
            global.alert.classList.add('feedback-alert__absolute');
        } else {
            throw `${position} is not in settings.allowPositions.`;
        }

        if (timerInSeconds) {
            setTimer(timerInSeconds);
        }
    }
    /**
     * Initialize internationalization settings.
     * @param {Object} i18n Properties in object should match with settings.allowedTranslations.
     */
    function i18n(i18n) {
        for (var propertyName in i18n) {
            var propertyValue = i18n[propertyName],
                isValidName = settings.allowedTranslations.includes(propertyName);
            if (!isValidName) {
                throw `${propertyName} is not in settings.allowedTranslations.`;
            }

            if (propertyValue && typeof (propertyValue) === 'string' && propertyValue.trim().length > 0) {
                global.i18n[propertyName] = propertyValue.trim();
            }
        }
    }
    /**
     * Initialize internationalization settings.
     * @param {Object} i18n Properties in object should match with settings.allowedTranslations.
     */
    function i18n(i18n) {
        for (var propertyName in i18n) {
            var propertyValue = i18n[propertyName],
                isValidName = settings.allowedTranslations.includes(propertyName);
            if (!isValidName) {
                throw `${propertyName} is not in settings.allowedTranslations.`;
            }

            if (propertyValue && typeof (propertyValue) === 'string' && propertyValue.trim().length > 0) {
                global.i18n[propertyName] = propertyValue.trim();
            }
        }
    }
    /*
     * Object exposed outside module
     */
    return {
        i18n,
        show,
        showAsInfo,
        showAsWarning,
        showAsError,
        showAsCallbackWarning,
        hide,
        setTimer,
        setup
    };
})();