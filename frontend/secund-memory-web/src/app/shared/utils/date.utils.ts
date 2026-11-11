/**
 * Matches the backend's DateOnly.ToString("yyyy-MM-dd") used for journal page
 * titles and the /api/journals/{date} route. Built from the LOCAL date
 * components (not toISOString, which converts to UTC first and silently
 * shifts the date by one for any timezone ahead of UTC — most of
 * Europe/Asia/Oceania — whenever local time has passed midnight but UTC
 * hasn't yet, or vice versa for timezones behind UTC).
 */
export function toJournalDateString(date: Date): string {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}

export function isToday(date: Date): boolean {
  return toJournalDateString(date) === toJournalDateString(new Date());
}

/** Adds (or, with a negative count, subtracts) whole days to a "yyyy-MM-dd" journal date string. */
export function shiftJournalDate(date: string, days: number): string {
  const [year, month, day] = date.split('-').map(Number);
  const shifted = new Date(year, month - 1, day);
  shifted.setDate(shifted.getDate() + days);
  return toJournalDateString(shifted);
}

/** "2026-11-12" -> "Thursday, November 12, 2026", for the journal view's heading. */
export function formatJournalDateLong(date: string): string {
  const [year, month, day] = date.split('-').map(Number);
  return new Date(year, month - 1, day).toLocaleDateString(undefined, {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  });
}
