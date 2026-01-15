using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using MangoTaikaDistrict.Domain.Entities;

namespace MangoTaikaDistrict.Infrastructure.Pdf;

public class ScoutsPdfDocument : IDocument
{
    private readonly List<Scout> _scouts;

    public ScoutsPdfDocument(List<Scout> scouts)
    {
        _scouts = scouts;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(30);

            page.Header()
                .Text("Rapport Scouts — MangoTaikaDistrict")
                .SemiBold().FontSize(16);

            page.Content().PaddingTop(10).Column(col =>
            {
                col.Item().Text($"Généré le: {DateTime.UtcNow:dd/MM/yyyy HH:mm} UTC").FontSize(10);

                col.Item().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2); // Matricule
                        columns.RelativeColumn(3); // Nom
                        columns.RelativeColumn(3); // Prenoms
                        columns.RelativeColumn(4); // Groupe
                        columns.RelativeColumn(2); // Statut
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Matricule").SemiBold();
                        header.Cell().Text("Nom").SemiBold();
                        header.Cell().Text("Prénoms").SemiBold();
                        header.Cell().Text("Groupe").SemiBold();
                        header.Cell().Text("Statut").SemiBold();
                    });

                    foreach (var s in _scouts)
                    {
                        table.Cell().Text(s.Matricule ?? "");
                        table.Cell().Text(s.Nom);
                        table.Cell().Text(s.Prenoms);
                        table.Cell().Text(s.Groupe?.Nom ?? "");
                        table.Cell().Text(s.Statut);
                    }
                });
            });

            page.Footer()
                .AlignCenter()
                .Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
        });
    }
}
