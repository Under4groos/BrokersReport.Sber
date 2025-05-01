using AngleSharp;
using AngleSharp.Html.Dom;
using BrokersReport.Sber.Structures;

namespace BrokersReport.Sber
{
    public class ReportParser
    {
        public ReportResult reportResult;

        public string Path
        {
            get; set;
        }

        private const string cellSelectorTotal = @"table.RatingAssets tr:nth-child(5) td:nth-child(3)";

        public async Task<ReportResult> ParseAsync()
        {
            if (reportResult.securities == null)
                reportResult.securities = new List<Securities>();
            reportResult.securities.Clear();
            if (string.IsNullOrEmpty(Path) || !File.Exists(Path))
                throw new FileNotFoundException();


            var context = BrowsingContext.New();

            string source = File.ReadAllText(Path);

            var document = await context.OpenAsync(res => res.Content(source));

            var v = document.Body?.Children.Where(e => e is IHtmlTableElement).ToList();
            if (v.Count > 2)
            {
                var tableChildrens = v[2].Children.First();
                var trList = tableChildrens.Children.Where(p => p.NodeName == "TR").ToList();

                var listSecurities = trList
                    .Where(w => (w.Children.First().ClassName == "l"))
                    .Where(w => (w.Children.Count() > 2 && w.Children[1].ClassName == "c"))
                    .ToArray();
                foreach (var row in listSecurities)
                {
                    var itemSecurities = new Securities();
                    var listItemsS = row.Children.Select(a => $"{a.InnerHtml}").ToArray();
                    for (int i = 0; i < listItemsS.Count(); i++)
                    {
                        string item = listItemsS[i];


                        switch (i)
                        {
                            case 0:
                                itemSecurities.Name = item;
                                break;
                            case 1:
                                itemSecurities.ISIN = item;
                                break;
                            case 2:
                                itemSecurities.MarketPriceName = item;
                                continue;
                            default:
                                if (itemSecurities.Numbers == null)
                                    itemSecurities.Numbers = new List<string>();

                                itemSecurities.Numbers.Add(item);
                                break;
                        }
                    }
                    reportResult.securities.Add(itemSecurities);
                }
            }
            reportResult.TotalPrice = document.QuerySelectorAll(cellSelectorTotal).First().InnerHtml;


            return reportResult;
        }
    }
}
