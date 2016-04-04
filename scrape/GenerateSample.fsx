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
      { StatementNo = statementNumberGenerator.Next(1,16)
        StandardNo = standardNumberGenerator.Next(1,120) }

    static member private GenerateUriNiceOrg (standard:Standard) =
        match standard with
            | { Standard.StatementNo = statementNo; Standard.StandardNo = standardNo } ->
                sprintf "https://www.nice.org.uk/guidance/qs%s/chapter/quality-statement-%s" (string standardNo) (string statementNo)

    static member GenerateUriDiscoveryTool (standard:Standard) =
        standard |> (fun x ->
                     match x with
                     | { StandardNo = standardNo; StatementNo = statementNo} ->
                           sprintf "https://ld.nice.org.uk/qualitystandards/qs%s/st%s/Statement.html" (string standardNo) (string statementNo))

    static member WriteCsv =
        use sw = new System.IO.StreamWriter("10percent.csv", false)
        sw.WriteLine("Nice Url,Discovery Tool Url, Standard No, Statement No")
        for data in stack do
            match data with
                | { StatementNo = stNo; StandardNo = stdNo; DiscoveryToolUri = dsUri; NiceOrgUri = niceUri} ->
                    sw.WriteLine((niceUri + "," + dsUri + "," + (string stdNo) + "," + (string stNo)))

    static member Generate10Percent =
        while(stack.Count < 88) do
            let standard = Standard.GetRandomNumbers
            let dsUri = Standard.GenerateUriDiscoveryTool standard
            let niceUri =  Standard.GenerateUriNiceOrg standard
            async{
                try
                  let! data = Http.AsyncRequest(niceUri, httpMethod = "HEAD")
                  if(data.StatusCode = 200) then
                    printfn "%A" standard
                    let data =
                        match standard with
                          | { StandardNo = stdNo; StatementNo = stNo } ->
                            { StatementNo = stNo; StandardNo = stdNo; NiceOrgUri = niceUri; DiscoveryToolUri = dsUri}

                    stack.Push(data) |> ignore
                with
                  | :? System.Net.WebException -> ()
                } |> Async.RunSynchronously
            Standard.WriteCsv


Standard.Generate10Percent
