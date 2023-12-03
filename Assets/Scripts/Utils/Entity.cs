using UnityEngine;
using UnityEngine.Events;
using Utils;
using System;
using System.Reflection;
using CustomAttribute;

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
        public float GetCurrentHealth() => _currentHealth;

        protected void _startLifeForm()
        {
            _currentHealth = m_HealthBase;
            _currentMaxHealth = m_HealthBase;
        }

        protected void _updateLifeForm()
        {
            if (_startInvincibility > 0f) _startInvincibility -= Time.deltaTime;
            if ((_currentHealth <= 0 || transform.position.y <= -1 ) && m_OnDying != null && !_isDead)
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

            if (m_InvincibiltyTime > 0f)
                StartCoroutine(Anim.Blink(gameObject, m_InvincibiltyTime));
        }

        public void InstantHeal(float recover)
        {
            UpdateCurrentHealth(_currentHealth + recover);
        }
        public void InstantHealPercent(float recover)
        {
            UpdateCurrentHealth(DMath.AddPercentage(_currentHealth, recover));
        }

        public void UpdateCurrentHealth(float newCurrentHealth)
        {
            _currentHealth = Mathf.Min(newCurrentHealth, _currentMaxHealth);
        }
    }

    public class Modifier : ScriptableObject
    {
        [Range(1, 3), UnDisplayable]
        public int Rarity = 1;

        private string DefineSymbol(dynamic value)
        {
            if (value is float)
                return value + " %";
            if (value is int)
                return "+ " + value;
            return "";
        }

        public override string ToString()
        {
            object a = GetType();
            FieldInfo[] fieldInfos = Type.GetType(a.ToString()).GetFields();

            string s = "";
            foreach ( FieldInfo f in fieldInfos)
            {
                dynamic value = f.GetValue(this);
                if (value is float || value is int)
                {
                    if (value == null || value == 0) continue;
                    if (Attribute.IsDefined(f, typeof(UnDisplayable))) continue;
                    if (s.Length != 0) s += " | ";
                    s += DefineSymbol(value) + " " + Translate.ModifierField(f.Name);
                }
            }
            return s;
        }
    }
}
