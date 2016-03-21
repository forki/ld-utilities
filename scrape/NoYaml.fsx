#r "/Users/Nate/_src/ld-utilities/packages/FSharp.Formatting/lib/net40/FSharp.Markdown.dll"

open FSharp.Markdown
open System
open System.IO

let fs = System.IO.Directory.EnumerateFiles("/Users/Nate/_src/ld-content", "Statement.md", SearchOption.AllDirectories)


let d = """
```
Setting:
    - Test
```
#Hello
"""

let md = Markdown.Parse d

