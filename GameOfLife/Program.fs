module GameOfLife.Program
open System
open System.Threading

let rows = 15
let columns = 15
let timer = 500

type Status = ``💀`` = 0 | ``😁`` = 1
type RNG = Security.Cryptography.RandomNumberGenerator

let private nextGeneration (grid: Status [,]) =
    grid
    |> Array2D.mapi (fun r c ->
        let aliveNeighbors =
            ((seq { -1 .. 1 }, seq { -1 .. 1 })
            ||> Seq.allPairs
            |> Seq.map (fun (x, y) ->  x + r, y + c)
            |> Seq.filter (fun (x, y) -> x < rows && y < columns && x >= 0 && y >= 0)
            |> Seq.sumBy (fun (x, y) -> int grid.[x, y])) 
            //The cell needs to be subtracted  from its neighbours as it was counted before 
            - (int grid.[r, c])
        function
        // Cell is lonely and dies OR Cell dies due to over population
        | Status.``😁`` when aliveNeighbors < 2 || aliveNeighbors > 3 -> Status.``💀``
        // A new cell is born
        | Status.``💀`` when aliveNeighbors = 3 -> Status.``😁``
        // stays the same
        | unchanged -> unchanged)

let private stringify (grid: Status [,]) =
    grid
    |> Array2D.mapi (fun _ y status -> status |> if y = columns - 1 then sprintf "%A\n" else string)
    |> Seq.cast<string>
    |> String.concat String.Empty

[<EntryPoint>]
let main _ =
    let cts = new CancellationTokenSource()
    Console.CancelKeyPress.Add(fun _ -> cts.Cancel(); Console.WriteLine "\n👋 Ending simulation.")
    //Define our async work - cold
    let work = async {
            // randomly initialize our grid
            let mutable grid =
                Array2D.init rows columns (fun _ _ -> RNG.GetInt32(0, 2) |> enum)
            while true do
                // Displaying the grid
                Console.SetCursorPosition(0, 0)
                grid |> stringify |> Console.Write
                grid <- nextGeneration grid
                do! Async.Sleep timer
        }
    // let's give our console
    // a good scrubbing
    Console.Clear()
    Console.BackgroundColor <- ConsoleColor.Black
    Console.CursorVisible <- false
    //Do The thing
    Async.RunSynchronously(work, cancellationToken = cts.Token)
    0