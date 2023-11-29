using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

namespace Utils
{
    public class Area
    {
        public static Vector3 GetRandomCoord(Bounds b)
        {
            return new Vector3(
                Random.Range(b.min.x, b.max.x),
                Random.Range(b.min.y, b.max.y),
                Random.Range(b.min.z, b.max.z)
            );
        }
        /**
         * Get a random coordinate in a cube
         * Define by its center and its scale
         */
        public static Vector3 GetRandomCoord(Vector3 center, Vector3 scale)
        {
            return GetRandomCoord(new Bounds(center, scale));
        }

        /**
         * Get a random coordinate in a cube except where the player is + its protection radius
         */
        public static Vector3? GetRandomCoord(Vector3 center, Vector3 scale, PlayerController player)
        {
            float protection = player.GetProtectionRadius();
            Bounds b = new(center, scale);
            if (Vector3.Distance(player.transform.position, b.min) < protection && Vector3.Distance(player.transform.position, b.max) < protection)
                return null;

            Vector3 finalPos = GetRandomCoord(b);
            if (Vector3.Distance(player.transform.position, finalPos) < protection)
                return GetRandomCoord(center, scale, player);
            return finalPos;
        }
    }

    public class Compare
    {
        /**
         * Compare if two gameobjects are the same instance
         */
        public static bool GameObjects(GameObject go1, GameObject go2) 
        {
            return go1.GetInstanceID() == go2.GetInstanceID();
        }
    }

    public class Anim
    {
        #region TMP_Text
        public static IEnumerator FadeIn(float t, TMP_Text txt)
        {
            txt.enabled = true;
            txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 0);
            while (txt.color.a < 1.0f)
            {
                txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, txt.color.a + (Time.deltaTime / t));
                yield return null;
            }
        }

        public static IEnumerator FadeOut(float t, TMP_Text txt)
        {
            txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 1);
            while (txt.color.a > 0.0f)
            {
                txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, txt.color.a - (Time.deltaTime / t));
                yield return null;
            }
            txt.enabled = false;
        }
        #endregion

        #region CanvaGroup
        public static IEnumerator FadeIn(float t, CanvasGroup c)
        {
            c.gameObject.SetActive(true);
            c.alpha = 0f;
            while (c.alpha < 1.0f)
            {
                c.alpha += Time.deltaTime / t;
                yield return null;
            }
        }

        public static IEnumerator FadeOut(float t, CanvasGroup c)
        {
            c.alpha = 1f;
            while (c.alpha > 0.0f)
            {
                c.alpha -= Time.deltaTime / t;
                yield return null;
            }
            c.gameObject.SetActive(false);
        }
        #endregion
    }

    public class DMath
    {
        public static float Percentage(float init, float percent)
        {
            return (percent * init / 100);
        }
        public static float AddPercentage(float init, float percent)
        {
            return init + Percentage(init, percent);
        }

        public static int AddPercentage(int init, float percent)
        {
            return (int)(init + Percentage(init, percent));
        }
    }

    public class Translate
    {
        public static string ModifierField(string fieldName)
        {
            return fieldName switch
            {
                "MovementSpeed" => "Mvt Speed",
                "ProtectionRadius" => "Spawn Protection",
                "MaxHealth" => "Max Health",
                "FireRate" => "Fire Rate",
                "Healing" => "♥",
                _ => "",
            };
        }
    }

    public class JsonFile
    {
        public class FinalScoreList
        {
            public List<FinalScore> Scores;
        }
        public static void AddData(string path, FinalScore dataToAdd)
        {
            FinalScoreList storedData = GetStoredData(path);
            storedData.Scores.Add(dataToAdd);
            string savedScore = JsonUtility.ToJson(storedData);
            File.WriteAllText(path, savedScore);
        }

        private static FinalScoreList GetStoredData(string path)
        {
            if (!File.Exists(path)) return new()
            {
                Scores = new List<FinalScore>()
            };

            string text = File.ReadAllText(path);
            return JsonUtility.FromJson<FinalScoreList>(text);
        }
    }
}
