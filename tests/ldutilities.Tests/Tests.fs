module ldutilities.Tests

open NUnit.Framework

let moveParent parents tuples  =
  match parents, tuples with
    | _,[] -> []
    | [],_ -> []
    | ((parentDepth, _)::_), ((depth, label)::_) ->
      match depth-parentDepth with
        | 1 -> (depth,label)::parents
        | _ -> parents


[<Test>]
let x = Assert.AreEqual(true,false)
