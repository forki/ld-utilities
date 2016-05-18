#r "../../ld-utilities/packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#r "../../ld-utilities/packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#load "Providers.fsx"

open Providers
open FSharp.Data
open System
open System.IO
open System.Text.RegularExpressions

let writePath = "../../ld-statement-content/qualitystandard"

let (++) x y = Path.Combine(x,y)
let createDirectory path = if(not (Directory.Exists(path))) then Directory.CreateDirectory(path) |> ignore
let writeFile path (data:string[]) =
    File.WriteAllLines(path, data) |> ignore


//Base directory
createDirectory writePath

//hack, sorry!
let returnString (x:JsonProvider<"jsonsamples/ListOfStatements.json", SampleIsList = true >.Result) =
    let string = x.Statements.String
    let array = x.Statements.Array
    if(string.IsSome) then
        string.Value
    else
        let a = array.Value |> Array.head
        a.JsonValue.AsString()


type Standard = {
    Number : string
    Title : string
    Uri : Uri }
with
    member this.Path =
        this.Uri.GetLeftPart(UriPartial.Path)
    static member List = [
        for qStandard in QualityStandardList.Load(listOfQStandards).Results do
          yield { Uri =  Uri(qStandard.Qs); Title = qStandard.QsText;
                  Number = (qStandard.QsSource.Replace("/guidance/", "")) }]


type Statement = { Index : int; Uri : Uri }
with
    member this.Path =
        this.Uri.GetLeftPart(UriPartial.Path)
    static member Crawl (uri : string) =
        let importIOUri = StatementExtractorBuilder uri
        jp.Load(importIOUri).Results
        |> Array.map(fun y -> y.Qs)
    static member List (qs:string) =
        let standardNo = int (qs.TrimStart('q', 's'))
        let mutable index = 1;
        let url =
          match standardNo with
              | x when (x <= 14 && x <> 10 && x <> 9) ->
                  let uriBuilder = ListOfStatementsUrl qs
                  Regex.Replace(uriBuilder,"list-of-quality-statements",
                                              "list-of-statements", RegexOptions.IgnoreCase)
              | _ -> ListOfStatementsUrl qs

        [ for statements in ListOfStatements.Load(url).Results do
            yield { Uri = Uri(returnString(statements)); Index = index }
            index <- index + 1 ]



let filterUnrequired (statementXs : Statement list) =
    statementXs |> List.filter(fun x -> (not(x.Path.Contains("related-nice-quality-standards"))))


//Load standards
for standard in Standard.List do
    let standardPath = writePath ++ (standard.Number + "/")
    createDirectory standardPath
    //Load statements for standard
    let statements = Statement.List standard.Number |> filterUnrequired
    for statement in statements do
        let statementPath = standardPath ++ ("st" + string statement.Index)
        createDirectory statementPath
        let html = Statement.Crawl statement.Path
        writeFile (statementPath ++ "Statement.html") html


Statement.List "qs120"


let standardPath = writePath ++ ("qs120" + "/")
let statements = Statement.List "qs120" |> filterUnrequired
for statement in statements do
  let statementPath = standardPath ++ ("st" + string statement.Index)
  createDirectory statementPath
  let html = Statement.Crawl statement.Path
  writeFile (statementPath ++ "Statement.html") html

(**

//total standards - reasonably sorted
let sorted = Standard.List |> List.sortBy(fun y -> y.Number)
for standard in sorted do
    printfn "\n QS Number: %s \t Title: %s \n" standard.Number standard.Title
printfn "Total: %d" (sorted |> List.length)


//bash diagnostic scripts

//empty files
find . -iname "Statement.html" -empty

//under certain size w/ size
find . -type f -size -500c -exec ls -lh {} \;

//remove all instances of {.title}
find . -name "Statement.md" -exec sed -i -e 's/{.title}//g' {} \;
**)
