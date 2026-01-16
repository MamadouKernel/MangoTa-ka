using MangoTaikaDistrict.Domain.Entities;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public interface IContentRepository
{
    // News
    Task<List<Actualite>> GetPublishedNewsAsync();
    Task<List<Actualite>> GetAllNewsAsync();
    Task<Actualite?> GetNewsAsync(Guid id);
    Task AddNewsAsync(Actualite a);

    // Albums / medias
    Task<List<Album>> GetPublicAlbumsAsync();
    Task<List<Album>> GetAllAlbumsAsync();
    Task<Album?> GetAlbumAsync(Guid id);
    Task AddAlbumAsync(Album a);
    Task AddMediaAsync(Media m);

    // Guestbook
    Task<List<LivreOrMessage>> GetGuestbookPublicAsync();
    Task<List<LivreOrMessage>> GetGuestbookAllAsync();
    Task<LivreOrMessage?> GetGuestbookAsync(Guid id);
    Task AddGuestbookAsync(LivreOrMessage m);

    // Livre d'or pages préremplies
    Task<List<LivreOrPage>> GetLivreOrPagesAsync();
    Task<LivreOrPage?> GetLivreOrPageAsync(Guid id);
    Task AddLivreOrPageAsync(LivreOrPage page);
    Task UpdateLivreOrPageAsync(LivreOrPage page);
    Task DeleteLivreOrPageAsync(Guid id);

    Task SaveAsync();
}
