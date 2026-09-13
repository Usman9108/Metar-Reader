(function () {
  const els = {
    clock: document.getElementById('clock'),
    brightBtns: document.querySelectorAll('.bright-opt'),
    picker: document.getElementById('picker'),
    pickerFace: document.getElementById('pickerFace'),
    pickerCode: document.getElementById('pickerCode'),
    pickerPlace: document.getElementById('pickerPlace'),
    listbox: document.getElementById('listbox'),
    briefBtn: document.getElementById('briefBtn'),
    scan: document.getElementById('scan'),
    errorBanner: document.getElementById('error-banner'),
    readout: document.getElementById('readout'),
    stationName: document.getElementById('stationName'),
    obsTime: document.getElementById('obsTime'),
    catBadge: document.getElementById('catBadge'),
    needle: document.getElementById('needle'),
    dialSpeed: document.getElementById('dialSpeed'),
    ticks: document.getElementById('ticks'),
    tTemp: document.getElementById('tTemp'),
    tVis: document.getElementById('tVis'),
    tCeil: document.getElementById('tCeil'),
    tAlt: document.getElementById('tAlt'),
    voiceLine: document.getElementById('voiceLine'),
    rawMetar: document.getElementById('rawMetar'),
  };

  if (!els.briefBtn) {
    return;
  }

  // ---------- Panel-brightness (day / night / auto) ----------
  (function initTheme() {
    const THEME_KEY = 'metar-panel-brightness';
    const media = window.matchMedia('(prefers-color-scheme: dark)');

    function resolveAuto() {
      return media.matches ? 'night' : 'day';
    }

    function applyTheme(pref) {
      const resolved = pref === 'auto' ? resolveAuto() : pref;
      document.documentElement.setAttribute('data-theme', resolved);
      els.brightBtns.forEach((b) => b.classList.toggle('active', b.dataset.themeChoice === pref));
    }

    function readStoredPref() {
      try { return localStorage.getItem(THEME_KEY); } catch (e) { return null; }
    }
    function storePref(pref) {
      try { localStorage.setItem(THEME_KEY, pref); } catch (e) { /* private mode / blocked storage */ }
    }

    let currentPref = readStoredPref() || 'auto';
    applyTheme(currentPref);

    els.brightBtns.forEach((btn) => {
      btn.addEventListener('click', () => {
        currentPref = btn.dataset.themeChoice;
        storePref(currentPref);
        applyTheme(currentPref);
      });
    });

    media.addEventListener('change', () => {
      if (currentPref === 'auto') applyTheme('auto');
    });
  })();

  // ---------- Clock ----------
  function tickClock() {
    els.clock.textContent = new Date().toISOString().slice(11, 19) + 'Z';
  }
  tickClock();
  setInterval(tickClock, 1000);

  // ---------- Wind dial ticks (drawn once) ----------
  (function drawTicks() {
    const cx = 66, cy = 66, rOuter = 58;
    for (let deg = 0; deg < 360; deg += 30) {
      const major = deg % 90 === 0;
      const len = major ? 10 : 6;
      const rad = (deg - 90) * Math.PI / 180;
      const x1 = cx + (rOuter - len) * Math.cos(rad);
      const y1 = cy + (rOuter - len) * Math.sin(rad);
      const x2 = cx + rOuter * Math.cos(rad);
      const y2 = cy + rOuter * Math.sin(rad);
      const line = document.createElementNS('http://www.w3.org/2000/svg', 'line');
      line.setAttribute('x1', x1); line.setAttribute('y1', y1);
      line.setAttribute('x2', x2); line.setAttribute('y2', y2);
      line.setAttribute('class', major ? 'tick-major' : 'tick');
      els.ticks.appendChild(line);
    }
  })();

  // ---------- Airport picker (custom listbox) ----------
  let selectedIcao = els.pickerCode.textContent.trim();

  function openListbox() {
    els.picker.classList.add('open');
    els.pickerFace.setAttribute('aria-expanded', 'true');
  }
  function closeListbox() {
    els.picker.classList.remove('open');
    els.pickerFace.setAttribute('aria-expanded', 'false');
  }

  els.pickerFace.addEventListener('click', (e) => {
    e.stopPropagation();
    els.picker.classList.contains('open') ? closeListbox() : openListbox();
  });
  document.addEventListener('click', (e) => {
    if (!els.picker.contains(e.target)) closeListbox();
  });
  document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape') closeListbox();
  });

  els.listbox.querySelectorAll('.option').forEach((option) => {
    option.addEventListener('click', () => {
      selectedIcao = option.dataset.icao;
      els.pickerCode.textContent = option.dataset.icao;
      els.pickerPlace.textContent = option.dataset.place;
      els.listbox.querySelectorAll('.option').forEach((o) => o.classList.toggle('active', o === option));
      closeListbox();
    });
  });

  // ---------- Fetch + render ----------
  function formatObsTime(isoLike) {
    const date = new Date(isoLike.replace(' ', 'T'));
    if (Number.isNaN(date.getTime())) return '';
    const hh = String(date.getUTCHours()).padStart(2, '0');
    const mm = String(date.getUTCMinutes()).padStart(2, '0');
    const day = String(date.getUTCDate()).padStart(2, '0');
    const month = date.toLocaleString('en-US', { month: 'short', timeZone: 'UTC' }).toUpperCase();
    return `OBS ${hh}${mm}Z · ${day} ${month} ${date.getUTCFullYear()}`;
  }

  function renderResult(data) {
    els.readout.classList.remove('animating');
    void els.readout.offsetWidth;
    els.readout.hidden = false;
    els.readout.classList.add('animating');

    els.stationName.textContent = data.stationName;
    els.obsTime.textContent = formatObsTime(data.observedAtUtc);

    const cat = (data.flightCategory || 'VFR').toLowerCase();
    els.catBadge.textContent = cat.toUpperCase();
    els.catBadge.className = 'cat-badge cat-' + cat;

    const hasDirection = data.windDirectionDegrees !== null && data.windDirectionDegrees !== undefined && !data.isVariableWind;
    els.needle.style.transform = 'rotate(' + (hasDirection ? data.windDirectionDegrees : 0) + 'deg)';
    els.needle.style.opacity = hasDirection ? '1' : '0.25';
    els.dialSpeed.textContent = data.windSpeedMph + (data.windGustMph ? 'G' + data.windGustMph : '');

    els.tTemp.innerHTML = data.tempF !== null && data.tempF !== undefined ? data.tempF + '<span class="unit">°F</span>' : '&mdash;';
    els.tVis.innerHTML = data.visibilityPhrase ? escapeHtml(data.visibilityPhrase) : '&mdash;';
    els.tCeil.innerHTML = data.ceilingFeet ? data.ceilingFeet.toLocaleString() + '<span class="unit">ft</span>' : 'None';
    els.tAlt.innerHTML = data.altimeterInHg !== null && data.altimeterInHg !== undefined
      ? data.altimeterInHg.toFixed(2) + '<span class="unit">inHg</span>'
      : '&mdash;';

    els.voiceLine.textContent = data.summary;
    els.rawMetar.innerHTML = escapeHtml(data.rawMetar) + '<span class="cursor"></span>';
  }

  function escapeHtml(str) {
    const div = document.createElement('div');
    div.textContent = str;
    return div.innerHTML;
  }

  function showError(message) {
    els.errorBanner.textContent = message;
    els.errorBanner.hidden = false;
    els.readout.hidden = true;
  }

  function hideError() {
    els.errorBanner.hidden = true;
    els.errorBanner.textContent = '';
  }

  async function fetchWeather() {
    if (!selectedIcao) return;

    hideError();
    els.scan.classList.add('active');
    els.briefBtn.disabled = true;

    try {
      const response = await fetch(`?handler=Weather&icao=${encodeURIComponent(selectedIcao)}`, {
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
      showError("Couldn't reach the weather service right now — please try again in a moment.");
    } finally {
      els.scan.classList.remove('active');
      els.briefBtn.disabled = false;
    }
  }

  els.briefBtn.addEventListener('click', () => {
    closeListbox();
    fetchWeather();
  });

  // Populate the panel immediately with the default airport.
  fetchWeather();
})();
