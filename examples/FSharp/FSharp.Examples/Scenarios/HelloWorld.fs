﻿module HelloWorldScenario

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open NBomber.Contracts
open NBomber.FSharp

let run () =

    let step1 = Step.create("step_1", fun context -> task {
        // you can define and execute any logic here,
        // for example: send http request, SQL query etc
        // NBomber will measure how much time it takes to execute your logic

        do! Task.Delay(TimeSpan.FromMilliseconds 200.0)
        return Response.Ok(42) // this value will be passed as response for the next step
    })

    let step2 = Step.create("step_2", fun context -> task {
        do! Task.Delay(TimeSpan.FromMilliseconds 200.0)
        let value = context.GetPreviousStepResponse<int>() // 42
        return Response.Ok()
    })

    Scenario.create "hello_world_scenario" [step1; step2]
    |> Scenario.withWarmUpDuration(TimeSpan.FromSeconds 10.0)
    |> Scenario.withLoadSimulations [
        RampConcurrentScenarios(copiesCount = 10, during = TimeSpan.FromSeconds 20.0)
        KeepConcurrentScenarios(copiesCount = 10, during = TimeSpan.FromMinutes 1.0)
        //RampScenariosPerSec(copiesCount = 10, during = TimeSpan.FromSeconds 20.0)
        //InjectScenariosPerSec(copiesCount = 10, during = TimeSpan.FromMinutes 1.0)
    ]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.run
    |> ignore
