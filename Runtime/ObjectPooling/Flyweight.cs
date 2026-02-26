using UnityEngine;

namespace PersonalPackage.ObjectPooling
{
    // From here just need to implement concrete classes that overrides the Flyweight and FlyweightSettings
    // Usage Example: Projectiles : Flyweight 
    // ProjectilesSettings : FlyweightSettings
    public abstract class Flyweight : MonoBehaviour
    {
        public FlyweightSettings settings; // This is the intrinsic state of the Flyweight Pattern
    }
}
