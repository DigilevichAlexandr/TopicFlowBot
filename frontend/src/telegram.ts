import WebApp from "@twa-dev/sdk";

export const tg = WebApp;

export function initTelegram() {
  try {
    tg.ready();
    tg.expand();
  } catch {
    // Local browser mode.
  }
}

export function getInitData(): string {
  return tg.initData ?? "";
}

export function sendDataToBot(payload: unknown) {
  try {
    tg.sendData(JSON.stringify(payload));
  } catch {
    // Ignore send errors in non-Telegram environments.
  }
}
