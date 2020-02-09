using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectPooling
{
    public class ObjectPool
    {
        public GameObject original;

        public List<PooledObject> pool;


        public ObjectPool(int size, GameObject original)
        {
            original = this.original;
            pool = new List<PooledObject>(size);
        }

        public GameObject CreateObject (Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject pooledObject;
            PooledObject poolable;

            if (pool.Count > 0)
            {
                pooledObject = pool[0].gameObject;
                RemoveFromPool(pooledObject);
            }
            else
            {
                pooledObject = GameObject.Instantiate(original, position, rotation, parent);

                if(!pooledObject.TryGetComponent<PooledObject>(out poolable))
                {
                    poolable = pooledObject.AddComponent<PooledObject>();
                }

                poolable.pool = this;
            }

            //Setup transform
            pooledObject.transform.position = position;
            pooledObject.transform.rotation = rotation;
            pooledObject.transform.parent = parent;
            return pooledObject;
        }

        public GameObject CreateObject (Vector3 position, Quaternion rotation)
        {
            return CreateObject(position, rotation, null);
        }

        public void AddToPool (GameObject objectToAdd)
        {
            AddToPool(objectToAdd.GetComponent<PooledObject>());
        }

        public void AddToPool (PooledObject objectToAdd)
        {
            pool.Add(objectToAdd);
            objectToAdd.gameObject.SetActive(false);
        }

        public void RemoveFromPool(GameObject objectToRemove)
        {
            PooledObject pooledObject = objectToRemove.GetComponent<PooledObject>();
            pool.Remove(pooledObject);

            pooledObject.pool = this;

            objectToRemove.SetActive(true);
        }
    }
}
