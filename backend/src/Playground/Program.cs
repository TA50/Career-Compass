using System.Net;
using System.Net.Mail;

var host = "smtp.gmail.com";
var port = 587;
var username = "awadalbrkal@gmail.com";
var password = "kgfl brzj lgeg wwzp ";

var client = new SmtpClient(host, port);
client.EnableSsl = true;
client.Credentials = new NetworkCredential(username, password);

var mail = new MailMessage
{
    From = new MailAddress(username),
    Subject = "Hello from .NET 6",
    Body = "Hello from .NET 6",
    IsBodyHtml = false
};


mail.To.Add("awadosman997@gmail.com");

client.Send(mail);