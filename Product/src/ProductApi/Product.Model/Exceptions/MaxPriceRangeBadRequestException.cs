namespace ProductApi.Model.Exceptions;

public class MaxPriceRangeBadRequestException : BadRequestException {
    public MaxPriceRangeBadRequestException() :
        base("Max price can't be less than min price.") {
    }
}