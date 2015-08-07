#r "../../../packages/FSharp.Data/lib/net40/FSharp.Data.dll"
open FSharp.Data


let filepath = "/Users/Nate/MEGA/Development/FSharp/ld-ontologies/skoscsv/Sample Settings vocabulary to go in SKOS format.csv"

let (|E|NE|) s =
  if System.String.IsNullOrEmpty s then E else NE s


let loadCsv = CsvFile.Load(filepath)


let test = Node("Care home", [Node("Residential Care Home", [Node("Nursing Home",[]); Node("Community",[])]); Node("Primary Care", [Node("GP Practise",[]); Node("Minor Injury Unit",[])])])




type Tree<'a> =
  | Empty
  | Node of 'a * list<Tree<'a>>


let def = [
  [Some "1"]
  [None;Some "2"]
  [None;Some "3"]
  [None;None;Some "4"]
  [None;Some "5"]
  ]



let tree xs =
  let depth = List.filter Option.isNone >> List.length
  let cell = List.find Option.isSome >> Option.get
  let rec tree t d = function
    | [] -> []
    | x::xs -> [
      let dep = depth x
      match t, dep, cell x with
      | Empty, 0, x -> yield! Node(x,[])::(tree Empty 0 xs)
      ]
  tree Empty 0 xs

tree def





let tree xs =
  let depth = List.filter Option.isNone >> List.length
  let cell = List.find Option.isSome >> Option.get
  let rec tree t d = function
    | [] -> []
    | x::xs -> [
        let d' = depth x
        match t, d',cell x with
        | Empty,0,x -> yield! Node(x,[])::(tree Empty 0 xs)
        //| Node(c,cx),0,1,x -> yield Node(x,cx @ (tree Empty (d') xs))
        | _,d',_ -> failwithf "wtf %d" d
      ]
  tree Empty 0 xs


tree def

