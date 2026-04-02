/**
 * DateUtils — fixes the backend/frontend datetime mismatch.
 *
 * ROOT CAUSE:
 *   The .NET backend stores and returns UTC ISO-8601 strings.
 *   Some are like  "2024-03-24T12:00:00"    (no Z suffix — still UTC)
 *   Some are like  "2024-03-24T12:00:00Z"   (explicit UTC)
 *   Some are like  "2024-03-24T12:00:00.000" (fractional seconds, no Z)
 *
 *   Angular's DatePipe treats strings WITHOUT a Z as LOCAL time, which shifts
 *   the displayed time by the user's UTC offset (e.g. UTC+6 = +6 hours wrong).
 *
 * FIX:
 *   Always normalise to explicit UTC ("Z" suffix) before parsing.
 *   All display helpers use this normalised Date object.
 */

export class DateUtils {

  /**
   * Normalise a backend ISO string to a proper UTC Date.
   * Returns null if the string is empty or unparseable.
   */
  static toDate(iso: string | null | undefined): Date | null {
    if (!iso) return null;
    try {
      // Strip fractional seconds for safety, then force UTC
      const normalised = iso.trim()
        .replace(/\.\d+/, '')           // remove .000 etc.
        .replace(/Z?$/, 'Z');           // ensure Z suffix
      const d = new Date(normalised);
      return isNaN(d.getTime()) ? null : d;
    } catch {
      return null;
    }
  }

  /** "HH:mm" — 24-hour time, UTC */
  static time(iso: string): string {
    const d = DateUtils.toDate(iso);
    if (!d) return '—';
    return d.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit', hour12: false, timeZone: 'UTC' });
  }

  /** "dd MMM yyyy" — e.g. "24 Mar 2024", UTC */
  static date(iso: string): string {
    const d = DateUtils.toDate(iso);
    if (!d) return '—';
    return d.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric', timeZone: 'UTC' });
  }

  /** "dd MMM yyyy, HH:mm" — full datetime, UTC */
  static dateTime(iso: string): string {
    const d = DateUtils.toDate(iso);
    if (!d) return '—';
    return DateUtils.date(iso) + ', ' + DateUtils.time(iso);
  }

  /** "EEE, dd MMM yyyy" — e.g. "Mon, 24 Mar 2024", UTC */
  static dateLong(iso: string): string {
    const d = DateUtils.toDate(iso);
    if (!d) return '—';
    return d.toLocaleDateString('en-GB', { weekday: 'short', day: '2-digit', month: 'short', year: 'numeric', timeZone: 'UTC' });
  }

  /** "dd MMM" — short date, UTC */
  static dateShort(iso: string): string {
    const d = DateUtils.toDate(iso);
    if (!d) return '—';
    return d.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', timeZone: 'UTC' });
  }

  /**
   * Convert to "datetime-local" input value (YYYY-MM-DDTHH:mm), UTC.
   * Used when pre-filling <input type="datetime-local"> from backend data.
   */
  static toInputValue(iso: string): string {
    const d = DateUtils.toDate(iso);
    if (!d) return '';
    const pad = (n: number) => n.toString().padStart(2, '0');
    return `${d.getUTCFullYear()}-${pad(d.getUTCMonth() + 1)}-${pad(d.getUTCDate())}T${pad(d.getUTCHours())}:${pad(d.getUTCMinutes())}`;
  }

  /**
   * Convert a datetime-local input value (local time YYYY-MM-DDTHH:mm)
   * to a UTC ISO string for sending to the backend.
   * Uses the browser's local timezone for the conversion.
   */
  static fromInputValue(localValue: string): string {
    if (!localValue) return '';
    const d = new Date(localValue);   // browser treats datetime-local as local time
    return isNaN(d.getTime()) ? '' : d.toISOString();
  }

  /**
   * Human-readable duration between two ISO strings.
   * e.g. "5h 30m"
   */
  static duration(depIso: string, arrIso: string): string {
    const dep = DateUtils.toDate(depIso);
    const arr = DateUtils.toDate(arrIso);
    if (!dep || !arr) return '—';
    const minutes = Math.round((arr.getTime() - dep.getTime()) / 60000);
    if (minutes <= 0) return '—';
    return minutes >= 60
      ? `${Math.floor(minutes / 60)}h ${minutes % 60}m`
      : `${minutes}m`;
  }
}
