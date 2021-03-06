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

using MessagePack;

#nullable enable

namespace Nighthollow.Data
{
  /// <summary>
  /// Stores global information about the user, such as their chosen school & available resources.
  /// </summary>
  [MessagePackObject]
  public sealed partial class UserData
  {
    public UserData(School primarySchool = School.Unknown)
    {
      PrimarySchool = primarySchool;
    }

    /// <summary>
    /// School selected by the user at game start.
    /// </summary>
    [Key(0)] public School PrimarySchool { get; }
  }
}