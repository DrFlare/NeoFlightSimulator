using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Architecture
{
    public class ObjectPool : MonoBehaviour
    {
        #region Fields

        private List<PooledObject> _items;

        #endregion

        #region Serialize Fields

        [SerializeField] private PooledObject template;
        [SerializeField] private uint initCount = 5;

        #endregion
        
        #region Functions

        private void FillPool()
        {
            for (uint i = 0; i < initCount; i++)
            {
                var instance = Instantiate(template, transform);
                instance.HomePool = this;
                Return(instance);
            }
        }

        public PooledObject Get(Transform newParent = null)
        {
            if (_items.Count <= 0) FillPool();
            var instance = _items.First();
            instance.transform.parent = newParent;
            instance.gameObject.SetActive(true);
            return instance;
        }

        public void Return(PooledObject obj)
        {
            obj.transform.parent = transform;
            obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            obj.gameObject.SetActive(false);
            _items.Add(obj);
        }

        #endregion

        #region Unity Functions

        private void Start()
        {
            FillPool();
        }

        #endregion
    }
}