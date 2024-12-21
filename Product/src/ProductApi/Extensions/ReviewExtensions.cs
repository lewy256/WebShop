using ProductApi.Entities;
using System.Linq.Expressions;

namespace ProductApi.Extensions;

public static class ReviewExtensions {
    public static IQueryable<Review> SortReviews(this IQueryable<Review> reviews, string? queryString) {
        if(string.IsNullOrWhiteSpace(queryString)) {
            return reviews.OrderBy(r => r.ReviewDate);
        }

        //To order by more than one property, it is necessary to create a composite index.
        var column = queryString.Trim().ToLower().Split(',')[0];
        var direction = column.EndsWith(" desc") ? " desc" : " asc";
        column = column.Replace(direction, "");

        Expression<Func<Review, object>> keySelector = column switch {
            "rating" => review => review.Rating,
            _ => review => review.ReviewDate
        };

        if(direction.Equals(" desc")) {
            return reviews.OrderByDescending(keySelector);
        }
        else {
            return reviews.OrderBy(keySelector);
        }
    }
}
