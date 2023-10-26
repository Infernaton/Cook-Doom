using UnityEngine;
using UnityEngine.Events;

namespace Entity
{
    public class LifeFormManager : MonoBehaviour
    {
        private float _startInvincibility;

        [SerializeField] protected float m_Health;
        [SerializeField] protected float m_InvincibiltyTime;
        [SerializeField] protected UnityEvent m_OnDying;

        protected void _updateLifeForm()
        {
            if (_startInvincibility > 0f) _startInvincibility -= Time.deltaTime;
            if (m_Health <= 0 && m_OnDying != null) m_OnDying.Invoke();
        }
        public void LoseHP(float damage)
        {
            if (_startInvincibility > 0f) return;
            m_Health -= damage;
            _startInvincibility = m_InvincibiltyTime;
        }
    }
}
