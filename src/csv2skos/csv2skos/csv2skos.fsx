#I "../../../packages"
#r "../../../packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#I "../../../packages/FSharp.RDF/lib/"
#r "../../../packages/FSharp.RDF/lib/FSharp.RDF.dll"
#r "../../../packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#r "../../../packages/VDS.Common/lib/net40-client/VDS.Common.dll"
#r "../../../packages/dotNetRDF/lib/net40/dotNetRDF.dll"

open VDS.Common
open FSharp.Data
open FSharp.RDF
open Assertion
open graph
open rdf

let (++) a b = System.IO.Path.Combine(a,b)

let csv = FSharp.Data.CsvFile.Load(__SOURCE_DIRECTORY__ ++ "../csv2skos/sample.csv")

let rec depthTuple c xs =
  match xs with
    | ""::xs -> depthTuple (c+1) xs
    | x::_ -> (c,x)


type context = { parent: int * string
                 previous: int * string
                 grandparents: (int * string) list
                 }

let createOwlResource label parentLabel =
    owl.cls !!("http://ld.nice.org.uk/qualitystandards/conditionanddisease#" + label) [
    !!("http://ld.nice.org.uk/qualitystandards#" + parentLabel)
    ] [
    dataProperty !!"rdfs:label" (label ^^xsd.string)
    ]

let moveParent (context:context) tuples  =
  match context, tuples with
    | _,[] -> context
    | { parent= (parentDepth,_); previous= previous; grandparents=grandparents}, (depth, label)::_ ->
      printfn "d %d" ( depth-parentDepth )
      match depth-parentDepth with
        | 2 -> {context with
                    parent = previous
                    previous = (depth,label)
                    grandparents= context.parent::grandparents}
        | 0 -> { context with
                    parent= List.head grandparents
                    previous = (depth,label)
                    grandparents= List.tail grandparents}
        | _ -> {context with previous = (depth,label)}

let rec owlGen (context:context) tuples =
  match tuples with
  | [] -> []
  | (depth, label)::tail ->
    let context' = moveParent context tuples
    createOwlResource label (snd context'.parent) :: owlGen (moveParent context tuples) tail

let newContext a = {
  parent=a
  previous=a
  grandparents= []
     }


let g = Graph.empty !!"http://test" []
let sb = System.Text.StringBuilder()
csv.Rows
|> Seq.map (fun a -> a.Columns)
|> Seq.map Array.toList
|> Seq.map (depthTuple 1)
|> Seq.toList
|> owlGen (newContext (0, "0-0"))
|> Assert.graph g
|> Graph.writeTtl (toString sb)


//test cases
let context = newContext (0, "0-0")

let context' = moveParent context [(1, "1-1")]
let test1 = context' = {parent=(0, "0-0"); previous=(1, "1-1");grandparents=[]}

let context'' = moveParent context' [(2, "2-1")]
let test2 = context'' = {parent=(1, "1-1"); previous=(2, "2-1");grandparents=[(0, "0-0")]}

let context''' = moveParent context'' [(2, "2-2")]
let test3 = context''' = { parent=(1,"1-1"); previous=(2, "2-2"); grandparents=[(0,"0-0")]}

let context'''' = moveParent context''' [1,("1-2")]
let test4 = context'''' = { parent=(0,"0-0"); previous=(1,"1-2"); grandparents=[]}

