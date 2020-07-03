module CliArgumentsScenario

open System
open System.Net.Http

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

// run the following command in command line to test CLI:
// dotnet FSharp.Examples.dll -c config.yaml -i infra_config.yaml
// or
// dotnet FSharp.Examples.dll --config config.yaml --infra infra_config.yaml
let run (args: string[]) =

    let httpClient = new HttpClient()

    let step = Step.create("pull html", fun context -> task {
        let! response = httpClient.GetAsync("https://nbomber.com",
                                            context.CancellationToken)

        match response.IsSuccessStatusCode with
        | true  -> let bodySize = int response.Content.Headers.ContentLength.Value
                   let headersSize = response.Headers.ToString().Length
                   return Response.Ok(sizeBytes = headersSize + bodySize)
        | false -> return Response.Fail()
    })

    let args =
        if args.Length > 0 then args
        else [|"-c"; "config.yaml"; "-i"; "infra_config.yaml"|]
        //else [|"--config"; "config.yaml"; "--infra"; "infra_config.yaml"|]

    Scenario.create "test_nbomber" [step]
    |> Scenario.withLoadSimulations [InjectScenariosPerSec(copiesCount = 150, during = TimeSpan.FromMinutes 1.0)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.runWithArgs args
    |> ignore
