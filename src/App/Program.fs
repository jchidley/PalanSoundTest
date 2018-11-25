open Library
// https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/functions/external-functions
open System.Runtime.InteropServices

// https://stackoverflow.com/questions/41797069/f-card-suits-not-displaying-in-console
module Kernel =
    [<DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)>]
    extern bool SetConsoleOutputCP(uint32 wCodePageID)

[<EntryPoint>]
let main argv =
    Kernel.SetConsoleOutputCP 65001u |> ignore

    Library.test()
    0 // return an integer exit code