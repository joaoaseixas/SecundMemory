namespace SecundMemory.Api.Contracts.Pages;

public record CreatePageRequest(string Title, bool IsJournal = false, DateOnly? JournalDate = null);
public record RenamePageRequest(string NewTitle);
