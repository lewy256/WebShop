using ProductApi.Model.Entities;

namespace ProductApi.Interfaces;

public interface IDataShaper<T> {
    IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string fieldsString);
    ShapedEntity ShapeData(T entity, string fieldsString);
}


