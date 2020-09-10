module GameOfLife.Program
open System
open System.Security.Cryptography
open System.Text
open System.Threading

let rows = 15
let columns = 15
let timeout = 500
let mutable runSimulation = true

type Status = ``💀`` = 0 | ``😁`` = 1

let private nextGeneration (currentGrid: Status [,]) = 
    let nextGeneration = Array2D.zeroCreate rows columns
    // Loop through every cell 
    for row in 1..(rows-2) do
        for column in 1..(columns-2) do
            // find your alive neighbor
            let mutable ``😁Neighbors`` = 0
            for i in -1..1 do
                for j in -1..1 do
                    ``😁Neighbors`` <- ``😁Neighbors`` + int currentGrid.[row+i, column+i]
            let currentCell = currentGrid.[row, column]
            // The cell needs to be subtracted 
            // from its neighbours as it was  
            // counted before 
            ``😁Neighbors`` <- ``😁Neighbors`` - int currentCell
            // Implementing the Rules of Life 
            nextGeneration.[row,column] <-
                match currentCell with
                // Cell is lonely and dies OR Cell dies due to over population
                | Status.``😁`` when ``😁Neighbors`` < 2 || ``😁Neighbors`` > 3 -> Status.``💀``
                // A new cell is born 
                | Status.``💀`` when ``😁Neighbors`` = 3 -> Status.``😁``
                // stays the same
                | _ -> currentCell
    nextGeneration

let private print (future: Status[,]) =
    let sb = StringBuilder()
    for row in 0..(rows-1) do
        for column in 0..(columns-1) do
            future.[row, column] |> string |> sb.Append |> ignore
        sb.AppendLine() |> ignore
    Console.BackgroundColor <- ConsoleColor.Black
    Console.CursorVisible <- false
    Console.SetCursorPosition(0,0)
    sb.ToString() |> Console.Write |> ignore
    Thread.Sleep(timeout)

[<EntryPoint>]
let main _ =
    // randomly initialize our grid
    let mutable grid = Array2D.init rows columns (fun _ _ -> RandomNumberGenerator.GetInt32(0, 2) |> enum<Status>)
    Console.CancelKeyPress.Add(
        fun _ -> 
            runSimulation <- false
            Console.WriteLine("\n👋 Ending simulation."))
    // let's give our console
    // a good scrubbing
    Console.Clear();
    // Displaying the grid 
    while runSimulation do
        print grid
        grid <- nextGeneration(grid)
    0
