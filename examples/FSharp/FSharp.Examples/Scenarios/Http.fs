﻿module HttpScenario

open System
open System.Net.Http

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.Plugins.Network.Ping
open NBomber.FSharp

let run () =

    // it's a very basic HTTP example, don't use it for production testing
    // for production purposes use NBomber.Http which use performance optimizations
    // you can find more here: https://github.com/PragmaticFlow/NBomber.Http

    use httpClient = new HttpClient()

    let step = Step.create("pull html", fun context -> task {
        let! response = httpClient.GetAsync("https://nbomber.com",
                                            context.CancellationToken)

        match response.IsSuccessStatusCode with
        | true  -> let bodySize = int response.Content.Headers.ContentLength.Value
                   let headersSize = response.Headers.ToString().Length
                   return Response.Ok(sizeBytes = headersSize + bodySize)
        | false -> return Response.Fail()
    })

    let pingPluginConfig = PingPluginConfig.CreateDefault ["nbomber.com"]
    use pingPlugin = new PingPlugin(pingPluginConfig)

    Scenario.create "test_nbomber" [step]
    |> Scenario.withLoadSimulations [
        InjectScenariosPerSec(copiesCount = 150, during = TimeSpan.FromMinutes 1.0)
        //RampScenariosPerSec(copiesCount = 100, during = TimeSpan.FromSeconds 20.0)
        //RampConcurrentScenarios(copiesCount = 100, during = TimeSpan.FromSeconds 20.0)
        //KeepConcurrentScenarios(copiesCount = 100, during = TimeSpan.FromMinutes 1.0)
    ]
    |> NBomberRunner.registerScenario
    //|> NBomberRunner.withApplicationType ApplicationType.Console
    |> NBomberRunner.withPlugins [pingPlugin]
    //|> NBomberRunner.loadInfraConfig "infra_config.json"
    //|> NBomberRunner.loadInfraConfig "infra_config.yaml"
    |> NBomberRunner.run
    |> ignore
