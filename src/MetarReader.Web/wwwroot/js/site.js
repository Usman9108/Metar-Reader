(function () {
    const select = document.getElementById('airport-select');
    const button = document.getElementById('get-weather-btn');
    const loading = document.getElementById('loading-indicator');
    const errorBanner = document.getElementById('error-banner');
    const resultCard = document.getElementById('result-card');
    const stationName = document.getElementById('station-name');
    const summaryText = document.getElementById('summary-text');
    const detailsList = document.getElementById('details-list');
    const observedAt = document.getElementById('observed-at');
    const rawMetar = document.getElementById('raw-metar');

    if (!button) {
        return;
    }

    button.addEventListener('click', fetchWeather);

    async function fetchWeather() {
        const icao = select.value;
        if (!icao) {
            return;
        }

        setLoading(true);
        hideError();
        resultCard.hidden = true;

        try {
            const response = await fetch(`?handler=Weather&icao=${encodeURIComponent(icao)}`, {
                headers: { Accept: 'application/json' },
            });

            if (!response.ok) {
                showError('Something went wrong talking to the server. Please try again.');
                return;
            }

            const data = await response.json();

            if (!data.success) {
                showError(data.error || 'Could not retrieve weather for that airport.');
                return;
            }

            renderResult(data);
        } catch (err) {
            showError('Couldn\'t reach the weather service right now — please try again in a moment.');
        } finally {
            setLoading(false);
        }
    }

    function renderResult(data) {
        stationName.textContent = data.stationName;
        summaryText.textContent = data.summary;

        detailsList.innerHTML = '';
        (data.details || []).forEach((detail) => {
            const li = document.createElement('li');
            li.textContent = detail;
            detailsList.appendChild(li);
        });

        observedAt.textContent = '';
        rawMetar.textContent = data.rawMetar || '';

        resultCard.hidden = false;
    }

    function setLoading(isLoading) {
        loading.hidden = !isLoading;
        button.disabled = isLoading;
    }

    function showError(message) {
        errorBanner.textContent = message;
        errorBanner.hidden = false;
    }

    function hideError() {
        errorBanner.hidden = true;
        errorBanner.textContent = '';
    }
})();
