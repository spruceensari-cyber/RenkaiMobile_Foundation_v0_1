using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Renkai.Combat;

namespace RenkaiMobile.UI
{
    public sealed class KillFeedController : MonoBehaviour
    {
        [SerializeField] private RectTransform feedRoot;
        [SerializeField] private float entryLifetime = 4.5f;
        private readonly Dictionary<Health, string> names = new Dictionary<Health, string>();

        private void Start()
        {
            Health[] all = FindObjectsByType<Health>(FindObjectsSortMode.None);
            foreach (Health health in all)
            {
                if (health == null) continue;
                names[health] = health.gameObject.name;
                health.Died += damage => OnDied(health, damage);
            }
        }

        private void OnDied(Health victim, DamageInfo damage)
        {
            string victimName = names.TryGetValue(victim, out string value) ? value : victim.name;
            string attackerName = damage.Instigator != null ? damage.Instigator.name : "World";
            AddEntry(attackerName + "  >  " + victimName + (damage.Headshot ? "  HEADSHOT" : ""));
        }

        private void AddEntry(string message)
        {
            if (feedRoot == null) return;
            var go = new GameObject("KillFeedEntry", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(feedRoot, false);
            Text text = go.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 22;
            text.alignment = TextAnchor.MiddleRight;
            text.color = Color.white;
            text.text = message;
            RectTransform rt = text.rectTransform;
            rt.sizeDelta = new Vector2(520f, 36f);
            StartCoroutine(RemoveLater(go));
        }

        private IEnumerator RemoveLater(GameObject go)
        {
            yield return new WaitForSeconds(entryLifetime);
            if (go != null) Destroy(go);
        }
    }
}
