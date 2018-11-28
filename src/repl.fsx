#I __SOURCE_DIRECTORY__
#r @"C:\Users\jackc\.nuget\packages\FSharp.Data\3.0.0\lib\netstandard2.0\FSharp.Data.dll"
#r @"C:\Users\jackc\.nuget\packages\naudio\1.9.0-preview2\lib\netstandard2.0\NAudio.dll"
#r @"C:\Users\jackc\.nuget\packages\awssdk.core\3.3.29.12\lib\netstandard1.3\AWSSDK.Core.dll"
#r @"C:\Users\jackc\.nuget\packages\awssdk.polly\3.3.9.4\lib\netstandard1.3\AWSSDK.Polly.dll"
#load @".\Library\Library.fs"

open Library
let combinations = seq { for one in letters do 
                            for two in letters do 
                                // for crow in consonants do
                                    yield (one.Palan + two.Palan)}


    
    // for orow in consonants do 
                            // for vrow in vowels do 
                                // for crow in consonants do
                                    // yield (orow, vrow, crow)}

// seq [|"E";"U";"T";"S";"F";"L"; "-E";"-U";"-T";"-S";"-F";"-L"|]
let a = [|"E";"U";"T";"S";"F";"L"|]
let b =  ((a |> Array.create 20 ) |> Array.concat)
let shuffleR (r : System.Random) xs = xs |> Seq.sortBy (fun _ -> r.Next())
let c = shuffleR (System.Random()) b

let out = combinations
//seq [|"E";"U";"T";"S";"F";"L"; "-E";"-U";"-T";"-S";"-F";"-L"|]
//            |> Seq.map (fun x -> x.Palan) 
            |> Seq.fold (fun acc elem ->  acc + ",\n\"" + elem + "\" : \"" + elem.ToLower() + "{^}{#Return}{^}{-|}\"" ) ""

System.IO.File.WriteAllText(".\\out.json", out)
