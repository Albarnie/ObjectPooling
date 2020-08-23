using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Albarnie.ObjectPooling
{
    public class ObjectPool
    {
        public GameObject original;

        public List<PooledObject> pool;
        public List<PooledObject> usedPool;

        public bool recycleObjects;

        public ObjectPool(int size, GameObject original, bool recycleObjects = false, bool populate = false)
        {
            this.original = original;

            pool = new List<PooledObject>(size);
            usedPool = new List<PooledObject>(size);

            this.recycleObjects = recycleObjects;
            if (populate)
            {
                for (int i = 0; i < size; i++)
                {
                    PooledObject pooledObject = AddObject(Vector3.zero, Quaternion.identity, null);
                }
            }
        }

        public GameObject CreateObject (Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject pooledObject;

            if (pool.Count > 0)
            {
                pooledObject = pool[0].gameObject;
                //Setup transform
                pooledObject.transform.parent = parent;
                pooledObject.transform.position = position;
                pooledObject.transform.rotation = rotation;

                RemoveFromPool(pooledObject);
            }
            //Recycle an object and try again
            else if (recycleObjects)
            {
                AddToPool(usedPool[0]);
                pooledObject = CreateObject(position, rotation, parent);
            }
            else
            {
                pooledObject = AddObject(position, rotation, parent).gameObject;
                RemoveFromPool(pooledObject);
            }
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
            usedPool.Remove(objectToAdd);
            objectToAdd.gameObject.SetActive(false);
        }

        //Instantiate an object and add it too the pool
        PooledObject AddObject (Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject pooledObject;
            PooledObject poolable;

            //Instantiate the object
            pooledObject = GameObject.Instantiate(original, position, rotation, parent);

            //If the object does not already have a PooledObject component, add one
            if (!pooledObject.TryGetComponent<PooledObject>(out poolable))
            {
                poolable = pooledObject.AddComponent<PooledObject>();
            }

            //Name the object
            pooledObject.name = original.name + string.Format(" ({0})", usedPool.Count + pool.Count);

            poolable.pool = this;
            AddToPool(poolable);

            return poolable;
        }

        public void RemoveFromPool(PooledObject pooledObject)
        {
            pool.Remove(pooledObject);
            usedPool.Add(pooledObject);

            pooledObject.pool = this;

            pooledObject.gameObject.SetActive(true);
        }

        public void RemoveFromPool(GameObject objectToRemove)
        {
            PooledObject pooledObject = objectToRemove.GetComponent<PooledObject>();
            RemoveFromPool(pooledObject);
        }
    }
}
