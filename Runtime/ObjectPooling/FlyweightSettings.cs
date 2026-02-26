using UnityEngine;

namespace PersonalPackage.ObjectPooling
{
    // From here just need to implement concrete classes that overrides the Flyweight and FlyweightSettings
    // Usage Example: Projectiles : Flyweight 
    // ProjectilesSettings : FlyweightSettings

    public abstract class FlyweightSettings : ScriptableObject
    {
        public FlyweightType flyweightType;
        public GameObject prefab;

        public abstract Flyweight Create();

        public virtual void OnGet(Flyweight flyweight)
        {
            flyweight.gameObject.SetActive(true);
        }

        public virtual void OnRelease(Flyweight flyweight)
        {
            flyweight.gameObject.SetActive(false);
        }


        public virtual void OnDestroyPoolObject(Flyweight flyweight)
        {
            Destroy(flyweight.gameObject);
        }
    }

    public enum FlyweightType
    {
        Type1,
        Type2
    }
}
