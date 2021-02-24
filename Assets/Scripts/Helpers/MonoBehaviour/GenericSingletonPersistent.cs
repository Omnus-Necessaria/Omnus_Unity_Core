public class GenericSingletonPersistent<T> : GenericSingleton<T> where T : GenericSingleton<T>
{
    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
}