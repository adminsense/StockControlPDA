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

