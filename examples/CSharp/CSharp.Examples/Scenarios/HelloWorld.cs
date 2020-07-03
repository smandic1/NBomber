using System;
using System.Threading.Tasks;
using NBomber.Contracts;
using NBomber.CSharp;

namespace CSharp.Examples.Scenarios
{
    class HelloWorldScenario
    {
        public static void Run()
        {
            var step1 = Step.Create("step_1", async context =>
            {
                // you can define and execute any logic here,
                // for example: send http request, SQL query etc
                // NBomber will measure how much time it takes to execute your logic

                await Task.Delay(TimeSpan.FromMilliseconds(200));
                return Response.Ok(42); // this value will be passed as response for the next step
            });

            var step2 = Step.Create("step_2", async context =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(200));
                var value = context.GetPreviousStepResponse<int>(); // 42
                return Response.Ok();
            });

            var scenario = ScenarioBuilder
                .CreateScenario("hello_world_scenario", step1, step2)
                .WithWarmUpDuration(TimeSpan.FromSeconds(10))
                .WithLoadSimulations(new []
                {
                    Simulation.RampConcurrentScenarios(copiesCount: 10, during: TimeSpan.FromSeconds(20)),
                    Simulation.KeepConcurrentScenarios(copiesCount: 10, during: TimeSpan.FromMinutes(1)),
                    // Simulation.RampScenariosPerSec(copiesCount: 10, during: TimeSpan.FromSeconds(20)),
                    // Simulation.InjectScenariosPerSec(copiesCount: 10, during: TimeSpan.FromMinutes(1))
                });

            NBomberRunner
                .RegisterScenarios(scenario)
                .Run();
        }
    }
}
