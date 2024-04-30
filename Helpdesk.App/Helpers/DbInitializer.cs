using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Helpdesk.Domain.Entities;
using Helpdesk.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SSO.DataAccess.Portgresql.Context;
using File = System.IO.File;

namespace Helpdesk.App.Helpers;

public class DbInitializer {
    private readonly IPhoneReferenceRepository _phoneReferenceRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly ILogger<DbInitializer> _logger;
    private readonly ApplicationContext _dbContext;
    
    public DbInitializer(
        IPhoneReferenceRepository phoneReferenceRepository, 
        ILogger<DbInitializer> logger, 
        ApplicationContext dbContext, 
        IArticleRepository articleRepository, 
        IRoomRepository roomRepository) {
        
        _phoneReferenceRepository = phoneReferenceRepository;
        _logger = logger;
        _dbContext = dbContext;
        _articleRepository = articleRepository;
        _roomRepository = roomRepository;
    }

    public async Task Init(CancellationToken ct = default) {
        var timer = Stopwatch.StartNew();
        _logger.LogInformation("Database initialization...");

        try {
            var pendingMigration = await this._dbContext.Database
                .GetPendingMigrationsAsync(ct).ConfigureAwait(false);
            if (pendingMigration.Any()) {
                _logger.LogInformation("Apply migrations to database...");
                await _dbContext.Database.MigrateAsync(ct).ConfigureAwait(false);
                _logger.LogInformation("Migrations completed");
            }

            await PhoneReferenceInit(ct);
            await ArticleInit(ct);
            await RoomInit(ct);
            
            _logger.LogInformation("Database initialization succeeded for {TotalSeconds}sec",
                timer.Elapsed.TotalSeconds);
        }
        catch (OperationCanceledException e) {
            _logger.LogInformation("Database initialization was interrupted");
            throw;
        }
        catch (Exception e) {
            _logger.LogError(e, "Database initialization error: {Message}", e.Message);
            throw;
        }
    }

    private async Task PhoneReferenceInit(CancellationToken ct) {
        try {
            var hasPhoneReference = await _phoneReferenceRepository.Any(ct);
            if (hasPhoneReference) return;

            var path = Path.Combine("Resources", "Initializers", "phone_book.json");
            await using var openStream = File.OpenRead(path);
            var phoneReferences = await JsonSerializer.DeserializeAsync<List<PhoneReferenceInitDto>>(openStream, cancellationToken: ct);
            if (phoneReferences is not null) {
                var phoneDbModels = phoneReferences.Select(phoneReference => new PhoneReference(
                    phoneReference.Position,
                    phoneReference.Department,
                    phoneReference.FullName,
                    phoneReference.Internal,
                    phoneReference.City,
                    phoneReference.Email
                ));
                await _phoneReferenceRepository.InsertRange(phoneDbModels, ct);
            }
        }
        catch (Exception e) {
            _logger.LogError(e, "Roles initialization error: {Message}", e.Message);
            throw;
        }
    }

    private async Task ArticleInit(CancellationToken ct) {
        try {
            var hasArticle = await _articleRepository.Any(ct);
            if (hasArticle) return;

            var path = Path.Combine("Resources", "Initializers", "articles.json");
            await using var openStream = File.OpenRead(path);
            var article = await JsonSerializer.DeserializeAsync<List<ArticleInitDto>>(openStream, cancellationToken: ct);
            if (article is not null) {
                var articleDbModels = article.Select(x => new Article(
                    x.Number,
                    x.Name
                ));
                await _articleRepository.InsertRange(articleDbModels, ct);
            }
        }
        catch (Exception e) {
            _logger.LogError(e, "Roles initialization error: {Message}", e.Message);
            throw;
        }
    }
    
    private async Task RoomInit(CancellationToken ct) {
        try {
            var hasRooms = await _roomRepository.Any(ct);
            if (hasRooms) return;

            var path = Path.Combine("Resources", "Initializers", "rooms.json");
            await using var openStream = File.OpenRead(path);
            var rooms = await JsonSerializer.DeserializeAsync<List<RoomInitDto>>(openStream, cancellationToken: ct);
            if (rooms is not null) {
                foreach (var room in rooms) {
                    var article = await _articleRepository.GetByName(room.BlockId, ct);
                    if (article != null) {
                        await _roomRepository.Insert(
                            new Room(room.RoomNumber, article, room.IpAddress, room.MacAddress), ct);
                    }
                }
            }
        }
        catch (Exception e) {
            _logger.LogError(e, "Roles initialization error: {Message}", e.Message);
            throw;
        }
    }
}
public class PhoneReferenceInitDto {
    public string Position { get; set; }
    public string Department { get; set; }
    public string FullName { get; set; }
    public long? Internal { get; set; }
    public long? City { get; set; }
    public string Email { get; set; }
}
public class ArticleInitDto {
    public string Name { get; set; }
    public string Number { get; set; }
}
public class RoomInitDto {
    public string IpAddress { get; set; }
    public string MacAddress { get; set; }
    public string BlockId { get; set; }
    public string RoomNumber { get; set; }
}