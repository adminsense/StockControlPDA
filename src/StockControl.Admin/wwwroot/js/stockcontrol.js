window.stockControl = window.stockControl || {};

window.stockControl.focusById = (id) => {
  try {
    const el = document.getElementById(id);
    if (!el) return;
    if (typeof el.focus === "function") el.focus();
    if (typeof el.select === "function") el.select();
  } catch {
    // ignore
  }
};

window.stockControl.blurActive = () => {
  try {
    const a = document.activeElement;
    if (a && typeof a.blur === "function") a.blur();
  } catch {
    // ignore
  }
};

/** Admin login: Set-Cookie must run in the browser (not server-side HttpClient). */
window.stockControl.loginAdmin = async (username, password) => {
  try {
    const res = await fetch("/api/auth/login?app=admin", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "same-origin",
      body: JSON.stringify({ username, password }),
    });
    const data = await res.json().catch(() => ({}));
    return {
      ok: res.ok && data.ok === true,
      token: data.token ?? null,
      displayName: data.displayName ?? null,
      error: data.error ?? (res.ok ? null : "Sign in failed."),
    };
  } catch {
    return { ok: false, token: null, displayName: null, error: "Sign in failed." };
  }
};

window.stockControl.logoutAdmin = async () => {
  try {
    await fetch("/api/auth/logout", {
      method: "POST",
      credentials: "same-origin",
    });
  } catch {
    // ignore
  }
};

// Blazor IJSRuntime only invokes global functions (not object.method paths).
globalThis.loginAdmin = (username, password) =>
  window.stockControl.loginAdmin(username, password);
globalThis.logoutAdmin = () => window.stockControl.logoutAdmin();
globalThis.stockControlFocusById = (id) => window.stockControl.focusById(id);
globalThis.stockControlBlurActive = () => window.stockControl.blurActive();
