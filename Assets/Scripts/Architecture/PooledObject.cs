using UnityEngine;

namespace Architecture
{
    public class PooledObject : MonoBehaviour
    {
        #region Properties

        public ObjectPool HomePool { get; set; }

        #endregion

        #region Functions

        public void ReturnToPool() => HomePool.Return(this);

        #endregion
    }
}