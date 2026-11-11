/** Matches the backend's DateOnly.ToString("yyyy-MM-dd") used for journal page titles and the /api/journals/{date} route. */
export function toJournalDateString(date: Date): string {
  return date.toISOString().slice(0, 10);
}

export function isToday(date: Date): boolean {
  return toJournalDateString(date) === toJournalDateString(new Date());
}
