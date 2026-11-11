// Combining Diacritical Marks block (U+0300-U+036F), built from numeric code
// points rather than typed literally, to avoid any ambiguity about which
// characters actually end up in this file.
const COMBINING_DIACRITICS = new RegExp(`[${String.fromCharCode(0x0300)}-${String.fromCharCode(0x036f)}]`, 'g');

export function slugify(value: string): string {
  return value
    .toLowerCase()
    .normalize('NFD')
    .replace(COMBINING_DIACRITICS, '') // strip accents left behind by NFD normalization
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/(^-|-$)/g, '');
}

export function isBlank(value: string | null | undefined): boolean {
  return !value || value.trim().length === 0;
}
