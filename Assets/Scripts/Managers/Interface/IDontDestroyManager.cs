public interface IDontDestroyManager
{
    public void DestroyGlobalManagers();
    public T GetSceneData<T>(string _fullPath) where T : class;
}
