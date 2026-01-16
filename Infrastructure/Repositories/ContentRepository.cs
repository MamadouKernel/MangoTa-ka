using Microsoft.EntityFrameworkCore;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Domain.Enums;
using MangoTaikaDistrict.Infrastructure.Data;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public class ContentRepository : IContentRepository
{
    private readonly AppDbContext _db;
    public ContentRepository(AppDbContext db) => _db = db;

    public Task<List<Actualite>> GetPublishedNewsAsync() =>
        _db.Actualites.Where(x => x.IsPublished)
          .OrderByDescending(x => x.PublishedAt)
          .ToListAsync();

    public Task<List<Actualite>> GetAllNewsAsync() =>
        _db.Actualites.OrderByDescending(x => x.CreatedAt).ToListAsync();

    public Task<Actualite?> GetNewsAsync(Guid id) => _db.Actualites.FirstOrDefaultAsync(x => x.Id == id);
    public Task AddNewsAsync(Actualite a) => _db.Actualites.AddAsync(a).AsTask();

    public Task<List<Album>> GetPublicAlbumsAsync() =>
        _db.Albums.Include(a => a.Medias)
          .Where(a => a.Visibilite == Visibilite.PUBLIC)
          .OrderByDescending(a => a.CreatedAt)
          .ToListAsync();

    public Task<List<Album>> GetAllAlbumsAsync() =>
        _db.Albums.Include(a => a.Medias).OrderByDescending(a => a.CreatedAt).ToListAsync();

    public Task<Album?> GetAlbumAsync(Guid id) =>
        _db.Albums.Include(a => a.Medias).FirstOrDefaultAsync(a => a.Id == id);

    public Task AddAlbumAsync(Album a) => _db.Albums.AddAsync(a).AsTask();
    public Task AddMediaAsync(Media m) => _db.Medias.AddAsync(m).AsTask();

    public Task<List<LivreOrMessage>> GetGuestbookPublicAsync() =>
        _db.LivreOr.Where(x => x.Statut == StatutModeration.VALIDE)
          .OrderByDescending(x => x.CreatedAt)
          .ToListAsync();

    public Task<List<LivreOrMessage>> GetGuestbookAllAsync() =>
        _db.LivreOr.OrderByDescending(x => x.CreatedAt).ToListAsync();

    public Task<LivreOrMessage?> GetGuestbookAsync(Guid id) => _db.LivreOr.FirstOrDefaultAsync(x => x.Id == id);
    public Task AddGuestbookAsync(LivreOrMessage m) => _db.LivreOr.AddAsync(m).AsTask();

    public Task<List<LivreOrPage>> GetLivreOrPagesAsync() =>
        _db.LivreOrPages.Where(p => p.IsActive).OrderBy(p => p.Ordre).ToListAsync();

    public Task<LivreOrPage?> GetLivreOrPageAsync(Guid id) =>
        _db.LivreOrPages.FirstOrDefaultAsync(p => p.Id == id);

    public Task AddLivreOrPageAsync(LivreOrPage page) => _db.LivreOrPages.AddAsync(page).AsTask();

    public Task UpdateLivreOrPageAsync(LivreOrPage page)
    {
        _db.LivreOrPages.Update(page);
        return Task.CompletedTask;
    }

    public async Task DeleteLivreOrPageAsync(Guid id)
    {
        var page = await GetLivreOrPageAsync(id);
        if (page != null)
        {
            _db.LivreOrPages.Remove(page);
        }
    }

    public Task SaveAsync() => _db.SaveChangesAsync();
}
