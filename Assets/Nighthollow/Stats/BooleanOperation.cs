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

using Nighthollow.Data;
using Nighthollow.Generated;
using Nighthollow.Model;

#nullable enable

namespace Nighthollow.Stats
{
  public sealed class BooleanOperation : IOperation
  {
    public BooleanOperation(bool setBoolean)
    {
      SetBoolean = setBoolean;
    }

    public bool SetBoolean { get; }

    public string Describe(string statDescription)
    {
      var split = statDescription.Split('/');
      return SetBoolean ? split[0] : split[1];
    }

    public SerializedOperation Serialize() =>
      new SerializedOperation(SetBoolean.ToString(), SetBoolean ? Operator.SetTrue : Operator.SetFalse);

    public override string ToString() => $"{nameof(SetBoolean)}: {SetBoolean}";
  }
}
