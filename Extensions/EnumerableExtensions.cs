using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace YellowPanda.Extensions
{
    public static class EnumerableExtensions
    {
        public static T RandomWeighted<T>(this IEnumerable<T> list, IEnumerable<float> weights)
        {
            return RandomWeighted(list, weights, Random.Range(0, 1000).ToString());
        }
        public static T RandomWeighted<T>(this IEnumerable<T> list, IEnumerable<float> weights, string seed)
        {
            if (list.Count() != weights.Count()) throw new System.Exception("List and Weights are not the same size");
            if (weights.Sum() == 0) throw new System.Exception("Weights can't be all 0");

            List<T> l = list.ToList();
            List<float> w = weights.ToList();

            float sum = 0;
            for (int i = 0; i < list.Count(); i++)
            {
                sum += w[i];
            }

            float rng01 = Mathf.Abs(seed.GetHashCode()) / (float)int.MaxValue;
            float rng = rng01 * sum;
            int c = 0;
            float sp = 0;
            while (sp < rng)
            {
                sp += w[c];
                c++;
            }
            c--;
            return l[c];
        }

        public static T RandomEntry<T>(this IEnumerable<T> list)
        {
            return list.ToArray()[Random.Range(0, list.Count())];
        }

        public static IEnumerable<T> ShuffleNew<T>(this IEnumerable<T> list)
        {
            List<T> clone = list.ToList();
            return Shuffle(clone);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
        {
            List<T> l = list.ToList();

            int n = list.Count();
            while (n > 1)
            {
                int k = Random.Range(0, n--);
                T temp = l[n];
                l[n] = l[k];
                l[k] = temp;
            }

            return l;
        }

        public static IEnumerable<T> ShuffleWeighted<T>(this IEnumerable<T> list, IEnumerable<float> weights)
        {
            if (list.Count() != weights.Count()) throw new System.Exception("List and Weights are not the same size");

            List<T> l = list.ToList();
            List<float> w = weights.ToList();

            Dictionary<T, float> rngs = new Dictionary<T, float>();
            for (int i = 0; i < list.Count(); i++)
            {
                rngs.Add(l[i], Random.value * w[i]);
            }

            List<T> nList = list.ToList();
            nList.Sort((T a, T b) => (int)((rngs[a] - rngs[b]) * 1000));
            return nList;

        }

    }

}