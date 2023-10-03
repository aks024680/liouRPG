
using UnityEngine;

namespace liou
{
    public class ParticleEffectController : MonoBehaviour
    {
        private ParticleSystem ps;
        private void Start()
        {
            ps = GetComponent<ParticleSystem>();
            float lifeTime = ps.main.duration;
            Destroy(gameObject,lifeTime);
        }
    }

}

