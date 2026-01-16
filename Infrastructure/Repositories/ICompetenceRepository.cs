using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Domain.Enums;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public interface ICompetenceRepository
{
    Task<List<Competence>> GetAllAsync();
    Task<List<Competence>> GetByTypeAsync(TypeCompetence type);
    Task<Competence?> GetByIdAsync(Guid id);
    Task<List<ScoutCompetence>> GetScoutCompetencesAsync(Guid scoutId);
    Task AddAsync(Competence c);
    Task AddScoutCompetenceAsync(ScoutCompetence sc);
    Task UpdateAsync(Competence c);
    Task DeleteAsync(Competence c);
    Task DeleteScoutCompetenceAsync(Guid id);
    Task SaveAsync();
}
