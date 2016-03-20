#r "/Users/Nate/_src/ld-utilities/packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#r "/Users/Nate/_src/ld-utilities/packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#load "Providers.fsx"

open Providers
open FSharp.Data
open System.Text.RegularExpressions

type QualityStatement = { Uri : string }
with
    static member RetrieveStatementUrls (qs:string) =
        let standardNo = int (qs.TrimStart('q', 's'))
        let url =
          match standardNo with
              | x when (x <= 16) ->
                  let uriBuilder = ListOfStatementsUrl qs
                  let rewrite = Regex.Replace(uriBuilder,"list-of-quality-statements",
                                              "list-of-statements", RegexOptions.IgnoreCase)
                  printfn "%s" rewrite
              | _ -> printfn "%s" (ListOfStatementsUrl qs)
        ()


type QualityStandard = {
    Number : string
    Title : string
    Uri : string }
with
    static member List = [
        for qStandard in QualityStandardList.Load(listOfQStandards).Results do
          yield { Uri =  qStandard.Qs; Title = qStandard.QsText;
                  Number = (qStandard.QsSource.Replace("/guidance/", "")) }]

for i in QualityStandard.List do
    printfn "%A" i


type ImportReport =
    | Success
    | Error of Uri : string



let a = jp.Load(QStatements "qs1")
for i in a.Results do
    printfn "%s" i.Statements
