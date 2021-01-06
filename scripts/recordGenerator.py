#!/usr/bin/env python3

import sys
import os
import re

class_regex = re.compile(r'public sealed partial class (\w+)')
property_regex = re.compile(r'\[Key\(\d+\)] public (.*?) (\w+) { get; }')


def uncap(s):
    return s[:1].lower() + s[1:] if s else ''


def parse(class_file):
    result = []
    class_name = None
    properties = []

    for line in class_file.readlines():
        if match := class_regex.search(line):
            if class_name:
                result.append({"class_name": class_name, "properties": properties})
            class_name = match.group(1)
            properties = []
        if match := property_regex.search(line):
            properties.append({"type": match.group(1), "name": match.group(2)})
    if class_name:
        result.append({"class_name": class_name, "properties": properties})
    return result


def property_arg(prop, p):
    if prop["name"] == p["name"]:
        return f'        {uncap(p["name"])}'
    else:
        return f'        {p["name"]}'


def generate(out, data):
    class_name = data["class_name"]
    out.write(f'\n  public sealed partial class {class_name}\n')
    out.write('  {\n')
    for prop in data["properties"]:
        out.write(f'    public {class_name} With{prop["name"]}({prop["type"]} {uncap(prop["name"])}) =>\n')
        out.write(f'      new {class_name}(\n')
        out.write(",\n".join([property_arg(prop, p) for p in data["properties"]]))
        out.write(');\n\n')
    out.write('  }\n')


print("Generating Records for " + sys.argv[1])
targets = []
for root, _, files in os.walk(sys.argv[1]):
    for file in files:
        if file.endswith(".cs") and "Generated" not in file:
            parsed = parse(open(os.path.join(root, file)))
            if parsed:
                targets.extend(parsed)

using = [
    "System.Collections.Immutable",
    "Nighthollow.Generated",
    "Nighthollow.Stats2"
]

out_file = open(os.path.join(sys.argv[1], "Generated.cs"), 'w')
out_file.write("// Generated Code - Do not Edit!\n\n")
for use in using:
    out_file.write(f'using {use};\n')
out_file.write("\n#nullable enable\n\n")
out_file.write("namespace Nighthollow.Data\n")
out_file.write("{\n")
for target in targets:
    generate(out_file, target)
out_file.write("}\n")
