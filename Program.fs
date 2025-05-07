open Giraffe
open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Http
open System.Text.Json

open Parse
open Fun

type MlInput = { mlInput: string}

type Response = { result: string }

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    builder.Services.AddGiraffe() |> ignore

    let app = builder.Build()
    app.UseStaticFiles() |> ignore

    let parseToBracketHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! input = ctx.BindJsonAsync<MlInput>()

                let parsedResult = print (fromString input.mlInput)

                let response = { result = parsedResult }

                return! json response next ctx
            }

    // app.MapGet("/", Func<string>(fun () -> "Hello World!")) |> ignore
    let webApp =
        choose [
            route "/" >=> htmlFile "./wwwroot/index.html"
            route "/api/parseToBracket" >=> parseToBracketHandler
        ]
    app.UseGiraffe(webApp)

    printfn "%A" Parse.e1

    printfn "%A" Parse.e2

    eval Parse.e1 [] |> printfn "%A"    
    eval Parse.e2 [] |> printfn "%A"

    print Parse.e1 |> printfn "%A"
    print Parse.e2 |> printfn "%A"

    app.Run()

    0 // Exit code

