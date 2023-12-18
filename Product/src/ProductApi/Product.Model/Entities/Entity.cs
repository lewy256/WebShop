using System.Collections;
using System.Dynamic;

namespace ProductApi.Model.Entities;

public class Entity : DynamicObject, IDictionary<string, object> {
    private readonly string _root = "Entity";
    private readonly IDictionary<string, object> _expando;

    public Entity() {
        _expando = new ExpandoObject();
    }

    public void Add(string key, object value) {
        _expando.Add(key, value);
    }

    public bool ContainsKey(string key) {
        return _expando.ContainsKey(key);
    }

    public ICollection<string> Keys {
        get { return _expando.Keys; }
    }

    public bool Remove(string key) {
        return _expando.Remove(key);
    }

    public bool TryGetValue(string key, out object value) {
        return _expando.TryGetValue(key, out value);
    }

    public ICollection<object> Values {
        get { return _expando.Values; }
    }

    public object this[string key] {
        get {
            return _expando[key];
        }
        set {
            _expando[key] = value;
        }
    }

    public void Add(KeyValuePair<string, object> item) {
        _expando.Add(item);
    }

    public void Clear() {
        _expando.Clear();
    }

    public bool Contains(KeyValuePair<string, object> item) {
        return _expando.Contains(item);
    }

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) {
        _expando.CopyTo(array, arrayIndex);
    }

    public int Count {
        get { return _expando.Count; }
    }

    public bool IsReadOnly {
        get { return _expando.IsReadOnly; }
    }

    public bool Remove(KeyValuePair<string, object> item) {
        return _expando.Remove(item);
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
        return _expando.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}
