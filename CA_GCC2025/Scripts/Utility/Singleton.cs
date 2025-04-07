using UnityEngine;

namespace Utility
{
    [DisallowMultipleComponent]
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance = null;
        public static T Instance => _instance;
        
        protected virtual void Awake()
        {
            if (_instance != null)
            {
                Destroy(this);
                return;
            }
            
            _instance = this as T;
        }
        
        protected virtual void OnDestroy()
        {
            if (ReferenceEquals(this, _instance))
            {
                _instance = null;
            }
        }
    }
}
