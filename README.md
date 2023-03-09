# ArFileReader

[![Nuget](https://img.shields.io/nuget/v/HiraokaHyperTools.ArFileReader)](https://www.nuget.org/packages/HiraokaHyperTools.ArFileReader)

This will read ar file format like: `.a` and `.lib`

```cs
var libFileBytes = File.ReadAllBytes(ResolvePath(libFile));

ArFileParser.Parse(libFileBytes)
    .ToList()
    .ForEach(
        it =>
        {
            Console.WriteLine($"{it.FileName} {it.FileSize}");
        }
    );
```

```cs
var arFileData = File.ReadAllBytes(libFile);
var ar = ArFileParser.ReparseAsGnu(arFileData, ArFileParser.Parse(arFileData));
foreach (var arEntry in ar)
{
    if (arEntry.FileName.EndsWith(".obj", StringComparison.InvariantCultureIgnoreCase))
    {
        Span<byte> objData = ArFileParser.ReadData(arFileData, arEntry);

        // ...
    }
}
```
