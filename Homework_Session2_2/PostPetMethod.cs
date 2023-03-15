
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

[assembly: Parallelize(Workers = 10, Scope = ExecutionScope.MethodLevel)]


namespace HomeWork_Session2_2
{
    [TestClass]
    public class PostPetMethod
    {
        private static RestClient restClient;

        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";

        private static readonly string PetEndpoint = "pet";

        private static string GetURL(string endpoint) => $"{BaseURL}{endpoint}";

        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<PetModel> cleanUpList = new List<PetModel>();

        [TestInitialize]
        public void TestInitialize()
        {
            restClient = new RestClient();
        }

        [TestCleanup]
        public async Task TestCleanUp()
        {
            foreach (var data in cleanUpList)
            {
                var restDeleteRequest = new RestRequest(GetURL($"{PetEndpoint}/{data.Id}"));
                var restDeleteResponse = await restClient.DeleteAsync(restDeleteRequest);

            }
        }


        [TestMethod]
        public async Task PostProject()
        {
            #region CreateNewPet
            //Create New Pet
            var newPet = new PetModel()
            {
                Id = 99,
                Name = "Mr.Pickles",
                Category = new PetCategory  { Name = "Poodle" },
                PhotoUrls = new string[] { "photo " },
                Tags = new PetCategory[] { new PetCategory { Name = "Tags" } },
                Status = "available"
            };

            // Send Post Request
            var temp = GetURI(PetEndpoint);
            var postRestRequest = new RestRequest(GetURI(PetEndpoint)).AddJsonBody(newPet);
            var postRestResponse = await restClient.ExecutePostAsync(postRestRequest);
            #endregion

            // Verify Post Request Status Code
            Assert.AreEqual(HttpStatusCode.OK, postRestResponse.StatusCode, "Status Code is not equal to 200");

            // Send Get Request

         
            var getRestRequest = new RestRequest(GetURI($"{PetEndpoint}/{newPet.Id}"), Method.Get);
            var getRestResponse = await restClient.ExecuteAsync<PetModel>(getRestRequest);


            Assert.AreEqual(HttpStatusCode.OK, getRestResponse.StatusCode, "Status code is not equal to 200");
     
            Assert.AreEqual(newPet.Name, getRestResponse.Data.Name, "Name did not Match");
            Assert.AreEqual(newPet.Category.Name, getRestResponse.Data.Category.Name, "Category did not Match");
            Assert.AreEqual(newPet.PhotoUrls[0], getRestResponse.Data.PhotoUrls[0], "PhotoUrls not found");
            Assert.AreEqual(newPet.Tags[0].Name, getRestResponse.Data.Tags[0].Name, "tags not found");


            // CleanUp
            cleanUpList.Add(newPet);
        }
    }
}