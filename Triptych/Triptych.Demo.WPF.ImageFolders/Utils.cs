/*
Project: Triptych (http://triptych.codeplex.com)
Filename: Utils.cs
Version: 20140111
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triptych.Demo.WPF.ImageFolders
{
  public static class Utils
  {

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
    {
      T[] elements = source.ToArray();
      for (int i = elements.Length - 1; i >= 0; i--)
      {
        // Swap element "i" with a random earlier element it (or itself)
        // ... except we don't really need to swap it fully, as we can
        // return it immediately, and afterwards it's irrelevant.
        int swapIndex = rng.Next(i + 1);
        yield return elements[swapIndex];
        elements[swapIndex] = elements[i];
      }
    }

    public static int CoerceIndex(int index, int count)
    {
      if (index < 0)
        return count - 1;
      else if (index >= count)
        return 0;
      else
        return index;
    }

    public static string TrimLeft(this string s, string prefix)
    {
      return s.StartsWith(prefix) ? s.Substring(prefix.Length) : s;
    }

  }
}
