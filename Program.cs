using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace laba3_po_metodichke
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 7777; 
            string directoryPath = @"C:\Users\Dariya\Pictures\Saved Pictures"; 
            TcpListener listener = new TcpListener(IPAddress.Any, port); 
            listener.Start(); 
            Console.WriteLine($"Сервер запущен на порту {port}");
            while (true) 
            {
                using (TcpClient client = listener.AcceptTcpClient())
                {
                    NetworkStream stream = client.GetStream(); 
                    StreamReader reader = new StreamReader(stream);  
                    StreamWriter writer = new StreamWriter(stream); 
                    writer.AutoFlush = true;
                    string request = reader.ReadLine(); 

                    if (!string.IsNullOrEmpty(request)) 
                    {
                        string[] requestParts = request.Split(' '); 
                        string httpMethod = requestParts[0]; 
                        string url = requestParts[1];

                        if (httpMethod == "GET") 
                        {
                            if (url.EndsWith("/")) 
                            {
                                string[] files = Directory.GetFiles(directoryPath); 

                                StringBuilder html = new StringBuilder();
                                html.Append("HTTP/1.1 200 OK\r\n");
                                html.Append("Content-Type: text/html\r\n\r\n");
                                html.Append("<html><head><meta charset=\"utf-8\"></head><body style=background-color:blue>");
                                html.Append("<h1 align= center style=color:white> Файлы, содержащиеся в каталоге</h1>");
                                html.Append("<ul style=color:white>");

                                foreach (string file in files) 

                                {
                                    string fileName = Path.GetFileName(file);
                                    string fileUrl = $"/{fileName}";
                                    html.Append($"<li><a href=\"{fileUrl}\" style=color:white>{fileName}</a></li>");
                                }

                                html.Append("</ul></body></html>");
                                byte[] buffer = Encoding.Default.GetBytes(html.ToString());
                                writer.Write(html.ToString());
                            }
                            else 
                            {
                                string fileName = Path.GetFileName(url);
                                string filePath = Path.Combine(directoryPath, fileName);

                                if (File.Exists(filePath)) 
                                {
                                    string contentType;
                                    if (fileName.EndsWith(".html"))
                                    {
                                        contentType = "html";
                                    }
                                    else
                                    {
                                        contentType = "text/plain";
                  
                                    }

                                    byte[] buffer = File.ReadAllBytes(filePath);

                                    writer.Write("HTTP/1.1 200 OK\r\n");
                                    writer.Write($"Content-Type: {contentType}\r\n");
                                    writer.Write($"Content-Length: {buffer.Length}\r\n\r\n");
                                    stream.Write(buffer, 0, buffer.Length);
                                }
                                else 
                                {
                                    writer.Write("HTTP/1.1 404 Not Found\r\n\r\n");
                                }
                            }
                        }
                    }

                    client.Close(); 
                }
            }
        }
    }
}
