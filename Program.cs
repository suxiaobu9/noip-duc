using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.File(@"./data/duc-.log", rollingInterval: RollingInterval.Day, shared: true)
    .WriteTo.Console()
    .CreateLogger();

int delayMin = 5;

string? username = Environment.GetEnvironmentVariable("NOIP_USERNAME");
string? password = Environment.GetEnvironmentVariable("NOIP_PASSWORD");
string? hostname = Environment.GetEnvironmentVariable("NOIP_HOSTNAME");
string? envDelayMin = Environment.GetEnvironmentVariable("DELAY_MIN");

if (!string.IsNullOrWhiteSpace(envDelayMin))
{
    var success = int.TryParse(envDelayMin, out var min);
    if (success)
        delayMin = min;
}

if (string.IsNullOrWhiteSpace(username) ||
    string.IsNullOrWhiteSpace(password) ||
    string.IsNullOrWhiteSpace(hostname))
{

    Log.Error($"{nameof(username)} and ${nameof(password)} and ${nameof(hostname)} can't be null or empty or whitespace");
    throw new Exception($"{nameof(username)} and ${nameof(password)} and ${nameof(hostname)} can't be null or empty or whitespace");
}

while (true)
{
    // 取得目前的公開IP位址
    string currentIp = await GetPublicIpAddress();

    // 構建更新No-IP域名IP的URL
    string updateUrl = $"https://dynupdate.no-ip.com/nic/update?hostname={hostname}&myip={currentIp}";

    // 建立HttpClient物件
    using HttpClient client = new();

    // 設定基本驗證標頭
    string authHeader = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}"));
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

    // 發送HTTP GET請求以更新IP
    using HttpResponseMessage response = await client.GetAsync(updateUrl);

    // 檢查回應狀態碼
    if (response.IsSuccessStatusCode)
    {
        Log.Information("IP address updated successfully.");
    }
    else
    {
        var content = await response.Content.ReadAsStringAsync();
        Log.Warning("Failed to update IP address. {content}", content);
    }

    await Task.Delay(TimeSpan.FromMinutes(delayMin));
}

static async Task<string> GetPublicIpAddress()
{
    var apiUrls = new string[]
    {
        "https://checkip.amazonaws.com",
        "https://ifconfig.co/ip",
        "https://ipinfo.io/ip"
    };

    using HttpClient client = new();

    foreach (var url in apiUrls)
    {
        HttpResponseMessage response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            continue;

        string ip = await response.Content.ReadAsStringAsync();
        return ip.Trim();
    }

    throw new Exception("Failed to retrieve public IP address.");
}