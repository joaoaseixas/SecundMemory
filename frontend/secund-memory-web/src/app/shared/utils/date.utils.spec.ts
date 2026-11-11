import { formatJournalDateLong, isToday, shiftJournalDate, toJournalDateString } from './date.utils';

describe('toJournalDateString', () => {
  it('formats using local date components, not UTC', () => {
    // A plain local-midnight Date for a specific calendar day, regardless of the
    // machine's timezone offset — this is exactly the case toISOString() got wrong.
    const date = new Date(2026, 10, 12);
    expect(toJournalDateString(date)).toBe('2026-11-12');
  });

  it('pads single-digit months and days', () => {
    expect(toJournalDateString(new Date(2026, 0, 5))).toBe('2026-01-05');
  });
});

describe('isToday', () => {
  it('is true for a Date constructed as right now', () => {
    expect(isToday(new Date())).toBe(true);
  });

  it('is false for a date far in the past', () => {
    expect(isToday(new Date(2000, 0, 1))).toBe(false);
  });
});

describe('shiftJournalDate', () => {
  it('adds a day within the same month', () => {
    expect(shiftJournalDate('2026-11-12', 1)).toBe('2026-11-13');
  });

  it('subtracts a day within the same month', () => {
    expect(shiftJournalDate('2026-11-12', -1)).toBe('2026-11-11');
  });

  it('rolls over a month boundary', () => {
    expect(shiftJournalDate('2026-11-30', 1)).toBe('2026-12-01');
  });

  it('rolls over a year boundary', () => {
    expect(shiftJournalDate('2026-12-31', 1)).toBe('2027-01-01');
  });
});

describe('formatJournalDateLong', () => {
  it('includes the full year and day of month', () => {
    const formatted = formatJournalDateLong('2026-11-12');
    expect(formatted).toContain('2026');
    expect(formatted).toContain('12');
  });
});
