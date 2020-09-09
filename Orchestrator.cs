using FunctionAppTest.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FunctionAppTest
{
    public static class Orchestrator
    {
        #region Orchestrator Function
        [FunctionName("Orchestrator")]
        public static async Task<List<string>> RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            string inputData = GetInputData(context);
            List<string> outputs = await GetInfoFromActivityFunctions(context, inputData);
            return outputs;
        }
        #endregion

        #region Activity Functions
        [FunctionName("Orchestrator_Hello")]
        public static string SayHello([ActivityTrigger] string input, ILogger log)
        {
            log.LogInformation($"Saying hello to {input}.");
            return $"Hello {input}!";
        }

        [FunctionName("Orchestrator_To_Upper")]
        public static string ToUppper([ActivityTrigger] string input, ILogger log)
        {
            string upper = input.ToUpper();
            log.LogInformation($"To upper {upper}.");
            return upper;
        }

        [FunctionName("Orchestrator_To_Lower")]
        public static string ToLower([ActivityTrigger] string input, ILogger log)
        {
            string lower = input.ToLower();
            log.LogInformation($"To lower {lower}.");
            return lower;
        }
        #endregion

        #region Auxiliar Methods
        private static string GetInputData(IDurableOrchestrationContext context)
        {
            string inputData = "Cristian";
            Input contextInputData = context.GetInput<Input>();
            if (!string.IsNullOrEmpty(contextInputData.Name))
                inputData = contextInputData.Name;
            return inputData;
        }

        private static async Task<List<string>> GetInfoFromActivityFunctions(IDurableOrchestrationContext context, string inputData)
        {
            List<string> outputs = new List<string>();
            outputs.Add(await context.CallActivityAsync<string>("Orchestrator_Hello", inputData));
            outputs.Add(await context.CallActivityAsync<string>("Orchestrator_To_Upper", inputData));
            outputs.Add(await context.CallActivityAsync<string>("Orchestrator_To_Lower", inputData));
            return outputs;
        }
        #endregion
    }
}