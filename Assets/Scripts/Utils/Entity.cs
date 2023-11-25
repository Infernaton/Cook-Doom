using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace Entity
{
    public class LifeFormManager : MonoBehaviour
    {

        [SerializeField] protected float m_HealthBase;
        [SerializeField] protected float m_InvincibiltyTime;
        [SerializeField] protected UnityEvent m_OnDying;

        private float _startInvincibility;
        protected float _currentHealth;
        protected float _currentMaxHealth;
        private bool _isDead;

        public float GetMaxHealth() => _currentMaxHealth;

        protected void _startLifeForm()
        {
            _currentHealth = m_HealthBase;
            _currentMaxHealth = m_HealthBase;
        }

        protected void _updateLifeForm()
        {
            if (_startInvincibility > 0f) _startInvincibility -= Time.deltaTime;
            if (_currentHealth <= 0 && m_OnDying != null && !_isDead)
            {
                _isDead = true;
                m_OnDying.Invoke();
            }
        }
        public void LoseHP(float damage)
        {
            if (_startInvincibility > 0f) return;
            _currentHealth -= damage;
            _startInvincibility = m_InvincibiltyTime;
        }

        public void InstantHeal(float recover)
        {
            UpdateCurrentHealth(_currentHealth + recover);
        }
        public void InstantHealPercent(float recover)
        {
            UpdateCurrentHealth(Math.AddPercentage(_currentHealth, recover));
        }

        public void UpdateCurrentHealth(float newCurrentHealth)
        {
            _currentHealth = Mathf.Min(newCurrentHealth, _currentMaxHealth);
        }
    }

    public class Modifier : ScriptableObject
    {
        [Range(1, 3)]
        public int Rarity = 1;
    }
}
