using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GameLogic
{
    public class HttpHeler : Singleton<HttpHeler>
    {
        public async Task<string> SendFileToServer(string toakenUrl, string url, string filepath)
        {
            Debug.Log("SendFileToServer: " + toakenUrl + " | " + url + " | " + filepath);

            try
            {
                SocketsHttpHandler handler = new SocketsHttpHandler();
                handler.SslOptions.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                string token = "";
                using (HttpClient client = new HttpClient(handler))
                {
                    var response = await client.GetAsync(toakenUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        AuthenticationResultData mAuthenticationResultData = await response.Content.ReadFromJsonAsync<AuthenticationResultData>();
                        token = mAuthenticationResultData.token;
                    }
                    else
                    {
                        Debug.LogError($"Url:{url} get faild. ErrorCode:{response.StatusCode}");
                        return response.StatusCode.ToString();
                    }
                }

                SocketsHttpHandler handler2 = new SocketsHttpHandler();
                handler2.SslOptions.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                using (HttpClient client = new HttpClient(handler2))
                {
                    Debug.Log("token: " + token);

                    byte[] bytebuffer = File.ReadAllBytes(filepath);
                    MultipartFormDataContent mMultipartContent = new MultipartFormDataContent();
                    HttpContent mHttpContent = new ByteArrayContent(bytebuffer);
                    mMultipartContent.Add(mHttpContent, "file", "temp.json");

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.ProxyAuthorization = new AuthenticationHeaderValue("Bearer", token);
                    
                    var response = await client.PostAsync(url, mMultipartContent);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Debug.Log("responseBody: " + responseBody);
                        return responseBody;
                    }
                    else
                    {
                        foreach(var v in client.DefaultRequestHeaders)
                        {
                            Debug.Log("Header: " + v.Key + ", " + v.Value);
                        }

                        Debug.LogError($"Url:{url} get faild. ErrorCode:{response.StatusCode}");
                        return response.StatusCode.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("responseBody: " + ex);
                return ex.Message;
            }
        }

    }
}
