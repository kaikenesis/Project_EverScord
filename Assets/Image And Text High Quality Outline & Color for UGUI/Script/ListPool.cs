﻿/*
work by adamman
*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdammanWorkSpace{

  // Object pool to avoid allocations.
  static class ListPool<T>
  {
      private static readonly ObjectPool<List<T>> s_ListPool = new ObjectPool<List<T>>(null, l => l.Clear());

      public static List<T> Get()
      {
          return s_ListPool.Get(); 
      }

      public static void Release(List<T> toRelease)
      {
          s_ListPool.Release(toRelease);
      }
  }
}