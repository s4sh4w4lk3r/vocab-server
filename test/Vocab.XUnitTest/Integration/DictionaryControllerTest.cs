using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;
using Vocab.Application.Validators;
using Vocab.Application.ValueObjects;
using Vocab.Core.Entities;
using Vocab.WebApi;
using Vocab.XUnitTest.Shared;

namespace Vocab.XUnitTest.Integration
{
    [TestCaseOrderer(
    ordererTypeName: "Vocab.XUnitTest.Shared.PriorityOrderer",
    ordererAssemblyName: Constants.ASSEMBLY_NAME)]
    public class DictionaryControllerTest(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        private static readonly string token = """
            
            """;
        private static readonly JsonSerializerOptions jsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        private static long dictionaryId = 0;
        private static readonly string firstName = "test";
        private static readonly string secondName = "test-renamed";

#pragma warning disable IDE0052 // Remove unread private members
        private static bool test1Called = false;
        private static bool test2Called = false;
#pragma warning restore IDE0052 // Remove unread private members

        [Fact, TestPriority(1)]
        public async Task TestInsert()
        {
            test1Called = true;

            using HttpClient httpClient = factory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);

            var response = await httpClient.PostAsync($"dictionaries?name={firstName}", null);
            response.EnsureSuccessStatusCode();

            using Stream contentStream = response.Content.ReadAsStream();
            ResultVocab<StatementDictionary>? resultVocab = await JsonSerializer.DeserializeAsync<ResultVocab<StatementDictionary>>(contentStream, jsonSerializerOptions);

            ArgumentNullException.ThrowIfNull(resultVocab);
            ArgumentNullException.ThrowIfNull(resultVocab.Value);

            var valResult = new StatementDictionaryValidator(false).Validate(resultVocab.Value);
            dictionaryId = resultVocab.Value.Id;
            Assert.True(valResult.IsValid, valResult.ToString());
        }

        [Fact, TestPriority(2)]
        public async Task TestRename()
        {
            test2Called = true;

            using HttpClient httpClient = factory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);

            var response = await httpClient.PatchAsync($"dictionaries/{dictionaryId}?name={secondName}", null);
            response.EnsureSuccessStatusCode();

            using Stream contentStream = response.Content.ReadAsStream();
            ResultVocab? resultVocab = await JsonSerializer.DeserializeAsync<ResultVocab>(contentStream, jsonSerializerOptions);

            ArgumentNullException.ThrowIfNull(resultVocab);

            Assert.True(resultVocab.Success, "Получен False от API.");
            // TODO: сделать методы в апи для получения списка словарей и одного словаря по айди.
        }
    }
}
