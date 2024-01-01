using ProductApi.Model.Entities;
using ProductApi.Shared.Model.PriceHistoryDtos;
using System.Linq.Expressions;

namespace ProductApi.Service.Extensions;

public static class PriceHistoryExtensions {
    public static IQueryable<PriceHistory> FilterPricesHistory(this IQueryable<PriceHistory> pricesHistory, PriceHistoryParameters priceHistoryParameters) {
        if(priceHistoryParameters.MinPrice is not null && priceHistoryParameters.MaxPrice is not null) {
            pricesHistory = pricesHistory.Where(r => r.PriceValue >= priceHistoryParameters.MinPrice && r.PriceValue <= priceHistoryParameters.MaxPrice);
        }
        if(priceHistoryParameters.StartDate is not null && priceHistoryParameters.EndDate is not null) {
            pricesHistory = pricesHistory.Where(r => r.StartDate >= priceHistoryParameters.StartDate && r.EndDate <= priceHistoryParameters.EndDate);
        }

        return pricesHistory;
    }

    public static IQueryable<PriceHistory> SortPricesHistory(this IQueryable<PriceHistory> pricesHistory, string? queryString) {
        if(string.IsNullOrWhiteSpace(queryString)) {
            return pricesHistory.OrderBy(e => e.PriceValue);
        }

        //To order by more than one property, it is necessary to create a composite index.
        var column = queryString.Trim().ToLower().Split(',')[0];
        var direction = column.EndsWith(" desc") ? " desc" : " asc";
        column = column.Replace(direction, "");

        Expression<Func<PriceHistory, object>> keySelector = column switch {
            "priceValue" => product => product.PriceValue,
            "startDate" => product => product.StartDate,
            "endDate" => product => product.EndDate,
            _ => product => product.PriceValue
        };

        if(direction.Equals(" desc")) {
            return pricesHistory.OrderByDescending(keySelector);
        }
        else {
            return pricesHistory.OrderBy(keySelector);
        }
    }
}
