using Elasticsearch.Models;
using Nest;

namespace ElasticSearch
{
    public static class ElasticSearchExtension
    {
        public static void AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
        {
            var baseUrl = configuration["ElasticSettings:baseUrl"];
            var index = configuration["ElasticSettings:defaultIndex"];

            var settings = new ConnectionSettings(new Uri(baseUrl ?? ""))
                .PrettyJson()
                .CertificateFingerprint("b61ad2e270e51e065e63b561f81e68dbff8aaa8443eabee2913038259b2b05db")
                .BasicAuthentication("elastic", "cAElXLzTOu*g8_cgdubV")
                .DefaultIndex(index);
            settings.EnableApiVersioningHeader();
            AddDefaultMappings(settings);

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);

            CreateIndex(client, index);
        }

        private static void AddDefaultMappings(ConnectionSettings settings)
        {
            settings
                .DefaultMappingFor<ArticleModel>(m => m
                    .Ignore(p => p.Link)
                    .Ignore(p => p.AuthorLink)
                );
        }

        private static void CreateIndex(IElasticClient client, string indexName)
        {
            var createIndexResponse = client.Indices.Create(indexName,
                index => index.Map<ArticleModel>(x => x.AutoMap())
            );
        }
    }
}
