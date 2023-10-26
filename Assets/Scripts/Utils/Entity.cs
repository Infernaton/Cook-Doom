using UnityEngine;
using UnityEngine.Events;

namespace Entity
{
    public class LifeFormManager : MonoBehaviour
    {

        [SerializeField] protected float m_HealthBase;
        [SerializeField] protected float m_InvincibiltyTime;
        [SerializeField] protected UnityEvent m_OnDying;

        private float _startInvincibility;
        protected float _currentHealth;

        protected void _startLifeForm()
        {
            _currentHealth = m_HealthBase;
        }

        protected void _updateLifeForm()
        {
            if (_startInvincibility > 0f) _startInvincibility -= Time.deltaTime;
            if (_currentHealth <= 0 && m_OnDying != null) m_OnDying.Invoke();
        }
        public void LoseHP(float damage)
        {
            if (_startInvincibility > 0f) return;
            _currentHealth -= damage;
            _startInvincibility = m_InvincibiltyTime;
        }
    }
}
