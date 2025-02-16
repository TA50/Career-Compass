var example = "https://www.example.com?test=1";

var uri = new Uri(example);

var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
query["code"] = "123";
var uriBuilder = new UriBuilder(uri)
{
    Query = query.ToString(),
    Port = -1
};

Console.WriteLine(uriBuilder.ToString());