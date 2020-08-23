using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Albarnie.ObjectPooling
{
    public class PooledParticle : PooledObject
    {
        public ParticleSystem system;
        public float timeOut = 10;

        private void OnEnable()
        {
            system.Play();
            Destroy(timeOut);
        }

        private void OnDisable()
        {
            system.Clear();
        }
    }
}
