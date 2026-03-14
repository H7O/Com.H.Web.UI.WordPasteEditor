/**
 * H7.WordPasteEditor bootstrap – glue between the server control and
 * the WordPasteEditor / WordCleaner client libraries.
 *
 * Provides H7.WpeInit(editorId, hfId, options) which:
 *   - Creates / re-creates a WordPasteEditor instance
 *   - Wires debounced sync from the contenteditable div to the hidden input
 *   - Flushes before every form submit (full and async postback)
 *   - Exposes element.flushSync() for manual flush
 *   - Tracks instances by ID to tear down cleanly on UpdatePanel refreshes
 */
(function (root) {
    var ns = root.H7 = root.H7 || {};
    var registry = {};

    ns.WpeInit = function (editorId, hfId, options) {
        options = options || {};
        var debounce = options.debounce || 300;
        var readOnly = !!options.readOnly;

        var el = document.getElementById(editorId);
        var hf = document.getElementById(hfId);
        if (!el || !hf) return;

        // ── Tear down previous instance (by stable ID) ─────────
        var prev = registry[editorId];
        if (prev) {
            if (prev.instance) prev.instance.destroy();
            if (prev.el) prev.el.removeEventListener('input', prev.onInput);
            if (prev.form) prev.form.removeEventListener('submit', prev.flush);
        }

        // ── Populate editor from hidden field ──────────────────
        el.innerHTML = hf.value || '';

        var state = { el: el, instance: null, flush: null, onInput: null, form: null };

        // ── Create editor instance ─────────────────────────────
        if (!readOnly) {
            state.instance = new WordPasteEditor(el);
        } else {
            el.contentEditable = 'false';
        }

        // ── Debounced sync: editor → hidden field ──────────────
        var timer = 0;
        state.flush = function () { clearTimeout(timer); hf.value = el.innerHTML; };
        state.onInput = function () {
            clearTimeout(timer);
            timer = setTimeout(state.flush, debounce);
        };

        if (!readOnly) {
            el.addEventListener('input', state.onInput);
        }

        // ── Flush before form submit ───────────────────────────
        state.form = el.closest('form') || document.forms[0];
        if (state.form) {
            state.form.addEventListener('submit', state.flush);
        }

        // ── Store for cleanup & expose flushSync ───────────────
        registry[editorId] = state;
        el.flushSync = state.flush;
    };
})(typeof self !== 'undefined' ? self : this);
