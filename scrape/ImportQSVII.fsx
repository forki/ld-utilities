#r "/Users/Nate/_src/ld-utilities/packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#load "Gubbins.fsx"

open FSharp.Data
open System.IO
open System
open System.Text.RegularExpressions
open Gubbins
open System.Linq
open System.Text
open System.Collections.Generic

let er = System.Collections.ArrayList()

let logError ex =
  printfn "%A" ex
  er.Add(ex) |> ignore

type Result =
  | Success of string
  | Error of string
  | None

let rQS (url:string) (u:string) =
  let r = Regex.Replace(url, "qs[0-9]{1,2}", u)
  r

let urlReWrite qsNo (chapter:string) =
    let trimChapter = chapter.LastIndexOf("#")
    let noDivPointer = chapter.Substring(0,trimChapter)
    sprintf "https://www.nice.org.uk/guidance/%s/chapter/%s" qsNo noDivPointer


let rUrl (u:string) qsNo =
  let mutable url = ""
  if (u.Contains("#")) then
    url <- urlReWrite qsNo u
  else
    url <- u
  let r = Regex.Replace(QStatement, "guidance(.*?)&", url + "&")
  r


let QSList (qs:string) = [
   for i in QStatements.Load(qs).Results ->
   match i with
   | _ -> string ((i.JsonValue.Item("list_of_quality_statments")))
   ]


let removeEmpty = function
  | @"""<p> </p>""" | @"""<p></p>""" -> None
  | x -> Success(x)


let Href d =
  let a = Regex.Unescape d
  let h = html.Parse(a).Html
  h.Descendants ["a"]
  |> Seq.choose (fun x ->
                 x.TryGetAttribute("href")
                 |> Option.map(fun a -> a.Value()))

let getHref d =
  let h = Href(d)
  h |> Seq.head

let getUrl html qsNo =
  let out = (getHref html)
  (rUrl out qsNo)

let writeToFile (qs:string) i qStandardNo =
    printfn "%s %A %A" qs i qStandardNo
  let qsNum = sprintf "%s%s" (workingDir) (qStandardNo)
  if not (Directory.Exists(workingDir) && Directory.Exists(qsNum)) then
    Directory.CreateDirectory(workingDir) |> ignore
    Directory.CreateDirectory(qsNum) |> ignore
  Directory.CreateDirectory(sprintf "%s/st%i" qsNum i) |> ignore
  let statement = sprintf "%s/st%i/Statement.html" qsNum i
  let title = @"class=""title"""
  let noTitle = qs.Replace(title, "")
  File.WriteAllText(statement, noTitle)
  printfn "Just created st%i qsNo:%s" i qStandardNo


let writeTitle qs titleText =
  let path = sprintf "%s%s/%s" workingDir qs "Title.md"
  File.WriteAllText(path, titleText)
  ()


let QStatements (qs:string) =
  let sNo = int (qs.TrimStart('q', 's'))
  match sNo with
    | x when x > 16 ->
      QSList(rQS listOfQStatements qs) |> Seq.map(fun a -> removeEmpty a)
    | x when (x <= 16) ->
      QSList(Regex.Replace((rQS listOfQStatements qs),"list-of-quality-statements", "list-of-statements",
                           RegexOptions.IgnoreCase))
      |> Seq.map(fun a -> removeEmpty a)
    | _ -> seq { yield Error(sprintf "%s: %s" qs (string sNo))}


let manual qsNo =
  try
    QStatements qsNo
    |> Seq.iteri(fun i id ->
                match id with
                | Success (y) ->
                    let url = getUrl y qsNo
                    QStatementProv.Load(url).Results
                    |> Seq.iter (fun z -> writeToFile (String.Concat(z.Qs)) i qsNo |> ignore)
                | Error ex -> printfn "%A" ex
                | None -> ())
  with
    | :? ArgumentException as ex -> printfn "%A" ex
    | ex -> printfn "%A" ex

let error = System.Collections.Generic.List<string>()

let exec =
    let QStandardList = [for i in Gubbins.allQS.Load(listOfQStandards).Results do yield (i.QsSource.Replace("/guidance/", ""), i.QsText)]
    QStandardList
    |> Seq.iter(fun qs ->
                try
                  let qsNo, Title = qs
                  printfn "No: %s // Title: %s" qsNo Title
                  manual qsNo
                  (writeTitle qsNo Title) |> ignore
                with
                    | ex -> printfn "Error %s" ex.Message)

(**

file - html2md.sh
execute - find . -name "*.html" | xargs -P 5 -n 1 ~/Downloads/html2md.sh
top level qualitystandards dir

#/bin/bash

htmlF="$1"
mdF="${htmlF/html/md}"
echo "$mdF"

curl -L "https://api.cloudconvert.com/convert" \
  -F file=@"$htmlF" \
  -F "apikey=dT1bmP-qVQD4u5BzB4tIa2x0VnQc5MwX9M0jkd2dFOu6ytq97xOaIRK2CaZTLKGUp9VtpjIo1VcYqgsZGsTYVg" \
  -F "input=upload" \
  -F "download=inline" \
  -F "inputformat=html" \
  -F "outputformat=md" \
  -F "converteroptions[output_markdown_syntax]=pandoc" \
    > "$mdF"
**)
