using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace PersonalPackage.ObjectPooling
{
    public class FlyweightFactory : MonoBehaviour // Todo: Make the spawned objects be under a parent game object, to keep the hierarchy clean
    {
        [SerializeField] private bool collectionCheck = true;
        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxPoolSize = 100;

        private static FlyweightFactory s_instance;
        private static GameObject s_parentGameObject;

        private readonly Dictionary<FlyweightType, IObjectPool<Flyweight>> pools = new();
        private readonly Dictionary<FlyweightType, GameObject> typeParents = new();

        private void Awake()
        {
            if (s_instance == null)
            {
                s_instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);

            SetupParentGameObject();
        }

        private static void SetupParentGameObject()
        {
            s_parentGameObject = new GameObject("FlyweightFactory_PooledObjects");
        }

        public static Flyweight Spawn(FlyweightSettings settings)
        {
            return s_instance.GetPoolFor(settings)?.Get();
        }

        public static void ReturnToPool(Flyweight flyweight)
        {
            s_instance.GetPoolFor(flyweight.settings)?.Release(flyweight);
            GameObject typeParent = s_instance.GetOrCreateTypeParent(flyweight.settings.flyweightType);
            if (flyweight.transform.parent != typeParent.transform) flyweight.transform.parent = typeParent.transform;
        }

        private GameObject GetOrCreateTypeParent(FlyweightType type)
        {
            if (typeParents.TryGetValue(type, out GameObject parent)) return parent;
            parent = new GameObject(type.ToString());
            parent.transform.SetParent(s_parentGameObject.transform);
            typeParents.Add(type, parent);
            return parent;
        }

        private IObjectPool<Flyweight> GetPoolFor(FlyweightSettings settings)
        {
            if (pools.TryGetValue(settings.flyweightType, out IObjectPool<Flyweight> pool)) return pool;
            GetOrCreateTypeParent(settings.flyweightType);
            pool = new ObjectPool<Flyweight>(
                settings.Create,
                settings.OnGet,
                settings.OnRelease,
                settings.OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize
            );
            pools.Add(settings.flyweightType, pool);
            return pool;
        }
    }
}
