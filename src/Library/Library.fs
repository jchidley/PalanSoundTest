module Library
open System
open NAudio.Wave
open Amazon.Polly
open Amazon.Polly.Model
open FSharp.Data
open Amazon

let [<Literal>] assets = @".\assets"

let [<Literal>] csvIPA = @"Phonetic symbols for English with Unicode numbers.csv"

let  [<Literal>] fileIPA = __SOURCE_DIRECTORY__ + "\\" + csvIPA

let rnd = Random()
// https://stackoverflow.com/questions/33312260/how-can-i-select-a-random-value-from-a-list-using-f

let shuffleR (r : Random) xs = xs |> Seq.sortBy (fun _ -> r.Next())
let randPick (rnd:Random) arr = 
      Seq.item (rnd.Next(Seq.length arr)) arr

type PhonemeData = CsvProvider<fileIPA>

let palanLetters = [|"E";"U";"T";"S";"F";"L"|]
let phonemeData = PhonemeData.Load(fileIPA)

let vowels = phonemeData.Rows 
                |> Seq.filter (fun r ->
                r.Type = "Vowel" && r.Palan.Length > 0 && Array.contains r.Palan palanLetters )
let consonants = phonemeData.Rows 
                |> Seq.filter (fun r -> 
                r.Type = "Consonant" && r.Palan.Length > 0 && Array.contains r.Palan palanLetters)

let letters = phonemeData.Rows 
                |> Seq.filter (fun r -> Array.contains r.Palan palanLetters)

// http://mark-dot-net.blogspot.com/2014/02/fire-and-forget-audio-playback-with.html
let sayAudioFile audF = 
    let audioFile = new AudioFileReader(audF)
    let outputDevice = new WaveOutEvent()

    let onPlaybackStopped (args:StoppedEventArgs ) = 
        outputDevice.Dispose()
        audioFile.Dispose()

    outputDevice.Init audioFile
    outputDevice.PlaybackStopped.Add (onPlaybackStopped)
    outputDevice.Play()

// https://www.phon.ucl.ac.uk/home/wells/phoneticsymbolsforenglish_Unicode.htm

AWSConfigs.AWSProfileName <- "JackChidley"
AWSConfigs.AWSRegion <- "eu-west-2" 
let pc = new AmazonPollyClient()

let phonemeOut() = 
    let out = randPick rnd letters
    let out2 = randPick rnd letters

    out.Palan + out2.Palan, out.IPA + out2.IPA
    // let onset = randPick rnd consonants
    // let vowel = randPick rnd vowels
    // let coda = randPick rnd consonants
    // onset, vowel, coda

let writeOutput out = 
  let fOut = (System.IO.Path.Combine (assets, out + ".mp3"))
  if (System.IO.File.Exists(fOut))
  then 
    ()
    // printfn "File %s already exists" fOut
  else 
    let sreq = new SynthesizeSpeechRequest()
    sreq.TextType <- TextType.Ssml
    let o = """<speak><prosody rate="x-slow"><phoneme alphabet="ipa" ph=" """ +
            out + """ ">""" + 
            out + """</phoneme></prosody></speak>"""
    printfn "%s" o
    sreq.Text <- o
    sreq.OutputFormat <- OutputFormat.Mp3
    sreq.VoiceId <- VoiceId.Emma
    let sres = pc.SynthesizeSpeechAsync(sreq)
    let response = sres.GetAwaiter().GetResult()
    use fileStream = System.IO.File.Create(fOut)
    response.AudioStream.CopyTo(fileStream)
    fileStream.Flush()
    fileStream.Close()
    // printfn "File %s written" fOut

let rec readInput() = 
    let palan, ipa = phonemeOut()
    printfn "%s %s" ipa palan
    writeOutput ipa
    sayAudioFile (System.IO.Path.Combine (assets, ipa + ".mp3"))
    let inStr = (Console.ReadLine())
    let uInStr = inStr.ToUpper()
    if uInStr.Contains ("STOP")
    then 
        printfn "stopping"
        sayAudioFile (System.IO.Path.Combine (assets, "stop" + ".mp3")) 
        System.Threading.Thread.Sleep( 500 )
    else 
        // if uInStr.Contains palan 
        // then sayAudioFile (@".\sounds\" + "jes"+ ".mp3")
        // else sayAudioFile (@".\sounds\" + "nəʊ" + ".mp3")
        System.Threading.Thread.Sleep( 500 )
        readInput()

let test () = 
    printfn "Type what you hear, press enter"
    printfn "%s" System.Environment.CurrentDirectory
    writeOutput "jes"
    writeOutput "nəʊ"
    writeOutput "stop"
    writeOutput "stop naʊ"

    readInput()
