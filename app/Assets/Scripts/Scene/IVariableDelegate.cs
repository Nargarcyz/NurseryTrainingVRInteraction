public interface IVariableDelegate
{
    object GetValue(string key);
    void SetValue(string key, object value);

    object GetUserVariable(string key);
    void SetUserVariable(string key, object value);
}