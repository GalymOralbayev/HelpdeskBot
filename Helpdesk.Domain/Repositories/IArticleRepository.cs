using Helpdesk.Domain.Entities;

namespace Helpdesk.Domain.Repositories;

public interface IArticleRepository {
    Task<bool> Any(CancellationToken ct);
    Task InsertRange(IEnumerable<Article> articles, CancellationToken ct);
    Task<Article> GetByName(string articleName, CancellationToken ct);
    Task<List<Article>> SearchByArticleNumber(string articleNumber, CancellationToken ct);
    Task<List<Article>> GetAll(CancellationToken ct);
}