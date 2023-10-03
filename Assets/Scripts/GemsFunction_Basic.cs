using UnityEngine;

namespace liou
{
    /// <summary>
    /// 寶石拾取系統
    /// </summary>
    public class GemsFunction_Basic : MonoBehaviour
    {
        private GameManager_Basic gameManager;
        private void Start()
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Basic>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                gameManager.AddGems();
                Destroy(gameObject);
            }
        }
    }
}

