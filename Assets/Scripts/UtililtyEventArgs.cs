
using System;
using UnityEngine;
namespace liou
{
    public class UtililtyEventArgs : MonoBehaviour
    {

    }
    public class FireEventArgs : EventArgs
    {
        public float FireDamage { get; set; }
        public float FireForce { get; set; }
    
        public FireEventArgs (float damageData, float forceData)
        {
            FireDamage = damageData;
            FireForce = forceData;
        }
    }
}
