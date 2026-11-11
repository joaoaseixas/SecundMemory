export interface Page {
  id: string;
  workspaceId: string;
  title: string;
  namespace: string | null;
  aliases: string[];
  isJournal: boolean;
  journalDate: string | null;
  createdAtUtc: string;
  lastModifiedAtUtc: string | null;
}

export interface Backlink {
  sourceBlockId: string;
  sourcePageId: string;
  sourcePageTitle: string;
  snippet: string;
}
