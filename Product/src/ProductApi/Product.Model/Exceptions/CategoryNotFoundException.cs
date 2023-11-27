namespace ProductApi.Model.Exceptions;

public class CategoryNotFoundException : NotFoundException {
    public CategoryNotFoundException(int categoryId)
        : base($"The category with id: {categoryId} doesn't exist in the database.") {
    }
}