using Common.Helpers;
using UnityEngine;

public class GenericSingleton<T> : CachedMonoBehaviour where T : Component
{
    public static T Instance
    {
        get
        {
            if (instance != null) return instance;

            var objectsOfType = (T[]) FindObjectsOfType(typeof(T));

            if (objectsOfType == null || objectsOfType.Length == 0)
            {
                var obj = new GameObject(typeof(T).Name);
                instance = obj.AddComponent<T>();

#if UNITY_EDITOR
                CreateFolderObject(obj);
#endif

                return instance;
            }

            if (objectsOfType.Length == 1)
            {
                instance = objectsOfType[0];
                return instance;
            }

            Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
            instance = objectsOfType[0];

            return instance;
        }
        protected set => instance = value;
    }

    private static T instance;

    public static bool IsInstanceValid => instance != null;

#if UNITY_EDITOR
    /// <summary>
    /// Creating folder for singleton objects, but only in editor.
    /// </summary>
    /// <param name="obj">Object to fold.</param>
    private static void CreateFolderObject(GameObject obj)
    {
        bool   isPersistent = typeof(T) == typeof(GenericSingletonPersistent<>);
        string folderName   = isPersistent ? "Persistent" : "Singletons";
        var    folder       = GameObject.Find(folderName);
        if (folder == null)
        {
            folder = new GameObject(folderName);
            if (isPersistent)
            {
                DontDestroyOnLoad(folder);
            }
        }

        obj.transform.parent = folder.transform;
    }
#endif

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        else if (instance != this)
        {
            var parent = transform.parent;
            Debug.LogWarning(
                             $"[Singleton] Duplicate singleton of type [{typeof(T).Name}]! Destroying object [{gameObject.name}]" + (parent != null
                                                                                                                                         ? $"with parent [{parent.name}]."
                                                                                                                                         : "."));
            Destroy(this);
        }
    }

    public virtual void OnDestroy()
    {
        instance = null;
    }

    public virtual void OnApplicationQuit()
    {
        instance = null;
    }
}