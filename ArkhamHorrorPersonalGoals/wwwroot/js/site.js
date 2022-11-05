var davisSoftware = davisSoftware || {};

davisSoftware.ready = function (fn) {
    if (document.readyState != 'loading') {
        fn();
    } else {
        document.addEventListener('DOMContentLoaded', fn);
    }
};

davisSoftware.getCookie = function (name) {
    let prefix = name + '=';
    let ca = document.cookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(prefix) == 0) {
            return c.substring(prefix.length, c.length);
        }
    }
    return '';
}

davisSoftware.setCookie = function (name, value, expiryDays) {
    const d = new Date();
    d.setTime(d.getTime() + (expiryDays * 24 * 60 * 60 * 1000));
    let expires = "expires=" + d.toUTCString();
    document.cookie = name + "=" + value + ";" + expires + ";path=/";
}

davisSoftware.setIdentifier = function () {

    const cookieName = 'ArkhamID';

    var cookie = davisSoftware.getCookie(cookieName);

    if (!cookie) {
        let r = (Math.random() + 1).toString(36).substring(7);
        davisSoftware.setCookie(cookieName, r, 365);
    }
}

davisSoftware.getState = function (e) {

    //console.log("Getting current state");

    if (e) {
        e.preventDefault();
    }

    const request = new XMLHttpRequest();
    request.open('GET', '/Home/GetState', true);

    request.onload = function (e) {

        const request = this;

        if (request.status >= 200 && request.status < 400) {
            document.querySelector('.js-goals').innerHTML = request.response;
        } else {
            // We reached our target server, but it returned an error
        }
    };

    request.onerror = function (e) {
        // There was a connection error of some sort

        const request = this;

        console.error('Something went wrong', request, e);
    };

    request.send();
};

davisSoftware.setVisitorName = function (e) {

    //console.log("setting visitor name");

    const visitorName = document.querySelector('input[name="visitorName"]').value;

    if (e) {
        e.preventDefault();
    }

    const request = new XMLHttpRequest();
    request.open('GET', '/Home/SetVisitorName?name=' + visitorName, true);

    request.onload = function (e) {

        const request = this;

        if (request.status >= 200 && request.status < 400) {
            // Success!
            const data = JSON.parse(request.response);

            davisSoftware.getState();
        } else {
            // We reached our target server, but it returned an error

        }
    };

    request.onerror = function (e) {
        // There was a connection error of some sort

        const request = this;
    };

    request.send();
};

davisSoftware.chooseGoal = function (e) {

    //console.log("Choosing goal");

    const goalId = e.target.getAttribute('data-id');

    if (e) {
        e.preventDefault();
    }

    const request = new XMLHttpRequest();
    request.open('GET', '/Home/ChooseGoal?id=' + goalId, true);

    request.onload = function (e) {
        davisSoftware.getState();
    };

    request.send();
}

davisSoftware.showGoals = function (e) {

    //console.log("Showing goals");

    if (e) {
        e.preventDefault();
    }

    const request = new XMLHttpRequest();
    request.open('GET', '/Home/ShowGoals', true);

    request.onload = function (e) {
        davisSoftware.getState();
    };

    request.send();
}

davisSoftware.reset = function (e) {

    //console.log("Resetting");

    if (e) {
        e.preventDefault();
    }

    const request = new XMLHttpRequest();
    request.open('GET', '/Home/Reset', true);

    request.onload = function (e) {
        davisSoftware.getState();
    };

    request.send();
}

davisSoftware.ready(function () {

    davisSoftware.setIdentifier();

    document.querySelector('input[name="visitorName"]').addEventListener('keyup', function (event) {
        if (event.key === "Enter") {
            davisSoftware.setVisitorName();
        }
    });

    document.querySelector('.js-setVisitorName').addEventListener('click', davisSoftware.setVisitorName);

    document.querySelector('.js-goals').addEventListener('click', function (e) {
        if (e.target.classList.contains('js-chooseGoal')) {
            davisSoftware.chooseGoal(e);
        }
    });

    document.querySelector('.js-showGoals').addEventListener('click', davisSoftware.showGoals);

    document.querySelector('.js-reset').addEventListener('click', davisSoftware.reset);

    window.setInterval(davisSoftware.getState, 2000);
    davisSoftware.getState();
});
