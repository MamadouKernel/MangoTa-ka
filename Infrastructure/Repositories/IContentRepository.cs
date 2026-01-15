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

    Task SaveAsync();
}
