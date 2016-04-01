#r "/Users/Nate/_src/ld-utilities/packages/FSharp.Data/lib/net40/FSharp.Data.dll"

open System
open FSharp.Data
open System.Collections.Concurrent

System.Net.ServicePointManager.DefaultConnectionLimit <- System.Int32.MaxValue

type Data =
    {
        StatementNo : int
        StandardNo : int
        DiscoveryToolUri : string
        NiceOrgUri : string
    }

let stack = new ConcurrentStack<Data>()

type Standard =
    {
        StatementNo : int
        StandardNo : int
    }
with

    static member private GetRandomNumbers =
      let statementNumberGenerator = System.Random()
      let standardNumberGenerator = System.Random()
      { StatementNo = statementNumberGenerator.Next(16)
        StandardNo = standardNumberGenerator.Next(120) }

    static member private GenerateUriNiceOrg =
        let vals = Standard.GetRandomNumbers
        match vals with
            | { Standard.StatementNo = statementNo; Standard.StandardNo = standardNo } ->
                (vals,sprintf "https://www.nice.org.uk/guidance/qs%s/chapter/quality-statement-%s" (string standardNo) (string statementNo))

    static member GenerateUriDiscoveryTool (Standard:Standard) =
        Standard |> (fun x ->
                     match x with
                     | { StandardNo = standardNo; StatementNo = statementNo} ->
                           sprintf "https://ld.nice.org.uk/qualitystandards/qs%s/st%s/Statement.html" (string standardNo) (string statementNo))

    static member WriteCsv =
        use sw = new System.IO.StreamWriter("10percent.csv", false)
        sw.WriteLine("Nice Url,Discovery Tool Url, Standard No, Statement No")

    static member Generate10Percent =
        while(dict.Count < 88) do
            async{
                try
                  let standard,uri = Standard.GenerateUriNiceOrg
                  let! data = Http.AsyncRequest(uri, httpMethod = "HEAD")
                  if(data.StatusCode = 200) then
                    (standard,uri) |> ignore
                with
                  | :? System.Net.WebException -> ()
                } |> Async.RunSynchronously
            printfn "%d" dict.Count
