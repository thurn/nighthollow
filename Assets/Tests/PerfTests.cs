// Copyright © 2020-present Derek Thurn

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//    https://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;

namespace Nighthollow.Tests
{
  public sealed class PerfTests
  {
    [Test, Performance]
    public void Vector2_operations()
    {
      var a = Vector2.one;
      var b = Vector2.zero;

      Measure.Method(() =>
        {
          Vector2.MoveTowards(a, b, 0.5f);
          Vector2.ClampMagnitude(a, 0.5f);
          Vector2.Reflect(a, b);
          Vector2.SignedAngle(a, b);
        })
        .MeasurementCount(20)
        .Run();
    }
  }
}