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

using System.Reflection;
using UnityEngine;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class PlayerPrefsService
  {
    public abstract class Key
    {
      protected Key(string key)
      {
        Value = key;
      }

      public string Value { get; }
    }

    public sealed class Filter : Key
    {
      public Filter(string tableName, PropertyInfo property) : base($"Filters/{tableName}/{property.Name}")
      {
      }
    }

    sealed class LoadScenario : Key
    {
      public LoadScenario() : base("Scenarios/LoadScenario")
      {
      }
    }

    public static readonly Key LoadScenarioKey = new LoadScenario();

    public bool HasKey(Key pref) => PlayerPrefs.HasKey(pref.Value);

    public void SetInt(Key pref, int value)
    {
      PlayerPrefs.SetInt(pref.Value, value);
    }

    public int GetInt(Key pref) => PlayerPrefs.GetInt(pref.Value);

    public void SetString(Key pref, string value)
    {
      PlayerPrefs.SetString(pref.Value, value);
    }

    public string GetString(Key pref) => PlayerPrefs.GetString(pref.Value);
  }
}