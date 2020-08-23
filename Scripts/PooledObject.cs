using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Albarnie.ObjectPooling
{
    public class PooledObject : MonoBehaviour
    {
        public ObjectPool pool;

        public void Destroy(float time = 0)
        {
            if (time <= Time.deltaTime)
            {
                DestroyImmediate();
            }
            else
            {
                Invoke("DestroyImmediate", time);
            }
        }

        public void DestroyImmediate()
        {
            pool.AddToPool(gameObject);
        }

    }
}
