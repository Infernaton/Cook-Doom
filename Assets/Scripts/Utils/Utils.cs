using System;
using System.Collections;
using UnityEngine;

namespace Utils
{
    public class Area
    {
        /**
         * Get a random coordinate in a cube
         * Define by its center and its scale
         */
        public static Vector3 GetRandomCoord(Vector3 center, Vector3 scale)
        {
            Bounds b = new Bounds(center, scale);
            return new Vector3(
                UnityEngine.Random.Range(b.min.x, b.max.x),
                UnityEngine.Random.Range(b.min.y, b.max.y),
                UnityEngine.Random.Range(b.min.z, b.max.z)
            );
        }
    }

    public class LifeFormManager : MonoBehaviour
    {
        public float m_Health;
        public float m_InvincibiltyTime;
        public float _startInvincibility;
        public bool m_DestroyWhenDie = true;

        protected void _updateLifeForm()
        {
            if (_startInvincibility > 0f) _startInvincibility -= Time.deltaTime;
            if (m_Health <= 0 && m_DestroyWhenDie) Destroy(gameObject);
        }
        public void LoseHP(float damage)
        {
            if (_startInvincibility > 0f) return;
            m_Health -= damage;
            _startInvincibility = m_InvincibiltyTime;
        }
    }
}
