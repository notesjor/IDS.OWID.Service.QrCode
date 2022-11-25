using System.Text;
using Tfres;
using Net.Codecrete.QrCodeGenerator;
using IDS.OWID.Service.QrCode.Code;

namespace IDS.OWID.Service.QrCode
{
  internal class Program
  {
    static void Main(string[] args)
    {
      var server = new Server("*", 34801, (obj) => obj.Response.Send(File.ReadAllText("openapi.json")));
      server.AddEndpoint(HttpMethod.Get, "/svg", GetQrCodeSvg);
      server.AddEndpoint(HttpMethod.Get, "/png", GetQrCodePng);
      while (true)
        Thread.Sleep(25000);
    }
        
    private static void GetQrCodePng(HttpContext obj)
    {
      try
      {
        obj.Response.Send(GetQrCode(obj).ToPng(2, 1), "image/png");
      }
      catch
      {
        obj.Response.Send(System.Net.HttpStatusCode.InternalServerError);
      }
    }

    private static void GetQrCodeSvg(HttpContext obj)
    {
      try
      {
        obj.Response.Send(GetQrCode(obj).ToSvgString(1));
      }
      catch
      {
        obj.Response.Send(System.Net.HttpStatusCode.InternalServerError);
      }
    }

    private static Net.Codecrete.QrCodeGenerator.QrCode GetQrCode(HttpContext obj)
    {
      var data = obj.GetData();

      var domain = data.ContainsKey("d") ? (data["d"] == "i" ? "ids-mannheim.de" : "owid.de") : "owid.de";
      var subdomain = data.ContainsKey("s") ? data["s"] : "www";
      var path = data.ContainsKey("p") ? data["p"] : "";

      var url = $"https://{subdomain}.{domain}/{path}";

      var qr = Net.Codecrete.QrCodeGenerator.QrCode.EncodeText(url, Net.Codecrete.QrCodeGenerator.QrCode.Ecc.Medium);
      return qr;
    }
  }
}