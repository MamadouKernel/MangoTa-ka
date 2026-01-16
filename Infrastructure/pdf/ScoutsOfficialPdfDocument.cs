using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using MangoTaikaDistrict.Domain.Entities;

namespace MangoTaikaDistrict.Infrastructure.Pdf;

public class ScoutsOfficialPdfDocument : IDocument
{
    private readonly ScoutsReportData _data;
    private readonly List<Scout> _scouts;
    private readonly byte[]? _logoBytes;

    public ScoutsOfficialPdfDocument(ScoutsReportData data, List<Scout> scouts, byte[]? logoBytes)
    {
        _data = data;
        _scouts = scouts;
        _logoBytes = logoBytes;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(28);

            page.Header().Element(ComposeHeader);

            page.Content().PaddingTop(12).Column(col =>
            {
                col.Spacing(12);

                col.Item().Element(ComposeSummaryCards);
                col.Item().Element(ComposeStatsTables);

                col.Item().PaddingTop(6).Text("Liste (extrait)").SemiBold().FontSize(12);
                col.Item().Element(ComposeScoutsTable);
            });

            page.Footer().Element(ComposeFooter);
        });
    }

    void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.Spacing(12);

            row.ConstantItem(72).Height(72).Element(c =>
            {
                if (_logoBytes is not null && _logoBytes.Length > 0)
                    c.Image(_logoBytes).FitArea();
                else
                    c.Image(Placeholders.Image(72, 72)).FitArea();
            });

            row.RelativeItem().Column(col =>
            {
                col.Item().Text("MANGO TAIKA DISTRICT").SemiBold().FontSize(18);
                col.Item().Text("Rapport Officiel — Scouts").FontSize(12);
                col.Item().Text($"Généré le: {_data.GeneratedAtUtc:dd/MM/yyyy HH:mm} UTC").FontSize(10).FontColor(Colors.Grey.Darken2);
            });

            row.ConstantItem(160).AlignRight().Column(col =>
            {
                col.Item().Text("Confidentiel").SemiBold().FontColor(Colors.Red.Darken1);
                col.Item().Text("Usage interne").FontSize(10).FontColor(Colors.Grey.Darken2);
            });
        });

        container.PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
    }

    void ComposeSummaryCards(IContainer container)
    {
        container.Row(row =>
        {
            row.Spacing(10);

            row.RelativeItem().Element(c => StatCard(c, "Total scouts", _data.TotalScouts.ToString()));
            row.RelativeItem().Element(c => StatCard(c, "Total groupes", _data.TotalGroupes.ToString()));
            row.RelativeItem().Element(c => StatCard(c, "Top groupe", _data.ByGroupe.OrderByDescending(x => x.Value).FirstOrDefault().Key ?? "-"));
        });
    }

    void StatCard(IContainer c, string label, string value)
    {
        c.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(col =>
        {
            col.Item().Text(label).FontSize(10).FontColor(Colors.Grey.Darken2);
            col.Item().Text(value).SemiBold().FontSize(14);
        });
    }

    void ComposeStatsTables(IContainer container)
    {
        container.Row(row =>
        {
            row.Spacing(10);

            row.RelativeItem().Element(c => KeyValueTable(c, "Répartition par statut", _data.ByStatut));
            row.RelativeItem().Element(c => KeyValueTable(c, "Répartition par sexe", _data.BySexe));
            row.RelativeItem().Element(c => KeyValueTable(c, "Top groupes (10)", _data.ByGroupe
                .OrderByDescending(x => x.Value).Take(10).ToDictionary(x => x.Key, x => x.Value)));
        });
    }

    void KeyValueTable(IContainer container, string title, Dictionary<string, int> data)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(col =>
        {
            col.Item().Text(title).SemiBold().FontSize(11);

            col.Item().PaddingTop(6).Table(t =>
            {
                t.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(5);
                    cols.RelativeColumn(2);
                    cols.RelativeColumn(5);
                });

                // header
                t.Header(h =>
                {
                    h.Cell().Text("Libellé").SemiBold().FontSize(9);
                    h.Cell().AlignRight().Text("Nb").SemiBold().FontSize(9);
                    h.Cell().Text("Visuel").SemiBold().FontSize(9);
                });

                var max = data.Count == 0 ? 1 : Math.Max(1, data.Max(x => x.Value));

                foreach (var kv in data.OrderByDescending(x => x.Value))
                {
                    var label = string.IsNullOrWhiteSpace(kv.Key) ? "N/A" : kv.Key;
                    t.Cell().Text(label).FontSize(9);
                    t.Cell().AlignRight().Text(kv.Value.ToString()).FontSize(9);

                    // mini bar “safe” (sans graphe compliqué)
                    var barCount = (int)Math.Round(18.0 * kv.Value / max);
                    var bar = new string('■', barCount).PadRight(18, '·');
                    t.Cell().Text(bar).FontSize(9).FontColor(Colors.Grey.Darken2);
                }
            });
        });
    }

    void ComposeScoutsTable(IContainer container)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Table(t =>
        {
            t.ColumnsDefinition(cols =>
            {
                cols.RelativeColumn(2); // matricule
                cols.RelativeColumn(3); // nom
                cols.RelativeColumn(3); // prenoms
                cols.RelativeColumn(4); // groupe
                cols.RelativeColumn(2); // statut
            });

            t.Header(h =>
            {
                h.Cell().Text("Matricule").SemiBold();
                h.Cell().Text("Nom").SemiBold();
                h.Cell().Text("Prénoms").SemiBold();
                h.Cell().Text("Groupe").SemiBold();
                h.Cell().Text("Statut").SemiBold();
            });

            foreach (var s in _scouts.Take(200)) // extrait pour éviter PDF trop lourd
            {
                t.Cell().Text(s.Matricule ?? "");
                t.Cell().Text(s.Nom);
                t.Cell().Text(s.Prenoms);
                t.Cell().Text(s.Groupe?.Nom ?? "");
                t.Cell().Text(s.Statut);
            }
        });

        container.PaddingTop(4).Text("NB: La liste complète peut être exportée en Excel.").FontSize(9).FontColor(Colors.Grey.Darken2);
    }

    void ComposeFooter(IContainer container)
    {
        container.PaddingTop(8).Column(col =>
        {
            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

            col.Item().PaddingTop(8).Row(r =>
            {
                r.RelativeItem().Column(c =>
                {
                    c.Item().Text("Signature Gestionnaire").FontSize(10);
                    c.Item().PaddingTop(18).LineHorizontal(1);
                });

                r.ConstantItem(30);

                r.RelativeItem().Column(c =>
                {
                    c.Item().Text("Signature Superviseur").FontSize(10);
                    c.Item().PaddingTop(18).LineHorizontal(1);
                });

                r.ConstantItem(30);

                r.RelativeItem().Column(c =>
                {
                    c.Item().Text("Signature Administrateur").FontSize(10);
                    c.Item().PaddingTop(18).LineHorizontal(1);
                });
            });

            col.Item().AlignCenter().PaddingTop(8).Text(x =>
            {
                x.Span("Page ").FontSize(9).FontColor(Colors.Grey.Darken2);
                x.CurrentPageNumber().FontSize(9).FontColor(Colors.Grey.Darken2);
                x.Span(" / ").FontSize(9).FontColor(Colors.Grey.Darken2);
                x.TotalPages().FontSize(9).FontColor(Colors.Grey.Darken2);
            });
        });
    }
}
